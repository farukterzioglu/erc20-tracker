using System.Collections.Generic;

namespace erc20_tracker
{
    public class Settings
    {
        public string ContractAddress { get; set; }
        public List<string> TrackedAddresses { get; set; }
        public string ZeroMqUrl {get;set;}
    }
}