/**
 * Author: Jonathan LEI
 * Email: xJonathan@outlook.com
 * Github: https://github.com/xJonathanLEI
 * Created: Jan 2018
 * Last Updated: Jan 2018
 * Introduction:
 * - An implementation of Ethereum data gateway using Etherscan APIs.
**/

using System.Net.Http;
using EVM.Structures;
using Uint256;

namespace EVM.DataQuery
{
    /// <summary>
    /// An implementation of Ethereum data gateway using Etherscan APIs
    /// </summary>
    public class EtherscanDataGateway : DataGateway
    {
        public override Transaction getTransactionByHash(uint256 txHash)
        {
            throw new System.NotImplementedException();
        }

        public override uint256 getStorageAt(uint256 address, uint256 index, int atBlock)
        {
            throw new System.NotImplementedException();
        }

        public override uint256 getStorageAt(uint256 address, uint256 index, string tag)
        {
            throw new System.NotImplementedException();
        }

        public override uint256 getBlockHashByHeight(int blockHeight)
        {
            throw new System.NotImplementedException();
        }
    }
}
