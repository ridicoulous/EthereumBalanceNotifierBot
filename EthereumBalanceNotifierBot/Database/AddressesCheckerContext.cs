using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumBalanceNotifierBot.Database
{   
    public class AddressesCheckerContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
       

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=AddressesChecker.db");
        }
    }

  
}
