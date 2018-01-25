/**
 * Author: Jonathan LEI
 * Email: xJonathan@outlook.com
 * Github: https://github.com/xJonathanLEI
 * Created: Jan 2018
 * Last Updated: Jan 2018
 * Introduction:
 * - A simble big-endian implementation of unsigned 256-bit integers in C#
 * - The code is used in EVM
**/

using System;
using System.Text;
using Uint256.Exceptions;

namespace Uint256
{
    public class uint256
    {
        private readonly byte[] m_bytes;

        private const int VALUE_SIZE = 32;

        public byte[] bytes { get { return m_bytes; } }

        public uint256()
        {
            m_bytes = new byte[VALUE_SIZE];
        }

        public uint256(byte[] bytes)
        {
            if (bytes.Length > VALUE_SIZE)
                throw new Uint256ConstructorOverflow();
            m_bytes = bytes.LeftPadTo(VALUE_SIZE);
        }

        public uint256 exp(uint256 exponent)
        {
            uint256 ret = 1;
            for(uint256 i = 0; i < exponent; i ++)
                ret *= this;
            return ret;
        }

        public uint ToUInt32(bool ignoreOverflow = false)
        {
            if (!ignoreOverflow && this > 0xffffffff)
                throw new Uint256ConversionOverflow();
            byte[] bys = new byte[sizeof(uint)];
            for (int i = 0; i < bys.Length; i ++)
                bys[i] = bytes[VALUE_SIZE - i - 1];
            return BitConverter.ToUInt32(bys, 0);
        }

        /// <summary>
        /// Express the uint256 value in hex.
        /// The length of the string will ALWAYS be an integral multiple of 2.
        /// </summary>
        /// <param name="leadingZeros">Whether the leading zero BYTES are displayed</param>
        /// <returns>The value in hex</returns>
        public string ToString(bool leadingZeros)
        {
            StringBuilder str = new StringBuilder();
            bool nonZeroMet = false;
            for (int i = 0; i < VALUE_SIZE; i++)
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
            return str.Length == 0 ? "00" : str.ToString();
        }

        /// <summary>
        /// Express the uint256 value in hex, with no leading zeros.
        /// The string returned will never start with a zero, unless the whole value itself is zero.
        /// </summary>
        /// <returns>The value in hex</returns>
        public string ToStringMinimal()
        {
            StringBuilder str = new StringBuilder();
            bool nonZeroMet = false;
            bool anyByteAppended = false;
            for (int i = 0; i < VALUE_SIZE; i++)
            {
                if (bytes[i] != 0x00)
                    nonZeroMet = true;
                else if (!nonZeroMet)
                    continue;
                if (anyByteAppended)
                    str.Append(bytes[i].ToString("x2"));
                else
                    str.Append(bytes[i].ToString("x"));
                anyByteAppended = true;
            }
            return str.Length == 0 ? "0" : str.ToString();
        }

        // Implicit conversion from int to uint256
        public static implicit operator uint256(int sint)
        {
            /**
             * We assume they mean to use unsigned-int.
             * Say, when you write uint256 a = 1, you really mean [0x00, ..., 0x01], not some freaky huge numbers.
            **/
            byte[] bytes = BitConverter.GetBytes(sint);
            // Little endian used by BitConverter
            Array.Reverse(bytes);
            return new uint256(bytes);
        }

        public static implicit operator uint256(long slong)
        {
            byte[] bytes = BitConverter.GetBytes(slong);
            Array.Reverse(bytes);
            return new uint256(bytes);
        }

        public static implicit operator uint256(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Remove(0, 2);
            if (hex.Length % 2 != 0)
                hex = "0" + hex;
            byte[] ret = new byte[hex.Length / 2];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            return new uint256(ret);
        }

        public static bool operator ==(uint256 leftOperand, uint256 rightOperand)
        {
            for (int i = 0; i < VALUE_SIZE; i++)
                if (leftOperand.bytes[i] != rightOperand.bytes[i])
                    return false;
            return true;
        }

        public static bool operator !=(uint256 leftOperand, uint256 rightOperand)
        {
            return !(leftOperand == rightOperand);
        }

        public static uint256 operator +(uint256 leftOperand, uint256 rightOperand)
        {
            uint256 carry = leftOperand & rightOperand;
            uint256 result = leftOperand ^ rightOperand;
            while (carry != 0)
            {
                uint256 shiftedCarry = carry << 1;
                carry = result & shiftedCarry;
                result = result ^ shiftedCarry;
            }
            return result;
        }

        public static uint256 operator -(uint256 leftOperand, uint256 rightOperand)
        {
            uint256 carry = (!leftOperand) & rightOperand;
            uint256 result = leftOperand ^ rightOperand;
            while (carry != 0)
            {
                uint256 shiftedCarry = carry << 1;
                carry = (!result) & shiftedCarry;
                result = result ^ shiftedCarry;
            }
            return result;
        }

        public static uint256 operator *(uint256 leftOperand, uint256 rightOperand)
        {
            uint256 ret = 0;
            for (uint256 i = 0; i < rightOperand; i++)
                ret += leftOperand;
            return ret;
        }

        public static uint256 operator /(uint256 leftOperand, uint256 rightOperand)
        {
            if (rightOperand == 0) throw new Uint256Division0();
            uint256 ret = 0;
            while (leftOperand >= rightOperand)
            {
                leftOperand -= rightOperand;
                ret++;
            }
            return ret;
        }

        public static uint256 operator %(uint256 leftOperand, uint256 rightOperand)
        {
            if (rightOperand == 0) throw new Uint256Division0();
            while (leftOperand >= rightOperand)
                leftOperand -= rightOperand;
            return leftOperand;
        }

        public static uint256 operator <<(uint256 leftOperand, int bits)
        {
            return new uint256(leftOperand.bytes.LEFTSHIFT(bits));
        }

        public static uint256 operator >>(uint256 leftOperand, int bits)
        {
            return new uint256(leftOperand.bytes.RIGHTSHIFT(bits));
        }

        public static uint256 operator &(uint256 leftOperand, uint256 rightOperand)
        {
            return new uint256(leftOperand.bytes.AND(rightOperand.bytes));
        }

        public static uint256 operator |(uint256 leftOperand, uint256 rightOperand)
        {
            return new uint256(leftOperand.bytes.OR(rightOperand.bytes));
        }

        public static uint256 operator ^(uint256 leftOperand, uint256 rightOperand)
        {
            return new uint256(leftOperand.bytes.XOR(rightOperand.bytes));
        }

        public static uint256 operator !(uint256 leftOperand)
        {
            return new uint256(leftOperand.bytes.NOT());
        }

        public static bool operator <(uint256 leftOperand, uint256 rightOperand)
        {
            for (int i = 0; i < leftOperand.bytes.Length; i++)
                if (leftOperand.bytes[i] < rightOperand.bytes[i])
                    return true;
                else if(leftOperand.bytes[i] > rightOperand.bytes[i])
                    return false;
            return false;
        }

        public static bool operator >(uint256 leftOperand, uint256 rightOperand)
        {
            for (int i = 0; i < leftOperand.bytes.Length; i++)
                if (leftOperand.bytes[i] > rightOperand.bytes[i])
                    return true;
                else if (leftOperand.bytes[i] < rightOperand.bytes[i])
                    return false;
            return false;
        }

        public static bool operator <=(uint256 leftOperand, uint256 rightOperand)
        {
            return !(leftOperand > rightOperand);
        }

        public static bool operator >=(uint256 leftOperand, uint256 rightOperand)
        {
            return !(leftOperand < rightOperand);
        }

        public static uint256 operator ++(uint256 leftOperand)
        {
            return leftOperand + 1;
        }
    }
}
