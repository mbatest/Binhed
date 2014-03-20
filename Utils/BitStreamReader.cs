using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utils
{
    /// <summary>
    /// class to parse a buffer of bytes in a bitwise manner 
    /// </summary>
    public partial class BitStreamReader
    {
        FileStream fs;
        #region Private data
        bool littleEndian = true;
        byte[] buffer;
        long currentBit;

        public long CurrentBit
        {
            get { return currentBit; }
            set
            {
                currentBit = value;
                if (currentBit == 8)
                {
                    currentBit = 0;
                    Position += 1;
                }
            }
        }
        public bool LittleEndian
        {
            get { return littleEndian; }
            set { littleEndian = value; }
        }
        public byte[] InnerBuffer
        {
            get { return buffer; }
            set { buffer = value; }
        }
        #endregion
        private string name;
        #region Properties
        public string Name { get { return name; } }
        public long Length
        {
            get
            {
                if (fs != null) return fs.Length;
                if (buffer != null) return buffer.Length;
                return 0;
            }
        }
        public long BitPosition
        {
            get { return currentBit; }
            set
            {
                currentBit = value;
            }
        }
        public long StreamPosition { get { return fs.Position; } }

        public long Position
        {
            get
            {
                if (fs != null)
                    return fs.Position;
                else return currentBit / 8;
            }
            set
            {
                if (fs != null)
                    fs.Position = value;
                //               currentBit = value * 8;
            }
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
            name = "";
            this.littleEndian = littleEndian;
            BitPosition = 0;
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
            name = fs.Name;
            littleEndian = isBigEndian;
        }
        /// <summary>
        /// Align along the next byte
        /// </summary>
        public void Align()
        {
      /*      if (currentBit % 8 != 0)
                currentBit += (8 - currentBit % 8);*/
        }
        public bool Eof
        {
            get
            {
                return Position >= Length;
            }
        }
        public void Close()
        {
            if (fs != null) fs.Close();
        }
        public long FindBinaryPattern(string pattern, long start)
        {
            char[] d = pattern.ToCharArray();
 /*           while (BitPosition < Length)
            {
                Position = (int)start;
                for (long i = start; i < Length; i++)
                {
                    string a = ReadBit().ToString();
                    if (a == d[0].ToString())
                    {
                        bool found = true;
                        long startPattern = BitPosition;
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
            }*/
            return -1;
        }
        #region byte reading
        /// <summary>
        /// Read a byte starting from current bit position
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            if(fs!=null)
            {
                byte b =(byte)fs.ReadByte();
                return b;
            }
            else if (Position < InnerBuffer.Length)
            {
                byte b = this.InnerBuffer[Position];
                return b;
            }
            return 0;
            /*   List<int> bits = ReadBitRange(8);
               return (byte)RangeToInt(bits);*/
        }
        /// <summary>
        /// Reads a byte at a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte ReadByte(long index)
        {
 //           BitPosition = index * 8;
            return ReadByte();
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
                byte a = ReadByte();
                b.Add(a);
            }
            return b.ToArray();
        }
        public long ReadIntegerFromBytes(int length)
        {
            List<byte> b = new List<byte>();
            for (int u = 0; u < length; u++)
            {
                b.Add(ReadByte());
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
        public byte[] ReadBlock(int length)
        {
            byte[] dat = new byte[length];
            if (fs != null)
            {
                fs.Read(dat, 0, length);
            }
            else
            {
              
                Buffer.BlockCopy(buffer, (int)Position, dat, 0, length);
                Position += length;
            }
            return dat;
        }
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
     }
}
