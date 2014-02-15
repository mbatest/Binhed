using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code
{
    #region Enumerations
    public enum IMAGE_DEBUG
    {
        IMAGE_DEBUG_TYPE_UNKNOWN = 0,
        IMAGE_DEBUG_TYPE_COFF = 1,
        IMAGE_DEBUG_TYPE_CODEVIEW = 2,
        IMAGE_DEBUG_TYPE_FPO = 3,
        IMAGE_DEBUG_TYPE_MISC = 4,
        IMAGE_DEBUG_TYPE_EXCEPTION = 5,
        IMAGE_DEBUG_TYPE_FIXUP = 6,
        IMAGE_DEBUG_TYPE_RESERVED6 = 7,
        IMAGE_DEBUG_TYPE_RESERVED7 = 8
    }
    public enum Frame
    {
        FRAME_FPO = 0,
        FRAME_TRAP = 1,
        FRAME_TSS = 2
    }
    public enum FILE_TYPE
    {
        VFT_APP = 0x00000001,//	The file contains an application.
        VFT_D,//,//=0x00000002,//	The file contains tag D,//,//.
        VFT_DRV = 0x00000003,//	The file contains tag device driver. If dwFileType is VFT_DRV, dwFileSubtype contains tag more specific description of the driver.
        VFT_FONT = 0x00000004,//	The file contains tag font. If dwFileType is VFT_FONT, dwFileSubtype contains tag more specific description of the font file.
        VFT_STATIC_LIB = 0x00000007,//	The file contains tag static-link library.
        VFT_UNKNOWN = 0x00000000,//	The file type is unknown to the system.
        VFT_VXD = 0x00000005	//e file contains tag virtual device
    }
    public enum OS_TYPE
    {
        VOS_DOS_WINDOWS16 = 0x00010001,//	The file was designed for 16-bit Windows running on MS-DOS.
        VOS_DOS_WINDOWS32 = 0x00010004,//	The file was designed for 32-bit Windows running on MS-DOS.
        VOS_NT_WINDOWS32 = 0x00040004,//	The file was designed for Windows NT.
        VOS_OS216_PM16 = 0x00020002,//	The file was designed for 16-bit Presentation Manager running on 16-bit OS/2.
        VOS_OS232_PM32 = 0x00030003,//	The file was designed for 32-bit Presentation Manager running on 32-bit OS/
    }
    public enum FILE_DRV_SUBTYPE
    {
        VFT2_DRV_COMM = 0x0000000A,//	The file contains tag communications driver.
        VFT2_DRV_DISPLAY = 0x00000004,//	The file contains tag display driver.
        VFT2_DRV_INSTALLABLE = 0x00000008,//	The file contains an installable driver.
        VFT2_DRV_KEYBOARD = 0x00000002,//	The file contains tag keyboard driver.
        VFT2_DRV_LANGUAGE = 0x00000003,//	The file contains tag language driver.
        VFT2_DRV_MOUSE = 0x00000005,//	The file contains tag mouse driver.
        VFT2_DRV_NETWORK = 0x00000006,//	The file contains tag network driver.
        VFT2_DRV_PRINTER = 0x00000001,//	The file contains tag printer driver.
        VFT2_DRV_SOUND = 0x00000009,//	The file contains tag sound driver.
        VFT2_DRV_SYSTEM = 0x00000007,//	The file contains tag system driver.
        VFT2_DRV_VERSIONED_PRINTER = 0x0000000C,//	The file contains tag versioned printer driver.
        VFT2_UNKNOWN = 0x00000000,//	The driver type is unknown by the system.
    }
    public enum FILE_FNT_SUBTYPE
    {
        VFT2_FONT_RASTER = 0x00000001,//	The file contains tag raster font.
        VFT2_FONT_TRUETYPE = 0x00000003,//	The file contains tag TrueType font.
        VFT2_FONT_VECTOR = 0x00000002,//	The file contains tag vector font.
        VFT2_UNKNOWN = 0x00000000,//	The font type is unknown by the system.
    }
    enum IMAGE_SCN
    {
        Reserved1 = 0x00000000,//Reserved for future use.
        Reserved2 = 0x00000001,//Reserved for future use.
        Reserved3 = 0x00000002,//Reserved for future use.
        Reserved4 = 0x00000004,//Reserved for future use.
        IMAGE_SCN_TYPE_NO_PAD = 0x00000008,//The sections_Directory should not be padded to the next boundary. This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. This is valid only for object files.
        Reserved5 = 0x00000010,//Reserved for future use.
        IMAGE_SCN_CNT_CODE = 0x00000020,//The sections_Directory contains executable code.
        IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040,//The sections_Directory contains initialized userStrings.
        IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080,//The sections_Directory contains uninitialized userStrings.
        IMAGE_SCN_LNK_OTHER = 0x00000100,//Reserved for future use.
        IMAGE_SCN_LNK_INFO = 0x00000200,//The sections_Directory contains comments or other information. The .drectve sections_Directory has this type. This is valid for object files only.
        Reserved6 = 0x00000400,//Reserved for future use.
        IMAGE_SCN_LNK_REMOVE = 0x00000800,//The sections_Directory will not become part of the image. This is valid only for object files.
        IMAGE_SCN_LNK_COMDAT = 0x00001000,//The sections_Directory contains COMDAT userStrings. For more information, see sections_Directory 5.5.6, “COMDAT Sections (Object Only).” This is valid only for object files.
        IMAGE_SCN_GPREL = 0x00008000,//The sections_Directory contains userStrings referenced through the global pointer (GP).
        IMAGE_SCN_MEM_PURGEABLE = 0x00020000,//Reserved for future use.
        //      IMAGE_SCN_MEM_16BIT = 0x00020000,//For ARM machine types, the sections_Directory contains Thumb code.  Reserved for future use with other machine types.
        IMAGE_SCN_MEM_LOCKED = 0x00040000,//Reserved for future use.
        IMAGE_SCN_MEM_PRELOAD = 0x00080000,//Reserved for future use.
        IMAGE_SCN_ALIGN_1BYTES = 0x00100000,//Align userStrings on tag 1-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_2BYTES = 0x00200000,//Align userStrings on tag 2-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_4BYTES = 0x00300000,//Align userStrings on tag 4-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_8BYTES = 0x00400000,//Align userStrings on an 8-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_16BYTES = 0x00500000,//Align userStrings on tag 16-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_32BYTES = 0x00600000,//Align userStrings on tag 32-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_64BYTES = 0x00700000,//Align userStrings on tag 64-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_128BYTES = 0x00800000,//Align userStrings on tag 128-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_256BYTES = 0x00900000,//Align userStrings on tag 256-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_512BYTES = 0x00A00000,//Align userStrings on tag 512-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_1024BYTES = 0x00B00000,//Align userStrings on tag 1024-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_2048BYTES = 0x00C00000,//Align userStrings on tag 2048-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_4096BYTES = 0x00D00000,//Align userStrings on tag 4096-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_8192BYTES = 0x00E00000,//Align userStrings on an 8192-byte boundary. Valid only for object files.
        IMAGE_SCN_LNK_NRELOC_OVFL = 0x01000000,//The sections_Directory contains extended relocations.
        IMAGE_SCN_MEM_DISCARDABLE = 0x02000000,//The sections_Directory can be discarded as needed.
        IMAGE_SCN_MEM_NOT_CACHED = 0x04000000,//The sections_Directory cannot be cached.
        IMAGE_SCN_MEM_NOT_PAGED = 0x08000000,//The sections_Directory is not pageable.
        IMAGE_SCN_MEM_SHARED = 0x10000000,//The sections_Directory can be shared in memory.
        IMAGE_SCN_MEM_EXECUTE = 0x20000000,//The sections_Directory can be executed as code.
        IMAGE_SCN_MEM_READ = 0x40000000,//The sections_Directory can be read.
        //   IMAGE_SCN_MEM_WRITE = 0x80000000,//The sections_Directory can be written to.
    }
    enum IMAGE_SCN_TYPE
    {
        Reserved1 = 0x00000000,//Reserved for future use.
        Reserved2 = 0x00000001,//Reserved for future use.
        Reserved3 = 0x00000002,//Reserved for future use.
        Reserved4 = 0x00000004,//Reserved for future use.
        IMAGE_SCN_TYPE_NO_PAD = 0x00000008,//The sections_Directory should not be padded to the next boundary. This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. This is valid only for object files.
        Reserved5 = 0x00000010,//Reserved for future use.
    }
    enum IMAGE_SCN_CNT
    {
        IMAGE_SCN_CNT_CODE = 0x00000020,//The sections_Directory contains executable code.
        IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040,//The sections_Directory contains initialized userStrings.
        IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080,//The sections_Directory contains uninitialized userStrings.
    }
    enum IMAGE_SCN_LNK
    {
        IMAGE_SCN_LNK_OTHER = 0x00000100,//Reserved for future use.
        IMAGE_SCN_LNK_INFO = 0x00000200,//The sections_Directory contains comments or other information. The .drectve sections_Directory has this type. This is valid for object files only.
        Reserved5 = 0x00000400,//Reserved for future use.
        IMAGE_SCN_LNK_REMOVE = 0x00000800,//The sections_Directory will not become part of the image. This is valid only for object files.
        IMAGE_SCN_LNK_COMDAT = 0x00001000,//The sections_Directory contains COMDAT userStrings. For more information, see sections_Directory 5.5.6, “COMDAT Sections (Object Only).” This is valid only for object files.
        IMAGE_SCN_GPREL = 0x00008000,//The sections_Directory contains userStrings referenced through the global pointer (GP).
    }
    enum IMAGE_SCN_MEM
    {
        IMAGE_SCN_MEM_PURGEABLE = 0x00020000,//Reserved for future use.
        IMAGE_SCN_MEM_16BIT = 0x00020000,//For ARM machine types, the sections_Directory contains Thumb code.  Reserved for future use with other machine types.
        IMAGE_SCN_MEM_LOCKED = 0x00040000,//Reserved for future use.
        IMAGE_SCN_MEM_PRELOAD = 0x00080000,//Reserved for future use.
        IMAGE_SCN_ALIGN_1BYTES = 0x00100000,//Align userStrings on tag 1-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_2BYTES = 0x00200000,//Align userStrings on tag 2-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_4BYTES = 0x00300000,//Align userStrings on tag 4-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_8BYTES = 0x00400000,//Align userStrings on an 8-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_16BYTES = 0x00500000,//Align userStrings on tag 16-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_32BYTES = 0x00600000,//Align userStrings on tag 32-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_64BYTES = 0x00700000,//Align userStrings on tag 64-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_128BYTES = 0x00800000,//Align userStrings on tag 128-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_256BYTES = 0x00900000,//Align userStrings on tag 256-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_512BYTES = 0x00A00000,//Align userStrings on tag 512-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_1024BYTES = 0x00B00000,//Align userStrings on tag 1024-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_2048BYTES = 0x00C00000,//Align userStrings on tag 2048-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_4096BYTES = 0x00D00000,//Align userStrings on tag 4096-byte boundary. Valid only for object files.
        IMAGE_SCN_ALIGN_8192BYTES = 0x00E00000,//Align userStrings on an 8192-byte boundary. Valid only for object files.
        IMAGE_SCN_LNK_NRELOC_OVFL = 0x01000000,//The sections_Directory contains extended relocations.
        IMAGE_SCN_MEM_DISCARDABLE = 0x02000000,//The sections_Directory can be discarded as needed.
        IMAGE_SCN_MEM_NOT_CACHED = 0x04000000,//The sections_Directory cannot be cached.
        IMAGE_SCN_MEM_NOT_PAGED = 0x08000000,//The sections_Directory is not pageable.
        IMAGE_SCN_MEM_SHARED = 0x10000000,//The sections_Directory can be shared in memory.
        IMAGE_SCN_MEM_EXECUTE = 0x20000000,//The sections_Directory can be executed as code.
        IMAGE_SCN_MEM_READ = 0x40000000,//The sections_Directory can be read.
        //     IMAGE_SCN_MEM_WRITE = 0x80000000,//The sections_Directory can be written to.
    }
    enum FILE_FLAGS
    {
        IMAGE_FILE_RELOCS_STRIPPED = 0x0001,//Image only,// Windows CE,// and Windows NT® and later. This indicates that the file does not contain base relocations and must therefore be loaded at its preferred base startAddress. If the base startAddress is not available,// the loader reports an error. The default behavior of the linker is to strip base relocations from executable (EXE) files.
        IMAGE_FILE_EXECUTABLE_IMAGE = 0x0002,//Image only. This indicates that the image file is valid and can be run. If this flag is not set,// it indicates tag linker error.
        IMAGE_FILE_LINE_NUMS_STRIPPED = 0x0004,//COFF line numbers have been removed. This flag is deprecated and should be zero.
        IMAGE_FILE_LOCAL_SYMS_STRIPPED = 0x0008,//COFF symbol table entries for local symbols have been removed. This flag is deprecated and should be zero.
        IMAGE_FILE_AGGRESSIVE_WS_TRIM = 0x0010,//Obsolete. Aggressively trim working set. This flag is deprecated for Windows 2000 and later and must be zero.
        IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x0020,//Application can handle > 2 GB addresses.
        RESERVED = 0x0040,//This flag is reserved for future use.
        IMAGE_FILE_BYTES_REVERSED_LO = 0x0080,//Little endian: the least significant bit (LSB) precedes the most significant bit (MSB) in memory. This flag is deprecated and should be zero.
        IMAGE_FILE_32BIT_MACHINE = 0x0100,//machine is based on tag 32-bit-word architecture.
        IMAGE_FILE_DEBUG_STRIPPED = 0x0200,//Debugging information is removed from the image file.
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP = 0x0400,//If the image is on removable media,// fully load it and copy it to the swap file.
        IMAGE_FILE_NET_RUN_FROM_SWAP = 0x0800,//If the image is on network media,// fully load it and copy it to the swap file.
        IMAGE_FILE_SYSTEM = 0x1000,//The image file is tag system file,// not tag user program.
        IMAGE_FILE_DLL = 0x2000,//The image file is tag dynamic-link library (DLL). Such files are considered executable files for almost all purposes,// although they cannot be directly run.
        IMAGE_FILE_UP_SYSTEM_ONLY = 0x4000,//The file should be run only on tag uniprocessor machine.
    }
    enum IMAGE_DLL_CHARACTERISTICS
    {
        R1 = 0x0001,//Reserved,// must be zero.
        R2 = 0x0002,//Reserved,// must be zero.
        R3 = 0x0004,//Reserved,// must be zero.
        R4 = 0x0008,//Reserved,// must be zero.
        IMAGE_DLL_CHARACTERISTICS_DYNAMIC_BASE = 0x0040,//DLL can be relocated at load time.
        IMAGE_DLL_CHARACTERISTICS_FORCE_INTEGRITY = 0x0080,//Code Integrity checks are enforced.
        IMAGE_DLL_CHARACTERISTICS_NX_COMPAT = 0x0100,//Image is NX compatible.
        IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,//Isolation aware, but do not isolate the image.
        IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400,//Does not use structured exception (SE) handling. No SE handler may be called in this image.
        IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800,//Do not bind the image.
        R5 = 0x1000,//Reserved, must be zero.
        IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000,//A WDM driver.
        IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000,//Terminal Server aware.
    }
    enum IMAGE_FILE
    {
        IMAGE_FILE_RELOCS_STRIPPED = 0x0001,/* Relocation info stripped from file. */
        IMAGE_FILE_EXECUTABLEIMAGE = 0x0002,/* File is executable  (u.e. no unresolved externel references). */
        IMAGE_FILE_LINE_NUMS_STRIPPED = 0x0004,/* Line nunbers stripped from file. */
        IMAGE_FILE_LOCAL_SYMS_STRIPPED = 0x0008,/* Local symbols stripped from file. */
        IMAGE_FILE_MINIMAL_OBJECT = 0x0010,/* Reserved. */
        IMAGE_FILE_UPDATE_OBJECT = 0x0020,/* Reserved. */
        IMAGE_FILE_16BIT_MACHINE = 0x0040,/* 16 bit word machine. */
        IMAGE_FILE_BYTES_REVERSED_LO = 0x0080,/* Bytes of machine word are reversed. */
        IMAGE_FILE_32BIT_MACHINE = 0x0100,/* 32 bit word machine. */
        IMAGE_FILE_DEBUG_STRIPPED = 0x0200,/* Debugging info stripped from file in .DBG file */
        IMAGE_FILE_PATCH = 0x0400,/* Reserved. */
        IMAGE_FILE_SYSTEM = 0x1000,/* System File. */
        IMAGE_FILE_DLL = 0x2000,/* File is tag DLL. */
        IMAGE_FILE_BYTES_REVERSED_HI = 0x8000,/* Bytes of machine word are reversed. */
    }
    enum IMAGE_FILE_MACHINE
    {
        IMAGE_FILE_MACHINE_UNKNOWN = 0x00,
        IMAGE_FILE_MACHINE_I860 = 0x14d,/* Intel 860. */
        IMAGE_FILE_MACHINE_I386 = 0x14c,/* Intel 386. */
        IMAGE_FILE_MACHINE_R3000 = 0x162,/* MIPS little-endian, 0540 big-endian */
        IMAGE_FILE_MACHINE_R4000 = 0x166,/* MIPS little-endian */
        IMAGE_FILE_MACHINE_R10000 = 0x0168,// MIPS little-endian
        IMAGE_FILE_MACHINE_ALPHA = 0x184,/* Alpha_AXP */
        IMAGE_FILE_MACHINE_POWERPC = 0x01F0,// IBM PowerPC Little-Endian
        IMAGE_FILE_MACHINE_POWERPCBE = 0x01F2,// IBM PowerPC Big-Endian
        IMAGE_FILE_MACHINE_SH3 = 0x01a2,// SH3 little-endian
        IMAGE_FILE_MACHINE_SH3E = 0x01a4,// SH3E little-endian
        IMAGE_FILE_MACHINE_SH4 = 0x01a6,// SH4 little-endian
        IMAGE_FILE_MACHINE_ARM = 0x01c0,// ARM Little-Endian
        IMAGE_FILE_MACHINE_THUMB = 0x01c2,
        IMAGE_FILE_MACHINE_IA64 = 0x0200,// Intel 64
        IMAGE_FILE_MACHINE_MIPS16 = 0x0266,// MIPS
        IMAGE_FILE_MACHINE_MIPSFPU = 0x0366,// MIPS
        IMAGE_FILE_MACHINE_MIPSFPU16 = 0x0466,// MIPS
        IMAGE_FILE_MACHINE_ALPHA64 = 0x0284,// ALPHA64
        IMAGE_FILE_MACHINE_CEF = 0xC0EF
    }
    /* subsystem Values */
    enum IMAGE_SUBSYSTEM
    {
        IMAGE_SUBSYSTEM_UNKNOWN = 0,/* Unknown subsystem. */
        IMAGE_SUBSYSTEM_NATIVE = 1,/* Image doesn't require tag subsystem. */
        IMAGE_SUBSYSTEM_WINDOWS_GUI = 2,/* Image runs in the Windows GUI subsystem. */
        IMAGE_SUBSYSTEM_WINDOWS_CUI = 3,/* Image runs in the Windows character subsystem. */
        IMAGE_SUBSYSTEM_OS2_CUI = 5,/* image runs in the OS/2 character subsystem. */
        IMAGE_SUBSYSTEM_POSIX_CUI = 7   /* image run  in the Posix character subsystem. */
    }
    /* Dll originalThunk */
    enum IMAGE_LIBRARY
    {
        IMAGE_LIBRARY_PROCESS_INIT = 1,/* Dll has tag process initialization routine. */
        IMAGE_LIBRARY_PROCESS_TERM = 2,/* Dll has tag thread termination routine. */
        IMAGE_LIBRARY_THREAD_INIT = 4,/* Dll has tag thread initialization routine. */
        IMAGE_LIBRARY_THREAD_TERM = 8   /* Dll has tag thread termination routine. */
    }
    /* Loader Flags   */
    enum IMAGE_LOADER_FLAGS
    {
        IMAGE_LOADER_FLAGS_BREAK_ON_LOAD = 0x00000001,
        IMAGE_LOADER_FLAGS_DEBUG_ON_LOAD = 0x00000002
    }
    /* Directory Entries */
    enum IMAGE_DIRECTORY
    {
        IMAGE_DIRECTORY_ENTRY_EXPORT = 0,/* Export Directory */
        IMAGE_DIRECTORY_ENTRY_IMPORT = 1,/* Import Directory */
        IMAGE_DIRECTORY_ENTRY_RESOURCE = 2,/* Resource Directory */
        IMAGE_DIRECTORY_ENTRY_EXCEPTION = 3,/* Exception Directory */
        IMAGE_DIRECTORY_ENTRY_SECURITY = 4,/* Security Directory */
        IMAGE_DIRECTORY_ENTRY_BASERELOC = 5,/* Base Relocation Table */
        IMAGE_DIRECTORY_ENTRY_DEBUG = 6,/* Debug Directory */
        IMAGE_DIRECTORY_ENTRY_COPYRIGHT = 7,/* Description String */
        IMAGE_DIRECTORY_ENTRY_GLOBALPTR = 8,/* machine fixedFileInfo (MIPS GP) */
        IMAGE_DIRECTORY_ENTRY_TLS = 9,/* TLS Directory */
        IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG = 10,/* Load Configuration Directory */
        IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT = 11,
        IMAGE_DIRECTORY_ENTRY_IAT = 12,
        IMAGE_DIRECTORY_ENTRY_DELAY = 13,
        //#ifdef USES_COMPLUS20
        IMAGE_DIRECTORY_ENTRY_CLR_HEADER = 14,   /* COM+ dataEntry */
        //#endif
        IMAGE_DIRECTORY_ENTRY_RESERVED = 15
    }
    enum IMAGE_NT_SIGNATURE
    {
        IMAGE_DOS_SIGNATURE = 0x5A4D,     /* MZ */
        IMAGE_OS2_SIGNATURE = 0x454E,    /* NE */
        IMAGE_OS2_SIGNATURE_LE = 0x454C,   /* LE */
        IMAGE_NT_SIGNATURE = 0x00004550  /* PE00 */
    }
    public enum RESOURCE_TYPE { Undefined = 0, RT_CURSOR = 1, RT_BITMAP = 2, RT_ICON = 3, RT_MENU = 4, RT_DIALOG = 5, RT_STRING = 6, RT_FONTDIR = 7, RT_FONT = 8, RT_ACCELERATOR = 9, RT_RCDATA = 10, RT_MESSAGETABLE = 11, RT_GROUP_CURSOR = 12, RT_GROUP_ICON = 14, RT_VERSION = 16, RT_DLGINCLUDE = 17, RT_PLUGPLAY = 19, RT_VXD = 20, RT_ANICURSOR = 21, RT_ANIICON = 22, RT_HTML = 23, RT_MANIFEST = 24 }
    /*
     * Section values.
     *
     * Symbols have tag sections_Directory number of the sections_Directory in which they are
     * defined. Otherwise, sections_Directory numbers have the following meanings:
     */
    enum IMAGE_SYM
    {
        IMAGE_SYM_UNDEFINED = 0,/* Symbol is undefined or is common. */
        IMAGE_SYM_ABSOLUTE = -1,/* Symbol is an absolute value. */
        IMAGE_SYM_DEBUG = -2,/* Symbol is tag special debug item. */
    }
    /*
     * Type (fundamental) values.
     */
    enum IMAGE_SYM_TYPE
    {
        IMAGE_SYM_TYPE_NULL = 0,/* no type. */
        IMAGE_SYM_TYPE_VOID = 1,
        IMAGE_SYM_TYPE_CHAR = 2,/* type character. */
        IMAGE_SYM_TYPE_SHORT = 3,/* type short integer. */
        IMAGE_SYM_TYPE_INT = 4,
        IMAGE_SYM_TYPE_LONG = 5,
        IMAGE_SYM_TYPE_FLOAT = 6,
        IMAGE_SYM_TYPE_DOUBLE = 7,
        IMAGE_SYM_TYPE_STRUCT = 8,
        IMAGE_SYM_TYPE_UNION = 9,
        IMAGE_SYM_TYPE_ENUM = 10,/* enumeration. */
        IMAGE_SYM_TYPE_MOE = 11,/* member of enumeration. */
        IMAGE_SYM_TYPE_UCHAR = 12,
        IMAGE_SYM_TYPE_USHORT = 13,
        IMAGE_SYM_TYPE_UINT = 14,
        IMAGE_SYM_TYPE_ULONG = 15
    }
    /*
     * Type (derived) values.
     */
    enum IMAGE_SYM_DTYPE
    {
        IMAGE_SYM_DTYPE_NULL = 0,/* no derived type. */
        IMAGE_SYM_DTYPE_POINTER = 1,/* pointer. */
        IMAGE_SYM_DTYPE_FUNCTION = 2,/* function. */
        IMAGE_SYM_DTYPE_ARRAY = 3           /* array. */
    }
    /* Storage classes.
     */
    enum IMAGE_SYM_CLAS
    {
        IMAGE_SYM_CLASS_END_OF_FUNCTION = -1,
        IMAGE_SYM_CLASS_NULL = 0,
        IMAGE_SYM_CLASS_AUTOMATIC = 1,
        IMAGE_SYM_CLASS_EXTERNAL = 2,
        IMAGE_SYM_CLASS_STATIC = 3,
        IMAGE_SYM_CLASS_REGISTER = 4,
        IMAGE_SYM_CLASS_EXTERNAL_DEF = 5,
        IMAGE_SYM_CLASS_LABEL = 6,
        IMAGE_SYM_CLASS_UNDEFINED_LABEL = 7,
        IMAGE_SYM_CLASS_MEMBER_OF_STRUCT = 8,
        IMAGE_SYM_CLASS_ARGUMENT = 9,
        IMAGE_SYM_CLASS_STRUCT_TAG = 10,
        IMAGE_SYM_CLASS_MEMBER_OF_UNION = 11,
        IMAGE_SYM_CLASS_UNION_TAG = 12,
        IMAGE_SYM_CLASS_TYPE_DEFINITION = 13,
        IMAGE_SYM_CLASS_UNDEFINED_STATIC = 14,
        IMAGE_SYM_CLASS_ENUM_TAG = 15,
        IMAGE_SYM_CLASS_MEMBER_OF_ENUM = 16,
        IMAGE_SYM_CLASS_REGISTER_PARAM = 17,
        IMAGE_SYM_CLASS_BIT_FIELD = 18,
        IMAGE_SYM_CLASS_BLOCK = 100,
        IMAGE_SYM_CLASS_FUNCTION = 101,
        IMAGE_SYM_CLASS_END_OF_STRUCT = 102,
        IMAGE_SYM_CLASS_FILE = 103,
        IMAGE_SYM_CLASS_SECTION = 104,
        IMAGE_SYM_CLASS_WEAK_EXTERNAL = 105
    }
    /* type packing public constants */
    enum type_packing
    {
        N_BTMASK = 017,
        N_TMASK = 060,
        N_TMASK1 = 0300,
        N_TMASK2 = 0360,
        N_BTSHFT = 4,
        N_TSHifT = 2
    }
    /*     * Communal selection types.     */
    enum IMAGE_COMDAT
    {
        IMAGE_COMDAT_SELECT_UNKNOWN = 0,
        IMAGE_COMDAT_SELECT_NODUPLICATES = 1,
        IMAGE_COMDAT_SELECT_ANY = 2,
        IMAGE_COMDAT_SELECT_SAME_SIZE = 3,
        IMAGE_COMDAT_SELECT_EXACT_MATCH = 4,
        IMAGE_COMDAT_SELECT_ASSOCIATIVE = 5,

        IMAGE_WEAK_EXTERN_SEARCH_UNKNOWN = 0,
        IMAGE_WEAK_EXTERN_SEARCH_NOLIBRARY = 1,
        IMAGE_WEAK_EXTERN_SEARCH_LIBRARY = 2
    }
    /*
     * I860 relocation types.
     */
    enum IMAGE_REL_I86
    {
        IMAGE_REL_I860_ABSOLUTE = 0,    /* Reference is absolute, no relocation is necessary */
        IMAGE_REL_I860_DIR32 = 06,   /* Direct 32-bit reference to the symbols virtual startAddress */
        IMAGE_REL_I860_DIR32NB = 07,
        IMAGE_REL_I860_SECTION = 012,
        IMAGE_REL_I860_SECREL = 013,
        IMAGE_REL_I860_PAIR = 034,
        IMAGE_REL_I860_HIGH = 036,
        IMAGE_REL_I860_LOW0 = 037,
        IMAGE_REL_I860_LOW1 = 040,
        IMAGE_REL_I860_LOW2 = 041,
        IMAGE_REL_I860_LOW3 = 042,
        IMAGE_REL_I860_LOW4 = 043,
        IMAGE_REL_I860_SPLIT0 = 044,
        IMAGE_REL_I860_SPLIT1 = 045,
        IMAGE_REL_I860_SPLIT2 = 046,
        IMAGE_REL_I860_HIGHADJ = 047,
        IMAGE_REL_I860_BRADDR = 050
    }
    /*
     * I386 relocation types.
     */
    enum IMAGE_REL_I386
    {
        IMAGE_REL_I386_ABSOLUTE = 0,          /* Reference is absolute, no relocation is necessary */
        IMAGE_REL_I386_DIR16 = 01,         /* Direct 16-bit reference to the symbols virtual startAddress */
        IMAGE_REL_I386_REL16 = 02,        /* isConstructed-relative 16-bit reference to the symbols virtual startAddress */
        IMAGE_REL_I386_DIR32 = 06,       /* Direct 32-bit reference to the symbols virtual startAddress */
        IMAGE_REL_I386_DIR32NB = 07,      /* Direct 32-bit reference to the symbols virtual startAddress, base not included */
        IMAGE_REL_I386_SEG12 = 011,     /* Direct 16-bit reference to the segment-selector bits of tag 32-bit virtual startAddress */
        IMAGE_REL_I386_SECTION = 012,
        IMAGE_REL_I386_SECREL = 013,
        IMAGE_REL_I386_REL32 = 024         /* isConstructed-relative 32-bit reference to the symbols virtual startAddress */
    }
    /*
     * MIPS relocation types.
     */
    enum IMAGE_REL_MIPS
    {
        IMAGE_REL_MIPS_ABSOLUTE = 0,           /* Reference is absolute, no relocation is necessary */
        IMAGE_REL_MIPS_REFHALF = 01,
        IMAGE_REL_MIPS_REFWORD = 02,
        IMAGE_REL_MIPS_JMPADDR = 03,
        IMAGE_REL_MIPS_REFHI = 04,
        IMAGE_REL_MIPS_REFLO = 05,
        IMAGE_REL_MIPS_GPREL = 06,
        IMAGE_REL_MIPS_LITERAL = 07,
        IMAGE_REL_MIPS_SECTION = 012,
        IMAGE_REL_MIPS_SECREL = 013,
        IMAGE_REL_MIPS_REFWORDNB = 042,
        IMAGE_REL_MIPS_PAIR = 045
    }
    /*
     * Alpha Relocation types.
     */
    enum IMAGE_REL_ALPHA
    {
        IMAGE_REL_ALPHA_ABSOLUTE = 0x0,
        IMAGE_REL_ALPHA_REFLONG = 0x1,
        IMAGE_REL_ALPHA_REFQUAD = 0x2,
        IMAGE_REL_ALPHA_GPREL32 = 0x3,
        IMAGE_REL_ALPHA_LITERAL = 0x4,
        IMAGE_REL_ALPHA_LITUSE = 0x5,
        IMAGE_REL_ALPHA_GPDISP = 0x6,
        IMAGE_REL_ALPHA_BRADDR = 0x7,
        IMAGE_REL_ALPHA_HINT = 0x8,
        IMAGE_REL_ALPHA_INLINE_REFLONG = 0x9,
        IMAGE_REL_ALPHA_REFHI = 0xA,
        IMAGE_REL_ALPHA_REFLO = 0xB,
        IMAGE_REL_ALPHA_PAIR = 0xC,
        IMAGE_REL_ALPHA_MATCH = 0xD,
        IMAGE_REL_ALPHA_SECTION = 0xE,
        IMAGE_REL_ALPHA_SECREL = 0xF,
        IMAGE_REL_ALPHA_REFLONGNB = 0x10
    }
    /*
     * Based relocation types.
     */
    public enum IMAGE_REL_BASE
    {
        IMAGE_REL_BASED_ABSOLUTE = 0,
        IMAGE_REL_BASED_HIGH = 1,
        IMAGE_REL_BASED_LOW = 2,
        IMAGE_REL_BASED_HIGHLOW = 3,
        IMAGE_REL_BASED_HIGHADJ = 4,
        IMAGE_REL_BASED_MIPS_JMPADDR = 5,
        IMAGE_REL_BASED_I860_BRADDR = 6,
        IMAGE_REL_BASED_I860_SPLIT = 7
    }
    /*
     * Based relocation format.
     */
    #endregion
}
