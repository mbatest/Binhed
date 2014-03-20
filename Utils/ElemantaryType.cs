using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Utils
{
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
        public ELEMENTARY_TYPE() { }
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
        public ELEMENTARY_TYPE(int s, long offset, Type t)
        {
            PositionOfStructureInFile = offset;
            this.t = t;
            value = s;
            LengthInFile = 4;
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
