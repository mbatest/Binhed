using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Management;
using Utils;

namespace LowLevel
{
    public class RawDiskAccess : LOCALIZED_DATA
    {
        public long SecteurNumber = 0;
        public long Offset;
        public int SizeBuffer = 0x200;
        public int Cylinder = 0;
        public int Track = 0;
        public int Sector = 1;
        public byte[] SectorBuffer;
        #region Private members
        ulong totalCylinders;
        ulong numberOfTracks;
        ulong numberOfSectors;
        ulong totalTracks;
        ulong totalSectors;
        List<Win32_DiskDrive> physicalDisks;
        MasterBootRecord mbr;
        TreeNode mbrNode;
        Win32_DiskDrive currentDrive;
        Partition currentPartition;
        #endregion
        #region Properties
        public List<Win32_DiskDrive> Physical_Disks
        {
            get { return physicalDisks; }
            set { physicalDisks = value; }
        }
        public Win32_DiskDrive Current_Drive
        {
            get { return currentDrive; }
            set { currentDrive = value; }
        }
        public ulong Total_Cylinders
        {
            get { return totalCylinders; }
            set { totalCylinders = value; }
        }
        public ulong Number_Of_Tracks
        {
            get { return numberOfTracks; }
            set { numberOfTracks = value; }
        }
        public ulong Number_Of_Sectors
        {
            get { return numberOfSectors; }
            set { numberOfSectors = value; }
        }
        public ulong Total_Tracks
        {
            get { return totalTracks; }
            set { totalTracks = value; }
        }
        public ulong Total_Sectors
        {
            get { return totalSectors; }
            set { totalSectors = value; }
        }
        public MasterBootRecord Master_Boot_Record
        {
            get { return mbr; }
            set { mbr = value; }
        }
        #endregion
        public RawDiskAccess()
        {
            physicalDisks = new List<Win32_DiskDrive>();
            ManagementObjectSearcher res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_DiskDrive"));
            mbrNode = new TreeNode("Master boot record");
            foreach (ManagementObject o in res.Get())
            {
                Win32_DiskDrive p = new Win32_DiskDrive(o);
                physicalDisks.Add(p);
            }
            currentDrive = physicalDisks[0];
            numberOfSectors = currentDrive.TotalSectors;
            numberOfTracks = currentDrive.TotalTracks;
        }
        #region Public methods
        public object RequestData(object o)
        {
            object detail = null;
            Type t = o.GetType();
            if (t.Name == typeof(MFT).Name)
            {
            }
            if (t.Name == typeof(Partition).Name)
            {
                Partition p = (Partition)o;
                this.currentPartition = p;
            }
            if (t.Name == typeof(Root16Entry).Name)
            {
                Root16Entry root = (Root16Entry)o;
                if (root.IsSubdirectory)
                {
                    long st = root.DataStartSector;
                    BitStreamReader sw = new BitStreamReader(ReadSectors(st, 10 * (short)currentPartition.Boot_Sector.Bytes_Per_Sector.Value), false);
                    Directory rd = new Directory(sw, currentPartition, st * (short)currentPartition.Boot_Sector.Bytes_Per_Sector.Value, 0);
                    detail = rd;
                }
            }
            if (t.Name == typeof(INDEX_ENTRY).Name)
            {
                INDEX_ENTRY indx = (INDEX_ENTRY)o;
                BootRecord_NTFS boot = (BootRecord_NTFS)currentPartition.Boot_Sector;
                long off = (uint)indx.FileReference.SegmentNumberLowPart.Value * 2 + 
                    currentPartition.ClusterToSector((long)boot.MFT_Start.Value);
                BitStreamReader sw = new BitStreamReader(ReadSectors(off, 2 * (short)boot.Bytes_Per_Sector.Value), false);
                FILE_RECORD_SEGMENT fs = new FILE_RECORD_SEGMENT(sw, off * (short)boot.Bytes_Per_Sector.Value, currentPartition, (int)(uint)indx.FileReference.SegmentNumberLowPart.Value);
                if (fs.Is_Directory)
                {
                    foreach (ATTRIBUTE_RECORD_HEADER att in fs.Attributes)
                    {
                        if (att.Attribute_Type == ATTRIBUTE_TYPE_CODE.INDEX_ALLOCATION)
                        {
                            ParseIndexAllocation(boot, fs, att, currentPartition);
                            break;
                        }
                    }
           
                    detail = fs;
                }
                if (fs.Is_File)
                {
                    detail = fs;
                }
            }
            if (t.Name==typeof(ATTRIBUTE_RECORD_HEADER).Name)
            {
                ATTRIBUTE_RECORD_HEADER att = (ATTRIBUTE_RECORD_HEADER)o;
                switch (att.Attribute_Type == ATTRIBUTE_TYPE_CODE.ATTRIBUTE_LIST)
                {
                }
            }
            if (t.Name==typeof(ATTRIBUTE_LIST_ENTRY).Name)
            {
                ATTRIBUTE_LIST_ENTRY att = (ATTRIBUTE_LIST_ENTRY)o;
                BootRecord_NTFS boot = (BootRecord_NTFS)currentPartition.Boot_Sector;
                long off = (uint)att.Base_File_Reference_of_the_attribute.SegmentNumberLowPart.Value * 2 + 
                    currentPartition.ClusterToSector((long)boot.MFT_Start.Value);
                BitStreamReader sw = new BitStreamReader(ReadSectors(off, 2 * (short)boot.Bytes_Per_Sector.Value), false);
                FILE_RECORD_SEGMENT fs = new FILE_RECORD_SEGMENT(sw, off * (short)boot.Bytes_Per_Sector.Value, currentPartition, (int)(uint)att.Base_File_Reference_of_the_attribute.SegmentNumberLowPart.Value);
                foreach (ATTRIBUTE_RECORD_HEADER attr in fs.Attributes)
                {
                    if (attr.Attribute_Type == ATTRIBUTE_TYPE_CODE.INDEX_ALLOCATION)
                    {
                        ParseIndexAllocation(boot, fs, attr, currentPartition);
                        break;
                    }
                }
                detail = fs;
            }
            return detail;
        }
        public object ParseBlock(long sector, int length)
        {
            object dt = null;
            byte[] data = ReadSectors(sector, length);
            string head = Encoding.Default.GetString(data, 0, 4); 
            switch (head)
            {
                case "FILE":
                    if (currentPartition == null)
                        return null;
                    FILE_RECORD_SEGMENT fs = new FILE_RECORD_SEGMENT(new BitStreamReader(data, false), sector * 0x200, currentPartition, 0);
                    dt = fs;
                    break;
                case "INDX":
                    data = ReadSectors(sector, 8*0x200);
                    INDX indx = new INDX(new BitStreamReader(data, false), sector * 0x200);
                    dt = indx;
                    break;
                case "RDR":
                    break;
            }
            return dt;
        }
        public void ReadsFirstSector()
        {
            SizeBuffer = (int)currentDrive.BytesPerSector;
            mbr = ReadMasterBoot(0);
        }
        public void Change(long shift, int sb)
        {
            SecteurNumber = shift;
            long[] chs = SectorToChs(SecteurNumber);
            Cylinder = (int)chs[0];
            Track = (int)chs[1];
            Sector = (int)chs[2];
            SizeBuffer = sb;
            SectorBuffer = ReadSectors(SecteurNumber, sb);
        }
        #endregion
        #region Private Methods
        private void ParseIndexAllocation(BootRecord_NTFS boot, FILE_RECORD_SEGMENT fs, ATTRIBUTE_RECORD_HEADER att, Partition p)
        {
            long st = (long)(att.Nonresident.Startcluster * (byte)boot.Sectors_Per_Cluster.Value + (uint)boot.Hidden_Sectors.Value + p.Start_Partition);
            int nb = att.Nonresident.Numbercluster / (int)boot.Cluster_Per_Index.Value;
            for (int i = 0; i < nb; i++)
            {
                fs.Index.Add(new INDX(new BitStreamReader(ReadSectors(st, (int)boot.Cluster_Per_Index.Value * (byte)boot.Sectors_Per_Cluster.Value * (short)boot.Bytes_Per_Sector.Value), false), st * (short)boot.Bytes_Per_Sector.Value));
                st += (int)boot.Cluster_Per_Index.Value * (byte)boot.Sectors_Per_Cluster.Value;
            }
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
        private MasterBootRecord ReadMasterBoot(long startPartition)
        {
            SectorBuffer = ReadSectors(startPartition, SizeBuffer);
            if (SectorBuffer == null)
                return null;
            MasterBootRecord mbr = new MasterBootRecord(new BitStreamReader(SectorBuffer, false), Offset);
            foreach (Partition p in mbr.Partitions)
            {
                SectorBuffer = ReadSectors((uint)p.Start_Sector.Value + startPartition, SizeBuffer);
                BitStreamReader sw = new BitStreamReader(SectorBuffer, false);
                switch (p.Partition_Type)
                {
                    case PartitionType.NTFS:
                        #region NTFS
                        p.Start_Partition = startPartition;
                        p.Boot_Sector = new BootRecord_NTFS(sw, ((uint)p.Start_Sector.Value + startPartition) * SizeBuffer);
                        BootRecord_NTFS nt = (BootRecord_NTFS)p.Boot_Sector;
                        long startMFT = p.ClusterToSector((long)nt.MFT_Start.Value);
                        int FileRecordsNumber = 0x23;
                        p.Master_File_Table = new MFT(new BitStreamReader(ReadSectors(startMFT, FileRecordsNumber * 2 * (short)p.Boot_Sector.Bytes_Per_Sector.Value), false), startMFT * SizeBuffer, p, FileRecordsNumber);
                        foreach (FILE_RECORD_SEGMENT frs in p.Master_File_Table.File_Records)
                        {
                            switch (frs.Name)
                            {
                                case "$AttrDef":
                                    #region Attributes
                                    foreach (ATTRIBUTE_RECORD_HEADER att in frs.Attributes)
                                    {
                                        if (att.Attribute_Type == ATTRIBUTE_TYPE_CODE.DATA)
                                        {
                                            long st = p.ClusterToSector((int)(att.Nonresident.Startcluster));// * (byte)p.Boot_Sector.Sectors_Per_Cluster.Value + (uint)p.Boot_Sector.Hidden_Sectors.Value);
                                            byte[] am = ReadSectors(st, 0x40 * 2 * 0x200);
                                            BitStreamReader sm = new BitStreamReader(am, false);
                                            frs.Attribute_Definitions = new List<ATTRIBUTE_DEFINITION_DATA>();
                                            for (int i = 0; i < 0x10; i++)
                                            {
                                                sm.Position = i * 0xa0;// catch with standard length
                                                ATTRIBUTE_DEFINITION_DATA def = new ATTRIBUTE_DEFINITION_DATA(sm, st * 0x200);
                                                frs.Attribute_Definitions.Add(def);
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                    #endregion
                                case ".":
                                    #region Root directory
                                    foreach (ATTRIBUTE_RECORD_HEADER att in frs.Attributes)
                                    {
                                        if (att.Attribute_Type == ATTRIBUTE_TYPE_CODE.INDEX_ALLOCATION)
                                        {
                                            ParseIndexAllocation((BootRecord_NTFS) p.Boot_Sector, frs, att, p);
                                            break;
                                        }
                                    }
                                    break;
                                    #endregion
                            }
                        }
                        break;
                        #endregion
                    case PartitionType.Dell_Utility:
                    case PartitionType.FAT12:
                    case PartitionType.FAT16:
                        #region FAT16
                        p.Boot_Sector = new BootRecord_Fat16(sw, (uint)p.Start_Sector.Value * SizeBuffer);
                        p.directoryOffset = (uint)p.Start_Sector.Value + (short)p.Boot_Sector.Reserved_Sectors.Value + (short)((BootRecord_Fat16)p.Boot_Sector).Sectors_Per_FAT.Value * 2;
                        BitStreamReader ms = new BitStreamReader(ReadSectors(p.directoryOffset, (short)((BootRecord_Fat16)p.Boot_Sector).Root_Directory_Entries.Value * 0x20), false);
                        p.Root_Directory = new Directory(ms, p, p.directoryOffset * (short)p.Boot_Sector.Bytes_Per_Sector.Value, (short)((BootRecord_Fat16)p.Boot_Sector).Root_Directory_Entries.Value);
                        break;
                        #endregion
                    case PartitionType.Win95_OSR2_FAT32_adressage_LBA:
                    case PartitionType.Win95_OSR2_FAT32_adressage_CHS:
                        #region FAT32
                        p.Boot_Sector = new BootRecord_Fat32(sw, (uint)p.Start_Sector.Value * SizeBuffer);
                        p.directoryOffset = (uint)p.Start_Sector.Value + (short)p.Boot_Sector.Reserved_Sectors.Value + (int)((BootRecord_Fat32)p.Boot_Sector).Sectors_Per_FAT.Value * (byte)((BootRecord_Fat32)p.Boot_Sector).Number_of_Fats.Value;
                        ms = new BitStreamReader(ReadSectors(p.directoryOffset, 0x40 * 200), false);
                        p.Root_Directory = new Directory(ms, p, p.directoryOffset * (short)p.Boot_Sector.Bytes_Per_Sector.Value, (short)((BootRecord_Fat32)p.Boot_Sector).Root_Directory_Entries.Value);
                        break;
                        #endregion
                    case PartitionType.Étendue_adressage_LBA:
                        #region Extended Partition
                        p.Extended_Partition_Master_Boot = ReadMasterBoot((uint)p.Start_Sector.Value);
                        break;
                        #endregion
                }
            }
            return mbr;
        }
        private byte[] ReadSectors(long startSector, long length)
        {
            long[] chs = SectorToChs(startSector);
            if (chs == null)
                return null;
            Offset = startSector * SizeBuffer;
            /*     short FILE_ATTRIBUTE_NORMAL = 0x80;
              short INVALID_HANDLE_VALUE = -1;
              uint GENERIC_WRITE = 0x40000000;
              uint CREATE_NEW = 1;
              uint CREATE_ALWAYS = 2;*/
            uint GENERIC_READ = 0x80000000;
            uint OPEN_EXISTING = 3;
            SafeFileHandle handleValue = CreateFile(currentDrive.DeviceID, GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (handleValue.IsInvalid)
            {
                return null;
                //                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            byte[] buffer = new byte[length];
            int read = 0;
            int moveToHigh = (int)(Offset >> 32);
            SetFilePointer(handleValue, (int) Offset, out moveToHigh, EMoveMethod.Begin);
            ReadFile(handleValue, buffer, (int)length, out read, IntPtr.Zero);
            handleValue.Close();
            return buffer;
        }
        private void WriteSectors(long startSector, byte[] buffer)
        {
            long[] chs = SectorToChs(startSector);
            if (chs == null)
                return;
            Offset = startSector * SizeBuffer;
            uint GENERIC_WRITE = 0x40000000;
            uint OPEN_EXISTING = 3;
            SafeFileHandle handleValue = CreateFile(currentDrive.DeviceID, GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (handleValue.IsInvalid)
            {
                return;
                //                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            uint written = 0;
            int moveToHigh = (int)(Offset >> 32);
            System.Threading.NativeOverlapped ov = new System.Threading.NativeOverlapped();
            ov.OffsetHigh = moveToHigh;
            ov.OffsetLow = (int)Offset;
            SetFilePointer(handleValue, (int)Offset, out moveToHigh, EMoveMethod.Begin);
            WriteFile(handleValue, buffer, (uint)buffer.Length, out written, ref ov);
            handleValue.Close();
            return;
        }
        #endregion
        #region External
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
        [DllImport("kernel32.dll")]
        static extern bool WriteFile(SafeFileHandle handle, byte[] lpBuffer,
           uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten,
           [In] ref System.Threading.NativeOverlapped lpOverlapped); 
        #endregion
    }
}
