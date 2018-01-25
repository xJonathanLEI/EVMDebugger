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
            uint256 a = 4;
            uint256 b = 5;
            uint256 c = a + b;

            // PUSH1 0x05, PUSH1 0x04, RETURN
            EVMInterpreter evm = new EVMInterpreter(new byte[] { 0x60, 0x05, 0x60, 0x04, 0xf3 });

            ShowEVMStatus(evm);

            while (evm.executeOnce())
                ShowEVMStatus(evm);

            ShowEVMStatus(evm);

            StringBuilder returnStr = new StringBuilder();
            for (int i = 0; i < evm.returnData.Length; i++)
                returnStr.Append(evm.returnData[i].ToString("x2") + (i == evm.returnData.Length - 1 ? "" : ", "));
            Console.WriteLine(string.Format("Return data: [" + returnStr.ToString() + "]"));
        }

        private void ShowEVMStatus(EVMInterpreter evm)
        {
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
            for (int i = 0; i < evm.memory.Count / 32; i ++)
            {
                StringBuilder memorySlot = new StringBuilder();
                for (int offset = 0; offset < 32; offset++)
                    memorySlot.Append(evm.memory[i + offset].ToString("x2") + (offset == 31 ? "" : " "));
                memory.Append(string.Format("0x{0}: \t{1}\r\n", i.ToHex(), memorySlot.ToString()));
            }

            Console.WriteLine(string.Format("==========EVM Status==========\r\nPC:\t{0}\r\nStack:\t{1}\r\nMemory:\r\n{2}", evm.pc, stack.ToString(), memory.ToString()));
        }
    }
}
