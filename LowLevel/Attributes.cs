using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
//http://ntfs-cmd.googlecode.com/svn-history/r10/trunk/ntfs.h
namespace LowLevel
{
    interface IATTRIBUTE_RECORD_HEADER
    {
        ELEMENTARY_TYPE Flags { get; set; }
        FORM_CODE Attribute_Form { get; set; }
        ELEMENTARY_TYPE Attribute_Id { get; set; }
        ELEMENTARY_TYPE Name_Length { get; set; }
        ELEMENTARY_TYPE Name_Offset { get; set; }
        ELEMENTARY_TYPE RecordLength { get; set; }
        ELEMENTARY_TYPE Type_Code { get; set; }
        string ToString();
    }
    public class ATTRIBUTE_RECORD_HEADER : LOCALIZED_DATA, LowLevel.IATTRIBUTE_RECORD_HEADER
    {
        ELEMENTARY_TYPE type_Code;//00
        ELEMENTARY_TYPE recordLength;// uint recordLength;//0x04
        ELEMENTARY_TYPE form;//0x05

        ELEMENTARY_TYPE nameLength;//byte nameLength;//0x09
        ELEMENTARY_TYPE nameOffset;//ushort nameOffset;//0x0A
        ELEMENTARY_TYPE flags;//ushort flags;//0x0C
        ELEMENTARY_TYPE attributeId;//ushort attributeId;//0x1E 

        FORM_CODE formCode;//0x08 : non resident flag
        RESIDENT resident;//
        NONRESIDENT nonresident;
        FILE_NAME f;
        STANDARD_INFORMATION st;
        VOLUME_NAME v;
        INDEX_ROOT ind;
        ATTRIBUTE_DATA attributeData;
        INDEX_ALLOCATION indx;
        OBJECT_ID obId;
        SECURITY_DESCRIPTOR security;
        List<ATTRIBUTE_LIST_ENTRY> attribute_list;
        #region basic attributes fields
        public ELEMENTARY_TYPE Type_Code
        {
            get { return type_Code; }
            set { type_Code = value; }
        }
        public ELEMENTARY_TYPE RecordLength
        { get { return recordLength; } set { recordLength = value; } }
        public ELEMENTARY_TYPE Form
        {
            get { return form; }
            set { form = value; }
        }
        public ELEMENTARY_TYPE Name_Length
        {
            get { return nameLength; }
            set { nameLength = value; }
        }
        public ELEMENTARY_TYPE Name_Offset
        {
            get { return nameOffset; }
            set { nameOffset = value; }
        }
        public ELEMENTARY_TYPE Attribute_Name
        {
            get
            {
                if ((resident != null))
                    if (resident.AttributeName != null)
                        return resident.AttributeName;
                if ((nonresident != null))
                    if (nonresident.AttributeName != null)
                        return nonresident.AttributeName;
                return null;
            }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public ELEMENTARY_TYPE Attribute_Id
        {
            get { return attributeId; }
            set { attributeId = value; }
        }
        public ATTRIBUTE_TYPE_CODE Attribute_Type
        {
            get { return (ATTRIBUTE_TYPE_CODE)(uint)type_Code.Value; }
        }
        public FORM_CODE Attribute_Form
        {
            get { return formCode; }
            set { formCode = value; }
        }
        public RESIDENT Resident
        {
            get { return resident; }
            set { resident = value; }
        }
        public NONRESIDENT Nonresident
        {
            get { return nonresident; }
            set { nonresident = value; }
        }
        #endregion
        #region Detailed attributes
        public INDEX_ROOT Index_Root
        {
            get { return ind; }
            set { ind = value; }
        }
        public VOLUME_NAME Volume_Name
        {
            get { return v; }
            set { v = value; }
        }
        public FILE_NAME File_Name
        {
            get { return f; }
            set { f = value; }
        }
        public STANDARD_INFORMATION Standard_Information
        {
            get { return st; }
            set { st = value; }
        }
        public ATTRIBUTE_DATA Attribute_Data
        {
            get { return attributeData; }
            set { attributeData = value; }
        }
        public INDEX_ALLOCATION Indx
        {
            get { return indx; }
            set { indx = value; }
        }
        public OBJECT_ID Object_Id
        {
            get { return obId; }
            set { obId = value; }
        }
        public SECURITY_DESCRIPTOR Security
        {
            get { return security; }
            set { security = value; }
        }
        public List<ATTRIBUTE_LIST_ENTRY> Attribute_list
        {
            get { return attribute_list; }
            set { attribute_list = value; }
        }

        #endregion

        public ATTRIBUTE_RECORD_HEADER(BitStreamReader sw, long offset)
        {         
            PositionOfStructureInFile = sw.Position + offset;
            int start = sw.Position;
            type_Code = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//
            if ((uint)type_Code.Value == 0xffffffff)
                return;
            recordLength = new ELEMENTARY_TYPE(sw, offset, typeof(uint));// (uint)sw.ReadInteger();
            form = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            formCode = (FORM_CODE)(byte)form.Value;
            nameLength = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            nameOffset = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//(ushort)sw.ReadShort();
            flags = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//(ushort)sw.ReadShort();
            attributeId = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//(ushort)sw.ReadShort();
            switch (formCode)
            {
                case FORM_CODE.RESIDENT_FORM:
                    #region Resident Attribute
                    resident = new RESIDENT(sw, offset, (byte)nameLength.Value);
                    switch ((ATTRIBUTE_TYPE_CODE)(uint)type_Code.Value)
                    {
                        case ATTRIBUTE_TYPE_CODE.FILE_NAME:
                            f = new FILE_NAME(sw, offset);
                            break;
                        case ATTRIBUTE_TYPE_CODE.VOLUME_NAME:
                            v = new VOLUME_NAME(sw, offset, (int)(uint)resident.ValueLength.Value);
                            break;
                        case ATTRIBUTE_TYPE_CODE.STANDARD_INFORMATION:
                            st = new STANDARD_INFORMATION(sw, offset);
                            break;
                        case ATTRIBUTE_TYPE_CODE.INDEX_ROOT:
                            ind = new INDEX_ROOT(sw, offset);
                            if (sw.Position < start + (uint)recordLength.Value)
                            {
                                sw.Position = start + (short)resident.ValueOffset.Value;
                                attributeData = new ATTRIBUTE_DATA(sw, offset, (uint)resident.ValueLength.Value);
                            }
                            break;
                        case ATTRIBUTE_TYPE_CODE.OBJECT_ID:
                            obId = new OBJECT_ID(sw, offset, (int)(uint)resident.ValueLength.Value);
                            break;
                        case ATTRIBUTE_TYPE_CODE.DATA:
                            attributeData = new ATTRIBUTE_DATA(sw, offset, (uint)resident.ValueLength.Value);
                            break;
                        case ATTRIBUTE_TYPE_CODE.SECURITY_DESCRIPTOR:
                       //     security = new SECURITY_DESCRIPTOR(sw, Offset);
                            break;
                        case ATTRIBUTE_TYPE_CODE.ATTRIBUTE_LIST:
                            attribute_list = new List<ATTRIBUTE_LIST_ENTRY>();
                            long end = sw.Position + (uint)resident.ValueLength.Value;
                            while (sw.Position < end)
                            {
                                attribute_list.Add(new ATTRIBUTE_LIST_ENTRY(sw,offset));
                            }
                            break;
                        default:
                            sw.Position = start + (short)resident.ValueOffset.Value;
                            attributeData = new ATTRIBUTE_DATA(sw, offset, (uint)resident.ValueLength.Value);
                            break;
                    }
                    break;
                    #endregion
                case FORM_CODE.NONRESIDENT_FORM:
                    #region Non resident attribute
                    nonresident = new NONRESIDENT(sw, offset, (byte)nameLength.Value);
                    switch ((ATTRIBUTE_TYPE_CODE)(uint)type_Code.Value)
                    {
                        case ATTRIBUTE_TYPE_CODE.DATA:
                            attributeData = new ATTRIBUTE_DATA(sw, offset);
                            break;
                        case ATTRIBUTE_TYPE_CODE.INDEX_ALLOCATION:
                            indx = new INDEX_ALLOCATION(sw, offset);
                            break;
                        default:
                            attributeData = new ATTRIBUTE_DATA(sw, offset);
                            break;
                    }
                    break;
                    #endregion
            }
            sw.Position = (int)(PositionOfStructureInFile - offset + (uint)recordLength.Value);
            LengthInFile = (uint)recordLength.Value;
        }
        public override string ToString()
        {
            if (File_Name != null)
                return attributeId.ToString() + " " + (string)File_Name.FileName.Value;
            return attributeId.ToString() + " " + Attribute_Type.ToString();
        }
    }
    #region Attributes record modifiers
    public class RESIDENT : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE valueLength;//0x10
        ELEMENTARY_TYPE valueOffset;//0x14
        ELEMENTARY_TYPE indexedFlag;//0x16
        public byte[] reserved;//[1];0x17
        ELEMENTARY_TYPE attributeName; // if nameLength!=0
       public ELEMENTARY_TYPE ValueLength
        {
            get { return valueLength; }
            set { valueLength = value; }
        }
        public ELEMENTARY_TYPE ValueOffset
        {
            get { return valueOffset; }
            set { valueOffset = value; }
        }
        public ELEMENTARY_TYPE AttributeName
        {
            get { return attributeName; }
            set { attributeName = value; }
        }
        public RESIDENT(BitStreamReader sw, long offset, int nameLength)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            valueLength = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//; (uint)sw.ReadInteger();
            valueOffset = new ELEMENTARY_TYPE(sw, offset, typeof(short));//(ushort)sw.ReadShort();
            indexedFlag = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            sw.ReadBytes(1);
            if (nameLength > 0)
                attributeName = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, nameLength);//sw.ReadString(nameLength, Encoding.Unicode);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class NONRESIDENT : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE lowestVcn;//virtual cluster number (VCN)//0x10
        ELEMENTARY_TYPE highestVcn;// 0x18
        ELEMENTARY_TYPE mappingPairsOffset;//0x20
        ELEMENTARY_TYPE compressionUnitSize;//0x22
        public byte[] Padding;//[5];//0x24
        ELEMENTARY_TYPE allocatedSize;//0x28
        ELEMENTARY_TYPE realSize;//0x30
        ELEMENTARY_TYPE validDataLength;//0x38
        ELEMENTARY_TYPE totalAllocated;//0x40
        List<MAPPING_PAIR> mappingPairs = new List<MAPPING_PAIR>();
        ELEMENTARY_TYPE attributeName; // if nameLength!=0

        public ELEMENTARY_TYPE LowestVcn
        {
            get { return lowestVcn; }
            set { lowestVcn = value; }
        }
        public ELEMENTARY_TYPE HighestVcn
        {
            get { return highestVcn; }
            set { highestVcn = value; }
        }
        public ELEMENTARY_TYPE MappingPairsOffset
        {
            get { return mappingPairsOffset; }
            set { mappingPairsOffset = value; }
        }
        public ELEMENTARY_TYPE CompressionUnitSize
        {
            get { return compressionUnitSize; }
            set { compressionUnitSize = value; }
        }
        public ELEMENTARY_TYPE AllocatedLength
        {
            get { return allocatedSize; }
            set { allocatedSize = value; }
        }
        public ELEMENTARY_TYPE RealSize
        {
            get { return realSize; }
            set { realSize = value; }
        }
        public ELEMENTARY_TYPE ValidDataLength
        {
            get { return validDataLength; }
            set { validDataLength = value; }
        }
        public ELEMENTARY_TYPE TotalAllocated
        {
            get { return totalAllocated; }
            set { totalAllocated = value; }
        }
        public ELEMENTARY_TYPE AttributeName
        {
            get { return attributeName; }
            set { attributeName = value; }
        }
        public List<MAPPING_PAIR> MappingPairsArray
        {
            get { return mappingPairs; }
        }
        public int Startcluster
        {
            get { return (int)mappingPairs[0].StartCluster.Value; }
        }
        public int Numbercluster
        {
            get { return (int)mappingPairs[0].NumberOfClusters.Value; }
        }
        public NONRESIDENT(BitStreamReader sw, long offset, int nameLength)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            lowestVcn = new ELEMENTARY_TYPE(sw, offset, typeof(long));// sw.ReadLong();
            highestVcn = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            mappingPairsOffset = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//ushort)sw.ReadShort();
            compressionUnitSize = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            ELEMENTARY_TYPE a = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 5);
    //        sw.ReadBytes(5);
            if ((long)lowestVcn.Value == 0)
            {
                allocatedSize = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
                realSize = new ELEMENTARY_TYPE(sw, offset, typeof(long));// sw.ReadLong();
                validDataLength = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            }
            if ((byte)compressionUnitSize.Value != 0)
                totalAllocated = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            if (nameLength > 0)
                attributeName = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, nameLength);// sw.ReadString(nameLength, Encoding.Unicode);//new ELEMENTARY_TYPE(sw, Offset, nameLength, Encoding.Unicode);
            mappingPairs.Add(new MAPPING_PAIR(sw, offset));
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class MAPPING_PAIR : LOCALIZED_DATA
    {
        int lengthOfClusterNumber;
        int lengthOfNumberOfClusters;
        ELEMENTARY_TYPE b;
        ELEMENTARY_TYPE numberOfCLusters;
        ELEMENTARY_TYPE startCluster;
        public ELEMENTARY_TYPE Description
        {
            get { return b; }
            set { b = value; }
        }
        public ELEMENTARY_TYPE NumberOfClusters
        {
            get { return numberOfCLusters; }
            set { numberOfCLusters = value; }
        }
        public ELEMENTARY_TYPE StartCluster
        {
            get { return startCluster; }
            set { startCluster = value; }
        }
        public MAPPING_PAIR(BitStreamReader sw, long offset)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            b = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            lengthOfClusterNumber = ((byte)b.Value & 0xf0) / 0x10;
            lengthOfNumberOfClusters = (byte)b.Value & 0x0f;
            numberOfCLusters = new ELEMENTARY_TYPE(sw, offset, typeof(int), lengthOfNumberOfClusters);//(int)sw.ReadIntegerFromBytes(lengthOfNumberOfClusters);
            startCluster = new ELEMENTARY_TYPE(sw, offset, typeof(int), lengthOfClusterNumber);//(int)sw.ReadIntegerFromBytes(lengthOfClusterNumber);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return ((int)startCluster.Value).ToString("x2") + ", " + ((int)numberOfCLusters.Value).ToString("x2");
        }
    }
    #endregion
    public class ATTRIBUTE_DATA : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE attributeData;

        public ELEMENTARY_TYPE Attribute_Data
        {
            get { return attributeData; }
            set { attributeData = value; }
        }
        public List<string> Data_To_String
        {
            get
            {
                if (attributeData == null)
                    return null;
                List<string> s = new List<string>();

                BitStreamReader bs = new BitStreamReader((byte[])attributeData.Value, false);
                while (bs.Position < bs.Length)
                {
                    s.Add(bs.ReadStringZ(Encoding.Default));
                }
                return s;
            }
        }
        public ATTRIBUTE_DATA(BitStreamReader sw, long offset, uint valueLength)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            attributeData = new ELEMENTARY_TYPE(sw,offset,typeof(byte[]),(int)valueLength);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public ATTRIBUTE_DATA(BitStreamReader sw, long offset)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class ATTRIBUTE_LIST_ENTRY : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE attribute_Type;// 0x00 4 Type int
        ELEMENTARY_TYPE record_length;//0x04 2 short*
        ELEMENTARY_TYPE name_length;// (N) 0x06 1byte
        ELEMENTARY_TYPE offset_to_Name;// (a) 0x07 1
        ELEMENTARY_TYPE starting_VCN;// (b) 0x08 8
        MFT_SEGMENT_REFERENCE base_File_Reference_of_the_attribute;// 0x10 8*
        ELEMENTARY_TYPE attribute_Id;// (c) 0x18 2
        ELEMENTARY_TYPE name;// in Unicode (if N > 0) 0x1A 2N 
 
        public ATTRIBUTE_TYPE_CODE Attribute
        {
            get { return (ATTRIBUTE_TYPE_CODE)(int)attribute_Type.Value; }
        }
        public ELEMENTARY_TYPE Attribute_Type
        {
            get { return attribute_Type; }
            set { attribute_Type = value; }
        }
        public ELEMENTARY_TYPE Record_length
        {
            get { return record_length; }
            set { record_length = value; }
        }
        public ELEMENTARY_TYPE Name_length
        {
            get { return name_length; }
            set { name_length = value; }
        }
        public ELEMENTARY_TYPE Offset_to_Name
        {
            get { return offset_to_Name; }
            set { offset_to_Name = value; }
        }
        public ELEMENTARY_TYPE Starting_VCN
        {
            get { return starting_VCN; }
            set { starting_VCN = value; }
        }
        public MFT_SEGMENT_REFERENCE Base_File_Reference_of_the_attribute
        {
            get { return base_File_Reference_of_the_attribute; }
            set { base_File_Reference_of_the_attribute = value; }
        }
        public ELEMENTARY_TYPE Attribute_Id
        {
            get { return attribute_Id; }
            set { attribute_Id = value; }
        }
        public ELEMENTARY_TYPE Name
        {
            get { return name; }
            set { name = value; }
        }

        public ATTRIBUTE_LIST_ENTRY(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            attribute_Type= new ELEMENTARY_TYPE(sw,offset,typeof(int));
            record_length = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            name_length = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            offset_to_Name = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            starting_VCN = new ELEMENTARY_TYPE(sw, offset, typeof(long));
            base_File_Reference_of_the_attribute = new MFT_SEGMENT_REFERENCE(sw, offset);
            attribute_Id = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            if ((byte)name_length.Value > 0)
                name = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, (byte)name_length.Value);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
            if (LengthInFile % 8 > 0)
            {
                sw.ReadBytes(8 - ((int)LengthInFile % 8));
            }
        }
        public override string ToString()
        {
            return Attribute + " " + Name; ;
        }
    }
    public class BITMAP : LOCALIZED_DATA
    {
        public BITMAP(BitStreamReader sw, long offset)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class EA : LOCALIZED_DATA
    {
        public EA(BitStreamReader sw, long offset)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class EA_INFORMATION : LOCALIZED_DATA
    {
        public EA_INFORMATION(BitStreamReader sw, long offset)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    #region Security
    public class SECURITY_DESCRIPTOR : LOCALIZED_DATA
    {
        SECURITY_HEADER header;
        ACL auditing_ACL;
        ACL permissions_ACL;

        public SECURITY_HEADER Header
        {
            get { return header; }
            set { header = value; }
        }
        public ACL Auditing_ACL
        {
            get { return auditing_ACL; }
            set { auditing_ACL = value; }
        }
        public ACL Permissions_ACL
        {
            get { return permissions_ACL; }
            set { permissions_ACL = value; }
        }
        public SECURITY_DESCRIPTOR(BitStreamReader sw, long offset)
        {

            PositionOfStructureInFile = sw.Position + offset;
            header = new SECURITY_HEADER(sw, offset);
            auditing_ACL = new ACL(sw, offset);
            permissions_ACL = new ACL(sw, offset);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class SECURITY_HEADER : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE revision;// 0x00 1

        public ELEMENTARY_TYPE Revision
        {
            get { return revision; }
            set { revision = value; }
        }
        byte Padding;// 0x01 1 
        ELEMENTARY_TYPE control_Flags;// 0x02 2 (b) 

        public ELEMENTARY_TYPE Control_Flags
        {
            get { return control_Flags; }
            set { control_Flags = value; }
        }
        ELEMENTARY_TYPE offset_to_User_SID;// 0x04 4
        ELEMENTARY_TYPE offset_to_Group_SID;// 0x08 4
        ELEMENTARY_TYPE offset_to_SACL;// 0x0C 4
        ELEMENTARY_TYPE offset_to_DACL;// 0x10 4

        public ELEMENTARY_TYPE Offset_to_User_SID
        {
            get { return offset_to_User_SID; }
            set { offset_to_User_SID = value; }
        }
        public ELEMENTARY_TYPE Offset_to_Group_SID
        {
            get { return offset_to_Group_SID; }
            set { offset_to_Group_SID = value; }
        }
        public ELEMENTARY_TYPE Offset_to_SACL
        {
            get { return offset_to_SACL; }
            set { offset_to_SACL = value; }
        }
        public ELEMENTARY_TYPE Offset_to_DACL
        {
            get { return offset_to_DACL; }
            set { offset_to_DACL = value; }
        }
        public SECURITY_HEADER(BitStreamReader sw, long offset)
        {

            PositionOfStructureInFile = sw.Position + offset;
            revision = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            control_Flags = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            offset_to_User_SID = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            offset_to_Group_SID = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            offset_to_SACL = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            offset_to_DACL = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class ACL : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE aCL_Revision;//0x00)
        byte Padding;//  
        ELEMENTARY_TYPE aCL_size; //0x02 2
        ELEMENTARY_TYPE aCE_count;//0x04 2  
        byte[] Padding2;//2 
        List<ACE> aCE;

        public ELEMENTARY_TYPE ACL_Revision
        {
            get { return aCL_Revision; }
            set { aCL_Revision = value; }
        }
        public ELEMENTARY_TYPE ACL_size
        {
            get { return aCL_size; }
            set { aCL_size = value; }
        }
        public ELEMENTARY_TYPE ACE_count
        {
            get { return aCE_count; }
            set { aCE_count = value; }
        }
        public List<ACE> ACE
        {
            get { return aCE; }
            set { aCE = value; }
        }
        public ACL(BitStreamReader sw, long offset)
        {
            aCE = new List<ACE>();
            PositionOfStructureInFile = sw.Position + offset;
            aCL_Revision = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            sw.ReadByte();
            aCL_size = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            sw.ReadBytes(2);
            aCE_count = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            for (int i = 0; i < (short)aCE_count.Value; i++)
                aCE.Add(new ACE(sw, offset));
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class ACE : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE aCE_Type;// 0x00 1 
        ELEMENTARY_TYPE flags;// 0x01 1
        ELEMENTARY_TYPE size;// 0x02 2
        ELEMENTARY_TYPE access_mask;// 0x04 4
        ELEMENTARY_TYPE sID;// 0x08 V


        public ELEMENTARY_TYPE ACE_Type
        {
            get { return aCE_Type; }
            set { aCE_Type = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public ELEMENTARY_TYPE Size
        {
            get { return size; }
            set { size = value; }
        }
        public ELEMENTARY_TYPE Access_mask
        {
            get { return access_mask; }
            set { access_mask = value; }
        }
        public ELEMENTARY_TYPE SID
        {
            get { return sID; }
            set { sID = value; }
        }
        public ACE(BitStreamReader sw, long offset)
        {

            PositionOfStructureInFile = sw.Position + offset;
            aCE_Type = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            flags = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            size = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            access_mask = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }

    #endregion
    public class FILE_NAME : LOCALIZED_DATA
    {
        MFT_SEGMENT_REFERENCE parentDirectory;
        ELEMENTARY_TYPE creationTime;//
        ELEMENTARY_TYPE lastModificationTime;//
        ELEMENTARY_TYPE lastChangeTime;//
        ELEMENTARY_TYPE lastAccessTime;//
        ELEMENTARY_TYPE allocatedLength;//
        ELEMENTARY_TYPE fileSize;//
        ELEMENTARY_TYPE fileAttributes;//
        ELEMENTARY_TYPE nameLength;
        ELEMENTARY_TYPE fileNameNamespace;
        ELEMENTARY_TYPE fileName;

        public MFT_SEGMENT_REFERENCE ParentDirectory
        {
            get { return parentDirectory; }
            set { parentDirectory = value; }
        }
        public ELEMENTARY_TYPE CreationTime
        {
            get { return creationTime; }
        }
        public ELEMENTARY_TYPE LastModificationTime
        {
            get { return lastModificationTime; }
        }
        public ELEMENTARY_TYPE LastChangeTime
        {
            get { return lastChangeTime; }
        }
        public ELEMENTARY_TYPE LastAccessTime
        {
            get { return lastAccessTime; }
        }
        public ELEMENTARY_TYPE AllocatedLength
        {
            get { return allocatedLength; }
            set { allocatedLength = value; }
        }
        public ELEMENTARY_TYPE FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }
        public ELEMENTARY_TYPE FileAttributes
        {
            get { return fileAttributes; }
            set { fileAttributes = value; }
        }
        public ELEMENTARY_TYPE NameLength
        {
            get { return nameLength; }
            set { nameLength = value; }
        }
        public FILENAME_NAMESPACE FileNameNamespace
        { get { return (FILENAME_NAMESPACE)(byte)fileNameNamespace.Value; } }
        public ELEMENTARY_TYPE FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public bool isReadOnly
        { get { return ((int)fileAttributes.Value & 0x001) == 0x0001; } }
        public bool isHidden
        { get { return ((int)fileAttributes.Value & 0x002) == 0x0002; } }
        public bool isSystem
        { get { return ((int)fileAttributes.Value & 0x004) == 0x0004; } }
        public bool isArchive
        { get { return ((int)fileAttributes.Value & 0x020) == 0x0020; } }
        public bool isDevice
        { get { return ((int)fileAttributes.Value & 0x040) == 0x0040; } }
        public bool isNormal
        { get { return ((int)fileAttributes.Value & 0x080) == 0x0080; } }
        public bool isTemporary
        { get { return ((int)fileAttributes.Value & 0x100) == 0x0100; } }
        public bool isSparseFile
        { get { return ((int)fileAttributes.Value & 0x200) == 0x0200; } }
        public bool isReparsePoint
        { get { return ((int)fileAttributes.Value & 0x400) == 0x0400; } }
        public bool isCompressed
        { get { return ((int)fileAttributes.Value & 0x800) == 0x0800; } }
        public bool isOffLine
        { get { return ((int)fileAttributes.Value & 0x1000) == 0x1000; } }
        public bool isNotContentIndexed
        { get { return ((int)fileAttributes.Value & 0x2000) == 0x2000; } }
        public bool isEncrypted
        { get { return ((int)fileAttributes.Value & 0x4000) == 0x4000; } }
        public bool isDirectory
        { get { return ((int)fileAttributes.Value & 0x10000000) == 0x10000000; } }
        public bool isFile
        { get { return !isDirectory; } }
        public bool isIndexView
        { get { return ((int)fileAttributes.Value & 0x20000000) == 0x20000000; } }
        public FILE_NAME(BitStreamReader sw, long offset)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            parentDirectory = new MFT_SEGMENT_REFERENCE(sw, offset);
            creationTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));// sw.ReadLong();
            lastModificationTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));// sw.ReadLong();
            lastChangeTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));// sw.ReadLong();
            lastAccessTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));// sw.ReadLong();
            allocatedLength = new ELEMENTARY_TYPE(sw, offset, typeof(long));// sw.ReadLong();
            fileSize = new ELEMENTARY_TYPE(sw, offset, typeof(long));// sw.ReadLong();
            fileAttributes = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            sw.ReadInteger();
            nameLength = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            fileNameNamespace = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            if ((byte)nameLength.Value > 0)
                fileName = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, (byte)nameLength.Value);// sw.ReadString(nameLength, Encoding.Unicode);
             LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return (string)fileName.Value;
        }
    }
    #region Indexes
    public class INDX : LOCALIZED_DATA
    {
        MULTI_SECTOR_HEADER header;
        ELEMENTARY_TYPE vcn;
        INDEX_HEADER index;
        List<INDEX_ENTRY> entries = new List<INDEX_ENTRY>();

        public MULTI_SECTOR_HEADER Header
        {
            get { return header; }
            set { header = value; }
        }
        public ELEMENTARY_TYPE VCN
        {
            get { return vcn; }
            set { vcn = value; }
        }
        public INDEX_HEADER indexHeader
        {
            get { return index; }
            set { index = value; }
        }
        public List<INDEX_ENTRY> Entries
        {
            get { return entries; }
            set { entries = value; }
        }
        public INDX(BitStreamReader sw, long offset)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            header = new MULTI_SECTOR_HEADER(sw, offset);
            vcn = new ELEMENTARY_TYPE(sw, offset, typeof(long));// sw.ReadLong();
            int start = sw.Position;
            indexHeader = new INDEX_HEADER(sw, offset);
            sw.Position = (int)indexHeader.Offset_to_first_Index_Entry.Value + start;
            while (sw.Position < start + (int)indexHeader.Total_size_of_Index_Entries.Value)
            {
                entries.Add(new INDEX_ENTRY(sw, offset));
            }

            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Index"; ;
        }
    }
    public class INDEX_ALLOCATION  : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE file_reference;
        ELEMENTARY_TYPE lengthOfIndexEntry;
        ELEMENTARY_TYPE lengthOfStream;
       ELEMENTARY_TYPE flags;

        public ELEMENTARY_TYPE File_reference
        {
            get { return file_reference; }
            set { file_reference = value; }
        }

        public ELEMENTARY_TYPE LengthOfIndexEntry
        {
            get { return lengthOfIndexEntry; }
            set { lengthOfIndexEntry = value; }
        }
        public ELEMENTARY_TYPE LengthOfStream
        {
            get { return lengthOfStream; }
            set { lengthOfStream = value; }
        }
         public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
         public INDEX_ALLOCATION(BitStreamReader sw, long offset)
         {
            
             PositionOfStructureInFile = sw.Position + offset;
             file_reference = new ELEMENTARY_TYPE(sw, offset, typeof(long));
             lengthOfIndexEntry = new ELEMENTARY_TYPE(sw, offset, typeof(short));
             lengthOfStream = new ELEMENTARY_TYPE(sw, offset, typeof(short));
             flags = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
             LengthInFile = sw.Position + offset - PositionOfStructureInFile;
         }
         public override string ToString()
         {
             return "";
         }
    }
    public class INDEX_ROOT : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE attributeType;
        ELEMENTARY_TYPE collation_Rule;

        ELEMENTARY_TYPE size_of_Index_Allocation_Entry;// BytesPerIndexBuffer; 
        ELEMENTARY_TYPE clusters_per_Index_Record;//Blocks Per indexHeader Buffer;
        List<INDEX_ENTRY> entries;
        ELEMENTARY_TYPE padding;// 3 bytes (Align to 8 bytes) 

        INDEX_HEADER indexHeader;
        public ELEMENTARY_TYPE AttributeType
        {
            get { return attributeType; }
        }
        public ELEMENTARY_TYPE CollationRule
        {
            get { return collation_Rule; }
            set { collation_Rule = value; }
        }
        public COLLATION_RULE Collation_Rule
        {
            get { return (COLLATION_RULE)(int)collation_Rule.Value; }
        }
        public ELEMENTARY_TYPE Size_of_Index_Allocation_Entry
        {
            get { return size_of_Index_Allocation_Entry; }
            set { size_of_Index_Allocation_Entry = value; }
        }
        public ELEMENTARY_TYPE Clusters_per_Index_Record
        {
            get { return clusters_per_Index_Record; }
            set { clusters_per_Index_Record = value; }
        }
        public ELEMENTARY_TYPE Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        public INDEX_HEADER IndexHeader
        {
            get { return indexHeader; }
            set { indexHeader = value; }
        }
        public List<INDEX_ENTRY> Index_Entries
        {
            get { return entries; }
            set { entries = value; }
        }
        public INDEX_ROOT(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            attributeType = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, 2);//sw.ReadString(2,Encoding.Unicode);
            collation_Rule = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            size_of_Index_Allocation_Entry = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            clusters_per_Index_Record = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            padding = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 3);//sw.ReadBytes(3);
            int start = sw.Position;
            indexHeader = new INDEX_HEADER(sw, offset);
            if (sw.Position < start + (int)indexHeader.Total_size_of_Index_Entries.Value)
            {
                entries = new List<INDEX_ENTRY>();
                while (sw.Position < start + (int)indexHeader.Total_size_of_Index_Entries.Value)
                {
                    entries.Add(new INDEX_ENTRY(sw, offset));
                }
            }
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class INDEX_HEADER : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE offset_to_first_Index_Entry;//First entry
        ELEMENTARY_TYPE total_size_of_Index_Entries;// first free byte
        ELEMENTARY_TYPE allocated_size_of_the_Index_Entries;// Total number of bytes available, from the start of the first  index entry.
        ELEMENTARY_TYPE flags;
        byte[] padding;// 3 bytes (Align to 8 bytes) 
        public ELEMENTARY_TYPE Offset_to_first_Index_Entry
        {
            get { return offset_to_first_Index_Entry; }
            set { offset_to_first_Index_Entry = value; }
        }
        public ELEMENTARY_TYPE Total_size_of_Index_Entries
        {
            get { return total_size_of_Index_Entries; }
            set { total_size_of_Index_Entries = value; }
        }
        public ELEMENTARY_TYPE Allocated_size_of_the_Index_Entries
        {
            get { return allocated_size_of_the_Index_Entries; }
            set { allocated_size_of_the_Index_Entries = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public INDEX_FLAGS FlagsData
        { get { return (INDEX_FLAGS)(byte)flags.Value; } }
        public INDEX_HEADER(BitStreamReader sw, long offset)
        {
            int start=sw.Position;
           
            PositionOfStructureInFile = sw.Position + offset;
            offset_to_first_Index_Entry = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            int startentries = sw.Position - 4 + (int)offset_to_first_Index_Entry.Value;
            total_size_of_Index_Entries = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            allocated_size_of_the_Index_Entries = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            flags = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            padding = sw.ReadBytes(3);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class INDEX_ENTRY : LOCALIZED_DATA
    {
        //  Define a union to distinguish directory indices from view indices
        //  Reference to file containing the attribute with this attribute value.
        MFT_SEGMENT_REFERENCE fileReference;      //  Offset = 0x000
        //  Length of this index entry, in bytes.
        ELEMENTARY_TYPE length;                   //  Offset = 0x008
        //  Length of attribute value, in bytes.  The attribute value  immediately follows this record.
        ELEMENTARY_TYPE attributeLength;          //  Offset = 0x00A
        //
        //  INDEX_ENTRY_xxx Flags_Meaning.
        //
        ELEMENTARY_TYPE flags;                    //  Offset = 0x00C
        ushort padding;                           //  Offset = 0x00E
        INDEX_ENTRY_DATA att;

        public MFT_SEGMENT_REFERENCE FileReference
        {
            get { return fileReference; }
            set { fileReference = value; }
        }
        public ELEMENTARY_TYPE Length
        {
            get { return length; }
            set { length = value; }
        }
        public ELEMENTARY_TYPE AttributeLength
        {
            get { return attributeLength; }
            set { attributeLength = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public INDEX_ENTRY_FLAGS FlagsData
        {
            get { return (INDEX_ENTRY_FLAGS)(ushort)flags.Value; }
        }
        public INDEX_ENTRY_DATA Attribute_Data
        {
            get { return att; }
            set { att = value; }
        }
        public INDEX_ENTRY(BitStreamReader sw, long offset)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            int start = sw.Position;
            fileReference = new MFT_SEGMENT_REFERENCE(sw, offset);
            length = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));// (ushort)sw.ReadShort();
            attributeLength = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));// (ushort)sw.ReadShort();
            flags = new ELEMENTARY_TYPE(sw, offset, typeof(ushort));//(ushort)sw.ReadShort();
            padding = (ushort)sw.ReadShort();
            if ((ushort)attributeLength.Value > 0)
            {
                try
                {
                    att = new INDEX_ENTRY_DATA(sw, offset);
                }
                catch { }
            }
            sw.Position = start + (ushort)length.Value;
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            if (Attribute_Data != null)
                return Attribute_Data.ToString();
            else
                return "";
        }
    }
    public class INDEX_ENTRY_DATA : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE referenceToParentDirectory;
        ELEMENTARY_TYPE creationTime;                                          //  Offset = 0x000
        ELEMENTARY_TYPE lastModificationTime;                                  //  Offset = 0x008
        ELEMENTARY_TYPE lastChangeTime;                                        //  Offset = 0x010
        ELEMENTARY_TYPE lastAccessTime;                                        //  Offset = 0x018
        ELEMENTARY_TYPE physicalSize; //long physicalSize;
        ELEMENTARY_TYPE logicalSize; //long logicalSize;
        ELEMENTARY_TYPE flags;

        ELEMENTARY_TYPE extendedAttributes;
        ELEMENTARY_TYPE nameLength;
        ELEMENTARY_TYPE fileType;
        ELEMENTARY_TYPE fileName;
        public ELEMENTARY_TYPE ReferenceToParentDirectory
        {
            get { return referenceToParentDirectory; }
            set { referenceToParentDirectory = value; }
        }
        public ELEMENTARY_TYPE CreationTime
        {
            get { return creationTime; }
        }
        public ELEMENTARY_TYPE LastModificationTime
        {
            get { return lastModificationTime; }
        }
        public ELEMENTARY_TYPE LastChangeTime
        {
            get { return lastChangeTime; }
        }
        public ELEMENTARY_TYPE LastAccessTime
        {
            get { return lastAccessTime; }
        }
        public ELEMENTARY_TYPE PhysicalSize
        {
            get { return physicalSize; }
            set { physicalSize = value; }
        }
        public ELEMENTARY_TYPE LogicalSize
        {
            get { return logicalSize; }
            set { logicalSize = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public INDEX_ENTRY_FLAGS FlagsData
        {
            get { return (INDEX_ENTRY_FLAGS)(int)flags.Value; }
        }
        public ELEMENTARY_TYPE ExtendedAttributes
        {
            get { return extendedAttributes; }
            set { extendedAttributes = value; }
        }
        public ELEMENTARY_TYPE NameLength
        {
            get { return nameLength; }
            set { nameLength = value; }
        }
        public ELEMENTARY_TYPE FileType
        {
            get { return fileType; }
            set { fileType = value; }
        }
        public ELEMENTARY_TYPE FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public INDEX_ENTRY_DATA(BitStreamReader sw, long offset)
        {

            PositionOfStructureInFile = sw.Position + offset;
            referenceToParentDirectory = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            try
            {
                creationTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));//sw.ReadLong();
            }
            catch { }
            try
            {
                lastModificationTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));//sw.ReadLong();
            }
            catch { }
            try
            {
                lastChangeTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));//sw.ReadLong();
            }
            catch { }
            try
            {
                lastAccessTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));//sw.ReadLong();
            }
            catch { }
            physicalSize = new ELEMENTARY_TYPE(sw, offset, typeof(long));// sw.ReadLong();
            logicalSize = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            flags = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            extendedAttributes = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            nameLength = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//.ReadByte();
            fileType = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            fileName = new ELEMENTARY_TYPE(sw, offset, Encoding.Unicode, (byte)nameLength.Value);//sw.ReadString((byte)nameLength.Value, Encoding.Unicode);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return (string)FileName.Value;
        }
     }
    #endregion
    public class LOGGED_UTILITY_STREAM  : LOCALIZED_DATA
    {
        public LOGGED_UTILITY_STREAM(BitStreamReader sw, long offset)
        {          
            PositionOfStructureInFile = sw.Position + offset;
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class OBJECT_ID : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE object_Id;
        ELEMENTARY_TYPE birth_Volume_Id;// Volume where file was created 
        ELEMENTARY_TYPE birth_Object_Id;// Original Object Id of file 
        ELEMENTARY_TYPE domain_Id;// Domain in which object was created 

        public ELEMENTARY_TYPE Object_Id
        {
            get { return object_Id; }
            set { object_Id = value; }
        }
        public ELEMENTARY_TYPE Birth_Volume_Id
        {
            get { return birth_Volume_Id; }
            set { birth_Volume_Id = value; }
        }
        public ELEMENTARY_TYPE Birth_Object_Id
        {
            get { return birth_Object_Id; }
            set { birth_Object_Id = value; }
        }
        public ELEMENTARY_TYPE Domain_Id
        {
            get { return domain_Id; }
            set { domain_Id = value; }
        }
        public OBJECT_ID(BitStreamReader sw, long offset, int length)
        {

            PositionOfStructureInFile = sw.Position + offset;
            if (length >= 0x10)
                object_Id = new ELEMENTARY_TYPE(sw, offset, typeof(Guid));
            if (length >= 0x20)
                birth_Volume_Id = new ELEMENTARY_TYPE(sw, offset, typeof(Guid));
            if (length >= 0x30)
                birth_Object_Id = new ELEMENTARY_TYPE(sw, offset, typeof(Guid));
            if (length >= 0x40)
                domain_Id = new ELEMENTARY_TYPE(sw, offset, typeof(Guid));
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class REPARSE_POINT  : LOCALIZED_DATA
    {
        public REPARSE_POINT(BitStreamReader sw, long offset)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class STANDARD_INFORMATION : LOCALIZED_DATA //  sizeof = 0x048
    {
        ELEMENTARY_TYPE creationTime;                                          //  Offset = 0x000
        ELEMENTARY_TYPE lastModificationTime;                                  //  Offset = 0x008
        ELEMENTARY_TYPE lastChangeTime;                                        //  Offset = 0x010
        ELEMENTARY_TYPE lastAccessTime;                                        //  Offset = 0x018
        ELEMENTARY_TYPE fileAttributes;                                           //  Offset = 0x020
        ELEMENTARY_TYPE maximumVersions;                                          //  Offset = 0x024
        ELEMENTARY_TYPE versionNumber;                                            //  Offset = 0x028
        ELEMENTARY_TYPE classId;                                                  //  Offset = 0x02c
        ELEMENTARY_TYPE ownerId;                                                  //  Offset = 0x030
        ELEMENTARY_TYPE securityId;                                               //  Offset = 0x034   
        ELEMENTARY_TYPE quotaCharged;                                         //  Offset = 0x038
        ELEMENTARY_TYPE usn;                                                  //  Offset = 0x040  

        public ELEMENTARY_TYPE CreationTime
        {
            get { return creationTime; }
        }
        public ELEMENTARY_TYPE LastModificationTime
        {
            get { return lastModificationTime; }
        }
        public ELEMENTARY_TYPE LastChangeTime
        {
            get { return lastChangeTime; }
        }
        public ELEMENTARY_TYPE LastAccessTime
        {
            get { return lastAccessTime; }
        }
        public ELEMENTARY_TYPE FileAttributes
        {
            get { return fileAttributes; }
            set { fileAttributes = value; }
        }
        public ELEMENTARY_TYPE MaximumVersions
        {
            get { return maximumVersions; }
            set { maximumVersions = value; }
        }
        public ELEMENTARY_TYPE VersionNumber
        {
            get { return versionNumber; }
            set { versionNumber = value; }
        }
        public ELEMENTARY_TYPE ClassId
        {
            get { return classId; }
            set { classId = value; }
        }
        public ELEMENTARY_TYPE OwnerId
        {
            get { return ownerId; }
            set { ownerId = value; }
        }
        public ELEMENTARY_TYPE SecurityId
        {
            get { return securityId; }
            set { securityId = value; }
        }
        public ELEMENTARY_TYPE QuotaCharged
        {
            get { return quotaCharged; }
            set { quotaCharged = value; }
        }
        public ELEMENTARY_TYPE Usn
        {
            get { return usn; }
            set { usn = value; }
        }
        public bool isReadOnly
        { get { return ((uint)fileAttributes.Value & 0x001) == 0x0001; } }
        public bool isHidden
        { get { return ((uint)fileAttributes.Value & 0x002) == 0x0002; } }
        public bool isSystem
        { get { return ((uint)fileAttributes.Value & 0x004) == 0x0004; } }
        public bool isArchive
        { get { return ((uint)fileAttributes.Value & 0x020) == 0x0020; } }
        public bool isDevice
        { get { return ((uint)fileAttributes.Value & 0x040) == 0x0040; } }
        public bool isNormal
        { get { return ((uint)fileAttributes.Value & 0x080) == 0x0080; } }
        public bool isTemporary
        { get { return ((uint)fileAttributes.Value & 0x100) == 0x0100; } }
        public bool isSparseFile
        { get { return ((uint)fileAttributes.Value & 0x200) == 0x0200; } }
        public bool isReparsePoint
        { get { return ((uint)fileAttributes.Value & 0x400) == 0x0400; } }
        public bool isCompressed
        { get { return ((uint)fileAttributes.Value & 0x800) == 0x0800; } }
        public bool isOffLine
        { get { return ((uint)fileAttributes.Value & 0x1000) == 0x1000; } }
        public bool isNotContentIndexed
        { get { return ((uint)fileAttributes.Value & 0x2000) == 0x2000; } }
        public bool isEncrypted
        { get { return ((uint)fileAttributes.Value & 0x4000) == 0x4000; } }

        public STANDARD_INFORMATION(BitStreamReader sw, long offset)
        {

            PositionOfStructureInFile = sw.Position + offset;
            creationTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));// sw.ReadLong();
            lastModificationTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));// sw.ReadLong();
            lastChangeTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));// sw.ReadLong();
            lastAccessTime = new ELEMENTARY_TYPE(sw, offset, typeof(DateTime));// sw.ReadLong();
            fileAttributes = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//(uint)sw.ReadInteger();
            maximumVersions = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//(uint)sw.ReadInteger();
            versionNumber = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//(uint)sw.ReadInteger();
            classId = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//(uint)sw.ReadInteger();
            ownerId = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//(uint)sw.ReadInteger();
            securityId = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//(uint)sw.ReadInteger();
            quotaCharged = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            usn = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
        public DateTime ToDateTime(long shift)
        {
            DateTime d = new DateTime(1601, 1, 1, 1, 0, 1);//"12:00 A.M. January 1, 1601 ";
            TimeSpan ts = new TimeSpan(shift);
            d = d + ts;
            return d;
        }
    }
    public class VOLUME_NAME : LOCALIZED_DATA
    {
        string volumeName;
        public string VolumeName
        {
            get { return volumeName; }
            set { volumeName = value; }
        }
        public VOLUME_NAME(BitStreamReader sw, long off, int length)
        {
           
            PositionOfStructureInFile = sw.Position + off;
            volumeName = sw.ReadString(length / 2, Encoding.Unicode);//new ELEMENTARY_TYPE(sw, Offset, length / 2, Encoding.Unicode);//
            LengthInFile = sw.Position + off - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return (string)volumeName;
        }
    }
    public class VOLUME_INFORMATION : LOCALIZED_DATA
    {
        long reserved;
        byte majorVersionNumber;//0x08
        byte minorVersionNumber;//0x09
        ushort flags;//0x0A;
        int reserved2;
        public byte MajorVersionNumber
        {
            get { return majorVersionNumber; }
            set { majorVersionNumber = value; }
        }

        public byte MinorVersionNumber
        {
            get { return minorVersionNumber; }
            set { minorVersionNumber = value; }
        }

        public VOLUME_INFORMATION_FLAGS Flags
        {
            //To do
            get { return (VOLUME_INFORMATION_FLAGS)flags; }
        }
        public VOLUME_INFORMATION(BitStreamReader sw, long offset)
        {
           
            PositionOfStructureInFile = sw.Position + offset;
            reserved = sw.ReadLong();
            majorVersionNumber = sw.ReadByte();
            minorVersionNumber = sw.ReadByte();
            flags = (ushort)sw.ReadShort();
            reserved2 = sw.ReadInteger();
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
}
