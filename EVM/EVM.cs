/**
 * Author: Jonathan LEI
 * Email: xJonathan@outlook.com
 * Github: https://github.com/xJonathanLEI
 * Created: Jan 2018
 * Last Updated: Jan 2018
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
using EVM.DataQuery;
using Uint256;

namespace EVM
{
    public partial class EVMInterpreter
    {
        /**
         * In this implementation of EVM, stack adjustment is handled by the instruction itself.
         * Unlike some other implementations (e.g. cpp-ethereum) that adjust the position of
         * the stack pointer before every execution.
        **/
        private byte[] m_byteCode;
        private int m_pc;
        private Instruction m_op;
        private List<byte> m_memory;
        private List<uint256> m_stack;
        private List<EVMLog> m_log;
        /**
         * Having only one storage variable is not competible for contract calls.
         * The current version does not support external contract calls so this is fine.
         * Use state as container of storages instead in the future.
         **/
        private Dictionary<uint256, uint256> m_storage;
        private bool m_flagReverted = false;
        private bool m_flagSuicided = false;

        // Used to calculate memory expansion gas consumption. Useless right now as gas is ignored for the moment.
        private uint256 m_newMemSize;
        private byte[] m_return;

        private SHA3.SHA3Managed keccak = new SHA3.SHA3Managed(256);
        private DataGateway edg;

        // Read-only variables
        public byte[] byteCode { get { return m_byteCode; } }
        public int pc { get { return m_pc; } }
        public List<byte> memory { get { return m_memory; } }
        public List<uint256> stack { get { return m_stack; } }
        public List<EVMLog> log { get { return m_log; } }
        public Dictionary<uint256, uint256> storage { get { return m_storage; } }
        public byte[] returnData { get { return m_return; } }
        public bool flagReverted { get { return m_flagReverted; } }
        public bool flagSuicide { get { return m_flagSuicided; } }

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
            m_log = new List<EVMLog>();
            m_storage = new Dictionary<uint256, uint256>();
            edg = new EtherscanDataGateway();
        }

        public EVMInterpreter(byte[] byteCode, Dictionary<uint256, uint256> storage)
        {
            m_byteCode = byteCode;
            m_pc = 0;
            m_memory = new List<byte>();
            m_stack = new List<uint256>();
            m_log = new List<EVMLog>();
            m_storage = storage;
            edg = new EtherscanDataGateway();
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
        public async Task<bool> executeOnce()
        {
            fetchInstruction();
            switch(m_op)
            {
                //
                // Call-related instructions
                //
                case (Instruction.CREATE2):
                case (Instruction.CREATE):
                case (Instruction.DELEGATECALL):
                case (Instruction.STATICCALL):
                case (Instruction.CALL):
                case (Instruction.CALLCODE):
                    // Calling external contracts not supported yet (fixing this is not too hard though. PR welcomed)
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.RETURN):
                // Reverting state is not handled but only indicated by a flag
                case (Instruction.REVERT):
                    updateMem(memNeed(m_stack[0], m_stack[1]));
                    // A hopefully it ain't trying to return more than 0x0fffffff bytes of data lol
                    m_return = new byte[m_stack[1].ToUInt32()];
                    m_memory.CopyTo((int)m_stack[0].ToUInt32(), m_return, 0, m_return.Length);
                    m_stack.RemoveAt(0);
                    m_stack.RemoveAt(0);
                    m_flagReverted = m_op == Instruction.REVERT;
                    return false;

                case (Instruction.SUICIDE):
                    //Suicide not handled either
                    m_stack.RemoveAt(0);
                    m_flagSuicided = true;
                    return false;

                case (Instruction.STOP):
                    return false;

                //
                // instructions potentially expanding memory
                //

                case (Instruction.MLOAD):
                    updateMem(m_stack[0] + 32);
                    byte[] bytesMloaded = new byte[32];
                    m_memory.CopyTo((int)m_stack[0].ToUInt32(), bytesMloaded, 0, 32);
                    m_stack[0] = new uint256(bytesMloaded);
                    m_pc++;
                    break;

                case (Instruction.MSTORE):
                    updateMem(m_stack[0] + 32);
                    int mstoreStartIndex = (int)m_stack[0].ToUInt32();
                    for (int i = 0; i < 32; i++)
                        m_memory[mstoreStartIndex + i] = m_stack[1].bytes[i];
                    m_stack.RemoveAt(0);
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.MSTORE8):
                    updateMem(m_stack[0] + 1);
                    m_memory[(int)m_stack[0].ToUInt32()] = m_stack[1].bytes[31];
                    m_stack.RemoveAt(0);
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.SHA3):
                    updateMem(memNeed(m_stack[0], m_stack[1]));
                    int sha3Offset = (int)m_stack[0].ToUInt32();
                    int sha3Size = (int)m_stack[1].ToUInt32();
                    byte[] sha3Input = new byte[sha3Size];
                    m_memory.CopyTo(sha3Offset, sha3Input, 0, sha3Size);
                    m_stack.RemoveAt(0);
                    m_stack[0] = new uint256(keccak.ComputeHash(sha3Input));
                    m_pc++;
                    break;

                case (Instruction.LOG0):
                case (Instruction.LOG1):
                case (Instruction.LOG2):
                case (Instruction.LOG3):
                case (Instruction.LOG4):
                    updateMem(memNeed(m_stack[0], m_stack[1]));
                    List<uint256> listOfTopics = new List<uint256>();
                    int noOfTopics = (int)m_op - (int)Instruction.LOG0;
                    int logOffset = (int)m_stack[0].ToUInt32();
                    int logSize = (int)m_stack[1].ToUInt32();
                    byte[] logData = new byte[logSize];
                    m_memory.CopyTo(logOffset, logData, 0, logSize);
                    for (int i = 0; i < noOfTopics; i ++)
                        listOfTopics.Add(m_stack[i + 2]);
                    m_log.Add(new EVMLog()
                    {
                        topics = listOfTopics,
                        data = logData
                    });
                    for (int i = 0; i < noOfTopics + 2; i++)
                        m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.EXP):
                    m_stack[1] = m_stack[0].exp(m_stack[1]);
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                //
                // ordinary instructions
                //

                case (Instruction.ADD):
                    m_stack[1] = m_stack[0] + m_stack[1];
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.MUL):
                    m_stack[1] = m_stack[0] * m_stack[1];
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.SUB):
                    m_stack[1] = m_stack[0] - m_stack[1];
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.DIV):
                    if (m_stack[1] != 0)
                        m_stack[1] = m_stack[0] / m_stack[1];
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.SDIV):
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.MOD):
                    if (m_stack[1] != 0)
                        m_stack[1] = m_stack[0] % m_stack[1];
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.SMOD):
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.NOT):
                    m_stack[0] = !m_stack[0];
                    m_pc++;
                    break;

                case (Instruction.LT):
                    m_stack[1] = m_stack[0] < m_stack[1] ? 1 : 0;
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.GT):
                    m_stack[1] = m_stack[0] > m_stack[1] ? 1 : 0;
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.SLT):
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.SGT):
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.EQ):
                    m_stack[1] = m_stack[0] == m_stack[1] ? 1 : 0;
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.ISZERO):
                    m_stack[0] = m_stack[0] == 0 ? 1 : 0;
                    m_pc++;
                    break;

                case (Instruction.AND):
                    m_stack[1] = m_stack[0] & m_stack[1];
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.OR):
                    m_stack[1] = m_stack[0] | m_stack[1];
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.XOR):
                    m_stack[1] = m_stack[0] ^ m_stack[1];
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.BYTE):
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.ADDMOD):
                    if (m_stack[2] != 0)
                        m_stack[2] = (m_stack[0] + m_stack[1]) % m_stack[2];
                    m_stack.RemoveAt(0);
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.MULMOD):
                    if (m_stack[2] != 0)
                        m_stack[2] = (m_stack[0] * m_stack[1]) % m_stack[2];
                    m_stack.RemoveAt(0);
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.SIGNEXTEND):
                    throw new EVMInstructionNotYetSupported();

                // EIP 615 NOT supported
                case (Instruction.JUMPTO):
                case (Instruction.JUMPIF):
                case (Instruction.JUMPV):
                case (Instruction.JUMPSUB):
                case (Instruction.JUMPSUBV):
                case (Instruction.RETURNSUB):
                case (Instruction.BEGINSUB):
                case (Instruction.BEGINDATA):
                case (Instruction.GETLOCAL):
                case (Instruction.PUTLOCAL):
                    throw new EVMEIPNotYetSupported();

                // EIP 616 NOT supported
                case (Instruction.XADD):
                case (Instruction.XMUL):
                case (Instruction.XSUB):
                case (Instruction.XDIV):
                case (Instruction.XSDIV):
                case (Instruction.XMOD):
                case (Instruction.XSMOD):
                case (Instruction.XLT):
                case (Instruction.XGT):
                case (Instruction.XSLT):
                case (Instruction.XSGT):
                case (Instruction.XEQ):
                case (Instruction.XISZERO):
                case (Instruction.XAND):
                case (Instruction.XOOR):
                case (Instruction.XXOR):
                case (Instruction.XNOT):
                case (Instruction.XSHL):
                case (Instruction.XSHR):
                case (Instruction.XSAR):
                case (Instruction.XROL):
                case (Instruction.XROR):
                case (Instruction.XMLOAD):
                case (Instruction.XMSTORE):
                case (Instruction.XSLOAD):
                case (Instruction.XSSTORE):
                case (Instruction.XVTOWIDE):
                case (Instruction.XWIDETOV):
                case (Instruction.XPUSH):
                case (Instruction.XPUT):
                case (Instruction.XGET):
                case (Instruction.XSWIZZLE):
                case (Instruction.XSHUFFLE):
                    throw new EVMEIPNotYetSupported();

                // 00000000
                case (Instruction.ADDRESS):
                    m_stack.Insert(0, transaction.to);
                    m_pc++;
                    break;

                case (Instruction.ORIGIN):
                    // TODO: REMEMBER TO MODIFY THIS AFTER CALLING IS SUPPORTED
                    // OR IT WILL BECOME A VERY HARD TO FIND BUG
                    m_stack.Insert(0, transaction.sender);
                    m_pc++;
                    break;

                case (Instruction.BALANCE):
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.CALLER):
                    m_stack.Insert(0, transaction.sender);
                    m_pc++;
                    break;

                case (Instruction.CALLVALUE):
                    m_stack.Insert(0, transaction.value);
                    m_pc++;
                    break;

                case (Instruction.CALLDATALOAD):
                    byte[] cdloadBytes = new byte[32];
                    int cdloadOffset = (int)m_stack[0].ToUInt32();
                    for (int i = 0; i < 32; i++)
                        cdloadBytes[i] = cdloadOffset + i < transaction.data.Length ? transaction.data[cdloadOffset + i] : (byte)0;
                    m_stack[0] = new uint256(cdloadBytes);
                    m_pc++;
                    break;

                case (Instruction.CALLDATASIZE):
                    m_stack.Insert(0, transaction.data.Length);
                    m_pc++;
                    break;

                case (Instruction.RETURNDATASIZE):
                case (Instruction.CODESIZE):
                case (Instruction.EXTCODESIZE):
                case (Instruction.CALLDATACOPY):
                case (Instruction.RETURNDATACOPY):
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.CODECOPY):
                    updateMem(memNeed(m_stack[0], m_stack[1]));
                    copyDataToMemory(byteCode, m_stack[0], m_stack[1], m_stack[2]);
                    m_stack.RemoveAt(0);
                    m_stack.RemoveAt(0);
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.EXTCODECOPY):
                case (Instruction.GASPRICE):
                case (Instruction.BLOCKHASH):
                case (Instruction.COINBASE):
                case (Instruction.TIMESTAMP):
                case (Instruction.NUMBER):
                case (Instruction.DIFFICULTY):
                case (Instruction.GASLIMIT):
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.POP):
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.PUSHC):
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.PUSH1):
                    m_pc++;
                    m_stack.Insert(0, new uint256(new byte[] { byteCode[m_pc] }));
                    m_pc++;
                    break;
                case (Instruction.PUSH2):
                case (Instruction.PUSH3):
                case (Instruction.PUSH4):
                case (Instruction.PUSH5):
                case (Instruction.PUSH6):
                case (Instruction.PUSH7):
                case (Instruction.PUSH8):
                case (Instruction.PUSH9):
                case (Instruction.PUSH10):
                case (Instruction.PUSH11):
                case (Instruction.PUSH12):
                case (Instruction.PUSH13):
                case (Instruction.PUSH14):
                case (Instruction.PUSH15):
                case (Instruction.PUSH16):
                case (Instruction.PUSH17):
                case (Instruction.PUSH18):
                case (Instruction.PUSH19):
                case (Instruction.PUSH20):
                case (Instruction.PUSH21):
                case (Instruction.PUSH22):
                case (Instruction.PUSH23):
                case (Instruction.PUSH24):
                case (Instruction.PUSH25):
                case (Instruction.PUSH26):
                case (Instruction.PUSH27):
                case (Instruction.PUSH28):
                case (Instruction.PUSH29):
                case (Instruction.PUSH30):
                case (Instruction.PUSH31):
                case (Instruction.PUSH32):
                    m_pc++;
                    int pushLength = m_op - Instruction.PUSH1 + 1;
                    byte[] pushBytes = new byte[pushLength];
                    Array.Copy(m_byteCode, m_pc, pushBytes, 0, pushLength);
                    m_stack.Insert(0, new uint256(pushBytes));
                    m_pc += pushLength;
                    break;

                case (Instruction.JUMP):
                    if (m_byteCode[(int)m_stack[0].ToUInt32()] != (int)Instruction.JUMPDEST)
                        throw new EVMNotJumpDestException();
                    m_pc = (int)m_stack[0].ToUInt32();
                    m_stack.RemoveAt(0);
                    break;

                case (Instruction.JUMPI):
                    if (m_stack[1] != 0)
                    {
                        if (m_byteCode[(int)m_stack[0].ToUInt32()] != (int)Instruction.JUMPDEST)
                            throw new EVMNotJumpDestException();
                        m_pc = (int)m_stack[0].ToUInt32();
                    }
                    else
                        m_pc++;
                    m_stack.RemoveAt(0);
                    m_stack.RemoveAt(0);
                    break;

                case (Instruction.JUMPC):
                    m_pc = (int)m_stack[0].ToUInt32();
                    m_stack.RemoveAt(0);
                    break;

                case (Instruction.JUMPCI):
                    if (m_stack[1] == 0)
                        m_pc = (int)m_stack[0].ToUInt32();
                    else
                        m_pc++;
                    m_stack.RemoveAt(0);
                    m_stack.RemoveAt(0);
                    break;

                case (Instruction.DUP1):
                case (Instruction.DUP2):
                case (Instruction.DUP3):
                case (Instruction.DUP4):
                case (Instruction.DUP5):
                case (Instruction.DUP6):
                case (Instruction.DUP7):
                case (Instruction.DUP8):
                case (Instruction.DUP9):
                case (Instruction.DUP10):
                case (Instruction.DUP11):
                case (Instruction.DUP12):
                case (Instruction.DUP13):
                case (Instruction.DUP14):
                case (Instruction.DUP15):
                case (Instruction.DUP16):
                    int dupN = (int)m_op - (int)Instruction.DUP1;
                    m_stack.Insert(0, m_stack[dupN]);
                    m_pc++;
                    break;

                case (Instruction.SWAP1):
                case (Instruction.SWAP2):
                case (Instruction.SWAP3):
                case (Instruction.SWAP4):
                case (Instruction.SWAP5):
                case (Instruction.SWAP6):
                case (Instruction.SWAP7):
                case (Instruction.SWAP8):
                case (Instruction.SWAP9):
                case (Instruction.SWAP10):
                case (Instruction.SWAP11):
                case (Instruction.SWAP12):
                case (Instruction.SWAP13):
                case (Instruction.SWAP14):
                case (Instruction.SWAP15):
                case (Instruction.SWAP16):
                    int swapN = (int)m_op - (int)Instruction.SWAP1 + 1;
                    uint256 swapTemp = m_stack[0];
                    m_stack[0] = m_stack[swapN];
                    m_stack[swapN] = swapTemp;
                    m_pc++;
                    break;

                case (Instruction.SLOAD):
                    // If storage already exists (even with value 0x0), take that.
                    // Other wise, find storage data online with DataGateway.
                    // Take the storage data at the PREVIOUS block height
                    if (!m_storage.ContainsKey(m_stack[0]))
                        m_storage.Add(m_stack[0], await edg.getStorageAt(transaction.to, m_stack[0], (int)(ext.blockheight - 1).ToUInt32()));
                    m_stack[0] = m_storage[m_stack[0]];
                    m_pc++;
                    break;

                case (Instruction.SSTORE):
                    if (!m_storage.ContainsKey(m_stack[0]))
                        m_storage.Add(m_stack[0], m_stack[1]);
                    else
                        m_storage[m_stack[0]] = m_stack[1];
                    m_stack.RemoveAt(0);
                    m_stack.RemoveAt(0);
                    m_pc++;
                    break;

                case (Instruction.PC):
                    m_stack.Insert(0, m_pc);
                    m_pc++;
                    break;

                case (Instruction.MSIZE):
                    m_stack.Insert(0, m_memory.Count);
                    m_pc++;
                    break;

                case (Instruction.GAS):
                    throw new EVMInstructionNotYetSupported();

                case (Instruction.JUMPDEST):
                    m_pc++;
                    break;

                case (Instruction.INVALID):
                default:
                    throw new EVMInstructionNotYetSupported();
            }
            return true;
        }

        private void fetchInstruction()
        {
            if (m_pc >= byteCode.Length)
                throw new EVMNoInstruction();
            m_op = (Instruction)byteCode[m_pc];
        }

        // Exapnd memory as needed
        private void updateMem(uint256 newSize)
        {
            // Accepting uint256 just for convenience. Should actually use int instead (since int would be sufficient, or long if you prefer)
            m_newMemSize = (newSize + 31) / 32 * 32;
            if (m_newMemSize > m_memory.Count)
                m_memory.AddRange(new byte[(int)m_newMemSize.ToUInt32(false) - m_memory.Count]);
        }

        private uint256 memNeed(uint256 offset, uint256 size)
        {
            return size == 0 ? 0 : offset + size;
        }

        // This is absolutely not a performance-oriented way of copying... But the EVM is for debugging anyways
        private void copyDataToMemory(byte[] data, uint256 offset, uint256 index, uint256 size)
        {
            for (uint256 i = 0; i < size; i++)
                m_memory[(int)(offset + i).ToUInt32()] = index + i >= data.Length ? (byte)0 : data[(int)(index + i).ToUInt32()];
        }
    }
}
