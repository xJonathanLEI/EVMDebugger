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
using System.Threading.Tasks;
using EVM.Structures;
using Uint256;
using Newtonsoft.Json.Linq;

namespace EVM.DataQuery
{
    /// <summary>
    /// An implementation of Ethereum data gateway using Etherscan APIs
    /// </summary>
    public class EtherscanDataGateway : DataGateway
    {
        private HttpClient hc = new HttpClient();

        private const string BLOCK_INFO_API = "https://api.etherscan.io/api?module=proxy&action=eth_getBlockByNumber&tag={0}&boolean=true";
        private const string GET_CODE_API = "https://api.etherscan.io/api?module=proxy&action=eth_getCode&address={0}&tag={1}";
        private const string GET_STORAGE_AT_API = "https://api.etherscan.io/api?module=proxy&action=eth_getStorageAt&address={0}&position={1}&tag={2}";
        private const string GET_TRANSACTION_BY_HASH_API = "https://api.etherscan.io/api?module=proxy&action=eth_getTransactionByHash&txhash={0}";

        public override async Task<Transaction> getTransactionByHash(uint256 txHash)
        {
            dynamic res = JObject.Parse(await hc.GetStringAsync(string.Format(GET_TRANSACTION_BY_HASH_API, "0x" + txHash.ToString(true))));
            return new Transaction()
            {
                sender = (string)res.result.from,
                to = (string)res.result.to,
                startGas = (string)res.result.gas,
                gasPrice = (string)res.result.gasPrice,
                data = Utility.parseHexString(res.result.input),
                value = (string)res.result.value
            };
        }

        public override async Task<uint256> getStorageAt(uint256 address, uint256 index, int atBlock)
        {
            return await getStorageAt(address, index, "0x" + ((uint256)atBlock).ToStringMinimal());
        }

        public override async Task<uint256> getStorageAt(uint256 address, uint256 index, string tag)
        {
            dynamic res = JObject.Parse(await hc.GetStringAsync(string.Format(GET_STORAGE_AT_API, "0x" + ((uint256)address).ToString(false), "0x" + index.ToString(true), tag)));
            return (string)res.result;
        }

        public override async Task<uint256> getBlockHashByHeight(int blockHeight)
        {
            dynamic res = JObject.Parse(await hc.GetStringAsync(string.Format(BLOCK_INFO_API, "0x" + ((uint256)blockHeight).ToStringMinimal())));
            return (string)res.result.hash;
        }
    }
}
