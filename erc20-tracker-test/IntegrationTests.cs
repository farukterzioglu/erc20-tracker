using System.Numerics;
using System.Threading.Tasks;
using erc20_tracker_test;
using Nethereum.HdWallet;
using Nethereum.StandardTokenEIP20.ContractDefinition;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NUnit.Framework;

namespace Tests
{
    public class IntegrationTests
    {
        string _contractAddress;
        string _privateKey;
        string _receiver;

        Account _account;
        Web3 _web3;

        [OneTimeSetUp]
        public async Task SetupOnce()
        {   
            SetAccount();
            _web3 = new Web3(_account, "http://localhost:7545");
            await DeployContract();
        }

        [Test]
        public void SetAccount() 
        {
            string seed = "hundred couple soup flash wait scorpion night donate manual mean flee ability";
            string pass = "";

            var wallet = new Wallet(seed, pass);

            _account = wallet.GetAccount(0);
            _privateKey = _account.PrivateKey;
            _receiver = wallet.GetAccount(1).Address;
        }

        [Test]
        public async Task DeployContract() 
        {
            var deploymentMessage = new TetherTokenDeployment { TotalSupply = 100000 };
            var deploymentHandler = _web3.Eth.GetContractDeploymentHandler<TetherTokenDeployment>();
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);
            
            _contractAddress = transactionReceipt.ContractAddress;
        }
        

        [Test]
        public async Task Transfer()
        {
            var transferHandler = _web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var transfer = new TransferFunction() { To = _receiver, Value = 100 };
            await transferHandler.SendRequestAndWaitForReceiptAsync(_contractAddress, transfer);

            var balanceHandler = _web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var currentBalance = await balanceHandler.QueryAsync<BigInteger>(_contractAddress, new BalanceOfFunction() { Owner = _receiver });
            
            Assert.AreEqual((BigInteger)100, currentBalance);
        }
    }
}