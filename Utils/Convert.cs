using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utils
{
    /// <summary>
    /// Necessary to handle big endianness
    /// </summary>
    public class BufferConvert
    {
        public static bool littleEndian = true;
        public static string ByteToString(byte[] buffer)
        {
            string ret = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                ret = ret + (char)buffer[i];
            }
            return ret;
        }
        public static string ByteToHex(byte[] buffer)
        {
            string ret = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                ret = ret + buffer[i].ToString("x2");
            }
            return ret;
        }
        public static int ByteToInteger(byte[] c)
        {
            uint data = 0;
            if (littleEndian)
            {
                for (int w = 0; w < c.Length; w++)
                {
                    data = 256 * data + (uint)c[c.Length - 1 - w];
                }
            }
            else
            {
                for (int w = c.Length - 1; w >= 0; w--)
                {
                    data = 256 * data + (uint)c[c.Length - 1 - w];
                }
            }
            return (int)data;
        }
        public static long ByteToLong(byte[] c)
        {
            long taille = 0;
            if (littleEndian)
            {
                for (int w = 0; w < c.Length; w++)
                {
                    taille = 256 * taille + (uint)c[c.Length - 1 - w];
                }
            }
            else
            {
                for (int w = c.Length - 1; w >= 0; w--)
                {
                    taille = 256 * taille + (uint)c[c.Length - 1 - w];
                }
            }
            return taille;
        }
        public static int ByteToShortInteger(byte[] c)
        {
            uint taille = 0;
            if (littleEndian)
            {
                taille = 256 * (uint)c[1] + (uint)c[0];
            }
            else
            {
                taille = 256 * (uint)c[0] + (uint)c[1];
            }
            return (int)taille;
        }
        /// <summary>
        /// Reads an integer in the buffer
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="add">start adress</param>
        /// <returns></returns>
        public static int ReadInteger(byte[] buffer, int add)
        {
            byte[] n = new byte[4];
            Buffer.BlockCopy(buffer, add, n, 0, 4);
            int nn = BufferConvert.ByteToInteger(n);
            return nn;
        }
        /// <summary>
        /// Reads a short integer from the buffer
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="add">start adress</param>
        /// <returns></returns>
        public static int ReadShortInteger(byte[] buffer, int add)
        {
            byte[] n = new byte[2];
            Buffer.BlockCopy(buffer, add, n, 0, 2);
            int nn = BufferConvert.ByteToInteger(n);//offset to clip data
            return nn;
        }
        /// <summary>
        /// Reads a byte integer from the buffer
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="add">start adress</param>
        /// <returns></returns>
        public static int ReadByteInteger(byte[] buffer, int add)
        {
             return buffer[add];
        }
        public static string ReadString(byte[] buffer, int add, int length)
        {
            byte[] n = new byte[length];
            Buffer.BlockCopy(buffer, add, n, 0, length);
            string nn = Encoding.Default.GetString(n);
            return nn;
        }
        public static int FromBCD(byte b)
        {
            int digit1 = b >> 4;
            int digit2 = b & 0x0f;
            return digit1 * 10 + digit2;
        }
        private static byte[] FillBuffer(FileStream sw,long length)
        {
            byte[] buffer = new byte[length];
            sw.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}