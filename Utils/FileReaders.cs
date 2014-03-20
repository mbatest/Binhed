using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;

namespace Utils
{
    /*
    public class BitStreamReader2 : BinaryReader
    {
        private long length;
        #region Properties
        public long Length
        {
            get { return BaseStream.Length; }
            set { length = value; }
        }
        public long BitPosition
        {
            get { return currentBit; }
            set { currentBit = value; }
        }
        public long Position
        {
            get { return currentBit / 8; }
            set { currentBit = value * 8; }
        }
        #endregion
        /// <summary>
        ///Initiates a binary reader to parse data in a byte array
        /// </summary>
        /// <param name="buffer">data to be parsed</param>
        /// <param name="littleEndian">True if integer are coded in little endian</param>
        /// <summary>
        /// Bitwise reading of a filestream
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fm"></param>
        /// <param name="fa"></param>
        /// <param name="ff"></param>
        /// <param name="isBigEndian"></param>
        public BitStreamReader2(string FileName, bool littleEndian)
            : base(File.Open(FileName, FileMode.Open))
        {
            this.littleEndian = littleEndian;
            currentBit = 0;
        }
        /// <summary>
        /// Align along the next byte
        /// </summary>
        public void Align()
        {
            if (currentBit % 8 != 0)
                currentBit += (8 - currentBit % 8);
        }
        public bool Eof { get { return Position >= Length; } }
        public void Close()
        {
            //          fs.Close();
        }
        public long FindBinaryPattern(string pattern, long start)
        {
            char[] d = pattern.ToCharArray();
            while (currentBit < Length)
            {
                Position = (int)start;
                for (long i = start; i < Length; i++)
                {
                    string a = ReadBit().ToString();
                    if (a == d[0].ToString())
                    {
                        bool found = true;
                        long startPattern = currentBit;
                        for (int k = 1; k < d.Length; k++)
                        {
                            a = ReadBit().ToString();
                            if (currentBit >= Length || d[k].ToString() != a)
                            {
                                found = false;
                                currentBit = (int)startPattern + 1;
                                break;
                            }
                        }
                        if (found)
                        {
                            return currentBit - d.Length + 1;
                        }
                    }
                }
            }
            return -1;
        }
        #region bit reading
        /// <summary>
        /// Reads a bit at a given index
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>value</returns>
        public int ReadBit(long index)
        {
            int value = 0;
            long byteIndex = index / 8;
            int bitIndex = (int)(index % 8);
            try
            {
                base.BaseStream.Position = byteIndex;
                value = base.ReadByte() & BitMask[bitIndex];
                value = value >> (7 - bitIndex);
            }
            catch { }
            return value;
        }
        /// <summary>
        /// Returns the current bit
        /// </summary>
        /// <returns></returns>
        public int ReadBit()
        {
            int value = ReadBit(currentBit);
            currentBit++;
            return value;
        }
        /// <summary>
        /// Reads a range of bits starting from the current position
        /// </summary>
        /// <param name="length">Number of bits</param>
        /// <returns></returns>
        public List<int> ReadBitRange(int length)
        {
            List<int> bits = new List<int>();
            long end = currentBit + length;
            while (currentBit < end)
            {
                bits.Add(ReadBit());
            }
            return bits;
        }
        /// <summary>
        /// Reads a range of bit
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        /// <returns></returns>
        public List<int> ReadBitRange(int start, int end)
        {
            List<int> bits = new List<int>();
            for (int w = start; w < end; w++)
            {
                bits.Add(ReadBit(w));
            }
            return bits;
        }
        /// <summary>
        /// Reads a range of bits as booleans starting from current bit position
        /// </summary>
        /// <param name="length">Number of bits</param>
        /// <returns></returns>
        public List<bool> ReadBools(int length)
        {
            List<bool> bits = new List<bool>();
            long end = currentBit + length;
            while (currentBit < end)
            {
                bits.Add(ReadBit() == 1);
            }
            return bits;
        }
        /// <summary>
        /// Reads current bit as a boolean
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            return ReadBit() == 1;
        }
        public int RangeToInt(List<int> bits)
        {
            int i = 0;
            for (int k = 0; k < bits.Count; k++)
            {
                i = (i * 2 + bits[k]);
            }
            return i;
        }
        public int ReadLongIntegerFromBits()
        {
            return ReadIntFromBits(8);
        }
        public int ReadShortIntegerFromBits()
        {
            return ReadIntFromBits(2);
        }
        /// <summary>
        /// Reads an integer starting from current bit position
        /// </summary>
        /// <param name="length">number of bits</param>
        /// <returns>integer read</returns>
        public int ReadIntFromBits(int length)
        {
            List<int> bytes = ReadBitRange(length);
            int i = 0;
            if (littleEndian)
            {
                for (int k = 0; k < length; k++)
                {
                    i = (i * 2 + bytes[k]);
                }
            }
            else
            {
                for (int k = length - 1; k >= 0; k--)
                {
                    i = (i * 2 + bytes[k]);
                }
            }
            return i;
        }
        /// <summary>
        /// Reads an integer starting from current bit position
        /// </summary>
        /// <param name="length">number of bits</param>
        /// <returns>integer read</returns>
        public ulong ReadLongIntegerFromBits(int length)
        {
            List<int> bits = ReadBitRange(length);
            ulong i = 0;
            if (littleEndian)
            {
                for (int k = 0; k < length; k++)
                {
                    i = (i * 2 + (ulong)bits[k]);
                }
            }
            else
            {
                for (int k = length - 1; k >= 0; k--)
                {
                    i = (i * 2 + (ulong)bits[k]);
                }
            }
            return i;
        }
        /// <summary>
        /// Reads an integer encoded in exponential Golomb code
        /// </summary>
        /// <returns></returns>
        public int ReadGolomb()
        {
            int st = ReadBit();

            if (st == 1)
                return 0;
            else
            {
                List<int> st2 = ReadBitRange(2);
                if (st2[0] == 1)
                {
                    int a = RangeToInt(st2);
                    return a - 1;
                }
                else
                {
                    List<int> st3 = ReadBitRange(3);
                    if (st3[0] == 1)
                    {
                        return RangeToInt(st3) - 1;
                    }
                    else
                    {
                        List<int> st4 = ReadBitRange(4);
                        if (st4[0] == 1)
                        {
                            return RangeToInt(st4) - 1;
                        }
                        else
                        {
                        }

                    }
                }
                return 0;

            }
        }
        private int ue() // Golomb
        {
            int r = 0;
            int i = 0;

            while (ReadBit() == 0 && i < 32 && currentBit < buffer.Length * 8)
            {
                i++;
            }
            r = RangeToInt(ReadBitRange(i));
            r += (1 << i) - 1;
            return r;
        }
        private int se()
        {
            int r = ue();
            if ((r & 0x01) == 0x01)
            {
                r = (r + 1) / 2;
            }
            else
            {
                r = -(r / 2);
            }
            return r;
        }
        /// <summary>
        /// PTDS for bluray
        /// </summary>
        /// <returns></returns>
        public long ReadPts_dts()
        {
            byte[] pts = new byte[5];
            Buffer.BlockCopy(buffer, (int)Position, pts, 0, 5);
            long p = (pts[0] & 0x0E << 29) + ((pts[1] & 0x7F) << 21) + ((pts[2] & 0xFE) << 14) + (pts[3] << 7) + ((pts[4] & 0xFE) >> 1);
            return p / 90;
        }
        /// <summary>
        /// Skips u bits
        /// </summary>
        /// <param name="u"></param>
        public void SkipBit(int i)
        {
            currentBit += i;
        }
        /// <summary>
        /// Skips u bytes
        /// </summary>
        /// <param name="u"></param>
        public void SkipByte(int i)
        {
            currentBit += i * 8;
        }
        /// <summary>
        /// Read EBML encoded integer
        /// </summary>
        /// <returns></returns>
        public ELEMENTARY_TYPE ReadVint()
        {
            ELEMENTARY_TYPE ret = null;
            int x = ReadBit();
            if (x == 1)
            {
                BitPosition--;
                //     ret = new ELEMENTARY_TYPE(this, 0, typeof(byte));
            }
            else
            {
                x = ReadBit();
                BitPosition -= 2;
                //    ret = new ELEMENTARY_TYPE(this, 0, typeof(byte));
            }
            return ret;
        }
        #endregion
        #region byte reading
        /// <summary>
        /// Reads BCD encoding
        /// </summary>
        /// <returns></returns>
        public int ReadBCD()
        {
            byte b = ReadByte();
            int digit1 = b >> 4;
            int digit2 = b & 0x0f;
            return digit1 * 10 + digit2;
        }
        public byte ReadByte()
        {
            currentBit += 8;
            return base.ReadByte();
        }
        public byte[] ReadBytes(int count)
        {
            currentBit += 8 * count;
            return base.ReadBytes(count);
        }
        /// <summary>
        /// Read a byte starting from current bit position
        /// </summary>
        /// <returns></returns>
        public byte ReadByteFromBits()
        {
            List<int> bits = ReadBitRange(8);
            return (byte)RangeToInt(bits);
        }
        /// <summary>
        /// Reads an integer from a range of byte
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public long ReadIntegerFromBytes(int length)
        {
            List<byte> b = new List<byte>();
            for (int u = 0; u < length; u++)
            {
                b.Add(ReadByte());
                BitPosition += 8;
            }
            byte[] bytes = b.ToArray();
            long i = 0;
            if (littleEndian)
            {
                for (int k = 0; k < length; k++)
                {
                    i = (i * 256 + bytes[k]);
                }
            }
            else
            {
                for (int k = length - 1; k >= 0; k--)
                {
                    i = (i * 256 + bytes[k]);
                }
            }
            return i;
        }
        public long ReadLong()
        {
            return ReadIntegerFromBytes(8);
        }
        public int ReadInteger()
        {
            return (int)ReadIntegerFromBytes(4);
        }
        public short ReadShort()
        {
            return (short)ReadIntegerFromBytes(2);
        }
        public Guid ReadGuid()
        {
            return new Guid(ReadBlock(16));
        }
        /// <summary>
        /// Reads an array of bytes starting from current bit position
        /// </summary>
        /// <param name="length">Number of bytes to be read</param>
        /// <returns></returns>
        public byte[] ReadBytesFromBits(int length)
        {
            List<byte> b = new List<byte>();
            for (int u = 0; u < length; u++)
            {
                b.Add(ReadByteFromBits());
            }
            return b.ToArray();
        }
        public byte[] ReadBlock(int length)
        {
            byte[] dat = new byte[length];
            Buffer.BlockCopy(buffer, (int)Position, dat, 0, length);
            Position += length;
            return dat;
        }
        /// <summary>
        /// Reads a string starting from current bit position
        /// </summary>
        /// <param name="length">String length</param>
        /// <returns></returns>
        public string ReadString(int length)
        {
            return Encoding.Default.GetString(ReadBytes(length));
        }
        public string ReadWindowString()
        {
            List<byte> bs = new List<byte>();
            byte b = ReadByte();
            while ((b != 0x0a) & Position < Length)
            {
                bs.Add(b);
            }
            ReadByte();
            return Encoding.Default.GetString(bs.ToArray());
        }
        /// <summary>
        /// Reads a zero ended string
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            List<byte> b = new List<byte>();
            int a = ReadByte();
            while (a != 0)
            {
                b.Add((byte)a);
                a = ReadByte();
            }
            return Encoding.Default.GetString(b.ToArray());
        }
        public string ReadString(int length, Encoding enc)
        {
            //à corriger pour tenir compte de l'encodage
            int mul = 1;
            if (enc == Encoding.Unicode)
                mul = 2;
            byte[] c = ReadBytes(length * mul);
            return enc.GetString(c);
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
        #endregion
        #region Private data
        bool littleEndian = true;

        public bool LittleEndian
        {
            get { return littleEndian; }
            set { littleEndian = value; }
        }
        byte[] buffer;

        public byte[] InnerBuffer
        {
            get { return buffer; }
            set { buffer = value; }
        }
        long currentBit;
        byte[] BitMask = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01, 0x00 };
        #endregion
    }
    */
    #region Event Handling
    public class ScrollArgs
    {
        long secteur;
        public long Secteur
        {
            get { return secteur; }
            set { secteur = value; }
        }
        public ScrollArgs(long sect)
        {
            secteur = sect;
        }
    }
    public delegate void ScrollHandler(object sender, ScrollArgs e);
    public class DataEventArgs
    {
        private byte[] data;

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }
        public DataEventArgs(byte[] data)
        {
            this.data = data;
        }
    }
    public delegate void DataEventHandler(object sender, DataEventArgs e);
    public class DataSelectedEventArgs
    {
        public long position;
        public long length;
        public object o;
        public string fileName;
        public DataSelectedEventArgs(long position, long length, string fileName)
        {
            this.position = position;
            this.length = length;
            this.fileName = fileName;
        }
        public DataSelectedEventArgs(long position, long length, object o)
        {
            this.position = position;
            this.length = length;
            this.o = o;
        }
        public DataSelectedEventArgs(long position, long length)
        {
            this.position = position;
            this.length = length;
        }
    }
    public delegate void DataSelectedEventHandler(object sender, DataSelectedEventArgs e);
    public class TabSelectedArgs
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public TabSelectedArgs(string name)
        {
            this.name = name;
        }
    }
    public delegate void TabSelectedEventHandler(object sender, TabSelectedArgs e);
    #endregion
    public class BinPosition
    {
        public long bitPosition;
        public string name;
        public Type type;
        public object value;
        public BinPosition(long bit, string n, Type t, object v)
        {
            bitPosition = bit;
            name = n;
            type = t;
            value = v;
        }
    }
}


