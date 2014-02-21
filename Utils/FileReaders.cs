using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;

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
            : base(path,FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
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
                while (a[0]!= 0)
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
        public long ReadLongInt()
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
    /// <summary>
    /// class to parse a buffer of bytes in a bitwise manner 
    /// </summary>
    public class BitStreamReader
    {
        FileStream fs;
        private long length;
        private string name;
        #region Properties
        public long Length
        {
            get { return length; }
            set { length = value; }
        }
        public string Name
        {
            get { return name; }
        }
        public int BitPosition
        {
            get { return currentBit; }
            set { currentBit = value; }
        }
        public int Position
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
        public BitStreamReader(byte[] buffer, bool littleEndian)
        {
            this.buffer = buffer;
            length = buffer.Length;
            name = "";
            this.littleEndian = littleEndian;
            currentBit = 0;
        }
        /// <summary>
        /// Bitwise reading of a filestream
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fm"></param>
        /// <param name="fa"></param>
        /// <param name="ff"></param>
        /// <param name="isBigEndian"></param>
        public BitStreamReader(string path, bool isBigEndian)
        {
            fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            length = fs.Length;
            name = fs.Name;
            littleEndian = isBigEndian;
  //          BufferConvert.littleEndian = littleEndian;
            this.buffer = new byte[(int)fs.Length];
            fs.Read(this.buffer, 0, (int)fs.Length);
            fs.Close();
        }
        /// <summary>
        /// Align along the next byte
        /// </summary>
        public void Align()
        {
            int rest = currentBit % 8;
            if (rest != 0)
                currentBit += (8 - rest);
        }
        public void Close()
        {
            //          fs.Close();
        }
        public long FindBinaryPattern(string pattern, long start)
        {
            char[] d = pattern.ToCharArray();
            while (currentBit < Length)
            {
                Position=(int) start;
                for (long i = start; i <Length; i++)
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
        public int ReadBit(int index)
        {
            int value = 0;
            int byteIndex = index / 8;
            int bitIndex = index % 8;
            try
            {
                value = (buffer[byteIndex] & BitMask[bitIndex]) >> (7 - bitIndex);
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
            int end = currentBit + length;
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
            int end = currentBit + length;
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
        public int ue() // Golomb
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
        public int se()
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
        public long ReadPts_dts()
        {
            byte[] pts = new byte[5];
            Buffer.BlockCopy(buffer, Position, pts, 0, 5);
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
        /// <summary>
        /// Read a byte starting from current bit position
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            List<int> bits = ReadBitRange(8);
            return (byte)RangeToInt(bits);
        }
        public long ReadIntegerFromBytes(int length)
        {
            List<byte> b = new List<byte>();
            for (int u = 0; u < length; u++)
            {
                b.Add(buffer[Position]);
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
        public byte[] ReadBytes(int length)
        {
            List<byte> b = new List<byte>();
            for (int u = 0; u < length; u++)
            {
                b.Add(ReadByte());
            }
            return b.ToArray();
        }
        public byte[] ReadBlock(int length)
        {
            byte[] dat = new byte[length];
            Buffer.BlockCopy(buffer, Position, dat, 0, length);
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
            while ((b != 0x0a)& Position<Length)
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
                b.Add((byte) a);
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
        int currentBit;
        byte[] BitMask = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01, 0x00 };
        #endregion
    }
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
    public interface ILOCALIZED_DATA
    {
        long PositionOfStructureInFile { get; set; }
        long LengthInFile { get; set; }
    }
    public class LOCALIZED_DATA : ILOCALIZED_DATA
    {
        private long position;
        private long length;
        public long PositionOfStructureInFile
        {
            get { return position; }
            set { position = value; }
        }
        public long LengthInFile
        {
            get { return length; }
            set { length = value; }
        }
        public override string ToString()
        {
            return "";
        }
    }
    [Serializable]
    public class ELEMENTARY_TYPE : LOCALIZED_DATA
    {
        object value = "";
        Type t;
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        /// <summary>
        /// Reads a given type
        /// </summary>
        /// <param name="sw">Stream</param>
        /// <param name="offset">Offset : used in raw access to refer to the origin of the disk</param>
        /// <param name="t">Type to read</param>
        public ELEMENTARY_TYPE(BitStreamReader sw, long offset, Type t)
        {
            PositionOfStructureInFile = sw.Position + offset;
            this.t = t;
            switch (t.Name)
            {
                case "Byte":
                    byte ub = sw.ReadByte();
                    value = ub;
                    break;
                case "Int16":
                    short us = sw.ReadShort();
                    value = us;
                    break;
                case "UInt16":
                    ushort u = (ushort)sw.ReadShort();
                    value = u;
                    break;
                case "Int32":
                    int uis = sw.ReadInteger();
                    value = uis;
                    break;
                case "UInt32":
                    uint ui = (uint)sw.ReadInteger();
                    value = ui;
                    break;
                case "UInt64":
                    ulong ulb = (ulong)sw.ReadLong();
                    value = ulb;
                    break;
                case "Int64":
                    long ul = sw.ReadLong();
                    value = ul;
                    break;
                case "DateTime":
                    long ud = sw.ReadLong();
                    value = ToDateTime(ud);
                    break;
                case "Guid":
                    Guid g = new Guid(sw.ReadBytes(0x10));
                    value = g;
                    break;
            }
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        /// <summary>
        /// Reads a given number of bytes into a given type
        /// </summary>
        /// <param name="sw">Stream</param>
        /// <param name="offset">Offset : used in raw access to refer to the origin of the disk</param>
        /// <param name="t">Type to read</param>
        /// <param name="length">Number of bytes</param>
        public ELEMENTARY_TYPE(BitStreamReader sw, long offset, Type t, int length)
        {
            PositionOfStructureInFile = sw.Position + offset;
            this.t = t;
            switch (t.Name)
            {
                case "Int32":
                    int uis = (int)sw.ReadIntegerFromBytes(length);
                    value = uis;
                    break;
                case "UInt32":
                    uint ui = (uint)sw.ReadIntegerFromBytes(length);
                    value = ui;
                    break;
                case "UInt64":
                    ulong ulb = (ulong)sw.ReadIntegerFromBytes(length);
                    value = ulb;
                    break;
                case "Int64":
                    long ul = sw.ReadIntegerFromBytes(length);
                    value = ul;
                    break;
                case "Byte[]":
                    byte[] b = sw.ReadBlock(length);
                    value = b;
                    break;
                case "Bitmap":
                    byte[] im = sw.ReadBlock(length);
                    using (MemoryStream ts = new MemoryStream(im, 0, length))
                    {
                        Image thumbnail = Image.FromStream(ts);
                        value = thumbnail;
                    }
                    break;
                /*               case "Short[]":
                                   short[] bs = sw.ReadBytes(length);
                                   value = bs;
                                   break;
                               case "Int32[]":
                                   int[] bi = sw.ReadBytes(length);
                                   value = bi;
                                   break;
                               case "Int64[]":
                                   long[] bl = sw.ReadBytes(length);
                                   value = bl;
                                   break;*/
            }
            LengthInFile = length;
        }
        /// <summary>
        /// Reads a string of given length
        /// </summary>
        /// <param name="sw">Stream</param>
        /// <param name="offset">Offset : used in raw access to refer to the origin of the disk</param>
        /// <param name="enc">Encoding</param>
        /// <param name="length">Length of string</param>
        public ELEMENTARY_TYPE(BitStreamReader sw, long offset, Encoding enc, int length)
        {
            PositionOfStructureInFile = sw.Position + offset;
            this.t = typeof(string);
            value = sw.ReadString(length, enc);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        /// <summary>
        /// Reads zero terminated string 
        /// </summary>
        /// <param name="sw">Stream</param>
        /// <param name="offset">offset</param>
        /// <param name="enc">Encoding</param>
        public ELEMENTARY_TYPE(BitStreamReader sw, long offset, Encoding enc)
        {
            PositionOfStructureInFile = sw.Position + offset;
            this.t = typeof(string);
            value = sw.ReadStringZ(enc);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public ELEMENTARY_TYPE(string s, long offset)
        {
            PositionOfStructureInFile = offset;
            this.t = typeof(string);
            value = s;
            LengthInFile = s.Length;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="offset">used in raw access to refer to the origin of the disk</param>
        /// <param name="length"></param>
        public ELEMENTARY_TYPE(string s, long offset, long length)
        {
            PositionOfStructureInFile = offset;
            this.t = typeof(string);
            value = s;
            LengthInFile = length;
        }
        public override string ToString()
        {
            switch (t.Name)
            {
                case "Byte":
                    return ((byte)Value).ToString("x2");
                case "Int16":
                    return ((short)Value).ToString("x4");
                case "UInt16":
                    return ((ushort)Value).ToString("x4");
                case "Int32":
                    return ((int)Value).ToString("x8");
                case "UInt32":
                    return ((uint)Value).ToString("x8");
                case "UInt64":
                    return ((ulong)Value).ToString("x16");
                case "Int64":
                    return ((long)Value).ToString("x16");
                case "DateTime":
                    return ((DateTime)Value).ToShortDateString() + " " + ((DateTime)Value).ToShortTimeString();
                case "String":
                    return (string)Value;
                case "Guid":
                    return ((Guid)Value).ToString();
                default:
                    return "";
            }
        }
        public DateTime ToDateTime(long shift)
        {
            DateTime d = new DateTime(1601, 1, 1, 1, 0, 1);//"12:00 A.M. January 1, 1601 ";
            TimeSpan ts = new TimeSpan(shift);
            d = d + ts;
            return d;
        }
    }
}


