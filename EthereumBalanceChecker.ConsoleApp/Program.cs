using EthereumBalanceNotifierBot;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace EthereumBalanceChecker.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appconfig.json", true, true)
                .Build();
            var admin = config["AdminId"];
            var key = config["BotKey"];
            var bot = new EtherBalanceBot(key, int.Parse(admin), "Checker");
            bot.Run();
            //AddressChecker d = new AddressChecker();
            //d.Lookup();
            Console.Read();
        }
    }
}
