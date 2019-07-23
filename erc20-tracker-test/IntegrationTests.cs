using System.Threading.Tasks;
using Nethereum.StandardTokenEIP20.ContractDefinition;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NUnit.Framework;

namespace Tests
{
    public class IntegrationTests
    {
        string _contractAddress;
        Account _account;
        Web3 _web3;

        [SetUp]
        public void Setup()
        {
            string url = "http://localhost:7545";
            string privateKey = "a9ac3598209f080e027c60bbee4d83dadb2bb738503c9fd974dc1bb55891ef05";

            _account = new Account(privateKey);
            _web3 = new Web3(_account, url);

            _contractAddress = "0xAbC0ecdaD3c1EFc21a6857F288def37CD5439d80";
        }

        [Test]
        public async Task Transfer()
        {
            var transferHandler = _web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var transfer = new TransferFunction() { To = "0xde769dda17eD7d2178a5646D279382d0CaEC8079", Value = 75 };
            await transferHandler.SendRequestAndWaitForReceiptAsync("0xAbC0ecdaD3c1EFc21a6857F288def37CD5439d80", transfer);
        }
    }
}