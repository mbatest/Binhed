using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utils
{
    /// <summary>
    /// Reads a large file in a bytewise manner, handling bid endianness
    /// </summary>
    public class BinaryFileReader : FileStream
    {
        bool littleEndian = true;
        public BinaryFileReader(string path, FileMode fm, FileAccess fa, FileShare ff, bool isLittleEndian)
            : base(path, fm, fa, ff)
        {
            littleEndian = isLittleEndian;
        }
        public BinaryFileReader(string path, bool isLittleEndian)
            : base(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        {
            littleEndian = isLittleEndian;
        }
        public string ReadString(int length)
        {
            return Encoding.Default.GetString(ReadBytes(length));
        }
        public string ReadString(int length, Encoding enc)
        {
            return enc.GetString(ReadBytes(length));
        }
        public string ReadStringZ(Encoding eng)
        {
            List<byte> bytes = new List<byte>();
            if (eng == Encoding.Unicode)
            {
                byte[] a = ReadBytes(2);
                while (a[0] != 0)
                {
                    bytes.Add(a[0]);
                    bytes.Add(a[1]);
                    a = ReadBytes(2);
                }
            }
            else
            {
                byte[] a = ReadBytes(1);
                while (a[0] != 0)
                {
                    bytes.Add(a[0]);
                    a = ReadBytes(1);
                }
            }
            return eng.GetString(bytes.ToArray());
        }
        /// <summary>
        /// Read short integer (int16)
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            return (short)BytesToInteger(ReadBytes(2));
        }
        public int ReadInteger()
        {
            return (int)BytesToInteger(ReadBytes(4));
        }
        public long ReadLongInteger()
        {
            return BytesToInteger(ReadBytes(8));
        }
        public byte[] ReadBytes(int length)
        {
            byte[] buffer = new byte[length];
            Read(buffer, 0, buffer.Length);
            return buffer;
        }
        public int ReadIndex(int offType)
        {
            switch (offType)
            {
                case 2: return ReadShort();
                case 4: return ReadInteger();
                default: return 0;
            }
        }
        public int FromBCD()
        {
            byte b = (byte)ReadByte();
            int digit1 = b >> 4;
            int digit2 = b & 0x0f;
            return digit1 * 10 + digit2;
        }
        public long BytesToInteger(byte[] c)
        {
            ulong data = 0;
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
            return (long)data;
        }
    }
}
