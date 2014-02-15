using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LowLevel
{
    #region Enumerations
    public enum MediaType
    {
        FLoppy1 = 0xF0,	//3.5" Double Sided, 80 tracks per side, 18 or 36 sectors per Track (1.44MB or 2.88MB) Used also for other media types such as tapes.[31],
        Hard_disk = 0xF8,	//Fixed disk (i.e., typically a partition on a Hard disk).[32],
        FLoppy2 = 0xF9,	//3.,5" Double sided, 80 tracks per side, 9 sectors per Track (720K) 5.25" Double sided, 80 tracks per side, 15 sectors per Track (1.2MB),
        FLoppy3 = 0xFA,	//5.25" Single sided, 80 tracks per side, 8 sectors per Track (320K),
        FLoppy4 = 0xFB,	//3.5" Double sided, 80 tracks per side, 8 sectors per Track (640K),
        FLoppy5 = 0xFC,	//5.25" Single sided, 40 tracks per side, 9 sectors per Track (180K),
        FLoppy6 = 0xFD,	//5.25" Double sided, 40 tracks per side, 9 sectors per Track (360K) 8" Double sided, 77 tracks per side, 26 sectors per Track, 128 bytes per Sector (500.5K),
        FLoppy7 = 0xFE,	//5.25" Single sided, 40 tracks per side, 8 sectors per Track (160K) 8" Single sided, 77 tracks per side, 26 sectors per Track, 128 bytes per Sector (250.25K) 8" Double sided, 77 tracks per side, 8 sectors per Track, 1024 bytes per Sector (1232K),
        FLoppy8 = 0xFF,	//5.25" Double sided, 40 tracks per side, 8 sectors per Track (320K),
    }
    public enum PartitionType
    {
        Vide = 0x00, FAT12 = 0x01, XENIX_root = 0x02, XENIX_usr = 0x03, FAT16_32Mio = 0x04, Étendue = 0x05, FAT16 = 0x06, NTFS = 0x07,
        AIX = 0x08, AIX_bootable = 0x09, OS2_Boot_Manager = 0x0a, Win95_OSR2_FAT32_adressage_CHS = 0x0b, Win95_OSR2_FAT32_adressage_LBA = 0x0c,
        Win95_FAT16_adressage_LBA = 0x0e, Étendue_adressage_LBA = 0x0f, OPUS = 0x10, Hidden_FAT12 = 0x11, Compaq_diagnostic = 0x12,
        Hidden_FAT16_32M = 0x14, Hidden_FAT16 = 0x16, Hidden_NTFS = 0x17, AST_SmartSleep = 0x18, Hidden_Win95_FAT32 = 0x1b,
        Hidden_Win95_FAT32_LBA = 0x1c, Hidden_Win95_FAT16_LBA = 0x1e, NEC_DOS = 0x24, Smart_File_System = 0x2f,
        AROS_RDB = 0x30, Plan_9 = 0x39, PartitionMagic_Recoverable_Partition_PqRP = 0x3c, Venix4_80286 = 0x40,
        PPC_PReP_Boot = 0x41, SFS = 0x42, QNX4x = 0x4d, QNX4x_2nde_partition = 0x4e, QNX4x_3e_partition = 0x4f, OnTrack_DM = 0x50,
        OnTrack_DM6_Aux = 0x51, CPM = 0x52, OnTrack_DM6_Aux2 = 0x53, OnTrackDM6 = 0x54, EZ_Drive = 0x55, Golden_Bow = 0x56, Priam_Edisk = 0x5c,
        SpeedStor = 0x61, GNU_HURD_or_Sys = 0x63, Novell_Netware = 0x64, Novell_Netware2 = 0x65, DiskSecure_Mult = 0x70, PC_IX = 0x75,
        Ancien_Minix = 0x80, Minix_ancien_Linux = 0x81, Swap_Linux_pool_ZFS = 0x82, ext2_ext3_ext4_ReiserFS_et_JFS = 0x83,
        OS2_hidden_C = 0x84, Linux_étendu = 0x85, NTFS_volume_set = 0x86, NTFS_volume_set2 = 0x87, Linux_LVM = 0x8e, Amoeba = 0x93,
        Amoeba_BBT = 0x94, SDF = 0x9d, BSD_OS = 0x9f, IBM_Thinkpad_hi = 0xa0, FreeBSD = 0xa5, OpenBSD = 0xa6, NeXTSTEP = 0xa7,
        Darwin_UFS = 0xa8, NetBSD = 0xa9, Darwin_boot = 0xab, HFS = 0xaf, BSDI_fs = 0xb7, BSDI_swap = 0xb8, Boot_Wizard_hid_Acronis_Hidden = 0xbb,
        Acronis_Secure_Zone = 0xbc, Solaris_boot = 0xbe, Fichier_CrashDumpsys_CTOS_III_ = 0xc0, DRDOS_sec_FAT = 0xc1, DRDOS_sec_FAT2 = 0xc4,
        DRDOS_sec_FAT3 = 0xc6, Syrinx = 0xc7, Système_de_fichiers_CTOS_III_PC = 0xcd, Non_FS_data = 0xda, CP_M_ = 0xdb,
        Dell_Utility = 0xde, BootIt = 0xdf, DOS_access = 0xe1, DOS_lecture_seule = 0xe3, SpeedStor2 = 0xe4, BeOS_fs = 0xeb,
        EFI_GPT1 = 0xee, EFI_FAT_12_16 = 0xef, Linux_PA_RISC_b = 0xf0, SpeedStor3 = 0xf1, SpeedStor4 = 0xf4, DOS_secondaire = 0xf2,
        MVTFS = 0xf7, Linux_raid_auto = 0xfd, LANstep = 0xfe, BBT = 0xff
    }
    public enum ATTR_DEF_FLAGS { Indexable = 0001, Regenerate = 0x0040, Non_resident = 0x0080 }
    public enum FileFlags { FILE_RECORD_SEGMENT_IN_USE = 0x0001, FILEFILE_NAME_INDEX_PRESENT = 0x0002 }
    public enum ATTRIBUTE_TYPE_CODE
    {
        STANDARD_INFORMATION = 0x10,//File attributes (such as read-only and archive), time stamps (such as file creation and last modified), and the hard link count.
        ATTRIBUTE_LIST = 0x20,//A list of attributes that make up the file and the file reference of the MFT file record in which each attribute is located.
        FILE_NAME = 0x30,//The name of the file, in Unicode characters.
        OBJECT_ID = 0x40,//An 16-byte object identifier assigned by the link-tracking service.
        SECURITY_DESCRIPTOR = 0x50,
        VOLUME_NAME = 0x60,//The volume label. Present in the ,Volume file.
        VOLUME_INFORMATION = 0x70,//The volume information. Present in the ,Volume file.
        DATA = 0x80,//The contents of the file.
        INDEX_ROOT = 0x90,//Used to implement filename allocation for large directories.
        INDEX_ALLOCATION = 0xA0,//Used to implement filename allocation for large directories.
        BITMAP = 0xB0,//A bitmap index for a large directory.
        REPARSE_POINT = 0xC0,// The reparse point data.
        LOGGED_UTILITY_STREAM = 0x100
    }
    public enum FORM_CODE { RESIDENT_FORM = 0x00, NONRESIDENT_FORM = 0x01 }
    public enum ATTRIBUTE_FLAGS { Compressed = 0x0001, Encrypted = 0x4000, Sparse = 0x8000 }
    public enum VOLUME_INFORMATION_FLAGS
    {
        Dirty = 0x0001, Resize_LogFile = 0x0002, Upgrade_on_Mount = 0x0004,
        Mounted_on_NT4 = 0x0008, Delete_USN_underway = 0x0010, Repair_Object_Ids = 0x0020, Modified_by_chkdsk = 0x8000
    }
    public enum STANDARD_INFORMATION_FLAGS
    {
        ReadOnly = 0x0001, Hidden = 0x0002, System = 0x0004, Archive = 0x0020, Device = 0x0040, Normal = 0x0080, Temporary = 0x0100,
        Sparse_File = 0x0200, Reparse_Point = 0x0400, Compressed = 0x0800, Offline = 0x1000, Not_Content_Indexed = 0x2000, Encrypted = 0x4000
    }
    public enum INDEX_FLAGS
    {
        Small_Index = 0x00,// (fits in indexHeader Root) 
        Large_index = 0x01// (indexHeader Allocation needed 
    }
    public enum INDEX_ENTRY_FLAGS { INDEX_ENTRY_NODE = 0x0001, INDEX_ENTRY_END = 0x0002, INDEX_ENTRY_POINTER_FORM = 0x8000, NTFS_INVALID_VCN = -1 }
    public enum COLLATION_RULE
    {
        BINARY = 00, FILE_NAME = 01, UNICODE_STRING = 02, ULONG = 0x10,
        SID = 0x11, SecurityHash = 0x12, ULONGGS = 0x13
    }
    public enum SYSTEM_FILE_NUMBER
    {
        MASTER_FILE_TABLE_NUMBER = 0,  //  $Master_File_Table
        MASTER_FILE_TABLE2_NUMBER = 1,   //  $MftMirr
        LOG_FILE_NUMBER = 2,   //  $LogFile
        VOLUME_DASD_NUMBER = 3,   //  $Volume
        ATTRIBUTE_DEF_TABLE_NUMBER = 4,   //  $AttrDef
        ROOT_FILE_NAME_INDEX_NUMBER = 5,   //  .
        BIT_MAP_FILE_NUMBER = 6,   //  $BitMap
        BOOT_FILE_NUMBER = 7,   //  $Boot_Sector
        BAD_CLUSTER_FILE_NUMBER = 8,   //  $BadClus
        QUOTA_TABLE_NUMBER = 9,   //  $Quota
        UPCASE_TABLE_NUMBER = 10,  //  $UpCase
        CAIRO_NUMBER = 11,//  $Cairo
        FIRST_USER_FILE_NUMBER = 16
    }
    public enum FILENAME_NAMESPACE { POSIX = 0, Win32 = 1, DOS = 2, Win32_DOS = 3 }
    public enum INDEX_ALLOCATION_FLAGS { Index_entry_points_to_a_subnode = 0x01, Last_index_entry_in_the_node = 0x02 }
    #endregion
}
