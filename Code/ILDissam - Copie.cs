using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Code
{
    public class zzz
    {
        string[] typerefnames;
        string[] typedefnames;
        public static void Main(string args)
        {
            zzz a = new zzz();
            a.abc(args);
        }
        public void abc(string args)
        {
            InitializeObjects(args);
            ReadPEStructures();
            DisplayPEStructures();
            ImportAdressTable();
            CLRHeader();
            ReadStreamsData();
            FillTableSizes();
            ReadTablesIntoStructures();
            DisplayTableForDebugging();
        }
        public AssemblyTable[] AssemblyStruct;
        public AssemblyRefTable[] AssemblyRefStruct;
        public CustomAttributeTable[] CustomAttributeStruct;
        public ModuleTable[] ModuleStruct;
        public TypeDefTable[] TypeDefStruct;
        public TypeRefTable[] TypeRefStruct;
        public InterfaceImplTable[] InterfaceImplStruct;
        public MethodTable[] MethodStruct;
        public StandAloneSigTable[] StandAloneSigStruct;
        public MemberRefTable[] MemberRefStruct;
        public TypeSpecTable[] TypeSpecStruct;
        public ParamTable[] ParamStruct;
        public FieldTable[] FieldStruct;
        public FieldMarshalTable[] FieldMarshalStruct;
        public FieldRVATable[] FieldRVAStruct;
        public FieldLayoutTable[] FieldLayoutStruct;
        public ConstantsTable[] ConstantsStruct;
        public PropertyMapTable[] PropertyMapStruct;
        public PropertyTable[] PropertyStruct;
        public MethodSemanticsTable[] MethodSemanticsStruct;
        public EventTable[] EventStruct;
        public EventMapTable[] EventMapStruct;
        public FileTable[] FileStruct;
        public ModuleRefTable[] ModuleRefStruct;
        public ManifestResourceTable[] ManifestResourceStruct;
        public ClassLayoutTable[] ClassLayoutStruct;
        public MethodImpTable[] MethodImpStruct;
        public NestedClassTable[] NestedClassStruct;
        public ExportedTypeTable[] ExportedTypeStruct;
        public DeclSecurityTable[] DeclSecurityStruct;
        public ImplMapTable[] ImplMapStruct;
        int[] sizes;
        FileStream s;
        BinaryReader r;
        int subsystem;
        int stackreserve;
        int stackcommit;
        int datad;
        int entrypoint;
        int ImageBase;
        int sectiona;
        int filea;
        int[] datadirectoryrva;
        int[] datadirectorysize;
        long sectionoffset;
        short sections;
        int metadatarva;
        int entrypointtoken;
        int[] SVirtualAddress;
        int[] SSizeOfRawData;
        int[] SPointerToRawData;
        long startofmetadata;
        string[] streamnames;
        int tableoffset;
        int[] rows;
        long valid;
        byte[] metadata;
        byte[] strings;
        byte[] us;
        byte[] guid;
        byte[] blob;
        int[] offset;
        int[] ssize;
        byte[][] names;
        int offsetstring = 2;
        int offsetblob = 2;
        int offsetguid = 2;
        int vtablerva;
        int vtablesize;
        int exportaddressrva;
        int exportaddresssize;
        int corflags;
        string[] tablenames;
        string filename;
        public void InitializeObjects(string args)
        {
            tablenames = new String[]{"Module" , "TypeRef" ,"TypeDef" ,"FieldPtr","Field", "MethodPtr","Method","ParamPtr" , "Param", "InterfaceImpl", "MemberRef", "Constant", "CustomAttribute", "FieldMarshal", "DeclSecurity", "ClassLayout", "FieldLayout", "StandAloneSig" , "EventMap","EventPtr", "Event", "PropertyMap", "PropertyPtr", "Properties","MethodSemantics","MethodImpl","ModuleRef","TypeSpec","ImplMap","FieldRVA","ENCLog","ENCMap","Assembly",
"AssemblyProcessor","AssemblyOS","AssemblyRef","AssemblyRefProcessor",
"AssemblyRefOS","File","ExportedType","ManifestResource",
"NestedClass","TypeTyPar","MethodTyPar"};
            if (args.Length == 0)
                filename = "C:\\mdata\\b.exe";
            else
                filename = args;
        }
        public void ReadPEStructures()
        {
            try
            {
                s = new FileStream(filename, FileMode.Open);
            }
            catch { }
            r = new BinaryReader(s);
            s.Seek(60, SeekOrigin.Begin);
            int ii = r.ReadInt32();
            s.Seek(ii, SeekOrigin.Begin);
            byte sig1, sig2, sig3, sig4;
            sig1 = r.ReadByte();
            sig2 = r.ReadByte();
            sig3 = r.ReadByte();
            sig4 = r.ReadByte();
            //First Structure
            short machine = r.ReadInt16();
            sections = r.ReadInt16();
            int time = r.ReadInt32();
            int pointer = r.ReadInt32();
            int symbols = r.ReadInt32();
            int headersize = r.ReadInt16();
            int characteristics = r.ReadInt16();
            sectionoffset = s.Position + headersize;
            //Second Structure
            int magic = r.ReadInt16();
            int major = r.ReadByte();
            int minor = r.ReadByte();
            int sizeofcode = r.ReadInt32();
            int sizeofdata = r.ReadInt32();
            int sizeofudata = r.ReadInt32();
            entrypoint = r.ReadInt32();
            int baseofcode = r.ReadInt32();
            int baseofdata = r.ReadInt32();
            ImageBase = r.ReadInt32();
            sectiona = r.ReadInt32();
            filea = r.ReadInt32();
            int majoros = r.ReadInt16();
            int minoros = r.ReadInt16();
            int majorimage = r.ReadInt16();
            int minorimage = r.ReadInt16();
            int majorsubsystem = r.ReadInt16();
            int minorsubsystem = r.ReadInt16();
            int verison = r.ReadInt32();
            int imagesize = r.ReadInt32();
            int sizeofheaders = r.ReadInt32();
            int checksum = r.ReadInt32();
            subsystem = r.ReadInt16();
            int dllflags = r.ReadInt16();
            stackreserve = r.ReadInt32();
            stackcommit = r.ReadInt32();
            int heapreserve = r.ReadInt32();
            int heapcommit = r.ReadInt32();
            int loader = r.ReadInt32();
            datad = r.ReadInt32();
            datadirectoryrva = new int[16];
            datadirectorysize = new int[16];
            for (int i = 0; i <= 15; i++)
            {
                datadirectoryrva[i] = r.ReadInt32();
                datadirectorysize[i] = r.ReadInt32();
            }
            if (datadirectorysize[14] == 0)
                throw new System.Exception("Not a valid CLR file");
            s.Position = sectionoffset;
            SVirtualAddress = new int[sections];
            SSizeOfRawData = new int[sections];
            SPointerToRawData = new int[sections];
            for (int i = 0; i < sections; i++)
            {
                r.ReadBytes(12);
                SVirtualAddress[i] = r.ReadInt32();
                SSizeOfRawData[i] = r.ReadInt32();
                SPointerToRawData[i] = r.ReadInt32();
                r.ReadBytes(16);
            }
        }
        public void DisplayPEStructures()
        {
            Console.WriteLine("");
            Console.WriteLine("// Microsoft (R) .NET Framework IL Disassembler. Version 1.0.3705.0");
            Console.WriteLine("// Copyright (C) Microsoft Corporation 1998-2001. All rights reserved.");
            Console.WriteLine("");
            Console.WriteLine("// PE Header:");
            Console.WriteLine("// Subsystem: {0}", subsystem.ToString("x8"));
            Console.WriteLine("// Native entry point address: {0}", entrypoint.ToString("x8"));
            Console.WriteLine("// Image base: {0}", ImageBase.ToString("x8"));
            Console.WriteLine("// Section alignment: {0}", sectiona.ToString("x8"));
            Console.WriteLine("// File alignment: {0}", filea.ToString("x8"));
            Console.WriteLine("// Stack reserve size: {0}", stackreserve.ToString("x8"));
            Console.WriteLine("// Stack commit size: {0}", stackcommit.ToString("x8"));
            Console.WriteLine("// Directories: {0}", datad.ToString("x8"));
            DisplayDataDirectory(datadirectoryrva[0], datadirectorysize[0], "Export Directory");
            DisplayDataDirectory(datadirectoryrva[1], datadirectorysize[1], "Import Directory");
            DisplayDataDirectory(datadirectoryrva[2], datadirectorysize[2], "Resource Directory");
            DisplayDataDirectory(datadirectoryrva[3], datadirectorysize[3], "Exception Directory");
            DisplayDataDirectory(datadirectoryrva[4], datadirectorysize[4], "Security Directory");
            DisplayDataDirectory(datadirectoryrva[5], datadirectorysize[5], "Base Relocation Table");
            DisplayDataDirectory(datadirectoryrva[6], datadirectorysize[6], "Debug Directory");
            DisplayDataDirectory(datadirectoryrva[7], datadirectorysize[7], "Architecture Specific");
            DisplayDataDirectory(datadirectoryrva[8], datadirectorysize[8], "Global Pointer");
            DisplayDataDirectory(datadirectoryrva[9], datadirectorysize[9], "TLS Directory");
            DisplayDataDirectory(datadirectoryrva[10], datadirectorysize[10], "Load Config Directory");
            DisplayDataDirectory(datadirectoryrva[11], datadirectorysize[11], "Bound Import Directory");
            DisplayDataDirectory(datadirectoryrva[12], datadirectorysize[12], "Import Address Table");
            DisplayDataDirectory(datadirectoryrva[13], datadirectorysize[13], "Delay Load IAT");
            DisplayDataDirectory(datadirectoryrva[14], datadirectorysize[14], "CLR Header");
            Console.WriteLine("");
        }
        public void DisplayDataDirectory(int rva, int size, string ss)
        {
            string sfinal = "";
            sfinal = String.Format("// {0:x}", rva);
            sfinal = sfinal.PadRight(12);
            sfinal = sfinal + String.Format("[{0:x}", size);
            sfinal = sfinal.PadRight(21);
            sfinal = sfinal + String.Format("] address [size] of {0}:", ss);
            if (ss == "CLR Header")
                sfinal = sfinal.PadRight(67);
            else
                sfinal = sfinal.PadRight(68);
            Console.WriteLine(sfinal);
        }
        public void ImportAdressTable()
        {
            long stratofimports = ConvertRVA(datadirectoryrva[1]);
            s.Position = stratofimports;
            Console.WriteLine("// Import Address Table");
            int j = 0;
            while (true)
            {
                int rvaimportlookuptable = r.ReadInt32();
                if (rvaimportlookuptable == 0)
                    break;
                int datetimestamp = r.ReadInt32();
                int forwarderchain = r.ReadInt32();
                int name = r.ReadInt32();
                int rvaiat = r.ReadInt32();
                s.Position = ConvertRVA(name);
                Console.Write("// ");
                DisplayStringFromFile();
                Console.WriteLine("// {0} Import Address Table", rvaiat.ToString("x8"));
                Console.WriteLine("// {0} Import Name Table", name.ToString("x8"));
                Console.WriteLine("// {0} time date stamp", datetimestamp);
                Console.WriteLine("// {0} Index of first forwarder reference", forwarderchain);
                Console.WriteLine("//");
                int jj = 0;
                while (true)
                {
                    long pos = ConvertRVA(rvaimportlookuptable);
                    if (pos == -1)
                        break;
                    s.Position = pos + jj * 4;
                    int pos1 = r.ReadInt32();
                    if (pos1 == 0)
                        break;
                    pos = ConvertRVA(rvaimportlookuptable);
                    if (pos == -1)
                        break;
                    s.Position = pos;
                    short hint = r.ReadInt16();
                    Console.Write("// ");
                    if (hint.ToString("X").Length == 1)
                        Console.Write(" {0}", hint.ToString("x"));
                    if (hint.ToString("X").Length == 2)
                        Console.Write(" {0}", hint.ToString("x"));
                    if (hint.ToString("X").Length == 3)
                        Console.Write("{0}", hint.ToString("x"));
                    Console.Write(" ");
                    DisplayStringFromFile();
                    jj++;
                }
                Console.WriteLine("");
                j++;
                s.Position = stratofimports + j * 20;
            }
            Console.WriteLine("// Delay Load Import Address Table");
            if (datadirectoryrva[13] == 0)
                Console.WriteLine("// No data.");
        }
        public void DisplayStringFromFile()
        {
            while (true)
            {
                byte b = (byte)s.ReadByte();
                if (b == 0)
                    break;
                Console.Write("{0}", (char)b);
            }
            Console.WriteLine("");
        }
        public void FillTableSizes()
        {
            int modulesize = 2 + offsetstring + offsetguid + offsetguid + offsetguid;
            int typerefsize = GetCodedIndexSize("ResolutionScope") + offsetstring + offsetstring;
            int typedefsize = 4 + offsetstring + offsetstring + GetCodedIndexSize("TypeDefOrRef") + GetTableSize() + GetTableSize();
            int fieldsize = 2 + offsetstring + offsetblob;
            int methodsize = 4 + 2 + 2 + offsetstring + offsetblob + GetTableSize();
            int paramsize = 2 + 2 + offsetstring;
            int interfaceimplsize = GetTableSize() + GetCodedIndexSize("TypeDefOrRef");
            int memberrefsize = GetCodedIndexSize("MemberRefParent") + offsetstring + offsetblob;
            int constantsize = 2 + GetCodedIndexSize("HasConst") + offsetblob;
            int customattributesize = GetCodedIndexSize("HasCustomAttribute") + GetCodedIndexSize("HasCustomAttributeType") + offsetblob;
            int fieldmarshallsize = GetCodedIndexSize("HasFieldMarshal") + offsetblob;
            int declsecuritysize = 2 + GetCodedIndexSize("HasDeclSecurity") + offsetblob;
            int classlayoutsize = 2 + 4 + GetTableSize();
            int fieldlayoutsize = 4 + GetTableSize();
            int stanalonssigsize = offsetblob;
            int eventmapsize = GetTableSize() + GetTableSize();
            int eventsize = 2 + offsetstring + GetCodedIndexSize("TypeDefOrRef");
            int propertymapsize = GetTableSize() + GetTableSize();
            int propertysize = 2 + offsetstring + offsetblob;
            int methodsemantics = 2 + GetTableSize() + GetCodedIndexSize("HasSemantics");
            int methodimplsize = GetTableSize() + GetCodedIndexSize("MethodDefOrRef") + GetCodedIndexSize("MethodDefOrRef");
            int modulerefsize = offsetstring;
            int typespecsize = offsetblob;
            int implmapsize = 2 + GetCodedIndexSize("MemberForwarded") + offsetstring + GetTableSize();
            int fieldrvasize = 4 + GetTableSize();
            int assemblysize = 4 + 2 + 2 + 2 + 2 + 4 + offsetblob + offsetstring + offsetstring;
            int assemblyrefsize = 2 + 2 + 2 + 2 + 4 + offsetblob + offsetstring + offsetstring + offsetblob;
            int filesize = 4 + offsetstring + offsetblob;
            int exportedtype = 4 + 4 + offsetstring + offsetstring + GetCodedIndexSize("Implementation");
            int manifestresourcesize = 4 + 4 + offsetstring + GetCodedIndexSize("Implementation");
            int nestedclasssize = GetTableSize() + GetTableSize();
            sizes = new int[] { modulesize, typerefsize, typedefsize, 2, fieldsize, 2, methodsize, 2, paramsize, interfaceimplsize, memberrefsize, constantsize, customattributesize, fieldmarshallsize, declsecuritysize, classlayoutsize, fieldlayoutsize, stanalonssigsize, eventmapsize, 2, eventsize, propertymapsize, 2, propertysize, methodsemantics, methodimplsize, modulerefsize, typespecsize, implmapsize, fieldrvasize, 2, 2, assemblysize, 4, 12, assemblyrefsize, 6, 14, filesize, exportedtype, manifestresourcesize, nestedclasssize };
        }
        public int GetCodedIndexSize(string i)
        {
            if (i == "Implementation")
            {
                if (rows[0x26] >= 16384 || rows[0x23] >= 16384 || rows[0x27] >= 16384)
                    return 4;
                else
                    return 2;
            }
            else if (i == "MemberForwarded")
            {
                if (rows[0x04] >= 32768 || rows[0x06] >= 32768)
                    return 4;
                else
                    return 2;
            }
            else if (i == "MethodDefOrRef")
            {
                if (rows[0x06] >= 32768 || rows[0x0A] >= 32768)
                    return 4;
                else
                    return 2;
            }
            else if (i == "HasSemantics")
            {
                if (rows[0x14] >= 32768 || rows[0x17] >= 32768)
                    return 4;
                else
                    return 2;
            }
            else if (i == "HasDeclSecurity")
            {
                if (rows[0x02] >= 16384 || rows[0x06] >= 16384 || rows[0x20] >= 16384)
                    return 4;
                else
                    return 2;
            }
            else if (i == "HasFieldMarshal")
            {
                if (rows[0x04] >= 32768 || rows[0x08] >= 32768)
                    return 4;
                else
                    return 2;
            }
            else if (i == "TypeDefOrRef")
            {
                if (rows[0x02] >= 16384 || rows[0x01] >= 16384 || rows[0x1B] >= 16384)
                    return 4;
                else
                    return 2;
            }
            else if (i == "ResolutionScope")
            {
                if (rows[0x00] >= 8192 || rows[0x1a] >= 8192 || rows[0x23] >= 8192 || rows[0x01] >= 8192)
                    return 4;
                else
                    return 2;
            }
            else if (i == "HasConst")
            {
                if (rows[4] >= 16384 || rows[8] >= 16384 || rows[0x17] >= 16384)
                    return 4;
                else
                    return 2;
            }
            else if (i == "MemberRefParent")
            {
                if (rows[0x01] >= 8192 || rows[0x1a] >= 8192 || rows[0x06] >= 8192 || rows[0x1b] >= 8192)
                    return 4;
                else
                    return 2;
            }
            else if (i == "HasCustomAttribute")
            {
                if (rows[0x06] >= 2048 || rows[0x04] >= 2048 || rows[0x01] >= 2048 || rows[0x02] >= 2048 || rows[0x08] >= 2048 || rows[0x09] >= 2048 || rows[0x0a] >= 2048 || rows[0x00] >= 2048 || rows[0x0e] >= 2048 || rows[0x17] >= 2048 || rows[0x14] >= 2048 || rows[0x11] >= 2048 || rows[0x1a] >= 2048 || rows[0x1b] >= 2048 || rows[0x20] >= 2048 || rows[0x23] >= 2048 || rows[0x26] >= 2048 || rows[0x27] >= 2048 || rows[0x28] >= 2048)
                    return 4;
                else
                    return 2;
            }
            else if (i == "HasCustomAttributeType")
            {
                if (rows[0x06] >= 8192 || rows[0x0a] >= 8192)
                    return 4;
                else
                    return 2;
            }
            else
                return 2;
        }
        public int ReadCodedIndex(byte[] a, int o, string i)
        {
            int z = 0;
            int z1 = GetCodedIndexSize(i);
            if (z1 == 2)
                z = BitConverter.ToUInt16(a, o);
            if (z1 == 4)
                z = (int)BitConverter.ToUInt32(a, o);
            return z;
        }
        public bool tablepresent(byte i)
        {
            int p = (int)(valid >> i) & 1;
            for (int j = 0; j < i; j++)
            {
                int o = sizes[j] * rows[j];
                tableoffset = tableoffset + o;
            }
            if (p == 1)
                return true;
            else
                return false;
        }
        public void ReadTablesIntoStructures()
        {
            //Module
            int old = tableoffset;
            bool b = tablepresent(0);
            int offs = tableoffset;
            Console.WriteLine("\n Table Details \n\n");
            Console.WriteLine("Module Table Offset {0} Size {1}", offs, sizes[0]);
            tableoffset = old;
            if (b)
            {
                ModuleStruct = new ModuleTable[rows[0] + 1];
                for (int k = 1; k <= rows[0]; k++)
                {
                    ModuleStruct[k].Generation = BitConverter.ToUInt16(metadata, offs);
                    offs += 2;
                    ModuleStruct[k].Name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    ModuleStruct[k].Mvid = ReadGuidIndex(metadata, offs);
                    offs += offsetguid;
                    ModuleStruct[k].EncId = ReadGuidIndex(metadata, offs);
                    offs += offsetguid;
                    ModuleStruct[k].EncBaseId = ReadGuidIndex(metadata, offs);
                    offs += offsetguid;
                }
            }
            //TypeRef
            old = tableoffset;
            b = tablepresent(1);
            offs = tableoffset;
            Console.WriteLine("TypeRef Table Offset {0} Size {1}", offs, sizes[1]);
            tableoffset = old;
            if (b)
            {
                typerefnames = new string[rows[1] + 1];
                TypeRefStruct = new TypeRefTable[rows[1] + 1];
                for (int k = 1; k <= rows[1]; k++)
                {
                    TypeRefStruct[k].resolutionscope = ReadCodedIndex(metadata, offs, "ResolutionScope");
                    offs = offs + GetCodedIndexSize("ResolutionScope");
                    TypeRefStruct[k].name = ReadStringIndex(metadata, offs);
                    typerefnames[k] = GetString(TypeRefStruct[k].name);
                    offs = offs + offsetstring;
                    TypeRefStruct[k].nspace = ReadStringIndex(metadata, offs);
                    offs = offs + offsetstring;
                }
            }
            //TypeDef
            old = tableoffset;
            b = tablepresent(2);
            offs = tableoffset;
            Console.WriteLine("TypeDef Table Offset {0} Size {1}", offs, sizes[2]);
            tableoffset = old;
            if (b)
            {
                typedefnames = new string[rows[2] + 1];
                TypeDefStruct = new TypeDefTable[rows[2] + 1];
                for (int k = 1; k <= rows[2]; k++)
                {
                    TypeDefStruct[k].flags = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    TypeDefStruct[k].name = ReadStringIndex(metadata, offs);
                    typedefnames[k] = GetString(TypeDefStruct[k].name);
                    offs += offsetstring;
                    TypeDefStruct[k].nspace = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    TypeDefStruct[k].cindex = ReadCodedIndex(metadata, offs, "TypeDefOrRef");
                    offs += GetCodedIndexSize("TypeDefOrRef");
                    TypeDefStruct[k].findex = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                    TypeDefStruct[k].mindex = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                }
            }
            //Field
            old = tableoffset;
            b = tablepresent(4);
            offs = tableoffset;
            Console.WriteLine("Field Table Offset {0} Size {1}", offs, sizes[4]);
            tableoffset = old;
            if (b)
            {
                FieldStruct = new FieldTable[rows[4] + 1];
                for (int k = 1; k <= rows[4]; k++)
                {
                    FieldStruct[k].flags = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    FieldStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    FieldStruct[k].sig = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //Method
            old = tableoffset;
            b = tablepresent(6);
            offs = tableoffset;
            Console.WriteLine("Method Table Offset {0} Size {1}", offs, sizes[6]);
            tableoffset = old;
            if (b)
            {
                MethodStruct = new MethodTable[rows[6] + 1];
                for (int k = 1; k <= rows[6]; k++)
                {
                    MethodStruct[k].rva = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    MethodStruct[k].impflags = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    MethodStruct[k].flags = (int)BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    MethodStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    MethodStruct[k].signature = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                    MethodStruct[k].param = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                }
            }
            //Param
            old = tableoffset;
            b = tablepresent(8);
            offs = tableoffset;
            Console.WriteLine("Param Table Offset {0} Size {1}", offs, sizes[8]);
            tableoffset = old;
            if (b)
            {
                ParamStruct = new ParamTable[rows[8] + 1];
                for (int k = 1; k <= rows[8]; k++)
                {
                    ParamStruct[k].pattr = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    ParamStruct[k].sequence = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    ParamStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                }
            }
            //InterfaceImpl
            old = tableoffset;
            b = tablepresent(9);
            offs = tableoffset;
            Console.WriteLine("InterfaceImpl Table Offset {0} Size {1}", offs, sizes[9]);
            tableoffset = old;
            if (b)
            {
                InterfaceImplStruct = new InterfaceImplTable[rows[9] + 1];
                for (int k = 1; k <= rows[9]; k++)
                {
                    InterfaceImplStruct[k].classindex = ReadCodedIndex(metadata, offs, "TypeDefOrRef");
                    offs += GetCodedIndexSize("TypeDefOrRef");
                    InterfaceImplStruct[k].interfaceindex = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                }
            }
            //MemberRef
            old = tableoffset;
            b = tablepresent(10);
            offs = tableoffset;
            Console.WriteLine("MemberRef Table Offset {0} Size {1}", offs, sizes[10]);
            tableoffset = old;
            if (b)
            {
                MemberRefStruct = new MemberRefTable[rows[10] + 1];
                for (int k = 1; k <= rows[10]; k++)
                {
                    MemberRefStruct[k].clas = ReadCodedIndex(metadata, offs, "MemberRefParent");
                    offs += GetCodedIndexSize("MemberRefParent");
                    MemberRefStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    MemberRefStruct[k].sig = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //Constants 
            old = tableoffset;
            b = tablepresent(11);
            offs = tableoffset;
            Console.WriteLine("Constant Table Offset {0} Size {1}", offs, sizes[11]);
            tableoffset = old;
            if (b)
            {
                ConstantsStruct = new ConstantsTable[rows[11] + 1];
                for (int k = 1; k <= rows[11]; k++)
                {
                    ConstantsStruct[k].dtype = metadata[offs];
                    offs += 2;
                    ConstantsStruct[k].parent = ReadCodedIndex(metadata, offs, "HasConst");
                    offs += GetCodedIndexSize("HasConst");
                    ConstantsStruct[k].value = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //CustomAttribute
            old = tableoffset;
            b = tablepresent(12);
            offs = tableoffset;
            Console.WriteLine("CustomAttribute Table Offset {0} Size {1}", offs, sizes[12]);
            tableoffset = old;
            if (b)
            {
                CustomAttributeStruct = new CustomAttributeTable[rows[12] + 1];
                for (int k = 1; k <= rows[12]; k++)
                {
                    CustomAttributeStruct[k].parent = ReadCodedIndex(metadata, offs, "HasCustomAttribute");
                    offs += GetCodedIndexSize("HasCustomAttribute");
                    CustomAttributeStruct[k].type = ReadCodedIndex(metadata, offs, "HasCustomAttributeType");
                    offs += GetCodedIndexSize("HasCustomAttributeType");
                    CustomAttributeStruct[k].value = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //FieldMarshal
            old = tableoffset;
            b = tablepresent(13);
            offs = tableoffset;
            Console.WriteLine("FieldMarshal Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                FieldMarshalStruct = new FieldMarshalTable[rows[13] + 1];
                for (int k = 1; k <= rows[13]; k++)
                {
                    FieldMarshalStruct[k].coded = ReadCodedIndex(metadata, offs, "HasFieldMarshal");
                    offs += GetCodedIndexSize("HasFieldMarshal");
                    FieldMarshalStruct[k].index = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //DeclSecurity
            old = tableoffset;
            b = tablepresent(14);
            offs = tableoffset;
            Console.WriteLine("DeclSecurity Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                DeclSecurityStruct = new DeclSecurityTable[rows[14] + 1];
                for (int k = 1; k <= rows[14]; k++)
                {
                    DeclSecurityStruct[k].action = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    DeclSecurityStruct[k].coded = ReadCodedIndex(metadata, offs, "HasDeclSecurity");
                    offs += GetCodedIndexSize("HasDeclSecurity");
                    DeclSecurityStruct[k].bindex = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //ClassLayout
            old = tableoffset;
            b = tablepresent(15);
            offs = tableoffset;
            Console.WriteLine("ClassLayout Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                ClassLayoutStruct = new ClassLayoutTable[rows[15] + 1];
                for (int k = 1; k <= rows[15]; k++)
                {
                    ClassLayoutStruct[k].packingsize = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    ClassLayoutStruct[k].classsize = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    ClassLayoutStruct[k].parent = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                }
            }
            //FieldLayout
            old = tableoffset;
            b = tablepresent(16);
            offs = tableoffset;
            Console.WriteLine("FieldLayout Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                FieldLayoutStruct = new FieldLayoutTable[rows[16] + 1];
                for (int k = 1; k <= rows[16]; k++)
                {
                    FieldLayoutStruct[k].offset = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    FieldLayoutStruct[k].fieldindex = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                }
            }
            //StandAloneSig
            old = tableoffset;
            b = tablepresent(17);
            offs = tableoffset;
            Console.WriteLine("StandAloneSig Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                StandAloneSigStruct = new StandAloneSigTable[rows[17] + 1];
                for (int k = 1; k <= rows[17]; k++)
                {
                    StandAloneSigStruct[k].index = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //EventMap
            old = tableoffset;
            b = tablepresent(18);
            offs = tableoffset;
            Console.WriteLine("EventMap Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                EventMapStruct = new EventMapTable[rows[18] + 1];
                for (int k = 1; k <= rows[18]; k++)
                {
                    EventMapStruct[k].index = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                    EventMapStruct[k].eindex = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                }
            }
            //Event
            old = tableoffset;
            b = tablepresent(20);
            offs = tableoffset;
            Console.WriteLine("Event Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                EventStruct = new EventTable[rows[20] + 1];
                for (int k = 1; k <= rows[20]; k++)
                {
                    EventStruct[k].attr = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    EventStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    EventStruct[k].coded = ReadCodedIndex(metadata, offs, "TypeDefOrRef");
                    offs += GetCodedIndexSize("TypeDefOrRef");
                }
            }
            //PropertyMap
            old = tableoffset;
            b = tablepresent(21);
            offs = tableoffset;
            Console.WriteLine("PropertyMap Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                PropertyMapStruct = new PropertyMapTable[rows[21] + 1];
                for (int k = 1; k <= rows[21]; k++)
                {
                    PropertyMapStruct[k].parent = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                    PropertyMapStruct[k].propertylist = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                }
            }
            //Property
            old = tableoffset;
            b = tablepresent(23);
            offs = tableoffset;
            Console.WriteLine("Property Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                PropertyStruct = new PropertyTable[rows[23] + 1];
                for (int k = 1; k <= rows[23]; k++)
                {
                    PropertyStruct[k].flags = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    PropertyStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    PropertyStruct[k].type = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //MethodSemantics
            old = tableoffset;
            b = tablepresent(24);
            offs = tableoffset;
            Console.WriteLine("MethodSemantics Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                MethodSemanticsStruct = new MethodSemanticsTable[rows[24] + 1];
                for (int k = 1; k <= rows[24]; k++)
                {
                    MethodSemanticsStruct[k].methodsemanticsattributes = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    MethodSemanticsStruct[k].methodindex = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                    MethodSemanticsStruct[k].association = ReadCodedIndex(metadata, offs, "HasSemantics");
                    offs += GetCodedIndexSize("HasSemantics");
                }
            }
            //MethodImpl
            old = tableoffset;
            b = tablepresent(25);
            offs = tableoffset;
            Console.WriteLine("MethodImpl Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                MethodImpStruct = new MethodImpTable[rows[25] + 1];
                for (int k = 1; k <= rows[25]; k++)
                {
                    MethodImpStruct[k].classindex = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                    MethodImpStruct[k].codedbody = ReadCodedIndex(metadata, offs, "MethodDefOrRef");
                    offs += GetCodedIndexSize("MethodDefOrRef");
                    MethodImpStruct[k].codeddef = ReadCodedIndex(metadata, offs, "MethodDefOrRef");
                    offs += GetCodedIndexSize("MethodDefOrRef");
                }
            }
            //ModuleRef
            old = tableoffset;
            b = tablepresent(26);
            offs = tableoffset;
            Console.WriteLine("ModuleRef Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                ModuleRefStruct = new ModuleRefTable[rows[26] + 1];
                for (int k = 1; k <= rows[26]; k++)
                {
                    ModuleRefStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                }
            }
            //TypeSpec
            old = tableoffset;
            b = tablepresent(27);
            offs = tableoffset;
            Console.WriteLine("TypeSpec Table Offset {0} size={1}", offs, rows[27]);
            tableoffset = old;
            if (b)
            {
                TypeSpecStruct = new TypeSpecTable[rows[27] + 1];
                for (int k = 1; k <= rows[27]; k++)
                {
                    TypeSpecStruct[k].signature = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //ImplMap
            old = tableoffset;
            b = tablepresent(28);
            offs = tableoffset;
            Console.WriteLine("ImplMap Table Offset offs={0} rows={1} len={2}", offs, rows[28], metadata.Length);
            tableoffset = old;
            if (b)
            {
                ImplMapStruct = new ImplMapTable[rows[28] + 1];
                for (int k = 1; k <= rows[28]; k++)
                {
                    ImplMapStruct[k].attr = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    ImplMapStruct[k].cindex = ReadCodedIndex(metadata, offs, "MemberForwarded");
                    offs += GetCodedIndexSize("MemberForwarded");
                    ImplMapStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    ImplMapStruct[k].scope = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                }
            }
            //FieldRVA
            old = tableoffset;
            b = tablepresent(29);
            offs = tableoffset;
            Console.WriteLine("FieldRVA Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                FieldRVAStruct = new FieldRVATable[rows[29] + 1];
                for (int k = 1; k <= rows[29]; k++)
                {
                    FieldRVAStruct[k].rva = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    FieldRVAStruct[k].fieldi = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                }
            }
            //Assembly
            old = tableoffset;
            b = tablepresent(32);
            offs = tableoffset;
            Console.WriteLine("Assembly Table Offset {0}", offs);
            tableoffset = old;
            AssemblyStruct = new AssemblyTable[rows[32] + 1];
            if (b)
            {
                for (int k = 1; k <= rows[32]; k++)
                {
                    AssemblyStruct[k].HashAlgId = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    AssemblyStruct[k].major = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    AssemblyStruct[k].minor = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    AssemblyStruct[k].build = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    AssemblyStruct[k].revision = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    AssemblyStruct[k].flags = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    AssemblyStruct[k].publickey = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                    AssemblyStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    AssemblyStruct[k].culture = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                }
            }
            //AssemblyRef
            old = tableoffset;
            b = tablepresent(35);
            offs = tableoffset;
            Console.WriteLine("AssembleyRef Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                AssemblyRefStruct = new AssemblyRefTable[rows[35] + 1];
                for (int k = 1; k <= rows[35]; k++)
                {
                    AssemblyRefStruct[k].major = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    AssemblyRefStruct[k].minor = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    AssemblyRefStruct[k].build = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    AssemblyRefStruct[k].revision = BitConverter.ToInt16(metadata, offs);
                    offs += 2;
                    AssemblyRefStruct[k].flags = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    AssemblyRefStruct[k].publickey = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                    AssemblyRefStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    AssemblyRefStruct[k].culture = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    AssemblyRefStruct[k].hashvalue = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //File
            old = tableoffset;
            b = tablepresent(38);
            offs = tableoffset;
            Console.WriteLine("File Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                FileStruct = new FileTable[rows[38] + 1];
                for (int k = 1; k <= rows[38]; k++)
                {
                    FileStruct[k].flags = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    FileStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    FileStruct[k].index = ReadBlobIndex(metadata, offs);
                    offs += offsetblob;
                }
            }
            //ExportedType
            old = tableoffset;
            b = tablepresent(39);
            offs = tableoffset;
            Console.WriteLine("ExportedType Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                ExportedTypeStruct = new ExportedTypeTable[rows[39] + 1];
                for (int k = 1; k <= rows[39]; k++)
                {
                    ExportedTypeStruct[k].flags = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    ExportedTypeStruct[k].typedefindex = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    ExportedTypeStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    ExportedTypeStruct[k].nspace = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    ExportedTypeStruct[k].coded = ReadCodedIndex(metadata, offs, "Implementation");
                    offs += GetCodedIndexSize("Implementation");
                }
            }
            //ManifestResource
            old = tableoffset;
            b = tablepresent(40);
            offs = tableoffset;
            Console.WriteLine("ManifestResource Table Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                ManifestResourceStruct = new ManifestResourceTable[rows[40] + 1];
                for (int k = 1; k <= rows[40]; k++)
                {
                    ManifestResourceStruct[k].offset = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    ManifestResourceStruct[k].flags = BitConverter.ToInt32(metadata, offs);
                    offs += 4;
                    ManifestResourceStruct[k].name = ReadStringIndex(metadata, offs);
                    offs += offsetstring;
                    ManifestResourceStruct[k].coded = ReadCodedIndex(metadata, offs, "Implementation");
                    offs += GetCodedIndexSize("");
                }
            }
            //Nested Classes
            old = tableoffset;
            b = tablepresent(41);
            offs = tableoffset;
            Console.WriteLine("Nested Classes Offset {0}", offs);
            tableoffset = old;
            if (b)
            {
                NestedClassStruct = new NestedClassTable[rows[41] + 1];
                for (int k = 1; k <= rows[41]; k++)
                {
                    NestedClassStruct[k].nestedclass = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                    NestedClassStruct[k].enclosingclass = ReadTableIndex(metadata, offs);
                    offs += GetTableSize();
                }
            }
        }
        public long ConvertRVA(long rva)
        {
            int i;
            for (i = 0; i < sections; i++)
            {
                if (rva >= SVirtualAddress[i] && (rva < SVirtualAddress[i] + SSizeOfRawData[i]))
                    break;
            }
            if (i >= SPointerToRawData.Length)
                return -1;
            return SPointerToRawData[i] + (rva - SVirtualAddress[i]);
        }
        public void CLRHeader()
        {
            Console.WriteLine("// CLR Header:");
            s.Position = ConvertRVA(datadirectoryrva[14]);
            int size = r.ReadInt32();
            int majorruntimeversion = r.ReadInt16();
            int minorruntimeversion = r.ReadInt16();
            metadatarva = r.ReadInt32();
            int metadatasize = r.ReadInt32();
            corflags = r.ReadInt32();
            entrypointtoken = r.ReadInt32();
            int resourcesrva = r.ReadInt32();
            int resourcessize = r.ReadInt32();
            int strongnamesigrva = r.ReadInt32();
            int strongnamesigsize = r.ReadInt32();
            int codemanagerrva = r.ReadInt32();
            int codemanagersize = r.ReadInt32();
            vtablerva = r.ReadInt32();
            vtablesize = r.ReadInt32();
            exportaddressrva = r.ReadInt32();
            exportaddresssize = r.ReadInt32();
            int managednativeheaderrva = r.ReadInt32();
            int managednativeheadersize = r.ReadInt32();
            Console.WriteLine("// {0} Header Size", size);
            Console.WriteLine("// {0} Major Runtime Version", majorruntimeversion);
            Console.WriteLine("// {0} Minor Runtime Version", minorruntimeversion);
            Console.WriteLine("// {0} Flags", corflags);
            string dummy = "// " + entrypointtoken.ToString("x");
            dummy = dummy.PadRight(12) + "Entrypoint Token";
            Console.WriteLine(dummy);
            DisplayDataDirectory(metadatarva, metadatasize, "Metadata Directory");
            DisplayDataDirectory(resourcesrva, resourcessize, "Resources Directory");
            DisplayDataDirectory(strongnamesigrva, strongnamesigsize, "Strong Name Signature");
            DisplayDataDirectory(codemanagerrva, codemanagersize, "CodeManager Table");
            DisplayDataDirectory(vtablerva, vtablesize, "VTableFixups Directory");
            DisplayDataDirectory(exportaddressrva, exportaddresssize, "Export Address Table");
            DisplayDataDirectory(managednativeheaderrva, managednativeheadersize, "Precompile Header");
            Console.WriteLine("// Code Manager Table:");
            if (codemanagerrva == 0)
                Console.WriteLine("// default");
        }
        public void DisplayStrings()
        {
       //     Console.WriteLine("\n Strings Stream\n");
            for (int k = 0; k < ssize[1]; k++)
            {
         //       Console.Write("{0}", (char)strings[k]);
                if (strings[k] == 0)
                    Console.WriteLine("");
            }
        }
        public void DisplayUS()
        {
            Console.WriteLine("\n US Stream\n");
            for (int k = 0; k < ssize[2]; k += 2)
            {
                Console.Write("{0}", (char)us[k]);
                if (us[k] == 0)
                    Console.WriteLine("");
            }
        }
        public void DisplayGuid()
        {
            int st = 1;
            Console.WriteLine("\n GUID Stream\n");
            Console.Write("{");
            Console.Write("{0}{1}{2}{3}", guid[st + 2].ToString("X"), guid[st + 1].ToString("X"), guid[st].ToString("X"), guid[st - 1].ToString("X"));
            Console.Write("-{0}{1}-", guid[st + 3].ToString("X"), guid[st + 4].ToString("X"));
            Console.Write("{0}{1}-", guid[st + 6].ToString("X"), guid[st + 5].ToString("X"));
            Console.Write("{0}{1}-", guid[st + 7].ToString("X"), guid[st + 8].ToString("X"));
            Console.Write("{0}{1}{2}{3}{4}{5}", guid[st + 9].ToString("X"), guid[st + 10].ToString("X"), guid[st + 11].ToString("X"), guid[st + 12].ToString("X"), guid[st + 13].ToString("X"), guid[st + 14].ToString("X"));
            Console.Write("}\n");
        }
        public void ReadStreamsData()
        {
            startofmetadata = ConvertRVA(metadatarva);
            Console.WriteLine("\nMetadata Details\n");
            Console.WriteLine("Start of Metadata {0} rva={1}", metadatarva, startofmetadata);
            s.Position = startofmetadata;
            s.Seek(4 + 2 + 2 + 4, SeekOrigin.Current);
            int lengthofstring = r.ReadInt32();
            Console.WriteLine("Length of String {0}", lengthofstring);
            s.Seek(lengthofstring, SeekOrigin.Current);
            long padding = s.Position % 4;
            padding = 4 - padding;
            s.Seek(2, SeekOrigin.Current);
            int streams = r.ReadInt16();
            Console.WriteLine("No of streams {0} Position={1}", streams, s.Position);
            streamnames = new string[5];
            offset = new int[5];
            ssize = new int[5];
            names = new byte[5][];
            names[0] = new byte[10];
            names[1] = new byte[10];
            names[2] = new byte[10];
            names[3] = new byte[10];
            names[4] = new byte[10];
            int j;
            Console.WriteLine("\n Stream Details\n");
            for (int i = 0; i < streams; i++)
            {
                offset[i] = r.ReadInt32();
                ssize[i] = r.ReadInt32();
                Console.WriteLine("offset={0} size={1} Position={2}", offset[i], ssize[i], s.Position);
                j = 0;
                byte bb;
                while (true)
                {
                    bb = r.ReadByte();
                    if (bb == 0)
                        break;
                    names[i][j] = bb;
                    j++;
                }
                names[i][j] = bb;
                streamnames[i] = GetStreamNames(names[i]);
                while (true)
                {
                    if (s.Position % 4 == 0)
                        break;
                    byte b = r.ReadByte();
                    if (b != 0)
                    {
                        s.Seek(-1, SeekOrigin.Current);
                        break;
                    }
                }
            }
            for (int i = 0; i < streams; i++)
            {
                if (streamnames[i] == "#~")
                {
                    metadata = new byte[ssize[i]];
                    s.Seek(startofmetadata + offset[i], SeekOrigin.Begin);
                    for (int k = 0; k < ssize[i]; k++)
                        metadata[k] = r.ReadByte();
                }
                if (streamnames[i] == "#Strings")
                {
                    strings = new byte[ssize[i]];
                    s.Seek(startofmetadata + offset[i], SeekOrigin.Begin);
                    for (int k = 0; k < ssize[i]; k++)
                        strings[k] = r.ReadByte();
          //          DisplayStrings();
                }
                if (streamnames[i] == "#US")
                {
                    us = new byte[ssize[i]];
                    s.Seek(startofmetadata + offset[i], SeekOrigin.Begin);
                    for (int k = 0; k < ssize[i]; k++)
                        us[k] = r.ReadByte();
//                    DisplayUS();
                }
                if (streamnames[i] == "#GUID")
                {
                    guid = new byte[ssize[i]];
                    s.Seek(startofmetadata + offset[i], SeekOrigin.Begin);
                    for (int k = 0; k < ssize[i]; k++)
                        guid[k] = r.ReadByte();
          //          DisplayGuid();
                }
                if (streamnames[i] == "#Blob")
                {
                    blob = new byte[ssize[i]];
                    s.Seek(startofmetadata + offset[i], SeekOrigin.Begin);
                    for (int k = 0; k < ssize[i]; k++)
                        blob[k] = r.ReadByte();
                }
            }
            Console.WriteLine("\n Stream offset and stream size\n");
            for (int i = 0; i < streams; i++)
            {
                Console.WriteLine("{0}...offset: {1} size:{2}", streamnames[i], offset[i], ssize[i]);
            }
            int heapsizes = metadata[6];
            if ((heapsizes & 0x01) == 0x01)
                offsetstring = 4;
            if ((heapsizes & 0x02) == 0x02)
                offsetguid = 4;
            if ((heapsizes & 0x04) == 0x04)
                offsetblob = 4;
            valid = BitConverter.ToInt64(metadata, 8);
            tableoffset = 24;
            rows = new int[64];
            Array.Clear(rows, 0, rows.Length);
            for (int k = 0; k <= 63; k++)
            {
                int tablepresent = (int)(valid >> k) & 1;
                if (tablepresent == 1)
                {
                    rows[k] = BitConverter.ToInt32(metadata, tableoffset);
                    tableoffset += 4;
                }
            }
            Console.WriteLine("\nNumber of Rows in the tables: \n");
            for (int k = 62; k >= 0; k--)
            {
                int tablepresent = (int)(valid >> k) & 1;
                if (tablepresent == 1)
                {
                    Console.WriteLine("{0} : {1}", tablenames[k], rows[k]);
                }
            }
        }
        public string GetStreamNames(byte[] b)
        {
            int i = 0;
            while (b[i] != 0)
            {
                i++;
            }
            System.Text.Encoding e = System.Text.Encoding.UTF8;
            string s = e.GetString(b, 0, i);
            return s;
        }
        public int GetTableSize()
        {
            return 2;
        }
        public int ReadStringIndex(byte[] a, int o)
        {
            int z = 0;
            if (offsetstring == 2)
                z = BitConverter.ToUInt16(a, o);
            if (offsetstring == 4)
                z = (int)BitConverter.ToUInt32(a, o);
            return z;
        }
        public int ReadBlobIndex(byte[] a, int o)
        {
            int z = 0;
            if (offsetblob == 2)
                z = BitConverter.ToUInt16(a, o);
            if (offsetblob == 4)
                z = (int)BitConverter.ToUInt32(a, o);
            return z;
        }
        public int ReadGuidIndex(byte[] a, int o)
        {
            int z = 0;
            if (offsetguid == 2)
                z = BitConverter.ToUInt16(a, o);
            if (offsetguid == 4)
                z = (int)BitConverter.ToUInt32(a, o);
            return z;
        }
        public int ReadTableIndex(byte[] a, int o)
        {
            int z = 0;
            int z1 = GetTableSize();
            if (z1 == 2)
                z = BitConverter.ToUInt16(a, o);
            if (z1 == 4)
                z = (int)BitConverter.ToUInt32(a, o);
            return z;
        }
        public void DisplayAllMethods(int i)
        {
            int start, startofnext = 0;
            start = TypeDefStruct[i].mindex;
            if (i == (TypeDefStruct.Length - 1))
            {
                startofnext = MethodStruct.Length;
            }
            else
                startofnext = TypeDefStruct[i + 1].mindex;
            Console.WriteLine("Number Of Methods {0}", startofnext - start);
            for (int j = start; j < startofnext; j++)
            {
                Console.WriteLine("{0}. {1}", j, GetString(MethodStruct[j].name));
            }
        }
        public void DisplayAllFields(int i)
        {
            if (FieldStruct == null)
                return;
            int start, startofnext = 0;
            start = TypeDefStruct[i].findex;
            if (i == (TypeDefStruct.Length - 1))
            {
                startofnext = FieldStruct.Length;
            }
            else
                startofnext = TypeDefStruct[i + 1].findex;
            Console.WriteLine("Number Of Fields {0}", startofnext - start);
            for (int j = start; j < startofnext; j++)
            {
                Console.WriteLine("{0}. {1}", j, GetString(FieldStruct[j].name));
            }
        }
        public void DisplayAllParams(int i)
        {
            if (ParamStruct == null)
                return;
            int start, startofnext = 0;
            start = MethodStruct[i].param;
            if (i == (MethodStruct.Length - 1))
            {
                startofnext = ParamStruct.Length;
            }
            else
                startofnext = MethodStruct[i + 1].param;
            Console.WriteLine("Number Of Params {0}", startofnext - start);
            for (int j = start; j < startofnext; j++)
            {
                Console.WriteLine("{0}. {1}", j, GetString(ParamStruct[j].name));
            }
        }
        public void DisplayAllEvents(int i)
        {
            int ii;
            if (EventMapStruct == null)
                return;
            for (ii = 1; ii < EventMapStruct.Length; ii++)
            {
                if (i == (EventMapStruct[ii].index))
                    break;
            }
            if (ii == EventMapStruct.Length)
                return;
            int start = EventMapStruct[ii].eindex;
            int end;
            if (ii == EventMapStruct.Length - 1)
                end = EventStruct.Length - 1;
            else
                end = EventMapStruct[ii + 1].eindex - 1;
            Console.WriteLine("Number of Events {0}", end - start + 1);
            for (int jj = start; jj <= end; jj++)
            {
                Console.WriteLine("{0}. {1}", jj, GetString(EventStruct[jj].name));
            }
        }
        public void DisplayAllProperties(int i)
        {
            int ii;
            if (PropertyMapStruct == null || PropertyMapStruct.Length == 1)
                return;
            for (ii = 1; ii < PropertyMapStruct.Length; ii++)
            {
                if (i == (PropertyMapStruct[ii].parent))
                    break;
            }
            if (ii == PropertyMapStruct.Length)
                return;
            int start = PropertyMapStruct[ii].propertylist;
            int end;
            if (ii + 1 == PropertyMapStruct.Length)
                end = PropertyStruct.Length - 1;
            else
                end = PropertyMapStruct[ii + 1].propertylist - 1;
            Console.WriteLine("Number of Properties {0}", end - start + 1);
            for (int jj = start; jj <= end; jj++)
            {
                Console.WriteLine("{0}. {1}", jj, GetString(PropertyStruct[jj].name));
            }
        }
        public string GetString(int starting)
        {
            int i = starting;
            while (strings[i] != 0)
            {
                i++;
            }
            System.Text.Encoding e = System.Text.Encoding.UTF8;
            string s = e.GetString(strings, starting, i - starting);
            if (s.Length == 0)
                return "";
            else
                return s;
        }
        public int CorSigUncompressData(byte[] b, int index, out int answer)
        {
            int cb = 0;
            answer = 0;
            if ((b[index] & 0x80) == 0x00)
            {
                cb = 1;
                answer = b[index];
            }
            if ((b[index] & 0xC0) == 0x80)
            {
                cb = 2;
                answer = ((b[index] & 0x3f) << 8) | b[index + 1];
            }
            if ((b[index] & 0xE0) == 0xC0)
            {
                cb = 3;
                answer = ((b[index] & 0x1f) << 24) | (b[index + 1] << 16) | (b[index + 2] << 8) | b[index + 3];
            }
            return cb;
        }
        public string GetType(int b)
        {
            if (b == 0x01)
                return "void";
            if (b == 0x02)
                return "bool";
            if (b == 0x03)
                return "char";
            if (b == 0x04)
                return "int8";
            if (b == 0x05)
                return "unsigned int8";
            if (b == 0x06)
                return "int16";
            if (b == 0x07)
                return "unsigned int16";
            if (b == 0x08)
                return "int32";
            if (b == 0x09)
                return "unsigned int32";
            if (b == 0x0a)
                return "int64";
            if (b == 0x0b)
                return "unsigned int64";
            if (b == 0x0c)
                return "float32";
            if (b == 0x0d)
                return "float64";
            if (b == 0x0e)
                return "string";
            if (b == 0x0f)
                return "pointer";
            if (b == 0x10)
                return "referencetype";
            if (b == 0x11)
                return "valuetype";
            if (b == 0x12)
                return "class";
            if (b == 0x14)
                return "array";
            if (b == 0x16)
                return "typed byref";
            if (b == 0x18)
                return "intptr";
            if (b == 0x19)
                return "uintptr";
            if (b == 0x1b)
                return "function ptr";
            if (b == 0x1c)
                return "object";
            if (b == 0x1d)
                return "sd array";
            if (b == 0x1f)
                return "reqd mod";
            if (b == 0x20)
                return "opt mod";
            if (b == 0x21)
                return "internal";
            if (b == 0x40)
                return "typed mod";
            if (b == 0x41)
                return "sentinel";
            if (b == 0x45)
                return "pinned";
            return "user defined...unknown";
        }
        public string GetCustomAttributeTypeTable(int a)
        {
            string s = "";
            int tag = a & 0x07;
            if (tag == 0)
                s = s + "NotUsed";
            if (tag == 1)
                s = s + "NotUsed";
            if (tag == 2)
                s = s + "MethodDef";
            if (tag == 3)
                s = s + "MethodRef";
            if (tag == 4)
                s = s + "NotUsed";
            return s;
        }
        public int GetResolutionScopeValue(int a)
        {
            return a >> 2;
        }
        public string GetResolutionScopeTable(int a)
        {
            string s = "";
            int tag = a & 0x03;
            if (tag == 0)
                s = s + "Module";
            if (tag == 1)
                s = s + "ModuleRef";
            if (tag == 2)
                s = s + "AssemblyRef";
            if (tag == 3)
                s = s + "TypeRef";
            return s;
        }
        public void DisplayModuleTable()
        {
            if (ModuleStruct != null)
            {
                Console.WriteLine("\nModule Table\n");
                for (int ii = 1; ii <= ModuleStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Generation: {0}", ModuleStruct[ii].Generation);
                    Console.WriteLine("Name :{0} {1}", GetString(ModuleStruct[ii].Name), ModuleStruct[ii].Name.ToString("X"));
                    Console.WriteLine("Mvid :#GUID[{0}]", ModuleStruct[ii].Mvid);
                    Console.WriteLine("EncId :#GUID[{0}]", ModuleStruct[ii].EncId);
                    Console.WriteLine("EncBaseId :#GUID[{0}]", ModuleStruct[ii].EncBaseId);
                    Console.WriteLine("");
                }
            }
        }
        public void DisplayTypeRefTable()
        {
            if (TypeRefStruct != null)
            {
                Console.WriteLine("\nTypeRef Table\n");
                for (int ii = 1; ii <= TypeRefStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row[{0}]", ii);
                    string tablename = GetResolutionScopeTable(TypeRefStruct[ii].resolutionscope);
                    int index = GetResolutionScopeValue(TypeRefStruct[ii].resolutionscope);
                    string s = DisplayTable(tablename, index);
                    Console.WriteLine("ResolutionScope:{0}[{1}]...{2} ", tablename, index, s);
                    Console.WriteLine("Name :{0}", GetString(TypeRefStruct[ii].name));
                    Console.WriteLine("Namespace :{0}", GetString(TypeRefStruct[ii].nspace));
                    Console.WriteLine("\n");
                }
            }
        }
        public void DisplayTypeDefTable()
        {
            if (TypeDefStruct != null)
            {
                Console.WriteLine("TypeDefTable\n");
                for (int ii = 1; ii <= TypeDefStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row:{0}", ii);
                    CorTypeAttr flags = (CorTypeAttr)TypeDefStruct[ii].flags;
                    Console.WriteLine("Flags : {0}", flags);
                    Console.WriteLine("Name : {0}", GetString(TypeDefStruct[ii].name));
                    Console.WriteLine("NameSpace : {0}", GetString(TypeDefStruct[ii].nspace));
                    string tablename = GetTypeDefOrRefTable(TypeDefStruct[ii].cindex);
                    int index = GetTypeDefOrRefValue(TypeDefStruct[ii].cindex);
                    string s = DisplayTable(tablename, index);
                    Console.WriteLine("Extends: {0}[{1}].....{2}", tablename, index, s);
                    Console.WriteLine("FieldList Field[{0}]", TypeDefStruct[ii].findex);
                    Console.WriteLine("MethodList Method[{0}]", TypeDefStruct[ii].mindex);
                    DisplayAllMethods(ii);
                    DisplayAllFields(ii);
                    DisplayAllEvents(ii);
                    DisplayAllProperties(ii);
                    Console.WriteLine("");
                }
            }
        }
        public void DisplayNestedClassTable()
        {
            if (NestedClassStruct != null)
            {
                Console.WriteLine("\nNested Classes Table\n");
                for (int ii = 1; ii <= NestedClassStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Nested Class TypeDef[{0}]...{1}", NestedClassStruct[ii].nestedclass, GetTypeDefTable(NestedClassStruct[ii].nestedclass));
                    Console.WriteLine("Enclosing Class TypeDef[{0}]...{1}", NestedClassStruct[ii].enclosingclass, GetTypeDefTable(NestedClassStruct[ii].enclosingclass));
                }
                Console.WriteLine("");
            }
        }
        public string DisplayFieldSignature(int index)
        {
            string s = "";
            int count = blob[index];
            Console.WriteLine("count {0} index {1}", count, index);
            for (int l = 1; l <= count; l++)
                s = s + blob[index + l].ToString() + " ";
            if (blob[index + 1] == 0x06)
            {
                s = s + ".." + GetType(blob[index + 2]);
            }
            return s;
        }
        public void DisplayFieldTable()
        {
            if (FieldStruct != null)
            {
                Console.WriteLine("\nFieldTable\n");
                for (int ii = 1; ii <= FieldStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Flags: {0}", FieldStruct[ii].flags);
                    Console.WriteLine("Name : {0}", GetString(FieldStruct[ii].name));
                    string s = DisplayFieldSignature(FieldStruct[ii].sig);
                    Console.WriteLine("Signature BLOB[{0}]...{1}", FieldStruct[ii].sig, s);
                }
            }
        }
        public void DisplayMethodTable()
        {
            if (MethodStruct != null)
            {
                Console.WriteLine("\nMethod Table\n");
                for (int ii = 1; ii <= MethodStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row :{0}", ii);
                    Console.WriteLine("RVA :{0}", MethodStruct[ii].rva.ToString("X"));
                    CorMethodImpl impflags = (CorMethodImpl)MethodStruct[ii].impflags;
                    Console.WriteLine("ImpFlags :{0}", impflags);
                    Console.WriteLine("Flags :{0}", MethodStruct[ii].flags);
                    Console.WriteLine("Name : {0}", GetString(MethodStruct[ii].name));
                    Console.WriteLine("Signature: #Blob[{0}]", MethodStruct[ii].signature);
                    Console.WriteLine("ParamList: Param[{0}]", MethodStruct[ii].param);
                    DisplayAllParams(ii);
                    DisplayMethodSignature(MethodStruct[ii].signature, ii);
                }
            }
        }
        public string GetParamAttributes(short a)
        {
            if (a == 0x00)
                return "None";
            if ((a & 0x01) == 0x01)
                return "[In]";
            if ((a & 0x02) == 0x02)
                return "[Out]";
            if ((a & 0x04) == 0x04)
                return "[Optional]";
            if ((a & 0x1000) == 0x1000)
                return "[Default]";
            if ((a & 0x2000) == 0x2000)
                return "[Field Marshal]";
            if ((a & 0xcfe0) == 0xcfe0)
                return "[Field Marshall]";
            return "Unknown";
        }
        public void DisplayParamTable()
        {
            if (ParamStruct != null)
            {
                Console.WriteLine("");
                Console.WriteLine("\nParam Table\n");
                for (int ii = 1; ii <= ParamStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("ParamAttributes {0} Bytes {1}", GetParamAttributes(ParamStruct[ii].pattr), ParamStruct[ii].pattr.ToString("X"));
                    Console.WriteLine("Sequence {0}", ParamStruct[ii].sequence);
                    Console.WriteLine("Name {0}", GetString(ParamStruct[ii].name));
                    Console.WriteLine("\n");
                }
            }
        }
        public void DisplayInterfaceImplTable()
        {
            if (InterfaceImplStruct != null)
            {
                Console.WriteLine("InterfaceImpl Table");
                for (int ii = 1; ii <= InterfaceImplStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    int ind = InterfaceImplStruct[ii].classindex;
                    Console.WriteLine("Class TypeDef[{0}]...{1}", ind, GetTypeDefTable(ind));
                    string tablename = GetTypeDefOrRefTable(InterfaceImplStruct[ii].interfaceindex);
                    int index = GetTypeDefOrRefValue(InterfaceImplStruct[ii].interfaceindex);
                    Console.WriteLine("Interface {0}[{1}] {2}", tablename, index, DisplayTable(tablename, index));
                }
            }
        }
        public string GetCallingConvention(int uncompressedbyte)
        {
            int firstbyte = uncompressedbyte;
            byte firstfourbits = (byte)(firstbyte & 0x0f);
            string s = " Calling Convention ";
            if (firstfourbits == 0x00)
                s = s + " DEFAULT ";
            if (firstfourbits == 0x01)
                s = s + " C ";
            if (firstfourbits == 0x02)
                s = s + " STDCALL ";
            if (firstfourbits == 0x03)
                s = s + " THISCALL ";
            if (firstfourbits == 0x04)
                s = s + " FASTCALL ";
            if (firstfourbits == 0x05)
                s = s + " VARARG ";
            if ((firstbyte & 0x20) == 0x20)
                s = s + " HASTHIS ";
            if ((firstbyte & 0x40) == 0x40)
                s = s + " EXPLICIT ";
            s = s + "\n";
            return s;
        }
        public string GetValueType(byte[] b, int bytes, int index, out int cb1)
        {
            int uncompressedbyte;
            int cb = CorSigUncompressData(b, index + 1, out uncompressedbyte);
            byte table = (byte)(uncompressedbyte & 0x03);
            int ind = uncompressedbyte >> 2;
            string s1 = "";
            if (table == 1)
                s1 = typerefnames[ind];
            if (table == 0)
                s1 = typedefnames[ind];
            cb1 = cb;
            return s1;
        }
        public string GetClassType(byte[] b, int bytes, int index, out int cb1)
        {
            int uncompressedbyte;
            int cb = CorSigUncompressData(b, index + 1, out uncompressedbyte);
            Console.WriteLine("Token Count cb={0} uncompresedbyte={1}", cb, uncompressedbyte);
            byte table = (byte)(uncompressedbyte & 0x03);
            int ind = uncompressedbyte >> 2;
            Console.WriteLine("Token Table={0} index={1}", table, ind);
            string s1 = "";
            if (table == 1)
                s1 = typerefnames[ind];
            if (table == 0)
                s1 = typedefnames[ind];
            cb1 = cb;
            return s1;
        }
        public string GetElemType(int index, byte[] b, out int cb1)
        {
            cb1 = 0;
            string s = "";
            if (index == b.Length)
                return s;
            byte type = b[index];
            if (type >= 0x01 && type <= 0x0e)
            {
                s = GetType(type);
                cb1 = 1;
            }
            if (type == 0x0f)
            {
                int cb2;
                if (b[index + 1] <= 0x0e)
                {
                    s = GetPointerType(b, index + 1, out cb2);
                    cb1 = cb2 + 1;
                }
                else if (b[index + 1] == 0x11)
                {
                    s = "valuetype " + GetTokenType(b, index + 1, out cb2) + "*";
                    cb1 = cb2 + 2;
                }
                else if (b[index + 2] == 0x12)
                {
                    s = "class " + GetTokenType(b, index + 1, out cb2) + "*";
                    cb1 = cb2 + 2;
                }
                else
                {
                    s = GetTokenType(b, index + 1, out cb2) + "*";
                    cb1 = cb2 + 1;
                }
            }
            if (type == 0x10)
            {
                int cb2;
                if (b[index + 1] >= 0x00 && b[index + 1] <= 0x0e)
                {
                    s = GetType(b[index + 1]) + "&";
                    cb1 = 2;
                }
                else if (b[index + 1] == 0x1D)
                {
                    s = GetType(b[index + 1]) + "[]&";
                    cb1 = 2;
                }
                else
                {
                    s = GetTokenType(b, index, out cb2);
                    cb1 = cb2 + 1;
                }
            }
            if (type == 0x11 || type == 0x12)
            {
                int cb2;
                s = GetTokenType(b, index, out cb2);
                cb1 = cb2 + 1;
            }
            if (type == 0x14)
            {
                int cb2;
                s = GetArrayType(b, index, out cb2);
                cb1 = cb2 + 1;
            }
            if (type == 0x16)
            {
                s = "typedbyref";
                cb1 = 1;
            }
            if (type == 0x18)
            {
                s = "native int";
                cb1 = 1;
            }
            if (type == 0x19)
            {
                s = "native unsigned int";
                cb1 = 1;
            }
            if (type == 0x1c)
            {
                s = "object";
                cb1 = 1;
            }
            if (type == 0x1d)
            {
                int i = 1;
                s = "[]";
                while (true)
                {
                    byte next = b[index + i];
                    if (next != 0x1d)
                        break;
                    s = s + "[]";
                    i = i + 1;
                }
                int cb2;
                s = s + GetElemType(index + i, b, out cb2);
                cb1 = i + cb2;
            }
            if (type == 0x20)
            {
                int cb2;
                s = "modopt " + GetTokenType(b, index, out cb2);
                cb1 = cb2 + 1;
            }
            if (type == 0x45)
            {
                int cb2;
                s = GetElemType(index + 1, b, out cb2) + " pinned";
                cb1 = cb2 + 1;
            }
            return s;
        }
        public string GetTokenType(byte[] b, int index, out int cb1)
        {
            string s = "";
            int uncompressedbyte;
            int cb = 0;
            if (b[index] == 0x10 && b[index + 1] == 0x11)
            {
                index++;
                cb = 1;
            }
            if (b[index] == 0x10 && b[index + 1] == 0x12)
            {
                index++;
                cb = 1;
            }
            cb = cb + CorSigUncompressData(b, index + 1, out uncompressedbyte);
            string s1 = DecodeToken(uncompressedbyte);
            if (b[index] == 0x12)
            {
                s = "class " + s1;
            }
            else if (b[index] == 0x11)
            {
                s = "valuetype " + s1;
            }
            else if (b[index] == 0x10)
            {
                s = "[out] " + s1;
            }
            else
                s = s1;
            cb1 = cb;
            return s;
        }
        public string DecodeToken(int token)
        {
            byte table = (byte)(token & 0x03);
            int ind = token >> 2;
            string s1 = "";
            if (table == 0)
                s1 = GetString(TypeDefStruct[ind].name);
            if (table == 1)
                s1 = GetString(TypeRefStruct[ind].name);
            return s1;
        }
        public string GetPointerType(byte[] b, int index, out int cb1)
        {
            string s = "";
            s = GetElemType(index, b, out cb1) + " *";
            return s;
        }
        public string GetArrayType(byte[] b, int index, out int cb1)
        {
            string s;
            int total = 1;
            int uncompressedbyte;
            int rank;
            int numsizes;
            int cb;
            s = GetElemType(index + 1, b, out cb1);
            total = total + cb1;
            s = s + " [";
            cb = CorSigUncompressData(b, index + total, out uncompressedbyte);
            total = total + cb;
            rank = uncompressedbyte;
            cb = CorSigUncompressData(b, index + total, out uncompressedbyte);
            total = total + cb;
            numsizes = uncompressedbyte;
            int[] sizearray = new int[numsizes];
            for (int l = 1; l <= numsizes; l++)
            {
                cb = CorSigUncompressData(b, index + total, out uncompressedbyte);
                total = total + cb;
                sizearray[l - 1] = uncompressedbyte;
            }
            cb = CorSigUncompressData(b, index + total, out uncompressedbyte);
            total = total + cb;
            int bounds = uncompressedbyte;
            int[] boundsarray = new int[bounds];
            for (int l = 1; l <= bounds; l++)
            {
                cb = CorSigUncompressData(b, index + total, out uncompressedbyte);
                total = total + cb;
                int ulSigned = uncompressedbyte & 0x1;
                uncompressedbyte = uncompressedbyte >> 1;
                if (ulSigned == 1)
                {
                    if (cb == 1)
                    {
                        uncompressedbyte = (int)((ushort)uncompressedbyte | 0xffffffc0);
                    }
                    else if (cb == 2)
                    {
                        uncompressedbyte = (int)((ushort)uncompressedbyte | 0xffffe000);
                    }
                    else
                    {
                        uncompressedbyte = (int)((ushort)uncompressedbyte | 0xf0000000);
                    }
                }
                boundsarray[l - 1] = uncompressedbyte;
            }
            for (int l = 0; l < bounds; l++)
            {
                if (numsizes != 0)
                {
                    int upper = boundsarray[l] + sizearray[l] - 1;
                    if (boundsarray[l] == 0 && sizearray[l] != 0)
                    {
                        s = s + sizearray[l];
                        if (l != bounds - 1)
                            s = s + ",";
                    }
                    if (boundsarray[l] == 0 && sizearray[l] == 0)
                        s = s + ",";
                    if (boundsarray[l] != 0 && sizearray[l] != 0)
                    {
                        s = s + boundsarray[l] + "..." + upper.ToString();
                    }
                    if (!(bounds - l == 1))
                        s = s + ",";
                }
            }
            int leftover = rank - numsizes;
            if (numsizes == 0)
                leftover--;
            for (int l = 1; l <= leftover; l++)
                s = s + ",";
            s = s + "]";
            cb1 = total - 1;
            return s;
        }
        public string GetMemberRefParentTable(int a)
        {
            string s = "";
            int tag = a & 0x07;
            if (tag == 0)
                s = s + "NotUsed";
            if (tag == 1)
                s = s + "TypeRef";
            if (tag == 2)
                s = s + "ModuleRef";
            if (tag == 3)
                s = s + "MethodDef";
            if (tag == 4)
                s = s + "TypeSpec";
            return s;
        }
        public int GetMemberRefParentValue(int a)
        {
            return a >> 3;
        }
        public void DisplayMemberRefTable()
        {
            if (MemberRefStruct != null)
            {
                Console.WriteLine("\nMemberRef Table\n");
                for (int ii = 1; ii <= MemberRefStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    string s = GetMemberRefParentTable(MemberRefStruct[ii].clas);
                    int rid = (int)GetMemberRefParentValue(MemberRefStruct[ii].clas);
                    Console.WriteLine("{0}[{1}]:{2}", s, rid, DisplayTable(s, rid));
                    Console.WriteLine("Name:{0}", GetString(MemberRefStruct[ii].name));
                    Console.Write("Signature #BLOB[{0}] ", MemberRefStruct[ii].sig);
                    Console.WriteLine("\n");
                }
            }
        }
        public int GetHasConstValue(int a)
        {
            return a >> 2;
        }
        public string GetHasConstTable(int a)
        {
            string s = "";
            int tag = a & 0x03;
            Console.Write("Parent: ");
            if (tag == 0)
                s = s + "FieldDef";
            if (tag == 1)
                s = s + "ParamDef";
            if (tag == 2)
                s = s + "Property";
            return s;
        }
        public long GetBytesFromArray(int value, int dtype)
        {
            long z = 0;
            if (dtype == 2 || dtype == 4 || dtype == 5)
                z = blob[value + 1];
            if (dtype == 3 || dtype == 6 || dtype == 7)
                z = BitConverter.ToInt16(blob, value + 1);
            if (dtype == 8 || dtype == 9)
                z = BitConverter.ToInt32(blob, value + 1);
            if (dtype == 0x0a || dtype == 0x0b)
                z = BitConverter.ToInt64(blob, value + 1);
            return z;
        }
        public void GetStringFromBlobArray(int value)
        {
            int count = blob[value];
            Console.Write(" Value ");
            for (int i = 1; i <= count; i = i + 2)
                Console.Write("{0}", (char)blob[value + i]);
        }
        public void GetBlobConstant(int value, int dtype)
        {
            int count = blob[value];
            Console.Write("Blob BLOB[{0}] Count {1} ", value, count);
            for (int l = 1; l <= count; l++)
            {
                Console.Write("{0} ", blob[value + l].ToString("X"));
            }
            if (dtype <= 0x0b)
            {
                long val = GetBytesFromArray(value, dtype);
                Console.Write(" Value {0} ", val);
            }
            if (dtype == 0x0e)
                GetStringFromBlobArray(value);
            Console.WriteLine("");
        }
        public void DisplayConstantTable()
        {
            if (ConstantsStruct != null)
            {
                Console.WriteLine("\nConstant Table\n");
                for (int ii = 1; ii <= ConstantsStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Type {0}", GetType(ConstantsStruct[ii].dtype));
                    string s = GetHasConstTable(ConstantsStruct[ii].parent);
                    int off = GetHasConstValue(ConstantsStruct[ii].parent);
                    int p = 0;
                    if (s == "FieldDef")
                    {
                        p = FieldStruct[off].name;
                    }
                    if (s == "ParamDef")
                    {
                        p = ParamStruct[off].name;
                    }
                    if (s == "Property")
                    {
                        p = PropertyStruct[off].name;
                    }
                    Console.WriteLine("{0}[{1}] Name of Constant... {2} ", s, off, GetString(p));
                    GetBlobConstant(ConstantsStruct[ii].value, ConstantsStruct[ii].dtype);
                }
            }
        }
        public int GetCustomAttributeTypevalue(int a)
        {
            return a >> 3;
        }
        public int GetHasCustomAttributeValue(int a)
        {
            return a >> 5;
        }
        public string GetHasCustomAttributeTable(int a)
        {
            string s = "";
            int tag = a & 0x1F;
            if (tag == 0)
                s = s + "MethodDef";
            if (tag == 1)
                s = s + "FieldDef";
            if (tag == 2)
                s = s + "TypeRef";
            if (tag == 3)
                s = s + "TypeDef";
            if (tag == 4)
                s = s + "ParamDef";
            if (tag == 5)
                s = s + "InterfaceImpl";
            if (tag == 6)
                s = s + "MemberRef";
            if (tag == 7)
                s = s + "Module";
            if (tag == 8)
                s = s + "DeclSecurity";
            if (tag == 9)
                s = s + "Property";
            if (tag == 10)
                s = s + "Event";
            if (tag == 11)
                s = s + "Signature";
            if (tag == 12)
                s = s + "ModuleRef";
            if (tag == 13)
                s = s + "TypeSpec";
            if (tag == 14)
                s = s + "Assembly";
            if (tag == 15)
                s = s + "AssemblyRef";
            if (tag == 16)
                s = s + "File";
            if (tag == 17)
                s = s + "ExportedType";
            if (tag == 18)
                s = s + "ManifestResource";
            return s;
        }
        public void GetCustomAttributeBlob(int index)
        {
            int count = blob[index];
            Console.Write("Value Blob[{0}] Count {1} Bytes ", index, count);
            for (int l = 1; l <= count; l++)
            {
                Console.Write("{0} ", blob[index + l].ToString("X"));
            }
            Console.WriteLine("");
        }
        public void DisplayCustomAttributeTable()
        {
            if (CustomAttributeStruct != null)
            {
                Console.WriteLine("\n CustomAttribute Table\n");
                for (int ii = 1; ii <= CustomAttributeStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row [{0}]", ii);
                    string tablename = GetHasCustomAttributeTable(CustomAttributeStruct[ii].parent);
                    int index = GetHasCustomAttributeValue(CustomAttributeStruct[ii].parent);
                    string s = DisplayTable(tablename, index);
                    Console.WriteLine("Parent: {0}[{1}]...{2}", tablename, index, s);
                    tablename = GetCustomAttributeTypeTable(CustomAttributeStruct[ii].type);
                    index = GetCustomAttributeTypevalue(CustomAttributeStruct[ii].type);
                    s = DisplayTable(tablename, index);
                    Console.WriteLine("Type: {0}[{1}]...{2} ", tablename, index, s);
                    GetCustomAttributeBlob(CustomAttributeStruct[ii].value);
                }
            }
        }
        public string GetFieldMarshalTable(int a)
        {
            string s = "";
            short tag = (short)(a & (short)0x01);
            if (tag == 0)
                s = s + "Field";
            if (tag == 1)
                s = s + "Param";
            return s;
        }
        public int GetFieldMarshalValue(int a)
        {
            return a >> 1;
        }
        public string GetMarshallType(byte a)
        {
            if (a == 2)
                return "NATIVE_TYPE_BOOLEAN";
            if (a == 3)
                return "NATIVE_TYPE_I1";
            if (a == 0x04)
                return "NATIVE_TYPE_U1";
            if (a == 0x05)
                return "NATIVE_TYPE_I2";
            if (a == 0x06)
                return "NATIVE_TYPE_U2";
            if (a == 0x07)
                return "NATIVE_TYPE_I4";
            if (a == 0x08)
                return "NATIVE_TYPE_U4";
            if (a == 0x09)
                return "NATIVE_TYPE_I8";
            if (a == 0x0a)
                return "NATIVE_TYPE_U8";
            if (a == 0x0b)
                return "NATIVE_TYPE_R4";
            if (a == 0x0c)
                return "NATIVE_TYPE_R8";
            if (a == 0x14)
                return "NATIVE_TYPE_LPSTR";
            if (a == 0x1f)
                return "NATIVE_TYPE_INT";
            if (a == 0x20)
                return "NATIVE_TYPE_UINT";
            if (a == 0x26)
                return "NATIVE_TYPE_FUNC";
            if (a == 0x2a)
                return "NATIVE_TYPE_ARRAY";
            if (a == 0x0f)
                return "NATIVE_TYPE_CURRENCY";
            if (a == 0x13)
                return "NATIVE_TYPE_BSTR";
            if (a == 0x15)
                return "NATIVE_TYPE_LPWSTR";
            if (a == 0x16)
                return "NATIVE_TYPE_LPTSTR";
            if (a == 0x17)
                return "NATIVE_TYPE_FIXEDSYSSTRING";
            if (a == 0x19)
                return "NATIVE_TYPE_IUNKNOWN";
            if (a == 0x1a)
                return "NATIVE_TYPE_IDISPATCH";
            if (a == 0x1b)
                return "NATIVE_TYPE_STRUCT";
            if (a == 0x1c)
                return "NATIVE_TYPE_INTF";
            if (a == 0x1d)
                return "NATIVE_TYPE_SAFEARRAY";
            if (a == 0x1e)
                return "NATIVE_TYPE_FIXEDARRAY";
            if (a == 0x22)
                return "NATIVE_TYPE_BYVALSTR";
            if (a == 0x13)
                return "NATIVE_TYPE_ANSIBSTR";
            if (a == 0x24)
                return "NATIVE_TYPE_TBSTR";
            if (a == 0x25)
                return "NATIVE_TYPE_VARIANTBOOL";
            if (a == 0x28)
                return "NATIVE_TYPE_ASANY";
            if (a == 0x2b)
                return "NATIVE_TYPE_LPSTRUCT";
            if (a == 0x2c)
                return "NATIVE_TYPE_CUSTOMMARSHALER";
            if (a == 0x2d)
                return "NATIVE_TYPE_ERROR";
            if (a == 0x50)
                return "NATIVE_TYPE_NOINFO";
            return "Unknown";
        }
        public void DisplayFieldMarshalTable()
        {
            if (FieldMarshalStruct != null)
            {
                Console.WriteLine("\nFieldMarshal Table\n");
                for (int ii = 1; ii <= FieldMarshalStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    string tablename = GetFieldMarshalTable(FieldMarshalStruct[ii].coded);
                    int index = GetFieldMarshalValue(FieldMarshalStruct[ii].coded);
                    Console.WriteLine("Parent {0}[{1}]...{2}", tablename, index, DisplayTable(tablename, index));
                    int count = blob[FieldMarshalStruct[ii].index];
                    Console.WriteLine("Native Type Blob[{0}] Count {1} {2} ", index, count, GetMarshallType(blob[FieldMarshalStruct[ii].index + 1]));
                }
            }
        }
        public string GetDeclSecurityTable(int a)
        {
            string s = "";
            short tag = (short)(a & (short)0x03);
            if (tag == 0)
                s = s + "TypeDef";
            if (tag == 1)
                s = s + "MethodDef";
            if (tag == 2)
                s = s + "Assembly";
            return s;
        }
        public int GetDeclSecurityValue(int a)
        {
            return a >> 2;
        }
        public void DisplayDeclSecurityTable()
        {
            if (DeclSecurityStruct != null)
            {
                Console.WriteLine("\nDeclSecurity Table\n");
                for (int ii = 1; ii <= DeclSecurityStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Action {0}", DeclSecurityStruct[ii].action);
                    string tablename = GetDeclSecurityTable(DeclSecurityStruct[ii].coded);
                    int ind = GetDeclSecurityValue(DeclSecurityStruct[ii].coded);
                    Console.WriteLine("Parent: {0}[{1}]... {2}", tablename, ind, DisplayTable(tablename, ind));
                    int uncompressedbyte;
                    int cb = CorSigUncompressData(blob, DeclSecurityStruct[ii].bindex, out uncompressedbyte);
                    int count = uncompressedbyte;
                    int index1 = DeclSecurityStruct[ii].bindex + cb;
                    Console.WriteLine("Permission Set BLOB[{0}] Count={1} ", index1, count);
                    for (int l = 0; l < count; l++)
                        Console.Write("{0}", (char)blob[index1 + l]);
                }
                Console.WriteLine("");
            }
        }
        public void DisplayClassLayoutTable()
        {
            if (ClassLayoutStruct != null)
            {
                Console.WriteLine("\nClassLayout Table\n");
                for (int ii = 1; ii <= ClassLayoutStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Packing Size {0}", ClassLayoutStruct[ii].packingsize);
                    Console.WriteLine("Class Size {0}", ClassLayoutStruct[ii].classsize);
                    int i = ClassLayoutStruct[ii].parent;
                    string s = GetTypeDefTable(i);
                    Console.WriteLine("Parent TypeDef[{0}]...{1} ", i, s);
                }
            }
        }
        public void DisplayFieldLayoutTable()
        {
            if (FieldLayoutStruct != null)
            {
                Console.WriteLine("\n Field Layout Table\n");
                for (int ii = 1; ii <= FieldLayoutStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Offset:{0}", FieldLayoutStruct[ii].offset);
                    int index = FieldLayoutStruct[ii].fieldindex;
                    string s = GetFieldTable(index);
                    Console.WriteLine("Field :Field[{0}]...{1}", index, s);
                }
            }
        }
        public string DisplayStandAloneSigSignature(int index)
        {
            string s = "";
            int count = blob[index];
            s = s + count.ToString() + " ";
            for (int l = 1; l <= count; l++)
                s = s + blob[index + l].ToString() + " ";
            return s;
        }
        public void DisplayStandAloneSigTable()
        {
            if (StandAloneSigStruct != null)
            {
                Console.WriteLine("\nStandAloneSig Table\n");
                for (int ii = 1; ii <= StandAloneSigStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    string s = DisplayStandAloneSigSignature(StandAloneSigStruct[ii].index);
                    Console.WriteLine("Signature BLOB[{0}] Count {1} ", StandAloneSigStruct[ii].index, s);
                }
            }
        }
        public string GetTypeRefTable(int index)
        {
            int name = TypeRefStruct[index].name;
            int nspace = TypeRefStruct[index].nspace;
            string s = GetString(nspace);
            if (s != "null")
                s = s + ".";
            else
                s = "";
            s = s + GetString(name);
            return s;
        }
        public string GetTypeDefOrRefTable(int a)
        {
            string s = "";
            short tag = (short)(a & (short)0x03);
            if (tag == 0)
                s = s + "TypeDef";
            if (tag == 1)
                s = s + "TypeRef";
            if (tag == 2)
                s = s + "TypeSpec";
            return s;
        }
        public int GetTypeDefOrRefValue(int a)
        {
            return a >> 2;
        }
        public string GetTypeDefTable(int index)
        {
            int name = TypeDefStruct[index].name;
            int nspace = TypeDefStruct[index].nspace;
            int cindex = TypeDefStruct[index].cindex;
            int typereftableindex = GetTypeDefOrRefValue(cindex);
            string tablename = GetTypeDefOrRefTable(cindex);
            string s1 = "";
            if (tablename == "TypeRef")
                s1 = " Extends " + GetTypeRefTable(typereftableindex);
            if (GetString(nspace) != "")
                s1 = s1 + " Namespace: " + GetString(nspace);
            string s = "Class " + GetString(name) + s1;
            return s;
        }
        public string GetEventTable(int index)
        {
            string s = "";
            int ind = EventStruct[index].name;
            s = GetString(ind);
            int coded = EventStruct[index].coded;
            string tablename = GetTypeDefOrRefTable(coded);
            int ind1 = GetTypeDefOrRefValue(coded);
            string s1 = DisplayTable(tablename, ind1);
            s = "Event " + s + " " + s1 + " ";
            return s;
        }
        public string DisplayEventsList(int start, int rowindex)
        {
            string s = "";
            int end = EventStruct.Length;
            int end1 = 10000;
            if (rowindex <= EventMapStruct.Length)
                end1 = EventMapStruct[rowindex].eindex;
            int end2 = 0;
            if (end <= end1)
                end2 = end;
            else
                end2 = end1;
            for (int i = start; i <= end2; i++)
                s = s + GetEventTable(i) + "\n";
            return s;
        }
        public void DisplayEventMapTable()
        {
            if (EventMapStruct != null)
            {
                Console.WriteLine("\nEventMap Table\n");
                for (int ii = 1; ii <= EventMapStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Parent TypeDef[{0}]...{1}", EventMapStruct[ii].index, GetTypeDefTable(EventMapStruct[ii].index));
                    string s = DisplayEventsList(EventMapStruct[ii].eindex, ii);
                    Console.Write("EventList Event[{0}]...{1}", EventMapStruct[ii].eindex, s);
                }
            }
        }
        public string GetEventsAttributes(int a)
        {
            string s = "";
            if ((a & 0x0200) == 0x0200)
                s = s + "Special Name";
            if ((a & 0x0400) == 0x0400)
                s = s + "RTSpecialName";
            if (s.Length == 0)
                return "None";
            else
                return s;
        }
        public void DisplayEventTable()
        {
            if (EventStruct != null)
            {
                Console.WriteLine("\nEvent Table\n");
                for (int ii = 1; ii <= EventStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Event Flags: {0}", GetEventsAttributes(EventStruct[ii].attr));
                    Console.WriteLine("Name: {0}", GetString(EventStruct[ii].name));
                    string tablename = GetTypeDefOrRefTable(EventStruct[ii].coded);
                    int index = GetTypeDefOrRefValue(EventStruct[ii].coded);
                    Console.WriteLine("Event Type:{0}[{1}]... {2}", tablename,
                    index, DisplayTable(tablename, index));
                }
            }
        }
        public string DisplayPropertiesList(int start, int rowindex)
        {
            string s = "";
            int end = PropertyStruct.Length;
            int end1 = 10000;
            if (rowindex <= PropertyMapStruct.Length)
                end1 = PropertyMapStruct[rowindex].propertylist;
            int end2 = 0;
            if (end <= end1)
                end2 = end;
            else
                end2 = end1;
            for (int i = start; i <= end2; i++)
                s = s + GetPropertyTable(i) + "\n";
            return s;
        }
        public string GetPropertyTable(int index)
        {
            string s = "";
            int ind = PropertyStruct[index].name;
            s = GetString(ind);
            return s;
        }
        public void DisplayPropertyMapTable()
        {
            if (PropertyMapStruct != null)
            {
                Console.WriteLine("\n Property Map Table\n");
                for (int ii = 1; ii <= PropertyMapStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Parent TypeDef[{0}]...{1}", PropertyMapStruct[ii].parent, GetTypeDefTable(PropertyMapStruct[ii].parent));
                    string s = DisplayPropertiesList(PropertyMapStruct[ii].propertylist, ii);
                    Console.WriteLine("PropertyList Property[{0}]...{1}", PropertyMapStruct[ii].propertylist, s);
                }
            }
        }
        public string DisplayPropertySignature(int index)
        {
            string s = "";
            int count = blob[index];
            s = s + count.ToString() + " ";
            for (int l = 1; l <= count; l++)
                s = s + blob[index + l].ToString() + " ";
            return s;
        }
        public void DisplayPropertiesTable()
        {
            if (PropertyStruct != null)
            {
                Console.WriteLine("\nProperties Table\n");
                for (int ii = 1; ii <= PropertyStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Flags [{0}]", PropertyStruct[ii].flags);
                    Console.WriteLine("Name {0}", GetString(PropertyStruct[ii].name));
                    string s = DisplayPropertySignature(PropertyStruct[ii].type);
                    Console.WriteLine("Type BLOB[{0}]...{1} ", PropertyStruct[ii].type, s);
                }
            }
        }
        public string GetMethodSemanticsAttributes(short a)
        {
            string s = "";
            if ((a & 0x01) == 0x01)
                s = s + "Setter";
            if ((a & 0x02) == 0x02)
                s = s + "Getter";
            if ((a & 0x04) == 0x04)
                s = s + "Other";
            if ((a & 0x08) == 0x08)
                s = s + "Event Addon";
            if ((a & 0x10) == 0x10)
                s = s + "Event Remove";
            if ((a & 0x20) == 0x20)
                s = s + "Event Fire";
            return s;
        }
        public int GetMethodDefValue(int a)
        {
            return a >> 1;
        }
        public string GetMethodDefTable(int a)
        {
            string s = "";
            short tag = (short)(a & (short)0x01);
            if (tag == 0)
                s = s + "MethodDef";
            if (tag == 1)
                s = s + "MethodRef";
            return s;
        }
        public string GetMethodTable(int index)
        {
            string s = "";
            int ind = MethodStruct[index].name;
            s = GetString(ind);
            return s;
        }
        public int GetHasSemanticsValue(int a)
        {
            return a >> 1;
        }
        public string GetHasSemanticsTable(int a)
        {
            string s = "";
            int tag = a & 0x01;
            if (tag == 0)
                s = s + "Event";
            if (tag == 1)
                s = s + "Property";
            return s;
        }
        public void DisplayMethodSemanticsTable()
        {
            if (MethodSemanticsStruct != null)
            {
                Console.WriteLine("\nMethodSemantics Table\n");
                for (int ii = 1; ii <= MethodSemanticsStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Semantics {0} ", GetMethodSemanticsAttributes(MethodSemanticsStruct[ii].methodsemanticsattributes));
                    string s = GetMethodTable(MethodSemanticsStruct[ii].methodindex);
                    Console.WriteLine("Method Method[{0}]...{1}", MethodSemanticsStruct[ii].methodindex, s);
                    string tablename = GetHasSemanticsTable(MethodSemanticsStruct[ii].association);
                    int index = GetHasSemanticsValue(MethodSemanticsStruct[ii].association);
                    s = DisplayTable(tablename, index);
                    Console.WriteLine("Association:{0}[{1}]...{2} ", tablename, index, s);
                }
            }
        }
        public void DisplayMethodImpTable()
        {
            if (MethodImpStruct != null)
            {
                Console.WriteLine("\n MethodImp Table\n");
                for (int ii = 1; ii <= MethodImpStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Class TypeDef[{0}]...{1}", MethodImpStruct[ii].classindex, GetTypeDefTable(MethodImpStruct[ii].classindex));
                    string tablename = GetMethodDefTable(MethodImpStruct[ii].codedbody);
                    int index = GetMethodDefValue(MethodImpStruct[ii].codedbody);
                    string s = DisplayTable(tablename, index);
                    Console.WriteLine("MethodBody {0}[{1}]...{2} ", tablename, index, s);
                    tablename = GetMethodDefTable(MethodImpStruct[ii].codeddef);
                    index = GetMethodDefValue(MethodImpStruct[ii].codeddef);
                    s = DisplayTable(tablename, index);
                    Console.WriteLine("MethodDeclaration {0}[{1}]...{2} ", tablename, index, s);
                }
            }
        }
        public void DisplayModuleRefTable()
        {
            if (ModuleRefStruct != null)
            {
                Console.WriteLine("\nModuleRef Table\n");
                for (int ii = 1; ii <= ModuleRefStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Name {0}", GetString(ModuleRefStruct[ii].name));
                }
            }
        }
        public void DisplayTypeSpecTableTable()
        {
            if (TypeSpecStruct != null)
            {
                Console.WriteLine("\nTypeSpec Table\n");
                for (int ii = 1; ii <= TypeSpecStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    byte count = blob[TypeSpecStruct[ii].signature];
                    Console.WriteLine("Signature BLOB[{0}] Count {1}", TypeSpecStruct[ii].signature, count);
                    for (int l = 1; l <= count; l++)
                    {
                        Console.WriteLine("{0} {1}", blob[TypeSpecStruct[ii].signature + l], GetType(blob[TypeSpecStruct[ii].signature + l]));
                    }
                    Console.WriteLine("");
                }
            }
        }
        public string GetPInvokeAttributes(short a)
        {
            string s = "";
            if ((a & 0x0001) == 0x0001)
                s = s + "NoMangle ";
            if ((a & 0x0006) == 0x0006)
                s = s + "CharSetMask ";
            if ((a & 0x0000) == 0x0000)
                s = s + "CharSetNotSpec ";
            if ((a & 0x0002) == 0x0002)
                s = s + "CharSetAnsi ";
            if ((a & 0x0004) == 0x0004)
                s = s + "CharSetUnicode ";
            if ((a & 0x0004) == 0x0004)
                s = s + "CharSetAuto ";
            if ((a & 0x0040) == 0x0040)
                s = s + "SupportsLastError ";
            if ((a & 0x0700) == 0x0700)
                s = s + "CallConvMask ";
            if ((a & 0x0100) == 0x0100)
                s = s + "CallConvWinapi ";
            if ((a & 0x0200) == 0x0200)
                s = s + "CallConvCdecl ";
            if ((a & 0x0300) == 0x0300)
                s = s + "CallConvStdcall ";
            if ((a & 0x0400) == 0x0400)
                s = s + "CallConvThiscall ";
            if ((a & 0x0500) == 0x0500)
                s = s + "CallConvFastcall ";
            return s;
        }
        public int GetMemberForwardedValue(int a)
        {
            return a >> 1;
        }
        public string GetMemberForwardedTable(int a)
        {
            string s = "";
            int tag = a & 0x01;
            if (tag == 0)
                s = "Field";
            if (tag == 1)
                s = "MethodDef";
            return s;
        }
        public string GetModuleRefTable(int index)
        {
            string s = "";
            int ind = ModuleRefStruct[index].name;
            s = GetString(ind);
            return s;
        }
        public void DisplayImplMapTable()
        {
            if (ImplMapStruct != null)
            {
                Console.WriteLine("ImplMap Table\n");
                for (int ii = 1; ii <= ImplMapStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Mapping Flags :{0}", GetPInvokeAttributes(ImplMapStruct[ii].attr));
                    string tablename = GetMemberForwardedTable(ImplMapStruct[ii].cindex);
                    int index = GetMemberForwardedValue(ImplMapStruct[ii].cindex);
                    string s = DisplayTable(tablename, index);
                    Console.WriteLine("MemberForwarded:{0}[{1}]...{2} ", tablename, index, s);
                    Console.WriteLine("Name :{0}", GetString(ImplMapStruct[ii].name));
                    index = ImplMapStruct[ii].scope;
                    Console.WriteLine("Import Scope :ModuleRef[{0}] {1}", index, GetModuleRefTable(index));
                }
            }
        }
        public string GetFieldTable(int index)
        {
            string s = "";
            int ind = FieldStruct[index].name;
            s = GetString(ind);
            return s;
        }
        public string GetFileAttributes(int a)
        {
            string s = "";
            if (a == 0x00)
                s = "ContainsMetaData";
            if (a == 0x01)
                s = "ContainsNoMetaData";
            return s;
        }
        public void DisplayFieldRVATable()
        {
            if (FieldRVAStruct != null)
            {
                Console.WriteLine("\n FieldRVA Tble \n");
                for (int ii = 1; ii <= FieldRVAStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("RVA {0}", FieldRVAStruct[ii].rva.ToString("X"));
                    int index = FieldRVAStruct[ii].fieldi;
                    Console.WriteLine("Field Field[{0}]... {1}", index, GetFieldTable(index));
                }
            }
        }
        public string GetAssemblyTable(int index)
        {
            int ind = AssemblyStruct[index].name;
            return GetString(ind);
        }
        public string GetAssemblyRefTable(int index)
        {
            int ind = AssemblyRefStruct[index].name;
            return GetString(ind);
        }
        public void DisplayAssemblyTable()
        {
            if (AssemblyStruct != null)
            {
                Console.WriteLine("Assembly Table\n");
                for (int ii = 1; ii <= AssemblyStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("HashAlgId {0}", AssemblyStruct[ii].HashAlgId);
                    Console.WriteLine("MajorVersion {0}", AssemblyStruct[ii].major);
                    Console.WriteLine("MinorVersion {0}", AssemblyStruct[ii].minor);
                    Console.WriteLine("BuildNumber {0}", AssemblyStruct[ii].build);
                    Console.WriteLine("RevisionNumber {0}", AssemblyStruct[ii].revision);
                    Console.WriteLine("Flags {0}", AssemblyStruct[ii].flags.ToString());
                    int uncompressedbyte;
                    int cb = CorSigUncompressData(blob, AssemblyStruct[ii].publickey, out uncompressedbyte);
                    cb--;
                    int count = uncompressedbyte;
                    Console.WriteLine("Public Key #BLOB[{0}] Count {1}", AssemblyStruct[ii].publickey, count);
                    for (int l = 1; l <= count; l++)
                    {
                        Console.Write("{0} ", blob[AssemblyStruct[ii].publickey + l + cb].ToString("X"));
                    }
                    if (count != 0)
                        Console.WriteLine("");
                    Console.WriteLine("Name:{0}", GetString(AssemblyStruct[ii].name));
                    Console.WriteLine("Culture:{0}", GetString(AssemblyStruct[ii].culture));
                }
            }
        }
        public void DisplayAssemblyRefTable()
        {
            if (AssemblyRefStruct != null)
            {
                Console.WriteLine("\n AssemblyRef Table\n");
                for (int ii = 1; ii <= AssemblyRefStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("MajorVersion {0}", AssemblyRefStruct[ii].major);
                    Console.WriteLine("MinorVersion {0}", AssemblyRefStruct[ii].minor);
                    Console.WriteLine("BuildNumber {0}", AssemblyRefStruct[ii].build);
                    Console.WriteLine("RevisionNumber {0}", AssemblyRefStruct[ii].revision);
                    Console.WriteLine("Flags {0}", AssemblyRefStruct[ii].flags.ToString());
                    int uncompressedbyte;
                    int cb = CorSigUncompressData(blob, AssemblyRefStruct[ii].publickey,
                    out uncompressedbyte);
                    cb--;
                    int count = uncompressedbyte;
                    Console.WriteLine("Public Key or Token #BLOB[{0}] Count {1}",
                    AssemblyRefStruct[ii].publickey, count);
                    for (int l = 1; l <= count; l++)
                    {
                        Console.Write("{0} ", blob[AssemblyRefStruct[ii].publickey + l + cb].ToString("X"));
                    }
                    if (count != 0)
                        Console.WriteLine("");
                    Console.WriteLine("Name:{0}", GetString(AssemblyRefStruct[ii].name));
                    Console.WriteLine("Culture:{0}", GetString(AssemblyRefStruct[ii].culture));
                    cb = CorSigUncompressData(blob, AssemblyRefStruct[ii].hashvalue, out uncompressedbyte);
                    cb--;
                    count = uncompressedbyte;
                    Console.WriteLine("Hash Value #BLOB[{0}] Count {1} ",
                    AssemblyRefStruct[ii].hashvalue, count);
                    for (int l = 1; l <= count; l++)
                    {
                        Console.Write("{0} ", blob[AssemblyRefStruct[ii].hashvalue + l + cb].ToString("X"));
                    }
                    if (count != 0)
                        Console.WriteLine("");
                }
            }
        }
        public void DisplayFileTable()
        {
            if (FileStruct != null)
            {
                Console.WriteLine("\nFile Table\n");
                for (int ii = 1; ii <= FileStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Flags {0}", GetFileAttributes(FileStruct[ii].flags));
                    Console.WriteLine("Name {0}", GetString(FileStruct[ii].name));
                    int uncompressedbyte;
                    int cb = CorSigUncompressData(blob, FileStruct[ii].index, out uncompressedbyte);
                    int count = uncompressedbyte;
                    cb--;
                    Console.WriteLine("HashValue BLOB[{0}] Count {1}", FileStruct[ii].index, count);
                    for (int l = 1; l <= count; l++)
                    {
                        Console.Write("{0} ", blob[FileStruct[ii].index + l + cb]);
                    }
                    Console.WriteLine("");
                }
            }
        }
        public int GetImplementationValue(int a)
        {
            return a >> 2;
        }
        public string GetImplementationTable(int a)
        {
            string s = "";
            int tag = a & 0x03;
            if (tag == 0)
                s = s + "File";
            if (tag == 1)
                s = s + "AssemblyRef";
            if (tag == 2)
                s = s + "ExportedType";
            return s;
        }
        public void DisplayExportedTypeTable()
        {
            if (ExportedTypeStruct != null)
            {
                Console.WriteLine("\nExported Table\n");
                for (int ii = 1; ii <= ExportedTypeStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Flags {0}", ExportedTypeStruct[ii].flags);
                    Console.WriteLine("TypeDef TYPEDEF[{0}]...{1}", ExportedTypeStruct[ii].typedefindex, GetTypeDefTable(ExportedTypeStruct[ii].typedefindex));
                    Console.WriteLine("TypeName {0}", GetString(ExportedTypeStruct[ii].name));
                    Console.WriteLine("NameSpace {0}", GetString(ExportedTypeStruct[ii].nspace));
                    string tablename = GetImplementationTable(ExportedTypeStruct[ii].coded);
                    int ind = GetImplementationValue(ExportedTypeStruct[ii].coded);
                    Console.WriteLine("Implementation {0}[{1}]...{2} ", tablename, ind, DisplayTable(tablename, ind));
                }
            }
        }
        public int GetManifestResourceValue(int a)
        {
            return a >> 2;
        }
        public string GetManifestResourceAttributes(int a)
        {
            string s = "";
            if ((a & 0x007) == 0x007)
                s = s + "VisibilityMask ";
            if ((a & 0x001) == 0x001)
                s = s + "Public ";
            if ((a & 0x002) == 0x002)
                s = s + "Private ";
            return s;
        }
        public string GetManifestResourceTable(int a)
        {
            string s = "";
            short tag = (short)(a & (short)0x03);
            if (tag == 0)
                s = s + "File";
            if (tag == 1)
                s = s + "AssemblyRef";
            if (tag == 2)
                s = s + "ExportedType";
            return s;
        }
        public void DisplayManifestResourceTable()
        {
            if (ManifestResourceStruct != null)
            {
                Console.WriteLine("\nManifest Resource Table\n");
                for (int ii = 1; ii <= ManifestResourceStruct.Length - 1; ii++)
                {
                    Console.WriteLine("Row {0}", ii);
                    Console.WriteLine("Offset {0}", ManifestResourceStruct[ii].offset);
                    Console.WriteLine("Flags {0}", GetManifestResourceAttributes(ManifestResourceStruct[ii].flags));
                    Console.WriteLine("Name {0}", GetString(ManifestResourceStruct[ii].name));
                    string tablename = GetManifestResourceTable(ManifestResourceStruct[ii].coded);
                    int index = GetManifestResourceValue(ManifestResourceStruct[ii].coded);
                    Console.WriteLine("Implementation: {0}[{1}]...{2}",
                    tablename, index, DisplayTable(tablename, index));
                }
            }
        }
        public string GetModuleTable(int index)
        {
            string s = "";
            int ind = ModuleStruct[index].Name;
            s = GetString(ind);
            return s;
        }
        public string GetParamTable(int index)
        {
            string s = "";
            int ind = ParamStruct[index].name;
            s = GetString(ind);
            return s;
        }
        public string GetMemberRefTable(int index)
        {
            string s = "";
            int ind = MemberRefStruct[index].name;
            s = GetString(ind);
            return s;
        }
        public string GetFileTable(int index)
        {
            string s = "";
            int ind = FileStruct[index].name;
            s = GetString(ind);
            return s;
        }
        public string DisplayTable(string tablename, int index)
        {
            string s = "";
            if (index == 0)
                return s;
            if (tablename == "Module")
                s = GetModuleTable(index);
            if (tablename == "TypeRef")
                s = GetTypeRefTable(index);
            if (tablename == "TypeDef")
                s = GetTypeDefTable(index);
            if (tablename == "Field")
                s = GetFieldTable(index);
            if (tablename == "MethodDef")
                s = GetMethodTable(index);
            if (tablename == "Param")
                s = GetParamTable(index);
            if (tablename == "MethodRef")
                s = GetMemberRefTable(index);
            if (tablename == "AssemblyRef")
                s = GetAssemblyRefTable(index);
            if (tablename == "Assembly")
                s = GetAssemblyTable(index);
            if (tablename == "ModuleRef")
                s = GetModuleRefTable(index);
            if (tablename == "File")
                s = GetFileTable(index);
            if (tablename == "Property")
                s = GetPropertyTable(index);
            if (tablename == "Event")
                s = GetEventTable(index);
            return s;
        }
        public void DisplayMethodSignature(int index1, int row)
        {
            if (!(index1 >= 0 && index1 < blob.Length))
                return;
            int cb, uncompressedbyte, count;
            cb = CorSigUncompressData(blob, index1, out uncompressedbyte);
            count = uncompressedbyte;
            byte[] b = new byte[count];
            Array.Copy(blob, index1 + cb, b, 0, count);
            if (b[2] == 0x20)
                return; // modopt not implemented
            if (b[2] == 0x1B)
                return;// Pointer To function not implemented
            int index = 0;
            cb = CorSigUncompressData(b, index, out uncompressedbyte);
            index++;
            cb = CorSigUncompressData(b, index, out uncompressedbyte);
            count = uncompressedbyte;
            index = index + cb;
            string ss11 = "";
            ss11 = GetElemType(index, b, out cb) + " " + ss11;
            Console.Write(ss11 + " " + GetString(MethodStruct[row].name) + "(");
            index = index + cb;
            int paramindex = MethodStruct[row].param;
            string ss = "", s1;
            string ss2 = "";
            int emptyparam = 0;
            if (ParamStruct != null && paramindex < ParamStruct.Length &&
            ParamStruct[paramindex].sequence == 0)
                emptyparam++;
            for (int l = 1; l <= count; l++)
            {
                if (b[index] == 0x2d)
                    break;
                s1 = GetElemType(index, b, out cb);
                while (s1[0] == '[' && s1[1] == ']')
                {
                    string ss1 = s1.Substring(2);
                    ss1 = ss1 + "[]";
                    s1 = ss1;
                }
                index = index + cb;
                ss2 = ss2 + s1;
                if (l != count)
                    ss2 = ss2 + ",";
                int ind = paramindex + l + emptyparam - 1;
                ss = ss + s1 + " " + GetString(ParamStruct[ind].name);
                if (l != count)
                    ss = ss + " , ";
            }
            Console.Write(ss + ")");
            Console.WriteLine("");
            Console.WriteLine("");
        }
        public void DisplayTableForDebugging()
        {
            DisplayModuleTable();
            DisplayTypeRefTable();
            DisplayTypeDefTable();
            DisplayNestedClassTable();
            DisplayFieldTable();
            DisplayMethodTable();
            DisplayParamTable();
            DisplayInterfaceImplTable();
            DisplayMemberRefTable();
            DisplayConstantTable();
            DisplayCustomAttributeTable();
            DisplayFieldMarshalTable();
            DisplayDeclSecurityTable();
            DisplayClassLayoutTable();
            DisplayFieldLayoutTable();
            DisplayStandAloneSigTable();
            DisplayEventMapTable();
            DisplayEventTable();
            DisplayPropertyMapTable();
            DisplayPropertiesTable();
            DisplayMethodSemanticsTable();
            DisplayMethodImpTable();
            DisplayModuleRefTable();
            DisplayTypeSpecTableTable();
            DisplayImplMapTable();
            DisplayFieldRVATable();
            DisplayAssemblyTable();
            DisplayAssemblyRefTable();
            DisplayFileTable();
            DisplayExportedTypeTable();
            DisplayManifestResourceTable();
        }
    }
}
