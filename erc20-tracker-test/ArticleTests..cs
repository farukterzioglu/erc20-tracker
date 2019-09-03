using System;
using System.Numerics;
using System.Threading.Tasks;
using erc20_tracker_test;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NUnit.Framework;
using Nethereum.HdWallet;
using System.Collections.Generic;

namespace Tests
{
    public class ArticleTests
    {
        string _privateKey;
        Web3 _web3;
        string _contractAddress;


        [OneTimeSetUp]
        public void SetupOnce()
        {   
            string url = "http://localhost:7545";
            string privateKey = "8329f3575f2634cc1b538467ba083cfb7faa85ae106693a8b93ea1e58c14df25";

            var account = new Account(privateKey);
            _web3 = new Web3(account, url);
        }

        [Test]
        public async Task DeployContract() 
        {
            var deploymentMessage = new TokenDeployment { TotalSupply = 10 };
            var deploymentHandler = _web3.Eth.GetContractDeploymentHandler<TokenDeployment>();
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);
            
            _contractAddress = transactionReceipt.ContractAddress;
        }

        [Test]
        public async Task GetBalance() 
        {
            string contractAddress = "0xD267808D8fB2F2D6b02074DAC2F00036D5FcB3EE";
            string ownerAddress = "0x228cD92AA7E3e2B4353597ec0B767B856d03E489";
            string receiverAddress = "0x576077aCC546d475845a4178ECF0B467CC7D7e10";

            var balanceHandler = _web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var ownerBalance = await balanceHandler.QueryAsync<BigInteger>(contractAddress, new BalanceOfFunction() { Owner = ownerAddress });
            Assert.AreEqual(10, (int) ownerBalance);

            var receiverBalance = await balanceHandler.QueryAsync<BigInteger>(contractAddress, new BalanceOfFunction() { Owner = receiverAddress });
            Assert.AreEqual(0, (int) receiverBalance);
        }

        [Test]
        public async Task Transfer()
        {
            string contractAddress = "0xD267808D8fB2F2D6b02074DAC2F00036D5FcB3EE";
            string ownerAddress = "0x228cD92AA7E3e2B4353597ec0B767B856d03E489";
            string receiverAddress = "0x576077aCC546d475845a4178ECF0B467CC7D7e10";

            var transferHandler = _web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var transfer = new TransferFunction() { To = receiverAddress, TokenAmount = 1 };
            await transferHandler.SendRequestAndWaitForReceiptAsync(contractAddress, transfer);

            var balanceHandler = _web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var ownerBalance = await balanceHandler.QueryAsync<BigInteger>(contractAddress, new BalanceOfFunction() { Owner = ownerAddress });
            Assert.AreEqual(9, (int) ownerBalance);

            var receiverBalance = await balanceHandler.QueryAsync<BigInteger>(contractAddress, new BalanceOfFunction() { Owner = receiverAddress });
            Assert.AreEqual(1, (int) receiverBalance);
        }

        [Test]
        public async Task GetEvents()
        {
            string contractAddress = "0xD267808D8fB2F2D6b02074DAC2F00036D5FcB3EE";
            
            var transferEventHandler = _web3.Eth.GetEvent<TransferEventDTO>(contractAddress);
            var filterAllTransferEvents = transferEventHandler.CreateFilterInput();
            var allTransferEvents = await transferEventHandler.GetAllChanges(filterAllTransferEvents);

            foreach (var transferEvent in allTransferEvents)
            {
                Console.WriteLine($"From: {transferEvent.Event.From}, To: {transferEvent.Event.To}, Amount: {transferEvent.Event.Value}");
                Console.WriteLine($"Block: {transferEvent.Log.BlockHash}, Transaction: {transferEvent.Log.TransactionHash}");
            }
        }

        [Test]
        public async Task GetEvents_Filtered()
        {
            string contractAddress = "0xD267808D8fB2F2D6b02074DAC2F00036D5FcB3EE";
            var transferEventHandler = _web3.Eth.GetEvent<TransferEventDTO>(contractAddress);
            
            string ownerAddress = "0x228cD92AA7E3e2B4353597ec0B767B856d03E489";
            string receiverAddress = "0x576077aCC546d475845a4178ECF0B467CC7D7e10";

            var filterAllTransferEvents = transferEventHandler.CreateFilterInput();
            var filterTransferEventsForSender = transferEventHandler.CreateFilterInput( ownerAddress);
            var filterTransferEventsForReceiver = transferEventHandler.CreateFilterInput(null, new []{receiverAddress});
            
            var filterAllTransferBetweenBlock = transferEventHandler.CreateFilterInput(
                    fromBlock: new BlockParameter(0),
                    toBlock: new BlockParameter(10)
                );

            var allTransferEvents = await transferEventHandler.GetAllChanges(filterAllTransferBetweenBlock);

            foreach (var transferEvent in allTransferEvents)
            {
                Console.WriteLine($"From: {transferEvent.Event.From}, To: {transferEvent.Event.To}, Amount: {transferEvent.Event.Value}");
                Console.WriteLine($"Block: {transferEvent.Log.BlockHash}, Transaction: {transferEvent.Log.TransactionHash}");
            }
        }

        [Test]
        public async Task GenerateAddresses()
        {   
            var wallet = new Wallet("carry fix final shield cruel learn pave during habit adapt alter habit", "");

            string[] addressList = new string[5];
            for (int i = 0; i < 5; i++)
            {
                var account = wallet.GetAccount(i);
                addressList[i] = account.Address;
            }

            string contractAddress = "0xD267808D8fB2F2D6b02074DAC2F00036D5FcB3EE";
            var transferEventHandler = _web3.Eth.GetEvent<TransferEventDTO>(contractAddress);
            var filter = transferEventHandler.CreateFilterInput(null, addressList);

            var transferEvents = await transferEventHandler.GetAllChanges(filter);
        }
    }
}