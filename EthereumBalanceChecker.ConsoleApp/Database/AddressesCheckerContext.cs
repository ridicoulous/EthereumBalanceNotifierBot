using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EthereumBalanceNotifierBot.Database
{
    public class AddressesCheckerContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public AddressesCheckerContext()
        {
            //this.Database.EnsureCreated();
            if(Database.GetPendingMigrations().Any())
                this.Database.Migrate();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=AddressesChecker.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Address>().HasKey(c => new { c.Id, c.UserId });
        }
    }
  
}
