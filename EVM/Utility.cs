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

        public static string ToString(this byte[] bytes, bool leadingZeros)
        {
            StringBuilder str = new StringBuilder();
            bool nonZeroMet = false;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (leadingZeros)
                    str.Append(bytes[i].ToString("x2"));
                else
                {
                    if (bytes[i] != 0x00)
                        nonZeroMet = true;
                    else if (!nonZeroMet)
                        continue;
                    str.Append(bytes[i].ToString("x2"));
                }
            }
            return str.ToString();
        }

        public static string ToHex(this int i)
        {
            byte[] bytes = BitConverter.GetBytes(i);
            Array.Reverse(bytes);
            return bytes.ToString(true);
        }
    }
}
