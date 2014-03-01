using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using Utils;

namespace Code
{
    public class MetadataParser : IMAGE_BASE_DATA
    {
        public static int offsetstring = 2;
        public static int offsetblob = 2;
        public static int offsetguid = 2;
        #region Private members
        #region Tables
        private AssemblyTableRow[] assemblyStruct;
        private AssemblyRefTableRow[] assemblyRefStruct;
        private CustomAttributeTableRow[] customAttributeStruct;
        private ModuleTableRow[] moduleStruct;
        private TypeDefTableRow[] typeDefStruct;
        private TypeRefTableRow[] typeRefStruct;
        private InterfaceImplTableRow[] interfaceImplStruct;
        private MethodTableRow[] methodStruct;
        private StandAloneSigTableRow[] standAloneSigStruct;
        private MemberRefTableRow[] memberRefStruct;
        private TypeSpecTableRow[] typeSpecStruct;
        private ParamTableRow[] paramStruct;
        private FieldTableRow[] fieldStruct;
        private FieldMarshalTableRow[] fieldMarshalStruct;
        private FieldRVATableRow[] fieldRVAStruct;
        private FieldLayoutTableRow[] fieldLayoutStruct;
        private ConstantsTableRow[] constantsStruct;
        private PropertyMapTableRow[] propertyMapStruct;
        private PropertyTableRow[] propertyStruct;
        private MethodSemanticsTableRow[] methodSemanticsStruct;
        private EventTableRow[] eventStruct;
        private EventMapTableRow[] eventMapStruct;
        private FileTableRow[] fileStruct;
        private ModuleRefTableRow[] moduleRefStruct;
        private ManifestResourceTableRow[] manifestResourceStruct;
        private ClassLayoutTableRow[] classLayoutStruct;
        private MethodImpTableRow[] methodImpStruct;
        private NestedClassTableRow[] nestedClassStruct;
        private ExportedTypeTableRow[] exportedTypeStruct;
        private ImplMapTableRow[] implMapStruct;
        private MethodSpecTableRow[] methodSpecStruct;
        private DeclSecurityTableRow[] declSecurityStruct;
        #endregion
        BinaryFileReader sw;
        long startOfMetadata;
        int[] rows;
        long valid;
        byte[] strings;
        byte[] us;
        byte[] guid;
        byte[] blob;
        int[] sizes;
        private string signature;
        private int majorVersion;
        private int minorVersion;
        private short flags;
        private string str;
        private List<string> userStrings;
        private List<Guid> guids;
        private ClrStream[] clrstreams;
        #endregion
        #region Properties
        public AssemblyTableRow[] AssemblyStruct
        {
            get { return assemblyStruct; }
            set { assemblyStruct = value; }
        }
        public AssemblyRefTableRow[] AssemblyRefStruct
        {
            get { return assemblyRefStruct; }
            set { assemblyRefStruct = value; }
        }
        public CustomAttributeTableRow[] CustomAttributeStruct
        {
            get { return customAttributeStruct; }
            set { customAttributeStruct = value; }
        }
        public ModuleTableRow[] ModuleStruct
        {
            get { return moduleStruct; }
            set { moduleStruct = value; }
        }
        public TypeDefTableRow[] TypeDefStruct
        {
            get { return typeDefStruct; }
            set { typeDefStruct = value; }
        }
        public TypeRefTableRow[] TypeRefStruct
        {
            get { return typeRefStruct; }
            set { typeRefStruct = value; }
        }
        public InterfaceImplTableRow[] InterfaceImplStruct
        {
            get { return interfaceImplStruct; }
            set { interfaceImplStruct = value; }
        }
        public MethodTableRow[] MethodStruct
        {
            get { return methodStruct; }
            set { methodStruct = value; }
        }
        public StandAloneSigTableRow[] StandAloneSigStruct
        {
            get { return standAloneSigStruct; }
            set { standAloneSigStruct = value; }
        }
        public MemberRefTableRow[] MemberRefStruct
        {
            get { return memberRefStruct; }
            set { memberRefStruct = value; }
        }
        public TypeSpecTableRow[] TypeSpecStruct
        {
            get { return typeSpecStruct; }
            set { typeSpecStruct = value; }
        }
        public ParamTableRow[] ParamStruct
        {
            get { return paramStruct; }
            set { paramStruct = value; }
        }
        public FieldTableRow[] FieldStruct
        {
            get { return fieldStruct; }
            set { fieldStruct = value; }
        }
        public FieldMarshalTableRow[] FieldMarshalStruct
        {
            get { return fieldMarshalStruct; }
            set { fieldMarshalStruct = value; }
        }
        public FieldRVATableRow[] FieldRVAStruct
        {
            get { return fieldRVAStruct; }
            set { fieldRVAStruct = value; }
        }
        public FieldLayoutTableRow[] FieldLayoutStruct
        {
            get { return fieldLayoutStruct; }
            set { fieldLayoutStruct = value; }
        }
        public ConstantsTableRow[] ConstantsStruct
        {
            get { return constantsStruct; }
            set { constantsStruct = value; }
        }
        public PropertyMapTableRow[] PropertyMapStruct
        {
            get { return propertyMapStruct; }
            set { propertyMapStruct = value; }
        }
        public PropertyTableRow[] PropertyStruct
        {
            get { return propertyStruct; }
            set { propertyStruct = value; }
        }
        public MethodSemanticsTableRow[] MethodSemanticsStruct
        {
            get { return methodSemanticsStruct; }
            set { methodSemanticsStruct = value; }
        }
        public EventTableRow[] EventStruct
        {
            get { return eventStruct; }
            set { eventStruct = value; }
        }
        public EventMapTableRow[] EventMapStruct
        {
            get { return eventMapStruct; }
            set { eventMapStruct = value; }
        }
        public FileTableRow[] FileStruct
        {
            get { return fileStruct; }
            set { fileStruct = value; }
        }
        public ModuleRefTableRow[] ModuleRefStruct
        {
            get { return moduleRefStruct; }
            set { moduleRefStruct = value; }
        }
        public ManifestResourceTableRow[] ManifestResourceStruct
        {
            get { return manifestResourceStruct; }
            set { manifestResourceStruct = value; }
        }
        public ClassLayoutTableRow[] ClassLayoutStruct
        {
            get { return classLayoutStruct; }
            set { classLayoutStruct = value; }
        }
        public MethodImpTableRow[] MethodImpStruct
        {
            get { return methodImpStruct; }
            set { methodImpStruct = value; }
        }
        public NestedClassTableRow[] NestedClassStruct
        {
            get { return nestedClassStruct; }
            set { nestedClassStruct = value; }
        }
        public ExportedTypeTableRow[] ExportedTypeStruct
        {
            get { return exportedTypeStruct; }
            set { exportedTypeStruct = value; }
        }
        public DeclSecurityTableRow[] DeclSecurityStruct
        {
            get { return declSecurityStruct; }
            set { declSecurityStruct = value; }
        }
        public ImplMapTableRow[] ImplMapStruct
        {
            get { return implMapStruct; }
            set { implMapStruct = value; }
        }
        public MethodSpecTableRow[] MethodSpecStruct
        {
            get { return methodSpecStruct; }
            set { methodSpecStruct = value; }
        }
        public string VersionString
        {
            get { return str; }
            set { str = value; }
        }
        public List<string> UserStrings
        {
            get { return userStrings; }
            set { userStrings = value; }
        }
        public List<Guid> Guids
        {
            get { return guids; }
            set { guids = value; }
        }
        public ClrStream[] Clrstreams
        {
            get { return clrstreams; }
            set { clrstreams = value; }
        }
        public string Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        public int MajorVersion
        {
            get { return majorVersion; }
            set { majorVersion = value; }
        }
        public int MinorVersion
        {
            get { return minorVersion; }
            set { minorVersion = value; }
        }
        #endregion
        public MetadataParser(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            this.sw = sw;
            ReadStreamsData();
            FillTableSizes();
            ReadTablesIntoStructures();
            DisplayTableForDebugging();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        #region Main methods
        private void ReadStreamsData()
        {
            startOfMetadata = sw.Position;
            signature = sw.ReadString(4);
            majorVersion = sw.ReadShort();
            minorVersion = sw.ReadShort();
            int reserved = sw.ReadInteger();
            int lengthofstring = sw.ReadInteger();
            str = sw.ReadString(lengthofstring);
            int leftover = lengthofstring % 4;
            if (leftover > 0)
                sw.ReadBytes(leftover);
            flags = sw.ReadShort();// Reserved ??
            int streams = sw.ReadShort();
            clrstreams = new ClrStream[5];
            for (int i = 0; i < streams; i++)
            {
                clrstreams[i] = new ClrStream(sw, startOfMetadata);
            }
            for (int i = 0; i < streams; i++)
            {
                sw.Position = clrstreams[i].offset;
                if (clrstreams[i].Name == "#Strings")
                {
                    strings = sw.ReadBytes(clrstreams[i].size);
                }
                if (clrstreams[i].Name == "#US")
                {
                    #region User strings
                    us = sw.ReadBytes(clrstreams[i].size);
                    sw.Position = clrstreams[i].offset;
                    userStrings = new List<string>();
                    int ind = sw.ReadShort();
                    sw.ReadByte();
                    while (sw.Position < clrstreams[i].offset + clrstreams[i].size)
                    {
                        int length = sw.ReadByte();
                        if ((length & 0x80) == 0x80)
                            length = (length & 0x7F) * 256 + sw.ReadByte();
                        if (length > 1)
                        {
                            byte[] a = sw.ReadBytes(length - 1);
                            userStrings.Add(Encoding.Unicode.GetString(a));
                        }
                    }
                    #endregion
                }
                if (clrstreams[i].Name == "#GUID")
                {
                    #region Guids
                    guid = sw.ReadBytes(clrstreams[i].size);
                    if (guid.Length > 0)
                    {
                        guids = new List<Guid>();
                        BitStreamReader ms = new BitStreamReader(guid, true);
                        while (ms.Position < ms.Length)
                            guids.Add(new Guid(ms.ReadBytes(0x10)));
                    }
                    #endregion
                }
                if (clrstreams[i].Name == "#Blob")
                {
                    blob = sw.ReadBytes(clrstreams[i].size);
                }
            }
            #region handling #~ stream metadata
            sw.Position = clrstreams[0].offset;
            int res = sw.ReadInteger();
            int majorS = sw.ReadByte();
            int minorS = sw.ReadByte();
            int heapsizes = sw.ReadByte();// metadata[6];
            if ((heapsizes & 0x01) == 0x01)
                offsetstring = 4;
            if ((heapsizes & 0x02) == 0x02)
                offsetguid = 4;
            if ((heapsizes & 0x04) == 0x04)
                offsetblob = 4;
            sw.ReadByte();
            valid = sw.ReadLongInteger();
            long sorted = sw.ReadLongInteger();
            rows = new int[64];
            Array.Clear(rows, 0, rows.Length);
            for (int k = 0; k <= 63; k++)
            {
                int tablepresent = (int)(valid >> k) & 1;
                if (tablepresent == 1)
                {
                    rows[k] = sw.ReadInteger();
                }
            }
            #endregion
        }
        private void FillTableSizes()
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
            int methodspecsize = 2;
            sizes = new int[] { modulesize, typerefsize, typedefsize, 2, fieldsize, 2, methodsize, 2, paramsize, 
                interfaceimplsize, memberrefsize, constantsize, customattributesize, fieldmarshallsize, 
                declsecuritysize, classlayoutsize, fieldlayoutsize, stanalonssigsize, eventmapsize, 2, eventsize, 
                propertymapsize, 2, propertysize, methodsemantics, methodimplsize, modulerefsize, typespecsize, 
                implmapsize, fieldrvasize, 2, 2, assemblysize, 4, 12, assemblyrefsize, 6, 14, filesize, exportedtype, 
                manifestresourcesize, nestedclasssize, methodspecsize };
        }
        private void ReadTablesIntoStructures()
        {
            #region Read base userStrings
            for (int i = 0; i < 0x2c; i++)
            {
                if ((((valid >> i) & 1) == 1))// Check if table is present
                {
                    switch (i)
                    {
                        #region Module 0x00
                        case 0:
                            moduleStruct = new ModuleTableRow[rows[0] + 1];
                            for (int k = 1; k <= rows[0]; k++)
                            {
                                moduleStruct[k] = new ModuleTableRow(sw);
                                moduleStruct[k].Name = GetString(moduleStruct[k].name);
                                long p = sw.Position;
                                sw.Position = clrstreams[3].offset + moduleStruct[k].mvid;
                                moduleStruct[k].Mvid = new Guid(sw.ReadBytes(16));
                                sw.Position = clrstreams[3].offset + moduleStruct[k].encId;
                                moduleStruct[k].EncId = new Guid(sw.ReadBytes(16));
                                sw.Position = clrstreams[3].offset + moduleStruct[k].encBaseId;
                                moduleStruct[k].EncBaseId = new Guid(sw.ReadBytes(16));
                                sw.Position = p;
                            }
                            break;
                        #endregion
                        #region TypeRef 0x01
                        case 1:
                            typeRefStruct = new TypeRefTableRow[rows[1] + 1];
                            for (int k = 1; k <= rows[1]; k++)
                            {
                                typeRefStruct[k] = new TypeRefTableRow();
                                typeRefStruct[k].resolutionscope = ReadCodedIndex(sw, "ResolutionScope");
                                typeRefStruct[k].name = sw.ReadIndex(offsetstring);
                                typeRefStruct[k].nspace = sw.ReadIndex(offsetstring);

                                typeRefStruct[k].Name = GetString(+typeRefStruct[k].name);
                                typeRefStruct[k].NameSpace = GetString(+typeRefStruct[k].nspace);

                            }
                            break;
                        #endregion
                        #region TypeDef 0x02
                        case 2:
                            typeDefStruct = new TypeDefTableRow[rows[2] + 1];
                            for (int k = 1; k <= rows[2]; k++)
                            {
                                typeDefStruct[k] = new TypeDefTableRow(sw);
                                typeDefStruct[k].cindex = ReadCodedIndex(sw, "TypeDefOrRef");
                                typeDefStruct[k].findex = ReadTableIndex(sw);
                                typeDefStruct[k].mindex = ReadTableIndex(sw);

                                typeDefStruct[k].Name = GetString(+typeDefStruct[k].nameAddr);
                                typeDefStruct[k].NameSpace = GetString(+typeDefStruct[k].nspace);

                            }
                            break;
                        #endregion
                        #region Field 0x04
                        case 4:
                            fieldStruct = new FieldTableRow[rows[4] + 1];
                            for (int k = 1; k <= rows[4]; k++)
                            {
                                fieldStruct[k] = new FieldTableRow(sw);
 
                                fieldStruct[k].Name = GetString(fieldStruct[k].nameAddr);
                            }
                            break;
                        #endregion
                        #region  Method 0x06
                        case 6:
                            methodStruct = new MethodTableRow[rows[6] + 1];
                            for (int k = 1; k <= rows[6]; k++)
                            {
                                methodStruct[k] = new MethodTableRow(sw);
 
                                methodStruct[k].param = ReadTableIndex(sw);
                                methodStruct[k].Name = GetString(+methodStruct[k].name);
                            }
                            break;
                        #endregion
                        #region Param 0x08
                        case 8:
                            paramStruct = new ParamTableRow[rows[8] + 1];
                            for (int k = 1; k <= rows[8]; k++)
                            {
                                paramStruct[k] = new ParamTableRow(sw);
                                paramStruct[k].Name = GetString(+paramStruct[k].nameAdd);
                            }
                            break;
                        #endregion
                        #region InterfaceImpl 0x09
                        case 9:
                            interfaceImplStruct = new InterfaceImplTableRow[rows[9] + 1];
                            for (int k = 1; k <= rows[9]; k++)
                            {
                                interfaceImplStruct[k] = new InterfaceImplTableRow();
                                interfaceImplStruct[k].classindex = ReadCodedIndex(sw, "TypeDefOrRef");
                                interfaceImplStruct[k].interfaceindex = ReadTableIndex(sw);
                            }
                            break;
                        #endregion
                        #region MemberRef 0x0A
                        case 10:
                            memberRefStruct = new MemberRefTableRow[rows[10] + 1];
                            for (int k = 1; k <= rows[10]; k++)
                            {
                                memberRefStruct[k] = new MemberRefTableRow();
                                memberRefStruct[k].clas = ReadCodedIndex(sw, "MemberRefParent");
                                memberRefStruct[k].nameAdd = sw.ReadIndex(offsetstring);

                                memberRefStruct[k].sig = sw.ReadIndex(offsetblob); ;
                                memberRefStruct[k].Name = GetString(+memberRefStruct[k].nameAdd);

                            }
                            break;
                        #endregion
                        #region Constants 0x0B
                        case 11:
                            constantsStruct = new ConstantsTableRow[rows[11] + 1];
                            for (int k = 1; k <= rows[11]; k++)
                            {
                                constantsStruct[k] = new ConstantsTableRow(sw);
                                constantsStruct[k].parent = ReadCodedIndex(sw, "HasConst");
                                constantsStruct[k].value = sw.ReadIndex(offsetblob); ;
                            }
                            break;
                        #endregion
                        #region CustomAttribute 0x0C
                        case 12:
                            customAttributeStruct = new CustomAttributeTableRow[rows[12] + 1];
                            for (int k = 1; k <= rows[12]; k++)
                            {
                                customAttributeStruct[k] = new CustomAttributeTableRow();
                                customAttributeStruct[k].parent = ReadCodedIndex(sw, "HasCustomAttribute");
                                customAttributeStruct[k].type = ReadCodedIndex(sw, "HasCustomAttributeType");
                                customAttributeStruct[k].value = sw.ReadIndex(offsetblob); ;

                            }
                            break;
                        #endregion
                        #region  FieldMarshal 0x0D
                        case 13:
                            fieldMarshalStruct = new FieldMarshalTableRow[rows[13] + 1];
                            for (int k = 1; k <= rows[13]; k++)
                            {
                                fieldMarshalStruct[k] = new FieldMarshalTableRow();
                                fieldMarshalStruct[k].coded = ReadCodedIndex(sw, "HasFieldMarshal");
                                fieldMarshalStruct[k].index = sw.ReadIndex(offsetblob); ;

                            }
                            break;
                        #endregion
                        #region DeclSecurity 0x0E
                        case 14:
                            declSecurityStruct = new DeclSecurityTableRow[rows[14] + 1];
                            for (int k = 1; k <= rows[14]; k++)
                            {
                                declSecurityStruct[k] = new DeclSecurityTableRow();
                                declSecurityStruct[k].action = sw.ReadShort();
                                declSecurityStruct[k].coded = ReadCodedIndex(sw, "HasDeclSecurity");
                                declSecurityStruct[k].bindex = sw.ReadIndex(offsetblob); ;

                            }
                            break;
                        #endregion
                        #region ClassLayout 0x0F
                        case 15:
                            classLayoutStruct = new ClassLayoutTableRow[rows[15] + 1];
                            for (int k = 1; k <= rows[15]; k++)
                            {
                                classLayoutStruct[k] = new ClassLayoutTableRow();
                                classLayoutStruct[k].packingsize = sw.ReadShort();
                                classLayoutStruct[k].classsize = sw.ReadInteger();
                                classLayoutStruct[k].parent = ReadTableIndex(sw);

                            }
                            break;
                        #endregion
                        #region FieldLayout 0x10
                        case 16:
                            fieldLayoutStruct = new FieldLayoutTableRow[rows[16] + 1];
                            for (int k = 1; k <= rows[16]; k++)
                            {
                                fieldLayoutStruct[k] = new FieldLayoutTableRow(sw);
                                fieldLayoutStruct[k].fieldindex = ReadTableIndex(sw);

                            }
                            break;
                        #endregion
                        #region StandAloneSig 0x11
                        case 17:
                            standAloneSigStruct = new StandAloneSigTableRow[rows[17] + 1];
                            for (int k = 1; k <= rows[17]; k++)
                            {
                                standAloneSigStruct[k] = new StandAloneSigTableRow();
                                standAloneSigStruct[k].index = sw.ReadIndex(offsetblob); ;
                            }
                            break;
                        #endregion
                        #region EventMap 0x12
                        case 18:
                            eventMapStruct = new EventMapTableRow[rows[18] + 1];
                            for (int k = 1; k <= rows[18]; k++)
                            {
                                eventMapStruct[k] = new EventMapTableRow();
                                eventMapStruct[k].index = ReadTableIndex(sw);
                                eventMapStruct[k].eindex = ReadTableIndex(sw);
                            }
                            break;
                        #endregion
                        #region Event 0x14
                        case 20:
                            eventStruct = new EventTableRow[rows[20] + 1];
                            for (int k = 1; k <= rows[20]; k++)
                            {
                                eventStruct[k] = new EventTableRow();
                                eventStruct[k].attr = sw.ReadShort();
                                eventStruct[k].name = sw.ReadIndex(offsetstring);
                                eventStruct[k].coded = ReadCodedIndex(sw, "TypeDefOrRef");

                                eventStruct[k].Name = GetString(+eventStruct[k].name);
                            }
                            break;
                        #endregion
                        #region PropertyMap 0x15
                        case 21:
                            propertyMapStruct = new PropertyMapTableRow[rows[21] + 1];
                            for (int k = 1; k <= rows[21]; k++)
                            {
                                propertyMapStruct[k] = new PropertyMapTableRow();
                                propertyMapStruct[k].parent = ReadTableIndex(sw);
                                propertyMapStruct[k].propertylist = ReadTableIndex(sw);
                            }
                            break;
                        #endregion
                        #region Property 0x17
                        case 23:
                            propertyStruct = new PropertyTableRow[rows[23] + 1];
                            for (int k = 1; k <= rows[23]; k++)
                            {
                                propertyStruct[k] = new PropertyTableRow(sw);

                                propertyStruct[k].Name = GetString(propertyStruct[k].nameAddr);
                            }
                            break;
                        #endregion
                        #region MethodSemantics 0x18
                        case 24:
                            methodSemanticsStruct = new MethodSemanticsTableRow[rows[24] + 1];
                            for (int k = 1; k <= rows[24]; k++)
                            {
                                methodSemanticsStruct[k] = new MethodSemanticsTableRow(sw);
                                methodSemanticsStruct[k].methodindex = ReadTableIndex(sw);
                                methodSemanticsStruct[k].association = ReadCodedIndex(sw, "HasSemantics");
                            }
                            break;
                        #endregion
                        #region MethodImpl 0x19
                        case 25:
                            methodImpStruct = new MethodImpTableRow[rows[25] + 1];
                            for (int k = 1; k <= rows[25]; k++)
                            {
                                methodImpStruct[k] = new MethodImpTableRow();
                                methodImpStruct[k].classindex = ReadTableIndex(sw);
                                methodImpStruct[k].codedbody = ReadCodedIndex(sw, "MethodDefOrRef");
                                methodImpStruct[k].codeddef = ReadCodedIndex(sw, "MethodDefOrRef");
                            }
                            break;
                        #endregion
                        #region ModuleRef 0x1A
                        case 26:
                            moduleRefStruct = new ModuleRefTableRow[rows[26] + 1];
                            for (int k = 1; k <= rows[26]; k++)
                            {
                                moduleRefStruct[k] = new ModuleRefTableRow(sw);
                                moduleRefStruct[k].Name = GetString(moduleRefStruct[k].nameAddr);
                            }
                            break;
                        #endregion
                        #region TypeSpec 0x1B
                        case 27:
                            typeSpecStruct = new TypeSpecTableRow[rows[27] + 1];
                            for (int k = 1; k <= rows[27]; k++)
                            {
                                typeSpecStruct[k] = new TypeSpecTableRow(sw);
                            }
                            break;
                        #endregion
                        #region ImplMap 0x1C
                        case 28:
                            implMapStruct = new ImplMapTableRow[rows[28] + 1];
                            for (int k = 1; k <= rows[28]; k++)
                            {
                                implMapStruct[k] = new ImplMapTableRow(sw);
      
                                implMapStruct[k].cindex = ReadCodedIndex(sw, "MemberForwarded");
                                implMapStruct[k].name = sw.ReadIndex(offsetstring);
                                implMapStruct[k].scope = ReadTableIndex(sw);

                                implMapStruct[k].Name = GetString(+implMapStruct[k].name);
                            }
                            break;
                        #endregion
                        #region FieldRVA 0x1D
                        case 29:
                            fieldRVAStruct = new FieldRVATableRow[rows[29] + 1];
                            for (int k = 1; k <= rows[29]; k++)
                            {
                                fieldRVAStruct[k] = new FieldRVATableRow(sw);
                                fieldRVAStruct[k].fieldi = ReadTableIndex(sw);

                            }
                            break;
                        #endregion
                        #region Assembly 0x20
                        case 32:
                            assemblyStruct = new AssemblyTableRow[rows[32] + 1];
                            for (int k = 1; k <= rows[32]; k++)
                            {
                                assemblyStruct[k] = new AssemblyTableRow(sw);
                                assemblyStruct[k].Name = GetString(assemblyStruct[k].name);
                            }
                            break;
                        #endregion
                        #region AssemblyRef 0x23
                        case 35:
                            assemblyRefStruct = new AssemblyRefTableRow[rows[35] + 1];
                            for (int k = 1; k <= rows[35]; k++)
                            {
                                assemblyRefStruct[k] = new AssemblyRefTableRow(sw);
                                assemblyRefStruct[k].Name = GetString(assemblyRefStruct[k].name);
                            }
                            break;
                        #endregion
                        #region AssemblyRefProcessor : 0x24
                        #endregion
                        #region AssemblyRefOS : 0x25
                        #endregion
                        #region File 0x26
                        case 38:
                            fileStruct = new FileTableRow[rows[38] + 1];
                            for (int k = 1; k <= rows[38]; k++)
                            {
                                fileStruct[k] = new FileTableRow(sw);
 
                                fileStruct[k].Name = GetString(+fileStruct[k].name);
                            }
                            break;
                        #endregion
                        #region ExportedType 0x27
                        case 39:
                            exportedTypeStruct = new ExportedTypeTableRow[rows[39] + 1];
                            for (int k = 1; k <= rows[39]; k++)
                            {
                                exportedTypeStruct[k] = new ExportedTypeTableRow(sw);
                                exportedTypeStruct[k].coded = ReadCodedIndex(sw, "Implementation");
                                exportedTypeStruct[k].Name = GetString(+exportedTypeStruct[k].nameAd);
                                exportedTypeStruct[k].NameSpace = GetString(+exportedTypeStruct[k].nspace);
                            }
                            break;
                        #endregion
                        #region ManifestResource 0x28
                        case 40:
                            manifestResourceStruct = new ManifestResourceTableRow[rows[40] + 1];
                            for (int k = 1; k <= rows[40]; k++)
                            {
                                manifestResourceStruct[k] = new ManifestResourceTableRow();
                                manifestResourceStruct[k].offset = sw.ReadInteger();
                                manifestResourceStruct[k].flags = sw.ReadInteger();
                                manifestResourceStruct[k].name = sw.ReadIndex(offsetstring);
                                manifestResourceStruct[k].coded = ReadCodedIndex(sw, "Implementation");
                                manifestResourceStruct[k].Name = GetString(+manifestResourceStruct[k].name);
                            }
                            break;
                        #endregion
                        #region Nested Classes 0x2A
                        case 41:
                            nestedClassStruct = new NestedClassTableRow[rows[41] + 1];
                            for (int k = 1; k <= rows[41]; k++)
                            {
                                nestedClassStruct[k] = new NestedClassTableRow();
                                nestedClassStruct[k].nestedclass = ReadTableIndex(sw);
                                nestedClassStruct[k].enclosingclass = ReadTableIndex(sw);
                            }
                            break;
                        #endregion
                        #region GenericParam 0x2A
                        case 42:
                            for (int k = 1; k <= rows[i]; k++)
                            {
                                sw.ReadBytes(sizes[i]);
                            }
                            break;
                        #endregion
                        #region Method spec 0x2B
                        case 43:
                            methodSpecStruct = new MethodSpecTableRow[rows[43] + 1];
                            for (int k = 1; k <= rows[i]; k++)
                            {
                                methodSpecStruct[k] = new MethodSpecTableRow(sw);
                            }
                            break;
                        #endregion
                        #region GenericParamConstraint 0x2C
                        case 44:
                            /*       methodSpecStruct = new MethodSpecTableRow[rows[43] + 1];
                                   for (int k = 1; k <= rows[u]; k++)
                                   {
                                       methodSpecStruct[k] = new MethodSpecTableRow();
                                       methodSpecStruct[k].Method = ms.ReadInteger();
                                       methodSpecStruct[k].Instantiation = ms.ReadInteger();
                                   }*/
                            break;
                        #endregion
                        default:
                            for (int k = 1; k <= rows[i]; k++)
                            {
                                sw.ReadBytes(sizes[i]);
                            }
                            break;
                    }
                }
            }
            #endregion
        }
        #endregion
        private int GetCodedIndexSize(string i)
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
        private int ReadCodedIndex(BinaryFileReader sw, string i)
        {
            switch (GetCodedIndexSize(i))
            {
                case 2: return sw.ReadShort();
                case 4: return sw.ReadInteger();
                default: return 0;
            }
        }
        private int GetTableSize()
        {
            return 2;
        }
        #region Read indexes
        private int ReadStringIndex(byte[] a, int o)
        {
            int z = 0;
            if (offsetstring == 2)
                z = BitConverter.ToUInt16(a, o);
            if (offsetstring == 4)
                z = (int)BitConverter.ToUInt32(a, o);
            return z;
        }
        private int ReadBlobIndex(byte[] a, int o)
        {
            int z = 0;
            if (offsetblob == 2)
                z = BitConverter.ToUInt16(a, o);
            if (offsetblob == 4)
                z = (int)BitConverter.ToUInt32(a, o);
            return z;
        }
        private int ReadGuidIndex(byte[] a, int o)
        {
            int z = 0;
            if (offsetguid == 2)
                z = BitConverter.ToUInt16(a, o);
            if (offsetguid == 4)
                z = (int)BitConverter.ToUInt32(a, o);
            return z;
        }
        private int ReadTableIndex(BinaryFileReader sw)
        {
            switch (GetTableSize())
            {
                case 2: return sw.ReadShort();
                case 4: return sw.ReadInteger();
                default: return 0;
            }
        }
        #endregion
        #region Get userStrings
        private string GetString(int position)
        {
            long st = sw.Position;
            sw.Position = (int)clrstreams[1].offset + position;
            string s = sw.ReadStringZ(Encoding.Default);
            sw.Position = st;
            if (s.Length == 0)
                return "";
            else
                return s;
        }
        private int GetResolutionScopeValue(int a)
        {
            return a >> 2;
        }
        private string GetType(int b)
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
        private string GetCustomAttributeTypeTable(int a)
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
        private int CorSigUncompressData(byte[] b, int index, out int answer)
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
        private string GetResolutionScopeTable(int a)
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
        private string GetParamAttributes(short a)
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
        private string GetCallingConvention(int uncompressedbyte)
        {
            int firstbyte = uncompressedbyte;
            byte firstfourbits = (byte)(firstbyte & 0x0f);
            string s = "Calling Convention ";
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
            //     s = s + "\n";
            return s;
        }
        private string GetValueType(byte[] b, int bytes, int index, out int cb1)
        {
            int uncompressedbyte;
            int cb = CorSigUncompressData(b, index + 1, out uncompressedbyte);
            byte table = (byte)(uncompressedbyte & 0x03);
            int ind = uncompressedbyte >> 2;
            string s1 = "";
            if (table == 1)
                s1 = typeRefStruct[ind].Name;
            if (table == 0)
                s1 = typeDefStruct[ind].Name;
            cb1 = cb;
            return s1;
        }
        private string GetClassType(byte[] b, int bytes, int index, out int cb1)
        {
            int uncompressedbyte;
            int cb = CorSigUncompressData(b, index + 1, out uncompressedbyte);
            byte table = (byte)(uncompressedbyte & 0x03);
            int ind = uncompressedbyte >> 2;
            string s1 = "";
            if (table == 1)
                s1 = typeRefStruct[ind].Name;
            if (table == 0)
                s1 = typeDefStruct[ind].Name;
            cb1 = cb;
            return s1;
        }
        private string GetElemType(int index, byte[] b, out int cb1)
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
        private string GetTokenType(byte[] b, int index, out int cb1)
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
        private string DecodeToken(int token)
        {
            byte table = (byte)(token & 0x03);
            int ind = token >> 2;
            string s1 = "";
            if (table == 0)
                s1 = GetString((int)clrstreams[1].offset + typeDefStruct[ind].nameAddr);
            if (table == 1)
                s1 = GetString(+typeRefStruct[ind].name);
            return s1;
        }
        private string GetPointerType(byte[] b, int index, out int cb1)
        {
            string s = "";
            s = GetElemType(index, b, out cb1) + " *";
            return s;
        }
        private string GetArrayType(byte[] b, int index, out int cb1)
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
        private string GetMemberRefParentTable(int a)
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
        private int GetMemberRefParentValue(int a)
        {
            return a >> 3;
        }
        private int GetHasConstValue(int a)
        {
            return a >> 2;
        }
        private string GetHasConstTable(int a)
        {
            string s = "";
            int tag = a & 0x03;
            if (tag == 0)
                s = s + "FieldDef";
            if (tag == 1)
                s = s + "ParamDef";
            if (tag == 2)
                s = s + "Property";
            return s;
        }
        private long GetBytesFromArray(int value, int dtype)
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
        private List<char> GetStringFromBlobArray(int value)
        {
            int count = blob[value];
            List<char> b = new List<char>();
            for (int i = 1; i <= count; i = i + 2)
                b.Add((char)blob[value + i]);
            return b;
        }
        private int GetCustomAttributeTypevalue(int a)
        {
            return a >> 3;
        }
        private int GetHasCustomAttributeValue(int a)
        {
            return a >> 5;
        }
        private string GetHasCustomAttributeTable(int a)
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
        private List<string> GetCustomAttributeBlob(int index)
        {
            int count = blob[index];
            List<string> bl = new List<string>();
            for (int l = 1; l <= count; l++)
            {
                bl.Add(blob[index + l].ToString("X"));
            }
            return bl;
        }
        private string GetFieldMarshalTable(int a)
        {
            string s = "";
            short tag = (short)(a & (short)0x01);
            if (tag == 0)
                s = s + "Field";
            if (tag == 1)
                s = s + "Param";
            return s;
        }
        private int GetFieldMarshalValue(int a)
        {
            return a >> 1;
        }
        private string GetMarshallType(byte a)
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
        private string GetDeclSecurityTable(int a)
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
        private int GetDeclSecurityValue(int a)
        {
            return a >> 2;
        }
        private string GetTypeRefTable(int index)
        {
            int name = typeRefStruct[index].name;
            int nspace = typeRefStruct[index].nspace;
            string s = GetString(+nspace);
            if (s != "null")
                s = s + ".";
            else
                s = "";
            s = s + GetString(+name);
            return s;
        }
        private string GetTypeDefOrRefTable(int a)
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
        private int GetTypeDefOrRefValue(int a)
        {
            return a >> 2;
        }
        private string GetTypeDefTable(int index)
        {
            int name = typeDefStruct[index].nameAddr;
            int nspace = typeDefStruct[index].nspace;
            int cindex = typeDefStruct[index].cindex;
            int typereftableindex = GetTypeDefOrRefValue(cindex);
            string tablename = GetTypeDefOrRefTable(cindex);
            string s1 = "";
            if (tablename == "TypeRef")
                s1 = " Extends " + GetTypeRefTable(typereftableindex);
            if (GetString(+nspace) != "")
                s1 = s1 + " Namespace: " + GetString(+nspace);
            string s = "Class " + GetString(+name) + s1;
            return s;
        }
        private string GetEventTable(int index)
        {
            string s = "";
            int ind = eventStruct[index].name;
            s = GetString(+ind);
            int coded = eventStruct[index].coded;
            string tablename = GetTypeDefOrRefTable(coded);
            int ind1 = GetTypeDefOrRefValue(coded);
            string s1 = DisplayTable(tablename, ind1);
            s = "Event " + s + " " + s1 + " ";
            return s;
        }
        private string GetEventsAttributes(int a)
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
        private string GetPropertyTable(int index)
        {
            string s = "";
            int ind = propertyStruct[index].nameAddr;
            s = GetString(+ind);
            return s;
        }
        private List<string> GetMethodSemanticsAttributes(short a)
        {
            List<string> s = new List<string>();
            if ((a & 0x01) == 0x01)
                s.Add("Setter");
            if ((a & 0x02) == 0x02)
                s.Add("Getter");
            if ((a & 0x04) == 0x04)
                s.Add("Other");
            if ((a & 0x08) == 0x08)
                s.Add("Event Addon");
            if ((a & 0x10) == 0x10)
                s.Add("Event Remove");
            if ((a & 0x20) == 0x20)
                s.Add("Event Fire");
            return s;
        }
        private int GetMethodDefValue(int a)
        {
            return a >> 1;
        }
        private string GetMethodDefTable(int a)
        {
            string s = "";
            short tag = (short)(a & (short)0x01);
            if (tag == 0)
                s = "MethodDef";
            if (tag == 1)
                s = "MethodRef";
            return s;
        }
        private string GetMethodTable(int index)
        {
            string s = "";
            int ind = methodStruct[index].name;
            s = GetString(+ind);
            return s;
        }
        private int GetHasSemanticsValue(int a)
        {
            return a >> 1;
        }
        private int GetMemberForwardedValue(int a)
        {
            return a >> 1;
        }
        private int GetImplementationValue(int a)
        {
            return a >> 2;
        }
        private int GetManifestResourceValue(int a)
        {
            return a >> 2;
        }
        private string GetHasSemanticsTable(int a)
        {
            string s = "";
            int tag = a & 0x01;
            if (tag == 0)
                s = s + "Event";
            if (tag == 1)
                s = s + "Property";
            return s;
        }
        private string GetPInvokeAttributes(short a)
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
        private string GetMemberForwardedTable(int a)
        {
            string s = "";
            int tag = a & 0x01;
            if (tag == 0)
                s = "Field";
            if (tag == 1)
                s = "MethodDef";
            return s;
        }
        private string GetModuleRefTable(int index)
        {
            string s = "";
            int ind = moduleRefStruct[index].nameAddr;
            s = GetString(+ind);
            return s;
        }
        private string GetFieldTable(int index)
        {
            string s = "";
            int ind = fieldStruct[index].nameAddr;
            s = GetString(+ind);
            return s;
        }
        private string GetAssemblyTable(int index)
        {
            int ind = assemblyStruct[index].name;
            return GetString(+ind);
        }
        private string GetAssemblyRefTable(int index)
        {
            int ind = assemblyRefStruct[index].name;
            return GetString(+ind);
        }
        private string GetImplementationTable(int a)
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
        private string GetManifestResourceAttributes(int a)
        {
            string s = "";
            if ((a & 0x007) == 0x007)
                s = s + "VisibilityMask ";
            if ((a & 0x001) == 0x001)
                s = s + "private ";
            if ((a & 0x002) == 0x002)
                s = s + "Private ";
            return s;
        }
        private string GetManifestResourceTable(int a)
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
        private string GetModuleTable(int index)
        {
            string s = "";
            int ind = moduleStruct[index].name;
            s = GetString(+ind);
            return s;
        }
        private string GetParamTable(int index)
        {
            string s = "";
            int ind = paramStruct[index].nameAdd;
            s = GetString(+ind);
            return s;
        }
        private string GetMemberRefTable(int index)
        {
            string s = "";
            int ind = memberRefStruct[index].nameAdd;
            s = GetString(+ind);
            return s;
        }
        private string GetFileTable(int index)
        {
            string s = "";
            int ind = fileStruct[index].name;
            s = GetString(+ind);
            return s;
        }
        #endregion
        #region Display functions
        private string DisplayFieldSignature(int index)
        {
            string s = "";
            int count = blob[index];
            for (int l = 1; l <= count; l++)
                s = s + blob[index + l].ToString("x2") + " ";
            if (blob[index + 1] == 0x06)
            {
                s = s + ".." + GetType(blob[index + 2]);
            }
            return s;
        }
        private string DisplayStandAloneSigSignature(int index)
        {
            string s = "";
            int count = blob[index];
            s = s + count.ToString() + " ";
            for (int l = 1; l <= count; l++)
                s = s + blob[index + l].ToString() + " ";
            return s;
        }
        private List<string> DisplayEventsList(int start, int rowindex)
        {
            List<string> s = new List<string>();
            int end = eventStruct.Length;
            int end1 = 10000;
            if (rowindex <= eventMapStruct.Length)
                end1 = eventMapStruct[rowindex].eindex;
            int end2 = 0;
            if (end <= end1)
                end2 = end;
            else
                end2 = end1;
            for (int i = start; i <= end2; i++)
                s.Add(GetEventTable(i));
            return s;
        }
        private List<string> DisplayPropertiesList(int start, int rowindex)
        {
            List<string> s = new List<string>();
            int end = propertyStruct.Length;
            int end1 = 10000;
            if (rowindex <= propertyMapStruct.Length)
                end1 = propertyMapStruct[rowindex].propertylist;
            int end2 = 0;
            if (end <= end1)
                end2 = end;
            else
                end2 = end1;
            for (int i = start; i <= end2; i++)
                s.Add(GetPropertyTable(i));
            return s;
        }
        private string DisplayPropertySignature(int index)
        {
            string s = "";
            int count = blob[index];
            s = s + count.ToString() + " ";
            for (int l = 1; l <= count; l++)
                s = s + blob[index + l].ToString() + " ";
            return s;
        }
        private string DisplayTable(string tablename, int index)
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
        public string DisplayMethodSignature(int index, int param, string name)
        {
            string s = "";
            /*          int count = blob[index];
                     s = s + count.ToString() + " ";
                     for (int l = 1; l <= count; l++)
                         s = s + blob[index + l].ToString() + " ";
                     byte[] blob1 = new byte[count];
                     Array.Copy(blob, index + 1, blob1, 0, count);
                     byte firstfourbits = (byte)(blob1[0] & 0x0f);
                     if (firstfourbits == 0x00)
                         s = s + " DEFAULT ";
                     if (firstfourbits == 0x05)
                         s = s + " VARARG ";
                     if ((blob1[0] & 0x20) == 0x20)
                         s = s + " HASTHIS ";
                     if ((blob1[0] & 0x40) == 0x40)
                         s = s + " EXPLICIT ";
                     int paramcount = blob1[1];
                     s = s + "Count " + paramcount.ToString() + " ";
                     s = s + " " + GetType(blob1[2]) + " ";
                     s = s + addressOfname + "(";
                     for (int k = 1; k <= paramcount; k++)
                     {
                         s = s + " " + GetType(blob1[2 + k]) + " " + paramnames[k];
                         if (k != paramcount)
                             s = s + ",";
                     }
                     s = s + ")";*/
            return s;
        }
        private List<string> DisplayMethodSignature(int index1, int row)
        {
            if (!(index1 >= 0 && index1 < blob.Length))
                return null;
            int cb, uncompressedbyte, count;
            cb = CorSigUncompressData(blob, index1, out uncompressedbyte);
            count = uncompressedbyte;
            byte[] b = new byte[count];
            Array.Copy(blob, index1 + cb, b, 0, count);
            if (b[2] == 0x20)
                return null; // modopt not implemented
            if (b[2] == 0x1B)
                return null;// Pointer To function not implemented
            List<string> signature = new List<string>();
            int index = 0;
            signature.Add(GetCallingConvention(b[0]));
            signature.Add(GetType(b[2]));
            cb = CorSigUncompressData(b, index, out uncompressedbyte);
            index++;
            cb = CorSigUncompressData(b, index, out uncompressedbyte);
            count = uncompressedbyte;
            index = index + cb;
            string ss11 = "";
            ss11 = GetElemType(index, b, out cb) + " " + ss11;
            index = index + cb;
            int paramindex = methodStruct[row].param;
            string ss = "(", s1;
            string ss2 = "";
            int emptyparam = 0;
            if (paramStruct != null && paramindex < paramStruct.Length &&
            paramStruct[paramindex].sequence == 0)
                emptyparam++;
            for (int l = 1; l <= count; l++)
            {
                if (b[index] == 0x2d)
                    break;
                s1 = GetElemType(index, b, out cb);
                if (s1 != null)
                {
                    if (s1.Length > 0)
                    {
                        while (s1[0] == '[' && s1[1] == ']')
                        {
                            string ss1 = s1.Substring(2);
                            ss1 = ss1 + "[]";
                            s1 = ss1;
                        }
                        index = index + cb;
                        ss2 = ss2 + s1;
                    }
                }
                if (l != count)
                    ss2 = ss2 + ",";
                int ind = paramindex + l + emptyparam - 1;
                ss = ss + s1 + " ";
                if (ind < paramStruct.Length)
                    ss += GetString(paramStruct[ind].nameAdd);
                if (l != count)
                    ss = ss + " , ";
            }
            signature.Add(ss + ")");
            return signature;
        }
        private List<MethodTableRow> DisplayAllMethods(int i)
        {
            int start, startofnext = 0;
            List<MethodTableRow> Methods = null;
            start = typeDefStruct[i].mindex;
            if (i == (typeDefStruct.Length - 1))
            {
                startofnext = methodStruct.Length;
            }
            else
                startofnext = typeDefStruct[i + 1].mindex;
            if (start < startofnext)
            {
                Methods = new List<MethodTableRow>();
                for (int j = start; j < startofnext; j++)
                {
                    Methods.Add(methodStruct[j]);
                }
            }
            return Methods;
        }
        private List<FieldTableRow> DisplayAllFields(int i)
        {
            if (fieldStruct == null)
                return null;
            List<FieldTableRow> data = null;
            int start, startofnext = 0;
            start = typeDefStruct[i].findex;
            if (i == (typeDefStruct.Length - 1))
            {
                startofnext = fieldStruct.Length;
            }
            else
                startofnext = typeDefStruct[i + 1].findex;
            if (start < startofnext)
            {
                data = new List<FieldTableRow>();
                for (int j = start; j < startofnext; j++)
                {
                    data.Add(fieldStruct[j]);
                }
            }
            return data;
        }
        private List<ParamTableRow> DisplayAllParams(int i)
        {
            if (paramStruct == null)
                return null;
            List<ParamTableRow> data = new List<ParamTableRow>();
            int start, startofnext = 0;
            start = methodStruct[i].param;
            if (i == (methodStruct.Length - 1))
            {
                startofnext = paramStruct.Length;
            }
            else
                startofnext = methodStruct[i + 1].param;
            for (int j = start; j < startofnext; j++)
            {
                data.Add(paramStruct[j]);
            }
            return data;
        }
        private List<EventTableRow> DisplayAllEvents(int i)
        {
            int ii;
            if (eventMapStruct == null)
                return null;
            List<EventTableRow> data = new List<EventTableRow>();
            for (ii = 1; ii < eventMapStruct.Length; ii++)
            {
                if (i == (eventMapStruct[ii].index))
                    break;
            }
            if (ii == eventMapStruct.Length)
                return null;
            int start = eventMapStruct[ii].eindex;
            int end;
            if (ii == eventMapStruct.Length - 1)
                end = eventStruct.Length - 1;
            else
                end = eventMapStruct[ii + 1].eindex - 1;
            for (int jj = start; jj <= end; jj++)
            {
                data.Add(eventStruct[jj]);
            }
            return data;
        }
        private List<PropertyTableRow> DisplayAllProperties(int i)
        {
            int ii;
            if (propertyMapStruct == null || propertyMapStruct.Length == 1)
                return null;
            List<PropertyTableRow> data = new List<PropertyTableRow>();
            for (ii = 1; ii < propertyMapStruct.Length; ii++)
            {
                if (i == (propertyMapStruct[ii].parent))
                    break;
            }
            if (ii == propertyMapStruct.Length)
                return null;
            int start = propertyMapStruct[ii].propertylist;
            int end;
            if (ii + 1 == propertyMapStruct.Length)
                end = propertyStruct.Length - 1;
            else
                end = propertyMapStruct[ii + 1].propertylist - 1;
            for (int jj = start; jj <= end; jj++)
            {
                data.Add(propertyStruct[jj]);
            }
            return data;
        }
        private void DisplayTypeRefTable()
        {
            if (typeRefStruct != null)
            {
                for (int ii = 1; ii <= typeRefStruct.Length - 1; ii++)
                {
                    typeRefStruct[ii].tableName = GetResolutionScopeTable(typeRefStruct[ii].resolutionscope);
                    int index = GetResolutionScopeValue(typeRefStruct[ii].resolutionscope);
                    typeRefStruct[ii].s = DisplayTable(typeRefStruct[ii].tableName, index);
                }
            }
        }
        private void DisplayTypeDefTable()
        {
            if (typeDefStruct != null)
            {
                for (int ii = 1; ii <= typeDefStruct.Length - 1; ii++)
                {
                    if (typeDefStruct[ii].Name.Contains("Dyn"))
                    {
                    }
                    typeDefStruct[ii].tablename = GetTypeDefOrRefTable(typeDefStruct[ii].cindex);
                    int index = GetTypeDefOrRefValue(typeDefStruct[ii].cindex);
                    typeDefStruct[ii].parentClass = DisplayTable(typeDefStruct[ii].tablename, index);
                    typeDefStruct[ii].methods = DisplayAllMethods(ii);
                    typeDefStruct[ii].fields = DisplayAllFields(ii);
                    typeDefStruct[ii].events = DisplayAllEvents(ii);
                    typeDefStruct[ii].properties = DisplayAllProperties(ii);
                }
            }
        }
        private void DisplayNestedClassTable()
        {
            if (nestedClassStruct != null)
            {
                for (int ii = 1; ii <= nestedClassStruct.Length - 1; ii++)
                {
                    nestedClassStruct[ii].nestedString = GetTypeDefTable(nestedClassStruct[ii].nestedclass);
                    nestedClassStruct[ii].enclosingString = GetTypeDefTable(nestedClassStruct[ii].enclosingclass);
                }
            }
        }
        private void DisplayFieldTable()
        {
            if (fieldStruct != null)
            {
                for (int ii = 1; ii <= fieldStruct.Length - 1; ii++)
                {
                    fieldStruct[ii].FieldSignature = DisplayFieldSignature(fieldStruct[ii].sig);
                }
            }
        }
        private void DisplayMethodTable()
        {
            if (methodStruct != null)
            {
                for (int ii = 1; ii <= methodStruct.Length - 1; ii++)
                {
                    methodStruct[ii].paras = DisplayAllParams(ii);
                    try
                    {
                        List<string> sigs = DisplayMethodSignature(methodStruct[ii].signature, ii);
                        if (sigs != null)
                            methodStruct[ii].meths = sigs[1] + " " + methodStruct[ii].Name + sigs[2];
                    }
                    catch { }
                }
            }
        }
        private void DisplayInterfaceImplTable()
        {
            if (interfaceImplStruct != null)
            {
                for (int ii = 1; ii <= interfaceImplStruct.Length - 1; ii++)
                {
                    int ind = interfaceImplStruct[ii].classindex;
                    //                 Console.WriteLine("Class TypeDef[{0}]...{1}", ind, GetTypeDefTable(ind));
                    string tablename = GetTypeDefOrRefTable(interfaceImplStruct[ii].interfaceindex);
                    int index = GetTypeDefOrRefValue(interfaceImplStruct[ii].interfaceindex);
                    //                 Console.WriteLine("Interface {0}[{1}] {2}", tablename, index, DisplayTable(tablename, index));
                }
            }
        }
        private void DisplayMemberRefTable()
        {
            if (memberRefStruct != null)
            {
                for (int ii = 1; ii <= memberRefStruct.Length - 1; ii++)
                {
                    memberRefStruct[ii].s = GetMemberRefParentTable(memberRefStruct[ii].clas);
                    memberRefStruct[ii].rid = (int)GetMemberRefParentValue(memberRefStruct[ii].clas);
                }
            }
        }
        private void DisplayCustomAttributeTable()
        {
            if (customAttributeStruct != null)
            {
                for (int ii = 1; ii <= customAttributeStruct.Length - 1; ii++)
                {
                    customAttributeStruct[ii].tablename = GetHasCustomAttributeTable(customAttributeStruct[ii].parent);
                    int index = GetHasCustomAttributeValue(customAttributeStruct[ii].parent);
                    customAttributeStruct[ii].s = DisplayTable(customAttributeStruct[ii].tablename, index);
                    customAttributeStruct[ii].tablename2 = GetCustomAttributeTypeTable(customAttributeStruct[ii].type);
                    index = GetCustomAttributeTypevalue(customAttributeStruct[ii].type);
                    customAttributeStruct[ii].t = DisplayTable(customAttributeStruct[ii].tablename2, index);
                    try
                    {
                        customAttributeStruct[ii].attrBlob = GetCustomAttributeBlob(customAttributeStruct[ii].value);
                    }
                    catch { }
                    Trace.WriteLine(ii.ToString());
                }
            }
        }
        private void DisplayFieldMarshalTable()
        {
            if (fieldMarshalStruct != null)
            {
                for (int ii = 1; ii <= fieldMarshalStruct.Length - 1; ii++)
                {
                    fieldMarshalStruct[ii].tablename = GetFieldMarshalTable(fieldMarshalStruct[ii].coded);
                    int index = GetFieldMarshalValue(fieldMarshalStruct[ii].coded);
                    fieldMarshalStruct[ii].s = DisplayTable(fieldMarshalStruct[ii].tablename, index);
                    int count = blob[fieldMarshalStruct[ii].index];
                    fieldMarshalStruct[ii].blob = GetMarshallType(blob[fieldMarshalStruct[ii].index + 1]);
                }
            }
        }
        private void DisplayDeclSecurityTable()
        {
            if (declSecurityStruct != null)
            {
                for (int ii = 1; ii <= declSecurityStruct.Length - 1; ii++)
                {
                    declSecurityStruct[ii].tablename = GetDeclSecurityTable(declSecurityStruct[ii].coded);
                    int ind = GetDeclSecurityValue(declSecurityStruct[ii].coded);
                    Console.WriteLine("Parent: {0}[{1}]... {2}", declSecurityStruct[ii].tablename, ind, DisplayTable(declSecurityStruct[ii].tablename, ind));
                    int uncompressedbyte;
                    declSecurityStruct[ii].cb = CorSigUncompressData(blob, declSecurityStruct[ii].bindex, out uncompressedbyte);
                    int count = uncompressedbyte;
                    int index1 = declSecurityStruct[ii].bindex + declSecurityStruct[ii].cb;
                    declSecurityStruct[ii].blob = new List<char>();
                    for (int l = 0; l < count; l++)
                        declSecurityStruct[ii].blob.Add((char)blob[index1 + l]);
                }
                Console.WriteLine("");
            }
        }
        private void DisplayClassLayoutTable()
        {
            if (classLayoutStruct != null)
            {
                for (int ii = 1; ii <= classLayoutStruct.Length - 1; ii++)
                {
                    classLayoutStruct[ii].s = GetTypeDefTable(classLayoutStruct[ii].parent);
                }
            }
        }
        private void DisplayFieldLayoutTable()
        {
            if (fieldLayoutStruct != null)
            {
                for (int ii = 1; ii <= fieldLayoutStruct.Length - 1; ii++)
                {
                    fieldLayoutStruct[ii].s = GetFieldTable(fieldLayoutStruct[ii].fieldindex);
                }
            }
        }
        private void DisplayStandAloneSigTable()
        {
            if (standAloneSigStruct != null)
            {
                for (int ii = 1; ii <= standAloneSigStruct.Length - 1; ii++)
                {
                    standAloneSigStruct[ii].s = DisplayStandAloneSigSignature(standAloneSigStruct[ii].index);
                }
            }
        }
        private void DisplayEventMapTable()
        {
            if (eventMapStruct != null)
            {
                for (int ii = 1; ii <= eventMapStruct.Length - 1; ii++)
                {
                    eventMapStruct[ii].s = DisplayEventsList(eventMapStruct[ii].eindex, ii);
                    eventMapStruct[ii].type = GetTypeDefTable(eventMapStruct[ii].index);
                }
            }
        }
        private void DisplayEventTable()
        {
            if (eventStruct != null)
            {
                for (int ii = 1; ii <= eventStruct.Length - 1; ii++)
                {
                    eventStruct[ii].tablename = GetTypeDefOrRefTable(eventStruct[ii].coded);
                    eventStruct[ii].index = GetTypeDefOrRefValue(eventStruct[ii].coded);
                    eventStruct[ii].c = DisplayTable(eventStruct[ii].tablename, eventStruct[ii].index);
                }
            }
        }
        private void DisplayPropertyMapTable()
        {
            if (propertyMapStruct != null)
            {
                for (int ii = 1; ii <= propertyMapStruct.Length - 1; ii++)
                {
                    propertyMapStruct[ii].typedef = GetTypeDefTable(propertyMapStruct[ii].parent);
                    propertyMapStruct[ii].s = DisplayPropertiesList(propertyMapStruct[ii].propertylist, ii);
                }
            }
        }
        private void DisplayPropertiesTable()
        {
            if (propertyStruct != null)
            {
                for (int ii = 1; ii <= propertyStruct.Length - 1; ii++)
                {
                    propertyStruct[ii].s = DisplayPropertySignature(propertyStruct[ii].type);
                }
            }
        }
        private void DisplayMethodSemanticsTable()
        {
            if (methodSemanticsStruct != null)
            {
                for (int ii = 1; ii <= methodSemanticsStruct.Length - 1; ii++)
                {
                    methodSemanticsStruct[ii].MethodSemanticsAttributes = GetMethodSemanticsAttributes(methodSemanticsStruct[ii].methodsemanticsattributes);
                    methodSemanticsStruct[ii].s = GetMethodTable(methodSemanticsStruct[ii].methodindex);
                    methodSemanticsStruct[ii].tablename = GetHasSemanticsTable(methodSemanticsStruct[ii].association);
                    int index = GetHasSemanticsValue(methodSemanticsStruct[ii].association);
                    methodSemanticsStruct[ii].t = DisplayTable(methodSemanticsStruct[ii].tablename, index);
                }
            }
        }
        private void DisplayMethodImpTable()
        {
            if (methodImpStruct != null)
            {
                for (int ii = 1; ii <= methodImpStruct.Length - 1; ii++)
                {
                    methodImpStruct[ii].tablename = GetMethodDefTable(methodImpStruct[ii].codedbody);
                    int index = GetMethodDefValue(methodImpStruct[ii].codedbody);
                    methodImpStruct[ii].s = DisplayTable(methodImpStruct[ii].tablename, index);
                    methodImpStruct[ii].tabledef = GetMethodDefTable(methodImpStruct[ii].codeddef);
                    index = GetMethodDefValue(methodImpStruct[ii].codeddef);
                    methodImpStruct[ii].t = DisplayTable(methodImpStruct[ii].tablename, index);
                }
            }
        }
        private void DisplayTypeSpecTableTable()
        {
            if (typeSpecStruct != null)
            {
                for (int ii = 1; ii <= typeSpecStruct.Length - 1; ii++)
                {
                    typeSpecStruct[ii].blob = new List<string>();
                    byte count = blob[typeSpecStruct[ii].signature];
                    for (int l = 1; l <= count; l++)
                    {
                        typeSpecStruct[ii].blob.Add(GetType(blob[typeSpecStruct[ii].signature + l]));
                    }
                }
            }
        }
        private void DisplayImplMapTable()
        {
            if (implMapStruct != null)
            {
                for (int ii = 1; ii <= implMapStruct.Length - 1; ii++)
                {
                    implMapStruct[ii].tablename = GetMemberForwardedTable(implMapStruct[ii].cindex);
                    int index = GetMemberForwardedValue(implMapStruct[ii].cindex);
                    implMapStruct[ii].s = DisplayTable(implMapStruct[ii].tablename, index);
                    index = implMapStruct[ii].scope;
                    implMapStruct[ii].t = GetModuleRefTable(index);
                }
            }
        }
        private void DisplayFieldRVATable()
        {
            if (fieldRVAStruct != null)
            {
                for (int ii = 1; ii <= fieldRVAStruct.Length - 1; ii++)
                {
                    fieldRVAStruct[ii].index = GetFieldTable(fieldRVAStruct[ii].fieldi);
                }
            }
        }
        private void DisplayAssemblyTable()
        {
            if (assemblyStruct != null)
            {
                for (int ii = 1; ii <= assemblyStruct.Length - 1; ii++)
                {
                    int uncompressedbyte;
                    int cb = CorSigUncompressData(blob, assemblyStruct[ii].publickey, out uncompressedbyte);
                    cb--;
                    int count = uncompressedbyte;
                    assemblyStruct[ii].blob = new List<string>();
                    for (int l = 1; l <= count; l++)
                    {
                        assemblyStruct[ii].blob.Add(blob[assemblyStruct[ii].publickey + l + cb].ToString("X"));
                    }
                }
            }
        }
        private void DisplayAssemblyRefTable()
        {
            if (assemblyRefStruct != null)
            {
                for (int ii = 1; ii <= assemblyRefStruct.Length - 1; ii++)
                {
                    int uncompressedbyte;
                    int cb = CorSigUncompressData(blob, assemblyRefStruct[ii].publickey,
                    out uncompressedbyte);
                    cb--;
                    int count = uncompressedbyte;
                    assemblyRefStruct[ii].blob = new List<string>();
                    for (int l = 1; l <= count; l++)
                    {
                        assemblyRefStruct[ii].blob.Add(blob[assemblyRefStruct[ii].publickey + l + cb].ToString("X"));
                    }
                    cb = CorSigUncompressData(blob, assemblyRefStruct[ii].hashvalue, out uncompressedbyte);
                    cb--;
                    count = uncompressedbyte;
                    assemblyRefStruct[ii].blobHash = new List<string>();
                    for (int l = 1; l <= count; l++)
                    {
                        assemblyRefStruct[ii].blobHash.Add(blob[assemblyRefStruct[ii].hashvalue + l + cb].ToString("X"));
                    }
                }
            }
        }
        private void DisplayConstantTable()
        {
            if (constantsStruct != null)
            {
                for (int ii = 1; ii <= constantsStruct.Length - 1; ii++)
                {
                    constantsStruct[ii].type = GetType(constantsStruct[ii].dtype);
                    string s = GetHasConstTable(constantsStruct[ii].parent);
                    int off = GetHasConstValue(constantsStruct[ii].parent);
                    int p = 0;
                    if (s == "FieldDef")
                    {
                        p = fieldStruct[off].nameAddr;
                    }
                    else if (s == "ParamDef")
                    {
                        p = paramStruct[off].nameAdd;
                    }
                    else if (s == "Property")
                    {
                        p = propertyStruct[off].nameAddr;
                    }
                    constantsStruct[ii].Name = GetString(p);
                    int count = blob[constantsStruct[ii].value];
                    constantsStruct[ii].b1 = new List<string>();
                    for (int l = 1; l <= count; l++)
                    {
                        constantsStruct[ii].b1.Add(blob[constantsStruct[ii].value + l].ToString("X"));
                    }
                    if (constantsStruct[ii].dtype <= 0x0b)
                    {
                        constantsStruct[ii].val = GetBytesFromArray(constantsStruct[ii].value, constantsStruct[ii].dtype);
                    }
                    if (constantsStruct[ii].dtype == 0x0e)
                        GetStringFromBlobArray(constantsStruct[ii].value);

                }
            }
        }
        private void DisplayFileTable()
        {
            if (fileStruct != null)
            {
                for (int ii = 1; ii <= fileStruct.Length - 1; ii++)
                {
                    if (fileStruct[ii].flags == 0x00)
                        fileStruct[ii].s = "ContainsMetaData";
                    else if (fileStruct[ii].flags == 0x01)
                        fileStruct[ii].s = "ContainsNoMetaData";

                    int uncompressedbyte;
                    fileStruct[ii].cb = CorSigUncompressData(blob, fileStruct[ii].index, out uncompressedbyte);
                    int count = uncompressedbyte;
                    fileStruct[ii].cb--;
                    fileStruct[ii].b = new List<byte>();
                    for (int l = 1; l <= count; l++)
                    {
                        fileStruct[ii].b.Add(blob[fileStruct[ii].index + l + fileStruct[ii].cb]);
                    }
                    Console.WriteLine("");
                }
            }
        }
        private void DisplayExportedTypeTable()
        {
            if (exportedTypeStruct != null)
            {
                for (int ii = 1; ii <= exportedTypeStruct.Length - 1; ii++)
                {
                    exportedTypeStruct[ii].Tablename = GetImplementationTable(exportedTypeStruct[ii].coded);
                    int ind = GetImplementationValue(exportedTypeStruct[ii].coded);
                    exportedTypeStruct[ii].tb = DisplayTable(exportedTypeStruct[ii].Tablename, ind);
                }
            }
        }
        private void DisplayManifestResourceTable()
        {
            if (manifestResourceStruct != null)
            {
                for (int ii = 1; ii <= manifestResourceStruct.Length - 1; ii++)
                {
                    manifestResourceStruct[ii].attr = GetManifestResourceAttributes(manifestResourceStruct[ii].flags);
                    manifestResourceStruct[ii].tablename = GetManifestResourceTable(manifestResourceStruct[ii].coded);
                    int index = GetManifestResourceValue(manifestResourceStruct[ii].coded);
                }
            }
        }
        private void DisplayTableForDebugging()
        {
            //     DisplayModuleTable();
            try
            {
                DisplayTypeRefTable();
                DisplayTypeDefTable();
                DisplayNestedClassTable();
                DisplayFieldTable();
                DisplayMethodTable();
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
                DisplayTypeSpecTableTable();
                DisplayImplMapTable();
                DisplayFieldRVATable();
                DisplayAssemblyTable();
                DisplayAssemblyRefTable();
                DisplayFileTable();
                DisplayExportedTypeTable();
                DisplayManifestResourceTable();
            }
            catch { }
        }
        #endregion
    }
    #region Table structures
    public class ExportedTypeTableRow : IMAGE_BASE_DATA
    {
        public int flags;
        public int typedefindex;
        public int nameAd;
        public int nspace;
        public int coded;
        private string nameSpace;
        private string tablename;

        public string NameSpace
        {
            get { return nameSpace; }
            set { nameSpace = value; }
        }
        public string Tablename
        {
            get { return tablename; }
            set { tablename = value; }
        }
        public string tb;
        public ExportedTypeTableRow()
        {
        }
        public ExportedTypeTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            flags = sw.ReadInteger();
            typedefindex = sw.ReadInteger();
            nameAd = sw.ReadIndex(MetadataParser.offsetstring);
            nspace = sw.ReadIndex(MetadataParser.offsetstring);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class NestedClassTableRow : IMAGE_BASE_DATA
    {
        public int nestedclass;
        public int enclosingclass;
        public string nestedString;
        public string enclosingString;
        public NestedClassTableRow()
        {
        }
        public NestedClassTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class MethodImpTableRow : IMAGE_BASE_DATA
    {
        public int classindex;
        public int codedbody;
        public int codeddef;
        public string tablename;
        public string s;
        public string tabledef;
        public string t;
        public MethodImpTableRow()
        {
        }
        public MethodImpTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class ClassLayoutTableRow : IMAGE_BASE_DATA
    {
        public short packingsize;
        public int classsize;
        public int parent;
        public string s;
        public ClassLayoutTableRow()
        {
        }
        public ClassLayoutTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class ManifestResourceTableRow : IMAGE_BASE_DATA
    {
        public int offset;
        public int flags;
        public int name;
        public string Name;
        public int coded;
        public string tablename;
        public string attr;
        public ManifestResourceTableRow()
        {
        }
        public ManifestResourceTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class ModuleRefTableRow : IMAGE_BASE_DATA
    {
        public int nameAddr;
        public string Name;
        public ModuleRefTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            nameAddr = sw.ReadIndex(MetadataParser.offsetstring);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class FileTableRow : IMAGE_BASE_DATA
    {
        public int flags;
        public int name;
        public int index;
        public string Name;
        public string s;
        public int cb;
        public List<byte> b;
         public FileTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            flags = sw.ReadInteger();
            name = sw.ReadIndex(MetadataParser.offsetstring);
            index = sw.ReadIndex(MetadataParser.offsetblob);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class EventTableRow : IMAGE_BASE_DATA
    {
        public short attr;
        public int name;
        public int coded;
        public string Name;
        public string tablename;
        public int index;
        public string c;
        public EventTableRow()
        {
        }
        public EventTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class EventMapTableRow : IMAGE_BASE_DATA
    {
        public int index;
        public int eindex;
        public List<string> s;
        public string type;
        public EventMapTableRow()
        {
        }
        public EventMapTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class MethodSemanticsTableRow : IMAGE_BASE_DATA
    {
        public short methodsemanticsattributes;
        public int methodindex;
        public int association;
        public string tablename;
        public string s;
        public string t;
        public List<string> MethodSemanticsAttributes;
        public MethodSemanticsTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            methodsemanticsattributes = sw.ReadShort();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class PropertyMapTableRow : IMAGE_BASE_DATA
    {
        public int parent;
        public int propertylist;
        public String typedef;
        public List<string> s;
        public PropertyMapTableRow()
        {
        }
        public PropertyMapTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class PropertyTableRow : IMAGE_BASE_DATA
    {
        public int flags;
        public int nameAddr;
        public int type;
        public string s;
        public PropertyTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            flags = sw.ReadShort();
            nameAddr = sw.ReadIndex(MetadataParser.offsetstring);
            type = sw.ReadIndex(MetadataParser.offsetblob); ;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class ConstantsTableRow : IMAGE_BASE_DATA
    {
        public short dtype;
        public int parent;
        public int value;
        public string Name;
        public string type;
        public List<string> b1;
        public long val;
        public ConstantsTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            dtype = sw.ReadShort(); 
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class FieldLayoutTableRow : IMAGE_BASE_DATA
    {
        public int offset;
        public int fieldindex;
        public string s;
        public FieldLayoutTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            offset = sw.ReadInteger(); 
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class FieldRVATableRow : IMAGE_BASE_DATA
    {
        public int rva;
        public int fieldi;
        public string index;
        public FieldRVATableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            rva = sw.ReadInteger();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }                             
    public class FieldMarshalTableRow : IMAGE_BASE_DATA
    {
        public int coded;
        public int index;
        public string tablename;
        public string s;
        public string blob;
    }
    public class FieldTableRow : IMAGE_BASE_DATA
    {
        public int flags;
        public int nameAddr;
        public int sig;
        public string FieldSignature;
        public FieldTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            flags = sw.ReadShort();
            nameAddr = sw.ReadIndex(MetadataParser.offsetstring);
            sig = sw.ReadIndex(MetadataParser.offsetblob); ;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class ParamTableRow : IMAGE_BASE_DATA
    {
        public short pattr;
        public int sequence;
        public int nameAdd;
        public ParamTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            pattr = sw.ReadShort();
            sequence = sw.ReadShort();
            nameAdd = sw.ReadIndex(MetadataParser.offsetstring);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class TypeSpecTableRow : IMAGE_BASE_DATA
    {
        public int signature;
        public List<string> blob;

        public TypeSpecTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            signature = sw.ReadIndex(MetadataParser.offsetblob);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class MemberRefTableRow : IMAGE_BASE_DATA
    {
        public int clas;
        public int nameAdd;
        public int sig;
        public string s;
        public int rid;
        public MemberRefTableRow()
        {
        }
        public MemberRefTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class StandAloneSigTableRow : IMAGE_BASE_DATA
    {
        public int index;
        public string s;
    }
    public class InterfaceImplTableRow : IMAGE_BASE_DATA
    {
        public int classindex;
        public int interfaceindex;
    }
    public class TypeDefTableRow : IMAGE_BASE_DATA
    {
        #region TypeDef Flags
        public bool IsTdNotPublic { get { return (((flags) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNotPublic); } }
        public bool IsTdPublic { get { return (((flags) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdPublic); } }
        public bool IsTdNestedPublic { get { return (((flags) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedPublic); } }
        public bool IsTdNestedPrivate { get { return (((flags) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedPrivate); } }
        public bool IsTdNestedFamily { get { return (((flags) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedFamily); } }
        public bool IsTdNestedAssembly { get { return (((flags) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedAssembly); } }
        public bool IsTdNestedFamANDAssem { get { return (((flags) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedFamANDAssem); } }
        public bool IsTdNestedFamORAssem { get { return (((flags) & (int)CorTypeAttr.tdVisibilityMask) == (int)CorTypeAttr.tdNestedFamORAssem); } }
        public bool IsTdNested { get { return (((flags) & (int)CorTypeAttr.tdVisibilityMask) >= (int)CorTypeAttr.tdNestedPublic); } }
        public bool IsTdAutoLayout { get { return (((flags) & (int)CorTypeAttr.tdLayoutMask) == (int)CorTypeAttr.tdAutoLayout); } }
        public bool IsTdSequentialLayout { get { return (((flags) & (int)CorTypeAttr.tdLayoutMask) == (int)CorTypeAttr.tdSequentialLayout); } }
        public bool IsTdExplicitLayout { get { return (((flags) & (int)CorTypeAttr.tdLayoutMask) == (int)CorTypeAttr.tdExplicitLayout); } }
        public bool IsTdClass { get { return (((flags) & (int)CorTypeAttr.tdClassSemanticsMask) == (int)CorTypeAttr.tdClass); } }
        public bool IsTdInterface { get { return (((flags) & (int)CorTypeAttr.tdClassSemanticsMask) == (int)CorTypeAttr.tdInterface); } }
        public bool IsTdAbstract { get { return ((flags) & (int)CorTypeAttr.tdAbstract) > 0; } }
        public bool IsTdSealed { get { return ((flags) & (int)CorTypeAttr.tdSealed) > 0; } }
        public bool IsTdSpecialName { get { return ((flags) & (int)CorTypeAttr.tdSpecialName) > 0; } }
        public bool IsTdImport { get { return ((flags) & (int)CorTypeAttr.tdImport) > 0; } }
        public bool IsTdSerializable { get { return ((flags) & (int)CorTypeAttr.tdSerializable) > 0; } }
        public bool IsTdAnsiClass { get { return (((flags) & (int)CorTypeAttr.tdStringFormatMask) == (int)CorTypeAttr.tdAnsiClass); } }
        public bool IsTdUnicodeClass { get { return (((flags) & (int)CorTypeAttr.tdStringFormatMask) == (int)CorTypeAttr.tdUnicodeClass); } }
        public bool IsTdAutoClass { get { return (((flags) & (int)CorTypeAttr.tdStringFormatMask) == (int)CorTypeAttr.tdAutoClass); } }
        public bool IsTdCustomFormatClass { get { return (((flags) & (int)CorTypeAttr.tdStringFormatMask) == (int)CorTypeAttr.tdCustomFormatClass); } }
        public bool IsTdBeforeFieldInit { get { return ((flags) & (int)CorTypeAttr.tdBeforeFieldInit) > 0; } }
        public bool IsTdForwarder { get { return ((flags) & (int)CorTypeAttr.tdForwarder) > 0; } }
        public bool IsTdRTSpecialName { get { return ((flags) & (int)CorTypeAttr.tdRTSpecialName) > 0; } }
        public bool IsTdHasSecurity { get { return ((flags) & (int)CorTypeAttr.tdHasSecurity) > 0; } }
        #endregion
        public int flags;
        public int nameAddr;
        public int nspace;
        public int cindex;
        public int findex;
        public int mindex;
        public string NameSpace;
        public string tablename;
        public string parentClass;
        public List<MethodTableRow> methods;
        public List<EventTableRow> events;
        public List<PropertyTableRow> properties;
        public List<FieldTableRow> fields;
        public TypeDefTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            flags = sw.ReadInteger();
            nameAddr = sw.ReadIndex(MetadataParser.offsetstring);
            nspace = sw.ReadIndex(MetadataParser.offsetstring);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class CustomAttributeTableRow : IMAGE_BASE_DATA
    {
        public int parent;
        public int type;
        public int value;
        public string tablename;
        public string s;
        public string tablename2;
        public string t;
        public List<string> attrBlob;
        public CustomAttributeTableRow()
        {
        }
        public CustomAttributeTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class AssemblyRefTableRow : IMAGE_BASE_DATA
    {
        public short major, minor, build, revision;
        public int flags;
        public int publickey;
        public int name;
        public int culture;
        public int hashvalue;
        public string Name;
        public List<string> blob;
        public List<string> blobHash;
        public override string ToString()
        {
            return Name;
        }
        public AssemblyRefTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            major = sw.ReadShort();
            minor = sw.ReadShort();
            build = sw.ReadShort();
            revision = sw.ReadShort();
            flags = sw.ReadInteger();
            publickey = sw.ReadIndex(MetadataParser.offsetblob); ;
            name = sw.ReadIndex(MetadataParser.offsetstring);
            culture = sw.ReadIndex(MetadataParser.offsetstring);
            hashvalue = sw.ReadIndex(MetadataParser.offsetblob); ;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class AssemblyTableRow : IMAGE_BASE_DATA
    {
        public int HashAlgId;
        public int major, minor, build, revision;
        public int flags;
        public int publickey;
        public int name;
        public int culture;
        public string Name;
        public List<string> blob;
        public AssemblyTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            HashAlgId = sw.ReadInteger();
            major = sw.ReadShort();
            minor = sw.ReadShort();
            build = sw.ReadShort();
            revision = sw.ReadShort();
            flags = sw.ReadInteger();
            publickey = sw.ReadIndex(MetadataParser.offsetblob);
            name = sw.ReadIndex(MetadataParser.offsetstring);
            culture = sw.ReadIndex(MetadataParser.offsetstring);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class ModuleTableRow : IMAGE_BASE_DATA
    {
        public int Generation;
        public int name;
        public int mvid;
        public int encId;
        public int encBaseId;
        public string Name;
        public Guid Mvid;
        public Guid EncId;
        public Guid EncBaseId;
        public ModuleTableRow()
        {
        }
        public ModuleTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            Generation = sw.ReadShort();
            name = sw.ReadIndex(MetadataParser.offsetstring);
            mvid = sw.ReadIndex(MetadataParser.offsetguid);
            encId = sw.ReadIndex(MetadataParser.offsetguid);
            encBaseId = sw.ReadIndex(MetadataParser.offsetguid);
            LengthInFile = sw.Position - PositionOfStructureInFile;

        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class TypeRefTableRow : IMAGE_BASE_DATA
    {
        public int resolutionscope;
        public int name;
        public int nspace;
        public string Name;
        public string NameSpace;
        public string tableName;
        public string s;
        public TypeRefTableRow()
        {
        }
        public TypeRefTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class MethodTableRow : IMAGE_BASE_DATA
    {
        #region CorMethodImpl
        public bool IsmiIL { get { return (((Implflags) & CorMethodImpl.miCodeTypeMask) == CorMethodImpl.miIL); } }
        public bool IsmiNative { get { return (((Implflags) & CorMethodImpl.miCodeTypeMask) == CorMethodImpl.miNative); } }
        public bool IsmiOPTIL { get { return (((Implflags) & CorMethodImpl.miCodeTypeMask) == CorMethodImpl.miOPTIL); } }
        public bool IsmiRuntime { get { return (((Implflags) & CorMethodImpl.miCodeTypeMask) == CorMethodImpl.miRuntime); } }
        public bool IsmiUnmanaged { get { return (((Implflags) & CorMethodImpl.miManagedMask) == CorMethodImpl.miUnmanaged); } }
        public bool IsmiManaged { get { return (((Implflags) & CorMethodImpl.miManagedMask) == CorMethodImpl.miManaged); } }
        public bool IsmiForwardRef { get { return ((Implflags) & CorMethodImpl.miForwardRef) > 0; } }
        public bool IsmiPreserveSig { get { return ((Implflags) & CorMethodImpl.miPreserveSig) > 0; } }
        public bool IsmiInternalCall { get { return ((Implflags) & CorMethodImpl.miInternalCall) > 0; } }
        public bool IsmiSynchronized { get { return ((Implflags) & CorMethodImpl.miSynchronized) > 0; } }
        public bool IsmiNoInlining { get { return ((Implflags) & CorMethodImpl.miNoInlining) > 0; } }
        public bool IsmiNoOptimization { get { return ((Implflags) & CorMethodImpl.miNoOptimization) > 0; } }
        #endregion
        #region CorMethodAttr
        public bool IsmdPrivateScope { get { return (((Flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdPrivateScope); } }
        public bool IsmdPrivate { get { return (((Flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdPrivate); } }
        public bool IsmdFamANDAssem { get { return (((Flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdFamANDAssem); } }
        public bool IsmdAssem { get { return (((Flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdAssem); } }
        public bool IsmdFamily { get { return (((Flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdFamily); } }
        public bool IsmdFamORAssem { get { return (((Flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdFamORAssem); } }
        public bool IsmdPublic { get { return (((Flags) & CorMethodAttr.mdMemberAccessMask) == CorMethodAttr.mdPublic); } }

        public bool IsmdStatic { get { return ((Flags) & CorMethodAttr.mdStatic) > 0; } }
        public bool IsmdFinal { get { return ((Flags) & CorMethodAttr.mdFinal) > 0; } }
        public bool IsmdVirtual { get { return ((Flags) & CorMethodAttr.mdVirtual) > 0; } }
        public bool IsmdHideBySig { get { return ((Flags) & CorMethodAttr.mdHideBySig) > 0; } }

        public bool IsmdReuseSlot { get { return (((Flags) & CorMethodAttr.mdVtableLayoutMask) == CorMethodAttr.mdReuseSlot); } }
        public bool IsmdNewSlot { get { return (((Flags) & CorMethodAttr.mdVtableLayoutMask) == CorMethodAttr.mdNewSlot); } }

        public bool IsmdCheckAccessOnOverride { get { return ((Flags) & CorMethodAttr.mdCheckAccessOnOverride) > 0; } }
        public bool IsmdAbstract { get { return ((Flags) & CorMethodAttr.mdAbstract) > 0; } }
        public bool IsmdSpecialName { get { return ((Flags) & CorMethodAttr.mdSpecialName) > 0; } }

        public bool IsmdPinvokeImpl { get { return ((Flags) & CorMethodAttr.mdPinvokeImpl) > 0; } }
        public bool IsmdUnmanagedExport { get { return ((Flags) & CorMethodAttr.mdUnmanagedExport) > 0; } }

        public bool IsmdRTSpecialName { get { return ((Flags) & CorMethodAttr.mdRTSpecialName) > 0; } }
        /*public bool  IsmdInstanceInitializer(Flags, str)     (((Flags) & CorMethodAttr.mdRTSpecialName) && !strcmp((str), COR_CTOR_METHOD_NAME));}}
        public bool  IsmdInstanceInitializerW(Flags, str)    (((Flags) & CorMethodAttr.mdRTSpecialName) && !wcscmp((str), COR_CTOR_METHOD_NAME_W));}}
        public bool  IsmdClassConstructor(Flags, str)        (((Flags) & CorMethodAttr.mdRTSpecialName) && !strcmp((str), COR_CCTOR_METHOD_NAME));}}
        public bool  IsmdClassConstructorW(Flags, str)       (((Flags) & CorMethodAttr.mdRTSpecialName) && !wcscmp((str), COR_CCTOR_METHOD_NAME_W));}}*/
        public bool IsmdHasSecurity { get { return ((Flags) & CorMethodAttr.mdHasSecurity) > 0; } }
        #endregion
        CorMethodImpl Implflags
        { get { return (CorMethodImpl)implflags; } }
        CorMethodAttr Flags
        { get { return (CorMethodAttr)flags; } }
        public int rva;
        public int implflags;
        public int flags;
        public int name;
        public int signature;
        public int param;
        public string Name;
        public List<ParamTableRow> paras;
        public string meths;
        public MethodTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            rva = sw.ReadInteger();
            implflags = sw.ReadShort();
            flags = (int)sw.ReadShort();
            name = sw.ReadIndex(MetadataParser.offsetstring);
            signature = sw.ReadIndex(MetadataParser.offsetblob);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class DeclSecurityTableRow : IMAGE_BASE_DATA
    {
        public int action;
        public int coded;
        public int bindex;
        public string tablename;
        public List<char> blob;
        public int cb;
        public DeclSecurityTableRow()
        {
        }
        public DeclSecurityTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class ImplMapTableRow : IMAGE_BASE_DATA
    {
        public short attr;
        public int cindex;
        public int name;
        public int scope;
        public string Name;
        public string tablename;
        public string s;
        public string t;
        public ImplMapTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            attr = sw.ReadShort(); 
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Name;
        }
    }
    public class GenericParamTableRow : IMAGE_BASE_DATA
    {
        public short number;
        public short flags;
        public int owner;
        public int name;
        public GenericParamTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class GenericParamConstraintTableRow : IMAGE_BASE_DATA
    {
        public int owner;
        public int constraint;
        public GenericParamConstraintTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class MethodSpecTableRow : IMAGE_BASE_DATA
    {
        public int Method;// (an index into the MethodDef or MemberRef table, specifying to which generic methodvthis row refers
        public int Instantiation;// (an index into the Blob heap (§23.2.15), holding the signature of this instantiation
        public MethodSpecTableRow(BinaryFileReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            Method = sw.ReadInteger();
            Instantiation = sw.ReadInteger();
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    #endregion
    public class ClrStream : IMAGE_BASE_DATA
    {
        private string name;
        public long offset;
        public int size;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public byte[] data;
        public ClrStream(BinaryFileReader sw, long startOfMetadata)
        {
            PositionOfStructureInFile = sw.Position;
            offset = sw.ReadInteger() + startOfMetadata;
            size = sw.ReadInteger();
            name = sw.ReadStringZ(Encoding.ASCII);
            while (true)
            {
                //padding
                if (sw.Position % 4 == 0)
                    break;
                byte b = (byte)sw.ReadByte();
                if (b != 0)
                {
                    sw.Position--;
                    break;
                }
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return name + " " + offset.ToString("x8") + " " + size.ToString("x8");
        }
    }
    #region Enumerations
    public enum CorFieldAttr
    {
        // member access mask - Use this mask to retrieve accessibility information.
        fdFieldAccessMask = 0x0007,
        fdPrivateScope = 0x0000,     // Member not referenceable.
        fdPrivate = 0x0001,     // Accessible only by the parent type.
        fdFamANDAssem = 0x0002,     // Accessible by sub-types only in this Assembly.
        fdAssembly = 0x0003,     // Accessibly by anyone in the Assembly.
        fdFamily = 0x0004,     // Accessible only by type and sub-types.
        fdFamORAssem = 0x0005,     // Accessibly by sub-types anywhere, plus anyone in assembly.
        fdPublic = 0x0006,     // Accessibly by anyone who has visibility to this scope.
        // endIAT member access mask

        // field contract attributes.
        fdStatic = 0x0010,     // Defined on type, else per instance.
        fdInitOnly = 0x0020,     // Field may only be initialized, not written to after init.
        fdLiteral = 0x0040,     // fixedFileInfo is compile time constant.
        fdNotSerialized = 0x0080,     // Field does not have to be serialized when type is remoted.

        fdSpecialName = 0x0200,     // field is special.  addressOfname describes how.

        // interop attributes
        fdPinvokeImpl = 0x2000,     // Implementation is forwarded through pinvoke.

        // Reserved Flags for runtime use only.
        fdReservedMask = 0x9500,
        fdRTSpecialName = 0x0400,     // Runtime(metadata internal APIs) should check addressOfname encoding.
        fdHasFieldMarshal = 0x1000,     // Field has marshalling information.
        fdHasDefault = 0x8000,     // Field has default.
        fdHasFieldRVA = 0x0100,     // Field has RVA.
    }
    public enum CorTypeAttr
    {
        // Use this mask to retrieve the type visibility information.
        tdVisibilityMask = 0x00000007,
        tdNotPublic = 0x00000000,     // Class is not public scope.
        tdPublic = 0x00000001,     // Class is public scope.
        tdNestedPublic = 0x00000002,     // Class is nested with public visibility.
        tdNestedPrivate = 0x00000003,     // Class is nested with private visibility.
        tdNestedFamily = 0x00000004,     // Class is nested with family visibility.
        tdNestedAssembly = 0x00000005,     // Class is nested with assembly visibility.
        tdNestedFamANDAssem = 0x00000006,     // Class is nested with family and assembly visibility.
        tdNestedFamORAssem = 0x00000007,     // Class is nested with family or assembly visibility.

        // Use this mask to retrieve class layout information
        tdLayoutMask = 0x00000018,
        tdAutoLayout = 0x00000000,     // Class fields are auto-laid out
        tdSequentialLayout = 0x00000008,     // Class fields are laid out sequentially
        tdExplicitLayout = 0x00000010,     // Layout is supplied explicitly
        // endIAT layout mask

        // Use this mask to retrieve class semantics information.
        tdClassSemanticsMask = 0x00000020,
        tdClass = 0x00000000,     // Type is a class.
        tdInterface = 0x00000020,     // Type is an interface.
        // endIAT semantics mask

        // Special semantics in addition to class semantics.
        tdAbstract = 0x00000080,     // Class is abstract
        tdSealed = 0x00000100,     // Class is concrete and may not be extended
        tdSpecialName = 0x00000400,     // Class addressOfname is special.  addressOfname describes how.

        // Implementation attributes.
        tdImport = 0x00001000,     // Class / interface is imported
        tdSerializable = 0x00002000,     // The class is Serializable.

        // Use tdStringFormatMask to retrieve string information for native interop
        tdStringFormatMask = 0x00030000,
        tdAnsiClass = 0x00000000,     // LPTSTR is interpreted as ANSI in this class
        tdUnicodeClass = 0x00010000,     // LPTSTR is interpreted as UNICODE
        tdAutoClass = 0x00020000,     // LPTSTR is interpreted automatically
        tdCustomFormatClass = 0x00030000,     // A non-standard encoding specified by CustomFormatMask
        tdCustomFormatMask = 0x00C00000,     // Use this mask to retrieve non-standard encoding information for native interop. The meaning of the values of these 2 bits is unspecified.

        // endIAT string format mask

        tdBeforeFieldInit = 0x00100000,     // Initialize the class any time before first static field access.
        tdForwarder = 0x00200000,     // This ExportedType is a type forwarder.

        // Flags reserved for runtime use.
        tdReservedMask = 0x00040800,
        tdRTSpecialName = 0x00000800,     // Runtime should check addressOfname encoding.
        tdHasSecurity = 0x00040000,     // Class has security associate with it.
    }
    public enum CorMethodAttr
    {
        // member access mask - Use this mask to retrieve accessibility information.
        mdMemberAccessMask = 0x0007,
        mdPrivateScope = 0x0000,     // Member not referenceable.
        mdPrivate = 0x0001,     // Accessible only by the parent type.
        mdFamANDAssem = 0x0002,     // Accessible by sub-types only in this Assembly.
        mdAssem = 0x0003,     // Accessibly by anyone in the Assembly.
        mdFamily = 0x0004,     // Accessible only by type and sub-types.
        mdFamORAssem = 0x0005,     // Accessibly by sub-types anywhere, plus anyone in assembly.
        mdPublic = 0x0006,     // Accessibly by anyone who has visibility to this scope.
        // endIAT member access mask

        // method contract attributes.
        mdStatic = 0x0010,     // Defined on type, else per instance.
        mdFinal = 0x0020,     // Method may not be overridden.
        mdVirtual = 0x0040,     // Method virtual.
        mdHideBySig = 0x0080,     // Method hides by addressOfname+sig, else just by addressOfname.

        // vtable layout mask - Use this mask to retrieve vtable attributes.
        mdVtableLayoutMask = 0x0100,
        mdReuseSlot = 0x0000,     // The default.
        mdNewSlot = 0x0100,     // Method always gets a new slot in the vtable.
        // endIAT vtable layout mask

        // method implementation attributes.
        mdCheckAccessOnOverride = 0x0200,     // Overridability is the same as the visibility.
        mdAbstract = 0x0400,     // Method does not provide an implementation.
        mdSpecialName = 0x0800,     // Method is special.  addressOfname describes how.

        // interop attributes
        mdPinvokeImpl = 0x2000,     // Implementation is forwarded through pinvoke.
        mdUnmanagedExport = 0x0008,     // Managed method exported via thunk to unmanaged code.

        // Reserved Flags for runtime use only.
        mdReservedMask = 0xd000,
        mdRTSpecialName = 0x1000,     // Runtime should check addressOfname encoding.
        mdHasSecurity = 0x4000,     // Method has security associate with it.
        mdRequireSecObject = 0x8000,     // Method calls another method containing security code.

    }
    public enum CorMethodImpl
    {
        // code impl mask
        miCodeTypeMask = 0x0003,   // Flags about code type.
        miIL = 0x0000,   // Method impl is IL.
        miNative = 0x0001,   // Method impl is native.
        miOPTIL = 0x0002,   // Method impl is OPTIL
        miRuntime = 0x0003,   // Method impl is provided by the runtime.
        // endIAT code impl mask

        // managed mask
        miManagedMask = 0x0004,   // Flags specifying whether the code is managed or unmanaged.
        miUnmanaged = 0x0004,   // Method impl is unmanaged, otherwise managed.
        miManaged = 0x0000,   // Method impl is managed.
        // endIAT managed mask

        // implementation info and interop
        miForwardRef = 0x0010,   // Indicates method is defined; used primarily in merge scenarios.
        miPreserveSig = 0x0080,   // Indicates method sig is not to be mangled to do HRESULT conversion.

        miInternalCall = 0x1000,   // Reserved for internal use.

        miSynchronized = 0x0020,   // Method is single threaded through the body.
        miNoInlining = 0x0008,   // Method may not be inlined.
        miNoOptimization = 0x0040,   // Method may not be optimized.
        miMaxMethodImplVal = 0xffff,   // Range check value
    }
    public enum CorParamAttr
    {
        pdIn = 0x0001,     // Param is [In]
        pdOut = 0x0002,     // Param is [out]
        pdOptional = 0x0010,     // Param is optional

        // Reserved Flags for Runtime use only.
        pdReservedMask = 0xf000,
        pdHasDefault = 0x1000,     // Param has default value.
        pdHasFieldMarshal = 0x2000,     // Param has FieldMarshal.

        pdUnused = 0xcfe0,
    }
    public enum CorPropertyAttr
    {
        prSpecialName = 0x0200,     // property is special.  addressOfname describes how.

        // Reserved Flags for Runtime use only.
        prReservedMask = 0xf400,
        prRTSpecialName = 0x0400,     // Runtime(metadata internal APIs) should check addressOfname encoding.
        prHasDefault = 0x1000,     // Property has default

        prUnused = 0xe9ff,
    }
    enum ReplacesCorHdrNumericDefines
    {
        // COM+ Header entry point Flags.
        COMIMAGE_FLAGS_ILONLY = 0x00000001,
        COMIMAGE_FLAGS_32BITREQUIRED = 0x00000002,
        COMIMAGE_FLAGS_IL_LIBRARY = 0x00000004,
        COMIMAGE_FLAGS_STRONGNAMESIGNED = 0x00000008,
        // DDBLD - Added Next Line - Still verifying general usage
        COMIMAGE_FLAGS_NATIVE_ENTRYPOINT = 0x00000010,
        // DDBLD - End of Add
        COMIMAGE_FLAGS_TRACKDEBUGDATA = 0x00010000,
        // Other kinds of Flags follow
    }
    public enum TableType { Module, TypeRef, TypeDef, FieldPtr, Field, MethodPtr, MethodDef, ParamPtr, Param, InterfaceImpl, MemberRef, Constant, CustomAttribute, FieldMarshal, DeclSecurity, ClassLayout, FieldLayout, StandAloneSig, EventMap, EventPtr, Event, PropertyMap, PropertyPtr, Properties, MethodSemantics, MethodImpl, ModuleRef, TypeSpec, ImplMap, FieldRVA, ENCLog, ENCMap, Assembly, AssemblyProcessor, AssemblyOS, AssemblyRef, AssemblyRefProcessor, AssemblyRefOS, File, ExportedType, ManifestResource, NestedClass, TypeTyPar, MethodTyPar }
    #endregion
}
