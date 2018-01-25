using System.Collections.Generic;
using Uint256;

namespace EVM
{
    namespace Structures
    {
        public struct Transaction
        {
            public uint256 sender { get; set; }
            public uint256 to { get; set; }
            public uint256 value { get; set; }
            public uint256 startGas { get; set; }
            public uint256 gasPrice { get; set; }
            public byte[] data { get; set; }
        }

        public struct Externalities
        {
            public uint256 blockheight { get; set; }
            public uint256 timestamp { get; set; }
            public uint256 coinbase { get; set; }
            public uint256 difficulty { get; set; }
            public uint256 gaslimit { get; set; }
        }

        public struct EVMLog
        {
            public List<uint256> topics;
            public byte[] data;
        }
    }
}
