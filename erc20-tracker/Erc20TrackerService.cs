using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Nethereum.HdWallet;
using Microsoft.Extensions.Logging;

namespace erc20_tracker
{
    public class Erc20TrackerService : BackgroundService
    {
        private readonly ILogger<Erc20TrackerService> _logger;
        private readonly Settings _settings;
        private readonly Web3 _web3;
        IModel _channel;
        private readonly List<string> _trackedAddresses; 

        public Erc20TrackerService(
            IOptions<Settings> settings, 
            ILogger<Erc20TrackerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = (settings ?? throw new ArgumentNullException(nameof(settings))).Value;
            
            _trackedAddresses = _settings.TrackedAddresses.Select( x=> x.ToLower()).ToList();
            _web3 = new Web3(_settings.NodeUrl);
        }

        private void PopulateAddressesFromHdWallet() 
        {
            var wallet = new Wallet(_settings.Seed, "");

            int addressCount = _settings.HdAddressCount > 0 ? _settings.HdAddressCount : 20;
            for (int i = 0; i < addressCount; i++)
            {
                var address = wallet.GetAccount(i).Address;
                if(_trackedAddresses.Contains(address)) continue;
                _trackedAddresses.Add(address);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(!string.IsNullOrEmpty(_settings.Seed)) PopulateAddressesFromHdWallet();

            if(!string.IsNullOrEmpty(_settings.RabbitMqHostName)) {
                // Initialize rabbitmq ->
                var factory = new ConnectionFactory() { 
                    HostName = _settings.RabbitMqHostName,
                    UserName = _settings.RabbitMqUsername,
                    Password = _settings.RabbitMqPassword };
                IConnection connection = factory.CreateConnection();
                _channel = connection.CreateModel();
                _channel.ExchangeDeclare(_settings.RabbitMqExchangeName, ExchangeType.Direct);

                foreach (var contract in _settings.ContractAddresses)
                {
                    var queueName = $"{_settings.RabbitMqExchangeName}_{contract}";
                    _channel.QueueDeclare(queueName, true, false, false);
                    _channel.QueueBind(queueName, _settings.RabbitMqExchangeName, contract);
                }
                _logger.LogInformation($"Initialized RabbitMq, host: {_settings.RabbitMqHostName}");
            }

            ulong lastProcessedBlock = _settings.LastProcessedBlock > 0 ? _settings.LastProcessedBlock : 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                ulong blockNumber = (ulong)(await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).Value;
                if (blockNumber == lastProcessedBlock ) {
                    await Task.Delay(TimeSpan.FromMilliseconds(1000), stoppingToken);
                    continue;
                }
                _logger.LogInformation($"Latest block : {blockNumber}");

                List<TokenTransaction> transactionList = new List<TokenTransaction>();
                foreach (var contract in _settings.ContractAddresses)
                {
                    var transferEventHandler = _web3.Eth.GetEvent<TransferEventDTO>(contract);
                    var filter = transferEventHandler.CreateFilterInput(
                        fromBlock: new BlockParameter(lastProcessedBlock + 1),
                        toBlock: new BlockParameter(blockNumber)
                    );
                    var events = await transferEventHandler.GetAllChanges(filter);
                    _logger.LogInformation($"Total {events.Count} events between blocks {lastProcessedBlock + 1}-{blockNumber}");

                    var trackedEventList = events
                        .Where( transactionEvent => _trackedAddresses.Contains(transactionEvent.Event.To))
                        .Select( transactionEvent => new TokenTransaction(){
                            ContractAddress = contract,
                            From = transactionEvent.Event.From,
                            To = transactionEvent.Event.To,
                            Value = transactionEvent.Event.Value
                        })
                        .ToList();
                    _logger.LogInformation($"{trackedEventList.Count} events for tracked addresses from contract {contract}");

                    transactionList.AddRange(trackedEventList);
                }

                if(_channel != null) SendTheTransactions(transactionList);

                lastProcessedBlock = blockNumber;
            }
        }

        private void SendTheTransactions(List<TokenTransaction> transactionList) {
            if(string.IsNullOrEmpty(_settings.RabbitMqHostName)) return; 

            foreach (var tx in transactionList) {
                var txData = JsonConvert.SerializeObject(tx);
                var body = Encoding.UTF8.GetBytes(txData);
                _channel.BasicPublish(
                    exchange: _settings.RabbitMqExchangeName, 
                    routingKey: tx.ContractAddress,
                    basicProperties: null, 
                    body: body);             
            }
        }
    }
}