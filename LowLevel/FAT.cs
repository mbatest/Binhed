using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace LowLevel
{
    public class Root16Entry : LOCALIZED_DATA
    {
        private ELEMENTARY_TYPE nameBytes;//0x00
        private ELEMENTARY_TYPE attributes;//0B
        private ELEMENTARY_TYPE reserved;//0x0C
        private ELEMENTARY_TYPE createHour;//0D
        private ELEMENTARY_TYPE createTime;//0x10
        private ELEMENTARY_TYPE createDate;
        private ELEMENTARY_TYPE lastAccessDate;
        private ELEMENTARY_TYPE lastModifiedTime;
        private ELEMENTARY_TYPE lastModifiedDate;
        private ELEMENTARY_TYPE startOfFile;
        private ELEMENTARY_TYPE sizeOfFile;
        int entryNumber;
        private bool isDeleted;
        private string name;
        private string extension;
        private long startSector;
        private long position;

        public ELEMENTARY_TYPE NameBytes
        {
            get { return nameBytes; }
            set { nameBytes = value; }
        }
        public ELEMENTARY_TYPE Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
        public ELEMENTARY_TYPE CreateHour
        {
            get { return createHour; }
            set { createHour = value; }
        }
        public ELEMENTARY_TYPE CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }
        public ELEMENTARY_TYPE CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }
        public ELEMENTARY_TYPE LastAccessDate
        {
            get { return lastAccessDate; }
            set { lastAccessDate = value; }
        }
        public ELEMENTARY_TYPE Extended_Attributes
        {
            get { return eaIndex; }
            set { eaIndex = value; }
        }
        public ELEMENTARY_TYPE Last_Modified_Time
        {
            get { return lastModifiedTime; }
            set { lastModifiedTime = value; }
        }
        public ELEMENTARY_TYPE Last_Modified_Date
        {
            get { return lastModifiedDate; }
            set { lastModifiedDate = value; }
        }
        public ELEMENTARY_TYPE StartOfFile
        {
            get { return startOfFile; }
            set { startOfFile = value; }
        }
        public ELEMENTARY_TYPE SizeOfFile
        {
            get { return sizeOfFile; }
            set { sizeOfFile = value; }
        }
        public DateTime? Creation_Date
        { get { try { return ShortToDate((byte[])createDate.Value, (byte[])createTime.Value); } catch {return null; } } }
        public DateTime? Last_Modidied
        { get { try { return ShortToDate((byte[])lastModifiedDate.Value, (byte[])lastModifiedTime.Value); } catch { return null; } } }
        public int Sequence_Number
        {
            get
            {
                if (HasLongFileName)
                    return (entryNumber) & 0x1F;
                else
                    return -1;
            }
        }
        public bool HasLongFileName
        {
            get { return ((byte)attributes.Value & 0x0F) == 0x0F; }
        }
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }
        #region Attributes values
        public bool IsReadOnly
        {
            get { return ((byte)attributes.Value & 0x01) == 0x01; }
        }
        public bool IsHidden
        {
            get { return ((byte)attributes.Value & 0x02) == 0x02; }
        }
        public bool IsSystem
        {
            get { return ((byte)attributes.Value & 0x04) == 0x04; }
        }
        public bool IsVolumeLabel
        {
            get { return ((byte)attributes.Value & 0x08) == 0x08; }
        }
        public bool IsSubdirectory
        {
            get { return ((byte)attributes.Value & 0x10) == 0x10; }
        }
        public bool IsArchive
        {
            get { return ((byte)attributes.Value & 0x20) == 0x20; }
        }
        public bool IsDevice
        {
            get { return ((byte)attributes.Value & 0x40) == 0x40; }
        }
        #endregion
        public long DataStartSector
        {
            get { return startSector; }
            set { startSector = value; }
        }
        public long Position
        {
            get { return position; }
            set { position = value; }
        }

        private ELEMENTARY_TYPE eaIndex;
        public Root16Entry(BitStreamReader sw, Partition p, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            long start = sw.Position;
            LengthInFile = 0x20;
            entryNumber = sw.ReadByte();
            if (entryNumber < 0x50)
            {
                ELEMENTARY_TYPE st = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, 05);
                attributes = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
                reserved = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
                ELEMENTARY_TYPE checksum = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
                ELEMENTARY_TYPE mid = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, 06);
                ELEMENTARY_TYPE cluster = new ELEMENTARY_TYPE(sw, offset, typeof(short));
                ELEMENTARY_TYPE end = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, 02);
                nameBytes = new ELEMENTARY_TYPE((string)st.Value + (string)mid.Value + (string)end.Value, offset);
                name = (string)nameBytes.Value;
                sw.Position = start + 0x20;
            }
            else
            {
                sw.Position--;
                nameBytes = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 11); //sw.ReadBytes(11);
                if (((byte[])nameBytes.Value)[0] == 0x00)
                    return;
                attributes = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
                reserved = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
                createHour = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
                createTime = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2); //sw.ReadBytes(2);
                createDate = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2); //sw.ReadBytes(2);
                lastAccessDate = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2); //sw.ReadBytes(2);
                eaIndex = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2); //sw.ReadBytes(2);
                lastModifiedTime = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2); //sw.ReadBytes(2);
                lastModifiedDate = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2); //sw.ReadBytes(2);
                startOfFile = new ELEMENTARY_TYPE(sw, offset, typeof(ushort)); ;// sw.ReadShort();
                sizeOfFile = new ELEMENTARY_TYPE(sw, offset, typeof(int), 4); //sw.ReadBytes(4);
                switch (((byte[])nameBytes.Value)[0])
                {
                    case 0xE5:
                        isDeleted = true;
                        break;
                    case 0x05:
                        ((byte[])nameBytes.Value)[0] = 0xE5;
                        break;
                    case 0x2E://parent
                        break;
                }
                if (HasLongFileName)
                {
                    List<byte> by = new List<byte>();
                    name = Encoding.Unicode.GetString((byte[])nameBytes.Value, 1, 10);
                    name += Encoding.Unicode.GetString((byte[])createTime.Value);
                    name += Encoding.Unicode.GetString((byte[])createDate.Value);
                    name += Encoding.Unicode.GetString((byte[])lastAccessDate.Value);
                    name += Encoding.Unicode.GetString((byte[])eaIndex.Value);
                    name += Encoding.Unicode.GetString((byte[])lastModifiedTime.Value);
                    name += Encoding.Unicode.GetString((byte[])lastModifiedDate.Value);
                    name += Encoding.Unicode.GetString(BitConverter.GetBytes((int)sizeOfFile.Value));
                }
                else
                {
                    name = Encoding.Default.GetString((byte[])nameBytes.Value, 0, 8).Trim();
                    extension = Encoding.Default.GetString((byte[])nameBytes.Value, 8, 3).Trim();
                }
                startSector = p.ClusterToSector((ushort)startOfFile.Value);
            }
        }
        /*Date Format: The date field is 16-bit. This is relative to 01/01/1980.
Bits 0 - 4: Day of month, range 1-31 
Bits 5 - 8: Month of year, range 1-12 
Bits 9 - 15: Count of years, range 0 - 127 
Time format: This is also 16-bit and its granularity is 2 seconds.

Bits 0 - 4: 2-second count, valid value range 0-29 inclusive (0 - 58 seconds) 
Bits 5 - 10: Minutes, range 0-59 
Bits 11 - 15: Hours, range 0-23 
*/
        private DateTime ShortToDate(byte[] date, byte[] time)
        {
            int d = (date[0] & 0x1F);
            int m = ((date[0] & 0xE0) >> 5) + ((date[1] & 0x01) << 3);
            int y = (date[1] & 0xFE) >> 1;
            int s = (time[0] & 0x1f);
            int mn = ((time[0] & 0xE0) >> 5) + ((time[1] & 0x07) << 3);
            int h = (time[1] & 0xF8) >> 3;
            return new DateTime(y + 1980, m, d, h, mn, s * 2);
        }
        public override string ToString()
        {
            string n = name.Trim();
            if (extension != "")
                n += "." + extension;
            return n;
        }
    }
    public class Directory : LOCALIZED_DATA
    {
        private List<Root16Entry> entries = new List<Root16Entry>();
        private long sector;
        public long Sector
        {
            get { return sector; }
            set { sector = value; }
        }
        public List<Root16Entry> Entries
        {
            get { return entries; }
            set { entries = value; }
        }
        public Directory(BitStreamReader sw, Partition p, long offset, int numberOfEntries)
        {

            PositionOfStructureInFile = sw.Position + offset;
            sector = offset / (short)p.Boot_Sector.Bytes_Per_Sector.Value;
            if(numberOfEntries==0)
            {
                while (sw.Position < sw.Length)
                {
                    Root16Entry entry = new Root16Entry(sw, p, offset);
                    if (entry.Name != null)
                        entries.Add(entry);
                    else
                        break;
                }
            }
            else
            for (int i = 0; i < numberOfEntries; i++)
            {

                entries.Add(new Root16Entry(sw, p, offset));
            }
            
            LengthInFile = 0x20 * numberOfEntries;
        }
        public override string ToString()
        {
            return "Directory";
        }
    }
    public class FAT : LOCALIZED_DATA
    {
        public List<ELEMENTARY_TYPE> fatEntries;
        public FAT(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            while (sw.Position <sw.Length)
                fatEntries.Add(new ELEMENTARY_TYPE(sw,offset , typeof(short)));
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
}
