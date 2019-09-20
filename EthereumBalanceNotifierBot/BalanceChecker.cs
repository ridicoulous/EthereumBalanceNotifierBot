using System;
using System.Threading.Tasks;
using EthereumBalanceNotifierBot.Interfaces;
using Nethereum.Web3;

namespace EthereumBalanceNotifierBot
{
    public class AddressChecker : IAddressChecker
    {
        private readonly string _nodeAddress;
        private readonly Web3 _web3;

        public AddressChecker(string endpoint = "https://mainnet.infura.io/v3/f675b22632524184b424833c5f78a7de")
        {
            _nodeAddress = endpoint;
            _web3 = new Web3(_nodeAddress);
        }

        public async Task<decimal> GetBalance(string address)
        {
            var balance = await _web3.Eth.GetBalance.SendRequestAsync(address);
            return Web3.Convert.FromWei(balance);
        }
    }
}
