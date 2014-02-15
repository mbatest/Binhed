using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
//http://hackipedia.org/Disk%20formats/File%20systems/NTFS,%20Windows%20NT%20Filesystem%20(Windows)/html,%20ntfsdoc-0.5/index.html
namespace LowLevel
{
    public class MFT : LOCALIZED_DATA
    {
        List<FILE_RECORD_SEGMENT> fileRecords = new List<FILE_RECORD_SEGMENT>();
        public List<FILE_RECORD_SEGMENT> File_Records
        {
            get { return fileRecords; }
            set { fileRecords = value; }
        }
        public MFT(BitStreamReader sw, long offset, Partition p, int FRNumber)
        {
            PositionOfStructureInFile = sw.Position + offset;
            for (int i = 0; i < FRNumber; i++)
            {
                sw.Position = i * 0x400;
                if (i == 0x10)
                {
                }
                FILE_RECORD_SEGMENT fs = new FILE_RECORD_SEGMENT(sw, offset, p, i);
                fileRecords.Add(fs);
            }
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class FILE_RECORD_SEGMENT : LOCALIZED_DATA
    {
        int mftRecordNumber;
        FILE_RECORD_SEGMENT_HEADER file_record_header;
        List<ATTRIBUTE_RECORD_HEADER> attributes = new List<ATTRIBUTE_RECORD_HEADER>();
        bool isFile = false;
        bool isDirectory = false;
        string name;
        private List<ATTRIBUTE_DEFINITION_DATA> attributeDefinitions;
        private long startSector = -1;
        List<INDX> indx = new List<INDX>();

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public bool Is_File
        {
            get { return isFile; }
            set { isFile = value; }
        }
        public bool Is_Directory
        {
            get { return isDirectory; }
            set { isDirectory = value; }
        }
        public FILE_RECORD_SEGMENT_HEADER File_record_header
        {
            get { return file_record_header; }
            set { file_record_header = value; }
        }
        public List<ATTRIBUTE_RECORD_HEADER> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
        public List<ATTRIBUTE_DEFINITION_DATA> Attribute_Definitions
        {
            get { return attributeDefinitions; }
            set { attributeDefinitions = value; }
        }
        public List<INDX> Index
        {
            get { return indx; }
            set { indx = value; }
        }
        public long Data_Start_Sector
        {
            get { return startSector; }
            set { startSector = value; }
        }
        public FILE_RECORD_SEGMENT(BitStreamReader sw, long offset, Partition p, int i)
        {
            PositionOfStructureInFile = sw.Position + offset;
            mftRecordNumber = i;
            int start = sw.Position;
            file_record_header = new FILE_RECORD_SEGMENT_HEADER(sw, offset);
            sw.Position = start + (ushort)file_record_header.First_Attribute_Offset.Value;
            int deb = sw.Position;
            if ((string)file_record_header.File_record_header.Signature.Value != "FILE")
                return;
            while (sw.Position < start + 0x400)
            {
                ATTRIBUTE_RECORD_HEADER att = new ATTRIBUTE_RECORD_HEADER(sw, offset);
                if (/*(att.Attribute_Type == 0)||*/((uint)att.Attribute_Type == 0xffffffff))
                    break;
                else attributes.Add(att);
                switch (att.Attribute_Type)
                {
                    case ATTRIBUTE_TYPE_CODE.FILE_NAME:
                        isFile = att.File_Name.isFile;
                        isDirectory = att.File_Name.isDirectory;
                        name = (string)att.File_Name.FileName.Value;
                        break;
                    case ATTRIBUTE_TYPE_CODE.DATA:
                        if (att.Nonresident != null)
                        {
                            startSector = p.ClusterToSector(att.Nonresident.Startcluster);
                        }
                        else
                            startSector = -1;
                        break;
                }
            }
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            string s = mftRecordNumber.ToString("x2") + " " + "Record " + name + " ";
            if (isFile) s += "File";
            if (isDirectory) s += "Directory";
            return s;
        }
    }
    public class FILE_RECORD_SEGMENT_HEADER : LOCALIZED_DATA
    {
        MULTI_SECTOR_HEADER multiSectorHeader;//0x00 
        ELEMENTARY_TYPE sequenceNumber;//0x10
        ELEMENTARY_TYPE hardLinksCount;//0x12
        ELEMENTARY_TYPE firstAttributeOffset;//0x14
        ELEMENTARY_TYPE flags;//0x16

        ELEMENTARY_TYPE logicalSizeOfRecord;
        ELEMENTARY_TYPE physicalSizeOfRecord;
        MFT_SEGMENT_REFERENCE baseFileRecordSegment;
        ELEMENTARY_TYPE numberOfAttributes;//or next available number ?
        List<short> UpdateSequenceArray = new List<short>();

        public MULTI_SECTOR_HEADER File_record_header
        {
            get { return multiSectorHeader; }
            set { multiSectorHeader = value; }
        }
        public ELEMENTARY_TYPE Sequence_Number
        {
            get { return sequenceNumber; }
            set { sequenceNumber = value; }
        }
        public ELEMENTARY_TYPE Hard_Links_Count
        {
            get { return hardLinksCount; }
            set { hardLinksCount = value; }
        }
        public ELEMENTARY_TYPE First_Attribute_Offset
        {
            get { return firstAttributeOffset; }
            set { firstAttributeOffset = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public ELEMENTARY_TYPE Logical_Size_Of_Record
        {
            get { return logicalSizeOfRecord; }
            set { logicalSizeOfRecord = value; }
        }
        public ELEMENTARY_TYPE Physical_Size_Of_Record
        {
            get { return physicalSizeOfRecord; }
            set { physicalSizeOfRecord = value; }
        }
        public MFT_SEGMENT_REFERENCE Base_File_Record_Segment
        {
            get { return baseFileRecordSegment; }
            set { baseFileRecordSegment = value; }
        }
        public ELEMENTARY_TYPE Number_Of_Attributes
        {
            get { return numberOfAttributes; }
            set { numberOfAttributes = value; }
        }
        public bool Is_In_Use
        {get{return (((ushort)flags.Value & 0x01)==0x01);}}
        public bool Is_A_Directory
        { get { return (((ushort)flags.Value & 0x02) == 0x02); } }
        // For integrity checking : http://www.pc-3000flash.com/eng/help/help_information/ntfs/update_sequence.htm
        public FILE_RECORD_SEGMENT_HEADER(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            multiSectorHeader = new MULTI_SECTOR_HEADER(sw, offset);
            sequenceNumber = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));// (ushort)sw.ReadShort();
            hardLinksCount = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//(ushort)sw.ReadShort();
            firstAttributeOffset = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//(ushort)sw.ReadShort();
            flags = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//sw.ReadShort();
            logicalSizeOfRecord = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//(uint)sw.ReadInteger();
            physicalSizeOfRecord = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//(uint)sw.ReadInteger();
            baseFileRecordSegment = new MFT_SEGMENT_REFERENCE(sw, offset);
            numberOfAttributes = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//(ushort)sw.ReadShort();
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
            int pos = sw.Position;
            /*           sw.Position = (int)(pos - LengthInFile + multiSectorHeader.UpdateSequenceArrayOffset);
                       for (int i = 0; i < multiSectorHeader.UpdateSequenceArraySize - 1; i++)
                           UpdateSequenceArray.Add(sw.ReadShort());*/
            sw.Position = pos;
        }
        public override string ToString()
        {
            return "File record";
        }
    }
    public class MULTI_SECTOR_HEADER : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE signature;//4
        ELEMENTARY_TYPE updateSequenceArrayOffset;//4
        ELEMENTARY_TYPE updateSequenceArraySize;//6
        ELEMENTARY_TYPE logFileSequenceNumber;//0x08
        public ELEMENTARY_TYPE Signature
        {
            get
            {
                return signature;
            }
        }
        public ELEMENTARY_TYPE UpdateSequenceArrayOffset
        {
            get { return updateSequenceArrayOffset; }
            set { updateSequenceArrayOffset = value; }
        }
        public ELEMENTARY_TYPE UpdateSequenceArraySize
        {
            get { return updateSequenceArraySize; }
            set { updateSequenceArraySize = value; }
        }
        public ELEMENTARY_TYPE LogFileSequenceNumber
        {
            get { return logFileSequenceNumber; }
            set { logFileSequenceNumber = value; }
        }
        public MULTI_SECTOR_HEADER(BitStreamReader sw, long off)
        {
            long offset = off;// * 0x200;
            PositionOfStructureInFile = sw.Position + off;
            signature = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 4);//must be 'FILE'
            updateSequenceArrayOffset = new ELEMENTARY_TYPE(sw, off, typeof(ushort));//(ushort)sw.ReadShort();
            updateSequenceArraySize = new ELEMENTARY_TYPE(sw, off, typeof(ushort));//(ushort)sw.ReadShort();
            logFileSequenceNumber = new ELEMENTARY_TYPE(sw, off, typeof(ulong));//(ulong)sw.ReadLong();
            LengthInFile = sw.Position + off - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    // Represents an address in the master file table (MFT). The address is tagged with
    // a circularly reused sequence number that is set at the time the MFT segment reference was valid.
    public class MFT_SEGMENT_REFERENCE : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE segmentNumberLowPart;
        ELEMENTARY_TYPE segmentNumberHighPart;
        ELEMENTARY_TYPE sequenceNumber;
        public ELEMENTARY_TYPE SegmentNumberLowPart
        {
            get { return segmentNumberLowPart; }
            set { segmentNumberLowPart = value; }
        }
        public ELEMENTARY_TYPE SegmentNumberHighPart
        {
            get { return segmentNumberHighPart; }
            set { segmentNumberHighPart = value; }
        }
        public ELEMENTARY_TYPE SequenceNumber
        {
            get { return sequenceNumber; }
            set { sequenceNumber = value; }
        }
        public MFT_SEGMENT_REFERENCE(BitStreamReader sw, long off)
        {
            long offset = off;// * 0x200;
            PositionOfStructureInFile = sw.Position + offset;
            segmentNumberLowPart = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//(uint) sw.ReadInteger();
            segmentNumberHighPart = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//(ushort)sw.ReadShort();
            sequenceNumber = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//(ushort)sw.ReadShort();
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return SegmentNumberHighPart.ToString() + "." + SegmentNumberLowPart.ToString();
        }
    }
    public class ATTRIBUTE_DEFINITION_DATA : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE name;
        ELEMENTARY_TYPE number;
        ELEMENTARY_TYPE type;
        ELEMENTARY_TYPE flags;
        ELEMENTARY_TYPE minSize;
        ELEMENTARY_TYPE maxSize;
        public ELEMENTARY_TYPE Name
        {
            get { return name; }
            set { name = value; }
        }
        public ELEMENTARY_TYPE Number
        {
            get { return number; }
            set { number = value; }
        }
        public ELEMENTARY_TYPE Type
        {
            get { return type; }
            set { type = value; }
        }
        public ATTR_DEF_FLAGS Flags_Meaning
        {
            get { return (ATTR_DEF_FLAGS)(uint)flags.Value; }
        }
        public ELEMENTARY_TYPE Minimum_Size
        {
            get { return minSize; }
            set { minSize = value; }
        }
        public ELEMENTARY_TYPE Maximum_Size
        {
            get { return maxSize; }
            set { maxSize = value; }
        }
        public ATTRIBUTE_DEFINITION_DATA(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            int start = sw.Position;
            name = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode);
            sw.Position = start + 0x80;
            number = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            type = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            flags = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//sw.ReadInteger();
            minSize = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            maxSize = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            LengthInFile = 0xa0;// 
        }
        public override string ToString()
        {
            return number.ToString() + " " + name;
        }
    }

}