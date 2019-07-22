using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace erc20_tracker
{
    public class TokenTransaction 
    {
        public string ContractAddress { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public BigInteger Value { get; set; }
    }

    [Event("Transfer")]
    public class TransferEventDTO : IEventDTO
    {
        [Parameter("address", "_from", 1, true)]
        public string From { get; set; }

        [Parameter("address", "_to", 2, true)]
        public string To { get; set; }

        [Parameter("uint256", "_value", 3, false)]
        public BigInteger Value { get; set; }
    }
}