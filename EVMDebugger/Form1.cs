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
            EVM.DataQuery.EtherscanDataGateway edg = new EVM.DataQuery.EtherscanDataGateway();
            uint256 u = await edg.getBlockHashByHeight(4968586);

            return;
            string strBase = "00000000000000000000000015ab2321d7e83d00c015048b567f4f6aadc1b022000000000000000000000000000000000000000000000000000000000000000";
            for (int i = 0; i < 6; i ++)
            {
                SHA3.SHA3Managed sha3 = new SHA3.SHA3Managed(256);
                byte[] hash = sha3.ComputeHash(Utility.parseHexString(strBase + i.ToString()));
                string ori = strBase + i.ToString();
                string h = "";
                foreach (byte b in hash)
                    h += b.ToString("x2");
                string url = "https://api.etherscan.io/api?module=proxy&action=eth_getStorageAt&address=0xD850942eF8811f2A866692A623011bDE52a462C1&position=0x" + h + "&tag=latest";
                System.Net.Http.HttpClient hc = new System.Net.Http.HttpClient();
                Console.WriteLine(string.Format("{0}:\r\n{1}\r\n{2}\r\n{3}\r\n\r\n", ori, h, url, await hc.GetStringAsync(url)));
            }
        }
    }
}
