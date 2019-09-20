using EthereumBalanceNotifierBot;
using System;

namespace EthereumBalanceChecker.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AddressChecker d = new AddressChecker();
            d.Lookup();
            Console.Read();
        }
    }
}
