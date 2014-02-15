using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;


using Utils;

//http://www.fort-awesome.net/blog/2010/03/25/MBR_VBR_and_Raw_Disk

namespace LowLevel
{
    public partial class Form1 : UserControl
    {
        long secteurNumber = 0;
        int sizebuff = 0x200;
        int cylinder = 0;
        int track = 0;
        int sector = 1;
        ulong totalCylinders;
        ulong numberOfTracks;
        ulong numberOfSectors;
        ulong totalTracks;
        ulong totalSectors;
        byte[] buf;
        List<Win32_DiskDrive> physicalDisks;
        MasterBootRecord mbr;
        TreeNode mbrNode;
        public enum EMoveMethod : uint
        {
            Begin = 0,
            Current = 1,
            End = 2
        }
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint SetFilePointer(
            [In] SafeFileHandle hFile,
            [In] int lDistanceToMove,
            [Out] out int lpDistanceToMoveHigh,
            [In] EMoveMethod dwMoveMethod);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
          uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
          uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32", SetLastError = true)]
        internal extern static int ReadFile(SafeFileHandle handle, byte[] bytes,
           int numBytesToRead, out int numBytesRead, IntPtr overlapped_MustBeZero);
        public Form1()
        {
            InitializeComponent();
            physicalDisks = new List<Win32_DiskDrive>();
            ManagementObjectSearcher res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_DiskDrive"));
            TreeNode tn = new TreeNode("Physical disks");
            treeView1.Nodes.Add(tn);
            mbrNode = new TreeNode("Master boot record");
            treeView1.Nodes.Add(mbrNode);
            foreach (ManagementObject o in res.Get())
            {
                Win32_DiskDrive p = new Win32_DiskDrive(o);
                physicalDisks.Add(p);
                ToolStripMenuItem m = new ToolStripMenuItem(p.DeviceID);
                m.Click += new EventHandler(m_Click);
                textBoxPath.DropDownItems.Add(m);
                tn.Nodes.Add(FillTreeNode(p));
            }

            textBoxOffset.Text = "0";
            textBoxSize.Text = sizebuff.ToString("x2");
            secteur.Text = secteurNumber.ToString();
            Cylinder.Text = cylinder.ToString();
            TrackN.Text = track.ToString();
            Sector.Text = sector.ToString();
            textBoxPath.Text = @"\\.\PhysicalDrive0";
        }
        void m_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem m = (ToolStripMenuItem) sender;
            textBoxPath.Text = m.Text;
        }
        private void buttonDump_Click(object sender, EventArgs e)
        {
            ReadsFirstSector();
        }
        private long CHSToSector(int[] chs)
        {
            return (chs[0] * (long)numberOfTracks + chs[1]) * (long)numberOfSectors + chs[2] - 1;
            
        }
        private long[] SectorToChs(long lba)
        {
            if ((numberOfSectors == 0) || (numberOfTracks == 0))
                return null;
            long[] chs = new long[3];
            chs[0] = lba / (long)(numberOfSectors * numberOfTracks);
            chs[1] = (lba / (long)numberOfSectors) % (long)numberOfTracks;
            chs[2] = (lba % (long)numberOfSectors) + 1;
            return chs;
        }
        private void ReadsFirstSector()
        {
            ReadData();
            if (secteurNumber == 0)
            {
    //           mbr = new MasterBootRecord(buf);
               treeView1.Nodes.Remove(mbrNode);
               mbrNode = FillTreeNode(mbr);
               treeView1.Nodes.Add(mbrNode);
            }
            Refresh();
        }
        private TreeNode FillTreeNode(object c)
        {
            Type t = c.GetType();
            System.Reflection.PropertyInfo[] p = t.GetProperties();
            TreeNode root = new TreeNode(c.ToString());
            foreach (System.Reflection.PropertyInfo v in p)
            {
                string s = v.Name + " : ";
                try
                {
                    if (c.GetType().GetProperty(v.Name).GetValue(c, null) != null)
                    {
                        if (v.PropertyType.IsArray)
                        {

                            Array a = (Array)c.GetType().GetProperty(v.Name).GetValue(c, null);
                            if (a.Length > 0)
                            {
                                TreeNode ts = new TreeNode(s);
                                ts.Tag = c.GetType().GetProperty(v.Name).GetValue(c, null);
                                root.Nodes.Add(ts);

                                foreach (var element in a)
                                {
                                    if (element != null)
                                    {
                                        TreeNode ta = new TreeNode(element.ToString());
                                        ta.Tag = element;
                                        ts.Nodes.Add(ta);
                                    }
                                }
                            }
                        }
                        else
                            if (v.PropertyType.IsGenericType)
                            {
                                if (v.PropertyType.Name.Contains("List"))
                                {
                                    Type[] tp = v.PropertyType.GetGenericArguments();
                                    IList a = (IList)c.GetType().GetProperty(v.Name).GetValue(c, null);
                                    if (a.Count > 0)
                                    {
                                        TreeNode ts = new TreeNode(s);
                                        ts.Tag = c.GetType().GetProperty(v.Name).GetValue(c, null);
                                        root.Nodes.Add(ts);

                                        foreach (var x in a)
                                        {
                                            TreeNode ta = FillTreeNode(x);
                                            ta.Tag = x;
                                            ts.Nodes.Add(ta);
                                       }
                                    }
                                }
                            }
                            else
                            {
                                switch (c.GetType().GetProperty(v.Name).GetValue(c, null).GetType().Name)
                                {
                                    #region Display according to type
                                    case "Byte":
                                        byte ub = (byte)c.GetType().GetProperty(v.Name).GetValue(c, null);
                                        s += "0x" + ub.ToString("x2");
                                        break;
                                    case "Int16":
                                        short u = (short)c.GetType().GetProperty(v.Name).GetValue(c, null);
                                        s += "0x" + u.ToString("x2");
                                        break;
                                    case "Int32":
                                        int ui = (int)c.GetType().GetProperty(v.Name).GetValue(c, null);
                                        s += "0x" + ui.ToString("x4");
                                        break;
                                    case "Int64":
                                        long ul = (long)c.GetType().GetProperty(v.Name).GetValue(c, null);
                                        s += "0x" + ul.ToString("x8");
                                        break;
                                    case "String":
                                        if (c.GetType().GetProperty(v.Name).GetValue(c, null).ToString() == "")
                                            s = "";
                                        else
                                            s += c.GetType().GetProperty(v.Name).GetValue(c, null).ToString();
                                        break;
                                    case "Bitmap":
                                        s = "";
                                        //        iml.Images.Add((Bitmap)c.GetType().GetProperty(v.Name).GetValue(c, null));
                                        TreeNode tal = new TreeNode("Album Art", 1, 1);
                                        //        treeView1.ImageList = iml;
                                        root.Nodes.Add(tal);
                                        break;
                                    default:
                                        //   if (c.GetType().GetProperty(v.nameAddr).GetValue(c, null).ToString() == "")
                                        //      s = "";
                                        //  else
                                        s += c.GetType().GetProperty(v.Name).GetValue(c, null).ToString();
                                        break;
                                    #endregion
                                }
                                if (s != "")
                                {
                                    TreeNode ts = new TreeNode(s);
                                    ts.Tag = c.GetType().GetProperty(v.Name).GetValue(c, null);
                                    root.Nodes.Add(ts);
                                }
                                else
                                {
                                }
                            }
                    }
                }
                catch { }
            }
            return root;
        }
        private void ReadData()
        {
            sizebuff = int.Parse(textBoxSize.Text, System.Globalization.NumberStyles.HexNumber);          
            secteurNumber = int.Parse(secteur.Text, System.Globalization.NumberStyles.HexNumber);
            ReadSector(secteurNumber);
        }

        private void ReadSector(long sector)
        {
            long[] chs = SectorToChs(sector);
            if (chs == null)
                return;
            Cylinder.Text = chs[0].ToString("x2");
            TrackN.Text = chs[1].ToString("x2");
            Sector.Text = chs[2].ToString("x2");
            long offset = sector * sizebuff;
            textBoxOffset.Text = offset.ToString("x2");
            short FILE_ATTRIBUTE_NORMAL = 0x80;
            short INVALID_HANDLE_VALUE = -1;
            uint GENERIC_READ = 0x80000000;
            uint GENERIC_WRITE = 0x40000000;
            uint CREATE_NEW = 1;
            uint CREATE_ALWAYS = 2;
            uint OPEN_EXISTING = 3;
            SafeFileHandle handleValue = CreateFile(textBoxPath.Text, GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (handleValue.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            buf = new byte[sizebuff];
            int read = 0;
            int moveToHigh;
            SetFilePointer(handleValue, (int)offset, out moveToHigh, EMoveMethod.Begin);
            ReadFile(handleValue, buf, sizebuff, out read, IntPtr.Zero);
            handleValue.Close();
        }
        private void Save()
        {
            SaveFileDialog mySFD = new SaveFileDialog();
            mySFD.FileName = "dump.bin";
            mySFD.InitialDirectory = Path.GetDirectoryName(Application.StartupPath);
            if (mySFD.ShowDialog() == DialogResult.OK)
            {
                FileStream myStream = File.OpenWrite(mySFD.FileName);
                myStream.Write(buf, 0, sizebuff);
                myStream.Flush();
                myStream.Close();
            }
        }
        private void next_Click(object sender, EventArgs e)
        {
            secteurNumber++;
            secteur.Text = secteurNumber.ToString("x2");
            textBoxOffset.Text = (secteurNumber * sizebuff).ToString("x2");
            ReadData();
        }
        private void back_Click(object sender, EventArgs e)
        {
            secteurNumber--;
            if (secteurNumber < 0)
                secteurNumber = 0;
            secteur.Text = secteurNumber.ToString("x2");
            textBoxOffset.Text = (secteurNumber * sizebuff).ToString("x2");
            ReadData();
        }
        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
            foreach (Win32_DiskDrive p in physicalDisks)
            {
                if (p.DeviceID.ToUpper() == textBoxPath.Text.ToUpper())
                {
                    totalCylinders = p.TotalCylinders;
                    numberOfTracks = p.TracksPerCylinder;
                    numberOfSectors = p.SectorsPerTrack;
                    totalSectors = p.TotalSectors;
                    totalTracks = p.TotalTracks;
                    ulong diskSize = p.Size;
                    break;
                }
            }
            secteurNumber = 0;
            secteur.Text = secteurNumber.ToString("x2");
            ReadsFirstSector();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode t = (TreeNode)treeView1.SelectedNode;
            switch (t.Text.ToLower())
            {
                case "master boot record":
                    secteurNumber = 0;
                    secteur.Text = secteurNumber.ToString("x2");
                    ReadsFirstSector();
                    break;
            }
            Object o = t.Tag;
            if (o.GetType().Name == typeof(Partition).Name)
            {
                Partition p = (Partition)o;
                secteur.Text = p.Start_Sector.ToString();
                secteurNumber = (uint)p.Start_Sector.Value;
                ReadSector((uint)p.Start_Sector.Value);
                BitStreamReader sw = new BitStreamReader(buf, false);
  /*              switch (p.Partition_Type)
                {
                    case Partition_Type.NTFS:
             //           BootRecord_NTFS brNtfs = new BootRecord_NTFS(sw);
                        TreeNode tntfs = FillTreeNode(brNtfs);
                        t.Nodes.Add(tntfs);
                        break;
                    case Partition_Type.Dell_Utility:
                    case Partition_Type.FAT12:
                    case Partition_Type.FAT16:
            //            BootRecord_Fat16 br16 = new BootRecord_Fat16(sw);
                        TreeNode tn = FillTreeNode(br16);
                        t.Nodes.Add(tn);
                        break;
                    case Partition_Type.Win95_OSR2_FAT32_adressage_LBA:
                    case Partition_Type.Win95_OSR2_FAT32_adressage_CHS:
        //                BootRecord_Fat32 br = new BootRecord_Fat32(sw);
                        TreeNode tn2 = FillTreeNode(br);
                        t.Nodes.Add(tn2);
                        break;
                    /*                        BootRecord_Dell brd = new BootRecord_Dell(sw);
                                            TreeNode tnd = FillTreeNode(brd);
                                            t.Nodes.Add(tnd);
                                            break;
                }*/
            }
        }
    }

}
