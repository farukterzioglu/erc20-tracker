using System.Collections.Generic;

namespace erc20_tracker
{
    public class Settings
    {
        public List<string> ContractAddresses { get; set; }
        public List<string> TrackedAddresses { get; set; }
        public string RabbitMqHostName {get;set;}
        public string RabbitMqUsername {get;set;}
        public string RabbitMqPassword {get;set;}
        public string RabbitMqExchangeName {get;set;}
        public string NodeUrl {get;set;}
        public string Seed { get; set; }
        public int HdAddressCount { get; set; }
        public ulong LastProcessedBlock { get; set; }
    }
}