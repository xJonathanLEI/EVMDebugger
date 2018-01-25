using System;
using Uint256.Exceptions;

namespace Uint256
{
    public static class BytesExtensions
    {
        // 0x0->0   0x1001->4
        public static int mostSignificantOne(this byte[] bytes)
        {
            for(int i = 0; i < bytes.Length; i ++)
                if (bytes[i] != 0)
                {
                    int currentSig = 0;
                    if (bytes[i] > 0b01111111)
                        currentSig = 8;
                    else if (bytes[i] > 0b00111111)
                        currentSig = 7;
                    else if (bytes[i] > 0b00011111)
                        currentSig = 6;
                    else if (bytes[i] > 0b00001111)
                        currentSig = 5;
                    else if (bytes[i] > 0b00000111)
                        currentSig = 4;
                    else if (bytes[i] > 0b00000011)
                        currentSig = 3;
                    else if (bytes[i] > 0b00000001)
                        currentSig = 2;
                    else if (bytes[i] > 0b00000000)
                        currentSig = 1;
                    return (bytes.Length - i - 1) * 8 + currentSig;
                }
            return 0;
        }

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