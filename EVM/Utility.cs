using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVM
{
    public static class Utility
    {
        public static byte[] parseHexString(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Remove(0, 2);
            if (hex.Length % 2 != 0)
                hex = "0" + hex;
            byte[] ret = new byte[hex.Length / 2];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            return ret;
        }
    }
}
