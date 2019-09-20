using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumBalanceNotifierBot.Database
{
    public class Address
    {
        public string Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
