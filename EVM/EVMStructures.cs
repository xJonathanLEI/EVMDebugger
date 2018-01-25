using System.Collections.Generic;

namespace EVM
{
    public partial class EVMInterpreter
    {
        public enum Instruction
        {
            STOP = 0x00,        //< halts execution
            ADD,                //< addition operation
            MUL,                //< mulitplication operation
            SUB,                //< subtraction operation
            DIV,                //< integer division operation
            SDIV,               //< signed integer division operation
            MOD,                //< modulo remainder operation
            SMOD,               //< signed modulo remainder operation
            ADDMOD,             //< unsigned modular addition
            MULMOD,             //< unsigned modular multiplication
            EXP,                //< exponential operation
            SIGNEXTEND,         //< extend length of signed integer

            LT = 0x10,          //< less-than comparision
            GT,                 //< greater-than comparision
            SLT,                //< signed less-than comparision
            SGT,                //< signed greater-than comparision
            EQ,                 //< equality comparision
            ISZERO,             //< simple not operator
            AND,                //< bitwise AND operation
            OR,                 //< bitwise OR operation
            XOR,                //< bitwise XOR operation
            NOT,                //< bitwise NOT opertation
            BYTE,               //< retrieve single byte from word

            SHA3 = 0x20,        //< compute SHA3-256 hash

            ADDRESS = 0x30,     //< get address of currently executing account
            BALANCE,            //< get balance of the given account
            ORIGIN,             //< get execution origination address
            CALLER,             //< get caller address
            CALLVALUE,          //< get deposited value by the instruction/transaction responsible for this execution
            CALLDATALOAD,       //< get input data of current environment
            CALLDATASIZE,       //< get size of input data in current environment
            CALLDATACOPY,       //< copy input data in current environment to memory
            CODESIZE,           //< get size of code running in current environment
            CODECOPY,           //< copy code running in current environment to memory
            GASPRICE,           //< get price of gas in current environment
            EXTCODESIZE,        //< get external code size (from another contract)
            EXTCODECOPY,        //< copy external code (from another contract)
            RETURNDATASIZE = 0x3d,  //< size of data returned from previous call
            RETURNDATACOPY = 0x3e,  //< copy data returned from previous call to memory

            BLOCKHASH = 0x40,   //< get hash of most recent complete block
            COINBASE,           //< get the block's coinbase address
            TIMESTAMP,          //< get the block's timestamp
            NUMBER,             //< get the block's number
            DIFFICULTY,         //< get the block's difficulty
            GASLIMIT,           //< get the block's gas limit

            POP = 0x50,         //< remove item from stack
            MLOAD,              //< load word from memory
            MSTORE,             //< save word to memory
            MSTORE8,            //< save byte to memory
            SLOAD,              //< load word from storage
            SSTORE,             //< save word to storage
            JUMP,               //< alter the program counter to a jumpdest
            JUMPI,              //< conditionally alter the program counter
            PC,                 //< get the program counter
            MSIZE,              //< get the size of active memory
            GAS,                //< get the amount of available gas
            JUMPDEST,           //< set a potential jump destination

            PUSH1 = 0x60,       //< place 1 byte item on stack
            PUSH2,              //< place 2 byte item on stack
            PUSH3,              //< place 3 byte item on stack
            PUSH4,              //< place 4 byte item on stack
            PUSH5,              //< place 5 byte item on stack
            PUSH6,              //< place 6 byte item on stack
            PUSH7,              //< place 7 byte item on stack
            PUSH8,              //< place 8 byte item on stack
            PUSH9,              //< place 9 byte item on stack
            PUSH10,             //< place 10 byte item on stack
            PUSH11,             //< place 11 byte item on stack
            PUSH12,             //< place 12 byte item on stack
            PUSH13,             //< place 13 byte item on stack
            PUSH14,             //< place 14 byte item on stack
            PUSH15,             //< place 15 byte item on stack
            PUSH16,             //< place 16 byte item on stack
            PUSH17,             //< place 17 byte item on stack
            PUSH18,             //< place 18 byte item on stack
            PUSH19,             //< place 19 byte item on stack
            PUSH20,             //< place 20 byte item on stack
            PUSH21,             //< place 21 byte item on stack
            PUSH22,             //< place 22 byte item on stack
            PUSH23,             //< place 23 byte item on stack
            PUSH24,             //< place 24 byte item on stack
            PUSH25,             //< place 25 byte item on stack
            PUSH26,             //< place 26 byte item on stack
            PUSH27,             //< place 27 byte item on stack
            PUSH28,             //< place 28 byte item on stack
            PUSH29,             //< place 29 byte item on stack
            PUSH30,             //< place 30 byte item on stack
            PUSH31,             //< place 31 byte item on stack
            PUSH32,             //< place 32 byte item on stack

            DUP1 = 0x80,        //< copies the highest item in the stack to the top of the stack
            DUP2,               //< copies the second highest item in the stack to the top of the stack
            DUP3,               //< copies the third highest item in the stack to the top of the stack
            DUP4,               //< copies the 4th highest item in the stack to the top of the stack
            DUP5,               //< copies the 5th highest item in the stack to the top of the stack
            DUP6,               //< copies the 6th highest item in the stack to the top of the stack
            DUP7,               //< copies the 7th highest item in the stack to the top of the stack
            DUP8,               //< copies the 8th highest item in the stack to the top of the stack
            DUP9,               //< copies the 9th highest item in the stack to the top of the stack
            DUP10,              //< copies the 10th highest item in the stack to the top of the stack
            DUP11,              //< copies the 11th highest item in the stack to the top of the stack
            DUP12,              //< copies the 12th highest item in the stack to the top of the stack
            DUP13,              //< copies the 13th highest item in the stack to the top of the stack
            DUP14,              //< copies the 14th highest item in the stack to the top of the stack
            DUP15,              //< copies the 15th highest item in the stack to the top of the stack
            DUP16,              //< copies the 16th highest item in the stack to the top of the stack

            SWAP1 = 0x90,       //< swaps the highest and second highest value on the stack
            SWAP2,              //< swaps the highest and third highest value on the stack
            SWAP3,              //< swaps the highest and 4th highest value on the stack
            SWAP4,              //< swaps the highest and 5th highest value on the stack
            SWAP5,              //< swaps the highest and 6th highest value on the stack
            SWAP6,              //< swaps the highest and 7th highest value on the stack
            SWAP7,              //< swaps the highest and 8th highest value on the stack
            SWAP8,              //< swaps the highest and 9th highest value on the stack
            SWAP9,              //< swaps the highest and 10th highest value on the stack
            SWAP10,             //< swaps the highest and 11th highest value on the stack
            SWAP11,             //< swaps the highest and 12th highest value on the stack
            SWAP12,             //< swaps the highest and 13th highest value on the stack
            SWAP13,             //< swaps the highest and 14th highest value on the stack
            SWAP14,             //< swaps the highest and 15th highest value on the stack
            SWAP15,             //< swaps the highest and 16th highest value on the stack
            SWAP16,             //< swaps the highest and 17th highest value on the stack

            LOG0 = 0xa0,        //< Makes a log entry; no topics.
            LOG1,               //< Makes a log entry; 1 topic.
            LOG2,               //< Makes a log entry; 2 topics.
            LOG3,               //< Makes a log entry; 3 topics.
            LOG4,               //< Makes a log entry; 4 topics.

            // these are generated by the interpreter - should never be in user code
            PUSHC = 0xac,       //< push value from constant pool
            JUMPC,              //< alter the program counter - pre-verified
            JUMPCI,             //< conditionally alter the program counter - pre-verified

            JUMPTO = 0xb0,      //< alter the program counter to a jumpdest
            JUMPIF,             //< conditionally alter the program counter
            JUMPSUB,            //< alter the program counter to a beginsub
            JUMPV,              //< alter the program counter to a jumpdest
            JUMPSUBV,           //< alter the program counter to a beginsub
            BEGINSUB,           //< set a potential jumpsub destination
            BEGINDATA,          //< begine the data section
            RETURNSUB,          //< return to subroutine jumped from
            PUTLOCAL,           //< pop top of stack to local variable
            GETLOCAL,           //< push local variable to top of stack

            XADD = 0xc1,        //< addition operation
            XMUL,               //< mulitplication operation
            XSUB,               //< subtraction operation
            XDIV,               //< integer division operation
            XSDIV,              //< signed integer division operation
            XMOD,               //< modulo remainder operation
            XSMOD,              //< signed modulo remainder operation
            XLT = 0xd0,         //< less-than comparision
            XGT,                //< greater-than comparision
            XSLT,               //< signed less-than comparision
            XSGT,               //< signed greater-than comparision
            XEQ,                //< equality comparision
            XISZERO,            //< simple not operator
            XAND,               //< bitwise AND operation
            XOOR,               //< bitwise OR operation
            XXOR,               //< bitwise XOR operation
            XNOT,               //< bitwise NOT opertation
            XSHL = 0xdb,        //< shift left opertation
            XSHR,               //< shift right opertation
            XSAR,               //< shift arithmetic right opertation
            XROL,               //< rotate left opertation
            XROR,               //< rotate right opertation
            XPUSH = 0xe0,       //< push vector to stack
            XMLOAD,             //< load vector from memory
            XMSTORE,            //< save vector to memory
            XSLOAD = 0xe4,      //< load vector from storage
            XSSTORE,            //< save vector to storage
            XVTOWIDE,           //< convert vector to wide integer
            XWIDETOV,           //< convert wide integer to vector
            XGET,               //< get data from vector
            XPUT,               //< put data in vector
            XSWIZZLE,           //< permute data in vector
            XSHUFFLE,           //< permute data in two vectors

            CREATE = 0xf0,      //< create a new account with associated code
            CALL,               //< message-call into an account
            CALLCODE,           //< message-call with another account's code only
            RETURN,             //< halt execution returning output data
            DELEGATECALL,       //< like CALLCODE but keeps caller's value and sender
            STATICCALL = 0xfa,  //< like CALL except state changing operation are not permitted (will throw)
            CREATE2 = 0xfb,     //< create a new account with associated code. sha3((sender + salt + sha3(code))
            REVERT = 0xfd,      //< stop execution and revert state changes, without consuming all provided gas
            INVALID = 0xfe,     //< dedicated invalid instruction
            SUICIDE = 0xff      //< halt execution and register account for later deletion
        }

        public enum Tier
        {
            Zero = 0,   // 0, Zero
            Base,       // 2, Quick
            VeryLow,    // 3, Fastest
            Low,        // 5, Fast
            Mid,        // 8, Mid
            High,       // 10, Slow
            Ext,        // 20, Ext
            Special,    // multiparam or otherwise special
            Invalid     // Invalid.
        }

        public static Dictionary<Instruction, int[]> instructionInfos = new Dictionary<Instruction, int[]>()
        { //                                               Add,  Args,  Ret,  GasPriceTier
	         { Instruction.STOP,          new int[] { 0,     0,    0,  (int)Tier.Zero } },
             { Instruction.ADD,           new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.SUB,           new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.MUL,           new int[] { 0,     2,    1,  (int)Tier.Low } },
             { Instruction.DIV,           new int[] { 0,     2,    1,  (int)Tier.Low } },
             { Instruction.SDIV,          new int[] { 0,     2,    1,  (int)Tier.Low } },
             { Instruction.MOD,           new int[] { 0,     2,    1,  (int)Tier.Low } },
             { Instruction.SMOD,          new int[] { 0,     2,    1,  (int)Tier.Low } },
             { Instruction.EXP,           new int[] { 0,     2,    1,  (int)Tier.Special } },
             { Instruction.NOT,           new int[] { 0,     1,    1,  (int)Tier.VeryLow } },
             { Instruction.LT,            new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.GT,            new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.SLT,           new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.SGT,           new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.EQ,            new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.ISZERO,        new int[] { 0,     1,    1,  (int)Tier.VeryLow } },
             { Instruction.AND,           new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.OR,            new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.XOR,           new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.BYTE,          new int[] { 0,     2,    1,  (int)Tier.VeryLow } },
             { Instruction.ADDMOD,        new int[] { 0,     3,    1,  (int)Tier.Mid } },
             { Instruction.MULMOD,        new int[] { 0,     3,    1,  (int)Tier.Mid } },
             { Instruction.SIGNEXTEND,    new int[] { 0,     2,    1,  (int)Tier.Low } },
             { Instruction.SHA3,          new int[] { 0,     2,    1,  (int)Tier.Special } },
             { Instruction.ADDRESS,       new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.BALANCE,       new int[] { 0,     1,    1,  (int)Tier.Special } },
             { Instruction.ORIGIN,        new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.CALLER,        new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.CALLVALUE,     new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.CALLDATALOAD,  new int[] { 0,     1,    1,  (int)Tier.VeryLow } },
             { Instruction.CALLDATASIZE,  new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.CALLDATACOPY,  new int[] { 0,     3,    0,  (int)Tier.VeryLow } },
             { Instruction.CODESIZE,      new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.CODECOPY,      new int[] { 0,     3,    0,  (int)Tier.VeryLow } },
             { Instruction.GASPRICE,      new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.EXTCODESIZE,   new int[] { 0,     1,    1,  (int)Tier.Special } },
             { Instruction.EXTCODECOPY,   new int[] { 0,     4,    0,  (int)Tier.Special } },
             { Instruction.RETURNDATASIZE, new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.RETURNDATACOPY, new int[] { 0,     3,    0,  (int)Tier.VeryLow } },
             { Instruction.BLOCKHASH,     new int[] { 0,     1,    1,  (int)Tier.Special } },
             { Instruction.COINBASE,      new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.TIMESTAMP,     new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.NUMBER,        new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.DIFFICULTY,    new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.GASLIMIT,      new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.POP,           new int[] { 0,     1,    0,  (int)Tier.Base } },
             { Instruction.MLOAD,         new int[] { 0,     1,    1,  (int)Tier.VeryLow } },
             { Instruction.MSTORE,        new int[] { 0,     2,    0,  (int)Tier.VeryLow } },
             { Instruction.MSTORE8,       new int[] { 0,     2,    0,  (int)Tier.VeryLow } },
             { Instruction.SLOAD,         new int[] { 0,     1,    1,  (int)Tier.Special } },
             { Instruction.SSTORE,        new int[] { 0,     2,    0,  (int)Tier.Special } },
             { Instruction.JUMP,          new int[] { 0,     1,    0,  (int)Tier.Mid } },
             { Instruction.JUMPI,         new int[] { 0,     2,    0,  (int)Tier.High } },
             { Instruction.PC,            new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.MSIZE,         new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.GAS,           new int[] { 0,     0,    1,  (int)Tier.Base } },
             { Instruction.JUMPDEST,      new int[] { 0,     0,    0,  (int)Tier.Special } },
             { Instruction.PUSH1,         new int[] { 1,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH2,         new int[] { 2,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH3,         new int[] { 3,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH4,         new int[] { 4,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH5,         new int[] { 5,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH6,         new int[] { 6,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH7,         new int[] { 7,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH8,         new int[] { 8,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH9,         new int[] { 9,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH10,        new int[] { 10,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH11,        new int[] { 11,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH12,        new int[] { 12,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH13,        new int[] { 13,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH14,        new int[] { 14,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH15,        new int[] { 15,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH16,        new int[] { 16,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH17,        new int[] { 17,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH18,        new int[] { 18,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH19,        new int[] { 19,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH20,        new int[] { 20,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH21,        new int[] { 21,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH22,        new int[] { 22,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH23,        new int[] { 23,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH24,        new int[] { 24,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH25,        new int[] { 25,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH26,        new int[] { 26,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH27,        new int[] { 27,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH28,        new int[] { 28,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH29,        new int[] { 29,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH30,        new int[] { 30,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH31,        new int[] { 31,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.PUSH32,        new int[] { 32,     0,    1,  (int)Tier.VeryLow } },
             { Instruction.DUP1,          new int[] { 0,     1,    2,  (int)Tier.VeryLow } },
             { Instruction.DUP2,          new int[] { 0,     2,    3,  (int)Tier.VeryLow } },
             { Instruction.DUP3,          new int[] { 0,     3,    4,  (int)Tier.VeryLow } },
             { Instruction.DUP4,          new int[] { 0,     4,    5,  (int)Tier.VeryLow } },
             { Instruction.DUP5,          new int[] { 0,     5,    6,  (int)Tier.VeryLow } },
             { Instruction.DUP6,          new int[] { 0,     6,    7,  (int)Tier.VeryLow } },
             { Instruction.DUP7,          new int[] { 0,     7,    8,  (int)Tier.VeryLow } },
             { Instruction.DUP8,          new int[] { 0,     8,    9,  (int)Tier.VeryLow } },
             { Instruction.DUP9,          new int[] { 0,     9,   10,  (int)Tier.VeryLow } },
             { Instruction.DUP10,         new int[] { 0,    10,   11,  (int)Tier.VeryLow } },
             { Instruction.DUP11,         new int[] { 0,    11,   12,  (int)Tier.VeryLow } },
             { Instruction.DUP12,         new int[] { 0,    12,   13,  (int)Tier.VeryLow } },
             { Instruction.DUP13,         new int[] { 0,    13,   14,  (int)Tier.VeryLow } },
             { Instruction.DUP14,         new int[] { 0,    14,   15,  (int)Tier.VeryLow } },
             { Instruction.DUP15,         new int[] { 0,    15,   16,  (int)Tier.VeryLow } },
             { Instruction.DUP16,         new int[] { 0,    16,   17,  (int)Tier.VeryLow } },
             { Instruction.SWAP1,         new int[] { 0,     2,    2,  (int)Tier.VeryLow } },
             { Instruction.SWAP2,         new int[] { 0,     3,    3,  (int)Tier.VeryLow } },
             { Instruction.SWAP3,         new int[] { 0,     4,    4,  (int)Tier.VeryLow } },
             { Instruction.SWAP4,         new int[] { 0,     5,    5,  (int)Tier.VeryLow } },
             { Instruction.SWAP5,         new int[] { 0,     6,    6,  (int)Tier.VeryLow } },
             { Instruction.SWAP6,         new int[] { 0,     7,    7,  (int)Tier.VeryLow } },
             { Instruction.SWAP7,         new int[] { 0,     8,    8,  (int)Tier.VeryLow } },
             { Instruction.SWAP8,         new int[] { 0,     9,    9,  (int)Tier.VeryLow } },
             { Instruction.SWAP9,         new int[] { 0,    10,   10,  (int)Tier.VeryLow } },
             { Instruction.SWAP10,        new int[] { 0,    11,   11,  (int)Tier.VeryLow } },
             { Instruction.SWAP11,        new int[] { 0,    12,   12,  (int)Tier.VeryLow } },
             { Instruction.SWAP12,        new int[] { 0,    13,   13,  (int)Tier.VeryLow } },
             { Instruction.SWAP13,        new int[] { 0,    14,   14,  (int)Tier.VeryLow } },
             { Instruction.SWAP14,        new int[] { 0,    15,   15,  (int)Tier.VeryLow } },
             { Instruction.SWAP15,        new int[] { 0,    16,   16,  (int)Tier.VeryLow } },
             { Instruction.SWAP16,        new int[] { 0,    17,   17,  (int)Tier.VeryLow } },
             { Instruction.LOG0,          new int[] { 0,     2,    0,  (int)Tier.Special } },
             { Instruction.LOG1,          new int[] { 0,     3,    0,  (int)Tier.Special } },
             { Instruction.LOG2,          new int[] { 0,     4,    0,  (int)Tier.Special } },
             { Instruction.LOG3,          new int[] { 0,     5,    0,  (int)Tier.Special } },
             { Instruction.LOG4,          new int[] { 0,     6,    0,  (int)Tier.Special } },

             { Instruction.JUMPTO,        new int[] { 2,     1,    0,  (int)Tier.VeryLow } },
             { Instruction.JUMPIF,        new int[] { 2,     2,    0,  (int)Tier.Low } },
             { Instruction.JUMPV,         new int[] { 2,     1,    0,  (int)Tier.Mid } },
             { Instruction.JUMPSUB,       new int[] { 2,     1,    0,  (int)Tier.Low } },
             { Instruction.JUMPSUBV,      new int[] { 2,     1,    0,  (int)Tier.Mid } },
             { Instruction.BEGINSUB,      new int[] { 0,     0,    0,  (int)Tier.Special } },
             { Instruction.BEGINDATA,     new int[] { 0,     0,    0,  (int)Tier.Special } },
             { Instruction.RETURNSUB,     new int[] { 0,     1,    0,  (int)Tier.Mid } },
             { Instruction.PUTLOCAL,      new int[] { 2,     1,    0,  (int)Tier.VeryLow } },
             { Instruction.GETLOCAL,      new int[] { 2,     0,    1,  (int)Tier.VeryLow } },

             { Instruction.XADD,          new int[] { 1,     0,    0,  (int)Tier.Special } },
             { Instruction.XMUL,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XSUB,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XDIV,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XSDIV,         new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XMOD,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XSMOD,         new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XLT,           new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XGT,           new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XSLT,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XSGT,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XEQ,           new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XISZERO,       new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XAND,          new int[] { 1,     1,    1,  (int)Tier.Special } },
             //{ Instruction.XOR,           new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XXOR,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XNOT,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XSHL,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XSHR,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XSAR,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XROL,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XROR,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XPUSH,         new int[] { 1,     1,    1,  (int)Tier.VeryLow } },
             { Instruction.XMLOAD,        new int[] { 1,     1,    1,  (int)Tier.VeryLow } },
             { Instruction.XMSTORE,       new int[] { 1,     2,    0,  (int)Tier.VeryLow } },
             { Instruction.XSLOAD,        new int[] { 1,     1,    1,  (int)Tier.Special } },
             { Instruction.XSSTORE,       new int[] { 1,     2,    0,  (int)Tier.Special } },
             { Instruction.XVTOWIDE,      new int[] { 1,     1,    1,  (int)Tier.VeryLow } },
             { Instruction.XWIDETOV,      new int[] { 1,     1,    1,  (int)Tier.VeryLow } },
             { Instruction.XPUT,          new int[] { 1,     3,    1,  (int)Tier.Special } },
             { Instruction.XGET,          new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XSWIZZLE,      new int[] { 1,     2,    1,  (int)Tier.Special } },
             { Instruction.XSHUFFLE,      new int[] { 1,     3,    1,  (int)Tier.Special } },
             { Instruction.CREATE,        new int[] { 0,     3,    1,  (int)Tier.Special } },
             { Instruction.CREATE2,       new int[] { 0,     4,    1,  (int)Tier.Special } },
             { Instruction.CALL,          new int[] { 0,     7,    1,  (int)Tier.Special } },
             { Instruction.CALLCODE,      new int[] { 0,     7,    1,  (int)Tier.Special } },
             { Instruction.RETURN,        new int[] { 0,     2,    0,  (int)Tier.Zero } },
             { Instruction.STATICCALL,    new int[] { 0,     6,    1,  (int)Tier.Special } },
             { Instruction.DELEGATECALL,  new int[] { 0,     6,    1,  (int)Tier.Special } },
             { Instruction.REVERT,        new int[] { 0,     2,    0,  (int)Tier.Special } },
             { Instruction.INVALID,       new int[] { 0,     0,    0,  (int)Tier.Zero    } },
             { Instruction.SUICIDE,       new int[] { 0,     1,    0,  (int)Tier.Special } },
 
	        // these are generated by the interpreter - should never be in user code
	         { Instruction.PUSHC,         new int[] { 3,     0,    1, (int)Tier.VeryLow } },
             { Instruction.JUMPC,         new int[] { 0,     1,    0, (int)Tier.Mid } },
             { Instruction.JUMPCI,        new int[] { 0,     2,    0, (int)Tier.High } }
        };
    }
}
