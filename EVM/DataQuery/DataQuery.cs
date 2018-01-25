/**
 * Author: Jonathan LEI
 * Email: xJonathan@outlook.com
 * Github: https://github.com/xJonathanLEI
 * Created: Jan 2018
 * Last Updated: Jan 2018
 * Introduction:
 * - An abstract class defining the interface for Ethereum data gateways
**/

using Uint256;
using EVM.Structures;
using System.Threading.Tasks;

namespace EVM.DataQuery
{
    /// <summary>
    /// Abstract class for any Ethereum data gateway implementation
    /// </summary>
    public abstract class DataGateway
    {
        /// <summary>
        /// Gets a transaction from a 256-bit Keccak hash.
        /// </summary>
        /// <param name="txHash">Transaction hash</param>
        /// <returns>The transaction if it exists, or null otherwise.</returns>
        public abstract Task<Transaction> getTransactionByHash(uint256 txHash);

        /// <summary>
        /// Get contract storage value
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="index">Storage index</param>
        /// <param name="atBlock">Block height</param>
        /// <returns>Storage value</returns>
        public abstract Task<uint256> getStorageAt(uint256 address, uint256 index, int atBlock);

        /// <summary>
        /// Get contract storage value
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="index">Storage index</param>
        /// <param name="tag">Search tag</param>
        /// <returns>Storage value</returns>
        public abstract Task<uint256> getStorageAt(uint256 address, uint256 index, string tag);

        /// <summary>
        /// Get contract code at address
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <returns>Contract bytecode</returns>
        public abstract Task<byte[]> getCodeAt(uint256 address);

        /// <summary>
        /// Get block hash by block height
        /// </summary>
        /// <param name="blockHeight">Block height</param>
        /// <returns>Block hash</returns>
        public abstract Task<uint256> getBlockHashByHeight(int blockHeight);
    }
}
