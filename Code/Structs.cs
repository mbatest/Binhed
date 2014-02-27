using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using Utils;
using System.ComponentModel;

namespace Code
{
    /* http://research.microsoft.com/en-us/um/redmond/projects/invisible/include/loaders/pe_image.h.htm
     http://www.skynet.ie/~caolan/pub/winresdump/winresdump/doc/pefile.html
     http://win32assembly.online.fr/
     Copyright (callOrJump) Microsoft Corporation. All rights reserved. */
    public class IMAGE_BASE_DATA : ILOCALIZED_DATA
    {
        private long position;
        private long length;
        private long offset;
        private long baseOffset;
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
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
        public long OffsetInSection
        {
            get { return offset; }
            set { offset = value; }
        }
        public long BaseOffset
        {
            get { return baseOffset; }
            set { baseOffset = value; }
        }
        public long OffsetFromRVA(long rva)
        {
            return rva - baseOffset;
        }
        public int sectionNumber;
    }
    #region File header format.
    public class IMAGE_DOS_HEADER : IMAGE_BASE_DATA
    {      /* DOS .EXE header */
        [CategoryAttribute("PE File"), DescriptionAttribute("Magic Number")]
        public string Magic
        {
            get
            {
                switch ((IMAGE_NT_SIGNATURE)e_magic)
                {
                    case IMAGE_NT_SIGNATURE.IMAGE_DOS_SIGNATURE: return "MZ : Dos signature";
                    case IMAGE_NT_SIGNATURE.IMAGE_NT_SIGNATURE: return "PE : NT signature";
                    case IMAGE_NT_SIGNATURE.IMAGE_OS2_SIGNATURE: return "NE : OS2";
                    case IMAGE_NT_SIGNATURE.IMAGE_OS2_SIGNATURE_LE: return "LE : OS2";
                    default: return "";
                }
            }
            set { ;}
        }
        public int AddressOfNewHeader
        {
            get { return e_lfanew; }
            set { e_lfanew = value; }
        }
        private short e_magic;                     /* Magic number */
        #region MSDOS dataEntry
        public short e_cblp;                      /* Bytes on last page of file */
        public short e_cp;                        /* Pages in file */
        public short e_crlc;                      /* Relocations */
        public short e_cparhdr;                   /* size of header in paragraphs */
        public short e_minalloc;                  /* Minimum extra paragraphs needed */
        public short e_maxalloc;                  /* Maximum extra paragraphs needed */
        public short e_ss;                        /* Initial (relative) SS value */
        public short e_sp;                        /* Initial SP value */
        public short e_csum;                      /* Checksum */
        public short e_ip;                        /* Initial IP value */
        public short e_cs;                        /* Initial (relative) CS value */
        public short e_lfarlc;                    /* File startAddress of relocation table */
        public short e_ovno;                      /* Overlay number */
        public byte[] e_res;//[2*4];                    /* Reserved words */
        public short e_oemid;                     /* OEM identifier (for e_oeminfo) */
        public short e_oeminfo;                   /* OEM information; e_oemid specific */
        public byte[] e_res2;//[2*10];                  /* Reserved words */
        #endregion
        private int e_lfanew;                    /* File startAddress of new exe header */
        IMAGE_STUB st;
        IMAGE_RICH_SIGNATURE rich;
        public IMAGE_RICH_SIGNATURE RichSignature
        {
            get { return rich; }
            set { rich = value; }
        }
        public IMAGE_STUB StubProgram
        {
            get { return st; }
        }
        public IMAGE_DOS_HEADER(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            e_magic = sw.ReadShort();                   /* Magic number */
            #region MSDOS dataEntry
            e_cblp = sw.ReadShort();                      /* Bytes on last page of file */
            e_cp = sw.ReadShort();                        /* Pages in file */
            e_crlc = sw.ReadShort();                      /* Relocations */
            e_cparhdr = sw.ReadShort();                   /* size of header in paragraphs */
            e_minalloc = sw.ReadShort();                  /* Minimum extra paragraphs needed */
            e_maxalloc = sw.ReadShort();                  /* Maximum extra paragraphs needed */
            e_ss = sw.ReadShort();                        /* Initial (relative) SS value */
            e_sp = sw.ReadShort();                        /* Initial SP value */
            e_csum = sw.ReadShort();                      /* Checksum */
            e_ip = sw.ReadShort();                        /* Initial IP value */
            e_cs = sw.ReadShort();                        /* Initial (relative) CS value */
            e_lfarlc = sw.ReadShort();                    /* File startAddress of relocation table */
            e_ovno = sw.ReadShort();                      /* Overlay number */
            e_res = sw.ReadBytes(8);//[4];                    /* Reserved words */
            e_oemid = sw.ReadShort();                     /* OEM identifier (for e_oeminfo) */
            e_oeminfo = sw.ReadShort();                   /* OEM information; e_oemid specific */
            e_res2 = sw.ReadBytes(20);//[10];                  /* Reserved words */
            #endregion
            // Location 0x3C
            e_lfanew = sw.ReadInteger();
            LengthInFile = sw.Position - PositionOfStructureInFile;
            // 0x40 -> addressNewExeHeader : MSDOS stub
            sw.Position = e_cparhdr * 0x10;
            st = new IMAGE_STUB(sw);
            //Stub program
            sw.Position = 0x80;
            rich = new IMAGE_RICH_SIGNATURE(sw);
        }
        public override string ToString()
        {
            return "Dos Header";
        }
    }
    public class IMAGE_STUB : IMAGE_BASE_DATA
    {
        private string message;
        private List<CodeLine> stubProgram;

        public List<CodeLine> StubProgram
        {
            get { return stubProgram; }
            set { stubProgram = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public IMAGE_STUB(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            stubProgram = Disassemble.ParseStub(sw.ReadBytes(0xe), PositionOfStructureInFile);
            message = sw.ReadStringZ(Encoding.Default);
            LengthInFile = sw.Position - PositionOfStructureInFile;
         }
        public override string ToString()
        {
            return "MS DOS program stub";
        }
    }
    public class IMAGE_RICH_SIGNATURE : IMAGE_BASE_DATA
    {
        private byte[] richSignature;

        public byte[] RichSignature
        {
            get { return richSignature; }
            set { richSignature = value; }
        }
        public IMAGE_RICH_SIGNATURE(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            int nSignDwords = 0;
            int dw;
            for (int i = 0; i < 100; i++)
            {
                dw = sw.ReadInteger();
                // is this the "Rich" terminator?
                if (dw == 0x68636952)
                {
                    nSignDwords = i;
                    break;
                }
            }
            if (nSignDwords > 0)
            {
                sw.Position = 0x80;
                richSignature = sw.ReadBytes((nSignDwords + 2) * 4);
                LengthInFile = sw.Position - PositionOfStructureInFile;
            }

        }
        public override string ToString()
        {
            return "Microsoft Rich signature";
        }
    }
    public class IMAGE_NT_HEADERS : IMAGE_BASE_DATA
    {
        private IMAGE_FILE_HEADER fileHeader;
        private IMAGE_OPTIONAL_HEADER optionalHeader;
        private byte[] signature;
        private string machine;
        public bool Is64bits = false;
        #region Properties
        public string Signature
        {
            get { return Encoding.Default.GetString(signature); }
            set { ;}
        }
        public IMAGE_FILE_HEADER FileHeader
        {
            get { return fileHeader; }
            set { fileHeader = value; }
        }
        public string Processor
        { get { return machine; } }
        public IMAGE_OPTIONAL_HEADER OptionalHeader
        {
            get { return optionalHeader; }
            set { optionalHeader = value; }
        }
        #endregion
        public IMAGE_NT_HEADERS(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            signature = sw.ReadBytes(4);
            fileHeader = new IMAGE_FILE_HEADER(sw);
            optionalHeader = new IMAGE_OPTIONAL_HEADER(sw);
            machine = ProcessorToString(fileHeader.Machine);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        private string ProcessorToString(byte[] buffer)
        {
            if ((int)buffer[0] == 0 && (int)buffer[1] == 0) { return "defined as an ANY / unknown machine type executable"; }
            if ((int)buffer[0] == 76 && (int)buffer[1] == 1) { return "i386 32bit executable"; }
            if ((int)buffer[0] == 100 && (int)buffer[1] == 134) { Is64bits = true; return "x86-64 64bit executable"; }
            if ((int)buffer[0] == 0 && (int)buffer[1] == 2) { return "Intel Itanium executable"; }
            if ((int)buffer[0] == 240 && (int)buffer[1] == 1) { return "Power PC (little endian) executable"; }
            if ((int)buffer[0] == 241 && (int)buffer[1] == 1) { return "Power PC (with floating point support) executable"; }
            if ((int)buffer[0] == 211 && (int)buffer[1] == 1) { return "Matsushita AM33"; }
            if ((int)buffer[0] == 192 && (int)buffer[1] == 1) { return "ARM little endian"; }
            if ((int)buffer[0] == 188 && (int)buffer[1] == 14) { return "EFI byte code"; }
            if ((int)buffer[0] == 65 && (int)buffer[1] == 144) { return "Mitsubishi M32R little endian"; }
            if ((int)buffer[0] == 102 && (int)buffer[1] == 2) { return "MIPS16"; }
            if ((int)buffer[0] == 102 && (int)buffer[1] == 3) { return "MIPS with Floating_Point_Préfix"; }
            if ((int)buffer[0] == 102 && (int)buffer[1] == 4) { return "MIPS 16 with Floating_Point_Préfix"; }
            if ((int)buffer[0] == 102 && (int)buffer[1] == 1) { return "MIPS little endian / R4000"; }
            if ((int)buffer[0] == 162 && (int)buffer[1] == 1) { return "Hitachi SH3"; }
            if ((int)buffer[0] == 163 && (int)buffer[1] == 1) { return "Hitachi SH3 DSP"; }
            if ((int)buffer[0] == 166 && (int)buffer[1] == 1) { return "Hitachi SH4"; }
            if ((int)buffer[0] == 168 && (int)buffer[1] == 1) { return "Hitachi SH5"; }
            if ((int)buffer[0] == 194 && (int)buffer[1] == 1) { return "Thumb"; }
            if ((int)buffer[0] == 105 && (int)buffer[1] == 1) { return "MIPS little-endian WCE v2"; }
            if ((int)buffer[0] == 132 && (int)buffer[1] == 1) { return "Alpha"; }
            if ((int)buffer[0] == 104 && (int)buffer[1] == 2) { return "Motorola 68000"; }
            if ((int)buffer[0] == 144 && (int)buffer[1] == 2) { return "PA-RISC"; }
            return "";
        }
        public override string ToString()
        {
            return "NT headers";
        }

    }
    public class IMAGE_FILE_HEADER : IMAGE_BASE_DATA
    {
        private byte[] machine;//2
        public byte[] Machine
        {
            get { return machine; }
            set { machine = value; }
        }
        public short NumberOfSections;
        private int timeDateStamp;//The low 32 bits of the number of seconds since 00:00 January 1, 1970 
        public string TimeDateStamp
        {
            get
            {
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
                TimeSpan ts = new TimeSpan(0, 0, timeDateStamp);
                dt = dt + ts;
                return dt.ToLongDateString() + " " + dt.ToLongTimeString();
            }
        }
        public int PointerToSymbolTable;//should be zero for an image because COFF debugging information is deprecated
        public int NumberOfSymbols;//should be zero for an image because COFF debugging information is deprecated
        public short SizeOfOptionalHeader;
        private short characteristics;
        public string Characteristics
        {
            get
            {
                string s = "";
                if ((characteristics & 0x001) == 1) s += "Relocs Stripped {0}";
                if ((characteristics & 0x002) == 2) s += "Executable Image ";
                if ((characteristics & 0x004) == 0x004) s += "Line Numbers Stripped ";
                if ((characteristics & 0x008) == 0x008) s += "Local Symbols Stripped ";
                if ((characteristics & 0x010) == 0x010) s += "Trim Local Set ";
                if ((characteristics & 0x020) == 0x020) s += "Can Handle Address Larger than 2Gb ";
                if ((characteristics & 0x080) == 0x080) s += "Bytes Reversed ";
                if ((characteristics & 0x0100) == 0x0100) s += "32 Bit Machine ";
                if ((characteristics & 0x0200) == 0x0200) s += "Debugging Info Stripped ";
                if ((characteristics & 0x0400) == 0x0400) s += "Removable Media Swap ";
                if ((characteristics & 0x0800) == 0x0800) s += "Net Swap ";
                if ((characteristics & 0x1000) == 0x1000) s += "System File ";
                if ((characteristics & 0x2000) == 0x2000) s += "Dll ";
                if ((characteristics & 0x4000) == 0x4000) s += "Uni-Processor Only ";
                if ((characteristics & 0x8000) == 0x8000) s += "High Bytes Reversed";
                return s;
            }
        }
        public IMAGE_FILE_HEADER(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            machine = sw.ReadBytes(2);
            NumberOfSections = sw.ReadShort();
            timeDateStamp = sw.ReadInteger();
            PointerToSymbolTable = sw.ReadInteger();
            NumberOfSymbols = sw.ReadInteger();
            SizeOfOptionalHeader = sw.ReadShort();
            characteristics = sw.ReadShort();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "File header";
        }
    }
    public class IMAGE_OPTIONAL_HEADER : IMAGE_BASE_DATA
    {
        #region Standard fields.
        public string MAGIC
        {
            get
            {
                switch (Magic)
                {
                    case 0x10b: return "PE32";
                    case 0x20b: return "PE32+";
                    default: return "Unknown";
                }
            }
            set { ;}
        }
        public short Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public int SizeOfCode;
        public int SizeOfInitializedData;
        public int SizeOfUninitializedData;
        public int AddressOfEntryPoint;
        public int BaseOfCode;//Relative Real_Address of code (".text" sections_Directory) in loaded image. 
        public int BaseOfData;//Relative Real_Address of uninitialized dataEntry (".bss" sections_Directory) in loaded image. P32 mais pas P32+
        #endregion
        #region NT additional fields.
        public Int64 ImageBase;// 4 ou 8 octets
        public int SectionAlignment;
        public int FileAlignment;
        public short MajorOperatingSystemVersion;
        public short MinorOperatingSystemVersion;
        public short MajorImageVersion;
        public short MinorImageVersion;
        public short MajorSubsystemVersion;
        public short MinorSubsystemVersion;
        public int Win32VersionValue;//  Reserved, must be zero. ???
        public int SizeOfImage;
        public int SizeOfHeaders;
        public int CheckSum;
        private short subsystem;
        public string Subsystem
        {
            get
            {
                switch ((IMAGE_SUBSYSTEM)subsystem)
                {
                    case IMAGE_SUBSYSTEM.IMAGE_SUBSYSTEM_NATIVE: return "Native";
                    case IMAGE_SUBSYSTEM.IMAGE_SUBSYSTEM_OS2_CUI: return "OS2 CUI";
                    case IMAGE_SUBSYSTEM.IMAGE_SUBSYSTEM_POSIX_CUI: return "Posix CUI";
                    case IMAGE_SUBSYSTEM.IMAGE_SUBSYSTEM_UNKNOWN: return "Unknown";
                    case IMAGE_SUBSYSTEM.IMAGE_SUBSYSTEM_WINDOWS_CUI: return "Windows CUI";
                    case IMAGE_SUBSYSTEM.IMAGE_SUBSYSTEM_WINDOWS_GUI: return "Windows GUI";
                    default: return "";
                }
            }
        }
        public short DllCharacteristics;//Flags used to indicate if tag DLL image includes entry points for process and thread initialization and termination
        public Int64 SizeOfStackReserve;//4 ou 8
        public Int64 SizeOfStackCommit;//4 ou 8
        public Int64 SizeOfHeapReserve;//4 ou 8
        public Int64 SizeOfHeapCommit;//4 ou 8
        public int LoaderFlags;//Reserved, must be 
        public int NumberOfRvaAndSizes;// The number of dataEntry-directory entries in the remainder of the optional header. Each describes tag location and size relative virtual adresses
        public IMAGE_DATA_DIRECTORY[] DataDirectory;//[IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16];
        #endregion
        public IMAGE_OPTIONAL_HEADER(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            Magic = sw.ReadShort();
            MajorLinkerVersion = (byte)sw.ReadByte();
            MinorLinkerVersion = (byte)sw.ReadByte();
            SizeOfCode = sw.ReadInteger();
            SizeOfInitializedData = sw.ReadInteger();
            SizeOfUninitializedData = sw.ReadInteger();
            AddressOfEntryPoint = sw.ReadInteger();
            BaseOfCode = sw.ReadInteger();
            switch (Magic)
            {
                case 0x10b:
                    BaseOfData = sw.ReadInteger();
                    ImageBase = sw.ReadInteger();
                    break;
                case 0x20b: ImageBase = sw.ReadLongInt();
                    break;
            }
            SectionAlignment = sw.ReadInteger();
            FileAlignment = sw.ReadInteger();
            MajorOperatingSystemVersion = sw.ReadShort();
            MinorOperatingSystemVersion = sw.ReadShort();
            MajorImageVersion = sw.ReadShort();
            MinorImageVersion = sw.ReadShort();
            MajorSubsystemVersion = sw.ReadShort();
            MinorSubsystemVersion = sw.ReadShort();
            Win32VersionValue = sw.ReadInteger();
            SizeOfImage = sw.ReadInteger();
            SizeOfHeaders = sw.ReadInteger();
            CheckSum = sw.ReadInteger();
            subsystem = sw.ReadShort();
            DllCharacteristics = sw.ReadShort();
            switch (Magic)
            {
                case 0x10b:
                    SizeOfStackReserve = sw.ReadInteger();
                    SizeOfStackCommit = sw.ReadInteger();
                    SizeOfHeapReserve = sw.ReadInteger();
                    SizeOfHeapCommit = sw.ReadInteger();
                    break;
                case 0x20b:
                    SizeOfStackReserve = sw.ReadLongInt();
                    SizeOfStackCommit = sw.ReadLongInt();
                    SizeOfHeapReserve = sw.ReadLongInt();
                    SizeOfHeapCommit = sw.ReadLongInt();
                    break;
            }
            LoaderFlags = sw.ReadInteger();//Obsolete
            NumberOfRvaAndSizes = sw.ReadInteger();
            LengthInFile = sw.Position - PositionOfStructureInFile;
            DataDirectory = new IMAGE_DATA_DIRECTORY[0x10];
            #region Data directories
            for (int i = 0; i < NumberOfRvaAndSizes /* Data.IMAGE_NUMBEROF_DIRECTORY_ENTRIES*/; i++)
            {
                IMAGE_DATA_DIRECTORY imd = new IMAGE_DATA_DIRECTORY(sw, i);
                DataDirectory[i] = imd;
            }
            #endregion

        }
        public override string ToString()
        {
            return "Optional header";
        }
    }
    #endregion
    #region Directory format.
    public class IMAGE_IMPORT_DIRECTORY_IAT : IMAGE_BASE_DATA
    {
        private List<IMAGE_IMPORT_DIRECTORY_IAT_ENTRY> iATEntries;

        public List<IMAGE_IMPORT_DIRECTORY_IAT_ENTRY> IATEntries
        {
            get { return iATEntries; }
            set { iATEntries = value; }
        }
        public long startIAT
        {
            get
            {
                return iATEntries[0].Address;
            }
        }
        public long endIAT
        {
            get
            {
                return iATEntries[iATEntries.Count - 1].Address + iATEntries[iATEntries.Count - 1].Name.Length * 2;
            }
        }
        public IMAGE_IMPORT_DIRECTORY_IAT(BinaryFileReader sw, long Offset, long size)
        {
            PositionOfStructureInFile = sw.Position;
            this.OffsetInSection = Offset;
            iATEntries = new List<IMAGE_IMPORT_DIRECTORY_IAT_ENTRY>();
            long a = 0;
            byte[] data = null;
            if (Executable.Is64bits)
            {
                a = sw.ReadLongInt();
                data = BitConverter.GetBytes(a);
            }
            else
            {
                a = sw.ReadInteger();
                data = BitConverter.GetBytes((int)a);
            }
            while (sw.Position <= Offset + size)
            {
                while (a != 0)
                {
                    iATEntries.Add(new IMAGE_IMPORT_DIRECTORY_IAT_ENTRY(sw.Position - 4, a, data));
                    a = sw.ReadInteger();
                }
                a = sw.ReadInteger();
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Import Adress table";
        }

    }
    public class IMAGE_IMPORT_DIRECTORY_IAT_ENTRY : IMAGE_BASE_DATA
    {
        private long address;
        private byte[] arrayPointer;
        public long intData;

        public long Address
        {
            get { return address; }
            set { address = value; }
        }
        public byte[] ArrayPointer
        {
            get { return arrayPointer; }
            set { arrayPointer = value; }
        }
        public IMAGE_IMPORT_DIRECTORY_IAT_ENTRY(long ad, long da, byte[] dat)
        {
            PositionOfStructureInFile = ad;
            address = ad;
            intData = da;
            arrayPointer = dat;
            LengthInFile = arrayPointer.Length;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class IMAGE_DATA_DIRECTORY : IMAGE_BASE_DATA
    {
        #region Properties
        public string DictionaryName
        {
            get
            {
                return Name;
            }
            set { ;}
        }
        public int Size
        {
            get { return size; }
            set { size = value; }
        }
        public long VirtualAddress
        {
            get { return virtualAddress; }
            set { virtualAddress = value; }
        }
        public IMAGE_IMPORT_DIRECTORY Import
        {
            get { return import; }
            set { import = value; }
        }
        public IMAGE_EXPORT_DIRECTORY Export
        {
            get { return export; }
            set { export = value; }
        }
        public CERTIFICATE_TABLE Certificate
        {
            get { return cert; }
            set { cert = value; }
        }
        #endregion
        private int DictionnaryCode;
        private int size;
        private long virtualAddress;
        private IMAGE_IMPORT_DIRECTORY import;
        private IMAGE_EXPORT_DIRECTORY export;
        private CERTIFICATE_TABLE cert;

        public IMAGE_COR20_HEADER clrHeader;
        public IMAGE_IMPORT_DIRECTORY_IAT IAT;
        public IMAGE_DEBUG_DIRECTORY eb;

        private static string[] array = new string[]{"Export Table","Import Table","Resource Table","Exception Table","Certificate Table","Base Relocation Table","Debug",
"Architecture","Global Ptr","TLS Table","Load Config Table","Bound Import","IAT","Delay Import Descriptor","CLR Runtime Header","Reserved"};

        public IMAGE_DATA_DIRECTORY(BinaryFileReader sw, int i)
        {
            PositionOfStructureInFile = sw.Position;
            virtualAddress = sw.ReadInteger();
            size = sw.ReadInteger();
            DictionnaryCode = i;
            Name = array[DictionnaryCode];
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public void Parse(BinaryFileReader sw)
        {
            long place = sw.Position;
            switch ((IMAGE_DIRECTORY)DictionnaryCode)
            {
                case IMAGE_DIRECTORY.IMAGE_DIRECTORY_ENTRY_EXPORT:
                    #region export
                    sw.Position = OffsetInSection;
                    try
                    {
                        export = new IMAGE_EXPORT_DIRECTORY(sw, BaseOffset);
                    }
                    catch { }
                    sw.Position = place;
                    break;
                    #endregion
                case IMAGE_DIRECTORY.IMAGE_DIRECTORY_ENTRY_IMPORT:
                    #region descriptors
                    sw.Position = OffsetInSection;
                    import = new IMAGE_IMPORT_DIRECTORY(sw, BaseOffset);
                    sw.Position = place;
                    break;
                    #endregion
                case IMAGE_DIRECTORY.IMAGE_DIRECTORY_ENTRY_RESOURCE:
                    break;
                case IMAGE_DIRECTORY.IMAGE_DIRECTORY_ENTRY_EXCEPTION:
                    break;
                case IMAGE_DIRECTORY.IMAGE_DIRECTORY_ENTRY_SECURITY:
                    #region Certificate
                    sw.Position = virtualAddress;
                    cert = new CERTIFICATE_TABLE(sw);
                    sw.Position = place;
                    break;
                    #endregion
                case IMAGE_DIRECTORY.IMAGE_DIRECTORY_ENTRY_BASERELOC:
                    break;
                case IMAGE_DIRECTORY.IMAGE_DIRECTORY_ENTRY_DEBUG:
                    #region Debug
                    sw.Position = OffsetInSection;
                    eb = new IMAGE_DEBUG_DIRECTORY(sw);
                    sw.Position = place;
                    #endregion
                    break;
                case IMAGE_DIRECTORY.IMAGE_DIRECTORY_ENTRY_IAT:
                    #region Import adress
                    sw.Position = OffsetInSection;
                    IAT = new IMAGE_IMPORT_DIRECTORY_IAT(sw, OffsetInSection, size);
                    sw.Position = place;
                    #endregion
                    break;
                case IMAGE_DIRECTORY.IMAGE_DIRECTORY_ENTRY_TLS:
                    break;
                case IMAGE_DIRECTORY.IMAGE_DIRECTORY_ENTRY_CLR_HEADER:
                    #region Clr Header
                    sw.Position = OffsetInSection;
                    clrHeader = new IMAGE_COR20_HEADER(sw, BaseOffset);
                    sw.Position = place;
                    #endregion
                    break;
            }
        }
        public override string ToString()
        {
            return DictionaryName;
        }
    }
    #endregion
    #region Section header format.  0x28 = 40 bytes
    public class IMAGE_SECTION_DIRECTORY : IMAGE_BASE_DATA
    {
        List<IMAGE_SECTION_HEADER> sections = new List<IMAGE_SECTION_HEADER>();

        public List<IMAGE_SECTION_HEADER> Sections
        {
            get { return sections; }
            set { sections = value; }
        }
        public int Count
        {
            get { return sections.Count; }
        }
        public IMAGE_SECTION_DIRECTORY(BinaryFileReader sw, int NumberOfSections)
        {
            PositionOfStructureInFile = sw.Position;
            for (int i = 0; i < NumberOfSections; i++)
            {
                IMAGE_SECTION_HEADER imd = new IMAGE_SECTION_HEADER(sw);
                sections.Add(imd);
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Section Header";
        }
    }
    public class IMAGE_SECTION_HEADER : IMAGE_BASE_DATA
    {
        public int sizeOfRawData;// size of Data
        public int pointerToRawData;// Address of dataEntry
        private short NumberOfLinenumbers;//Shoud be zero
        private uint characteristics;//Flags IMAGE_SCN
        #region Properties
        //   union {
        //     public int PhysicalAddress;
        public int VirtualSize;
        //   } Misc;
        public int VirtualAddress;
        public string SizeOfRawData
        {
            get { return sizeOfRawData.ToString("x8"); }
            set { ;}
        }
        public string PointerToRawData
        {
            get { return pointerToRawData.ToString("x8"); }
            set { ;}
        }
        public List<string> Characteristics
        {
            //Tout faux
            get
            {
                IMAGE_SCN ch = (IMAGE_SCN)characteristics;
                List<string> s = new List<string>();
                if ((ch & IMAGE_SCN.IMAGE_SCN_TYPE_NO_PAD) == IMAGE_SCN.IMAGE_SCN_TYPE_NO_PAD)
                {

                    s.Add("NO_PAD");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_TYPE_NO_PAD) == IMAGE_SCN.IMAGE_SCN_CNT_CODE)
                {
                    s.Add("CNT_CODE");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_CNT_INITIALIZED_DATA) == IMAGE_SCN.IMAGE_SCN_CNT_INITIALIZED_DATA)
                {
                    s.Add("INITIALIZED DATA");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_CNT_UNINITIALIZED_DATA) == IMAGE_SCN.IMAGE_SCN_CNT_UNINITIALIZED_DATA)
                {
                    s.Add("UNINITIALIZED DATA");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_LNK_OTHER) == IMAGE_SCN.IMAGE_SCN_LNK_OTHER)
                {
                    s.Add("LNK_OTHER");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_LNK_INFO) == IMAGE_SCN.IMAGE_SCN_LNK_INFO)
                {
                    s.Add("LNK_INFO");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_LNK_REMOVE) == IMAGE_SCN.IMAGE_SCN_LNK_REMOVE)
                {
                    s.Add("LNK_REMOVE");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_LNK_COMDAT) == IMAGE_SCN.IMAGE_SCN_LNK_COMDAT)
                {
                    s.Add("LNK_COMDAT");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_GPREL) == IMAGE_SCN.IMAGE_SCN_GPREL)
                {
                    s.Add("SCN_GPREL");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_MEM_PURGEABLE) == IMAGE_SCN.IMAGE_SCN_MEM_PURGEABLE)
                {
                    s.Add("MEM_PURGEABLE");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_MEM_LOCKED) == IMAGE_SCN.IMAGE_SCN_MEM_LOCKED)
                {
                    s.Add("MEM LOCKED");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_MEM_LOCKED) == IMAGE_SCN.IMAGE_SCN_MEM_LOCKED)
                {
                    s.Add("MEM PRELOAD");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_1BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_1BYTES)
                {
                    s.Add("ALIGN 1 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_2BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_2BYTES)
                {
                    s.Add("ALIGN 2 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_4BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_4BYTES)
                {
                    s.Add("ALIGN 4 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_8BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_8BYTES)
                {
                    s.Add("ALIGN 8 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_16BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_16BYTES)
                {
                    s.Add("ALIGN 16 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_32BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_32BYTES)
                {
                    s.Add("ALIGN 32 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_64BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_64BYTES)
                {
                    s.Add("ALIGN 64 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_128BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_128BYTES)
                {
                    s.Add("ALIGN 128 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_256BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_256BYTES)
                {
                    s.Add("ALIGN 256 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_512BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_512BYTES)
                {
                    s.Add("ALIGN 512 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_1024BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_1024BYTES)
                {
                    s.Add("ALIGN 1024 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_2048BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_2048BYTES)
                {
                    s.Add("ALIGN 2048 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_4096BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_4096BYTES)
                {
                    s.Add("ALIGN 4096 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_ALIGN_8192BYTES) == IMAGE_SCN.IMAGE_SCN_ALIGN_8192BYTES)
                {
                    s.Add("ALIGN 8192 BYTES");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_LNK_NRELOC_OVFL) == IMAGE_SCN.IMAGE_SCN_LNK_NRELOC_OVFL)
                {
                    s.Add("NRELOC_OVFL");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_MEM_DISCARDABLE) == IMAGE_SCN.IMAGE_SCN_MEM_DISCARDABLE)
                {
                    s.Add("MEM_DISCARDABLE");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_MEM_NOT_CACHED) == IMAGE_SCN.IMAGE_SCN_MEM_NOT_CACHED)
                {
                    s.Add("NOT_CACHED");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_MEM_NOT_PAGED) == IMAGE_SCN.IMAGE_SCN_MEM_NOT_PAGED)
                {
                    s.Add("NOT_PAGED");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_MEM_SHARED) == IMAGE_SCN.IMAGE_SCN_MEM_SHARED)
                {
                    s.Add("SHARED");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_MEM_EXECUTE) == IMAGE_SCN.IMAGE_SCN_MEM_EXECUTE)
                {
                    s.Add("EXECUTE");
                }
                if ((ch & IMAGE_SCN.IMAGE_SCN_MEM_READ) == IMAGE_SCN.IMAGE_SCN_MEM_READ)
                {
                    s.Add("READ");
                }
                return s;
            }
        }
        #endregion
        public int PointerToRelocations;
        public int PointerToLinenumbers;
        public short NumberOfRelocations;
        public List<IMAGE_DATA_DIRECTORY> dataDirs = new List<IMAGE_DATA_DIRECTORY>();
        public IMAGE_SECTION_HEADER(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            byte[] b = sw.ReadBytes(60);
            sw.Position = PositionOfStructureInFile;
            Name = sw.ReadString(8);//Data.IMAGE_SIZEOF_SHORT_NAME
            VirtualSize = sw.ReadInteger();// 4 Not used ?
            VirtualAddress = sw.ReadInteger();//4
            sizeOfRawData = sw.ReadInteger();//4
            pointerToRawData = sw.ReadInteger();//4
            PointerToRelocations = sw.ReadInteger();//4
            PointerToLinenumbers = sw.ReadInteger();//4
            NumberOfRelocations = sw.ReadShort();//2
            NumberOfLinenumbers = sw.ReadShort();//2
            characteristics = (uint)sw.ReadInteger();//4
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name + " " + pointerToRawData.ToString("x8");
        }
    }
    #endregion
    #region Relocation format.
    public class IMAGE_RELOCATION : IMAGE_BASE_DATA
    {
        public int VirtualAddress;
        public int SymbolTableIndex;
        public short Type;
        public IMAGE_REL_BASE relBase
        {
            get
            {
                return (IMAGE_REL_BASE)Type;
            }
        }
        public IMAGE_RELOCATION(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            short u = sw.ReadShort();
            Type = (short)((u & 0xf000) >> 12);
            VirtualAddress = u & 0x0fff;
            LengthInFile = 2;
        }
        public override string ToString()
        {
            return VirtualAddress.ToString("x8") + " " + relBase.ToString();
        }
    }
    public class IMAGE_BASE_RELOCATION : IMAGE_BASE_DATA
    {
        private IMAGE_RELOCATION[] reloc;

        public int VirtualAddress;
        public int SizeOfBlock;
//        public short[] TypeOffset;//[1]; */

        public IMAGE_RELOCATION[] Relocations
        {
            get { return reloc; }
            set { reloc = value; }
        }
        public IMAGE_BASE_RELOCATION(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            VirtualAddress = sw.ReadInteger();
            SizeOfBlock = sw.ReadInteger();
            long numWords = (PositionOfStructureInFile + SizeOfBlock - sw.Position) / 2;
  //          TypeOffset = new short[numWords];
            reloc = new IMAGE_RELOCATION[numWords];
            for (int u = 0; u < numWords; u++)
            {
    //            TypeOffset[u] = sw.ReadShort();
                reloc[u] = new IMAGE_RELOCATION(sw);
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Relocation";
        }
    }
    #endregion
    #region DLL support.
    #region Export Format
    public class EXPORTED_FUNCTION : IMAGE_BASE_DATA
    {
        public int addressOfFunction;
        public int ordinal;
        public int addressOfName;
        public EXPORTED_FUNCTION(int a, int o, int aName, string n)
        {
            PositionOfStructureInFile = a;
            addressOfFunction = a;
            ordinal = o;
            Name = n;
            addressOfName = aName;
            LengthInFile = 12;
        }
        public override string ToString()
        {
            return ordinal.ToString() + " " + addressOfFunction.ToString("x8") + " " + Name;
        }
    }
    public class IMAGE_EXPORT_DIRECTORY : IMAGE_BASE_DATA
    {
        private int characteristics;

        private int timeDateStamp;
        private short majorVersion;
        private short minorVersion;
        private int addressOfname;
        private string appName;


        public int Base;
        public long baseOffset;
        public int NumberOfFunctions;
        public int NumberOfNames;
        public int AddressOfFunctions;// Pointers to tables
        public int AddressOfNames;
        public long AddressOfNameOrdinals;
        private List<int> addressesOfFunctions = new List<int>();
        private List<int> addressesOfNames = new List<int>();
        private List<int> addressesOfOrdinals = new List<int>();
        private List<string> symbols = new List<string>();
        private List<EXPORTED_FUNCTION> exports = new List<EXPORTED_FUNCTION>();

        public int Characteristics
        {
            get { return characteristics; }
            set { characteristics = value; }
        }
        public int TimeDateStamp
        {
            get { return timeDateStamp; }
            set { timeDateStamp = value; }
        }
        public short MajorVersion
        {
            get { return majorVersion; }
            set { majorVersion = value; }
        }
        public short MinorVersion
        {
            get { return minorVersion; }
            set { minorVersion = value; }
        }
        public int AddressOfName
        {
            get { return addressOfname; }
            set { addressOfname = value; }
        }
        public string ApplicationName
        {
            get { return appName; }
            set { appName = value; }
        }
        public List<int> AddressesOfFunctions
        {
            get { return addressesOfFunctions; }
            set { addressesOfFunctions = value; }
        }
        public List<int> AddressesOfNames
        {
            get { return addressesOfNames; }
            set { addressesOfNames = value; }
        }
        public List<int> AddressesOfOrdinals
        {
            get { return addressesOfOrdinals; }
            set { addressesOfOrdinals = value; }
        }
        public List<string> Symbols
        {
            get { return symbols; }
            set { symbols = value; }
        }
        public List<EXPORTED_FUNCTION> Exports
        {
            get { return exports; }
            set { exports = value; }
        }
        public IMAGE_EXPORT_DIRECTORY(BinaryFileReader sw, long baseOffset)
        {
            // 11 members * 4 = 44 bytes
            PositionOfStructureInFile = sw.Position;
            this.baseOffset = baseOffset;
            characteristics = sw.ReadInteger();
            timeDateStamp = sw.ReadInteger();
            majorVersion = sw.ReadShort();
            minorVersion = sw.ReadShort();
            addressOfname = sw.ReadInteger();
            Base = sw.ReadInteger();
            NumberOfFunctions = sw.ReadInteger();
            NumberOfNames = sw.ReadInteger();
            AddressOfFunctions = sw.ReadInteger();
            AddressOfNames = sw.ReadInteger();
            AddressOfNameOrdinals = sw.ReadInteger();
            sw.Position = Offset(AddressOfFunctions);
            for (int i = 0; i < NumberOfFunctions; i++)
                addressesOfFunctions.Add(sw.ReadInteger());
            sw.Position = Offset(AddressOfNames);
            for (int i = 0; i < NumberOfNames; i++)
                addressesOfNames.Add(sw.ReadInteger());
            sw.Position = Offset(AddressOfNameOrdinals);
            for (int i = 0; i < NumberOfFunctions; i++)
                addressesOfOrdinals.Add(sw.ReadShort());
            appName = ReadText(sw, addressOfname);
            foreach (int ad in addressesOfNames)
            {
                string symb = ReadText(sw, ad);
                symbols.Add(symb);
            }
            for (int i = 0; i < NumberOfFunctions; i++)
            {
                EXPORTED_FUNCTION ex = new EXPORTED_FUNCTION(addressesOfFunctions[i], addressesOfOrdinals[i], addressesOfNames[i], symbols[i]);
                exports.Add(ex);
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        private string ReadText(BinaryFileReader sw, int ad)
        {
            long of = sw.Position;
            sw.Position = Offset(ad);
            List<byte> nm = new List<byte>();
            int a = sw.ReadByte();
            while (a != 0)
            {
                nm.Add((byte)a);
                a = sw.ReadByte();
            }
            string symb = Encoding.Default.GetString(nm.ToArray());
            sw.Position = of;
            return symb;
        }
        private long Offset(long Position)
        {
            return Position - baseOffset;
        }
        public override string ToString()
        {
            return "Export directory";
        }
    }
    #endregion
    #region Import Format
    // http://sandsprite.com/CodeStuff/Understanding_imports.html
    public class IMAGE_IMPORT_BY_NAME : IMAGE_BASE_DATA
    {
        private short hint;
        public short Hint
        {
            get { return hint; }
        }
        private string name;//[1];
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public List<byte> filler = new List<byte>();
        public IMAGE_IMPORT_BY_NAME(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            hint = sw.ReadShort();
            List<byte> nm = new List<byte>();
            int a = sw.ReadByte();
            while (a != 0)
            {
                nm.Add((byte)a);
                a = sw.ReadByte();
                if (sw.Position > sw.Length)
                    break;
            }
            while(a==0)
            {
                filler.Add((byte)a);
                a = sw.ReadByte();
            }
            sw.Position--;
            name = Encoding.Default.GetString(nm.ToArray());
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return hint.ToString("x4") + " " + name;
        }
    }
    public class IMAGE_THUNK_DATA : IMAGE_BASE_DATA
    {
        //Union type : use only one
        public long offset;
        public long baseOffset;
        public long Function;
        public uint Ordinal;
        IMAGE_IMPORT_BY_NAME addressOfData;
        public IMAGE_IMPORT_BY_NAME AddressOfData
        {
            get { return addressOfData; }
        }
        public IMAGE_THUNK_DATA(BinaryFileReader sw, long baseOffset)
        {
            this.baseOffset = baseOffset;
            PositionOfStructureInFile = sw.Position;
            if (Executable.Is64bits)
                Function = sw.ReadLongInt();
            else
                Function = (uint)sw.ReadInteger();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public void FillData(BinaryFileReader sw)
        {
            offset = sw.Position;

            uint test = 0;
            if (Executable.Is64bits)
                test = (uint)((Function & (0x4000000000000000 << 1)) >> 32);
            else
                test = (uint)Function & 0x80000000;
            if (test == 0x0)
            {
                if (Function - baseOffset > 0)
                {
                    sw.Position = Function - baseOffset;
                    addressOfData = new IMAGE_IMPORT_BY_NAME(sw);
                }
            }
            else
            {
                Ordinal = (uint)(Function & 0x0fFFFFFFFF);
            }
        }
        public override string ToString()
        {
            if (addressOfData != null)
                return addressOfData.Name;
            else return Function.ToString();
        }
    }
    public class IMAGE_IMPORT_DIRECTORY : IMAGE_BASE_DATA
    {
        private List<IMAGE_IMPORT_DESCRIPTOR> descriptors;

        public List<IMAGE_IMPORT_DESCRIPTOR> Descriptors
        {
            get { return descriptors; }
            set { descriptors = value; }
        }
        public IMAGE_IMPORT_DIRECTORY(BinaryFileReader sw, long baseOffset)
        {
            PositionOfStructureInFile = sw.Position;
            descriptors = new List<IMAGE_IMPORT_DESCRIPTOR>();
            IMAGE_IMPORT_DESCRIPTOR imdes = new IMAGE_IMPORT_DESCRIPTOR(sw, baseOffset);
            while (imdes.originalThunk != 0)
            {
                descriptors.Add(imdes);
                imdes = new IMAGE_IMPORT_DESCRIPTOR(sw, baseOffset);
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Import directory";
        }
    }
    public class IMAGE_IMPORT_DESCRIPTOR : IMAGE_BASE_DATA
    {
        public int originalThunk;
        private int timeDateStamp;

        public int forwarderChain;
        public int name;
        public string Name;
        public int firstThunk;
        List<IMAGE_THUNK_DATA> originalThunks = new List<IMAGE_THUNK_DATA>();
        List<IMAGE_THUNK_DATA> firstThunks = new List<IMAGE_THUNK_DATA>();
        #region Properties
        public int TimeDateStamp
        {
            get { return timeDateStamp; }
            set { timeDateStamp = value; }
        }
        public List<IMAGE_THUNK_DATA> FirstThunks
        {
            get { return firstThunks; }
            set { firstThunks = value; }
        }
        #endregion
        private long offset;
        public IMAGE_IMPORT_DESCRIPTOR(BinaryFileReader sw, long baseOffset)
        {
            PositionOfStructureInFile = sw.Position;
            originalThunk = sw.ReadInteger();
            timeDateStamp = sw.ReadInteger();
            forwarderChain = sw.ReadInteger();
            name = sw.ReadInteger();
            long pos = sw.Position;
            if (originalThunk == 0)
                return;
            sw.Position = name - baseOffset;
            List<byte> dat = new List<byte>();
            byte b = (byte)sw.ReadByte();
            while (b != 0)
            {
                dat.Add(b);
                b = (byte)sw.ReadByte();
                if (sw.Position > sw.Length)
                    break;
            }
            Name = Encoding.Default.GetString(dat.ToArray());
            sw.Position = pos;
            firstThunk = sw.ReadInteger();
            offset = sw.Position;
            sw.Position = originalThunk - baseOffset;
            IMAGE_THUNK_DATA or = new IMAGE_THUNK_DATA(sw, baseOffset);
            while (or.Function != 0)
            {
                firstThunks.Add(or);
                or = new IMAGE_THUNK_DATA(sw, baseOffset);
            }
            foreach (IMAGE_THUNK_DATA im in firstThunks)
            {
                im.FillData(sw);
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
            sw.Position = offset;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    #endregion
    /*     * Thread Local Storage
 

     void
    (MODENTRY *PIMAGE_TLS_CALLBACK) (
        PTR DllHandle,
        public int Reason,
        PTR Reserved
        );
        */
    public class IMAGE_TLS_DIRECTORY
    {
        public int StartAddressOfRawData;
        public int EndAddressOfRawData;
        public int AddressOfIndex;
        //    IMAGE_TLS_CALLBACK *AddressOfCallBacks;
        public int SizeOfZeroFill;
        public int Characteristics;
    }
    #endregion
    #region Resource Format
    /*
     * Resource directory consists of two counts, following by tag variable strLength
     * array of directory entries.  The first count is the number of entries at
     * beginning of the array that have actual names associated with each entry.
     * The entries are in ascending order, case insensitive strings.  The second
     * count is the number of entries that immediately follow the named entries.
     * This second count identifies the number of entries that have 31-bit integer
     * Ids as their nameIndex.  These entries are also sorted in ascending order.
     *
     * This public structure allows fast lookup by either nameIndex or number, but for any
     * given resource entry only one form of lookup is supported, not both.
     * This is consistant with the syntax of the .RC file and the .RES file.
     */
    public class IMAGE_RESOURCE_DIRECTORY : IMAGE_BASE_DATA
    {
        public static long StartOfResources;
        public static long RessourcesRVA;
        public int Level;
        public int Characteristics;//Resource Flags. This field is reserved for future use
        public int TimeDateStamp;
        public short MajorVersion;
        public short MinorVersion;
        public short NumberOfNamedEntries;//The number of directory entries immediately following the table that use strings to identify Type, nameIndex, or Language entries (depending on the level of the table).
        public short NumberOfIdEntries;//The number of directory entries immediately following the nameIndex entries that use numeric IDs for Type, nameIndex, or Language entries
        private List<IMAGE_RESOURCE_DIRECTORY_ENTRY> directoryEntries = new List<IMAGE_RESOURCE_DIRECTORY_ENTRY>();
        private List<IMAGE_RESOURCE_DATA_ENTRY> dataEntry = new List<IMAGE_RESOURCE_DATA_ENTRY>();
        List<IMAGE_RESOURCE_DIR_STRING_U> stringEntry = new List<IMAGE_RESOURCE_DIR_STRING_U>();
        private long offset;
        public List<IMAGE_RESOURCE_DIRECTORY_ENTRY> DirectoryEntries
        {
            get { return directoryEntries; }
            set { directoryEntries = value; }
        }
        public List<IMAGE_RESOURCE_DATA_ENTRY> DataEntries
        {
            get { return dataEntry; }
            set { dataEntry = value; }
        }
        public List<IMAGE_RESOURCE_DIR_STRING_U> StringEntries
        {
            get { return stringEntry; }
            set { stringEntry = value; }
        }
        public IMAGE_RESOURCE_DIRECTORY(BinaryFileReader sw, IMAGE_SECTION_HEADER im, int level)
        {
            PositionOfStructureInFile = sw.Position;
            offset = sw.Position;
            if (level == 0)
            {
                IMAGE_RESOURCE_DIRECTORY.StartOfResources = sw.Position;
                IMAGE_RESOURCE_DIRECTORY.RessourcesRVA = im.VirtualAddress;
            }
            Characteristics = sw.ReadInteger();
            TimeDateStamp = sw.ReadInteger();
            MajorVersion = sw.ReadShort();
            MinorVersion = sw.ReadShort();
            NumberOfNamedEntries = sw.ReadShort();
            NumberOfIdEntries = sw.ReadShort();
            for (int i = 0; i < NumberOfNamedEntries; i++)
            {
                IMAGE_RESOURCE_DIRECTORY_ENTRY imEn = new IMAGE_RESOURCE_DIRECTORY_ENTRY(sw, im, level);
                directoryEntries.Add(imEn);
            }
            for (int i = 0; i < NumberOfIdEntries; i++)
            {
                IMAGE_RESOURCE_DIRECTORY_ENTRY imEn = new IMAGE_RESOURCE_DIRECTORY_ENTRY(sw, im, level);
                directoryEntries.Add(imEn);
            }
            foreach (IMAGE_RESOURCE_DIRECTORY_ENTRY entry in directoryEntries)
            {
                if (entry.IsTextString)
                {
                    int off = (entry.name & 0x7fffffff) + (int)offset;
                    IMAGE_RESOURCE_DIR_STRING_U str = new IMAGE_RESOURCE_DIR_STRING_U(sw, off);
                    stringEntry.Add(str);
                }
                else
                    sw.Position = entry.DataAdress;
                if (entry.IsAnotherRessourceDir)
                {
                    try
                    {
                        IMAGE_RESOURCE_DIRECTORY imRDir = new IMAGE_RESOURCE_DIRECTORY(sw, im, level + 1);
                        entry.Subdirs.Add(imRDir);
                    }
                    catch { }
                }
                else
                {
                    IMAGE_RESOURCE_DATA_ENTRY imData = new IMAGE_RESOURCE_DATA_ENTRY(sw, (RESOURCE_TYPE)entry.name);
                    dataEntry.Add(imData);
                }
            }
            if (level == 0)
            {
                foreach (IMAGE_RESOURCE_DIRECTORY_ENTRY dire in directoryEntries)
                {
                    foreach (IMAGE_RESOURCE_DIRECTORY dir in dire.Subdirs)
                    {
                        try
                        {
                            ParseDataEntries(sw, dir, (RESOURCE_TYPE)dire.name);
                        }
                        catch { }
                    }
                }
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        private void ParseDataEntries(BinaryFileReader sw, IMAGE_RESOURCE_DIRECTORY dir, RESOURCE_TYPE res)
        {
            if (dir.DirectoryEntries.Count > 0)
                foreach (IMAGE_RESOURCE_DIRECTORY_ENTRY d in dir.DirectoryEntries)
                {
                    foreach (IMAGE_RESOURCE_DIRECTORY dd in d.Subdirs)
                    {
                        ParseDataEntries(sw, dd, res);
                    }
                }
            if (dir.DataEntries.Count > 0)
                foreach (IMAGE_RESOURCE_DATA_ENTRY dten in dir.DataEntries)
                {
                    dten.ReadData(sw, res);
                }
        }
        public override string ToString()
        {
            return "Number of Id Entries " + NumberOfIdEntries.ToString("x8");
        }
    }
    /*
     * Each directory contains the 32-bit nameIndex of the entry and an Real_Address,
     * relative to the beginning of the resource directory of the dataEntry associated
     * with this directory entry.  If the nameIndex of the entry is an actual text
     * string instead of an integer Id, then the high order bit of the nameIndex field
     * is set to one and the low order 31-bits are an Real_Address, relative to the
     * beginning of the resource directory of the string, which is of type
     * IMAGE_RESOURCE_DIRECTORY_STRING.  Otherwise the high bit is clear and the
     * low-order 31-bits are the integer Id that identify this resource directory
     * entry. If the directory entry is yet another resource directory (u.e. tag
     * subdirectory), then the high order bit of the Real_Address field will be
     * set to indicate this.  Otherwise the high bit is clear and the Real_Address
     * field points to tag resource dataEntry entry.
     */
    public class IMAGE_RESOURCE_DIRECTORY_ENTRY : IMAGE_BASE_DATA
    {
        public string Resource_type
        {
            get
            {
                if (level > 0)
                    return name.ToString();
                resType = (RESOURCE_TYPE)name;
                switch (resType)
                {
                    case RESOURCE_TYPE.RT_CURSOR: return "CURSOR";
                    case RESOURCE_TYPE.RT_BITMAP: return "BITMAP";
                    case RESOURCE_TYPE.RT_ICON: return "ICON";
                    case RESOURCE_TYPE.RT_MENU: return "MENU";
                    case RESOURCE_TYPE.RT_DIALOG: return "DIALOG";
                    case RESOURCE_TYPE.RT_STRING: return "STRING";
                    case RESOURCE_TYPE.RT_FONTDIR: return "FONTDIR";
                    case RESOURCE_TYPE.RT_FONT: return "FONT";
                    case RESOURCE_TYPE.RT_ACCELERATOR: return "ACCELERATOR";
                    case RESOURCE_TYPE.RT_RCDATA: return "RCDATA";
                    case RESOURCE_TYPE.RT_MESSAGETABLE: return "MESSAGETABLE";
                    case RESOURCE_TYPE.RT_GROUP_CURSOR: return "GROUP_CURSOR";
                    case RESOURCE_TYPE.RT_GROUP_ICON: return "GROUP_ICON";
                    case RESOURCE_TYPE.RT_VERSION: return "VERSION";
                    case RESOURCE_TYPE.RT_DLGINCLUDE: return "DLGINCLUDE";
                    case RESOURCE_TYPE.RT_PLUGPLAY: return "PLUGPLAY";
                    case RESOURCE_TYPE.RT_VXD: return "VXD";
                    case RESOURCE_TYPE.RT_ANICURSOR: return "ANICURSOR";
                    case RESOURCE_TYPE.RT_ANIICON: return "ANIICON";
                    case RESOURCE_TYPE.RT_HTML: return "HTML";
                    case RESOURCE_TYPE.RT_MANIFEST: return "MANIFEST";
                    default: return "";
                }
            }
        }
        private int level;
        public int name;
        public RESOURCE_TYPE resType;
        public int OffsetToData;
        public int DataAdress;
        private bool isAnotherRessourceDir;
        private bool isTextString;
        private List<IMAGE_RESOURCE_DIRECTORY> subdirs = new List<IMAGE_RESOURCE_DIRECTORY>();
        public List<IMAGE_RESOURCE_DIRECTORY> Subdirs
        {
            get { return subdirs; }
            set { subdirs = value; }
        }
        public bool IsAnotherRessourceDir
        {
            get { return isAnotherRessourceDir; }
            set { isAnotherRessourceDir = value; }
        }
        public bool IsTextString
        {
            get { return isTextString; }
            set { isTextString = value; }
        }
        public IMAGE_RESOURCE_DIRECTORY_ENTRY(BinaryFileReader sw, IMAGE_SECTION_HEADER im, int level)
        {
            PositionOfStructureInFile = sw.Position;
            this.level = level;
            //8 bytes
            name = sw.ReadInteger();//The addressOfname field stores an integer ID if its high bit is 0, or an PositionOfStructureInFile (in the lower 31 bits) to an IMAGE_RESOURCE_DIR_STRING_U structure if its high bit is 1. The PositionOfStructureInFile is relative to the startIAT of the resource sections_Directory, and this structure identifies a Unicode string that names a resource instance
            OffsetToData = sw.ReadInteger();//IMAGE_RESOURCE_DATA_ENTRY structure (which I'll discuss later) if its high bit is 0, or an PositionOfStructureInFile to another IMAGE_RESOURCE_DIRECTORY structure if its high bit is 1. Both of these offsets are relative to the beginning of the resource sections_Directory
            DataAdress = (OffsetToData & 0x7FFFFFF) + (int)IMAGE_RESOURCE_DIRECTORY.StartOfResources;
            isTextString = (name & 0x80000000) == 0x80000000;
            isAnotherRessourceDir = (OffsetToData & 0x80000000) == 0x80000000;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Name : " + name.ToString("x8") + " Offset : " + OffsetToData.ToString("x8");
        }
    }
    /*
     * For resource directory entries that have actual string names, the nameIndex
     * field of the directory entry points to an object of the following type.
     * All of these string nodes are stored together after the last resource
     * directory entry and before the first resource dataEntry object.  This minimizes
     * the impact of these variable strLength nodes on the alignment of the fixed
     * size directory entry nodes.
     */
    public class IMAGE_RESOURCE_DIRECTORY_STRING : IMAGE_BASE_DATA
    {
        private string nameString;
        public string NameString
        {
            get { return nameString; }
            set { nameString = value; }
        }
        public IMAGE_RESOURCE_DIRECTORY_STRING(BinaryFileReader sw, int off)
        {
            PositionOfStructureInFile = sw.Position;
            sw.Position = off;
            short strLength = sw.ReadShort();
            nameString = sw.ReadString(strLength);
            LengthInFile = sw.Position - PositionOfStructureInFile;
            sw.Position = PositionOfStructureInFile;

        }
        public override string ToString()
        {
            return NameString;
        }
    }
    public class IMAGE_RESOURCE_DIR_STRING_U : IMAGE_BASE_DATA
    {
        private string nameString;//[ 1 ];
        public string NameString
        {
            get { return nameString; }
            set { nameString = value; }
        }
        public IMAGE_RESOURCE_DIR_STRING_U(BinaryFileReader sw, int off)
        {
            PositionOfStructureInFile = sw.Position;
            sw.Position = off;
            short strLength = sw.ReadShort();
            nameString = Encoding.Unicode.GetString(sw.ReadBytes(2 * strLength));
            LengthInFile = sw.Position - PositionOfStructureInFile;
            sw.Position = PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    /*
     * Each resource dataEntry entry describes tag leaf node in the resource directory
     * tree.  It contains an Real_Address, relative to the beginning of the resource
     * directory of the dataEntry for the resource, tag size field that gives the number
     * of bytes of dataEntry at that Real_Address, tag CodePage that should be used when
     * decoding code point values within the resource dataEntry.  Typically for new
     * applications the code page would be the unicode code page.
     */
    public class IMAGE_RESOURCE_DATA : IMAGE_BASE_DATA
    {
        private Object ob = new object();

        public Object Ob
        {
            get { return ob; }
            set { ob = value; }
        }
        byte[] buffer;
        public IMAGE_RESOURCE_DATA(BinaryFileReader sw, RESOURCE_TYPE res, int Size)
        {
            PositionOfStructureInFile = sw.Position;
            switch (res)
            {
                //http://www.devsource.com/c/a/Architecture/Resources-From-PE-I/2/
                case RESOURCE_TYPE.RT_CURSOR:
                    break;
                case RESOURCE_TYPE.RT_BITMAP:
                    //                  BITMAPFILEHEADER bmfh;
                    /*                  BITMAPINFOHEADER pbmih;
                                      pbmih.biSize = ms.ReadInteger();
                                      pbmih.biWidth = ms.ReadLongInt();
                                      pbmih.biHeight = ms.ReadLongInt();
                                      pbmih.biPlanes = ms.ReadShort();
                                      pbmih.biBitCount = ms.ReadShort();
                                      pbmih.biCompression = ms.ReadInteger();
                                      pbmih.biSizeImage = ms.ReadInteger();
                                      pbmih.biXPelsPerMeter = ms.ReadLongInt();
                                      pbmih.biYPelsPerMeter = ms.ReadLongInt();
                                      pbmih.biClrUsed = ms.ReadInteger();
                                      pbmih.biClrImportant = ms.ReadInteger();*/
                    byte[] bitMap = sw.ReadBytes(Size);
                    Image b = Image.FromStream(new MemoryStream(bitMap));
                    break;
                case RESOURCE_TYPE.RT_ICON:
                    try
                    {
                        ICOHEADER icr = new ICOHEADER(sw);
                    }
                    catch { }
                    break;
                case RESOURCE_TYPE.RT_MENU:
                    break;
                case RESOURCE_TYPE.RT_DIALOG:
                    break;
                case RESOURCE_TYPE.RT_STRING:
                    sw.ReadShort();
                    int length = sw.ReadShort();
                    string st = sw.ReadStringZ(Encoding.Unicode);
                    ob = st;
                    break;
                case RESOURCE_TYPE.RT_FONTDIR:
                    break;
                case RESOURCE_TYPE.RT_FONT:
                    break;
                case RESOURCE_TYPE.RT_ACCELERATOR:
                    break;
                case RESOURCE_TYPE.RT_RCDATA:
                    break;
                case RESOURCE_TYPE.RT_MESSAGETABLE:
                    MESSAGE_RESOURCE_DATA mr = new MESSAGE_RESOURCE_DATA(sw);
                    ob = mr;
                    break;
                case RESOURCE_TYPE.RT_GROUP_CURSOR:
                    break; ;
                case RESOURCE_TYPE.RT_GROUP_ICON:
                    GRPICONDIR gpri = new GRPICONDIR(sw);
                    ob = gpri;
                    break;
                case RESOURCE_TYPE.RT_VERSION:
                    VS_VERSIONINFO vs = new VS_VERSIONINFO(sw);
                    ob = vs;
                    break;
                case RESOURCE_TYPE.RT_DLGINCLUDE:
                    break;
                case RESOURCE_TYPE.RT_PLUGPLAY:
                    break;
                case RESOURCE_TYPE.RT_VXD:
                    break;
                case RESOURCE_TYPE.RT_ANICURSOR:
                    break;
                case RESOURCE_TYPE.RT_ANIICON:
                    break;
                case RESOURCE_TYPE.RT_HTML:
                    break;
                case RESOURCE_TYPE.RT_MANIFEST:
                    buffer = sw.ReadBytes(Size);
                    string s = Encoding.Default.GetString(buffer);
                    ob = s;
                    break;
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class IMAGE_RESOURCE_DATA_ENTRY : IMAGE_BASE_DATA
    {
        private IMAGE_RESOURCE_DATA ob;

        public IMAGE_RESOURCE_DATA Ob
        {
            get { return ob; }
            set { ob = value; }
        }
        public int OffsetToData;
        public int Size;
        public int CodePage;
        public int Reserved;// Must be 0
        public long Real_Address;
        public byte[] buffer;
        public IMAGE_RESOURCE_DATA_ENTRY(BinaryFileReader sw, RESOURCE_TYPE resource)
        {
            PositionOfStructureInFile = sw.Position;
            OffsetToData = sw.ReadInteger(); //RVA
            Size = sw.ReadInteger();
            CodePage = sw.ReadInteger();
            Reserved = sw.ReadInteger();
            Real_Address = OffsetToData - (IMAGE_RESOURCE_DIRECTORY.RessourcesRVA - IMAGE_RESOURCE_DIRECTORY.StartOfResources);// -shift;// -NT_Headers.OptionalHeader.SizeOfHeaders;//shift = startofresources
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public void ReadData(BinaryFileReader sw, RESOURCE_TYPE res)
        {
            long ad = sw.Position;
            sw.Position = Real_Address;
            ob = new IMAGE_RESOURCE_DATA(sw, res, Size);
            sw.Position = ad;
        }
    }
    #region Version resource
    public class VS_FIXEDFILEINFO :IMAGE_BASE_DATA
    {
        private int dwSignature;//Contains the value 0xFEEF04BD
        private int dwStrucVersion;//The high-order word of this member contains the majorVersion version number, and the low-order word contains the minorVersion version number. 
        private int dwFileVersionMS;//The most significant 32 bits of the file'parentClass binary version number. This member is used with dwFileVersionLS to form tag 64-bit value used for numeric comparisons
        private int dwFileVersionLS;//The least significant 32 bits of the file'parentClass binary version number
        private int dwFileOS;
        private int dwFileType;
        private int dwFileSubtype;

        public int DwSignature
        {
            get { return dwSignature; }
            set { dwSignature = value; }
        }
        public int DwStrucVersion
        {
            get { return dwStrucVersion; }
            set { dwStrucVersion = value; }
        }
        public int DwFileVersionMS
        {
            get { return dwFileVersionMS; }
            set { dwFileVersionMS = value; }
        }
        public int DwFileVersionLS
        {
            get { return dwFileVersionLS; }
            set { dwFileVersionLS = value; }
        }
        public int DwFileOS
        {
            get { return dwFileOS; }
            set { dwFileOS = value; }
        }
        public int DwFileType
        {
            get { return dwFileType; }
            set { dwFileType = value; }
        }
        public int DwFileSubtype
        {
            get { return dwFileSubtype; }
            set { dwFileSubtype = value; }
        }

        public int dwProductVersionMS;//The most significant 32 bits of the binary version number of the product with which this file was distributed. This member is used with dwProductVersionLS to form tag 64-bit value used for numeric comparisons. 
        public int dwProductVersionLS;//The least significant 32 bits of the binary version number
        public int dwFileFlagsMask;
        public int dwFileFlags;

        public int dwFileDateMS;
        public int dwFileDateLS;
        public VS_FIXEDFILEINFO(BinaryFileReader sw)
        {
            /*     int File_Version:    12.0.6545.5000
       Product Version: 12.0.6545.0
       File Flags Mask: 0.63
       File Flags:      
       File OS:         NT (WINDOWS32)
       File Type:       APP
       File SubType:    UNKNOWN
       File Date:       00:00:00  00/00/0000*/
            PositionOfStructureInFile=sw.Position;
            dwSignature = sw.ReadInteger();
            dwStrucVersion = sw.ReadInteger();
            dwFileVersionMS = sw.ReadInteger();
            dwFileVersionLS = sw.ReadInteger();
            dwProductVersionMS = sw.ReadInteger();
            dwProductVersionLS = sw.ReadInteger();
            dwFileFlagsMask = sw.ReadInteger();
            dwFileFlags = sw.ReadInteger();
            dwFileOS = sw.ReadInteger();
            dwFileType = sw.ReadInteger();
            dwFileSubtype = sw.ReadInteger();
            dwFileDateMS = sw.ReadInteger();
            dwFileDateLS = sw.ReadInteger();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    #region Version Info
    public class VS_VERSIONINFO : IMAGE_BASE_DATA
    {
        public short wLength;
        public short wValueLength;
        public short wType;//This member is 1 if the version resource contains text userStrings and 0 if the version resource contains binary da
        public string szKey;//Unicode public string L"VS_VERSION_INFO". 
        public short Padding1;
        public short Padding2;
        private VS_FIXEDFILEINFO fixedFileInfo;
        private List<stringFileInfo> fileinfos = new List<stringFileInfo>();
        private List<VarFileInfo> varFileInfos = new List<VarFileInfo>();

        public OS_TYPE ostype
        { get { return (OS_TYPE)fixedFileInfo.DwFileOS; } }
        public VS_FIXEDFILEINFO FixedFileInfo
        {
            get { return fixedFileInfo; }
            set { fixedFileInfo = value; }
        }
        public List<stringFileInfo> Fileinfos
        {
            get { return fileinfos; }
            set { fileinfos = value; }
        }
        public List<VarFileInfo> VarFileInfos
        {
            get { return varFileInfos; }
            set { varFileInfos = value; }
        }
        public VS_VERSIONINFO(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            wLength = sw.ReadShort();
            long end = sw.Position - 2 + wLength;
            wValueLength = sw.ReadShort();
            wType = sw.ReadShort();
            szKey = sw.ReadStringZ(Encoding.Unicode);
            // Padding
            long padd = sw.Position % 4;
            sw.Position += (4 - padd);
            if (wValueLength > 0)
                fixedFileInfo = new VS_FIXEDFILEINFO(sw);
            while (sw.Position < end)
            {
                if ((sw.Position % 4) != 0)
                    sw.Position += (4 - sw.Position % 4);
                stringFileInfo fi = new stringFileInfo(sw);
                fileinfos.Add(fi);
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return szKey;
        }
    }
    public class stringFileInfo : IMAGE_BASE_DATA
    {
        public short wLength;//The strLength, in bytes, of the entire StringFileInfo block, including all structures indicated by the varFileInfos me
        public short wValueLength;//This member is always equal to zero. 
        public short wType;//1 if the version resource contains text userStrings and 0 if the version resource contains binary userStrings
        public string szKey;
        public short Padding;
        private stringTable children;
        private Var varChildren;

        public stringTable Children
        {
            get { return children; }
            set { children = value; }
        }
        public Var VarChildren
        {
            get { return varChildren; }
            set { varChildren = value; }
        }
        public stringFileInfo(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            wLength = sw.ReadShort();
            wValueLength = sw.ReadShort();
            wType = sw.ReadShort();
            szKey = sw.ReadStringZ(Encoding.Unicode);
            if ((sw.Position % 4) != 0)
                sw.Position += (4 - sw.Position % 4);
            switch (szKey.Trim())
            {
                case "StringFileInfo":
                    children = new stringTable(sw);
                    break;
                case "VarFileInfo":
                    varChildren = new Var(sw);
                    break;
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return szKey;
        }
    }
    public class VarFileInfo : IMAGE_BASE_DATA
    {
        public short wLength;
        public short wValueLength;
        public short wType;
        public string szKey;
        public short Padding;
        Var children;

        public Var Children
        {
            get { return children; }
            set { children = value; }
        }
        public VarFileInfo(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            wLength = sw.ReadShort();
            wValueLength = sw.ReadShort();
            wType = sw.ReadShort();
            szKey = sw.ReadStringZ(Encoding.Unicode);
            if ((sw.Position % 4) != 0)
                sw.Position += (4 - sw.Position % 4);
            PositionOfStructureInFile = sw.Position;
        }
        public override string ToString()
        {
            return szKey;
        }
    }
    public class stringTable : IMAGE_BASE_DATA
    {
        public short wLength;
        public short wValueLength;
        public short wType;
        public string szKey;//An 8-digit hexadecimal number stored as tag Unicode string. The four most significant digits represent the language identifier. The four least significant digits represent the code page for which the userStrings is formatted
        public short Padding;
        private List<Rstring> children = new List<Rstring>();

        public List<Rstring> Children
        {
            get { return children; }
            set { children = value; }
        }
        public stringTable(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            wLength = sw.ReadShort();
            long end = sw.Position - 2 + wLength;
            wValueLength = sw.ReadShort();//This member is always equal to zero
            wType = sw.ReadShort();
            szKey = sw.ReadStringZ(Encoding.Unicode);
            if ((sw.Position % 4) != 0)
                sw.Position += (4 - sw.Position % 4);
            while (sw.Position < end)
            {
                Rstring child = new Rstring(sw);
                children.Add(child);
                if ((sw.Position % 4) != 0)
                    sw.Position += (4 - sw.Position % 4);
            }
            PositionOfStructureInFile = sw.Position;
        }
        public override string ToString()
        {
            return szKey;
        }
    }
    //http://msdn.microsoft.com/en-us/library/ms646987(v=VS.85).aspx
    public class Rstring : IMAGE_BASE_DATA
    {
        public short wLength;
        public short wValueLength;
        public short wType;
        public string szKey;
        private string value;
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public Rstring(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            wLength = sw.ReadShort();
            wValueLength = sw.ReadShort();
            wType = sw.ReadShort();
            szKey = sw.ReadStringZ(Encoding.Unicode);
            if ((sw.Position % 4) != 0)
                sw.Position += (4 - sw.Position % 4);
            value = sw.ReadStringZ(Encoding.Unicode);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return szKey + " " + value;
        }
    }
    //http://msdn.microsoft.com/en-us/library/ms646994(v=VS.85).aspx
    public class Var : IMAGE_BASE_DATA
    {
        public short wLength;
        public short wValueLength;
        public short wType;//This member is 1 if the version resource contains text userStrings and 0 if the version resource contains binary userStrings. 
        public string szKey;
        private List<int> value = new List<int>();//An array of one or more values that are language and code page identifier pairs
        public List<int> Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public Var(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            wLength = sw.ReadShort();
            long end = sw.Position - 2 + wLength;
            wValueLength = sw.ReadShort();
            wType = sw.ReadShort();
            szKey = sw.ReadStringZ(Encoding.Unicode);
            if ((sw.Position % 4) != 0)
                sw.Position += (4 - sw.Position % 4);
            while (sw.Position < end)
                value.Add(sw.ReadInteger());
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return szKey;
        }
    }
    #endregion
    #endregion
    public struct RGBQUAD
    {
        byte rgbBlue;
        byte rgbGreen;
        byte rgbRed;
        byte rgbReserved;
    }
    public class BITMAPFILEHEADER : IMAGE_BASE_DATA
    {
        public short bfType;
        public int bfSize;
        public short bfReserved1;
        public short bfReserved2;
        public int bfOffBits;
        public BITMAPFILEHEADER(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class BITMAPINFOHEADER : IMAGE_BASE_DATA
    {
        public int biSize;//Specifies the size of the structure, in bytes
        public long biWidth;
        public long biHeight;
        public short biPlanes;//Specifies the number of planes for the target device. This value must be set to 1. 
        public short biBitCount;//the number of bits that define each pixel 
        public int biCompression;
        public int biSizeImage;
        public long biXPelsPerMeter;
        public long biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
        public BITMAPINFOHEADER(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public struct ICONIMAGE
    {
        public BITMAPINFOHEADER icHeader;      // DIB header
        public RGBQUAD[] icColors;//[1];   // Color table
        public byte[] icXOR;//[1];      // DIB bits for XOR mask
        public byte[] icAND;//[1];      // DIB bits for AND mask
    }
    #endregion
    #region Load Configuration Directory Entry
    public class IMAGE_LOAD_CONFIG_DIRECTORY : IMAGE_BASE_DATA
    {
        public int Characteristics;
        public int TimeDateStamp;
        public short MajorVersion;
        public short MinorVersion;
        public int GlobalFlagsClear;
        public int GlobalFlagsSet;
        public int CriticalSectionDefaultTimeout;
        public int DeCommitFreeBlockThreshold;
        public int DeCommitTotalFreeThreshold;
        public int[] Reserved;//[ 8 ];
    }
    /*
     * Function table entry format for MIPS/ALPHA images.  Function table is
     * pointed to by the IMAGE_DIRECTORY_ENTRY_EXCEPTION directory entry.
     * This definition duplicates ones in ntmips.h and ntalpha.h for use
     * by portable image file mungers.
     */
    public class IMAGE_RUNTIME_FUNCTION_ENTRY : IMAGE_BASE_DATA
    {
        public int BeginAddress;
        public int EndAddress;
        IntPtr ExceptionHandler;
        IntPtr HandlerData;
        public int PrologEndAddress;
    }
    #endregion
    #region  Debug Format
    public class IMAGE_DEBUG_DIRECTORY :IMAGE_BASE_DATA
    {
        public int characteristics;
        public int timeDateStamp;

        public string TimeDateStamp
        {
            get
            {
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
                TimeSpan ts = new TimeSpan(0, 0, timeDateStamp);
                dt = dt + ts;
                return dt.ToLongDateString() + " " + dt.ToLongTimeString();
            }
        }
        public short MajorVersion;
        public short MinorVersion;
        public int Type;
        public int SizeOfData;
        public int AddressOfRawData;
        public int PointerToRawData;
        IMAGE_DEBUG debType;
        internal IMAGE_DEBUG DebugType
        {
            get { return debType; }
            set { debType = value; }
        }
        public IMAGE_DEBUG_DIRECTORY(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            characteristics = sw.ReadInteger();
            timeDateStamp = sw.ReadInteger();
            MajorVersion = sw.ReadShort();
            MinorVersion = sw.ReadShort();
            Type = sw.ReadInteger();
            SizeOfData = sw.ReadInteger();
            AddressOfRawData = sw.ReadInteger();
            PointerToRawData = sw.ReadInteger();
            debType = (IMAGE_DEBUG)Type;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public IMAGE_DEBUG_DIRECTORY()
        {
        }
        public override string ToString()
        {
            return "Debug";
        }
    }
    public class IMAGE_COFF_SYMBOLS_HEADER : IMAGE_BASE_DATA
    {
        public int NumberOfSymbols;
        public int LvaToFirstSymbol;
        public int NumberOfLinenumbers;
        public int LvaToFirstLinenumber;
        public int RvaToFirstByteOfCode;
        public int RvaToLastByteOfCode;
        public int RvaToFirstByteOfData;
        public int RvaToLastByteOfData;
    }
    public class _FPO_DATA : IMAGE_BASE_DATA
    {
        public int ulOffStart;             /* Real_Address 1st byte of function code */
        public int cbProcSize;             /* # bytes in function */
        public int cdwLocals;              /* # bytes in locals/4 */
        public short cdwParams;              /* # bytes in params/4 */
        public short cbProlog;// : 8;           /* # bytes in prolog */
        public short cbRegs;//  : 3;           /* # regs saved */
        public short fHasSEH;// : 1;           /* TRUE if SEH in codeBuffer */
        public short fUseBP;//  : 1;           /* TRUE if EBP has been allocated */
        public short reserved;// : 1;           /* reserved for future use */
        public short cbFrame;// : 2;           /* frame type */
    }
    public class IMAGE_DEBUG_MISC : IMAGE_BASE_DATA
    {
        public int DataType;               /* type of misc dataEntry, see defines */
        public int Length;                 /* total strLength of record, rounded to four */
        /* byte multiple. */
        public byte Unicode;                /* TRUE if dataEntry is unicode string */
        public byte[] Reserved;//[ 3 ];
        public byte[] Data;//[ 1 ];              /* Actual dataEntry */
    }
    /*
     * Debugging information can be stripped from an image file and placed
     * in tag separate .DBG file, whose file nameIndex part is the same as the
     * image file nameIndex part (e.g. symbols for CMD.EXE could be stripped
     * and placed in CMD.DBG).  This is indicated by the IMAGE_FILE_DEBUG_STRIPPED
     * flag in the originalThunk field of the file header.  The beginning of
     * the .DBG file contains the following public classure which captures certain
     * information from the image file.  This allows tag debug to proceed even if
     * the original image file is not accessable.  This header is followed by
     * zero of more IMAGE_SECTION_HEADER public classures, followed by zero or more
     * IMAGE_DEBUG_DIRECTORY public classures.  The latter public classures and those in
     * the image file contain file offsets relative to the beginning of the
     * .DBG file.
     *
     * If symbols have been stripped from an image, the IMAGE_DEBUG_MISC public classure
     * is left in the image file, but not mapped.  This allows tag debugger to
     * compute the nameIndex of the .DBG file, from the nameIndex of the image in the
     * IMAGE_DEBUG_MISC public classure.
     */
    public class IMAGE_SEPARATE_DEBUG_HEADER
    {
        public short Signature;
        public short Flags;
        public short Machine;
        public short Characteristics;
        public int TimeDateStamp;
        public int CheckSum;
        public int ImageBase;
        public int SizeOfImage;
        public int NumberOfSections;
        public int ExportedNamesSize;
        public int DebugDirectorySize;
        public int[] Reserved;//[ 3 ];
    }
    #endregion
    #region Clr
    /*     * COM+ 2.0 header public classure.    */
    public class IMAGE_COR20_HEADER : IMAGE_BASE_DATA
    {
        public int Cb
        {
            get { return cb; }
            set { cb = value; }
        }
        public short MajorRuntimeVersion
        {
            get { return majorRuntimeVersion; }
            set { majorRuntimeVersion = value; }
        }
        public string FlagData
        {
            get { return flagData; }
            set { flagData = value; }
        }
        public short MinorRuntimeVersion
        {
            get { return minorRuntimeVersion; }
            set { minorRuntimeVersion = value; }
        }
        public MetadataParser Metadata
        {
            get { return metadata; }
            set { metadata = value; }
        }
        /* Header versioning */
        private int cb;
        private short majorRuntimeVersion;
        private short minorRuntimeVersion;

        /* Symbol table and startup information */
        public int flags;
        public int EntryPointToken;
        private string flagData = "";
        /* Binding information */
        IMAGE_DATA_DIRECTORY Resources;
        IMAGE_DATA_DIRECTORY StrongNameSignature;
        /* Regular fixup and binding information */
        IMAGE_DATA_DIRECTORY CodeManagerTable;
        IMAGE_DATA_DIRECTORY VTableFixups;
        IMAGE_DATA_DIRECTORY ExportAddressTableJumps;
        /* Managed Native Code */
        IMAGE_DATA_DIRECTORY EEInfoTable;
        IMAGE_DATA_DIRECTORY HelperTable;
        IMAGE_DATA_DIRECTORY DynamicInfo;
        IMAGE_DATA_DIRECTORY DelayLoadInfo;
        IMAGE_DATA_DIRECTORY ModuleImage;
        IMAGE_DATA_DIRECTORY ExternalFixups;
        IMAGE_DATA_DIRECTORY RidMap;
        IMAGE_DATA_DIRECTORY DebugMap;
        private MetadataParser metadata;

        public IMAGE_COR20_HEADER(BinaryFileReader sw, long baseOffset)
        {
            PositionOfStructureInFile = sw.Position;
            cb = sw.ReadInteger();
            MajorRuntimeVersion = sw.ReadShort();
            minorRuntimeVersion = sw.ReadShort();
            int rva = sw.ReadInteger();
            int size = sw.ReadInteger();
            long start = sw.Position;
            if (size > 0)
            {
                sw.Position = rva - baseOffset;//correct address
                metadata = new MetadataParser(sw);
            }
            sw.Position = start;
            flags = sw.ReadInteger();
            if ((flags & 0x01) == 0x01)
                flagData += "ILONLY ";
            if ((flags & 0x02) == 0x02)
                flagData += "32 Bit Required ";
            if ((flags & 0x08) == 0x08)
                flagData += "Strong Name Signature ";
            if ((flags & 0x010000) == 0x010000)
                flagData += "Track Debug Data ";
            EntryPointToken = sw.ReadInteger();
            int Resources_RVA = sw.ReadInteger();
            int Resources_Size = sw.ReadInteger();
            if (Resources_Size > 0)
            {
                sw.Position = Resources_RVA - baseOffset;//correct address
                //         Resources = new IMAGE_DATA_DIRECTORY(ms, 0, nt_headers);
            }
            int Strong_Name_Signature_RVA = sw.ReadInteger();
            int Strong_Name_Signature_Size = sw.ReadInteger();
            if (Strong_Name_Signature_Size > 0)
            {
                //   StrongNameSignature = new IMAGE_DATA_DIRECTORY(ms, 0, nt_headers);
            }
            int Code_Manager_Table_RVA = sw.ReadInteger();
            int Code_Manager_Table_Size = sw.ReadInteger();
            if (Code_Manager_Table_Size > 0)
            {
                //   CodeManagerTable = new IMAGE_DATA_DIRECTORY(ms, 0, nt_headers);
            }
            int VTable_Fixups_RVA = sw.ReadInteger();
            int VTable_Fixups_Size = sw.ReadInteger();
            if (VTable_Fixups_Size > 0)
            {
                //   VTableFixups = new IMAGE_DATA_DIRECTORY(ms, 0, nt_headers);
            }
            int Export_Address_Table_Jumps_RVA = sw.ReadInteger();
            int Export_Address_Table_Jumps_Size = sw.ReadInteger();
            if (Export_Address_Table_Jumps_Size > 0)
            {
                //   ExportAddressTableJumps = new IMAGE_DATA_DIRECTORY(ms, 0, nt_headers);
            }
            int Managed_Native_Header_RVA = sw.ReadInteger();
            int Managed_Native_Header_Size = sw.ReadInteger();
            if (Managed_Native_Header_Size > 0)
            {

            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    #endregion
    #region RT_MessageTable
    public class MESSAGE_RESOURCE_DATA : IMAGE_BASE_DATA
    {
        /// DWORD->unsigned int
        public uint NumberOfBlocks;
        /// MESSAGE_RESOURCE_BLOCK[1]
        public MESSAGE_RESOURCE_BLOCK[] Blocks;
        public MESSAGE_RESOURCE_ENTRY[] Messages;
        public MESSAGE_RESOURCE_DATA(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            NumberOfBlocks = (uint) sw.ReadInteger();
            Blocks = new MESSAGE_RESOURCE_BLOCK[NumberOfBlocks];
            for (int i = 0; i < NumberOfBlocks; i++)
            {
                Blocks[i] = new MESSAGE_RESOURCE_BLOCK(sw);
                sw.Position = Blocks[i].OffsetToEntries + PositionOfStructureInFile;
                Messages[i] = new MESSAGE_RESOURCE_ENTRY(sw);
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
            List<string> str = new List<string>();
            Messages= new MESSAGE_RESOURCE_ENTRY[NumberOfBlocks];
        }
    }
    public class MESSAGE_RESOURCE_BLOCK : IMAGE_BASE_DATA
    {
        /// DWORD->unsigned int
        public uint LowId;
        /// DWORD->unsigned int
        public uint HighId;
        /// DWORD->unsigned int
        public uint OffsetToEntries;
        public MESSAGE_RESOURCE_BLOCK(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LowId = (uint)sw.ReadInteger();
            HighId = (uint)sw.ReadInteger();
            OffsetToEntries = (uint)sw.ReadInteger();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class MESSAGE_RESOURCE_ENTRY : IMAGE_BASE_DATA
    {
        /// WORD->unsigned short
        public ushort Length;
        /// WORD->unsigned short
        public ushort Flags;
        /// BYTE[1]
        public string Text;
        public MESSAGE_RESOURCE_ENTRY(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            Length = (ushort)sw.ReadShort();
            Flags = (ushort)sw.ReadShort();
            Text = sw.ReadString(Length, Encoding.Unicode);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    #endregion
    #region Icons
    public class ICOHEADER : IMAGE_BASE_DATA
    {
        public short wReserved;  // Always 0
        public short wResID;     // Always 1 : 1 for icon (.ICO) image, 2 for cursor (.CUR) image??
        public short wNumImages; // Number of icon images/directory entries
        List<ICODIRENTRY> icdir = new List<ICODIRENTRY>();
        public ICOHEADER(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            wReserved = sw.ReadShort();
            wResID = sw.ReadShort();
            wNumImages = sw.ReadShort();
            for (int i = 0; i < wNumImages; i++)
            {
                icdir.Add(new ICODIRENTRY(sw));
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class ICODIRENTRY : IMAGE_BASE_DATA
    {
        public byte bWidth;
        public byte bHeight;
        public byte bColors;
        public byte bReserved;
        public short wPlanes;
        public short wBitCount;
        public int dwBytesInImage;
        public short wID;
        public ICODIRENTRY(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            bWidth = (byte)sw.ReadByte();
            bHeight = (byte)sw.ReadByte();
            bColors = (byte)sw.ReadByte();
            bReserved = (byte)sw.ReadByte();
            wPlanes = sw.ReadShort();
            wBitCount = sw.ReadShort();
            dwBytesInImage = sw.ReadInteger();
            wID = sw.ReadShort();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class ICONDIRENTRY : IMAGE_BASE_DATA
    {
        public byte bWidth;
        public byte bHeight;
        public byte bColors;
        public byte bReserved;
        public short wPlanes;
        public short wBitCount;
        public int dwBytesInImage;
        public int dwImageOffset; // offset from startIAT of header to the image
        public ICONDIRENTRY(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            bWidth = (byte)sw.ReadByte();
            bHeight = (byte)sw.ReadByte();
            bColors = (byte)sw.ReadByte();
            bReserved = (byte)sw.ReadByte();
            wPlanes = sw.ReadShort();
            wBitCount = sw.ReadShort();
            dwBytesInImage = sw.ReadInteger();
            dwImageOffset = sw.ReadInteger();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class GRPICONDIR : IMAGE_BASE_DATA
    {
        public short idReserved; // Reserved (must be 0)
        public short idType; // Resource type (1 for icons)
        public short idcount; // How many images?
        public List<GRPICONDIRENTRY> idEntries; // The entries for each image
        public GRPICONDIR(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            idReserved = sw.ReadShort();
            idType = sw.ReadShort();
            idcount = sw.ReadShort();
            idEntries = new List<GRPICONDIRENTRY>();
            for (int i = 0; i < idcount; i++)
            {
                idEntries.Add(new GRPICONDIRENTRY(sw));
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class GRPICONDIRENTRY : IMAGE_BASE_DATA
    {
        public byte bWidth; // Width, in pixels, of the image
        public byte bHeight; // Height, in pixels, of the image
        public byte bColorCount; // Number of colors in image (0 if >=8bpp)
        public byte bReserved; // Reserved
        public short wPlanes; // Color Planes
        public short wBitCount; // Bits per pixel
        public int dwBytesInRes; // how many bytes in this resource?
        public short nID; // the ID
        public GRPICONDIRENTRY(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            bWidth = (byte)sw.ReadByte();
            bHeight = (byte)sw.ReadByte();
            bColorCount = (byte)sw.ReadByte();
            bReserved = (byte)sw.ReadByte();
            wPlanes = sw.ReadShort();
            wBitCount = sw.ReadShort();
            dwBytesInRes = sw.ReadInteger();
            nID = sw.ReadShort();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    #endregion
    #region Not used
    public class Data
    {
        public const int IMAGE_SIZEOF_FILE_HEADER = 20;
        public const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;
        public const int IMAGE_SIZEOF_BASE_RELOCATION = 8;
        public const int IMAGE_SIZEOF_STD_OPTIONAL_HEADER = 28;
        public const int IMAGE_SIZEOF_NT_OPTIONAL_HEADER = 224;
        public const int IMAGE_SIZEOF_SECTION_HEADER = 40;
        public const int IMAGE_SIZEOF_SYMBOL = 18;
        public const int IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b;
        public const uint IMAGE_RESOURCE_NAME_IS_STRING = 0x80000000;
        public const uint IMAGE_RESOURCE_DATA_IS_DIRECTORY = 0x80000000;
        public const int IMAGE_SEPARATE_DEBUG_SIGNATURE = 0x4944;
        public const int IMAGE_SIZEOF_SHORT_NAME = 8;
        public const int IMAGE_SIZEOF_LINENUMBER = 6;
        public const long IMAGE_ORDINAL_FLAG = 0x80000000;
        // IMAGE_SNAP_BY_ORDINAL(Ordinal) ((Ordinal & IMAGE_ORDINAL_FLAG) != 0)
        // IMAGE_ORDINAL(Ordinal) (Ordinal & 0xffff)
        public const int IMAGE_SIZEOF_RELOCATION = 10;
        public const int IMAGE_SIZEOF_AUX_SYMBOL = 18;

        public const int SIZEOF_RFPO_DATA = 16;
        public const bool IMAGE_DEBUG_MISC_EXENAME = true;
        public const int IMAGE_SIZEOF_ARCHIVE_MEMBER_HDR = 60;
        /*         * Archive format.         */
        public const int IMAGE_ARCHIVE_START_SIZE = 8;
        public const string IMAGE_ARCHIVE_START = "!<arch>\n";
        public const string IMAGE_ARCHIVE_END = "`\n";
        public const string IMAGE_ARCHIVE_PAD = "\n";
        public const string IMAGE_ARCHIVE_LINKER_MEMBER = "/               ";
        public const string IMAGE_ARCHIVE_LONGNAMES_MEMBER = "//              ";
    }
    public class IMAGE_OS2_HEADER
    {      /* OS/2 .EXE header */
        public short ne_magic;                    /* Magic number */
        public byte ne_ver;                      /* Version number */
        public byte ne_rev;                      /* Revision number */
        public short ne_enttab;                   /* offset of Entry Table */
        public short ne_cbenttab;                 /* Number of bytes in Entry Table */
        public int ne_crc;                      /* Checksum of whole file */
        public short ne_flags;                    /* Flag word */
        public short ne_autodata;                 /* Automatic dataEntry segment number */
        public short ne_heap;                     /* Initial heap allocation */
        public short ne_stack;                    /* Initial stack allocation */
        public int ne_csip;                     /* Initial CS:IP setting */
        public int ne_sssp;                     /* Initial SS:SP setting */
        public short ne_cseg;                     /* Count of file segments */
        public short ne_cmod;                     /* Entries in Module Reference Table */
        public short ne_cbnrestab;                /* size of non-resident nameIndex table */
        public short ne_segtab;                   /* offset of Segment Table */
        public short ne_rsrctab;                  /* offset of Resource Table */
        public short ne_restab;                   /* offset of resident nameIndex table */
        public short ne_modtab;                   /* offset of Module Reference Table */
        public short ne_imptab;                   /* offset of Imported Names Table */
        public int ne_nrestab;                  /* offset of Non-resident Names Table */
        public short ne_cmovent;                  /* Count of movable entries */
        public short ne_align;                    /* Segment alignment shift count */
        public short ne_cres;                     /* Count of resource segments */
        public byte ne_exetyp;                   /* Target Operating system */
        public byte ne_flagsothers;              /* Other .EXE Flags */
        public short ne_pretthunks;               /* Real_Address to return thunks */
        public short ne_psegrefbytes;             /* Real_Address to segment ref. bytes */
        public short ne_swaparea;                 /* Minimum code swap area size */
        public short ne_expver;                   /* Expected Windows version number */
    }
    /*
         IMAGE_FIRST_SECTION( ntheader ) ((PIMAGE_SECTION_HEADER)        
            ((public int)ntheader +                                                  
             FIELD_OFFSET( IMAGE_NT_HEADERS, optionalHeader ) +                 
             ((PIMAGE_NT_HEADERS)(ntheader))->fileHeader.SizeOfOptionalHeader   
            ))
        */
    #region Symbol format.
    public class IMAGE_SYMBOL
    {
        // union {
        public byte[] ShortName;//[8];
        public class Name
        {
            public int Short;     /* if 0, use LongName */
            public int Long;      /* Real_Address into string table */
        }
        public byte[] LongName;//[2];
        // }
        public int Value;
        public short SectionNumber;
        public short Type;
        public byte StorageClass;
        public byte NumberOfAuxSymbols;
    }
    #region C MACROS
    /* Basic Type of  Flags */
    // BTYPE(Flags) ((Flags) & N_BTMASK)

    /* Is Flags tag pointer? 
    #ifndef ISPTR
     ISPTR(Flags) (((Flags) & N_TMASK) == (IMAGE_SYM_DTYPE_POINTER << N_BTSHFT))
    #endif

    /* Is Flags tag function? 
    #ifndef ISFCN
     ISFCN(Flags) (((Flags) & N_TMASK) == (IMAGE_SYM_DTYPE_FUNCTION << N_BTSHFT))
    #endif

    /* Is Flags an array? 

    #ifndef ISARY
     ISARY(Flags) (((Flags) & N_TMASK) == (IMAGE_SYM_DTYPE_ARRAY << N_BTSHFT))
    #endif

    /* Is Flags tag public classure, union, or enumeration TAG? 
    #ifndef ISTAG
     ISTAG(Flags) ((Flags)==IMAGE_SYM_CLASS_STRUCT_TAG || (Flags)==IMAGE_SYM_CLASS_UNION_TAG || (Flags)==IMAGE_SYM_CLASS_ENUM_TAG)
    #endif

    #ifndef INCREF
     INCREF(Flags) ((((Flags)&~N_BTMASK)<<N_TSHifT)|(IMAGE_SYM_DTYPE_POINTER<<N_BTSHFT)|((Flags)&N_BTMASK))
    #endif
    #ifndef DECREF
     DECREF(Flags) ((((Flags)>>N_TSHifT)&~N_BTMASK)|((Flags)&N_BTMASK))
    #endif

    /*
     * Auxiliary entry format.
     */
    #endregion
    public class IMAGE_AUX_SYMBOL
    {
        public class Misc
        {
            public int TagIndex;                      /* public class, union, or enum tag index */
            public class unk
            {
                public class LnSz
                {
                    public short Linenumber;             /* declaration line number */
                    public short Size;                   /* size of public class, union, or enum */
                }
                public int TotalSize;
            } ;
            public class FcnAry
            {
                public class Function
                {                            /* if ISFCN, tag, or .bb */
                    public int PointerToLinenumber;
                    public int PointerToNextFunction;
                }
                public class Array
                {                            /* if ISARY, up to 4 dimen. */
                    public short[] Dimension;//[4];
                }
            } ;
            public short TvIndex;                        /* tv index */
        }
        public class File
        {
            public byte[] Name;//[IMAGE_SIZEOF_SYMBOL];
        }
        public class Section
        {
            public int Length;                         /* sections_Directory strLength */
            public short NumberOfRelocations;            /* number of relocation entries */
            public short NumberOfLinenumbers;            /* number of line numbers */
            public int CheckSum;                       /* checksum for communal */
            public short Number;                         /* sections_Directory number to associate with */
            public byte Selection;                      /* communal selection type */
        }
    }
    #endregion
    #region     Line number format.   */
    public class IMAGE_LINENUMBER
    {
        public class Type
        {
            public int SymbolTableIndex;                       /* Symbol table index of function nameIndex if Linenumber is 0. */
            public int VirtualAddress;                         /* Virtual startAddress of line number. */
        }
        public short Linenumber;                                 /* Line number. */
    }
    public class IMAGE_ARCHIVE_MEMBER_HEADER
    {
        public byte[] Name;//[16];                                  /* File member nameIndex - `/' terminated. */
        public byte[] Date;//[12];                                  /* File member date - decimal. */
        public byte[] UserID;//[6];                                 /* File member user id - decimal. */
        public byte[] GroupID;//[6];                                /* File member group id - decimal. */
        public byte[] Mode;//[8];                                   /* File member mode - octal. */
        public byte[] Size;//[10];                                  /* File member size - decimal. */
        public byte[] EndHeader;//[2];                              /* String to endCode header. */
    }
    #endregion
    #endregion
}
