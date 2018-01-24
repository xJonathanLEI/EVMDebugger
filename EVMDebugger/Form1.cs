using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVM:
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

        private void button1_Click(object sender, EventArgs e)
        {
            EVMInterpreter evm = new EVMInterpreter(new byte[] { 0x60, 0x60 });
        }
    }
}
