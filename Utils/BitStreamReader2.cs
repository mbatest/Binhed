using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public partial class BitStreamReader
    {
        byte[] BitMask = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01, 0x00 };
        #region Reading data bit after bit
        /// <summary>
        /// Returns the current bit : most basic function
        /// </summary>
        /// <returns></returns>
        public int ReadBit()
        {
            int value = ReadBit(currentBit);
            currentBit++;
            Position--;
            if (currentBit == 8)
            {
                Position++;
                currentBit = 0;
            }
            return value;
        }
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
            value = (ReadByte(Position) & BitMask[bitIndex]);
            value = value >> (7 - bitIndex);

      //      currentBit = index + 1;
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
            for(int i = 0;i<length;i++)
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
                bits.Add(ReadBool());
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
        /// <summary>
        /// Reads UInt64 starting from the current position
        /// </summary>
        /// <returns></returns>
        public int ReadLongFromBits()
        {
            return ReadIntFromBits(8);
        }
        public int ReadShortFromBits()
        {
            return ReadIntFromBits(2);
        }
        /// <summary>
        /// Reads an integer of a given length starting from current bit position
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
        /// Reads an integer starting of a given length from current bit position
        /// </summary>
        /// <param name="length">number of bits</param>
        /// <returns>integer read</returns>
        public ulong ReadIntegerFromBits(int length)
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
        #region Golomb coded integers
        /// Reads an integer encoded in exponential Golomb code
        /// </summary>
        /// <returns></returns>
        public int ReadGolomb()
        {
            List<byte> bits = new  List<byte>();
            int leading = 0;
            while (ReadBit() == 0)
                leading++;
    /*        bits.Add(1);
            for(int i =0; i<leading;i++)
            {
                bits.Add((byte)ReadBit());
            }*/
            currentBit--;
            if(currentBit==-1)
            {
                Position--;
                currentBit = 7;
            }
            int a = ReadIntFromBits(leading + 1);
            return a - 1;
     /* Would work for alternating signs     
      * 
      * if (a == 1) return 0;
            else
                return a;
                if ((a % 2 == 0) & (a > 1))
                    return a / 2;
                else return -(a / 2);
           
        /*    double val = Math.Pow(2, leading);
            uint dta = (UInt32) val -1 + Get4(leading);
            return (int) dta;
            
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
            
            }*/
        }
        public int ue() // Golomb
        {
            int r = 0;
            int i = 0;
            while (ReadBit() == 0 && i < 32 && BitPosition < Length * 8)
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
        #endregion
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
            BitPosition += i;
        }
        /// <summary>
        /// Skips u bytes
        /// </summary>
        /// <param name="u"></param>
        public void SkipByte(int i)
        {
            BitPosition += i * 8;
        }
        public ELEMENTARY_TYPE ReadVint()
        {
            ELEMENTARY_TYPE ret = null;
            int x = ReadBit();
            if (x == 1)
            {
                BitPosition--;
                ret = new ELEMENTARY_TYPE(this, 0, typeof(byte));
            }
            else
            {
                x = ReadBit();
                BitPosition -= 2;
                ret = new ELEMENTARY_TYPE(this, 0, typeof(byte));
            }
            return ret;
        }
        #endregion

    }
}
