using EthereumBalanceNotifierBot.Database;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotFramework.Core;
using TelegramBotFramework.Core.Interfaces;

namespace EthereumBalanceChecker.ConsoleApp
{
    public class EtherBalanceBot : TelegramBotWrapper
    {
        private object dbLocker = new object();
        public AddressUtil AddressUtil = new AddressUtil();

        public EtherBalanceBot(string key, int adminId, string alias) : base(key, adminId, null, alias)
        {
           
        }
        public void AddAddress(long userId, string address, decimal balance)
        {
            lock (dbLocker)
            {
                using (var db = new AddressesCheckerContext())
                {
                    db.Addresses.Add(new Address(address, userId, balance));
                    db.SaveChanges();
                } 
            }
        }
    }

}
