using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumBalanceNotifierBot.Database
{
    public class Address
    {
        public Address()
        {

        }
        public Address(string address, long userId)
        {
            Id = address;
            UserId = userId;           
        }
        public string Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsNotificationEnabled { get; set; } = true;
        public decimal? BalanceGreaterThan { get; set; } = null;
        public decimal? BalanceLowerThan { get; set; } = null;
        public bool BalanceChanged { get; set; } = false;   
        public bool EveryTx { get; set; } = false;
        public bool IsMessageSended { get; set; } = false;
    }
}
