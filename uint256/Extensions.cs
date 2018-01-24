using System;
using Uint256.Exceptions;

namespace Uint256
{
    public static class BytesExtensions
    {
        public static bool IsZero(this byte[] bytes)
        {
            foreach (byte b in bytes)
                if (b != 0)
                    return false;
            return true;
        }

        public static byte[] LeftPadTo(this byte[] bytes, int desiredLength)
        {
            byte[] ret = new byte[desiredLength];
            Array.Copy(bytes, 0, ret, desiredLength - bytes.Length, bytes.Length);
            return ret;
        }

        public static byte[] AND(this byte[] leftOperand, byte[] rightOperand)
        {
            if (leftOperand.Length != rightOperand.Length)
                throw new BytesANDException();
            byte[] ret = new byte[leftOperand.Length];
            for(int i = 0; i < leftOperand.Length; i ++)
                ret[i] = (byte)(leftOperand[i] & rightOperand[i]);
            return ret;
        }

        public static byte[] OR(this byte[] leftOperand, byte[] rightOperand)
        {
            if (leftOperand.Length != rightOperand.Length)
                throw new BytesANDException();
            byte[] ret = new byte[leftOperand.Length];
            for (int i = 0; i < leftOperand.Length; i++)
                ret[i] = (byte)(leftOperand[i] | rightOperand[i]);
            return ret;
        }

        public static byte[] XOR(this byte[] leftOperand, byte[] rightOperand)
        {
            if (leftOperand.Length != rightOperand.Length)
                throw new BytesANDException();
            byte[] ret = new byte[leftOperand.Length];
            for (int i = 0; i < leftOperand.Length; i++)
                ret[i] = (byte)(leftOperand[i] ^ rightOperand[i]);
            return ret;
        }

        public static byte[] NOT(this byte[] leftOperand)
        {
            byte[] ret = new byte[leftOperand.Length];
            for (int i = 0; i < leftOperand.Length; i++)
                ret[i] = (byte)(leftOperand[i] ^ 0xff);
            return ret;
        }

        public static byte[] LEFTSHIFT(this byte[] leftOperand, int bits)
        {
            for (int i = 0; i < bits; i++)
            {
                bool overflow = false;
                for (int j = leftOperand.Length - 1; j >= 0; j--)
                {
                    byte oriByte = leftOperand[j];
                    leftOperand[j] = (byte)(leftOperand[j] << 1);
                    if (overflow)
                        leftOperand[j] += 0b1;
                    overflow = oriByte > 0b01111111;
                }
            }
            return leftOperand;
        }

        public static byte[] RIGHTSHIFT(this byte[] leftOperand, int bits)
        {
            for (int i = 0; i < bits; i++)
            {
                bool overflow = false;
                for (int j = 0; j < leftOperand.Length; j++)
                {
                    byte oriByte = leftOperand[j];
                    leftOperand[j] = (byte)(leftOperand[j] >> 1);
                    if (overflow)
                        leftOperand[j] += 0b10000000;
                    overflow = (oriByte % 2) == 1;
                }
            }
            return leftOperand;
        }
    }
}