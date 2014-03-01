using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace Code
{
    public partial class ExeViewer : UserControl
    {
        public event DataSelectedEventHandler dataSelected;
        Executable exe;
        public ExeViewer()
        {
            InitializeComponent();
        }
        public void Init(Executable exe)
        {
            this.exe = exe;
            TreeNode Root = new TreeNode(exe.FileName);
            treeView1.Nodes.Add(Root);
            #region Headers
            TreeNode h = new TreeNode(exe.Exeheader.ToString());
            h.Tag = exe.Exeheader;
            Root.Nodes.Add(h);
            h.Nodes.Add("Magic " + exe.Exeheader.Magic);
            h.Nodes.Add("Address " + exe.Exeheader.AddressOfNewHeader.ToString("x8"));
            h.Nodes.Add("Stub " + exe.Exeheader.StubProgram.ToString());
            TreeNode nt_h = new TreeNode("NT headers");
            Root.Nodes.Add(nt_h);
            nt_h.Tag = exe.NT_Headers;
            nt_h.Nodes.Add(new TreeNode(exe.NT_Headers.Signature));
            nt_h.Nodes.Add(new TreeNode(exe.NT_Headers.Processor));
            TreeNode Fh = new TreeNode("File Header");
            Fh.Tag = exe.NT_Headers.FileHeader;
            nt_h.Nodes.Add(Fh);
            Fh.Nodes.Add(new TreeNode("Machine : " + Encoding.Default.GetString(exe.NT_Headers.FileHeader.Machine)));
            Fh.Nodes.Add(new TreeNode("Number of Sections : " + exe.NT_Headers.FileHeader.NumberOfSections.ToString()));
            Fh.Nodes.Add(new TreeNode("Time Stamp : " + exe.NT_Headers.FileHeader.TimeDateStamp.ToString()));
            Fh.Nodes.Add(new TreeNode("Pointer to symbol table : " + exe.NT_Headers.FileHeader.PointerToSymbolTable.ToString("x8")));
            Fh.Nodes.Add(new TreeNode("Number of symbols : " + exe.NT_Headers.FileHeader.NumberOfSymbols.ToString("x8")));
            Fh.Nodes.Add(new TreeNode("Size of optional header : " + exe.NT_Headers.FileHeader.SizeOfOptionalHeader.ToString("x4")));
            Fh.Nodes.Add(new TreeNode("Characteristics : " + exe.NT_Headers.FileHeader.Characteristics));
            TreeNode Oph = new TreeNode("Optional Header");
            Oph.Tag = exe.NT_Headers.OptionalHeader;
            Oph.Nodes.Add("Magic : " + exe.NT_Headers.OptionalHeader.Magic.ToString("x2") + " (" + exe.NT_Headers.OptionalHeader.MAGIC + ")");
            Oph.Nodes.Add("Linker Version : " + exe.NT_Headers.OptionalHeader.MajorLinkerVersion.ToString("") + "." + exe.NT_Headers.OptionalHeader.MinorLinkerVersion.ToString(""));
            Oph.Nodes.Add("Size of code : " + exe.NT_Headers.OptionalHeader.SizeOfCode.ToString("x8"));
            Oph.Nodes.Add("Size of initialized data : " + exe.NT_Headers.OptionalHeader.SizeOfInitializedData.ToString("x8"));
            Oph.Nodes.Add("Size of uninitialized data : " + exe.NT_Headers.OptionalHeader.SizeOfUninitializedData.ToString("x8"));
            Oph.Nodes.Add("Adress of entry point : " + exe.NT_Headers.OptionalHeader.AddressOfEntryPoint.ToString("x8"));
            Oph.Nodes.Add("Base of code : " + exe.NT_Headers.OptionalHeader.BaseOfCode.ToString("x8"));
            Oph.Nodes.Add("Base of data : " + exe.NT_Headers.OptionalHeader.BaseOfData.ToString("x8"));
            Oph.Nodes.Add("Image Base : " + exe.NT_Headers.OptionalHeader.ImageBase.ToString("x8"));
            Oph.Nodes.Add("Section alignment : " + exe.NT_Headers.OptionalHeader.SectionAlignment.ToString("x8"));
            Oph.Nodes.Add("File alignement : " + exe.NT_Headers.OptionalHeader.FileAlignment.ToString("x8"));
            Oph.Nodes.Add("Operating system version : " + exe.NT_Headers.OptionalHeader.MajorOperatingSystemVersion.ToString("") + "." + exe.NT_Headers.OptionalHeader.MinorOperatingSystemVersion.ToString(""));
            Oph.Nodes.Add("Image version : " + exe.NT_Headers.OptionalHeader.MajorImageVersion.ToString("") + "." + exe.NT_Headers.OptionalHeader.MinorImageVersion.ToString(""));
            Oph.Nodes.Add("Subsystem version : " + exe.NT_Headers.OptionalHeader.MajorSubsystemVersion.ToString("") + "." + exe.NT_Headers.OptionalHeader.MinorSubsystemVersion.ToString(""));
            Oph.Nodes.Add("Win32 version : " + exe.NT_Headers.OptionalHeader.Win32VersionValue.ToString("x8"));
            Oph.Nodes.Add("Size of Image : " + exe.NT_Headers.OptionalHeader.SizeOfImage.ToString("x8"));
            Oph.Nodes.Add("Size of headers : " + exe.NT_Headers.OptionalHeader.SizeOfHeaders.ToString("x8"));
            Oph.Nodes.Add("Checksum : " + exe.NT_Headers.OptionalHeader.CheckSum.ToString("x8"));
            Oph.Nodes.Add("Subsystem : " + exe.NT_Headers.OptionalHeader.Subsystem);
            Oph.Nodes.Add("Dll characteristics : " + exe.NT_Headers.OptionalHeader.FileAlignment.ToString("x4"));
            Oph.Nodes.Add("Size of stack reserve : " + exe.NT_Headers.OptionalHeader.SizeOfStackReserve.ToString("x8"));
            Oph.Nodes.Add("Size of stack commit : " + exe.NT_Headers.OptionalHeader.SizeOfStackCommit.ToString("x8"));
            Oph.Nodes.Add("Size of heap reserve : " + exe.NT_Headers.OptionalHeader.SizeOfHeapReserve.ToString("x8"));
            Oph.Nodes.Add("Size of heap commit : " + exe.NT_Headers.OptionalHeader.SizeOfHeapCommit.ToString("x8"));
            Oph.Nodes.Add("Loader flags : " + exe.NT_Headers.OptionalHeader.LoaderFlags.ToString("x8"));
            Oph.Nodes.Add("Number of Data directorys : " + exe.NT_Headers.OptionalHeader.NumberOfRvaAndSizes.ToString("x8"));
            nt_h.Nodes.Add(Oph);
            #endregion
            #region Data directories
            TreeNode dtdir = new TreeNode("Data directories");
            foreach (IMAGE_DATA_DIRECTORY datDir in exe.DataDirs)
            {
                TreeNode dtd = new TreeNode(datDir.ToString());
                dtd.Tag = datDir;
                dtdir.Nodes.Add(dtd);
                if (datDir.Size > 0)
                {
                    dtd.Nodes.Add("Virtual Adress : " + datDir.VirtualAddress.ToString("x8"));
                    dtd.Nodes.Add("Size : " + datDir.Size.ToString("x8"));
                    switch (datDir.DictionaryName)
                    {
                        case "Export Table":
                            if (datDir.Export == null)
                                break;
                            TreeNode tname = new TreeNode(datDir.Export.ApplicationName);
                            dtd.Nodes.Add(tname);
                            tname.Tag = datDir.Export;
                            tname.Nodes.Add("Address of functions : " + datDir.Export.AddressOfFunctions.ToString("x8"));
                            tname.Nodes.Add("Address of Names : " + datDir.Export.AddressOfNames.ToString("x8"));
                            tname.Nodes.Add("Address of Ordinals : " + datDir.Export.AddressOfNameOrdinals.ToString("x8"));
                            foreach (EXPORTED_FUNCTION ex in datDir.Export.Exports)
                            {
                                TreeNode tex = new TreeNode(ex.Name);
                                tname.Nodes.Add(tex);
                                tex.Tag = ex;
                                tex.Nodes.Add("Ordinal : " + ex.ordinal.ToString());
                                tex.Nodes.Add("Address of function :" + ex.addressOfFunction.ToString("x8"));
                                tex.Nodes.Add("Address of name :" + ex.addressOfName.ToString("x8"));
                            }
                            break;
                        case "Import Table":
                            foreach (IMAGE_IMPORT_DESCRIPTOR imdesc in datDir.Import.Descriptors)
                            {
                                TreeNode tn = new TreeNode(imdesc.ToString());
                                tn.Tag = imdesc;
                                dtd.Nodes.Add(tn);
                                foreach (IMAGE_THUNK_DATA imth in imdesc.FirstThunks)
                                {
                                    string g = "";
                                    if (imth.AddressOfData != null)
                                    {
                                        g = imth.AddressOfData.ToString();
                                    }
                                    else
                                        g = imth.Ordinal.ToString();
                                    TreeNode tno = new TreeNode(g);
                                    tno.Tag = imth;
                                    tn.Nodes.Add(tno);
                                }
                            }
                            break;
                        case "Resource Table":
                            break;
                        case "Exception Table":
                            break;
                        case "Certificate Table":
                            //                ExpandCertificate(dtd, datDir.Certificate.Certificate);
                            if (datDir.Certificate != null)
                                certificateDisplay1.Init(datDir.Certificate.Certificate);
                            break;
                        case "Base Relocation Table":
                            TreeNode tbn = new TreeNode("Relocation Table");
                            dtd.Nodes.Add(tbn);
                            if (exe.RelocationTable != null)
                                foreach (IMAGE_RELOCATION a in exe.RelocationTable.Relocations)
                                {
                                    TreeNode ttn = new TreeNode(a.ToString());
                                    tbn.Nodes.Add(ttn);
                                    ttn.Tag = a;
                                }
                            break;
                        case "Debug":
                            if (exe.Debug_data != null)
                            {
                                dtd.Nodes.Add(exe.Debug_data.DebugType.ToString());
                                dtd.Nodes.Add("Time stamp " + " " + exe.Debug_data.TimeDateStamp);
                                dtd.Nodes.Add("Size of Data " + " " + exe.Debug_data.SizeOfData.ToString());
                                dtd.Nodes.Add("Address of Data " + " " + exe.Debug_data.AddressOfRawData.ToString());
                                dtd.Nodes.Add("Pointer " + " " + exe.Debug_data.PointerToRawData.ToString());
                            }
                            break;
                        case "Architecture":
                            break;
                        case "Global Ptr":
                            break;
                        case "TLS Table":
                            break;
                        case "Load Config Table":
                            break;
                        case "Bound Import":
                            break;
                        case "IAT":
                            break;
                        case "Delay Import Descriptor":
                            break;
                        case "CLR Runtime Header":
                            if (exe.DataDirs[14].clrHeader != null)
                                clrViewer1.Init(exe);
                            break;
                        case "Reserved":
                            break;
                    }
                }
            }
            #endregion
            #region Sections
            TreeNode sect = new TreeNode("Sections");
            Root.Nodes.Add(sect);
            foreach (IMAGE_SECTION_HEADER sch in exe.Sections.Sections)
            {
                TreeNode sec = new TreeNode(sch.Name);
                sect.Nodes.Add(sec);
                sec.Tag = sch;
                sec.Nodes.Add("Pointer to Raw Data : " + sch.PointerToRawData);
                sec.Nodes.Add("Size of Raw Data : " + sch.SizeOfRawData);
                sec.Nodes.Add("Virtual address : " + sch.VirtualAddress.ToString("x8"));
                sec.Nodes.Add("Virtual size : " + sch.VirtualSize.ToString("x8"));
                if (sch.dataDirs.Count > 0)
                {
                    TreeNode tn = new TreeNode("Data directories");
                    sec.Nodes.Add(tn);
                    foreach (IMAGE_DATA_DIRECTORY dr in sch.dataDirs)
                    {
                        TreeNode td = new TreeNode(dr.DictionaryName);
                        tn.Nodes.Add(td);
                    }
                }
                TreeNode chars = new TreeNode("Characteristics");
                sec.Nodes.Add(chars);
                foreach (string s in sch.Characteristics)
                    chars.Nodes.Add(s);
                if (sch.Name.Contains(".text"))
                {
                    if (exe.Disassembly != null)
                    {
                        TreeNode tn = new TreeNode("Code data");
                        sec.Nodes.Add(tn);
                        tn.Nodes.Add("Number of Lines : 0x" + exe.Disassembly.lineNumber.ToString("x8"));
                        TreeNode ttn = new TreeNode("Calls : 0x" + exe.Disassembly.subroutines.Count.ToString("x8"));
                        tn.Nodes.Add(ttn);
                        foreach (CallAdress c in exe.Disassembly.subroutines)
                        {
                            if ((c.cdLine.ToString().Contains("CALL")) & (c.cdLine.OpCode == 0xE8))
                                ttn.Nodes.Add("0x" + (exe.NT_Headers.OptionalHeader.BaseOfCode + c.offset).ToString("x8") + " : " + c.cdLine);
                        }
                    }
                }
            }
            Root.Nodes.Add(dtdir);
            #endregion
            #region Resources
            TreeNode res = new TreeNode("Resources");
            Root.Nodes.Add(res);
            if (exe.Resource != null)
            {
                TreeNode drentries = new TreeNode("Directory Entries");
                res.Nodes.Add(drentries);
                foreach (IMAGE_RESOURCE_DIRECTORY_ENTRY dire in exe.Resource.DirectoryEntries)
                {
                    TreeNode te = new TreeNode(dire.ToString() + " " + dire.Resource_type);
                    drentries.Nodes.Add(te);
                    te.Tag = dire;
                    switch (dire.resType)
                    {
                        case RESOURCE_TYPE.RT_ICON:
                             #region Icon
                            foreach (IMAGE_RESOURCE_DIRECTORY dir in dire.Subdirs)
                            {
                                foreach (IMAGE_RESOURCE_DIRECTORY_ENTRY di in dir.DirectoryEntries)
                                {
                                    foreach (IMAGE_RESOURCE_DIRECTORY d in di.Subdirs)
                                    {
                                        foreach (IMAGE_RESOURCE_DATA_ENTRY dat in d.DataEntries)
                                        {
                                            TreeNode ts= new TreeNode("Icon"/* ((ICOHEADER)dat.ob.ob).ToString()*/);
                                            ts.Tag = dat.Ob;
                                            te.Nodes.Add(ts);
                                            Type t = dat.Ob.GetType();
                                            System.Reflection.PropertyInfo[] p = t.GetProperties();
                                            List<string> str = new List<string>();
                                            foreach (System.Reflection.PropertyInfo v in p)
                                            {
                                                string s = v.Name;
                                                if(dat.Ob.GetType().GetProperty(v.Name).GetValue(dat.Ob, null)!=null)
                                                s += " : " + dat.Ob.GetType().GetProperty(v.Name).GetValue(dat.Ob, null).ToString();
                                                str.Add(s);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                            #endregion
                        case RESOURCE_TYPE.RT_VERSION:
                            #region Version
                            foreach (IMAGE_RESOURCE_DIRECTORY ird in dire.Subdirs)
                            {
                                foreach (IMAGE_RESOURCE_DIRECTORY_ENTRY ide in ird.DirectoryEntries)
                                {
                                    foreach (IMAGE_RESOURCE_DIRECTORY iderd in ide.Subdirs)
                                    {
                                        foreach (IMAGE_RESOURCE_DATA_ENTRY idede in iderd.DataEntries)
                                        {
                                            if (idede.Ob != null)
                                            {
                                                VS_VERSIONINFO vs = (VS_VERSIONINFO)idede.Ob.Ob;
                                                te.Nodes.Add("Length :" + vs.wLength.ToString("x4"));
                                                te.Nodes.Add("Length of value :" + vs.wValueLength.ToString("x4"));
                                                te.Nodes.Add("Signature :" + vs.FixedFileInfo.DwSignature.ToString("x8"));
                                                te.Nodes.Add("FileOs :" + vs.ostype.ToString());
                                                TreeNode vst = new TreeNode(vs.szKey);
                                                te.Tag = ide;
                                                te.Nodes.Add(vst);
                                                vst.Tag = vs;
                                                foreach (stringFileInfo sf in vs.Fileinfos)
                                                {
                                                    TreeNode vsf = new TreeNode(sf.szKey);
                                                    vst.Nodes.Add(vsf);
                                                    vsf.Tag = sf;
                                                    if (sf.Children != null)
                                                        foreach (Rstring st in sf.Children.Children)
                                                        {
                                                            TreeNode stf = new TreeNode(st.szKey + " : " + st.Value);
                                                            vsf.Nodes.Add(stf);
                                                            stf.Tag = st;
                                                        }
                                                    if (sf.VarChildren != null)
                                                    {
                                                        TreeNode stf = new TreeNode(sf.VarChildren.szKey + " : ");
                                                        vsf.Nodes.Add(stf);
                                                        stf.Tag = sf;
                                                        foreach (int i in sf.VarChildren.Value)
                                                        {
                                                            int a1 = i >> 16;
                                                            int a2 = i & 0x0000ffff;
                                                            stf.Nodes.Add(a2.ToString() + "/" + a1.ToString());
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                            #endregion
                        case RESOURCE_TYPE.RT_MANIFEST:
                            #region Manifest
                            foreach (IMAGE_RESOURCE_DIRECTORY dir in dire.Subdirs)
                            {
                                foreach (IMAGE_RESOURCE_DIRECTORY_ENTRY di in dir.DirectoryEntries)
                                {
                                    foreach (IMAGE_RESOURCE_DIRECTORY d in di.Subdirs)
                                    {
                                        foreach (IMAGE_RESOURCE_DATA_ENTRY dat in d.DataEntries)
                                        {
                                            TreeNode ts= new TreeNode( (string)dat.Ob.Ob);
                                            ts.Tag = dat.Ob;
                                            te.Nodes.Add(ts);
                                        }
                                    }
                                }
                            }
                            break;
                            #endregion
                        case RESOURCE_TYPE.RT_GROUP_ICON:
                            #region Group Icon
                            foreach (IMAGE_RESOURCE_DIRECTORY dir in dire.Subdirs)
                            {
                                foreach (IMAGE_RESOURCE_DIRECTORY_ENTRY di in dir.DirectoryEntries)
                                {
                                    TreeNode ted = new TreeNode(di.Resource_type);
                                    te.Nodes.Add(ted);
                                    ted.Tag = di;
                                    foreach (IMAGE_RESOURCE_DIRECTORY d in di.Subdirs)
                                    {
                                        foreach (IMAGE_RESOURCE_DATA_ENTRY dat in d.DataEntries)
                                        {
                                            GRPICONDIR s = (GRPICONDIR)dat.Ob.Ob;
                                            foreach (GRPICONDIRENTRY de in s.idEntries)
                                            {
                                                ted.Nodes.Add(de.wBitCount.ToString() + " bits, " + de.bWidth.ToString() + "x" + de.bHeight.ToString() + " Ordinal " + de.nID.ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                            #endregion
                        case RESOURCE_TYPE.RT_STRING:
                  //          List<string> str= (List<string>) d
                        default:
                            foreach (IMAGE_RESOURCE_DIRECTORY dir in dire.Subdirs)
                            {
                                FillTree(te, dir);
                            }
                            break;
                    }

                }
             }
            #endregion
            if (exe.Disassembly != null)
                showCode1.Init(exe);
        }
        private void ExpandCertificate(TreeNode root, ASN1_OBJECT cert)
        {
            TreeNode tN = new TreeNode(cert.ToString());
            root.Nodes.Add(tN);
            tN.Tag = cert;
            if (cert.Data != null)
                tN.Nodes.Add((cert).DataString);
            if (cert.Nodes != null)
                foreach (ASN1_OBJECT cr in cert.Nodes)
                    ExpandCertificate(tN, cr);
        }
        private void FillTree(TreeNode tn, IMAGE_RESOURCE_DIRECTORY dir)
        {
            TreeNode tte = new TreeNode(dir.ToString());
            tn.Nodes.Add(tte);
            tn.Tag = dir;
            if(dir.DirectoryEntries.Count>0)
                foreach (IMAGE_RESOURCE_DIRECTORY_ENTRY d in dir.DirectoryEntries)
                {
                    foreach (IMAGE_RESOURCE_DIRECTORY dd in d.Subdirs)
                    {
                        FillTree(tte, dd);
                    }
                }
            if(dir.DataEntries.Count>0)
                foreach (IMAGE_RESOURCE_DATA_ENTRY dten in dir.DataEntries)
                {
                    tte.Nodes.Add("Offset to data : "+ dten.OffsetToData.ToString("x8"));
                    tte.Nodes.Add("Real address :" + dten.Real_Address.ToString("x8"));
                    tte.Nodes.Add("Size : "+ dten.Size.ToString("x8"));
                    tte.Nodes.Add("Code Page : "+ dten.CodePage.ToString("x8"));
                    if (dten.Ob != null)
                    {
                        if (dten.Ob.GetType().Name == "String")
                        {
                            string st = (string)dten.Ob.Ob;
                            tte.Nodes.Add("Value : " + st);
                        }
                        else
                            tte.Nodes.Add(dten.Ob.ToString());
                    }
                   
                }
            if (dir.StringEntries.Count > 0)
            {
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            if (tn.Tag != null)
            {
                IMAGE_BASE_DATA im = (IMAGE_BASE_DATA)tn.Tag;
                if(dataSelected!=null)
                    dataSelected(this,new DataSelectedEventArgs(im.PositionOfStructureInFile,im.LengthInFile));
            }
            if (tn.Text == "Certificate Table")
            {
                certificateDisplay1.Init(exe.DataDirs[4].Certificate.Certificate);
            }
            if (tn.Text.Contains(".text"))
            {
                if (exe.Disassembly != null)
                    showCode1.Init(exe);
            }
            if (tn.Text.Contains("Code data"))
            {
                tn.Nodes.Clear();
                tn.Nodes.Add("Number of Lines : 0x" + exe.Disassembly.lineNumber.ToString("x8"));
                SortedList<long, CallAdress> ord = new SortedList<long, CallAdress>();
                foreach (CallAdress c in exe.Disassembly.subroutines)
                {
                    ord.Add(c.offset, c);
                }
                TreeNode ttn = new TreeNode("Calls : 0x" + ord.Count.ToString("x8"));
                tn.Nodes.Add(ttn);
                for (int i = 0; i < ord.Count; i++)
                {
                    CallAdress c = ord.Values[i];
                    TreeNode t = new TreeNode("0x" + (exe.NT_Headers.OptionalHeader.BaseOfCode + c.offset).ToString("x8") + " : " + c.cdLine);
                    t.Tag = exe.NT_Headers.OptionalHeader.BaseOfCode + c.offset + exe.NT_Headers.OptionalHeader.ImageBase;
                    ttn.Nodes.Add(t);
                }
                ttn = new TreeNode("Labels : 0x" + exe.Disassembly.references.Count.ToString("x8"));
                tn.Nodes.Add(ttn);
                SortedList<long, long> refer = new SortedList<long, long>();
                foreach (long c in exe.Disassembly.references)
                {
                    refer.Add(c, c);
                }
                for (int i = 0; i < refer.Count; i++)
                {
                    TreeNode t = new TreeNode("L" + (refer.Values[i]).ToString("x8"));
                    t.Tag = refer.Values[i];
                    ttn.Nodes.Add(t);
                }
            }
            if((tn.Text.Contains("L"))||(tn.Text.Contains("J")))
            {
                if (tn.Tag != null)
                {
                    long l;
                    if (tn.Tag.GetType().Name == "String")
                    {
                        if (long.TryParse((string)tn.Tag, out l))
                        {
                            long a = (long)tn.Tag;
                            showCode1.SelectLine(a);
                        }
                        string aa = tn.Text.Substring(0, tn.Text.IndexOf(':')).Trim().Replace("0x", "");
                        if (long.TryParse(aa, System.Globalization.NumberStyles.AllowHexSpecifier, null, out l))
                        {
                            //  long a = (long)tn.Tag;
                            showCode1.SelectLine(l);
                        }
                    }
                }
           }
            if (tn.Text.Contains("IAT"))
            {
                tn.Nodes.Clear();
                if (exe.IAT != null)
                    foreach (IMAGE_IMPORT_DIRECTORY_IAT_ENTRY imt in exe.IAT.IATEntries)
                    {
                        TreeNode tv = new TreeNode(imt.Name + " : " + imt.intData.ToString("x8"));
                        tn.Nodes.Add(tv);
                        tv.Tag = imt;
                    }
            }
            if (tn.Text.Contains("Import"))
            {
                tn.Nodes.Clear();
                foreach (IMAGE_DATA_DIRECTORY datDir in exe.DataDirs)
                {
                    if (datDir.DictionaryName.Contains(tn.Text))
                    {
                        tn.Nodes.Add("Virtual Adress : " + datDir.VirtualAddress.ToString("x8"));
                        tn.Nodes.Add("Size : " + datDir.Size.ToString("x8"));
                        if (datDir.Import != null)
                            foreach (IMAGE_IMPORT_DESCRIPTOR imdesc in datDir.Import.Descriptors)
                            {
                                TreeNode tx = new TreeNode(imdesc.ToString());
                                tn.Nodes.Add(tx);
                                tn.Tag = imdesc;
                                foreach (IMAGE_THUNK_DATA imth in imdesc.FirstThunks)
                                {
                                    if (imth.AddressOfData != null)
                                    {
                                        TreeNode tno = new TreeNode(imth.AddressOfData.Name);
                                        tx.Nodes.Add(tno);
                                        tno.Tag = imth;
                                        tno.Nodes.Add("Hint : " + imth.AddressOfData.Hint.ToString("x4"));
                                        tno.Nodes.Add("Function : " + imth.Function.ToString("x4"));
                                    }
                                }
                            }
                    }
                }
            }
        }       
    }

}
