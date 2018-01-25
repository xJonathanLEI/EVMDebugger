using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVM;
using EVM.Structures;
using Uint256;

namespace EVMDebugger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            uint256 a = 1024;
            uint256 b = 2;
            uint256 c = a / b;
            int d = (int)c.ToUInt32();

            EVM.DataQuery.EtherscanDataGateway edg = new EVM.DataQuery.EtherscanDataGateway();
            byte[] bytecode = await edg.getCodeAt("0x5d1B26d762b1973B8B7C2bFb196Ba2ED969dAF18");

            // PUSH1 0x05, PUSH1 0x04, RETURN
            EVMInterpreter evm = new EVMInterpreter(bytecode);
            evm.transaction.data = Utility.parseHexString("0x6ffcc719000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000ed6fa3c7");

            ShowEVMStatus(evm);

            while (await evm.executeOnce())
                ShowEVMStatus(evm);

            ShowEVMStatus(evm);

            StringBuilder returnStr = new StringBuilder();
            for (int i = 0; i < evm.returnData.Length; i++)
                returnStr.Append(evm.returnData[i].ToString("x2") + (i == evm.returnData.Length - 1 ? "" : ", "));
            Console.WriteLine(string.Format("Return data: [" + returnStr.ToString() + "]"));
        }

        private void ShowEVMStatus(EVMInterpreter evm)
        {
            // PC
            int instructAdd = EVM.EVMInterpreter.instructionInfos[(EVMInterpreter.Instruction)evm.byteCode[evm.pc]][0];
            StringBuilder pcPar = new StringBuilder();
            if (instructAdd > 0)
                pcPar.Append("\t0x");
            for (int i = 0; i < instructAdd; i ++)
                pcPar.Append(evm.byteCode[evm.pc + i + 1].ToString("x2"));

            // Stack
            StringBuilder stack = new StringBuilder();
            stack.Append("[");
            for (int i = 0; i < evm.stack.Count; i++)
                stack.Append(evm.stack[i].ToString(false) + (i == evm.stack.Count - 1 ? "" : ", "));
            stack.Append("]");

            // Memory
            if (evm.memory.Count % 32 != 0)
                throw new Exception("EVM memory size error");

            StringBuilder memory = new StringBuilder();
            for (int i = 0; i < evm.memory.Count / 16; i ++)
            {
                StringBuilder memorySlot = new StringBuilder();
                for (int offset = 0; offset < 16; offset++)
                    memorySlot.Append(evm.memory[i * 16 + offset].ToString("x2") + (offset == 15 ? "" : " "));
                memory.Append(string.Format("0x{0}: \t{1}\r\n", (i * 16).ToHex(), memorySlot.ToString()));
            }

            Console.WriteLine(string.Format("==========EVM Status==========\r\nPC:\t0x{0}\t{1}\r\nStack:\t{2}\r\nMemory:\r\n{3}", evm.pc.ToHex(false), ((EVM.EVMInterpreter.Instruction)evm.byteCode[evm.pc]).ToString() + pcPar.ToString(), stack.ToString(), memory.ToString()));
        }
    }
}
