using EthereumBalanceNotifierBot;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using System.Threading;

namespace EthereumBalanceChecker.ConsoleApp
{
    class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        static void Main(string[] args)
        {

            IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appconfig.json", true, true)
                .Build();
            var admin = config["AdminId"];
            var key = config["BotKey"];
            var bot = new EtherBalanceBot(key, int.Parse(admin), new System.Collections.Generic.List<string>() { "Address to check","Balance lower than"});
            bot.Run();
            _quitEvent.WaitOne();       
          
        }
    }
}
