using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace Code
{
    public class Executable : IMAGE_BASE_DATA
    {
        //http://www.vijaymukhi.com/documents/books/metadata/contents.htm 
        public static bool Is64bits;
        string fileName;
        IMAGE_DOS_HEADER exeheader;
        IMAGE_NT_HEADERS NT_headers;
        IMAGE_SECTION_DIRECTORY sections_Directory;
        IMAGE_RESOURCE_DIRECTORY imDir;
        IMAGE_BASE_RELOCATION a;
        long executableSize;
        Disassemble ds;
        public long imageDescriptorOffset;
        #region Properties
        public IMAGE_DATA_DIRECTORY[] DataDirs
        {
            get { return NT_headers.OptionalHeader.DataDirectory; }
        }
        public IMAGE_BASE_RELOCATION RelocationTable
        {
            get { return a; }
            set { a = value; }
        }
        public IMAGE_COR20_HEADER CLRHeader
        {
            get { return (DataDirs[14].clrHeader); }
        }
        public Disassemble Disassembly
        {
            get { return ds; }
            set { ds = value; }
        }
        public IMAGE_IMPORT_DIRECTORY_IAT IAT
        {
            get { return (DataDirs[12].IAT); }
        }
        public IMAGE_DEBUG_DIRECTORY Debug_data
        {
            get { return (DataDirs[6].eb); }
        }
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public IMAGE_DOS_HEADER Exeheader
        {
            get { return exeheader; }
            set { exeheader = value; }
        }
        public IMAGE_NT_HEADERS NT_Headers
        {
            get { return NT_headers; }
            set { NT_headers = value; }
        }
        public IMAGE_SECTION_DIRECTORY Sections
        {
            get { return sections_Directory; }
            set { sections_Directory = value; }
        }
        public IMAGE_RESOURCE_DIRECTORY Resource
        {
            get { return imDir; }
            set { imDir = value; }
        }
        public int SectionAlignment
        { get { return NT_Headers.OptionalHeader.SectionAlignment; } }
        public int FileAlignment
        { get { return NT_Headers.OptionalHeader.FileAlignment; } }
        #endregion
        public long ExecutableSize
        {
            get { return executableSize; }
            set { executableSize = value; }
        }
        public Executable(string FileName)
        {
            //http://support.microsoft.com/kb/65122
            fileName = FileName;
            BinaryFileReader sw = new BinaryFileReader(FileName, true);
            executableSize = sw.Length;
            #region Reading Headers
            exeheader = new IMAGE_DOS_HEADER(sw);
            sw.Position = exeheader.AddressOfNewHeader;
            NT_headers = new IMAGE_NT_HEADERS(sw);
            Is64bits = NT_headers.Is64bits;
            #endregion
            #region Sections
            sections_Directory = new IMAGE_SECTION_DIRECTORY(sw, NT_headers.FileHeader.NumberOfSections);
            #endregion
            #region Data directories
            for (int i = 0; i < NT_headers.OptionalHeader.DataDirectory.Length /* Data.IMAGE_NUMBEROF_DIRECTORY_ENTRIES*/; i++)
            {
                /*Ok, first you must find out in which sections_Directory the given RVA is. Then you calculate the file offset like this:
File offset := RVA - Virtual offset + Raw offset.
An Example: Assume we have the RVA 0x11A0. We see that this RVA is in the .text sections_Directory (0x11A0 is bigger than 1000 and smaller than 2000). Raw offset of the .text sections_Directory is 0x400. So File offset is 0x11A0 - 0x1000 + 0x400 = 0x5A0.
If RVA for example is 0x30D2 (it's in the .intData sections_Directory) file offset is 0x30D2 - 0x3000 + 0xA00 = 0xAD2. That's it!
                 */
                IMAGE_DATA_DIRECTORY dir = DataDirs[i];
                if (dir.Size > 0)
                {
                    for (int j = 0; j < sections_Directory.Count;j++ )
                    {
                        IMAGE_SECTION_HEADER sectionHeader = sections_Directory.Sections[j];
                        if ((dir.VirtualAddress >= sectionHeader.VirtualAddress) & (dir.VirtualAddress <= sectionHeader.VirtualAddress + sectionHeader.VirtualSize))
                        {
                            dir.sectionNumber = j;
                            dir.BaseOffset = sectionHeader.VirtualAddress - sectionHeader.pointerToRawData;
                            dir.OffsetInSection = dir.VirtualAddress - dir.BaseOffset;
                            sectionHeader.dataDirs.Add(dir);
                            break;
                        }
                    }
                    if (dir.Size > 0)
                        dir.Parse(sw);
                }
            }
            #endregion
            #region iATEntries
            if (DataDirs[0xC].IAT!=null)
                foreach (IMAGE_IMPORT_DIRECTORY_IAT_ENTRY im in DataDirs[0xC].IAT.IATEntries)
                {
                    string name = "";
                    foreach (IMAGE_IMPORT_DESCRIPTOR id in DataDirs[1].Import.Descriptors)
                    {
                        foreach (IMAGE_THUNK_DATA imt in id.FirstThunks)
                        {
                            if (imt.Function != 0)
                                if (imt.Function == im.intData)
                                {
                                    name = id.Name + "!" + imt.AddressOfData.Name;
                                    break;
                                }
                            if (name != "") break;
                        }
                    }
                    im.Name = name;
                }
            #endregion
            #region Parse sections_Directory
            foreach (IMAGE_SECTION_HEADER im in sections_Directory.Sections)
            {
                switch (im.Name.Replace("\0", ""))
                {
                    case ".text":
                        sw.Position = im.pointerToRawData;
                        try
                        {
                            if (DataDirs[0xe].Size == 0)
                                ds = new Disassemble(this, sw);
                        }
                        catch {
                            Console.WriteLine("error in dissassembler");
                        }
                        break;
                    case ".bss":
                        //uninitialized intData starts at the endIAT of the intData segment and contains all global variables and static variables
                        break;
                    case ".rdata":
                        break;
                    case ".data":
                        break;
                    case ".rsrc":
                        try
                        {
                            //http://blog.delroth.net/2010/12/acceder-tag-une-ressource-win32-de-type-stringtable-en-parsant-un-fichier-pe/
                            sw.Position = im.pointerToRawData;
                            imDir = new IMAGE_RESOURCE_DIRECTORY(sw, im, 0 /*, NT_headers*/);
                        }
                        catch { }
                        break;
                    case ".edata":
                        /*The .edata sections_Directory begins with the export directory structure IMAGE_EXPORT_DIRECTORY. 
                         * The export directory contains RVAs (relative virtual addresses) of the: Export Address Table:
                         * This contains address of exported entry points, exported intData and absolutes. An ordinal number
                         * is used to index the address table. The ORDINAL BASE must be subtracted from the ordinal number before indexing into the table.*/
                    case ".idata":
                        /*The .idata sections_Directory does the converse of what the .edata sections_Directory, described above, does. It maps symbols/ordinals back into RVAs. 
                         * The .idata begins with a import directory table IMAGE_IMPORT_DIRECTORY .The import directory table consists 
                         * of an array of IMAGE_IMPORT_DESCRIPTOR structures, one for each imported executable*/
                        break;
                    case ".pdata":
                        // Function table
                        sw.Position = im.pointerToRawData;
                        while (sw.Position < im.pointerToRawData + im.sizeOfRawData)
                        {
                            int Begin_Address = sw.ReadInteger();
                            int End_Address = sw.ReadInteger();
                            int Unwind_Information = sw.ReadInteger();
                        }
                        break;
                    case ".debug":
                        break;
                    case ".reloc":
                        sw.Position = im.pointerToRawData;
                        a = new IMAGE_BASE_RELOCATION(sw);
                        break;
                    default:
                        break;
                }
            }
            #endregion
        }
    }
    /*http://www.delorie.com/djgpp/doc/exe/
     00-01  0x4d, 0x5a. This is the "magic number" of an EXE file. The first byte of the file is 0x4d and the second is 0x5a. 
02-03  The number of bytes in the last block of the program that are actually used. If this value is zero, that means the entire last block is used (u.e. the effective value is 512). 
04-05  Number of blocks in the file that are part of the EXE file. If [02-03] is non-zero, only that much of the last block is used. 
06-07  Number of relocation entries stored after the header. May be zero. 
08-09  Number of paragraphs in the header. The program'parentClass dataEntry begins just after the header, and this field can be used to calculate the appropriate file Real_Address. The header includes the relocation entries. Note that some OSs and/or programs may fail if the header is not tag multiple of 512 bytes. 
0A-0B  Number of paragraphs of additional memory that the program will need. This is the equivalent of the BSS size in tag Unix program. The program can't be loaded if there isn't at least this much memory available to it. 
0C-0D  Maximum number of paragraphs of additional memory. Normally, the OS reserves all the remaining conventional memory for your program, but you can limit it with this field. 
0E-0F  Relative value of the stack segment. This value is added to the segment the program was loaded at, and the result is used to initialize the SS register. 
10-11  Initial value of the SP register. 
12-13  Word checksum. If set properly, the 16-bit sum of all words in the file should be zero. Usually, this isn't filled in. 
14-15  Initial value of the IP register. 
16-17  Initial value of the CS register, relative to the segment the program was loaded at. 
18-19  offset of the first relocation item in the file. 
1A-1B  Overlay number. Normally zero, meaning that it'parentClass the main program. 
     * 00h |  Old-style header info  |
        +-------------------------+
    20h |        Reserved         |
        +-------------------------+
    3Ch |   offset to segmented   |
        |       .EXE header       |
        +-------------------------+
    40h |  Relocation table and   |
        |  MS-DOS stub program    |
        +-------------------------+
        |  Segmented .EXE Header  |
       
    One block is 512 bytes, one paragraph is 16 bytes
     * The Real_Address of the beginning of the EXE dataEntry is computed like this:

exe_data_start = exe.header_paragraphs * 16L;

The Real_Address of the byte just after the EXE dataEntry (in DJGPP, the size of the stub and the startIAT of the COFF image) is computed like this:

extra_data_start = exe.blocks_in_file * 512L;
if (exe.bytes_in_last_block)
  extra_data_start -= (512 - exe.bytes_in_last_block);
    http://www.vowles-home.demon.co.uk/utils/file.callOrJump
     */
}
