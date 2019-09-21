using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework.Core.Objects;

namespace EthereumBalanceChecker.ConsoleApp.BotImplementation
{
    [TelegramBotModule(Author = "ridicoulous", Name = "BalanceCheckerBotModule", Version = "1.0")]
    public class BalanceCheckerBotModule : TelegramBotModuleBase<EtherBalanceBot>
    {
        //private readonly EtherBalanceBot _wrapper;
        public BalanceCheckerBotModule(EtherBalanceBot wrapper):base(wrapper)
        {
            //_wrapper = wrapper;
        }

        [ChatCommand(Triggers = new[] { "info" }, HelpText = "Gets the source code for this bot")]
        public CommandResponse GetSource(CommandEventArgs args)
        {
            return new CommandResponse("https://github.com/ridicoulous/EthereumBalanceNotifierBot\n" +
                "Donates are greatly appreciated:\n" +
                "`3A1pFjyRu4eeGrZTMXWNp2LyEZbeUDLENN`\n" +
                "`0x6fea7665684584884124c1867d7ec31b56c43373`\n" +
                "Feel free to open the issues", parseMode: ParseMode.Markdown);
        }

        [ChatCommand(Triggers = new[] { "stop" }, HideFromInline = true,  HelpText = "Stop/start receiving")]
        public CommandResponse MyAddress(CommandEventArgs args)
        {
            BotWrapper.ActivateAddress(long.Parse(args.Target));
            var address = BotWrapper.GetAddress(long.Parse(args.Target));
            string active = address.IsNotificationEnabled ? "active" : "not active";
            return new CommandResponse($"Your address `{address.Id}` is `{active}`", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
        [ChatCommand(Triggers = new[] { "balance" }, HideFromInline = true, HelpText = "Get balance")]
        public CommandResponse GetActualBalance(CommandEventArgs args)
        {           
            var address = BotWrapper.GetAddress(long.Parse(args.Target));          
            return new CommandResponse($"Your address `{address.Id}` balance is `{address.Balance}`", parseMode:Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
        [ChatCommand(Triggers = new[] { "details" }, HideFromInline = true,  HelpText = "Address details")]
        public CommandResponse GetAddress(CommandEventArgs args)
        {
            var address=BotWrapper.GetAddress(long.Parse(args.Target));
            if(address!=null)
                return new CommandResponse($"I monitoring now `{address.Id}` for balance lower than `{address.BalanceLowerThan}`",parseMode:Telegram.Bot.Types.Enums.ParseMode.Markdown);
            else
            {
                return new CommandResponse($"Here is empty. Press /start to add address to monitoring");
            }
        }
        [ChatCommand(Triggers = new[] { "all" }, HideFromInline = true, BotAdminOnly = true, HelpText = "Get all addresses")]
        public CommandResponse GetAll(CommandEventArgs args)
        {
            var address = BotWrapper.GetAddresses();
            if (address != null&&address.Any())
                return new CommandResponse($"`{String.Join($"\n",address)}`", parseMode:Telegram.Bot.Types.Enums.ParseMode.Markdown);
            else
            {
                return new CommandResponse($"Here is empty. Press /start to add address to monitoring");
            }
        }
        //[ChatCommand(Triggers = new[] { "a" }, HideFromInline = true, BotAdminOnly = true, HelpText = "", Parameters = new string[] { "address","balance" })]
        //public CommandResponse CatchAddress(CommandEventArgs args)
        //{
        //    if (BotWrapper.AddressUtil.IsValidEthereumAddressHexFormat(args.Parameters) && BotWrapper.AddressUtil.IsChecksumAddress(args.Parameters))
        //    {
        //        var parseParams = args.Parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        //        BotWrapper.AddAddress((long)args.SourceUser.ID, parseParams[0], decimal.Parse(parseParams[1]));
        //        return new CommandResponse($"Address {parseParams[0]} was added to monitoring");

        //    }
        //    return new CommandResponse("Looks like address is not valid. \n Send me eth address in format `0x000 10.42`");

        //}
    }
}
