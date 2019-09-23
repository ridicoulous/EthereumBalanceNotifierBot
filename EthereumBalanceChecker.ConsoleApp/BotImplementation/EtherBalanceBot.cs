using EthereumBalanceNotifierBot;
using EthereumBalanceNotifierBot.Database;
using Microsoft.EntityFrameworkCore;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelegramBotFramework.Core;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Objects;

namespace EthereumBalanceChecker.ConsoleApp
{
    public class EtherBalanceBot : TelegramBotWrapper
    {
        private object dbLocker = new object();
        public AddressUtil AddressUtil = new AddressUtil();
        public AddressChecker Checker = new AddressChecker();
        protected override CommandResponse GetAnswer(long userId, string answer)
        {
            switch (UsersWaitingAnswers[userId].Count)
            {
                case 2:
                    {
                        if (AddAddress(userId, answer))
                        {
                            break;

                        }
                        else
                        {
                            return SendQuestion(userId);
                        }
                    }
                case 1:
                    {
                        if (SetBalance(userId, answer))
                        {
                            break;
                        }
                        else
                        {
                            return SendQuestion(userId);
                        }
                    }
                default:
                    {
                        CheckAllBalances();
                        return SendQuestion(userId);

                    }
            }
            return base.GetAnswer(userId, answer);

        }
        AddressesCheckerContext _db;
        public EtherBalanceBot(string key, int adminId, List<string> questions) : base(key, adminId, null, "BalanceChecker", true, questions)
        {
            _db = new AddressesCheckerContext();
            Task.Run(Schedule);

        }
        public async Task<decimal> CheckBalance(string address)
        {
            return await Checker.GetBalance(address).ConfigureAwait(false);
        }
        private void Schedule()
        {
            while (true)
            {
                CheckAllBalances();
                SendNotifications();
                Thread.Sleep(1000 * 120);
            }
        }
        private void ExecuteSql(string command)
        {
            using (var transaction = _db.Database.BeginTransaction(isolationLevel: System.Data.IsolationLevel.ReadUncommitted))
            {
                var res = _db.Database.ExecuteSqlCommand(command);
                transaction.Commit();
            }
        }
        private void CheckAllBalances()
        {
            var addresses = GetAddresses();
            foreach (var a in addresses)
            {
                // CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var balace = CheckBalance(a).Result;
                ExecuteSql($"UPDATE Addresses SET Balance={decimal.Round(balace, 6)} WHERE Id='{a}';");
            }
           

        }
        private void SendNotifications()
        {
            var notify = GetAddressesToNotify();
            foreach (var n in notify)
            {
                Notify(n);
            }
        }
        private void Notify(Address address)
        {
            Send(new MessageSentEventArgs() { Target = address.UserId.ToString(), Response = new CommandResponse($"Your [address](https://etherscan.io/address/{address.Id}) balance is {address.Balance} ETH", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) });
            ExecuteSql($"UPDATE Addresses SET IsMessageSended=1 WHERE Id='{address.Id}';");

        }
        public void ActivateAddress(long userId)
        {
            lock (dbLocker)
            {
                var address = _db.Addresses.FirstOrDefault(c => c.UserId == userId);
                address.IsNotificationEnabled = !address.IsNotificationEnabled;
                address.IsMessageSended = false;
                _db.Entry(address).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _db.SaveChanges();
            }
        }
        public Address GetAddress(long userId)
        {
            lock (dbLocker)
            {
                var address = _db.Addresses.FirstOrDefault(c => c.UserId == userId);
                return address;
            }
        }
        public List<string> GetAddresses(bool needNotify = false)
        {
            lock (dbLocker)
            {
                var address = _db.Addresses.Select(c => c.Id);
                return address.ToList();
            }
        }
        public List<Address> GetAddressesToNotify()
        {
            lock (dbLocker)
            {
                var address = _db.Addresses.Where(c => c.IsNotificationEnabled && !c.IsMessageSended && c.Balance <= c.BalanceLowerThan);
                return address.ToList();
            }
        }
        public bool AddAddress(long userId, string address)
        {
            try
            {
                bool isSuccess = false;
                if (AddressUtil.IsChecksumAddress(address) || AddressUtil.IsValidEthereumAddressHexFormat(address))
                {
                    lock (dbLocker)
                    {
                        using (var db = new AddressesCheckerContext())
                        {
                            if (db.Addresses.Any(c => c.UserId == userId))
                            {
                                ExecuteSql($"DELETE FROM Addresses WHERE UserId='{userId}'");
                            }
                            var balance = CheckBalance(address).Result;
                            db.Addresses.Add(new Address(address, userId) { Balance = balance });
                            db.SaveChanges();
                            isSuccess = true;
                        }
                    }
                }
                else
                {
                    Send(new MessageSentEventArgs() { Target = userId.ToString(), Response = new CommandResponse($"Looks like `{address}` is not valid eth address, you have to check it`", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) });
                    return false;
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                Send(new MessageSentEventArgs() { Target = userId.ToString(), Response = new CommandResponse($"Exception `{ex.Message}` happened at your request, try again.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) });
                return false;
            }
        }
        public bool SetBalance(long userId, string balance)
        {
            bool isSuccess = false;
            decimal val = 0;
            if (decimal.TryParse(balance, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
            {
                if (val < 0)
                {
                    Send(new MessageSentEventArgs() { Target = userId.ToString(), Response = new CommandResponse($"I think, value must be positive") });
                    return false;
                }
                try
                {
                    lock (dbLocker)
                    {
                        using (var db = new AddressesCheckerContext())
                        {
                            var address = db.Addresses.FirstOrDefault(c => c.UserId == userId);
                            address.BalanceLowerThan = val;
                            _db.Entry(address).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                            db.SaveChanges();
                            isSuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Send(new MessageSentEventArgs() { Target = userId.ToString(), Response = new CommandResponse($"Exception `{ex.Message}` happened at your request, try again.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) });
                    return false;
                }
            }
            else
            {
                Send(new MessageSentEventArgs() { Target = userId.ToString(), Response = new CommandResponse($"Looks like `{balance}` is not valid value for balance, you have to check it`", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) });
                return false;

            }
            return isSuccess;
        }

    }

}
