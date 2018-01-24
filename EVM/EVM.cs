/**
 * Author: Jonathan LEI
 * Email: xJonathan@outlook.com
 * Github: https://github.com/xJonathanLEI
 * Created: Jan 2018
 * Last Updated: Jan 2018
 * Dependency:
 * - Nuget package: SHA3 (v0.9.2 used)
 * - - Installation: Install-Package SHA3
 * Introduction:
 * - This is an implementation of Ethereum Virtual Machine in C#
 * - Gas is ignored in this version. OutofGas never happens
 * - The code is used as part of the EVM debugger
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVM.Exceptions;
using EVM.Structures;
using Uint256;

namespace EVM
{
    public class EVMInterpreter
    {
        /**
         * In this implementation of EVM, stack adjustment is handled by the instruction itself.
         * Unlike some other implementations (e.g. cpp-ethereum) that adjust the position of
         * the stack pointer before every execution.
        **/
        private byte[] m_byteCode;
        private int m_pc;
        private List<byte> m_memory;
        private List<uint256> m_stack;
        /**
         * Having only one storage variable is not competible for contract calls.
         * The current version does not support external contract calls so this is fine.
         * Use state as container of storages instead in the future.
         **/
        private Dictionary<uint256, uint256> m_storage;
        private byte[] m_return;

        // Read-only variables
        public byte[] byteCode { get { return m_byteCode; } }
        public int pc { get { return m_pc; } }
        public List<byte> memory { get { return m_memory; } }
        public List<uint256> stack { get { return m_stack; } }
        public Dictionary<uint256, uint256> storage { get { return m_storage; } }
        public byte[] returnData { get { return m_return; } }

        public Transaction transaction = new Transaction() // Default transaction
        {
            data = new byte[0], // No data sent
            value = 0, // No ether sent
            gasPrice = 0x05d21dba00, // 25 Gwei
            startGas = 0x07a120, // 500,000
            sender = "71d61BBe11f4e11CFF69e56B967Aa1C1a586f778", // Fake account
            to = "5d1B26d762b1973B8B7C2bFb196Ba2ED969dAF18" // Fake contract account
        };
        public Externalities ext = new Externalities()
        {
            timestamp = 1516793709,
            blockheight = 4963838
        };

        public EVMInterpreter(byte[] byteCode)
        {
            m_byteCode = byteCode;
            m_pc = 0;
            m_memory = new List<byte>();
            m_stack = new List<uint256>();
            m_storage = new Dictionary<uint256, uint256>();
        }

        public EVMInterpreter(byte[] byteCode, Dictionary<uint256, uint256> storage)
        {
            m_byteCode = byteCode;
            m_pc = 0;
            m_memory = new List<byte>();
            m_stack = new List<uint256>();
            m_storage = storage;
        }

        /// <summary>
        /// Execute until the contract ends or throws an error.
        /// </summary>
        /// <returns>Data returned by the contract.</returns>
        public byte[] execute()
        {
            return new byte[0];
        }

        /// <summary>
        /// Execute the next instruction only.
        /// </summary>
        /// <returns>Returns false if execution ended, and true otherwise.</returns>
        public bool executeOnce()
        {
            return true;
        }
    }
}
