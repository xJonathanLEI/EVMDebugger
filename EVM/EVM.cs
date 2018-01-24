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
using Uint256;

namespace EVM
{
    public class EVM
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
        private Dictionary<uint256, uint256> m_storage;

        public byte[] byteCode { get { return m_byteCode; } }
        public int pc { get { return m_pc; } }
        public List<byte> memory { get { return m_memory; } }
        public List<uint256> stack { get { return m_stack; } }
        public Dictionary<uint256, uint256> storage { get { return m_storage; } }

        public EVM(byte[] byteCode)
        {
            m_byteCode = byteCode;
            m_pc = 0;
            m_memory = new List<byte>();
            m_stack = new List<uint256>();
            m_storage = new Dictionary<uint256, uint256>();
        }

        public EVM(byte[] byteCode, Dictionary<uint256, uint256> storage)
        {
            m_byteCode = byteCode;
            m_pc = 0;
            m_memory = new List<byte>();
            m_stack = new List<uint256>();
            m_storage = storage;
        }
    }
}
