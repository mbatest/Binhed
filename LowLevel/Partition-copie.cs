using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace LowLevel
{
    public class MasterBootRecord:RAW_ACCESS_DATA
    {
        public byte[] bootCode;
        private List<string> messages = new List<string>();
        private List<Partition> partitions = new List<Partition>();
        public List<string> Messages
        {
            get { return messages; }
            set { messages = value; }
        }
        public List<Partition> Partitions
        {
            get { return partitions; }
            set { partitions = value; }
        }
        public MasterBootRecord(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            bootCode = sw.ReadBytes(0x163);
            while (sw.Position < 0x1B2)
            {
                messages.Add(sw.ReadStringZ(Encoding.Default));
            }
            sw.Position= 0x1be;
            for (int i = 0; i < 4; i++)
            {
                Partition p = new Partition(sw);
                partitions.Add(p);
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Master boot record";
        }
    }
    public class Partition:RAW_ACCESS_DATA//16 bytes
    {
        private bool bootable;
        private ChsSector startingSector;
        private PartitionType partitionType;
        private ChsSector lastSector;
        private ELEMENTARY_TYPE relativeSector;
        private ELEMENTARY_TYPE totalSectors;
        private uint partitionSize;
        private IBootRecord boot;
        private RootDirectory rootDirectory;
        MFT mft;

        public IBootRecord Boot
        {
            get { return boot; }
            set { boot = value; }
        }
        public MFT Mft
        {
            get { return mft; }
            set { mft = value; }
        }
        public uint PartitionSize
        {
            get { return partitionSize; }
            set { partitionSize = value; }
        }
        public bool Bootable
        {
            get { return bootable; }
            set { bootable = value; }
        }
        public ChsSector StartingSector
        {
            get { return startingSector; }
            set { startingSector = value; }
        }
        public PartitionType PartitionType
        {
            get { return partitionType; }
            set { partitionType = value; }
        }
        public ChsSector LastSector
        {
            get { return lastSector; }
            set { lastSector = value; }
        }
        public ELEMENTARY_TYPE StartSector
        {
            get { return relativeSector; }
            set { relativeSector = value; }
        }
        public ELEMENTARY_TYPE TotalSectors
        {
            get { return totalSectors; }
            set { totalSectors = value; }
        }
        public RootDirectory RootDirectory
        {
            get { return rootDirectory; }
            set { rootDirectory = value; }
        }
        public Partition(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            bootable = (sw.ReadByte() == 0x80);
            startingSector = new ChsSector(sw);
            partitionType = (PartitionType)sw.ReadByte();
            lastSector = new ChsSector(sw);
            relativeSector = new ELEMENTARY_TYPE(sw, typeof(uint));//sw.ReadInteger();
            totalSectors = new ELEMENTARY_TYPE(sw, typeof(uint));//sw.ReadInteger();
            partitionSize = (uint)totalSectors.Value * 512;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return PartitionType.ToString();
        }
    }
    public class ChsSector:RAW_ACCESS_DATA
    {
        public long cylinder;
        public long head;
        public long sector;
        public ChsSector(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            byte[] b = sw.ReadBytes(3);
            head = b[0];
            sector = b[1] & 0x3f;
            cylinder = (b[1] & 0xC0) * 256 + b[2];
            LengthInFile = 3;
        }
        public override string ToString()
        {
            return head.ToString() + "," + cylinder.ToString() + "," + sector.ToString();
        }
    }
    //http://en.wikipedia.org/wiki/File_Allocation_Table#Boot_Sector
    public interface IBootRecord
    {
        ELEMENTARY_TYPE BytesPerSector { get; set; }
        ELEMENTARY_TYPE HiddenSectors { get; set; }
        byte[] Jmp { get; set; }
        MediaType MediaType { get; }
        ELEMENTARY_TYPE NumberOfHeads { get; set; }
        ELEMENTARY_TYPE OEM { get; set; }
        ELEMENTARY_TYPE ReservedSectors { get; set; }
        ELEMENTARY_TYPE SectorsPerCluster { get; set; }
        ELEMENTARY_TYPE SectorsPerTrack { get; set; }
    }
    public class BootRecord : RAW_ACCESS_DATA, LowLevel.IBootRecord
    {
        byte[] jmp;
        ELEMENTARY_TYPE oEM;
        ELEMENTARY_TYPE bytesPerSector;//0x000B Bytes Per ChsSector
        ELEMENTARY_TYPE sectorsPerCluster;//0x000D Sectors Per Cluster
        ELEMENTARY_TYPE reservedSectors;//0x000E Reserved Sectors
        byte fatCopies;//0x0010 Number of FATs
        short rootDirEntries;//0x0011 Root Entries
        short numSectors;//0x0013 Small Sectors
        byte mediaType;//0x0015 Media Descriptor
        short sectorsPerFAT;//0x0016 Sectors Per FAT
        ELEMENTARY_TYPE sectorsPerTrack;//0x0018 Sectors Per Track
        ELEMENTARY_TYPE numberOfHeads;//0x001A Number of Heads
        ELEMENTARY_TYPE hiddenSectors;//0x001C Hidden Sectors
        ELEMENTARY_TYPE totalSectors;//0x20
        byte[] signature;//0x1FE
        int sizeOfSystemArea;

        public byte[] Jmp
        {
            get { return jmp; }
            set { jmp = value; }
        }
        public ELEMENTARY_TYPE OEM
        {
            get { return oEM; }
            set { oEM = value; }
        }
        public ELEMENTARY_TYPE BytesPerSector
        {
            get { return bytesPerSector; }
            set { bytesPerSector = value; }
        }
        public ELEMENTARY_TYPE SectorsPerCluster
        {
            get { return sectorsPerCluster; }
            set { sectorsPerCluster = value; }
        }
        public ELEMENTARY_TYPE ReservedSectors
        {
            get { return reservedSectors; }
            set { reservedSectors = value; }
        }
        public byte FatCopies
        {
            get { return fatCopies; }
            set { fatCopies = value; }
        }
        public short RootDirEntries
        {
            get { return rootDirEntries; }
            set { rootDirEntries = value; }
        }
        public short NumSectors
        {
            get { return numSectors; }
            set { numSectors = value; }
        }
        public MediaType MediaType
        {
            get { return (MediaType)mediaType; }
        }
        public short SectorsPerFAT
        {
            get { return sectorsPerFAT; }
            set { sectorsPerFAT = value; }
        }
        public ELEMENTARY_TYPE SectorsPerTrack
        {
            get { return sectorsPerTrack; }
            set { sectorsPerTrack = value; }
        }
        public ELEMENTARY_TYPE NumberOfHeads
        {
            get { return numberOfHeads; }
            set { numberOfHeads = value; }
        }
        public ELEMENTARY_TYPE HiddenSectors
        {
            get { return hiddenSectors; }
            set { hiddenSectors = value; }
        }
        public int SizeOfSystemArea
        {
            get { return sizeOfSystemArea; }
            set { sizeOfSystemArea = value; }
        }
        public ELEMENTARY_TYPE TotalSectors
        {
            get { return totalSectors; }
            set { totalSectors = value; }
        }
        public byte[] Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        public BootRecord(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset * 512;
            jmp = sw.ReadBytes(3);
            oEM = new ELEMENTARY_TYPE(sw, 0, 4, Encoding.Default);// sw.ReadString(8);
            bytesPerSector = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            sectorsPerCluster = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            reservedSectors = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            fatCopies = sw.ReadByte();
            rootDirEntries = sw.ReadShort();
            numSectors = sw.ReadShort();
            mediaType = sw.ReadByte();
            sectorsPerFAT = sw.ReadShort();
            sectorsPerTrack = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            numberOfHeads = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            hiddenSectors = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//sw.ReadInteger();
            totalSectors = sw.ReadInteger();
        }
        public override string ToString()
        {
            return "Boot sector";
        }
    }
    public class BootRecord_FAT : BootRecord
    {
        byte[] jmp;
        ELEMENTARY_TYPE oEM;
        ELEMENTARY_TYPE bytesPerSector;//0x000B Bytes Per ChsSector
        ELEMENTARY_TYPE sectorsPerCluster;//0x000D Sectors Per Cluster
        ELEMENTARY_TYPE reservedSectors;//0x000E Reserved Sectors
        byte fatCopies;//0x0010 Number of FATs
        short rootDirEntries;//0x0011 Root Entries
        short numSectors;//0x0013 Small Sectors
        byte mediaType;//0x0015 Media Descriptor
        short sectorsPerFAT;//0x0016 Sectors Per FAT
        ELEMENTARY_TYPE sectorsPerTrack;//0x0018 Sectors Per Track
        ELEMENTARY_TYPE numberOfHeads;//0x001A Number of Heads
        ELEMENTARY_TYPE hiddenSectors;//0x001C Hidden Sectors
        int totalSectors;//0x20
        byte[] signature;//0x1FE
        int sizeOfSystemArea;

        public byte[] Jmp
        {
            get { return jmp; }
            set { jmp = value; }
        }
        public ELEMENTARY_TYPE OEM
        {
            get { return oEM; }
            set { oEM = value; }
        }
        public ELEMENTARY_TYPE BytesPerSector
        {
            get { return bytesPerSector; }
            set { bytesPerSector = value; }
        }
        public ELEMENTARY_TYPE SectorsPerCluster
        {
            get { return sectorsPerCluster; }
            set { sectorsPerCluster = value; }
        }
        public ELEMENTARY_TYPE ReservedSectors
        {
            get { return reservedSectors; }
            set { reservedSectors = value; }
        }
        public byte FatCopies
        {
            get { return fatCopies; }
            set { fatCopies = value; }
        }
        public short RootDirEntries
        {
            get { return rootDirEntries; }
            set { rootDirEntries = value; }
        }
        public short NumSectors
        {
            get { return numSectors; }
            set { numSectors = value; }
        }
        public MediaType MediaType
        {
            get { return (MediaType)mediaType; }
        }
        public short SectorsPerFAT
        {
            get { return sectorsPerFAT; }
            set { sectorsPerFAT = value; }
        }
        public ELEMENTARY_TYPE SectorsPerTrack
        {
            get { return sectorsPerTrack; }
            set { sectorsPerTrack = value; }
        }
        public ELEMENTARY_TYPE NumberOfHeads
        {
            get { return numberOfHeads; }
            set { numberOfHeads = value; }
        }
        public ELEMENTARY_TYPE HiddenSectors
        {
            get { return hiddenSectors; }
            set { hiddenSectors = value; }
        }
        public int SizeOfSystemArea
        {
            get { return sizeOfSystemArea; }
            set { sizeOfSystemArea = value; }
        }
        public int TotalSectors
        {
            get { return totalSectors; }
            set { totalSectors = value; }
        }
        public byte[] Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        public BootRecord_FAT(BitStreamReader sw, long offset): base(sw,offset);
        {
            PositionOfStructureInFile = sw.Position + offset * 512;
            jmp = sw.ReadBytes(3);
            oEM = new ELEMENTARY_TYPE(sw, 0, 4, Encoding.Default);// sw.ReadString(8);
            bytesPerSector = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            sectorsPerCluster = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            reservedSectors = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            fatCopies = sw.ReadByte();
            rootDirEntries = sw.ReadShort();
            numSectors = sw.ReadShort();
            mediaType = sw.ReadByte();
            sectorsPerFAT = sw.ReadShort();
            sectorsPerTrack = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            numberOfHeads = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            hiddenSectors = new ELEMENTARY_TYPE(sw, offset, typeof(uint));//sw.ReadInteger();
            totalSectors = sw.ReadInteger();
        }
        public override string ToString()
        {
            return "Boot sector";
        }
    }
    public class BootRecord_Fat16 : BootRecord_FAT
    {
        #region Extended BIOS Parameter Block
        ELEMENTARY_TYPE driveNumber;//0x24
        byte reserved;//0x25
        ELEMENTARY_TYPE eBSignature;//0x26
        ELEMENTARY_TYPE volumeID;//0x27
        string volumeLabel;//0x2B
        string fileSystem;//0x36
        byte[] bootCode;//0x3E
        #endregion

        public ELEMENTARY_TYPE DriveNumber
        {
            get { return driveNumber; }
            set { driveNumber = value; }
        }
        public ELEMENTARY_TYPE EBSignature
        {
            get { return eBSignature; }
            set { eBSignature = value; }
        }
        public ELEMENTARY_TYPE VolumeID
        {
            get { return volumeID; }
            set { volumeID = value; }
        }
        public string VolumeLabel
        {
            get { return volumeLabel; }
            set { volumeLabel = value; }
        }
        public string FileSystem
        {
            get { return fileSystem; }
            set { fileSystem = value; }
        }
        public BootRecord_Fat16(BitStreamReader sw, long offset)
            : base(sw, offset)
        {
            driveNumber = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadByte();
            reserved = sw.ReadByte();
            eBSignature = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            volumeID = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            volumeLabel = sw.ReadString(11);
            fileSystem = sw.ReadString(8);
            LengthInFile = sw.Position + offset * 512 - PositionOfStructureInFile;
            bootCode = sw.ReadBytes(448);
            sw.Position = 0x1FE;
            Signature = sw.ReadBytes(2);
            SizeOfSystemArea = (short)ReservedSectors.Value + FatCopies * SectorsPerFAT;
            SizeOfSystemArea += (int)Math.Ceiling((float)(RootDirEntries * 0x20) / (float)(short)BytesPerSector.Value);
   //         LengthInFile = sw.Position + offset * 0x200 - PositionOfStructureInFile;
        }
        public int ClusterToSector(int cluster)
        {
            return SizeOfSystemArea + (cluster - 2) * (int)SectorsPerCluster.Value;
        }
        public override string ToString()
        {
            return "Boot sector";
        }
    }
    public class BootRecord_Fat32 : BootRecord_FAT
    {
        int sectorPerFat32;//0x24
        short mirroringFlags;//0x28
        short version;//0x2A
        int clusterOfRootDirectory;//0x2C
        short sectorFS; //0x30
        short sectorOfFat;//0x32
        byte[] reserved32;//0x34
        byte driveNb32; //0x40;
        byte res;//0x41
        byte extBoot32;//0x42
        int volId32;//0x43
        string volLabel32;//0x47
        string fileSystem32;//0x52
        public int SectorPerFat32
        {
            get { return sectorPerFat32; }
            set { sectorPerFat32 = value; }
        }
        public short MirroringFlags
        {
            get { return mirroringFlags; }
            set { mirroringFlags = value; }
        }
        public short Version
        {
            get { return version; }
            set { version = value; }
        }
        public int ClusterOfRootDirectory
        {
            get { return clusterOfRootDirectory; }
            set { clusterOfRootDirectory = value; }
        }
        public short SectorFS
        {
            get { return sectorFS; }
            set { sectorFS = value; }
        }
        public short SectorOfFat
        {
            get { return sectorOfFat; }
            set { sectorOfFat = value; }
        }
        public byte DriveNb32
        {
            get { return driveNb32; }
            set { driveNb32 = value; }
        }
        public byte ExtBoot32
        {
            get { return extBoot32; }
            set { extBoot32 = value; }
        }

        public int VolId32
        {
            get { return volId32; }
            set { volId32 = value; }
        }
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

        public BootRecord_Fat32(BitStreamReader sw, long sectorNumber)
            : base(sw, sectorNumber)
        {
            sectorPerFat32 = sw.ReadInteger();
            mirroringFlags = sw.ReadShort();
            version = sw.ReadShort();
            clusterOfRootDirectory = sw.ReadInteger();
            sectorFS = sw.ReadShort();
            sectorOfFat = sw.ReadShort();
            sw.ReadBytes(12);
            driveNb32 = sw.ReadByte();
            sw.ReadByte();
            extBoot32 = sw.ReadByte();
            volId32 = sw.ReadInteger();
            volLabel32 = sw.ReadString(11);
            fileSystem32 = sw.ReadString(8);
            LengthInFile = sw.Position + sectorNumber * 512 - PositionOfStructureInFile;
            sw.Position = 0x1FE;
            Signature = sw.ReadBytes(2);
            SizeOfSystemArea = (int)ReservedSectors.Value + FatCopies * sectorPerFat32 + (int)Math.Ceiling((float)(RootDirEntries * 32) / (float)BytesPerSector.Value);
        }
        public int ClusterToSector(int cluster)
        {
            return SizeOfSystemArea + (int)ReservedSectors.Value + SectorPerFat32 * FatCopies + (cluster - 2) * (int)SectorsPerCluster.Value;
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
            Signature = sw.ReadBytes(2);
        }
        public override string ToString()
        {
            return "Boot sector";
        }
    }
    public class BootRecord_NTFS : BootRecord
    {
        private byte[] jmp;
        ELEMENTARY_TYPE oEM;
        ELEMENTARY_TYPE bytesPerSector;//0x000B Bytes Per ChsSector
        ELEMENTARY_TYPE sectorsPerCluster;//0x000D Sectors Per Cluster
        ELEMENTARY_TYPE reservedSectors;//0x000E Reserved Sectors
        byte[] notUsed;//0x0010 5 bytes not udes     
        byte mediaType;//0x0015 Media Descriptor
        short reserved;//0x0016 
        ELEMENTARY_TYPE sectorsPerTrack;//0x0018 Sectors Per Track
        ELEMENTARY_TYPE numberOfHeads;//0x001A Number of Heads
        ELEMENTARY_TYPE hiddenSectors;//0x001C Number of Heads
        byte[] notUsed2;//0x20 : 8 bytes
        ELEMENTARY_TYPE totalSectors;//0x28
        ELEMENTARY_TYPE mftStart;//0x30
        ELEMENTARY_TYPE mftMirrStart;//0x38
        ELEMENTARY_TYPE clusterPerFileRecordSegment;//0x40

        ELEMENTARY_TYPE clusterPerIndex;//0x44
        byte[] volSerialNumber;//0x58
        ELEMENTARY_TYPE crc;
        byte[] signature;//0x1FE
        public ELEMENTARY_TYPE OEM
        {
            get { return oEM; }
            set { oEM = value; }
        }
        public ELEMENTARY_TYPE BytesPerSector
        {
            get { return bytesPerSector; }
            set { bytesPerSector = value; }
        }
        public ELEMENTARY_TYPE SectorsPerCluster
        {
            get { return sectorsPerCluster; }
            set { sectorsPerCluster = value; }
        }
        public ELEMENTARY_TYPE ReservedSectors
        {
            get { return reservedSectors; }
            set { reservedSectors = value; }
        }
        public MediaType MediaType
        {
            get { return (MediaType)mediaType; }
        }
        public ELEMENTARY_TYPE SectorsPerTrack
        {
            get { return sectorsPerTrack; }
            set { sectorsPerTrack = value; }
        }
        public ELEMENTARY_TYPE NumberOfHeads
        {
            get { return numberOfHeads; }
            set { numberOfHeads = value; }
        }
        public ELEMENTARY_TYPE HiddenSectors
        {
            get { return hiddenSectors; }
            set { hiddenSectors = value; }
        }
        public ELEMENTARY_TYPE TotalSectors
        {
            get { return totalSectors; }
            set { totalSectors = value; }
        }
        public byte[] Jmp
        {
            get { return jmp; }
            set { jmp = value; }
        }
        public ELEMENTARY_TYPE MFTStart
        {
            get { return mftStart; }
            set { mftStart = value; }
        }
        public ELEMENTARY_TYPE MFTMirrStart
        {
            get { return mftMirrStart; }
            set { mftMirrStart = value; }
        }
        public ELEMENTARY_TYPE ClusterPerFileRecordSegment
        {
            get { return clusterPerFileRecordSegment; }
            set { clusterPerFileRecordSegment = value; }
        }
        public ELEMENTARY_TYPE ClusterPerIndex
        {
            get { return clusterPerIndex; }
            set { clusterPerIndex = value; }
        }
        public byte[] VolSerialNumber
        {
            get { return volSerialNumber; }
            set { volSerialNumber = value; }
        }
        public ELEMENTARY_TYPE Crc
        {
            get { return crc; }
            set { crc = value; }
        }
        public byte[] Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        public BootRecord_NTFS(BitStreamReader sw, long offset)
        {
            PositionOfStructureInFile = sw.Position + offset * 0x200;
            jmp = sw.ReadBytes(3);
            oEM = new ELEMENTARY_TYPE(sw, offset, 8, Encoding.Default);// sw.ReadString(8);
            bytesPerSector = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            sectorsPerCluster = new ELEMENTARY_TYPE(sw, offset, typeof(byte));//sw.ReadByte();
            reservedSectors = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            notUsed = sw.ReadBytes(5);
            mediaType = sw.ReadByte();
            sw.ReadShort();
            sectorsPerTrack = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            numberOfHeads = new ELEMENTARY_TYPE(sw, offset, typeof(short));//sw.ReadShort();
            hiddenSectors = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            notUsed2 = sw.ReadBytes(8);
            totalSectors = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            mftStart = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            mftMirrStart = new ELEMENTARY_TYPE(sw, offset, typeof(long));//sw.ReadLong();
            clusterPerFileRecordSegment = new ELEMENTARY_TYPE(sw, offset, typeof(int));// sw.ReadInteger();
            clusterPerIndex = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            volSerialNumber = sw.ReadBytes(8);
            crc = new ELEMENTARY_TYPE(sw, offset, typeof(int));//sw.ReadInteger();
            LengthInFile = sw.Position + offset * 0x200 - PositionOfStructureInFile;
         }
        public override string ToString()
        {
            return "Boot sector";
        }
    }
    // The "FS Information ChsSector" was introduced in FAT32 for speeding up access times of certain operations 
    // (in particular, getting the amount of free space). It is located type_Code a sector number specified in the boot record type_Code position 0x30 
    // (usually sector 1, immediately after the boot record).
    public class FSInformationSector:RAW_ACCESS_DATA
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
    public class FileList
    {
        public LinkedList<int> list;
    }
    public class FAT_16 : RAW_ACCESS_DATA
    {
        List<FileList> files;
        public FAT_16(byte[] buffer)
        {
            files = new List<FileList>();
        }
    }
    public class Root16Entry : RAW_ACCESS_DATA
    {
        private string name;
        private byte[] nameBytes;
        private string extension;
        private byte[] extensionByte;
        private byte attributes;//0B
        private byte reserved;
        private byte createHour;//0D
        private byte[] createTime;
        private byte[] createDate;
        private byte[] lastAccessDate;
        private byte[] lastModifiedTime;
        private byte[] lastModifiedDate;
        private short startOfFile;
        private byte[] sizeOfFile;
        private bool isDeleted;

        public int SequenceNumber
        {
            get
            {
                if (LongFileName)
                    return ((byte)name[0]) & 0x1F;
                else
                    return -1;
            }
        }
        public bool LongFileName
        {
            get { return (attributes & 0x0F) == 0x0F; }
        }
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }
        public byte Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
        public bool IsReadOnly
        {
            get { return (attributes & 0x01) == 0x01; }
        }
        public bool IsHidden
        {
            get { return (attributes & 0x02) == 0x02; }
        }
        public bool IsSystem
        {
            get { return (attributes & 0x04) == 0x04; }
        }
        public bool IsVolumeLabel
        {
            get { return (attributes & 0x08) == 0x08; }
        }
        public bool IsSubdirectory
        {
            get { return (attributes & 0x10) == 0x10; }
        }
        public bool IsArchive
        {
            get { return (attributes & 0x20) == 0x20; }
        }
        public bool IsDevice
        {
            get { return (attributes & 0x40) == 0x40; }
        }
        public byte CreateHour
        {
            get { return createHour; }
            set { createHour = value; }
        }
 /*       public short CreateTime
        {
            get { return Convert.ToInt16(createTime); }
        }
        public short CreateDate
        {
            get { return  Convert.ToInt16(createDate); }
        }
        public short LastAccessDate
        {
            get { return Convert.ToInt16(lastAccessDate); }
        }*/
        private byte[] eaIndex;
 /*      
        public short LastModifiedTime
        {
            get { return Convert.ToInt16(lastModifiedTime); }
        }
        public short LastModifiedDate
        {
            get { return Convert.ToInt16(lastModifiedDate); }
        }*/
        public short StartOfFile
        {
            get { return startOfFile; }
            set { startOfFile = value; }
        }
  /*      public int SizeOfFile
        {
            get { return Convert.ToInt32(sizeOfFile); }
        }*/
        public Root16Entry(BitStreamReader sw, long sectorNumber)
        {
            PositionOfStructureInFile = sw.Position + sectorNumber * 0x200;
            nameBytes= sw.ReadBytes(11);
            attributes = sw.ReadByte();
            reserved = sw.ReadByte();
            createHour = sw.ReadByte();
            createTime = sw.ReadBytes(2);
            createDate = sw.ReadBytes(2);
            lastAccessDate = sw.ReadBytes(2);
            eaIndex = sw.ReadBytes(2);
            lastModifiedTime = sw.ReadBytes(2);
            lastModifiedDate = sw.ReadBytes(2);
            startOfFile = sw.ReadShort();
            sizeOfFile = sw.ReadBytes(4);
            LengthInFile = 0x20;
            switch (nameBytes[0])
            {
                case 0xE5:
                    isDeleted = true;
                    break;
                case 0x05:
                    nameBytes[0] = 0xE5;
                    break;
                case 0x2E:
                    
                    break;
            }
            if (LongFileName)
            {
                List<byte> by = new List<byte>();
                name = Encoding.Unicode.GetString(nameBytes, 1, 10);
                name += Encoding.Unicode.GetString(createTime);
                name += Encoding.Unicode.GetString(createDate);
                name += Encoding.Unicode.GetString(lastAccessDate);
                name += Encoding.Unicode.GetString(eaIndex);
                name += Encoding.Unicode.GetString(lastModifiedTime);
                name += Encoding.Unicode.GetString(lastModifiedDate);
                name += Encoding.Unicode.GetString(sizeOfFile);
            }
            else
            {
                name = Encoding.Default.GetString(nameBytes, 0, 8);
                extension = Encoding.Default.GetString(nameBytes, 8, 3);
            }
        }
        private DateTime ShortToDate(short u)
        {
            return new DateTime();
        }
        public override string ToString()
        {
            return name.Trim() + "." + extension;
        }
    }
    public class RootDirectory : RAW_ACCESS_DATA
    {
        private List<Root16Entry> entries = new List<Root16Entry>();
        private long sector;

        public long Sector
        {
            get { return sector; }
            set { sector = value; }
        }
        public List<Root16Entry> Entries
        {
            get { return entries; }
            set { entries = value; }
        }
        public RootDirectory(BitStreamReader sw, int numberOfEntries, long sectorNumber)
        {
            PositionOfStructureInFile = sw.Position + sectorNumber * 0x200;
            sector = sectorNumber;
            for (int i = 0; i < numberOfEntries; i++)
            {
                
                entries.Add(new Root16Entry(sw, sectorNumber));
            }
            LengthInFile = 0x20 * numberOfEntries;
        }
        public override string ToString()
        {
            return "Root directory";
        }
    }
}
