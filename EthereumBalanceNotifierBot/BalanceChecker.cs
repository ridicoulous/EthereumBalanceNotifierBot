using System;
using System.Threading.Tasks;
using EthereumBalanceNotifierBot.Interfaces;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.Web3;
using Nethereum.Util;
using Nethereum.RPC.Eth.Subscriptions;
using Nethereum.RPC.Reactive.Eth;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using Nethereum.RPC.Reactive.Extensions;
namespace EthereumBalanceNotifierBot
{
    public class AddressChecker : IAddressChecker
    {
        private readonly string _nodeAddress;
        private readonly Web3 _web3;
       // private readonly StreamingWebSocketClient _socket;
        public AddressChecker(string endpoint = "https://mainnet.infura.io/v3/f675b22632524184b424833c5f78a7de")
        {
            _nodeAddress = endpoint;
            _web3 = new Web3(_nodeAddress);  
            //_socket = new StreamingWebSocketClient("wss://mainnet.infura.io/ws");
            //_socket.StartAsync().Wait();

        }

        public async Task<decimal> GetBalance(string address)
        {
            
            var balance = await _web3.Eth.GetBalance.SendRequestAsync(address);
            return Web3.Convert.FromWei(balance);
        }
        //public void Lookup()
        //{            
        //    //var pendingTransactionsSubscription = new EthNewPendingTransactionObservableSubscription(_socket);
        //    var ethGetBalance = new EthGetBalanceObservableHandler(_socket);
        //    var subs = ethGetBalance.GetResponseAsObservable().Subscribe(b =>
        //                    Console.WriteLine("Balance xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx: " + b.Value.ToString()));

         
        //   ethGetBalance.SendRequestAsync("0x3f5ce5fbfe3e9af3971dd833d26ba9b5c936f0be", BlockParameter.CreateLatest()).Wait();
        //}
    }
}
