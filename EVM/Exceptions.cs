using System;

namespace EVM.Exceptions
{
    public class EVMNoInstruction : Exception { }
    public class EVMInstructionNotYetSupported : Exception { }
    public class EVMEIPNotYetSupported : Exception { }
    public class EVMNotJumpDestException : Exception { }
}
