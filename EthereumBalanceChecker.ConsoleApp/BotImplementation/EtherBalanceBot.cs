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
                    return SendQuestion(userId);
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
                Thread.Sleep(1000 * 60);
            }
        }
        private void CheckAllBalances()
        {
            var addresses = GetAddresses();
            foreach (var a in addresses)
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var balace = CheckBalance(a).Result;
                var res = _db.Database.ExecuteSqlCommand($"UPDATE Addresses SET Balance={balace} WHERE Id={a};");
            }
            var notify = GetAddressesToNotify();
            foreach (var n in notify)
            {
                Notify(n);
            }

        }
        private void Notify(Address address)
        {
            Send(new MessageSentEventArgs() { Target = address.UserId.ToString(), Response = new CommandResponse($"Your [address](https://etherscan.io/address/{address.Id}) balance is {address.Balance} ETH", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) });
            _db.Database.ExecuteSqlCommand($"UPDATE Addresses SET IsMessageSended=1 WHERE Id={address.Id};");

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
            bool isSuccess = false;
            if (AddressUtil.IsChecksumAddress(address) && AddressUtil.IsValidEthereumAddressHexFormat(address))
            {
                try
                {
                    lock (dbLocker)
                    {
                        using (var db = new AddressesCheckerContext())
                        {
                            db.Addresses.Add(new Address(address, userId));
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
                Send(new MessageSentEventArgs() { Target = userId.ToString(), Response = new CommandResponse($"Looks like `{address}` is not valid eth address, you have to check it`", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown) });
                return false;

            }
            return isSuccess;
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
