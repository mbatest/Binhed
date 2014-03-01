using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
/*
 The first 32768 bytes of the Volume space shall not be used for the
recording of ECMA 167 structures. This area shall not be
referenced by the Unallocated Space Descriptor or any other
ECMA 167 descriptor. This is intended for use by the native
operating system.
 */
namespace LowLevel
{
    /*
    * ecma_167.h
   */
    /* Character set specification (ECMA 167r3 1/7.2.1) 
     http://www.ecma-international.org/publications/files/ECMA-ST/Ecma-119.pdf
    */
    public class charspec
    {
        byte charSetType;
        byte[] charSetInfo;//[63];
    }
    public class Constantes
    {
        /* Character Set Type (ECMA 167r3 1/7.2.1.1) */
        public static byte CHARSPEC_TYPE_CS0 = 0x00;  /* (1/7.2.2) */
        public static byte CHARSPEC_TYPE_CS1 = 0x01; /* (1/7.2.3) */
        public static byte CHARSPEC_TYPE_CS2 = 0x02;/* (1/7.2.4) */
        public static byte CHARSPEC_TYPE_CS3 = 0x03;/* (1/7.2.5) */
        public static byte CHARSPEC_TYPE_CS4 = 0x04;   /* (1/7.2.6) */
        public static byte CHARSPEC_TYPE_CS5 = 0x05;   /* (1/7.2.7) */
        public static byte CHARSPEC_TYPE_CS6 = 0x06;   /* (1/7.2.8) */
        public static byte CHARSPEC_TYPE_CS7 = 0x07;  /* (1/7.2.9) */
        public static byte CHARSPEC_TYPE_CS8 = 0x08; /* (1/7.2.10) */
        /* Standard Identifier (EMCA 167r2 2/9.1.2) */
        public static string VSD_STD_ID_NSR02 = "NSR02";/* (3/9.1) */
        /* Standard Identifier (ECMA 167r3 2/9.1.2) */
        public static string VSD_STD_ID_BEA01 = "BEA01";/* (2/9.2) */
        public static string VSD_STD_ID_BOOT2 = "BOOT2";/* (2/9.4) */
        public static string VSD_STD_ID_CD001 = "CD001";/* (ECMA-119) */
        public static string VSD_STD_ID_CDW02 = "CDW02";/* (ECMA-168) */
        public static string VSD_STD_ID_NSR03 = "NSR03"; /* (3/9.1) */
        public static string VSD_STD_ID_TEA01 = "TEA01";/* (2/9.3) */
        /* Partition Contents (ECMA 167r2 3/10.5.3) */
        public static string PD_PARTITION_CONTENTS_NSR02 = "+NSR02";
        /* Partition Contents (ECMA 167r3 3/10.5.5) */
        public static string PD_PARTITION_CONTENTS_FDC01 = "+FDC01";
        public static string PD_PARTITION_CONTENTS_CD001 = "+CD001";
        public static string PD_PARTITION_CONTENTS_CDW02 = "+CDW02";
        public static string PD_PARTITION_CONTENTS_NSR03 = "+NSR03";
        /* Partition Map Type (ECMA 167r3 3/10.7.1.1) */
        public static ushort GP_PARTITION_MAP_TYPE_UNDEF = 0x00;
        public static ushort GP_PARTIITON_MAP_TYPE_1 = 0x01;
        public static ushort GP_PARTITION_MAP_TYPE_2 = 0x02;
        /* Tag Identifier (ECMA 167r3 3/7.2.1) */
        public static ushort TAG_IDENT_PVD = 0x0001;
        public static ushort TAG_IDENT_AVDP = 0x0002;
        public static ushort TAG_IDENT_VDP = 0x0003;
        public static ushort TAG_IDENT_IUVD = 0x0004;
        public static ushort TAG_IDENT_PD = 0x0005;
        public static ushort TAG_IDENT_LVD = 0x0006;
        public static ushort TAG_IDENT_USD = 0x0007;
        public static ushort TAG_IDENT_TD = 0x0008;
        public static ushort TAG_IDENT_LVID = 0x0009;
        /* Tag Identifier (ECMA 167r3 4/7.2.1) */
        public static ushort TAG_IDENT_FSD = 0x0100;
        public static ushort TAG_IDENT_FID = 0x0101;
        public static ushort TAG_IDENT_AED = 0x0102;
        public static ushort TAG_IDENT_IE = 0x0103;
        public static ushort TAG_IDENT_TE = 0x0104;
        public static ushort TAG_IDENT_FE = 0x0105;
        public static ushort TAG_IDENT_EAHD = 0x0106;
        public static ushort TAG_IDENT_USE = 0x0107;
        public static ushort TAG_IDENT_SBD = 0x0108;
        public static ushort TAG_IDENT_PIE = 0x0109;
        public static ushort TAG_IDENT_EFE = 0x010A;
        /* File Characteristics (ECMA 167r3 4/14.4.3) */
        public static byte FID_FILE_CHAR_HIDDEN = 0x01;
        public static byte FID_FILE_CHAR_DIRECTORY = 0x02;
        public static byte FID_FILE_CHAR_DELETED = 0x04;
        public static byte FID_FILE_CHAR_PARENT = 0x08;
        public static byte FID_FILE_CHAR_METADATA = 0x10;
        /* FileTimeExistence (ECMA 167r3 4/14.10.5.6) */
        public static uint FTE_CREATION = 0x00000001;
        public static uint FTE_DELETION = 0x00000004;
        public static uint FTE_EFFECTIVE = 0x00000008;
        public static uint FTE_BACKUP = 0x00000002;
        public static ushort EXTATTR_CHAR_SET = 1;
        public static ushort EXTATTR_ALT_PERMS = 3;
        public static ushort EXTATTR_FILE_TIMES = 5;
        public static ushort EXTATTR_INFO_TIMES = 6;
        public static ushort EXTATTR_DEV_SPEC = 12;
        public static ushort EXTATTR_IMP_USE = 2048;
        public static uint EXTATTR_APP_USE = 65536;
        /* Extent Length (ECMA 167r3 4/14.14.1.1) */
        public static uint EXT_RECORDED_ALLOCATED = 0x00000000;
        public static uint EXT_NOT_RECORDED_ALLOCATED = 0x40000000;
        public static uint EXT_NOT_RECORDED_NOT_ALLOCATED = 0x80000000;
        public static uint EXT_NEXT_EXTENT_ALLOCDECS = 0xC0000000;

        /* Type and Time Zone (ECMA 167r3 1/7.3.1) */
        public static ushort TIMESTAMP_TYPE_MASK = 0xF000;
        public static ushort TIMESTAMP_TYPE_CUT = 0x0000;
        public static ushort TIMESTAMP_TYPE_LOCAL = 0x1000;
        public static ushort TIMESTAMP_TYPE_AGREEMENT = 0x2000;
        public static ushort TIMESTAMP_TIMEZONE_MASK = 0x0FFF;
        /* Flags (ECMA 167r3 1/7.4.1) */
        public static byte ENTITYID_FLAGS_DIRTY = 0x00;
        public static byte ENTITYID_FLAGS_PROTECTED = 0x01;

        /* Volume Structure Descriptor (ECMA 167r3 2/9.1) */
        public static byte VSD_STD_ID_LEN = 5;
        /* Flags (ECMA 167r3 2/9.4.12) */
        public static byte BOOT_FLAGS_ERASE = 0x01;

        /* Extent Descriptor (ECMA 167r3 3/7.1) */
        /* Flags (ECMA 167r3 3/10.1.21) */
        public static ushort PVD_FLAGS_VSID_COMMON = 0x0001;

        /* Partition Flags (ECMA 167r3 3/10.5.3) */
        public static ushort PD_PARTITION_FLAGS_ALLOC = 0x0001;
        /* Access Type (ECMA 167r3 3/10.5.7) */
        public static uint PD_ACCESS_TYPE_NONE = 0x00000000;
        public static uint PD_ACCESS_TYPE_READ_ONLY = 0x00000001;
        public static uint PD_ACCESS_TYPE_WRITE_ONCE = 0x00000002;
        public static uint PD_ACCESS_TYPE_REWRITABLE = 0x00000003;
        public static uint PD_ACCESS_TYPE_OVERWRITABLE = 0x00000004;
        /* Integrity Type (ECMA 167r3 3/10.10.3) */
        public static uint LVID_INTEGRITY_TYPE_OPEN = 0x00000000;
        public static uint LVID_INTEGRITY_TYPE_CLOSE = 0x00000001;
        /* Strategy Type (ECMA 167r3 4/14.6.2)*/
        public static ushort ICBTAG_STRATEGY_TYPE_UNDEF = 0x0000;
        public static ushort ICBTAG_STRATEGY_TYPE_1 = 0x0001;
        public static ushort ICBTAG_STRATEGY_TYPE_2 = 0x0002;
        public static ushort ICBTAG_STRATEGY_TYPE_3 = 0x0003;
        public static ushort ICBTAG_STRATEGY_TYPE_4 = 0x0004;
        /* File Type (ECMA 167r3 4/14.6.6) */
        public static byte ICBTAG_FILE_TYPE_UNDEF = 0x00;
        public static byte ICBTAG_FILE_TYPE_USE = 0x01;
        public static byte ICBTAG_FILE_TYPE_PIE = 0x02;
        public static byte ICBTAG_FILE_TYPE_IE = 0x03;
        public static byte ICBTAG_FILE_TYPE_DIRECTORY = 0x04;
        public static byte ICBTAG_FILE_TYPE_REGULAR = 0x05;
        public static byte ICBTAG_FILE_TYPE_BLOCK = 0x06;
        public static byte ICBTAG_FILE_TYPE_CHAR = 0x07;
        public static byte ICBTAG_FILE_TYPE_EA = 0x08;
        public static byte ICBTAG_FILE_TYPE_FIFO = 0x09;
        public static byte ICBTAG_FILE_TYPE_SOCKET = 0x0A;
        public static byte ICBTAG_FILE_TYPE_TE = 0x0B;
        public static byte ICBTAG_FILE_TYPE_SYMLINK = 0x0C;
        public static byte ICBTAG_FILE_TYPE_STREAMDIR = 0x0D;

        /* Flags (ECMA 167r3 4/14.6.8) */
        public static short ICBTAG_FLAG_AD_MASK = 0x0007;
        public static short ICBTAG_FLAG_AD_SHORT = 0x0000;
        public static short ICBTAG_FLAG_AD_LONG = 0x0001;
        public static short ICBTAG_FLAG_AD_EXTENDED = 0x0002;
        public static short ICBTAG_FLAG_AD_IN_ICB = 0x0003;
        public static short ICBTAG_FLAG_SORTED = 0x0008;
        public static short ICBTAG_FLAG_NONRELOCATABLE = 0x0010;
        public static short ICBTAG_FLAG_ARCHIVE = 0x0020;
        public static short ICBTAG_FLAG_SETUID = 0x0040;
        public static short ICBTAG_FLAG_SETGID = 0x0080;
        public static short ICBTAG_FLAG_STICKY = 0x0100;
        public static short ICBTAG_FLAG_CONTIGUOUS = 0x0200;
        public static short ICBTAG_FLAG_SYSTEM = 0x0400;
        public static short ICBTAG_FLAG_TRANSFORMED = 0x0800;
        public static short ICBTAG_FLAG_MULTIVERSIONS = 0x1000;
        public static short ICBTAG_FLAG_STREAM = 0x2000;

        /* Indirect Entry (ECMA 167r3 4/14.7) */
        /* Permissions (ECMA 167r3 4/14.9.5) 
         #define FE_PERM_O_EXEC                  0x00000001U
         #define FE_PERM_O_WRITE                 0x00000002U
         #define FE_PERM_O_READ                  0x00000004U
         #define FE_PERM_O_CHATTR                0x00000008U
         #define FE_PERM_O_DELETE                0x00000010U
         #define FE_PERM_G_EXEC                  0x00000020U
         #define FE_PERM_G_WRITE                 0x00000040U
         #define FE_PERM_G_READ                  0x00000080U
         #define FE_PERM_G_CHATTR                0x00000100U
         #define FE_PERM_G_DELETE                0x00000200U
         #define FE_PERM_U_EXEC                  0x00000400U
         #define FE_PERM_U_WRITE                 0x00000800U
         #define FE_PERM_U_READ                  0x00001000U
         #define FE_PERM_U_CHATTR                0x00002000U
         #define FE_PERM_U_DELETE                0x00004000U

         /* Record Format (ECMA 167r3 4/14.9.7) */
        public static byte FE_RECORD_FMT_UNDEF = 0x00;
        public static byte FE_RECORD_FMT_FIXED_PAD = 0x01;
        public static byte FE_RECORD_FMT_FIXED = 0x02;
        public static byte FE_RECORD_FMT_VARIABLE8 = 0x03;
        public static byte FE_RECORD_FMT_VARIABLE16 = 0x04;
        public static byte FE_RECORD_FMT_VARIABLE16_MSB = 0x05;
        public static byte FE_RECORD_FMT_VARIABLE32 = 0x06;
        public static byte FE_RECORD_FMT_PRINT = 0x07;
        public static byte FE_RECORD_FMT_LF = 0x08;
        public static byte FE_RECORD_FMT_CR = 0x09;
        public static byte FE_RECORD_FMT_CRLF = 0x0A;
        public static byte FE_RECORD_FMT_LFCR = 0x0B;

        /* Record Display Attributes (ECMA 167r3 4/14.9.8) */
        public static byte FE_RECORD_DISPLAY_ATTR_UNDEF = 0x00;
        public static byte FE_RECORD_DISPLAY_ATTR_1 = 0x01;
        public static byte FE_RECORD_DISPLAY_ATTR_2 = 0x02;
        public static byte FE_RECORD_DISPLAY_ATTR_3 = 0x03;

    }
    /* Timestamp (ECMA 167r3 1/7.3) */
    public class timestamp
    {
        ushort typeAndTimezone;
        ushort year;
        byte month;
        byte day;
        byte hour;
        byte minute;
        byte second;
        byte centiseconds;
        byte hundredsOfMicroseconds;
        byte microseconds;
    }
    public class kernel_timestamp
    {
        ushort typeAndTimezone;
        short year;
        byte month;
        byte day;
        byte hour;
        byte minute;
        byte second;
        byte centiseconds;
        byte hundredsOfMicroseconds;
        byte microseconds;
    }  ;
    /* Entity identifier (ECMA 167r3 1/7.4) */
    public class regid
    {
        byte flags;
        byte[] ident;//[23];
        byte[] identSuffix;//[8];
    }  ;
    public class Directory_Record : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE length_of_Directory_Record;
        ELEMENTARY_TYPE directory_Record_Root_Directory;
        ELEMENTARY_TYPE extended_Attribute_Record_Length;
        ELEMENTARY_TYPE data_length;
        ELEMENTARY_TYPE recording_date;
        ELEMENTARY_TYPE flags;//0x2 : c'est un répertoire 
        ELEMENTARY_TYPE interleave_Gap_Size;
        ELEMENTARY_TYPE volume_Sequence_Number;
        ELEMENTARY_TYPE length_of_File_Identifier;
        ELEMENTARY_TYPE file_Identifier;
        #region Properties
        public ELEMENTARY_TYPE Length_of_Directory_Record
        {
            get { return length_of_Directory_Record; }
            set { length_of_Directory_Record = value; }
        }
        public ELEMENTARY_TYPE Extended_Attribute_Record_Length
        {
            get { return extended_Attribute_Record_Length; }
            set { extended_Attribute_Record_Length = value; }
        }
        public ELEMENTARY_TYPE Directory_Record_Root_Directory
        {
            get { return directory_Record_Root_Directory; }
            set { directory_Record_Root_Directory = value; }
        }
        public ELEMENTARY_TYPE Data_length
        {
            get { return data_length; }
            set { data_length = value; }
        }
        public ELEMENTARY_TYPE Recording_date
        {
            get { return recording_date; }
            set { recording_date = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        ELEMENTARY_TYPE file_Unit_Size;
        public ELEMENTARY_TYPE File_Unit_Size
        {
            get { return file_Unit_Size; }
            set { file_Unit_Size = value; }
        }
        public ELEMENTARY_TYPE Interleave_Gap_Size
        {
            get { return interleave_Gap_Size; }
            set { interleave_Gap_Size = value; }
        }
        public ELEMENTARY_TYPE Volume_Sequence_Number
        {
            get { return volume_Sequence_Number; }
            set { volume_Sequence_Number = value; }
        }
        public ELEMENTARY_TYPE Length_of_File_Identifier
        {
            get { return length_of_File_Identifier; }
            set { length_of_File_Identifier = value; }
        }
        public ELEMENTARY_TYPE File_Identifier
        {
            get { return file_Identifier; }
            set { file_Identifier = value; }
        }
        public string FlagValue
        {
            get
            {
                string s = "";
                if ((byte)flags.Value == 0x02)
                    s = "Directory";
                return s;
            }
            set { string a = value; }
        }
        public string Recording
        {
            get
            {
                string s = "";
                string an = (1900 + ((byte[])Recording_date.Value)[0]).ToString();
                string mois = ((byte[])Recording_date.Value)[1].ToString();
                string jour = ((byte[])Recording_date.Value)[2].ToString();
                s = jour + "/" + mois + "/" + an;
                return s;
            }

        }
        #endregion
        public Directory_Record(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            length_of_Directory_Record = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            extended_Attribute_Record_Length = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            sw.Position += 4;
            directory_Record_Root_Directory = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            sw.Position += 4;
            data_length = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            recording_date = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 7);
            flags = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//0x2 : c'est un répertoire 
            file_Unit_Size = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            interleave_Gap_Size = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            sw.Position += 2;
            volume_Sequence_Number = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            length_of_File_Identifier = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            if (((byte)length_of_File_Identifier.Value) > 1)
            {
                file_Identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, (byte)length_of_File_Identifier.Value);
            }
            if (((byte)length_of_File_Identifier.Value) > 8)
            {
                sw.Position -= (byte)length_of_File_Identifier.Value;
                file_Identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, (byte)length_of_File_Identifier.Value - 2);
            }
            sw.Position = (int)(PositionOfStructureInFile - offset) + (byte)length_of_Directory_Record.Value;
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;

        }
        public override string ToString()
        {
            return (string)file_Identifier.Value ;
        }
    }
    public class Section_Entry_Extension : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE indicator;
        ELEMENTARY_TYPE bits;
        ELEMENTARY_TYPE vendor_unique_selection;
        public Section_Entry_Extension(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            indicator = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            bits = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            vendor_unique_selection = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class Section_Entry : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE boot_indicator;

        public ELEMENTARY_TYPE Boot_indicator
        {
            get { return boot_indicator; }
            set { boot_indicator = value; }
        }
        ELEMENTARY_TYPE boot_media_type;

        public ELEMENTARY_TYPE Boot_media_type
        {
            get { return boot_media_type; }
            set { boot_media_type = value; }
        }
        ELEMENTARY_TYPE load_segment;

        public ELEMENTARY_TYPE Load_segment
        {
            get { return load_segment; }
            set { load_segment = value; }
        }
        ELEMENTARY_TYPE system_type;

        public ELEMENTARY_TYPE System_type
        {
            get { return system_type; }
            set { system_type = value; }
        }
        ELEMENTARY_TYPE unused;

        public ELEMENTARY_TYPE Unused
        {
            get { return unused; }
            set { unused = value; }
        }
        ELEMENTARY_TYPE sector_count;

        public ELEMENTARY_TYPE Sector_count
        {
            get { return sector_count; }
            set { sector_count = value; }
        }
        ELEMENTARY_TYPE load_RBA;

        public ELEMENTARY_TYPE Load_RBA
        {
            get { return load_RBA; }
            set { load_RBA = value; }
        }
        ELEMENTARY_TYPE selection_criteria;

        public ELEMENTARY_TYPE Selection_criteria
        {
            get { return selection_criteria; }
            set { selection_criteria = value; }
        }
        ELEMENTARY_TYPE vendor_unique_selection;

        public ELEMENTARY_TYPE Vendor_unique_selection
        {
            get { return vendor_unique_selection; }
            set { vendor_unique_selection = value; }
        }
        public Section_Entry(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            boot_indicator = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            boot_media_type = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            load_segment = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            system_type = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            unused = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            sector_count = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            load_RBA = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            selection_criteria = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            vendor_unique_selection = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 0x13);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class Section_Header_Entry : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE header_Indicator;

        public ELEMENTARY_TYPE Header_Indicator
        {
            get { return header_Indicator; }
            set { header_Indicator = value; }
        }
        ELEMENTARY_TYPE platform_ID;

        public ELEMENTARY_TYPE Platform_ID
        {
            get { return platform_ID; }
            set { platform_ID = value; }
        }
        ELEMENTARY_TYPE number_sections;

        public ELEMENTARY_TYPE Number_sections
        {
            get { return number_sections; }
            set { number_sections = value; }
        }
        ELEMENTARY_TYPE id_String;

        public ELEMENTARY_TYPE Id_String
        {
            get { return id_String; }
            set { id_String = value; }
        }
        public Section_Header_Entry(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            header_Indicator = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            platform_ID = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            number_sections = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            id_String = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 0x1C);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }

    public class Initial_Entry : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE boot_indicator;

        public ELEMENTARY_TYPE Boot_indicator
        {
            get { return boot_indicator; }
            set { boot_indicator = value; }
        }
        ELEMENTARY_TYPE boot_media_type;

        public ELEMENTARY_TYPE Boot_media_type
        {
            get { return boot_media_type; }
            set { boot_media_type = value; }
        }
        ELEMENTARY_TYPE load_segment;

        public ELEMENTARY_TYPE Load_segment
        {
            get { return load_segment; }
            set { load_segment = value; }
        }
        ELEMENTARY_TYPE system_type;

        public ELEMENTARY_TYPE System_type
        {
            get { return system_type; }
            set { system_type = value; }
        }
        ELEMENTARY_TYPE unused;

        public ELEMENTARY_TYPE Unused
        {
            get { return unused; }
            set { unused = value; }
        }
        ELEMENTARY_TYPE sector_count;

        public ELEMENTARY_TYPE Sector_count
        {
            get { return sector_count; }
            set { sector_count = value; }
        }
        ELEMENTARY_TYPE load_RBA;

        public ELEMENTARY_TYPE Load_RBA
        {
            get { return load_RBA; }
            set { load_RBA = value; }
        }
        ELEMENTARY_TYPE unused2;

        public ELEMENTARY_TYPE Unused2
        {
            get { return unused2; }
            set { unused2 = value; }
        }
        public Initial_Entry(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            boot_indicator = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            boot_media_type = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            load_segment = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            system_type = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            unused = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            sector_count = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            load_RBA = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            unused2 = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 20);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }

    public class Validation_Entry : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE header;
        ELEMENTARY_TYPE platform_Id;
        /*0 = 80x86
1=Power PC
2=Mac*/
        ELEMENTARY_TYPE reserved;
        ELEMENTARY_TYPE id_string;
        ELEMENTARY_TYPE checksum;
        ELEMENTARY_TYPE keyBytes;
        public ELEMENTARY_TYPE Header
        {
            get { return header; }
            set { header = value; }
        }

        public ELEMENTARY_TYPE Platform_Id
        {
            get { return platform_Id; }
            set { platform_Id = value; }
        }
        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }

        public ELEMENTARY_TYPE Id_string
        {
            get { return id_string; }
            set { id_string = value; }
        }
        public ELEMENTARY_TYPE Checksum
        {
            get { return checksum; }
            set { checksum = value; }
        }

        public ELEMENTARY_TYPE KeyBytes
        {
            get { return keyBytes; }
            set { keyBytes = value; }
        }
        public Validation_Entry(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            header = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            platform_Id = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            reserved = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            id_string = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 22);
            checksum = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            keyBytes = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]),2);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class Booting_Catalog : LOCALIZED_DATA
    {
        Validation_Entry val_entry;
        Initial_Entry in_entry;
        Section_Header_Entry header;
        List<Section_Entry> sections;

        public Validation_Entry Val_entry
        {
            get { return val_entry; }
            set { val_entry = value; }
        }

        public Initial_Entry In_entry
        {
            get { return in_entry; }
            set { in_entry = value; }
        }

        public Section_Header_Entry Header
        {
            get { return header; }
            set { header = value; }
        }

        public List<Section_Entry> Sections
        {
            get { return sections; }
            set { sections = value; }
        }
        public Booting_Catalog(BitStreamReader sw, long offset)
        {

            PositionOfStructureInFile = sw.Position + offset;
            val_entry = new Validation_Entry(sw, offset);
            in_entry = new Initial_Entry(sw, offset);
            header = new Section_Header_Entry(sw, offset);
            Sections = new List<Section_Entry>();
            do
            {
                for (int i = 0; i < (short)header.Number_sections.Value; i++)
                {
                    Section_Entry sec = new Section_Entry(sw, offset);
                    Sections.Add(sec);
                }
            }
            while ((byte)header.Header_Indicator.Value != 0x91);

                LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class CDBootRecord : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE boot_Record_Indicator;
        ELEMENTARY_TYPE identifier;
        ELEMENTARY_TYPE version;
        ELEMENTARY_TYPE boot_System_identifier;
        ELEMENTARY_TYPE unused;
        ELEMENTARY_TYPE absolute_pointer_to_boot_catalog;
        public ELEMENTARY_TYPE Boot_Record_Indicator
        {
            get { return boot_Record_Indicator; }
            set { boot_Record_Indicator = value; }
        }
        public ELEMENTARY_TYPE Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }
        public ELEMENTARY_TYPE Version
        {
            get { return version; }
            set { version = value; }
        }
        public ELEMENTARY_TYPE Boot_System_identifier
        {
            get { return boot_System_identifier; }
            set { boot_System_identifier = value; }
        }
        public ELEMENTARY_TYPE Absolute_pointer_to_boot_catalog
        {
            get { return absolute_pointer_to_boot_catalog; }
            set { absolute_pointer_to_boot_catalog = value; }
        }
        public CDBootRecord (BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            boot_Record_Indicator = new ELEMENTARY_TYPE(sw,offset,typeof(byte));
            identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 5);
            version = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            boot_System_identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 0x20);
            unused = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]),0x20);
            absolute_pointer_to_boot_catalog = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class CDDirectory : LOCALIZED_DATA
    {
        List<Directory_Record> entries;
        List<CDDirectory> subentries;

        public List<Directory_Record> Entries
        {
            get { return entries; }
            set { entries = value; }
        }
       public List<CDDirectory> Subentries
        {
            get { return subentries; }
            set { subentries = value; }
        }
        public CDDirectory(BitStreamReader sw, long offset)
        {
            entries = new List<Directory_Record>();
            subentries = new List<CDDirectory>();
            PositionOfStructureInFile = sw.Position + offset;
            while (sw.InnerBuffer[sw.Position] > 1)
            {
                Directory_Record dr = new Directory_Record(sw, offset);
                entries.Add(dr);

            }
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }
    public class Volume_Structure_Description : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE structType;
        ELEMENTARY_TYPE stdIdent;//[VSD_STD_ID_LEN];
        //1 standard ECMA 119
        ELEMENTARY_TYPE structVersion;
        ELEMENTARY_TYPE unUsed;
        //This field shall be recorded according to 7.3.3.
        ELEMENTARY_TYPE volume_Space_Size;
        //This field shall be recorded according to 7.2.3
        ELEMENTARY_TYPE volume_Set_Size;
        //This field shall be recorded according to 7.2.3
        ELEMENTARY_TYPE volume_Sequence_Number;
        //This field shall be recorded according to 7.2.3
        ELEMENTARY_TYPE logical_block_Size;
        //This field shall be recorded according to 7.3.3
        ELEMENTARY_TYPE path_Table_Size;
        ELEMENTARY_TYPE location_of_Occurrence_L;
        ELEMENTARY_TYPE location_of_Optional_Occurrence;
        //This field shall be recorded according to 7.3.2.  = Both-byte orders
        ELEMENTARY_TYPE location_of_Occurrence_M;
        ELEMENTARY_TYPE location_of_Optional_Occurrence_M;

        //This field shall be recorded according to 9.1.
        Directory_Record dr;
        //texte
        ELEMENTARY_TYPE volume_Set_Identifier;
        ELEMENTARY_TYPE publisher_Identifier;
        ELEMENTARY_TYPE data_Preparer_Identifier;
        ELEMENTARY_TYPE application_Identifier;
       ELEMENTARY_TYPE copyright_File_Identifier;
       ELEMENTARY_TYPE abstract_File_Identifier;
        ELEMENTARY_TYPE bibliographic_File_Identifier;
        ELEMENTARY_TYPE boot_System;
        ELEMENTARY_TYPE volume_Creation_Date;
        ELEMENTARY_TYPE volume_Modification_Date;
        ELEMENTARY_TYPE volume_Expiration_Date;
        ELEMENTARY_TYPE boot_System_Identifier;
        ELEMENTARY_TYPE structData;//[2041];
        public ELEMENTARY_TYPE StructType
        {
            get { return structType; }
            set { structType = value; }
        }
        public ELEMENTARY_TYPE StdIdent
        {
            get { return stdIdent; }
            set { stdIdent = value; }
        }
        public ELEMENTARY_TYPE StructVersion
        {
            get { return structVersion; }
            set { structVersion = value; }
        }
        public ELEMENTARY_TYPE Volume_Space_Size
        {
            get { return volume_Space_Size; }
            set { volume_Space_Size = value; }
        }

        public ELEMENTARY_TYPE Volume_Set_Size
        {
            get { return volume_Set_Size; }
            set { volume_Set_Size = value; }
        }
        public ELEMENTARY_TYPE Volume_Sequence_Number
        {
            get { return volume_Sequence_Number; }
            set { volume_Sequence_Number = value; }
        }
        public ELEMENTARY_TYPE Logical_block_Size
        {
            get { return logical_block_Size; }
            set { logical_block_Size = value; }
        }
        public ELEMENTARY_TYPE Path_Table_Size
        {
            get { return path_Table_Size; }
            set { path_Table_Size = value; }
        }
        // This field shall be recorded according to 7.3.1.
        public ELEMENTARY_TYPE Location_of_Occurrence_L
        {
            get { return location_of_Occurrence_L; }
            set { location_of_Occurrence_L = value; }
        }
        public ELEMENTARY_TYPE Location_of_Optional_Occurrence
        {
            get { return location_of_Optional_Occurrence; }
            set { location_of_Optional_Occurrence = value; }
        }
        public ELEMENTARY_TYPE Location_of_Occurrence_M
        {
            get { return location_of_Occurrence_M; }
            set { location_of_Occurrence_M = value; }
        }
        public ELEMENTARY_TYPE Location_of_Optional_Occurrence_M
        {
            get { return location_of_Optional_Occurrence_M; }
            set { location_of_Optional_Occurrence_M = value; }
        }
        public Directory_Record Directory_Record
        {
            get { return dr; }
            set { dr = value; }
        }
        public ELEMENTARY_TYPE Volume_Set_Identifier
        {
            get { return volume_Set_Identifier; }
            set { volume_Set_Identifier = value; }
        }
        public ELEMENTARY_TYPE Publisher_Identifier
        {
            get { return publisher_Identifier; }
            set { publisher_Identifier = value; }
        }
        public ELEMENTARY_TYPE Data_Preparer_Identifier
        {
            get { return data_Preparer_Identifier; }
            set { data_Preparer_Identifier = value; }
        }
        public ELEMENTARY_TYPE Application_Identifier
        {
            get { return application_Identifier; }
            set { application_Identifier = value; }
        }
        public ELEMENTARY_TYPE Copyright_File_Identifier
        {
            get { return copyright_File_Identifier; }
            set { copyright_File_Identifier = value; }
        }
        public ELEMENTARY_TYPE Abstract_File_Identifier
        {
            get { return abstract_File_Identifier; }
            set { abstract_File_Identifier = value; }
        }
        public ELEMENTARY_TYPE Bibliographic_File_Identifier
        {
            get { return bibliographic_File_Identifier; }
            set { bibliographic_File_Identifier = value; }
        }//Dates format ~textuel
        public ELEMENTARY_TYPE Volume_Creation_Date
        {
            get { return volume_Creation_Date; }
            set { volume_Creation_Date = value; }
        }
        public ELEMENTARY_TYPE Volume_Modification_Date
        {
            get { return volume_Modification_Date; }
            set { volume_Modification_Date = value; }
        }
        public ELEMENTARY_TYPE Volume_Expiration_Date
        {
            get { return volume_Expiration_Date; }
            set { volume_Expiration_Date = value; }
        }
        public ELEMENTARY_TYPE Boot_System_Identifier
        {
            get { return boot_System_Identifier; }
            set { boot_System_Identifier = value; }
        }
        public ELEMENTARY_TYPE Boot_System
        {
            get { return boot_System; }
            set { boot_System = value; }
        }
        public string Creation_Date
        {
            get { return datestring((byte[])volume_Creation_Date.Value);}
        }
        ELEMENTARY_TYPE Modification_Date;
        ELEMENTARY_TYPE Expiration_Date;
        public string datestring(byte[] bind)
        {
            return "";
        }
        public Volume_Structure_Description(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            structType = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            stdIdent = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, Constantes.VSD_STD_ID_CD001.Length);
            structVersion = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            unUsed = new ELEMENTARY_TYPE(sw, offset, typeof(byte));
            boot_System_Identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 32);
            boot_System = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 32);
            sw.Position = 80;
            sw.Position += 4;
            volume_Space_Size = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            //89 to 120  Escape Sequences
            sw.Position = 120;
            sw.Position += 2;
            volume_Set_Size = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            sw.Position += 2;
            volume_Sequence_Number = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            sw.Position += 2;
            logical_block_Size = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            sw.Position += 4;
            path_Table_Size = new ELEMENTARY_TYPE(sw, offset, typeof(int));
            sw.Position += 2;
            location_of_Occurrence_L = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            sw.Position += 2;
            location_of_Optional_Occurrence = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            sw.Position += 2;
            location_of_Occurrence_M = new ELEMENTARY_TYPE(sw, offset, typeof(short));
            sw.Position = 156;//0x9C
            dr = new Directory_Record(sw, offset);
            sw.Position = 190;//0xBE
            volume_Set_Identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 128);
            publisher_Identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 128);
            data_Preparer_Identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 128);
            application_Identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 128);
            copyright_File_Identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 36);
            abstract_File_Identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 36);
            bibliographic_File_Identifier = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 36);
            sw.Position = 813;
            volume_Creation_Date = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 16);
            volume_Modification_Date = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 16);
            volume_Expiration_Date = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 16);
            structData = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2041 - (int)sw.Position);

            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
    }


    /* Beginning Extended Area Descriptor (ECMA 167r3 2/9.2) */
    public class beginningExtendedAreaDesc
    {
        byte structType;
        byte[] stdIdent;//[VSD_STD_ID_LEN];
        byte structVersion;
        byte[] structData;//[2041];
    } ;

    /* Terminating Extended Area Descriptor (ECMA 167r3 2/9.3) */
    public class terminatingExtendedAreaDesc
    {
        byte structType;
        byte[] stdIdent;//[VSD_STD_ID_LEN];
        byte structVersion;
        byte[] structData;//[2041];
    } ;

    /* Boot Descriptor (ECMA 167r3 2/9.4) */
    public class bootDesc
    {
        byte structType;
        byte[] stdIdent;//[VSD_STD_ID_LEN];
        byte structVersion;
        byte reserved1;
        regid archType;
        regid bootIdent;
        uint bootExtLocation;
        uint bootExtLength;
        ulong loadAddress;
        ulong startAddress;
        timestamp descCreationDateAndTime;
        ushort flags;
        byte[] reserved2;//[32];
        byte[] bootUse;//[1906];
    } 

    public class extent_ad
    {
        uint extLength;
        uint extLocation;
    }  

    public class kernel_extent_ad
    {
        uint extLength;
        uint extLocation;
    } ;

    /* Descriptor Tag (ECMA 167r3 3/7.2) */
    public class tag
    {
        ushort tagIdent;
        ushort descVersion;
        byte tagChecksum;
        byte reserved;
        ushort tagSerialNum;
        ushort descCRC;
        ushort descCRCLength;
        uint tagLocation;
        public tag(BitStreamReader sw)
        {
            tagIdent = (ushort) sw.ReadShort();
            descVersion = (ushort)sw.ReadShort();
        }
    };




    /* NSR Descriptor (ECMA 167r3 3/9.1) */
    public class NSRDesc
    {
        byte structType;
        byte[] stdIdent;//[VSD_STD_ID_LEN];
        byte structVersion;
        byte reserved;
        byte[] structData;//[2040];
    } ;

    /* Primary Volume Descriptor (ECMA 167r3 3/10.1) */
    public class primaryVolDesc
    {
        tag descTag;
        uint volDescSeqNum;
        uint primaryVolDescNum;
        byte[] volIdent;//[32];
        ushort volSeqNum;
        ushort maxVolSeqNum;
        ushort interchangeLvl;
        ushort maxInterchangeLvl;
        uint charSetList;
        uint maxCharSetList;
        byte[] volSetIdent;//[128];
        charspec descCharSet;
        charspec explanatoryCharSet;
        extent_ad volAbstract;
        extent_ad volCopyright;
        regid appIdent;
        timestamp recordingDateAndTime;
        regid impIdent;
        byte impUse;//[64];
        uint predecessorVolDescSeqLocation;
        ushort flags;
        byte reserved;//[22];
    } ;

    /* Anchor Volume Descriptor Pointer (ECMA 167r3 3/10.2) */
    public class anchorVolDescPtr
    {
        tag descTag;
        extent_ad mainVolDescSeqExt;
        extent_ad reserveVolDescSeqExt;
        byte reserved;//[480];
    } ;

    /* Volume Descriptor Pointer (ECMA 167r3 3/10.3) */
    public class volDescPtr
    {
        tag descTag;
        uint volDescSeqNum;
        extent_ad nextVolDescSeqExt;
        byte reserved;//[484];
    } ;

    /* Implementation Use Volume Descriptor (ECMA 167r3 3/10.4) */
    public class impUseVolDesc
    {
        tag descTag;
        uint volDescSeqNum;
        regid impIdent;
        byte impUse;//[460];
    } ;

    /* Partition Descriptor (ECMA 167r3 3/10.5) */
    public class partitionDesc
    {
        tag descTag;
        uint volDescSeqNum;
        ushort partitionFlags;
        ushort partitionNumber;
        regid partitionContents;
        byte partitionContentsUse;//[128];
        uint accessType;
        uint partitionStartingLocation;
        uint partitionLength;
        regid impIdent;
        byte impUse;//[128];
        byte reserved;//[156];
    } ;


    /* Logical Volume Descriptor (ECMA 167r3 3/10.6) */
    public class logicalVolDesc
    {
        tag descTag;
        uint volDescSeqNum;
        charspec descCharSet;
        byte logicalVolIdent;//[128];
        uint logicalBlockSize;
        regid domainIdent;
        byte logicalVolContentsUse;//[16];
        uint mapTableLength;
        uint numPartitionMaps;
        regid impIdent;
        byte impUse;//[128];
        extent_ad integritySeqExt;
        byte partitionMaps;//[0];
    } ;

    /* Generic Partition Map (ECMA 167r3 3/10.7.1) */
    public class genericPartitionMap
    {
        byte partitionMapType;
        byte partitionMapLength;
        byte partitionMapping;//[0];
    } ;



    /* Type 1 Partition Map (ECMA 167r3 3/10.7.2) */
    public class genericPartitionMap1
    {
        byte partitionMapType;
        byte partitionMapLength;
        ushort volSeqNum;
        ushort partitionNum;
    } ;

    /* Type 2 Partition Map (ECMA 167r3 3/10.7.3) */
    public class genericPartitionMap2
    {
        byte partitionMapType;
        byte partitionMapLength;
        byte partitionIdent;//[62];
    } ;

    /* Unallocated Space Descriptor (ECMA 167r3 3/10.8) */
    public class unallocSpaceDesc
    {
        tag descTag;
        uint volDescSeqNum;
        uint numAllocDescs;
        extent_ad allocDescs;//[0];
    } ;

    /* Terminating Descriptor (ECMA 167r3 3/10.9) */
    public class terminatingDesc
    {
        tag descTag;
        byte reserved;//[496];
    } ;

    /* Logical Volume Integrity Descriptor (ECMA 167r3 3/10.10) */
    public class logicalVolIntegrityDesc
    {
        tag descTag;
        timestamp recordingDateAndTime;
        uint integrityType;
        extent_ad nextIntegrityExt;
        byte logicalVolContentsUse;//[32];
        uint numOfPartitions;
        uint lengthOfImpUse;
        uint freeSpaceTable;//[0];
        uint sizeTable;//[0];
        byte impUse;//[0];
    } ;
 

    /* Recorded Address (ECMA 167r3 4/7.1) */
    public class lb_addr
    {
        uint logicalBlockNum;
        ushort partitionReferenceNum;
    } ;

    /* ... and its in-core analog */
    public class kernel_lb_addr
    {
        uint logicalBlockNum;
        short partitionReferenceNum;
    } ;

    /* Short Allocation Descriptor (ECMA 167r3 4/14.14.1) */
    public class short_ad
    {
        uint extLength;
        uint extPosition;
    }

    /* Long Allocation Descriptor (ECMA 167r3 4/14.14.2) */
    public class long_ad
    {
        uint extLength;
        lb_addr extLocation;
        byte impUse;//[6];
    }  ;

    public class kernel_long_ad
    {
        uint extLength;
        kernel_lb_addr extLocation;
        byte impUse;//[6];
    } ;

    /* Extended Allocation Descriptor (ECMA 167r3 4/14.14.3) */
    public class ext_ad
    {
        uint extLength;
        uint recordedLength;
        uint informationLength;
        lb_addr extLocation;
    }  ;

    public class kernel_ext_ad
    {
        uint extLength;
        uint recordedLength;
        uint informationLength;
        kernel_lb_addr extLocation;
    } ;

    /* Descriptor Tag (ECMA 167r3 4/7.2 - See 3/7.2) */


    /* File Set Descriptor (ECMA 167r3 4/14.1) */
    public class fileSetDesc
    {
        tag descTag;
        timestamp recordingDateAndTime;
        ushort interchangeLvl;
        ushort maxInterchangeLvl;
        uint charSetList;
        uint maxCharSetList;
        uint fileSetNum;
        uint fileSetDescNum;
        charspec logicalVolIdentCharSet;
        byte logicalVolIdent;//[128];
        charspec fileSetCharSet;
        byte fileSetIdent;//[32];
        byte copyrightFileIdent;//[32];
        byte abstractFileIdent;//[32];
        long_ad rootDirectoryICB;
        regid domainIdent;
        long_ad nextExt;
        long_ad streamDirectoryICB;
        byte reserved;//[32];
    } ;

    /* Partition Header Descriptor (ECMA 167r3 4/14.3) */
    public class partitionHeaderDesc
    {
        short_ad unallocSpaceTable;
        short_ad unallocSpaceBitmap;
        short_ad partitionIntegrityTable;
        short_ad freedSpaceTable;
        short_ad freedSpaceBitmap;
        byte reserved;//[88];
    } ;

    /* File Identifier Descriptor (ECMA 167r3 4/14.4) */
    public class fileIdentDesc
    {
        tag descTag;
        ushort fileVersionNum;
        byte fileCharacteristics;
        byte lengthFileIdent;
        long_ad icb;
        ushort lengthOfImpUse;
        byte impUse;//[0];
        byte fileIdent;//[0];
        byte padding;//[0];
    } ;


    /* Allocation Ext Descriptor (ECMA 167r3 4/14.5) */
    public class allocExtDesc
    {
        tag descTag;
        uint previousAllocExtLocation;
        uint lengthAllocDescs;
    } ;

    /* ICB Tag (ECMA 167r3 4/14.6) */
    public class icbtag
    {
        uint priorRecordedNumDirectEntries;
        ushort strategyType;
        ushort strategyParameter;
        ushort numEntries;
        byte reserved;
        byte fileType;
        lb_addr parentICBLocation;
        ushort flags;
    }  ;
      public class indirectEntry
    {
        tag descTag;
        icbtag icbTag;
        long_ad indirectICB;
    } ;

    /* Terminal Entry (ECMA 167r3 4/14.8) */
    public class terminalEntry
    {
        tag descTag;
        icbtag icbTag;
    } ;

    /* File Entry (ECMA 167r3 4/14.9) */
    public class fileEntry
    {
        tag descTag;
        icbtag icbTag;
        uint uid;
        uint gid;
        uint permissions;
        ushort fileLinkCount;
        byte recordFormat;
        byte recordDisplayAttr;
        uint recordLength;
        ulong informationLength;
        ulong logicalBlocksRecorded;
        timestamp accessTime;
        timestamp modificationTime;
        timestamp attrTime;
        uint checkpoint;
        long_ad extendedAttrICB;
        regid impIdent;
        ulong uniqueID;
        uint lengthExtendedAttr;
        uint lengthAllocDescs;
        byte extendedAttr;//[0];
        byte allocDescs;//[0];
    } ;

 
     /* Extended Attribute Header Descriptor (ECMA 167r3 4/14.10.1) */
    public class extendedAttrHeaderDesc
    {
        tag descTag;
        uint impAttrLocation;
        uint appAttrLocation;
    } ;

    /* Generic Format (ECMA 167r3 4/14.10.2) */
    public class genericFormat
    {
        uint attrType;
        byte attrSubtype;
        byte reserved;//[3];
        uint attrLength;
        byte attrData;//[0];
    } ;

    /* Character Set Information (ECMA 167r3 4/14.10.3) */
    public class charSetInfo
    {
        uint attrType;
        byte attrSubtype;
        byte reserved;//[3];
        uint attrLength;
        uint escapeSeqLength;
        byte charSetType;
        byte escapeSeq;//[0];
    } ;

    /* Alternate Permissions (ECMA 167r3 4/14.10.4) */
    public class altPerms
    {
        uint attrType;
        byte attrSubtype;
        byte reserved;//[3];
        uint attrLength;
        ushort ownerIdent;
        ushort groupIdent;
        ushort permission;
    } ;

    /* File Times Extended Attribute (ECMA 167r3 4/14.10.5) */
    public class fileTimesExtAttr
    {
        uint attrType;
        byte attrSubtype;
        byte reserved;//[3];
        uint attrLength;
        uint dataLength;
        uint fileTimeExistence;
        byte fileTimes;
    } ;
    /* Information Times Extended Attribute (ECMA 167r3 4/14.10.6) */
    public class infoTimesExtAttr
    {
        uint attrType;
        byte attrSubtype;
        byte reserved;//[3];
        uint attrLength;
        uint dataLength;
        uint infoTimeExistence;
        byte infoTimes;//[0];
    } ;
    /* Device Specification (ECMA 167r3 4/14.10.7) */
    public class deviceSpec
    {
        uint attrType;
        byte attrSubtype;
        byte reserved;//[3];
        uint attrLength;
        uint impUseLength;
        uint majorDeviceIdent;
        uint minorDeviceIdent;
        byte impUse;//[0];
    } ;

    /* Implementation Use Extended Attr (ECMA 167r3 4/14.10.8) */
    public class impUseExtAttr
    {
        uint attrType;
        byte attrSubtype;
        byte reserved;//[3];
        uint attrLength;
        uint impUseLength;
        regid impIdent;
        byte impUse;//[0];
    }

    /* Application Use Extended Attribute (ECMA 167r3 4/14.10.9) */
    public class appUseExtAttr
    {
        uint attrType;
        byte attrSubtype;
        byte reserved;//[3];
        uint attrLength;
        uint appUseLength;
        regid appIdent;
        byte appUse;//[0];
    } ;

    /* Unallocated Space Entry (ECMA 167r3 4/14.11) */
    public class unallocSpaceEntry
    {
        tag descTag;
        icbtag icbTag;
        uint lengthAllocDescs;
        byte allocDescs;//[0];
    } ;

    /* Space Bitmap Descriptor (ECMA 167r3 4/14.12) */
    public class spaceBitmapDesc
    {
        tag descTag;
        uint numOfBits;
        uint numOfBytes;
        byte bitmap;//[0];
    } ;

    /* Partition Integrity Entry (ECMA 167r3 4/14.13) */
    public class partitionIntegrityEntry
    {
        tag descTag;
        icbtag icbTag;
        timestamp recordingDateAndTime;
        byte integrityType;
        byte reserved;//[175];
        regid impIdent;
        byte impUse;//[256];
    } ;

    /* Short Allocation Descriptor (ECMA 167r3 4/14.14.1) */

    /* Long Allocation Descriptor (ECMA 167r3 4/14.14.2) */

    /* Extended Allocation Descriptor (ECMA 167r3 4/14.14.3) */

    /* Logical Volume Header Descriptor (ECMA 167r3 4/14.15) */
    public class logicalVolHeaderDesc
    {
        ulong uniqueID;
        byte reserved;//[24];
    } ;
    /* Path Component (ECMA 167r3 4/14.16.1) */
    public class pathComponent
    {
        byte componentType;
        byte lengthComponentIdent;
        ushort componentFileVersionNum;
        byte componentIdent;//[0];
    } ;

    /* File Entry (ECMA 167r3 4/14.17) */
    public class extendedFileEntry
    {
        tag descTag;
        icbtag icbTag;
        uint uid;
        uint gid;
        uint permissions;
        ushort fileLinkCount;
        byte recordFormat;
        byte recordDisplayAttr;
        uint recordLength;
        ulong informationLength;
        ulong objectSize;
        ulong logicalBlocksRecorded;
        timestamp accessTime;
        timestamp modificationTime;
        timestamp createTime;
        timestamp attrTime;
        uint checkpoint;
        uint reserved;
        long_ad extendedAttrICB;
        long_ad streamDirectoryICB;
        regid impIdent;
        ulong uniqueID;
        uint lengthExtendedAttr;
        uint lengthAllocDescs;
        byte extendedAttr;//[0];
        byte allocDescs;//[0];
    } 
}
