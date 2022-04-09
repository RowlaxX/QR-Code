using Bitmap;
using System;

namespace MyProject
{
    class Utils
    {
        //Write
        public static int Write(bool[] source, bool[] destination, int index)
        {
            for (int i = 0; i < source.Length; i++)
                destination[index + i] = source[i];
            return source.Length;
        }
        public static int WriteEndiannessBits(long n, bool[] array, int index, int length)
        {
            int j = index + length - 1;
            for (int i = 0; i < length; i++)
            {
                array[j - i] = n % 2 == 1;
                n /= 2;
            }
            return length;
        }
        public static bool[] GetEndiannessBits(long n, int length)
        {
            bool[] array = new bool[length];
            WriteEndiannessBits(n, array, 0, length);
            return array;
        }
        public static void WriteEndianness(uint n, int index, byte[] array)
        {
            for (int i = 0; i < 4; i++)
            {
                array[index + i] = (byte)(n % 256);
                n /= 256;
            }
        }
        public static void WriteEndianness(ushort n, int index, byte[] array)
        {
            for (int i = 0; i < 2; i++)
            {
                array[index + i] = (byte)(n % 256);
                n /= 256;
            }
        }

        //Read
        public static long ReadEndiannessBits(bool[] array, int index, int length)
        {
            long n = 0;
            long pow = 1;
            int j = index + length - 1;
            for (int i = 0; i < length; i++)
            {
                n += (array[j - i] ? 1 : 0) * pow;
                pow *= 2;
            }
            return n;
        }

        public static long ReadEndiannessBits(bool[] array)
        {
            return ReadEndiannessBits(array, 0, array.Length);
        }


        public static uint ReadUIntEndianness(int index, byte[] array)
        {
            uint n = 0;
            uint exp = 1;
            for (int i = 0; i < 4; i++)
            {
                n += array[index + i] * exp;
                exp *= 256;
            }
            return n;
        }

        public static ushort ReadUShortEndianness(int index, byte[] array)
        {
            ushort n = 0;
            ushort exp = 1;
            for (int i = 0; i < 2; i++)
            {
                n += (ushort)(array[index + i] * exp);
                exp *= 256;
            }
            return n;
        }

        //Others
        public static int FastLog2(long value)
        {
            if (value < 0)
                return 0;
            if (value == 0)
                return 1;

            long pow = 1;
            int i = 0;
            while ( pow <= value)
            {
                pow <<= 1;
                i++;
            }
            return i;
        }
    }
}