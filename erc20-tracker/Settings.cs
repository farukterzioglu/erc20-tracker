using System.Collections.Generic;

namespace erc20_tracker
{
    public class Settings
    {
        public List<string> ContractAddresses { get; set; }
        public List<string> TrackedAddresses { get; set; }
        public string ZeroMqUrl {get;set;}
        public string NodeUrl {get;set;}
    }
}