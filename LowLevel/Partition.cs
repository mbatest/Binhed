using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace LowLevel
{
    public class MasterBootRecord : LOCALIZED_DATA
    {
        public byte[] bootCode;
        private List<ELEMENTARY_TYPE> messages = new List<ELEMENTARY_TYPE>();
        private List<Partition> partitions = new List<Partition>();
        public List<ELEMENTARY_TYPE> Messages
        {
            get { return messages; }
            set { messages = value; }
        }
        public List<Partition> Partitions
        {
            get { return partitions; }
            set { partitions = value; }
        }
        public MasterBootRecord(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            bootCode = sw.ReadBytes(0x163);
            while (sw.Position < 0x1B2)
            {
                messages.Add(new ELEMENTARY_TYPE(sw, offset, Encoding.Default));
            }
            sw.Position = 0x1be;
            for (int i = 0; i < 4; i++)
            {
                Partition p = new Partition(sw, offset);
                partitions.Add(p);
            }
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class Partition : LOCALIZED_DATA//16 bytes
    {
        private bool bootable;
        private ChsSector startingSector;
        private PartitionType partitionType;
        private ChsSector lastSector;
        private ELEMENTARY_TYPE relativeSector;
        private ELEMENTARY_TYPE totalSectors;
        private ulong partitionSize;
        private IBootRecord boot;
        private Directory rootDirectory;
        public long directoryOffset;
        private MasterBootRecord mbr_ext;
        private long startPartition;
        protected FAT fat;

        MFT mft;
        #region Properties
        public IBootRecord Boot_Sector
        {
            get { return boot; }
            set { boot = value; }
        }
        public MFT Master_File_Table
        {
            get { return mft; }
            set { mft = value; }
        }
        public ulong Partition_Size
        {
            get { return partitionSize; }
            set { partitionSize = value; }
        }
        public bool Bootable
        {
            get { return bootable; }
            set { bootable = value; }
        }
        public ChsSector Starting_Sector
        {
            get { return startingSector; }
            set { startingSector = value; }
        }
        public PartitionType Partition_Type
        {
            get { return partitionType; }
            set { partitionType = value; }
        }
        public ChsSector Last_Sector
        {
            get { return lastSector; }
            set { lastSector = value; }
        }
        public ELEMENTARY_TYPE Start_Sector
        {
            get { return relativeSector; }
            set { relativeSector = value; }
        }
        public ELEMENTARY_TYPE Total_Sectors
        {
            get { return totalSectors; }
            set { totalSectors = value; }
        }
        public Directory Root_Directory
        {
            get { return rootDirectory; }
            set { rootDirectory = value; }
        }
        public long Start_Partition
        {
            get { return startPartition; }
            set { startPartition = value; }
        }
        public MasterBootRecord Extended_Partition_Master_Boot
        {
            get { return mbr_ext; }
            set { mbr_ext = value; }
        }
        #endregion
        public Partition(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            bootable = (sw.ReadByte() == 0x80);
            startingSector = new ChsSector(sw, offset);
            partitionType = (PartitionType)sw.ReadByte();
            lastSector = new ChsSector(sw, offset);
            relativeSector = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//sw.ReadInteger();
            totalSectors = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//sw.ReadInteger();
            partitionSize = (ulong)(uint)totalSectors.Value * 512;
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
         }
        public void ReadFat()
        {
            switch (partitionType)
            {
                case PartitionType.FAT12:
                case PartitionType.FAT16:
                    break;
                case LowLevel.PartitionType.Win95_OSR2_FAT32_adressage_LBA:
                    break;
            }
        }
        public long ClusterToSector(long cluster)
        {
            switch (partitionType)
            {
                case LowLevel.PartitionType.NTFS:
                    return cluster * (byte)Boot_Sector.Sectors_Per_Cluster.Value + (uint)Boot_Sector.Hidden_Sectors.Value + startPartition;
                case LowLevel.PartitionType.Dell_Utility:
                case LowLevel.PartitionType.FAT12:
                case LowLevel.PartitionType.FAT16:
                    return directoryOffset + /*((short)((BootRecord_Fat16)Boot_Sector).Root_Directory_Entries.Value * 0x20)/(short) Boot_Sector.Bytes_Per_Sector.Value+*/ (cluster) * (byte)((BootRecord_Fat16)Boot_Sector).Sectors_Per_Cluster.Value;
                case LowLevel.PartitionType.Win95_OSR2_FAT32_adressage_LBA:
                    return directoryOffset + (byte)((BootRecord_Fat32)Boot_Sector).Sectors_Per_Cluster.Value * (cluster - 2);
                default: return -1;
            }
        }
        public override string ToString()
        {
            return Partition_Type.ToString();
        }
    }
    public class ChsSector : LOCALIZED_DATA
    {
        public long cylinder;
        public long head;
        public long sector;
        public ChsSector(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset;
            byte[] b = sw.ReadBytes(3);
            head = b[0];
            sector = b[1] & 0x3f;
            cylinder = (b[1] & 0xC0) * 256 + b[2];
            LengthInFile = 3;
        }
        public override string ToString()
        {
            return "Head : " + head.ToString("x2") + ", Cylinder : " + cylinder.ToString("x2") + ", Secteur :" + sector.ToString("x2");
        }
    }
    //http://en.wikipedia.org/wiki/File_Allocation_Table#Boot_Sector
    public interface IBootRecord
    {
        ELEMENTARY_TYPE Bytes_Per_Sector { get; set; }
        ELEMENTARY_TYPE Hidden_Sectors { get; set; }
        ELEMENTARY_TYPE Jmp { get; set; }
        MediaType Media_Type { get; }
        ELEMENTARY_TYPE Number_Of_Heads { get; set; }
        ELEMENTARY_TYPE OEM { get; set; }
        ELEMENTARY_TYPE Reserved_Sectors { get; set; }
        ELEMENTARY_TYPE Sectors_Per_Cluster { get; set; }
        ELEMENTARY_TYPE Sectors_Per_Track { get; set; }
    }
    public class BootRecord : LOCALIZED_DATA, LowLevel.IBootRecord
    {
        ELEMENTARY_TYPE jmp;//0x000
        ELEMENTARY_TYPE oEM;//0x003
        ELEMENTARY_TYPE bytesPerSector;//0x000B Bytes Per ChsSector
        ELEMENTARY_TYPE sectorsPerCluster;//0x000D Sectors Per Cluster
        ELEMENTARY_TYPE reservedSectors;//0x000E Reserved Sectors
        ELEMENTARY_TYPE fatCopies;//0x0010 NumberOfFats
        ELEMENTARY_TYPE rootDirEntries;//0x0011 Root Entries
        ELEMENTARY_TYPE numSectors;//0x0013 Small Sectors
        ELEMENTARY_TYPE mediaType;//0x0015 Media Descriptor
        ELEMENTARY_TYPE sectorsPerFAT;//0x0016 Sectors Per FAT
        ELEMENTARY_TYPE sectorsPerTrack;//0x0018 Sectors Per Track
        ELEMENTARY_TYPE numberOfHeads;//0x001A Number of Heads
        ELEMENTARY_TYPE hiddenSectors;//0x001C Hidden Sectors
        ELEMENTARY_TYPE totalSectors;//0x20
        ELEMENTARY_TYPE signature;//0x1FE
        int sizeOfSystemArea;
        public int sizeOfFat;
        #region Properties
        public ELEMENTARY_TYPE Jmp
        {
            get { return jmp; }
            set { jmp = value; }
        }
        public ELEMENTARY_TYPE OEM
        {
            get { return oEM; }
            set { oEM = value; }
        }
        public ELEMENTARY_TYPE Bytes_Per_Sector
        {
            get { return bytesPerSector; }
            set { bytesPerSector = value; }
        }
        public ELEMENTARY_TYPE Sectors_Per_Cluster
        {
            get { return sectorsPerCluster; }
            set { sectorsPerCluster = value; }
        }
        public ELEMENTARY_TYPE Reserved_Sectors
        {
            get { return reservedSectors; }
            set { reservedSectors = value; }
        }
        public ELEMENTARY_TYPE Number_of_Fats
        {
            get { return fatCopies; }
            set { fatCopies = value; }
        }
        public ELEMENTARY_TYPE Root_Directory_Entries
        {
            get { return rootDirEntries; }
            set { rootDirEntries = value; }
        }
        public ELEMENTARY_TYPE Number_Sectors
        {
            get { return numSectors; }
            set { numSectors = value; }
        }
        public MediaType Media_Type
        {
            get { return (MediaType)(byte)mediaType.Value; }
        }
        public ELEMENTARY_TYPE Sectors_Per_FAT
        {
            get { return sectorsPerFAT; }
            set { sectorsPerFAT = value; }
        }
        public ELEMENTARY_TYPE Sectors_Per_Track
        {
            get { return sectorsPerTrack; }
            set { sectorsPerTrack = value; }
        }
        public ELEMENTARY_TYPE Number_Of_Heads
        {
            get { return numberOfHeads; }
            set { numberOfHeads = value; }
        }
        public ELEMENTARY_TYPE Hidden_Sectors
        {
            get { return hiddenSectors; }
            set { hiddenSectors = value; }
        }
        public int Size_of_System_Area
        {
            get { return sizeOfSystemArea; }
            set { sizeOfSystemArea = value; }
        }
        public ELEMENTARY_TYPE Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        #endregion
        public BootRecord(BitStreamReader sw, long offset)
        {

            PositionOfStructureInFile = sw.Position + offset;
            jmp = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 3);
            oEM = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 8);// sw.ReadString(8);
            bytesPerSector = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            sectorsPerCluster = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            reservedSectors = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            fatCopies = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            rootDirEntries = new ELEMENTARY_TYPE(sw, offset, typeof(short)); //sw.ReadShort();
            numSectors = new ELEMENTARY_TYPE(sw, offset, typeof(short)); //sw.ReadShort();
            mediaType = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            sectorsPerFAT = new ELEMENTARY_TYPE(sw, offset, typeof(short));// sw.ReadShort();
            sectorsPerTrack = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            numberOfHeads = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            hiddenSectors = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//sw.ReadInteger();
            totalSectors = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//sw.ReadInteger();
            sizeOfFat = (short)sectorsPerFAT.Value;
        }
        public override string ToString()
        {
            return "Boot sector";
        }
    }
    public class BootRecord_Fat16 : BootRecord
    {
        #region Extended BIOS Parameter Block
        ELEMENTARY_TYPE driveNumber;//0x24
        byte reserved;//0x25
        ELEMENTARY_TYPE eBSignature;//0x26
        ELEMENTARY_TYPE volumeID;//0x27
        ELEMENTARY_TYPE volumeLabel;//0x2B
        ELEMENTARY_TYPE fileSystem;//0x36
        byte[] bootCode;//0x3E
        #endregion
        
        public ELEMENTARY_TYPE Drive_Number
        {
            get { return driveNumber; }
            set { driveNumber = value; }
        }
        public ELEMENTARY_TYPE EBSignature
        {
            get { return eBSignature; }
            set { eBSignature = value; }
        }
        public ELEMENTARY_TYPE Volume_ID
        {
            get { return volumeID; }
            set { volumeID = value; }
        }
        public ELEMENTARY_TYPE Volume_Label
        {
            get { return volumeLabel; }
            set { volumeLabel = value; }
        }
        public ELEMENTARY_TYPE File_System
        {
            get { return fileSystem; }
            set { fileSystem = value; }
        }
        public BootRecord_Fat16(BitStreamReader sw, long offset)
            : base(sw, offset)
        {

            driveNumber = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            reserved = sw.ReadByte();
            eBSignature = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            volumeID = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            volumeLabel = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 11);// sw.ReadString(11);
            fileSystem = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 8);// sw.ReadString(8);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
            bootCode = sw.ReadBytes(448);
            sw.Position = 0x1FE;
            Signature = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2);
            Size_of_System_Area = (short)Reserved_Sectors.Value + (byte)Number_of_Fats.Value * (short)Sectors_Per_FAT.Value;
            Size_of_System_Area += (int)Math.Ceiling((float)((short)Root_Directory_Entries.Value * 0x20) / (float)(short)Bytes_Per_Sector.Value);
         }
        public int ClusterToSector(int cluster)
        {
            return Size_of_System_Area + (cluster - 2) * (int)Sectors_Per_Cluster.Value;
        }
        public override string ToString()
        {
            return "Boot sector";
        }
    }
    public class BootRecord_Fat32 : BootRecord
    {
        ELEMENTARY_TYPE mirroringFlags;//0x28
        ELEMENTARY_TYPE version;//0x2A
        ELEMENTARY_TYPE clusterOfRootDirectory;//0x2C
        ELEMENTARY_TYPE sectorFS; //0x30
        ELEMENTARY_TYPE sectorOfFat;//0x32
        byte[] reserved32;//0x34
        ELEMENTARY_TYPE driveNb32; //0x40;
        byte res;//0x41
        ELEMENTARY_TYPE extBoot32;//0x42
        ELEMENTARY_TYPE volId32;//0x43
        ELEMENTARY_TYPE volLabel32;//0x47
        ELEMENTARY_TYPE fileSystem32;//0x52
        public ELEMENTARY_TYPE Mirroring_Flags
        {
            get { return mirroringFlags; }
            set { mirroringFlags = value; }
        }
        public ELEMENTARY_TYPE Version
        {
            get { return version; }
            set { version = value; }
        }
        public ELEMENTARY_TYPE Cluster_Of_Root_Directory
        {
            get { return clusterOfRootDirectory; }
            set { clusterOfRootDirectory = value; }
        }
        public ELEMENTARY_TYPE Sector_FS
        {
            get { return sectorFS; }
            set { sectorFS = value; }
        }
        public ELEMENTARY_TYPE Sector_Of_Fat
        {
            get { return sectorOfFat; }
            set { sectorOfFat = value; }
        }
        public ELEMENTARY_TYPE DriveNb32
        {
            get { return driveNb32; }
            set { driveNb32 = value; }
        }
        public ELEMENTARY_TYPE ExtBoot32
        {
            get { return extBoot32; }
            set { extBoot32 = value; }
        }

        public ELEMENTARY_TYPE VolId32
        {
            get { return volId32; }
            set { volId32 = value; }
        }
        public ELEMENTARY_TYPE VolLabel32
        {
            get { return volLabel32; }
            set { volLabel32 = value; }
        }
        public ELEMENTARY_TYPE FileSystem32
        {
            get { return fileSystem32; }
            set { fileSystem32 = value; }
        }

        public BootRecord_Fat32(BitStreamReader sw, long offset)
            : base(sw, offset)
        {
            Sectors_Per_FAT = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            mirroringFlags = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            version = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            clusterOfRootDirectory = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            sectorFS = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            sectorOfFat = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            sw.ReadBytes(12);
            driveNb32 = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            sw.ReadByte();
            extBoot32 = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            volId32 = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            volLabel32 = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 11);// sw.ReadString(11);
            fileSystem32 = new ELEMENTARY_TYPE(sw, offset, Encoding.Default, 8);// sw.ReadString(8);
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
            sw.Position = 0x1FE;
            Signature = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2); //sw.ReadBytes(2);
            Size_of_System_Area = (short)Reserved_Sectors.Value + (byte)Number_of_Fats.Value * (int)Sectors_Per_FAT.Value + (int)Math.Ceiling((float)((short)Root_Directory_Entries.Value * 32) / (float)(short)Bytes_Per_Sector.Value);
            sizeOfFat = (int)Sectors_Per_FAT.Value;
        }
        public int ClusterToSector(int cluster)
        {
            return Size_of_System_Area + (int)Reserved_Sectors.Value + (int)Sectors_Per_FAT.Value * (byte)Number_of_Fats.Value + (cluster - 2) * (int)Sectors_Per_Cluster.Value;
        }

        public override string ToString()
        {
            return "Boot sector";
        }
    }
    public class BootRecord_Dell : BootRecord
    {
        string volLabel32;//0x2b
        string fileSystem32;//0x36

        public string VolLabel32
        {
            get { return volLabel32; }
            set { volLabel32 = value; }
        }
        public string FileSystem32
        {
            get { return fileSystem32; }
            set { fileSystem32 = value; }
        }

        public BootRecord_Dell(BitStreamReader sw, long sectorNumber)
            : base(sw, sectorNumber)
        {
            sw.Position = 0x2B;
            volLabel32 = sw.ReadString(11);
            fileSystem32 = sw.ReadString(8);
            sw.Position = 0x1FE;
            Signature = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2); ;
        }
        public override string ToString()
        {
            return "Boot sector";
        }
    }
    public class BootRecord_NTFS : BootRecord
    {
        byte[] notUsed2;//0x20 : 8 bytes
        ELEMENTARY_TYPE totalSectors;//0x28
        ELEMENTARY_TYPE mftStart;//0x30
        ELEMENTARY_TYPE mftMirrStart;//0x38
        ELEMENTARY_TYPE clusterPerFileRecordSegment;//0x40

        ELEMENTARY_TYPE clusterPerIndex;//0x44
        ELEMENTARY_TYPE volSerialNumber;//0x48
        ELEMENTARY_TYPE crc;//0x50

        public ELEMENTARY_TYPE Total_Sectors
        {
            get { return totalSectors; }
            set { totalSectors = value; }
        }
        public ELEMENTARY_TYPE MFT_Start
        {
            get { return mftStart; }
            set { mftStart = value; }
        }
        public ELEMENTARY_TYPE MFT_Mirr_Start
        {
            get { return mftMirrStart; }
            set { mftMirrStart = value; }
        }
        public ELEMENTARY_TYPE Cluster_Per_File_Record_Segment
        {
            get { return clusterPerFileRecordSegment; }
            set { clusterPerFileRecordSegment = value; }
        }
        public ELEMENTARY_TYPE Cluster_Per_Index
        {
            get { return clusterPerIndex; }
            set { clusterPerIndex = value; }
        }
        public ELEMENTARY_TYPE Volume_Serial_Number
        {
            get { return volSerialNumber; }
            set { volSerialNumber = value; }
        }
        public ELEMENTARY_TYPE Crc
        {
            get { return crc; }
            set { crc = value; }
        }
        public BootRecord_NTFS(BitStreamReader sw, long offset)
            : base(sw, offset)
        {

            //      PositionOfStructureInFile = sw.Position + Offset;
            notUsed2 = sw.ReadBytes(4);
            totalSectors = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            mftStart = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            mftMirrStart = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            clusterPerFileRecordSegment = new ELEMENTARY_TYPE(sw, offset, typeof(int));// sw.ReadInteger();
            clusterPerIndex = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            volSerialNumber = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 8); //sw.ReadBytes(8);
            crc = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            LengthInFile = sw.Position + offset - PositionOfStructureInFile;
            sw.Position = 0x1FE;
            Signature = new ELEMENTARY_TYPE(sw, offset, typeof(byte[]), 2); // sw.ReadBytes(2);
        }
        public override string ToString()
        {
            return "Boot sector";
        }
    }
    // The "FS Information ChsSector" was introduced in FAT32 for speeding up access times of certain operations 
    // (in particular, getting the amount of free space). It is located type_Code a Sector number specified in the boot record type_Code position 0x30 
    // (usually Sector 1, immediately after the boot record).
    public class FSInformationSector : LOCALIZED_DATA
    {
        public string rrAA;
        byte[] reserved = new byte[480];
        public string rrAA2;
        int numberOfFreeCLusters;
        int mostRecentlyAllocatedCluster;
        public FSInformationSector(BitStreamReader sw)
        {
            rrAA = sw.ReadString(4);
            reserved = sw.ReadBytes(480);
            rrAA2 = sw.ReadString(4);
            numberOfFreeCLusters = sw.ReadInteger();
            mostRecentlyAllocatedCluster = sw.ReadInteger();
        }
    }
}
