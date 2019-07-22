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

namespace erc20_tracker
{
    public class Erc20TrackerService : BackgroundService
    {
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly Settings _settings;
        private readonly Web3 _web3;

        public Erc20TrackerService(
            IApplicationLifetime applicationLifetime,
            IOptions<Settings> settings)
        {
            _settings = (settings ?? throw new ArgumentNullException(nameof(applicationLifetime))).Value;
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            _web3 = new Web3(_settings.NodeUrl);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ulong lastBlock = 0;

            while (!_applicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                ulong blockNumber = (ulong)(await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).Value;
                if (blockNumber == lastBlock ) {
                    await Task.Delay(TimeSpan.FromMilliseconds(1000), stoppingToken);
                    continue;
                }

                List<TokenTransaction> transactionList = new List<TokenTransaction>();
                foreach (var contract in _settings.ContractAddresses)
                {
                    var transferEventHandler = _web3.Eth.GetEvent<TransferEventDTO>(contract);
                    var filter = transferEventHandler.CreateFilterInput(
                        fromBlock: new BlockParameter(lastBlock),
                        toBlock: BlockParameter.CreateLatest()
                    );
                    var events = await transferEventHandler.GetAllChanges(filter);

                    foreach (var transactionEvent in events)
                    {
                        if(!_settings.TrackedAddresses.Select( x=> x.ToLower()).Contains(transactionEvent.Event.To)) continue;

                        var tx = new TokenTransaction(){
                            ContractAddress = contract,
                            From = transactionEvent.Event.From,
                            To = transactionEvent.Event.To,
                            Value = transactionEvent.Event.Value
                        };
                        transactionList.Add(tx);
                    }
                }

                // TODO : Send the tx list 

                lastBlock = blockNumber;
            }
        }
    }
}