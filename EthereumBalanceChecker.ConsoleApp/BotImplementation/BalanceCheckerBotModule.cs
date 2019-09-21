using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotFramework.Core.Objects;

namespace EthereumBalanceChecker.ConsoleApp.BotImplementation
{
    [TelegramBotModule(Author = "ridicoulous", Name = "BalanceCheckerBotModule", Version = "1.0")]

    public class BalanceCheckerBotModule : TelegramBotModuleBase
    {
        private readonly EtherBalanceBot _wrapper;
        public BalanceCheckerBotModule(EtherBalanceBot wrapper)
        {
            _wrapper = wrapper;
        }
        [ChatCommand(Triggers = new[] { "start" }, HideFromInline = true, BotAdminOnly = true, HelpText = "Start")]
        public CommandResponse Start(CommandEventArgs args)
        {            
            return new CommandResponse("Send me eth address in format `0x000 10.42`");
        }
        [ChatCommand(Triggers = new[] { "a" }, HideFromInline = true, BotAdminOnly = true, HelpText = "", Parameters = new string[] { "address","balance" })]
        public CommandResponse CatchAddress(CommandEventArgs args)
        {
            if (_wrapper.AddressUtil.IsValidEthereumAddressHexFormat(args.Parameters) && _wrapper.AddressUtil.IsChecksumAddress(args.Parameters))
            {
                var parseParams = args.Parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                _wrapper.AddAddress((long)args.SourceUser.ID, parseParams[0], decimal.Parse(parseParams[1]));
                return new CommandResponse($"Address {parseParams[0]} was added to monitoring");

            }
            return new CommandResponse("Looks like address is not valid. \n Send me eth address in format `0x000 10.42`");

        }
    }
}
