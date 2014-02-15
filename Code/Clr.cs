using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace Code
{

    public interface IClrRow
    {
        string ToString();
    }
    public class ClrRow : Code.IClrRow
    {
        public int nameIndex;
        public string name;
        public string nameSpace;
        protected BitStreamReader ms;
        protected long p;
        public override string ToString()
        {
            return name;
        }
        public ClrRow(BinaryFileReader sw, int i, int rowSize, List<ClrStream> clrstream)
        {
            byte[] buffer = sw.ReadBytes(rowSize);
            ms = new BitStreamReader(buffer, false);
        }
    }
    public class ClrRow_Module : ClrRow
    {
        public Guid Mguid;
        public Guid EngGuID;
        public Guid EncB;
        public ClrRow_Module(BinaryFileReader sw, int i, int rowSize, List<ClrStream> clrstream)
            : base(sw, i, rowSize, clrstream)
        {
            int Generation = ms.ReadShort();
            //short or int depends on heap byte ?
            nameIndex = ms.ReadShort();// name field is an index into the string stream
            int Mvid = ms.ReadShort();//mvid is an index into the Guid heap
            int EncId = ms.ReadShort();//index into the Guid heap
            int EncBaseId = ms.ReadShort();//index into the Guid heap
            long p = sw.Position;
            sw.Position = clrstream[1].offset + nameIndex;
            name = sw.ReadStringZ(Encoding.Default);
            sw.Position = clrstream[3].offset + Mvid;
            Mguid = new Guid(sw.ReadBytes(16));
            sw.Position = clrstream[3].offset + EncId;
            EngGuID = new Guid(sw.ReadBytes(16));
            sw.Position = clrstream[3].offset + EncBaseId;
            EncB = new Guid(sw.ReadBytes(16));
            sw.Position = p;
        }
    }
    public class ClrRow_TypeRef : ClrRow
    {
        public ClrRow_TypeRef(BinaryFileReader sw, int i, int rowSize, List<ClrStream> clrstream)
            : base(sw, i, rowSize, clrstream)
        {
            int resolutionscope = ms.ReadShort();
            nameIndex = ms.ReadShort();
            int nspace = ms.ReadShort();
            p = sw.Position;
            sw.Position = clrstream[1].offset + nameIndex;
            name = sw.ReadStringZ(Encoding.Default);
            sw.Position = clrstream[1].offset + nspace;
            nameSpace = sw.ReadStringZ(Encoding.Default);
            sw.Position = p;
        }
    }
    public class ClrRow_TypeDef : ClrRow
    {
        #region TypeDef Flags
        int typeDefFlag;
        public bool IsTdNotPublic { get { return (((typeDefFlag) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNotPublic); } }
        public bool IsTdPublic { get { return (((typeDefFlag) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdPublic); } }
        public bool IsTdNestedPublic { get { return (((typeDefFlag) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedPublic); } }
        public bool IsTdNestedPrivate { get { return (((typeDefFlag) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedPrivate); } }
        public bool IsTdNestedFamily { get { return (((typeDefFlag) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedFamily); } }
        public bool IsTdNestedAssembly { get { return (((typeDefFlag) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedAssembly); } }
        public bool IsTdNestedFamANDAssem { get { return (((typeDefFlag) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedFamANDAssem); } }
        public bool IsTdNestedFamORAssem { get { return (((typeDefFlag) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedFamORAssem); } }
        public bool IsTdNested { get { return (((typeDefFlag) & (int)CorTypeAttr.tdVisibilityMask) >= (int)CorTypeAttr.tdNestedPublic); } }
        public bool IsTdAutoLayout { get { return (((typeDefFlag) & (int)CorTypeAttr.tdLayoutMask) == (int)CorTypeAttr.tdAutoLayout); } }
        public bool IsTdSequentialLayout { get { return (((typeDefFlag) & (int)CorTypeAttr.tdLayoutMask) == (int)CorTypeAttr.tdSequentialLayout); } }
        public bool IsTdExplicitLayout { get { return (((typeDefFlag) & (int)CorTypeAttr.tdLayoutMask) == (int)CorTypeAttr.tdExplicitLayout); } }
        public bool IsTdClass { get { return (((typeDefFlag) & (int)CorTypeAttr.tdClassSemanticsMask) == (int)CorTypeAttr.tdClass); } }
        public bool IsTdInterface { get { return (((typeDefFlag) & (int)CorTypeAttr.tdClassSemanticsMask) == (int)CorTypeAttr.tdInterface); } }
        public bool IsTdAbstract { get { return ((typeDefFlag) & (int)CorTypeAttr.tdAbstract) > 0; } }
        public bool IsTdSealed { get { return ((typeDefFlag) & (int)CorTypeAttr.tdSealed) > 0; } }
        public bool IsTdSpecialName { get { return ((typeDefFlag) & (int)CorTypeAttr.tdSpecialName) > 0; } }
        public bool IsTdImport { get { return ((typeDefFlag) & (int)CorTypeAttr.tdImport) > 0; } }
        public bool IsTdSerializable { get { return ((typeDefFlag) & (int)CorTypeAttr.tdSerializable) > 0; } }
        public bool IsTdAnsiClass { get { return (((typeDefFlag) & (int)CorTypeAttr.tdStringFormatMask) == (int)CorTypeAttr.tdAnsiClass); } }
        public bool IsTdUnicodeClass { get { return (((typeDefFlag) & (int)CorTypeAttr.tdStringFormatMask) == (int)CorTypeAttr.tdUnicodeClass); } }
        public bool IsTdAutoClass { get { return (((typeDefFlag) & (int)CorTypeAttr.tdStringFormatMask) == (int)CorTypeAttr.tdAutoClass); } }
        public bool IsTdCustomFormatClass { get { return (((typeDefFlag) & (int)CorTypeAttr.tdStringFormatMask) == (int)CorTypeAttr.tdCustomFormatClass); } }
        public bool IsTdBeforeFieldInit { get { return ((typeDefFlag) & (int)CorTypeAttr.tdBeforeFieldInit) > 0; } }
        public bool IsTdForwarder { get { return ((typeDefFlag) & (int)CorTypeAttr.tdForwarder) > 0; } }
        public bool IsTdRTSpecialName { get { return ((typeDefFlag) & (int)CorTypeAttr.tdRTSpecialName) > 0; } }
        public bool IsTdHasSecurity { get { return ((typeDefFlag) & (int)CorTypeAttr.tdHasSecurity) > 0; } }
        #endregion
        public ClrRow_TypeDef(BinaryFileReader sw, int i, int rowSize, List<ClrStream> clrstream)
            : base(sw, i, rowSize, clrstream)
        {
            typeDefFlag = ms.ReadInteger();
            nameIndex = ms.ReadShort();
            int nspace = ms.ReadShort();
            int cindex = ms.ReadShort();
            int findex = ms.ReadShort();//First row of the FieldDef table ??
            int mindex = ms.ReadShort();//First row of the MethodDef table ??
            //            int tag = resolutionscope & 0x03;
            //            int riid = resolutionscope >> 2;
            p = sw.Position;
            sw.Position = clrstream[1].offset + nameIndex;
            name = sw.ReadStringZ(Encoding.Default);
            sw.Position = clrstream[1].offset + nspace;
            nameSpace = sw.ReadStringZ(Encoding.Default);
            sw.Position = p;
        }
    }
    public class ClrRow_MethodDef : ClrRow
    {
        public int rva;
        CorMethodImpl implflags;
        #region CorMethodImpl
        public bool IsmiIL { get { return (((implflags) & CorMethodImpl.miCodeTypeMask) == CorMethodImpl.miIL); } }
        public bool IsmiNative { get { return (((implflags) & CorMethodImpl.miCodeTypeMask) == CorMethodImpl.miNative); } }
        public bool IsmiOPTIL { get { return (((implflags) & CorMethodImpl.miCodeTypeMask) == CorMethodImpl.miOPTIL); } }
        public bool IsmiRuntime { get { return (((implflags) & CorMethodImpl.miCodeTypeMask) == CorMethodImpl.miRuntime); } }
        public bool IsmiUnmanaged { get { return (((implflags) & CorMethodImpl.miManagedMask) == CorMethodImpl.miUnmanaged); } }
        public bool IsmiManaged { get { return (((implflags) & CorMethodImpl.miManagedMask) == CorMethodImpl.miManaged); } }
        public bool IsmiForwardRef { get { return ((implflags) & CorMethodImpl.miForwardRef) > 0; } }
        public bool IsmiPreserveSig { get { return ((implflags) & CorMethodImpl.miPreserveSig) > 0; } }
        public bool IsmiInternalCall { get { return ((implflags) & CorMethodImpl.miInternalCall) > 0; } }
        public bool IsmiSynchronized { get { return ((implflags) & CorMethodImpl.miSynchronized) > 0; } }
        public bool IsmiNoInlining { get { return ((implflags) & CorMethodImpl.miNoInlining) > 0; } }
        public bool IsmiNoOptimization { get { return ((implflags) & CorMethodImpl.miNoOptimization) > 0; } }
        #endregion
        CorMethodAttr flags;
        #region CorMethodAttr
        public bool IsmdPrivateScope { get { return (((flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdPrivateScope); } }
        public bool IsmdPrivate { get { return (((flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdPrivate); } }
        public bool IsmdFamANDAssem { get { return (((flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdFamANDAssem); } }
        public bool IsmdAssem { get { return (((flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdAssem); } }
        public bool IsmdFamily { get { return (((flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdFamily); } }
        public bool IsmdFamORAssem { get { return (((flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdFamORAssem); } }
        public bool IsmdPublic { get { return (((flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdPublic); } }

        public bool IsmdStatic { get { return ((flags) & CorMethodAttr.mdStatic) > 0; } }
        public bool IsmdFinal { get { return ((flags) & CorMethodAttr.mdFinal) > 0; } }
        public bool IsmdVirtual { get { return ((flags) & CorMethodAttr.mdVirtual) > 0; } }
        public bool IsmdHideBySig { get { return ((flags) & CorMethodAttr.mdHideBySig) > 0; } }

        public bool IsmdReuseSlot { get { return (((flags) & CorMethodAttr.mdVtableLayoutMask) == CorMethodAttr.mdReuseSlot); } }
        public bool IsmdNewSlot { get { return (((flags) & CorMethodAttr.mdVtableLayoutMask) == CorMethodAttr.mdNewSlot); } }

        public bool IsmdCheckAccessOnOverride { get { return ((flags) & CorMethodAttr.mdCheckAccessOnOverride) > 0; } }
        public bool IsmdAbstract { get { return ((flags) & CorMethodAttr.mdAbstract) > 0; } }
        public bool IsmdSpecialName { get { return ((flags) & CorMethodAttr.mdSpecialName) > 0; } }

        public bool IsmdPinvokeImpl { get { return ((flags) & CorMethodAttr.mdPinvokeImpl) > 0; } }
        public bool IsmdUnmanagedExport { get { return ((flags) & CorMethodAttr.mdUnmanagedExport) > 0; } }

        public bool IsmdRTSpecialName { get { return ((flags) & CorMethodAttr.mdRTSpecialName) > 0; } }
        /*public bool  IsmdInstanceInitializer(Flags, str)     (((Flags) & CorMethodAttr.mdRTSpecialName) && !strcmp((str), COR_CTOR_METHOD_NAME));}}
        public bool  IsmdInstanceInitializerW(Flags, str)    (((Flags) & CorMethodAttr.mdRTSpecialName) && !wcscmp((str), COR_CTOR_METHOD_NAME_W));}}
        public bool  IsmdClassConstructor(Flags, str)        (((Flags) & CorMethodAttr.mdRTSpecialName) && !strcmp((str), COR_CCTOR_METHOD_NAME));}}
        public bool  IsmdClassConstructorW(Flags, str)       (((Flags) & CorMethodAttr.mdRTSpecialName) && !wcscmp((str), COR_CCTOR_METHOD_NAME_W));}}*/
        public bool IsmdHasSecurity { get { return ((flags) & CorMethodAttr.mdHasSecurity) > 0; } }
        #endregion
        public bool IsmdRequireSecObject { get { return ((flags) & CorMethodAttr.mdRequireSecObject) > 0; } }
        public ClrRow_MethodDef(BinaryFileReader sw, int i, int rowSize, List<ClrStream> clrstream)
            : base(sw, i, rowSize, clrstream)
        {
            rva = ms.ReadInteger();
            implflags = (CorMethodImpl)ms.ReadShort();// Various Flags indicating how the method is implemented. All-zeros indicate this is a pure-IL managed method
            flags = (CorMethodAttr)ms.ReadShort();//MethodAttributes
            nameIndex = ms.ReadShort();
            int signature = ms.ReadShort();//Offset into #Blob heap containing the signature (return type & parameter types) of the method
            int param = ms.ReadShort();
            p = sw.Position;
            sw.Position = clrstream[1].offset + nameIndex;
            name = sw.ReadStringZ(Encoding.Default);
            sw.Position = clrstream[4].offset + signature;
            //  nameSpace = sw.ReadStringZ(Encoding.Default);
            sw.Position = p;
        }
    }
    public class ClrRow_Field : ClrRow
    {
        CorFieldAttr fieldMask;
        public bool IsfdPrivateScope { get { return (((fieldMask) & CorFieldAttr.fdFieldAccessMask) == CorFieldAttr.fdPrivateScope); } }
        public bool IsfdPrivate { get { return (((fieldMask) & CorFieldAttr.fdFieldAccessMask) == CorFieldAttr.fdPrivate); } }
        public bool IsfdFamANDAssem { get { return (((fieldMask) & CorFieldAttr.fdFieldAccessMask) == CorFieldAttr.fdFamANDAssem); } }
        public bool IsfdAssembly { get { return (((fieldMask) & CorFieldAttr.fdFieldAccessMask) == CorFieldAttr.fdAssembly); } }
        public bool IsfdFamily { get { return (((fieldMask) & CorFieldAttr.fdFieldAccessMask) == CorFieldAttr.fdFamily); } }
        public bool IsfdFamORAssem { get { return (((fieldMask) & CorFieldAttr.fdFieldAccessMask) == CorFieldAttr.fdFamORAssem); } }
        public bool IsfdPublic { get { return (((fieldMask) & CorFieldAttr.fdFieldAccessMask) == CorFieldAttr.fdPublic); } }

        public bool IsfdStatic { get { return ((fieldMask) & CorFieldAttr.fdStatic) > 0; } }
        public bool IsfdInitOnly { get { return ((fieldMask) & CorFieldAttr.fdInitOnly) > 0; } }
        public bool IsfdLiteral { get { return ((fieldMask) & CorFieldAttr.fdLiteral) > 0; } }
        public bool IsfdNotSerialized { get { return ((fieldMask) & CorFieldAttr.fdNotSerialized) > 0; } }

        public bool IsfdPinvokeImpl { get { return ((fieldMask) & CorFieldAttr.fdPinvokeImpl) > 0; } }
        public bool IsfdSpecialName { get { return ((fieldMask) & CorFieldAttr.fdSpecialName) > 0; } }
        public bool IsfdHasFieldRVA { get { return ((fieldMask) & CorFieldAttr.fdHasFieldRVA) > 0; } }

        public bool IsfdRTSpecialName { get { return ((fieldMask) & CorFieldAttr.fdRTSpecialName) > 0; } }
        public bool IsfdHasFieldMarshal { get { return ((fieldMask) & CorFieldAttr.fdHasFieldMarshal) > 0; } }
        public bool IsfdHasDefault { get { return ((fieldMask) & CorFieldAttr.fdHasDefault) > 0; } }
        public string Type;
        public string GetType(int b)
        {
            if (b == 0x01)
                return "void";
            if (b == 0x02)
                return "boolean";
            if (b == 0x03)
                return "char";
            if (b == 0x04)
                return "byte";
            if (b == 0x05)
                return "ubyte";
            if (b == 0x06)
                return "short";
            if (b == 0x07)
                return "ushort";
            if (b == 0x08)
                return "int";
            if (b == 0x09)
                return "uint";
            if (b == 0x0a)
                return "long";
            if (b == 0x0b)
                return "ulong";
            if (b == 0x0c)
                return "float";
            if (b == 0x0d)
                return "double";
            if (b == 0x0e)
                return "string";
            return "unknown";
        }
        public ClrRow_Field(BinaryFileReader sw, int i, int rowSize, List<ClrStream> clrstream)
            : base(sw, i, rowSize, clrstream)
        {
            fieldMask = (CorFieldAttr)ms.ReadShort();
            nameIndex = ms.ReadShort();// name field is an index into the string stream
            int signature = ms.ReadShort();
            long p = sw.Position;
            sw.Position = clrstream[1].offset + nameIndex;
            name = sw.ReadStringZ(Encoding.Default);
            sw.Position = clrstream[4].offset + signature;
            byte[] bl = sw.ReadBytes(4);
            sw.Position = p;
            if (bl[1] == 0x06)
            {
                Type = GetType(bl[2]);
            }
        }
    }
    public class ClrRow_Param : ClrRow
    {
        public int pattr;
        public int sequence;
        public ClrRow_Param(BinaryFileReader sw, int i, int rowSize, List<ClrStream> clrstream)
            : base(sw, i, rowSize, clrstream)
        {
            pattr = ms.ReadShort();
            //short or int depends on heap byte ?
            sequence = ms.ReadShort();//mvid is an index into the Guid heap
            nameIndex = ms.ReadShort();// name field is an index into the string stream
            long p = sw.Position;
            sw.Position = clrstream[1].offset + nameIndex;
            name = sw.ReadStringZ(Encoding.Default);
            sw.Position = p;
        }
    }
    public class ClrTable
    {
        public int index;
        public long offset;
        public int numberOfRows;
        public int name;
        public List<int> indexesToString = new List<int>();
        public List<int> indexesToGuid = new List<int>();
        public string tableName
        {
            get
            {
                string[] tablenames = new String[]{"Module", "TypeRef","TypeDef","FieldPtr","Field","MethodPtr","MethodDef","ParamPtr",
                "Param", "InterfaceImpl", "MemberRef", "Constant", "CustomAttribute", "FieldMarshal", "DeclSecurity", "ClassLayout", 
                "FieldLayout", "StandAloneSig" , "EventMap","EventPtr", "Event", "PropertyMap", "PropertyPtr", "Properties","MethodSemantics",
                "MethodImpl","ModuleRef","TypeSpec","ImplMap","FieldRVA","ENCLog","ENCMap","Assembly","AssemblyProcessor","AssemblyOS",
                "AssemblyRef","AssemblyRefProcessor","AssemblyRefOS","File","ExportedType","ManifestResource","NestedClass","TypeTyPar","MethodTyPar"};
                string t = "";
                if (numberOfRows != 0)
                    t = tablenames[index];
                return t;
            }
        }
        public int rowSize;
        public List<string> rows;
        private List<IClrRow> clrRows = new List<IClrRow>();

        public List<IClrRow> ClrRows
        {
            get { return clrRows; }
            set { clrRows = value; }
        }
        List<ClrStream> clrstreams;
        public ClrTable(BinaryFileReader sw, int index, List<ClrStream> clrstreams)
        {
            byte[] sizes = { 10, 6, 14, 2, 6, 2, 14, 2, 6, 4, 6, 6, 6, 4, 6, 8, 6, 2, 4, 2, 6, 4, 2, 6, 6, 6, 2, 2, 8, 6, 8, 4, 22, 4, 12, 20, 6, 14, 8, 14, 12, 4 };
            offset = sw.Position;
            numberOfRows = sw.ReadInteger();
            rowSize = sizes[index];
            this.index = index;
            this.clrstreams = clrstreams;
        }
        public void parseRows(BinaryFileReader sw, long offset)
        {
            IClrRow row = null;
            for (int i = 0; i < numberOfRows; i++)
            {
                switch ((TableType)index)
                {
                    case TableType.Module://Module : 10 bytes
                        row = new ClrRow_Module(sw, index, rowSize, clrstreams);
                        break;
                    case TableType.TypeRef: //TypeRef : 6 bytes
                        row = new ClrRow_TypeRef(sw, index, rowSize, clrstreams);
                        break;
                    case TableType.TypeDef: //14 bytes
                        row = new ClrRow_TypeDef(sw, index, rowSize, clrstreams);
                        break;
                    case TableType.Field:
                        row = new ClrRow_Field(sw, index, rowSize, clrstreams);
                        break;
                    case TableType.ParamPtr:
                        break;
                    case TableType.Param:
                        row = new ClrRow_Param(sw, index, rowSize, clrstreams);
                        break;
                    case TableType.MethodDef: //
                        row = new ClrRow_MethodDef(sw, index, rowSize, clrstreams);
                        break;
                    case TableType.FieldPtr:
                    default:
                        row = new ClrRow(sw, index, rowSize, clrstreams);
                        break;
                }

                clrRows.Add(row);

            }
        }
        public override string ToString()
        {
            return tableName + " " + numberOfRows.ToString(); ;
        }
    }
    public class MetadataDirectory
    {
        public string signature;
        public int major;
        public int minor;
        public string str;
        public int flags;
        public List<ClrStream> clrstreams = new List<ClrStream>();
        public MetaData meta;
        public MetadataDirectory(BinaryFileReader sw, IMAGE_NT_HEADERS nt_headers)
        {
            long startofmetadata = sw.Position;
            signature = sw.ReadString(4);
            major = sw.ReadShort();
            minor = sw.ReadShort();
            int reserved = sw.ReadInteger();
            int len = sw.ReadInteger();
            str = sw.ReadString(len);
            int leftover = len % 4;
            if (leftover > 0)
                sw.ReadBytes(leftover);
            flags = sw.ReadShort();// Reserved ??
            int streams = sw.ReadShort();
            for (int i = 0; i < streams; i++)
            {
                clrstreams.Add(new ClrStream(sw, startofmetadata));
            }
            for (int i = 0; i < streams; i++)
            {
                sw.Position = /*startOfMetadata +*/ clrstreams[i].offset;
                switch (clrstreams[i].name[1])
                {
                    case '~':
                        meta = new MetaData(sw, clrstreams);
                        break;
                    /*                 case 'S':
                                          List<string> stri = new List<string>();
                                          break;
                                      case 'U':
                                          //              us = sw.ReadBytes(clrstream[i].size);
                                          break;
                                      case 'G':
                                          //              guid = sw.ReadBytes(clrstream[i].size);
                                          break;
                                      case 'B':
                                          //              blob = sw.ReadBytes(clrstream[i].size);
                                          break;*/
                }
            }
        }
    }
    public class MetaData
    {
        public int[] rows = new int[64];
        public int NumberOfTables
        {
            get
            {
                int a = 0;
                for (int i = 0; i < rows.Length; i++)
                    if (rows[i] != 0)
                        a++;
                return a;
            }
        }
        public string[] tabletype = new string[64];
        long tableoffset;
        public ClrTable[] clrTables = new ClrTable[64];
        public MetaData(BinaryFileReader sw, List<ClrStream> clrstreams)
        {
            tableoffset = sw.Position + 24;//??
            int resrves = sw.ReadInteger();
            int version = sw.ReadByte();
            int minversion = sw.ReadByte();
            int heap = sw.ReadByte();// heap offsets within the table use 2 or 4 bytes - bit 1 for #Strings, bit 2 for #GUID, bit 3 for #Blob
            int reserved = sw.ReadByte(); //must be 1
            Array.Clear(rows, 0, rows.Length);
            byte[] metadata = sw.ReadBytes(8);
            long valid = BitConverter.ToInt64(metadata, 0);
            byte[] sortedTables = sw.ReadBytes(8);// ??
            for (int k = 0; k <= 63; k++)
            {
                int tablepresent = (int)(valid >> k) & 1;
                if (tablepresent == 1)
                {
                    //                  ms.Position = tableoffset;
                    ClrTable cl = new ClrTable(sw, k, clrstreams);
                    clrTables[k] = cl;
                    rows[k] = cl.numberOfRows;
                }
            }
            long actualTablesOffset = sw.Position;
            //         xyz(sw, 0);
            //         xyz(sw, 1);
            foreach (ClrTable cl in clrTables)
                if (cl != null)
                    cl.parseRows(sw, 0);

        }
    }
 }
