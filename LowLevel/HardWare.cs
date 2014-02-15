using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using Utils;

namespace LowLevel
{
    public class Win32_BIOS
    {
        public ushort[] BiosCharacteristics;
        public string[] BIOSVersion;
        public string BuildNumber;
        public string Caption;
        public string CodeSet;
        public string CurrentLanguage;
        public string Description;
        public string IdentificationCode;
        public ushort InstallableLanguages;
        public DateTime InstallDate;
        public string LanguageEdition;
        public string[] ListOfLanguages;
        public string Manufacturer;
        public string Name;
        public string OtherTargetOS;
        public bool PrimaryBIOS;
        public DateTime ReleaseDate;
        public string SerialNumber;
        public string SMBIOSBIOSVersion;
        public ushort SMBIOSMajorVersion;
        public ushort SMBIOSMinorVersion;
        public bool SMBIOSPresent;
        public string SoftwareElementID;
        public ushort SoftwareElementState;
        public string Status;
        public ushort TargetOperatingSystem;
        public string Version;
        public Win32_BIOS(ManagementObject o)
        {
            if (o["BiosCharacteristics"] != null) BiosCharacteristics = (ushort[])o["BiosCharacteristics"];
            if (o["BIOSVersion"] != null) BIOSVersion = (string[])o["BIOSVersion"];
            if (o["BuildNumber"] != null) BuildNumber = (string)o["BuildNumber"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["CodeSet"] != null) CodeSet = (string)o["CodeSet"];
            if (o["CurrentLanguage"] != null) CurrentLanguage = (string)o["CurrentLanguage"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["IdentificationCode"] != null) IdentificationCode = (string)o["IdentificationCode"];
            if (o["InstallableLanguages"] != null) InstallableLanguages = (ushort)o["InstallableLanguages"];
            if (o["InstallDate"] != null) InstallDate = (DateTime)o["InstallDate"];
            if (o["LanguageEdition"] != null) LanguageEdition = (string)o["LanguageEdition"];
            if (o["ListOfLanguages"] != null) ListOfLanguages = (string[])o["ListOfLanguages"];
            if (o["Manufacturer"] != null) Manufacturer = (string)o["Manufacturer"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["OtherTargetOS"] != null) OtherTargetOS = (string)o["OtherTargetOS"];
            if (o["PrimaryBIOS"] != null) PrimaryBIOS = (bool)o["PrimaryBIOS"];
         //   if (value["ReleaseDate"] != null) ReleaseDate = (DateTime)value["ReleaseDate"];
            if (o["SerialNumber"] != null) SerialNumber = (string)o["SerialNumber"];
            if (o["SMBIOSBIOSVersion"] != null) SMBIOSBIOSVersion = (string)o["SMBIOSBIOSVersion"];
            if (o["SMBIOSMajorVersion"] != null) SMBIOSMajorVersion = (ushort)o["SMBIOSMajorVersion"];
            if (o["SMBIOSMinorVersion"] != null) SMBIOSMinorVersion = (ushort)o["SMBIOSMinorVersion"];
            if (o["SMBIOSPresent"] != null) SMBIOSPresent = (bool)o["SMBIOSPresent"];
            if (o["SoftwareElementID"] != null) SoftwareElementID = (string)o["SoftwareElementID"];
            if (o["SoftwareElementState"] != null) SoftwareElementState = (ushort)o["SoftwareElementState"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["TargetOperatingSystem"] != null) TargetOperatingSystem = (ushort)o["TargetOperatingSystem"];
            if (o["Version"] != null) Version = (string)o["Version"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_BootConfiguration
    {
        public string BootDirectory;
        public string Caption;
        public string ConfigurationPath;
        public string Description;
        public string LastDrive;
        public string Name;
        public string ScratchDirectory;
        public string SettingID;
        public string TempDirectory;
        public Win32_BootConfiguration(ManagementObject o)
        {
            if (o["BootDirectory"] != null) BootDirectory = (string)o["BootDirectory"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigurationPath"] != null) ConfigurationPath = (string)o["ConfigurationPath"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["LastDrive"] != null) LastDrive = (string)o["LastDrive"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["ScratchDirectory"] != null) ScratchDirectory = (string)o["ScratchDirectory"];
            if (o["SettingID"] != null) SettingID = (string)o["SettingID"];
            if (o["TempDirectory"] != null) TempDirectory = (string)o["TempDirectory"];


        }
    }
    public class Win32_DiskPartition
    {
        public ushort Access;
        public ushort Availability;
        public ulong BlockSize;
        public bool Bootable;
        public bool BootPartition;
        public string Caption;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public string Description;
        public string DeviceID;
        public uint DiskIndex;
        public bool ErrorCleared;
        public string ErrorDescription;
        public string ErrorMethodology;
        public uint HiddenSectors;
        public uint Index;
        public DateTime InstallDate;
        public uint LastErrorCode;
        public string Name;
        public ulong NumberOfBlocks;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public bool PrimaryPartition;
        public string Purpose;
        public bool RewritePartition;
        public ulong Size;
        public ulong StartingOffset;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;
        public string Type;
        public Win32_DiskPartition(ManagementObject o)
        {
            if (o["Access"] != null) Access = (ushort)o["Access"];
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["BlockSize"] != null) BlockSize = (ulong)o["BlockSize"];
            if (o["Bootable"] != null) Bootable = (bool)o["Bootable"];
            if (o["BootPartition"] != null) BootPartition = (bool)o["BootPartition"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["DiskIndex"] != null) DiskIndex = (uint)o["DiskIndex"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["ErrorMethodology"] != null) ErrorMethodology = (string)o["ErrorMethodology"];
            if (o["HiddenSectors"] != null) HiddenSectors = (uint)o["HiddenSectors"];
            if (o["Index"] != null) Index = (uint)o["Index"];
            if (o["InstallDate"] != null) InstallDate = (DateTime)o["InstallDate"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfBlocks"] != null) NumberOfBlocks = (ulong)o["NumberOfBlocks"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["PrimaryPartition"] != null) PrimaryPartition = (bool)o["PrimaryPartition"];
            if (o["Purpose"] != null) Purpose = (string)o["Purpose"];
            if (o["RewritePartition"] != null) RewritePartition = (bool)o["RewritePartition"];
            if (o["Size"] != null) Size = (ulong)o["Size"];
            if (o["StartingOffset"] != null) StartingOffset = (ulong)o["StartingOffset"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
            if (o["Type"] != null) Type = (string)o["Type"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_Fan
    {
        public bool ActiveCooling;
        public ushort Availability;
        public string Caption;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public string Description;
        public long DesiredSpeed;
        public string DeviceID;
        public bool ErrorCleared;
        public string ErrorDescription;
        public DateTime InstallDate;
        public uint LastErrorCode;
        public string Name;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;
        public bool VariableSpeed;
        public Win32_Fan(ManagementObject o)
        {
            if (o["ActiveCooling"] != null) ActiveCooling = (bool)o["ActiveCooling"];
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DesiredSpeed"] != null) DesiredSpeed = (long)o["DesiredSpeed"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["InstallDate"] != null) InstallDate = (DateTime)o["InstallDate"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
            if (o["VariableSpeed"] != null) VariableSpeed = (bool)o["VariableSpeed"];

        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_MotherboardDevice
    {
        public ushort Availability;
        public string Caption;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public string Description;
        public string DeviceID;
        public bool ErrorCleared;
        public string ErrorDescription;
        public DateTime InstallDate;
        public uint LastErrorCode;
        public string Name;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string PrimaryBusType;
        public string RevisionNumber;
        public string SecondaryBusType;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;
        public Win32_MotherboardDevice(ManagementObject o)
        {
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["InstallDate"] != null) InstallDate = (DateTime)o["InstallDate"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["PrimaryBusType"] != null) PrimaryBusType = (string)o["PrimaryBusType"];
            if (o["RevisionNumber"] != null) RevisionNumber = (string)o["RevisionNumber"];
            if (o["SecondaryBusType"] != null) SecondaryBusType = (string)o["SecondaryBusType"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];

        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_Processor
    {
        public ushort AddressWidth;
        public ushort Architecture;
        public ushort Availability;
        public string Caption;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public ushort CpuStatus;
        public string CreationClassName;
        public uint CurrentClockSpeed;
        public ushort CurrentVoltage;
        public ushort DataWidth;
        public string Description;
        public string DeviceID;
        public bool ErrorCleared;
        public string ErrorDescription;
        public uint ExtClock;
        public ushort Family;
        public DateTime InstallDate;
        public uint L2CacheSize;
        public uint L2CacheSpeed;
        public uint L3CacheSize;
        public uint L3CacheSpeed;
        public uint LastErrorCode;
        public ushort Level;
        public ushort LoadPercentage;
        public string Manufacturer;
        public uint MaxClockSpeed;
        public string Name;
        public uint NumberOfCores;
        public uint NumberOfLogicalProcessors;
        public string OtherFamilyDescription;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string ProcessorId;
        public ushort ProcessorType;
        public ushort Revision;
        public string Role;
        public string SocketDesignation;
        public string Status;
        public ushort StatusInfo;
        public string Stepping;
        public string SystemCreationClassName;
        public string SystemName;
        public string UniqueId;
        public ushort UpgradeMethod;
        public string Version;
        public uint VoltageCaps;
        public Win32_Processor(ManagementObject o)
        {
            if (o["AddressWidth"] != null) AddressWidth = (ushort)o["AddressWidth"];
            if (o["Architecture"] != null) Architecture = (ushort)o["Architecture"];
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CpuStatus"] != null) CpuStatus = (ushort)o["CpuStatus"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["CurrentClockSpeed"] != null) CurrentClockSpeed = (uint)o["CurrentClockSpeed"];
            if (o["CurrentVoltage"] != null) CurrentVoltage = (ushort)o["CurrentVoltage"];
            if (o["DataWidth"] != null) DataWidth = (ushort)o["DataWidth"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["ExtClock"] != null) ExtClock = (uint)o["ExtClock"];
            if (o["Family"] != null) Family = (ushort)o["Family"];
            if (o["InstallDate"] != null) InstallDate = (DateTime)o["InstallDate"];
            if (o["L2CacheSize"] != null) L2CacheSize = (uint)o["L2CacheSize"];
            if (o["L2CacheSpeed"] != null) L2CacheSpeed = (uint)o["L2CacheSpeed"];
            if (o["L3CacheSize"] != null) L3CacheSize = (uint)o["L3CacheSize"];
            if (o["L3CacheSpeed"] != null) L3CacheSpeed = (uint)o["L3CacheSpeed"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Level"] != null) Level = (ushort)o["Level"];
            if (o["LoadPercentage"] != null) LoadPercentage = (ushort)o["LoadPercentage"];
            if (o["Manufacturer"] != null) Manufacturer = (string)o["Manufacturer"];
            if (o["MaxClockSpeed"] != null) MaxClockSpeed = (uint)o["MaxClockSpeed"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfCores"] != null) NumberOfCores = (uint)o["NumberOfCores"];
            if (o["NumberOfLogicalProcessors"] != null) NumberOfLogicalProcessors = (uint)o["NumberOfLogicalProcessors"];
            if (o["OtherFamilyDescription"] != null) OtherFamilyDescription = (string)o["OtherFamilyDescription"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["ProcessorId"] != null) ProcessorId = (string)o["ProcessorId"];
            if (o["ProcessorType"] != null) ProcessorType = (ushort)o["ProcessorType"];
            if (o["Revision"] != null) Revision = (ushort)o["Revision"];
            if (o["Role"] != null) Role = (string)o["Role"];
            if (o["SocketDesignation"] != null) SocketDesignation = (string)o["SocketDesignation"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["Stepping"] != null) Stepping = (string)o["Stepping"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
            if (o["UniqueId"] != null) UniqueId = (string)o["UniqueId"];
            if (o["UpgradeMethod"] != null) UpgradeMethod = (ushort)o["UpgradeMethod"];
            if (o["Version"] != null) Version = (string)o["Version"];
            if (o["VoltageCaps"] != null) VoltageCaps = (uint)o["VoltageCaps"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    //http://msdn.microsoft.com/en-us/library/windows/desktop/aa394081(v=VS.85).aspx
    public class Win32_DiskDrive:LOCALIZED_DATA
    {
        private ushort availability;
        private uint bytesPerSector;
       public ushort[] capabilities;
         private string[] capabilityDescriptions;
        private string caption;
        private string compressionMethod;
        private uint configManagerErrorCode;
        private bool configManagerUserConfig;
        private string creationClassName;
        private ulong defaultBlockSize;
        private string description;
        private string deviceID;

        private string systemName;
        private ulong totalCylinders;
        private uint totalHeads;
        private ulong totalSectors;
        private ulong totalTracks;
        private uint tracksPerCylinder;
        public ushort Availability
        {
            get { return availability; }
            set { availability = value; }
        }
        public string CompressionMethod
        {
            get { return compressionMethod; }
            set { compressionMethod = value; }
        }
        public uint ConfigManagerErrorCode
        {
            get { return configManagerErrorCode; }
            set { configManagerErrorCode = value; }
        }
        public bool ConfigManagerUserConfig
        {
            get { return configManagerUserConfig; }
            set { configManagerUserConfig = value; }
        }
        public string CreationClassName
        {
            get { return creationClassName; }
            set { creationClassName = value; }
        }
        public ulong DefaultBlockSize
        {
            get { return defaultBlockSize; }
            set { defaultBlockSize = value; }
        }
        public string[] CapabilityDescriptions
        {
            get { return capabilityDescriptions; }
            set { capabilityDescriptions = value; }
        }
        public uint BytesPerSector
        {
            get { return bytesPerSector; }
            set { bytesPerSector = value; }
        }
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string DeviceID
        {
            get { return deviceID; }
            set { deviceID = value; }
        }
        public bool ErrorCleared;
        public string ErrorDescription;
        public string ErrorMethodology;
        public string FirmwareRevision;
        public uint Index;
        public DateTime InstallDate;
        public string InterfaceType;
        public uint LastErrorCode;
        public string Manufacturer;
        public ulong MaxBlockSize;
        public ulong MaxMediaSize;
        public bool MediaLoaded;
        public string MediaType;
        public ulong MinBlockSize;
        public string Model;
        public string Name;
        public bool NeedsCleaning;
        public uint NumberOfMediaSupported;
        public uint Partitions;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public uint SCSIBus;
        public ushort SCSILogicalUnit;
        public ushort SCSIPort;
        public ushort SCSITargetId;
        private uint sectorsPerTrack;

        public uint SectorsPerTrack
        {
            get { return sectorsPerTrack; }
            set { sectorsPerTrack = value; }
        }
        public string serialNumber;
        private uint signature;
        private ulong size;

        public uint Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        public ulong Size
        {
            get { return size; }
            set { size = value; }
        }
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName
        {
            get { return systemName; }
            set { systemName = value; }
        }
        public ulong TotalCylinders
        {
            get { return totalCylinders; }
            set { totalCylinders = value; }
        }
        public uint TotalHeads
        {
            get { return totalHeads; }
            set { totalHeads = value; }
        }
        public ulong TotalSectors
        {
            get { return totalSectors; }
            set { totalSectors = value; }
        }
        public ulong TotalTracks
        {
            get { return totalTracks; }
            set { totalTracks = value; }
        }
        public uint TracksPerCylinder
        {
            get { return tracksPerCylinder; }
            set { tracksPerCylinder = value; }
        }
        public Win32_DiskDrive(ManagementObject o)
        {
            if (o["Availability"] != null) availability = (ushort)o["Availability"];
            if (o["BytesPerSector"] != null) bytesPerSector = (uint)o["BytesPerSector"];
            if (o["Capabilities"] != null) capabilities = (ushort[])o["Capabilities"];
            if (o["CapabilityDescriptions"] != null) capabilityDescriptions = (string[])o["CapabilityDescriptions"];
            if (o["Caption"] != null) caption = (string)o["Caption"];
            if (o["CompressionMethod"] != null) compressionMethod = (string)o["CompressionMethod"];
            if (o["ConfigManagerErrorCode"] != null) configManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) configManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) creationClassName = (string)o["CreationClassName"];
            if (o["DefaultBlockSize"] != null) defaultBlockSize = (ulong)o["DefaultBlockSize"];
            if (o["Description"] != null) description = (string)o["Description"];
            if (o["DeviceID"] != null) deviceID = (string)o["DeviceID"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["ErrorMethodology"] != null) ErrorMethodology = (string)o["ErrorMethodology"];
            if (o["FirmwareRevision"] != null) FirmwareRevision = (string)o["FirmwareRevision"];
            if (o["Index"] != null) Index = (uint)o["Index"];
            if (o["InstallDate"] != null) InstallDate = (DateTime)o["InstallDate"];
            if (o["InterfaceType"] != null) InterfaceType = (string)o["InterfaceType"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Manufacturer"] != null) Manufacturer = (string)o["Manufacturer"];
            if (o["MaxBlockSize"] != null) MaxBlockSize = (ulong)o["MaxBlockSize"];
            if (o["MaxMediaSize"] != null) MaxMediaSize = (ulong)o["MaxMediaSize"];
            if (o["MediaLoaded"] != null) MediaLoaded = (bool)o["MediaLoaded"];
            if (o["MediaType"] != null) MediaType = (string)o["MediaType"];
            if (o["MinBlockSize"] != null) MinBlockSize = (ulong)o["MinBlockSize"];
            if (o["Model"] != null) Model = (string)o["Model"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NeedsCleaning"] != null) NeedsCleaning = (bool)o["NeedsCleaning"];
            if (o["NumberOfMediaSupported"] != null) NumberOfMediaSupported = (uint)o["NumberOfMediaSupported"];
            if (o["Partitions"] != null) Partitions = (uint)o["Partitions"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["SCSIBus"] != null) SCSIBus = (uint)o["SCSIBus"];
            if (o["SCSILogicalUnit"] != null) SCSILogicalUnit = (ushort)o["SCSILogicalUnit"];
            if (o["SCSIPort"] != null) SCSIPort = (ushort)o["SCSIPort"];
            if (o["SCSITargetId"] != null) SCSITargetId = (ushort)o["SCSITargetId"];
            if (o["SectorsPerTrack"] != null) sectorsPerTrack = (uint)o["SectorsPerTrack"];
            if (o["SerialNumber"] != null) serialNumber = (string)o["SerialNumber"];
            if (o["Signature"] != null) signature = (uint)o["Signature"];
            if (o["Size"] != null) size = (ulong)o["Size"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) systemName = (string)o["SystemName"];
            if (o["TotalCylinders"] != null) totalCylinders = (ulong)o["TotalCylinders"];
            if (o["TotalHeads"] != null) totalHeads = (uint)o["TotalHeads"];
            if (o["TotalSectors"] != null) totalSectors = (ulong)o["TotalSectors"];
            if (o["TotalTracks"] != null) totalTracks = (ulong)o["TotalTracks"];
            if (o["TracksPerCylinder"] != null) tracksPerCylinder = (uint)o["TracksPerCylinder"];
        }
        public override string ToString()
        {
            return DeviceID + " : "+ Caption;
        }
    }
    public class Win32_Battery
    {
        public ushort Availability;
        public uint BatteryRechargeTime;
        public ushort BatteryStatus;
        public string Caption;
        public ushort Chemistry;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public string Description;
        public uint DesignCapacity;
        public ulong DesignVoltage;
        public string DeviceID;
        public bool ErrorCleared;
        public string ErrorDescription;
        public ushort EstimatedChargeRemaining;
        public uint EstimatedRunTime;
        public uint ExpectedBatteryLife;
        public uint ExpectedLife;
        public uint FullChargeCapacity;
        public DateTime InstallDate;
        public uint LastErrorCode;
        public uint MaxRechargeTime;
        public string Name;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string SmartBatteryVersion;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;
        public uint TimeOnBattery;
        public uint TimeToFullCharge;
        public Win32_Battery(ManagementObject o)
        {
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["BatteryRechargeTime"] != null) BatteryRechargeTime = (uint)o["BatteryRechargeTime"];
            if (o["BatteryStatus"] != null) BatteryStatus = (ushort)o["BatteryStatus"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["Chemistry"] != null) Chemistry = (ushort)o["Chemistry"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DesignCapacity"] != null) DesignCapacity = (uint)o["DesignCapacity"];
            if (o["DesignVoltage"] != null) DesignVoltage = (ulong)o["DesignVoltage"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["EstimatedChargeRemaining"] != null) EstimatedChargeRemaining = (ushort)o["EstimatedChargeRemaining"];
            if (o["EstimatedRunTime"] != null) EstimatedRunTime = (uint)o["EstimatedRunTime"];
            if (o["ExpectedBatteryLife"] != null) ExpectedBatteryLife = (uint)o["ExpectedBatteryLife"];
            if (o["ExpectedLife"] != null) ExpectedLife = (uint)o["ExpectedLife"];
            if (o["FullChargeCapacity"] != null) FullChargeCapacity = (uint)o["FullChargeCapacity"];
            if (o["InstallDate"] != null) InstallDate = (DateTime)o["InstallDate"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["MaxRechargeTime"] != null) MaxRechargeTime = (uint)o["MaxRechargeTime"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["SmartBatteryVersion"] != null) SmartBatteryVersion = (string)o["SmartBatteryVersion"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
            if (o["TimeOnBattery"] != null) TimeOnBattery = (uint)o["TimeOnBattery"];
            if (o["TimeToFullCharge"] != null) TimeToFullCharge = (uint)o["TimeToFullCharge"];
        }
        public override string ToString()
        {
            return "Win32 battery";
        }
    }
    public class Win32_CDROMDrive
    {
        public ushort Availability;
        public ushort[] Capabilities;
        public string[] CapabilityDescriptions;
        public string Caption;
        public string CompressionMethod;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public ulong DefaultBlockSize;
        public string Description;
        public string DeviceID;
        public string Drive;
        public bool DriveIntegrity;
        public bool ErrorCleared;
        public string ErrorDescription;
        public string ErrorMethodology;
        public ushort FileSystemFlags;
        public uint FileSystemFlagsEx;
        public string Id;
        public DateTime InstallDate;
        public uint LastErrorCode;
        public string Manufacturer;
        public ulong MaxBlockSize;
        public uint MaximumComponentLength;
        public ulong MaxMediaSize;
        public bool MediaLoaded;
        public string MediaType;
        public string MfrAssignedRevisionLevel;
        public ulong MinBlockSize;
        public string Name;
        public bool NeedsCleaning;
        public uint NumberOfMediaSupported;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string RevisionLevel;
        public uint SCSIBus;
        public ushort SCSILogicalUnit;
        public ushort SCSIPort;
        public ushort SCSITargetId;
        public string SerialNumber;
        public ulong Size;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;
        public double TransferRate;
        public string VolumeName;
        public string VolumeSerialNumber;
        public Win32_CDROMDrive(ManagementObject o)
        {
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["Capabilities"] != null) Capabilities = (ushort[])o["Capabilities"];
            if (o["CapabilityDescriptions"] != null) CapabilityDescriptions = (string[])o["CapabilityDescriptions"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["CompressionMethod"] != null) CompressionMethod = (string)o["CompressionMethod"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["DefaultBlockSize"] != null) DefaultBlockSize = (ulong)o["DefaultBlockSize"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["Drive"] != null) Drive = (string)o["Drive"];
            if (o["DriveIntegrity"] != null) DriveIntegrity = (bool)o["DriveIntegrity"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["ErrorMethodology"] != null) ErrorMethodology = (string)o["ErrorMethodology"];
            if (o["FileSystemFlags"] != null) FileSystemFlags = (ushort)o["FileSystemFlags"];
            if (o["FileSystemFlagsEx"] != null) FileSystemFlagsEx = (uint)o["FileSystemFlagsEx"];
            if (o["Id"] != null) Id = (string)o["Id"];
            if (o["InstallDate"] != null) InstallDate = (DateTime)o["InstallDate"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Manufacturer"] != null) Manufacturer = (string)o["Manufacturer"];
            if (o["MaxBlockSize"] != null) MaxBlockSize = (ulong)o["MaxBlockSize"];
            if (o["MaximumComponentLength"] != null) MaximumComponentLength = (uint)o["MaximumComponentLength"];
            if (o["MaxMediaSize"] != null) MaxMediaSize = (ulong)o["MaxMediaSize"];
            if (o["MediaLoaded"] != null) MediaLoaded = (bool)o["MediaLoaded"];
            if (o["MediaType"] != null) MediaType = (string)o["MediaType"];
            if (o["MfrAssignedRevisionLevel"] != null) MfrAssignedRevisionLevel = (string)o["MfrAssignedRevisionLevel"];
            if (o["MinBlockSize"] != null) MinBlockSize = (ulong)o["MinBlockSize"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NeedsCleaning"] != null) NeedsCleaning = (bool)o["NeedsCleaning"];
            if (o["NumberOfMediaSupported"] != null) NumberOfMediaSupported = (uint)o["NumberOfMediaSupported"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["RevisionLevel"] != null) RevisionLevel = (string)o["RevisionLevel"];
            if (o["SCSIBus"] != null) SCSIBus = (uint)o["SCSIBus"];
            if (o["SCSILogicalUnit"] != null) SCSILogicalUnit = (ushort)o["SCSILogicalUnit"];
            if (o["SCSIPort"] != null) SCSIPort = (ushort)o["SCSIPort"];
            if (o["SCSITargetId"] != null) SCSITargetId = (ushort)o["SCSITargetId"];
            if (o["SerialNumber"] != null) SerialNumber = (string)o["SerialNumber"];
            if (o["Size"] != null) Size = (ulong)o["Size"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
            if (o["TransferRate"] != null) TransferRate = (double)o["TransferRate"];
            if (o["VolumeName"] != null) VolumeName = (string)o["VolumeName"];
            if (o["VolumeSerialNumber"] != null) VolumeSerialNumber = (string)o["VolumeSerialNumber"];
        }
        public override string ToString()
        {
            return "";
        }
    }
    public class Win32_ComputerSystem
    {
        public ushort AdminPasswordStatus;
        public bool AutomaticManagedPagefile;
        public bool AutomaticResetBootOption;
        public bool AutomaticResetCapability;
        public ushort BootOptionOnLimit;
        public ushort BootOptionOnWatchDog;
        public bool BootROMSupported;
        public string BootupState;
        public string Caption;
        public ushort ChassisBootupState;
        public string CreationClassName;
        public short CurrentTimeZone;
        public bool DaylightInEffect;
        public string Description;
        public string DNSHostName;
        public string Domain;
        public ushort DomainRole;
        public bool EnableDaylightSavingsTime;
        public ushort FrontPanelResetStatus;
        public bool InfraredSupported;
        public string InitialLoadInfo;
        public DateTime InstallDate;
        public ushort KeyboardPasswordStatus;
        public string LastLoadInfo;
        public string Manufacturer;
        public string Model;
        public string Name;
        public string NameFormat;
        public bool NetworkServerModeEnabled;
        public uint NumberOfLogicalProcessors;
        public uint NumberOfProcessors;
        public ulong[] OEMLogoBitmap;
        public string[] OEMstringArray;
        public bool PartOfDomain;
        public long PauseAfterReset;
        public ushort PCSystemType;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public ushort PowerOnPasswordStatus;
        public ushort PowerState;
        public ushort PowerSupplyState;
        public string PrimaryOwnerContact;
        public string PrimaryOwnerName;
        public ushort ResetCapability;
        public short ResetCount;
        public short ResetLimit;
        public string[] Roles;
        public string Status;
        public string[] SupportContactDescription;
        public ushort SystemStartupDelay;
        public string[] SystemStartupOptions;
        public ulong SystemStartupSetting;
        public string SystemType;
        public ushort ThermalState;
        public ulong TotalPhysicalMemory;
        public string UserName;
        public ushort WakeUpType;
        public string Workgroup;
        public Win32_ComputerSystem(ManagementObject o)
        {
            if (o["AdminPasswordStatus"] != null) AdminPasswordStatus = (ushort)o["AdminPasswordStatus"];
            if (o["AutomaticManagedPagefile"] != null) AutomaticManagedPagefile = (bool)o["AutomaticManagedPagefile"];
            if (o["AutomaticResetBootOption"] != null) AutomaticResetBootOption = (bool)o["AutomaticResetBootOption"];
            if (o["AutomaticResetCapability"] != null) AutomaticResetCapability = (bool)o["AutomaticResetCapability"];
            if (o["BootOptionOnLimit"] != null) BootOptionOnLimit = (ushort)o["BootOptionOnLimit"];
            if (o["BootOptionOnWatchDog"] != null) BootOptionOnWatchDog = (ushort)o["BootOptionOnWatchDog"];
            if (o["BootROMSupported"] != null) BootROMSupported = (bool)o["BootROMSupported"];
            if (o["BootupState"] != null) BootupState = (string)o["BootupState"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ChassisBootupState"] != null) ChassisBootupState = (ushort)o["ChassisBootupState"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["CurrentTimeZone"] != null) CurrentTimeZone = (short)o["CurrentTimeZone"];
            if (o["DaylightInEffect"] != null) DaylightInEffect = (bool)o["DaylightInEffect"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DNSHostName"] != null) DNSHostName = (string)o["DNSHostName"];
            if (o["Domain"] != null) Domain = (string)o["Domain"];
            if (o["DomainRole"] != null) DomainRole = (ushort)o["DomainRole"];
            if (o["EnableDaylightSavingsTime"] != null) EnableDaylightSavingsTime = (bool)o["EnableDaylightSavingsTime"];
            if (o["FrontPanelResetStatus"] != null) FrontPanelResetStatus = (ushort)o["FrontPanelResetStatus"];
            if (o["InfraredSupported"] != null) InfraredSupported = (bool)o["InfraredSupported"];
            if (o["InitialLoadInfo"] != null) InitialLoadInfo = (string)o["InitialLoadInfo"];
            if (o["InstallDate"] != null) InstallDate = (DateTime)o["InstallDate"];
            if (o["KeyboardPasswordStatus"] != null) KeyboardPasswordStatus = (ushort)o["KeyboardPasswordStatus"];
            if (o["LastLoadInfo"] != null) LastLoadInfo = (string)o["LastLoadInfo"];
            if (o["Manufacturer"] != null) Manufacturer = (string)o["Manufacturer"];
            if (o["Model"] != null) Model = (string)o["Model"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NameFormat"] != null) NameFormat = (string)o["NameFormat"];
            if (o["NetworkServerModeEnabled"] != null) NetworkServerModeEnabled = (bool)o["NetworkServerModeEnabled"];
            if (o["NumberOfLogicalProcessors"] != null) NumberOfLogicalProcessors = (uint)o["NumberOfLogicalProcessors"];
            if (o["NumberOfProcessors"] != null) NumberOfProcessors = (uint)o["NumberOfProcessors"];
            if (o["OEMLogoBitmap"] != null) OEMLogoBitmap = (ulong[])o["OEMLogoBitmap"];
            if (o["OEMstringArray"] != null) OEMstringArray = (string[])o["OEMstringArray"];
            if (o["PartOfDomain"] != null) PartOfDomain = (bool)o["PartOfDomain"];
            if (o["PauseAfterReset"] != null) PauseAfterReset = (long)o["PauseAfterReset"];
            if (o["PCSystemType"] != null) PCSystemType = (ushort)o["PCSystemType"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["PowerOnPasswordStatus"] != null) PowerOnPasswordStatus = (ushort)o["PowerOnPasswordStatus"];
            if (o["PowerState"] != null) PowerState = (ushort)o["PowerState"];
            if (o["PowerSupplyState"] != null) PowerSupplyState = (ushort)o["PowerSupplyState"];
            if (o["PrimaryOwnerContact"] != null) PrimaryOwnerContact = (string)o["PrimaryOwnerContact"];
            if (o["PrimaryOwnerName"] != null) PrimaryOwnerName = (string)o["PrimaryOwnerName"];
            if (o["ResetCapability"] != null) ResetCapability = (ushort)o["ResetCapability"];
            if (o["ResetCount"] != null) ResetCount = (short)o["ResetCount"];
            if (o["ResetLimit"] != null) ResetLimit = (short)o["ResetLimit"];
            if (o["Roles"] != null) Roles = (string[])o["Roles"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["SupportContactDescription"] != null) SupportContactDescription = (string[])o["SupportContactDescription"];
            if (o["SystemStartupDelay"] != null) SystemStartupDelay = (ushort)o["SystemStartupDelay"];
            if (o["SystemStartupOptions"] != null) SystemStartupOptions = (string[])o["SystemStartupOptions"];
            if (o["SystemStartupSetting"] != null) SystemStartupSetting = (ulong)o["SystemStartupSetting"];
            if (o["SystemType"] != null) SystemType = (string)o["SystemType"];
            if (o["ThermalState"] != null) ThermalState = (ushort)o["ThermalState"];
            if (o["TotalPhysicalMemory"] != null) TotalPhysicalMemory = (ulong)o["TotalPhysicalMemory"];
            if (o["UserName"] != null) UserName = (string)o["UserName"];
            if (o["WakeUpType"] != null) WakeUpType = (ushort)o["WakeUpType"];
            if (o["Workgroup"] != null) Workgroup = (string)o["Workgroup"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_Volume
    {
        public ushort Access;
        public bool Automount;
        public ushort Availability;
        public ulong BlockSize;
        public ulong Capacity;
        public string Caption;
        public bool Compressed;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public string Description;
        public string DeviceID;
        public bool DirtyBitSet;
        public string DriveLetter;
        public uint DriveType;
        public bool ErrorCleared;
        public string ErrorDescription;
        public string ErrorMethodology;
        public string FileSystem;
        public ulong FreeSpace;
        public bool IndexingEnabled;
        public string InstallDate;
        public string Label;
        public uint LastErrorCode;
        public uint MaximumFileNameLength;
        public string Name;
        public ulong NumberOfBlocks;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string Purpose;
        public bool QuotasEnabled;
        public bool QuotasIncomplete;
        public bool QuotasRebuilding;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;
        public uint SerialNumber;
        public bool SupportsDiskQuotas;
        public bool SupportsFileBasedCompression;
        public Win32_Volume(ManagementObject o)
        {
            if (o["Access"] != null) Access = (ushort)o["Access"];
            if (o["Automount"] != null) Automount = (bool)o["Automount"];
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["BlockSize"] != null) BlockSize = (ulong)o["BlockSize"];
            if (o["Capacity"] != null) Capacity = (ulong)o["Capacity"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["Compressed"] != null) Compressed = (bool)o["Compressed"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["DirtyBitSet"] != null) DirtyBitSet = (bool)o["DirtyBitSet"];
            if (o["DriveLetter"] != null) DriveLetter = (string)o["DriveLetter"];
            if (o["DriveType"] != null) DriveType = (uint)o["DriveType"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["ErrorMethodology"] != null) ErrorMethodology = (string)o["ErrorMethodology"];
            if (o["FileSystem"] != null) FileSystem = (string)o["FileSystem"];
            if (o["FreeSpace"] != null) FreeSpace = (ulong)o["FreeSpace"];
            if (o["IndexingEnabled"] != null) IndexingEnabled = (bool)o["IndexingEnabled"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["Label"] != null) Label = (string)o["Label"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["MaximumFileNameLength"] != null) MaximumFileNameLength = (uint)o["MaximumFileNameLength"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfBlocks"] != null) NumberOfBlocks = (ulong)o["NumberOfBlocks"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["Purpose"] != null) Purpose = (string)o["Purpose"];
            if (o["QuotasEnabled"] != null) QuotasEnabled = (bool)o["QuotasEnabled"];
            if (o["QuotasIncomplete"] != null) QuotasIncomplete = (bool)o["QuotasIncomplete"];
            if (o["QuotasRebuilding"] != null) QuotasRebuilding = (bool)o["QuotasRebuilding"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
            if (o["SerialNumber"] != null) SerialNumber = (uint)o["SerialNumber"];
            if (o["SupportsDiskQuotas"] != null) SupportsDiskQuotas = (bool)o["SupportsDiskQuotas"];
            if (o["SupportsFileBasedCompression"] != null) SupportsFileBasedCompression = (bool)o["SupportsFileBasedCompression"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_LogicalDisk
    {
        public ushort Access;
        public ushort Availability;
        public ulong BlockSize;
        public string Caption;
        public bool Compressed;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public string Description;
        public string DeviceID;
        public uint DriveType;
        public bool ErrorCleared;
        public string ErrorDescription;
        public string ErrorMethodology;
        public string FileSystem;
        public ulong FreeSpace;
        public string InstallDate;
        public uint LastErrorCode;
        public uint MaximumComponentLength;
        public uint MediaType;
        public string Name;
        public ulong NumberOfBlocks;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string ProviderName;
        public string Purpose;
        public bool QuotasDisabled;
        public bool QuotasIncomplete;
        public bool QuotasRebuilding;
        public ulong Size;
        public string Status;
        public ushort StatusInfo;
        public bool SupportsDiskQuotas;
        public bool SupportsFileBasedCompression;
        public string SystemCreationClassName;
        public string SystemName;
        public bool VolumeDirty;
        public string VolumeName;
        public string VolumeSerialNumber;
        public Win32_LogicalDisk(ManagementObject o)
        {
            if (o["Access"] != null) Access = (ushort)o["Access"];
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["BlockSize"] != null) BlockSize = (ulong)o["BlockSize"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["Compressed"] != null) Compressed = (bool)o["Compressed"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["DriveType"] != null) DriveType = (uint)o["DriveType"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["ErrorMethodology"] != null) ErrorMethodology = (string)o["ErrorMethodology"];
            if (o["FileSystem"] != null) FileSystem = (string)o["FileSystem"];
            if (o["FreeSpace"] != null) FreeSpace = (ulong)o["FreeSpace"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["MaximumComponentLength"] != null) MaximumComponentLength = (uint)o["MaximumComponentLength"];
            if (o["MediaType"] != null) MediaType = (uint)o["MediaType"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfBlocks"] != null) NumberOfBlocks = (ulong)o["NumberOfBlocks"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["ProviderName"] != null) ProviderName = (string)o["ProviderName"];
            if (o["Purpose"] != null) Purpose = (string)o["Purpose"];
            if (o["QuotasDisabled"] != null) QuotasDisabled = (bool)o["QuotasDisabled"];
            if (o["QuotasIncomplete"] != null) QuotasIncomplete = (bool)o["QuotasIncomplete"];
            if (o["QuotasRebuilding"] != null) QuotasRebuilding = (bool)o["QuotasRebuilding"];
            if (o["Size"] != null) Size = (ulong)o["Size"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SupportsDiskQuotas"] != null) SupportsDiskQuotas = (bool)o["SupportsDiskQuotas"];
            if (o["SupportsFileBasedCompression"] != null) SupportsFileBasedCompression = (bool)o["SupportsFileBasedCompression"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
            if (o["VolumeDirty"] != null) VolumeDirty = (bool)o["VolumeDirty"];
            if (o["VolumeName"] != null) VolumeName = (string)o["VolumeName"];
            if (o["VolumeSerialNumber"] != null) VolumeSerialNumber = (string)o["VolumeSerialNumber"];
        }
        public override string ToString()
        {
            return Caption;
        }
    }
    public class Win32_MemoryArray
    {
        public ushort Access;
        public byte[] AdditionalErrorData;
        public ushort Availability;
        public ulong BlockSize;
        public string Caption;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public bool CorrectableError;
        public string CreationClassName;
        public string Description;
        public string DeviceID;
        public ulong EndingAddress;
        public ushort ErrorAccess;
        public ulong ErrorAddress;
        public bool ErrorCleared;
        public byte[] ErrorData;
        public ushort ErrorDataOrder;
        public string ErrorDescription;
        public ushort ErrorGranularity;
        public ushort ErrorInfo;
        public string ErrorMethodology;
        public ulong ErrorResolution;
        public string ErrorTime;
        public uint ErrorTransferSize;
        public string InstallDate;
        public uint LastErrorCode;
        public string Name;
        public ulong NumberOfBlocks;
        public string OtherErrorDescription;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string Purpose;
        public ulong StartingAddress;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public bool SystemLevelAddress;
        public string SystemName;
        public Win32_MemoryArray(ManagementObject o)
        {
            if (o["Access"] != null) Access = (ushort)o["Access"];
            if (o["AdditionalErrorData"] != null) AdditionalErrorData = (byte[])o["AdditionalErrorData"];
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["BlockSize"] != null) BlockSize = (ulong)o["BlockSize"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CorrectableError"] != null) CorrectableError = (bool)o["CorrectableError"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["EndingAddress"] != null) EndingAddress = (ulong)o["EndingAddress"];
            if (o["ErrorAccess"] != null) ErrorAccess = (ushort)o["ErrorAccess"];
            if (o["ErrorAddress"] != null) ErrorAddress = (ulong)o["ErrorAddress"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorData"] != null) ErrorData = (byte[])o["ErrorData"];
            if (o["ErrorDataOrder"] != null) ErrorDataOrder = (ushort)o["ErrorDataOrder"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["ErrorGranularity"] != null) ErrorGranularity = (ushort)o["ErrorGranularity"];
            if (o["ErrorInfo"] != null) ErrorInfo = (ushort)o["ErrorInfo"];
            if (o["ErrorMethodology"] != null) ErrorMethodology = (string)o["ErrorMethodology"];
            if (o["ErrorResolution"] != null) ErrorResolution = (ulong)o["ErrorResolution"];
            if (o["ErrorTime"] != null) ErrorTime = (string)o["ErrorTime"];
            if (o["ErrorTransferSize"] != null) ErrorTransferSize = (uint)o["ErrorTransferSize"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfBlocks"] != null) NumberOfBlocks = (ulong)o["NumberOfBlocks"];
            if (o["OtherErrorDescription"] != null) OtherErrorDescription = (string)o["OtherErrorDescription"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["Purpose"] != null) Purpose = (string)o["Purpose"];
            if (o["StartingAddress"] != null) StartingAddress = (ulong)o["StartingAddress"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemLevelAddress"] != null) SystemLevelAddress = (bool)o["SystemLevelAddress"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_MemoryDevice
    {
        public ushort Access;
        public byte[] AdditionalErrorData;
        public ushort Availability;
        public ulong BlockSize;
        public string Caption;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public bool CorrectableError;
        public string CreationClassName;
        public string Description;
        public string DeviceID;
        public ulong EndingAddress;
        public ushort ErrorAccess;
        public ulong ErrorAddress;
        public bool ErrorCleared;
        public byte[] ErrorData;
        public ushort ErrorDataOrder;
        public string ErrorDescription;
        public ushort ErrorGranularity;
        public ushort ErrorInfo;
        public string ErrorMethodology;
        public ulong ErrorResolution;
        public string ErrorTime;
        public uint ErrorTransferSize;
        public string InstallDate;
        public uint LastErrorCode;
        public string Name;
        public ulong NumberOfBlocks;
        public string OtherErrorDescription;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string Purpose;
        public ulong StartingAddress;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public bool SystemLevelAddress;
        public string SystemName;
        public Win32_MemoryDevice(ManagementObject o)
        {
            if (o["Access"] != null) Access = (ushort)o["Access"];
            if (o["AdditionalErrorData"] != null) AdditionalErrorData = (byte[])o["AdditionalErrorData"];
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["BlockSize"] != null) BlockSize = (ulong)o["BlockSize"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CorrectableError"] != null) CorrectableError = (bool)o["CorrectableError"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["EndingAddress"] != null) EndingAddress = (ulong)o["EndingAddress"];
            if (o["ErrorAccess"] != null) ErrorAccess = (ushort)o["ErrorAccess"];
            if (o["ErrorAddress"] != null) ErrorAddress = (ulong)o["ErrorAddress"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorData"] != null) ErrorData = (byte[])o["ErrorData"];
            if (o["ErrorDataOrder"] != null) ErrorDataOrder = (ushort)o["ErrorDataOrder"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["ErrorGranularity"] != null) ErrorGranularity = (ushort)o["ErrorGranularity"];
            if (o["ErrorInfo"] != null) ErrorInfo = (ushort)o["ErrorInfo"];
            if (o["ErrorMethodology"] != null) ErrorMethodology = (string)o["ErrorMethodology"];
            if (o["ErrorResolution"] != null) ErrorResolution = (ulong)o["ErrorResolution"];
            if (o["ErrorTime"] != null) ErrorTime = (string)o["ErrorTime"];
            if (o["ErrorTransferSize"] != null) ErrorTransferSize = (uint)o["ErrorTransferSize"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfBlocks"] != null) NumberOfBlocks = (ulong)o["NumberOfBlocks"];
            if (o["OtherErrorDescription"] != null) OtherErrorDescription = (string)o["OtherErrorDescription"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["Purpose"] != null) Purpose = (string)o["Purpose"];
            if (o["StartingAddress"] != null) StartingAddress = (ulong)o["StartingAddress"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemLevelAddress"] != null) SystemLevelAddress = (bool)o["SystemLevelAddress"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_Keyboard
    {
        public ushort Availability;
        public string Caption;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public string Description;
        public string DeviceID;
        public bool ErrorCleared;
        public string ErrorDescription;
        public string InstallDate;
        public bool IsLocked;
        public uint LastErrorCode;
        public string Layout;
        public string Name;
        public ushort NumberOfFunctionKeys;
        public ushort Password;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;
        public Win32_Keyboard(ManagementObject o)
        {
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["IsLocked"] != null) IsLocked = (bool)o["IsLocked"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Layout"] != null) Layout = (string)o["Layout"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfFunctionKeys"] != null) NumberOfFunctionKeys = (ushort)o["NumberOfFunctionKeys"];
            if (o["Password"] != null) Password = (ushort)o["Password"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_NetworkAdapter
    {
        public string AdapterType;
        public ushort AdapterTypeID;
        public bool AutoSense;
        public ushort Availability;
        public string Caption;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public string Description;
        public string DeviceID;
        public bool ErrorCleared;
        public string ErrorDescription;
        public string GUID;
        public uint Index;
        public string InstallDate;
        public bool Installed;
        public uint InterfaceIndex;
        public uint LastErrorCode;
        public string MACAddress;
        public string Manufacturer;
        public uint MaxNumberControlled;
        public ulong MaxSpeed;
        public string Name;
        public string NetConnectionID;
        public ushort NetConnectionStatus;
        public bool NetEnabled;
        public string[] NetworkAddresses;
        public string PermanentAddress;
        public bool PhysicalAdapter;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public string ProductName;
        public string ServiceName;
        public ulong Speed;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;
        public string TimeOfLastReset;
        public Win32_NetworkAdapter(ManagementObject o)
        {
            if (o["AdapterType"] != null) AdapterType = (string)o["AdapterType"];
            if (o["AdapterTypeID"] != null) AdapterTypeID = (ushort)o["AdapterTypeID"];
            if (o["AutoSense"] != null) AutoSense = (bool)o["AutoSense"];
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["GUID"] != null) GUID = (string)o["GUID"];
            if (o["Index"] != null) Index = (uint)o["Index"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["Installed"] != null) Installed = (bool)o["Installed"];
            if (o["InterfaceIndex"] != null) InterfaceIndex = (uint)o["InterfaceIndex"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["MACAddress"] != null) MACAddress = (string)o["MACAddress"];
            if (o["Manufacturer"] != null) Manufacturer = (string)o["Manufacturer"];
            if (o["MaxNumberControlled"] != null) MaxNumberControlled = (uint)o["MaxNumberControlled"];
            if (o["MaxSpeed"] != null) MaxSpeed = (ulong)o["MaxSpeed"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NetConnectionID"] != null) NetConnectionID = (string)o["NetConnectionID"];
            if (o["NetConnectionStatus"] != null) NetConnectionStatus = (ushort)o["NetConnectionStatus"];
            if (o["NetEnabled"] != null) NetEnabled = (bool)o["NetEnabled"];
            if (o["NetworkAddresses"] != null) NetworkAddresses = (string[])o["NetworkAddresses"];
            if (o["PermanentAddress"] != null) PermanentAddress = (string)o["PermanentAddress"];
            if (o["PhysicalAdapter"] != null) PhysicalAdapter = (bool)o["PhysicalAdapter"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["ProductName"] != null) ProductName = (string)o["ProductName"];
            if (o["ServiceName"] != null) ServiceName = (string)o["ServiceName"];
            if (o["Speed"] != null) Speed = (ulong)o["Speed"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
            if (o["TimeOfLastReset"] != null) TimeOfLastReset = (string)o["TimeOfLastReset"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_OperatingSystem
    {
        public string BootDevice;
        public string BuildNumber;
        public string BuildType;
        public string Caption;
        public string CodeSet;
        public string CountryCode;
        public string CreationClassName;
        public string CSCreationClassName;
        public string CSDVersion;
        public string CSName;
        public short CurrentTimeZone;
        public bool DataExecutionPrevention_Available;
        public bool DataExecutionPrevention_32BitApplications;
        public bool DataExecutionPrevention_Drivers;
        public byte DataExecutionPrevention_SupportPolicy;
        public bool Debug;
        public string Description;
        public bool Distributed;
        public uint EncryptionLevel;
        public byte ForegroundApplicationBoost;
        public ulong FreePhysicalMemory;
        public ulong FreeSpaceInPagingFiles;
        public ulong FreeVirtualMemory;
        public string InstallDate;
        public uint LargeSystemCache;
        public string LastBootUpTime;
        public string Localstring;
        public string Locale;
        public string Manufacturer;
        public uint MaxNumberOfProcesses;
        public ulong MaxProcessMemorySize;
        public string[] MUILanguages;
        public string Name;
        public uint NumberOfLicensedUsers;
        public uint NumberOfProcesses;
        public uint NumberOfUsers;
        public uint OperatingSystemSKU;
        public string Organization;
        public string OSArchitecture;
        public uint OSLanguage;
        public uint OSProductSuite;
        public ushort OSType;
        public string OtherTypeDescription;
        public bool PAEEnabled;
        public string PlusProductID;
        public string PlusVersionNumber;
        public bool Primary;
        public uint ProductType;
        public string RegisteredUser;
        public string SerialNumber;
        public ushort ServicePackMajorVersion;
        public ushort ServicePackMinorVersion;
        public ulong SizeStoredInPagingFiles;
        public string Status;
        public uint SuiteMask;
        public string SystemDevice;
        public string SystemDirectory;
        public string SystemDrive;
        public ulong TotalSwapSpaceSize;
        public ulong TotalVirtualMemorySize;
        public ulong TotalVisibleMemorySize;
        public string Version;
        public string WindowsDirectory;
        public Win32_OperatingSystem(ManagementObject o)
        {
            if (o["BootDevice"] != null) BootDevice = (string)o["BootDevice"];
            if (o["BuildNumber"] != null) BuildNumber = (string)o["BuildNumber"];
            if (o["BuildType"] != null) BuildType = (string)o["BuildType"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["CodeSet"] != null) CodeSet = (string)o["CodeSet"];
            if (o["CountryCode"] != null) CountryCode = (string)o["CountryCode"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["CSCreationClassName"] != null) CSCreationClassName = (string)o["CSCreationClassName"];
            if (o["CSDVersion"] != null) CSDVersion = (string)o["CSDVersion"];
            if (o["CSName"] != null) CSName = (string)o["CSName"];
            if (o["CurrentTimeZone"] != null) CurrentTimeZone = (short)o["CurrentTimeZone"];
            if (o["DataExecutionPrevention_Available"] != null) DataExecutionPrevention_Available = (bool)o["DataExecutionPrevention_Available"];
            if (o["DataExecutionPrevention_32BitApplications"] != null) DataExecutionPrevention_32BitApplications = (bool)o["DataExecutionPrevention_32BitApplications"];
            if (o["DataExecutionPrevention_Drivers"] != null) DataExecutionPrevention_Drivers = (bool)o["DataExecutionPrevention_Drivers"];
            if (o["DataExecutionPrevention_SupportPolicy"] != null) DataExecutionPrevention_SupportPolicy = (byte)o["DataExecutionPrevention_SupportPolicy"];
            if (o["Debug"] != null) Debug = (bool)o["Debug"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["Distributed"] != null) Distributed = (bool)o["Distributed"];
            if (o["EncryptionLevel"] != null) EncryptionLevel = (uint)o["EncryptionLevel"];
            if (o["ForegroundApplicationBoost"] != null) ForegroundApplicationBoost = (byte)o["ForegroundApplicationBoost"];
            if (o["FreePhysicalMemory"] != null) FreePhysicalMemory = (ulong)o["FreePhysicalMemory"];
            if (o["FreeSpaceInPagingFiles"] != null) FreeSpaceInPagingFiles = (ulong)o["FreeSpaceInPagingFiles"];
            if (o["FreeVirtualMemory"] != null) FreeVirtualMemory = (ulong)o["FreeVirtualMemory"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["LargeSystemCache"] != null) LargeSystemCache = (uint)o["LargeSystemCache"];
            if (o["LastBootUpTime"] != null) LastBootUpTime = (string)o["LastBootUpTime"];
            //     if (value["Localstring"] != null) Localstring = (string)value["Localstring"];
            if (o["Locale"] != null) Locale = (string)o["Locale"];
            if (o["Manufacturer"] != null) Manufacturer = (string)o["Manufacturer"];
            if (o["MaxNumberOfProcesses"] != null) MaxNumberOfProcesses = (uint)o["MaxNumberOfProcesses"];
            if (o["MaxProcessMemorySize"] != null) MaxProcessMemorySize = (ulong)o["MaxProcessMemorySize"];
            if (o["MUILanguages"] != null) MUILanguages = (string[])o["MUILanguages"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfLicensedUsers"] != null) NumberOfLicensedUsers = (uint)o["NumberOfLicensedUsers"];
            if (o["NumberOfProcesses"] != null) NumberOfProcesses = (uint)o["NumberOfProcesses"];
            if (o["NumberOfUsers"] != null) NumberOfUsers = (uint)o["NumberOfUsers"];
            if (o["OperatingSystemSKU"] != null) OperatingSystemSKU = (uint)o["OperatingSystemSKU"];
            if (o["Organization"] != null) Organization = (string)o["Organization"];
            if (o["OSArchitecture"] != null) OSArchitecture = (string)o["OSArchitecture"];
            if (o["OSLanguage"] != null) OSLanguage = (uint)o["OSLanguage"];
            if (o["OSProductSuite"] != null) OSProductSuite = (uint)o["OSProductSuite"];
            if (o["OSType"] != null) OSType = (ushort)o["OSType"];
            if (o["OtherTypeDescription"] != null) OtherTypeDescription = (string)o["OtherTypeDescription"];
            if (o["PAEEnabled"] != null) PAEEnabled = (bool)o["PAEEnabled"];
            if (o["PlusProductID"] != null) PlusProductID = (string)o["PlusProductID"];
            if (o["PlusVersionNumber"] != null) PlusVersionNumber = (string)o["PlusVersionNumber"];
            if (o["Primary"] != null) Primary = (bool)o["Primary"];
            if (o["ProductType"] != null) ProductType = (uint)o["ProductType"];
            if (o["RegisteredUser"] != null) RegisteredUser = (string)o["RegisteredUser"];
            if (o["SerialNumber"] != null) SerialNumber = (string)o["SerialNumber"];
            if (o["ServicePackMajorVersion"] != null) ServicePackMajorVersion = (ushort)o["ServicePackMajorVersion"];
            if (o["ServicePackMinorVersion"] != null) ServicePackMinorVersion = (ushort)o["ServicePackMinorVersion"];
            if (o["SizeStoredInPagingFiles"] != null) SizeStoredInPagingFiles = (ulong)o["SizeStoredInPagingFiles"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["SuiteMask"] != null) SuiteMask = (uint)o["SuiteMask"];
            if (o["SystemDevice"] != null) SystemDevice = (string)o["SystemDevice"];
            if (o["SystemDirectory"] != null) SystemDirectory = (string)o["SystemDirectory"];
            if (o["SystemDrive"] != null) SystemDrive = (string)o["SystemDrive"];
            if (o["TotalSwapSpaceSize"] != null) TotalSwapSpaceSize = (ulong)o["TotalSwapSpaceSize"];
            if (o["TotalVirtualMemorySize"] != null) TotalVirtualMemorySize = (ulong)o["TotalVirtualMemorySize"];
            if (o["TotalVisibleMemorySize"] != null) TotalVisibleMemorySize = (ulong)o["TotalVisibleMemorySize"];
            if (o["Version"] != null) Version = (string)o["Version"];
            if (o["WindowsDirectory"] != null) WindowsDirectory = (string)o["WindowsDirectory"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_VideoController
    {
        public ushort[] AcceleratorCapabilities;
        public string AdapterCompatibility;
        public string AdapterDACType;
        public uint AdapterRAM;
        public ushort Availability;
        public string[] CapabilityDescriptions;
        public string Caption;
        public uint ColorTableEntries;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public uint CurrentBitsPerPixel;
        public uint CurrentHorizontalResolution;
        public ulong CurrentNumberOfColors;
        public uint CurrentNumberOfColumns;
        public uint CurrentNumberOfRows;
        public uint CurrentRefreshRate;
        public ushort CurrentScanMode;
        public uint CurrentVerticalResolution;
        public string Description;
        public string DeviceID;
        public uint DeviceSpecificPens;
        public uint DitherType;
        public string DriverDate;
        public string DriverVersion;
        public bool ErrorCleared;
        public string ErrorDescription;
        public uint ICMIntent;
        public uint ICMMethod;
        public string InfFilename;
        public string InfSection;
        public string InstallDate;
        public string InstalledDisplayDrivers;
        public uint LastErrorCode;
        public uint MaxMemorySupported;
        public uint MaxNumberControlled;
        public uint MaxRefreshRate;
        public uint MinRefreshRate;
        public bool Monochrome;
        public string Name;
        public ushort NumberOfColorPlanes;
        public uint NumberOfVideoPages;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public ushort ProtocolSupported;
        public uint ReservedSystemPaletteEntries;
        public uint SpecificationVersion;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;
        public uint SystemPaletteEntries;
        public string TimeOfLastReset;
        public ushort VideoArchitecture;
        public ushort VideoMemoryType;
        public ushort VideoMode;
        public string VideoModeDescription;
        public string VideoProcessor;
        public Win32_VideoController(ManagementObject o)
        {
            if (o["AcceleratorCapabilities"] != null) AcceleratorCapabilities = (ushort[])o["AcceleratorCapabilities"];
            if (o["AdapterCompatibility"] != null) AdapterCompatibility = (string)o["AdapterCompatibility"];
            if (o["AdapterDACType"] != null) AdapterDACType = (string)o["AdapterDACType"];
            if (o["AdapterRAM"] != null) AdapterRAM = (uint)o["AdapterRAM"];
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["CapabilityDescriptions"] != null) CapabilityDescriptions = (string[])o["CapabilityDescriptions"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ColorTableEntries"] != null) ColorTableEntries = (uint)o["ColorTableEntries"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["CurrentBitsPerPixel"] != null) CurrentBitsPerPixel = (uint)o["CurrentBitsPerPixel"];
            if (o["CurrentHorizontalResolution"] != null) CurrentHorizontalResolution = (uint)o["CurrentHorizontalResolution"];
            if (o["CurrentNumberOfColors"] != null) CurrentNumberOfColors = (ulong)o["CurrentNumberOfColors"];
            if (o["CurrentNumberOfColumns"] != null) CurrentNumberOfColumns = (uint)o["CurrentNumberOfColumns"];
            if (o["CurrentNumberOfRows"] != null) CurrentNumberOfRows = (uint)o["CurrentNumberOfRows"];
            if (o["CurrentRefreshRate"] != null) CurrentRefreshRate = (uint)o["CurrentRefreshRate"];
            if (o["CurrentScanMode"] != null) CurrentScanMode = (ushort)o["CurrentScanMode"];
            if (o["CurrentVerticalResolution"] != null) CurrentVerticalResolution = (uint)o["CurrentVerticalResolution"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["DeviceSpecificPens"] != null) DeviceSpecificPens = (uint)o["DeviceSpecificPens"];
            if (o["DitherType"] != null) DitherType = (uint)o["DitherType"];
            if (o["DriverDate"] != null) DriverDate = (string)o["DriverDate"];
            if (o["DriverVersion"] != null) DriverVersion = (string)o["DriverVersion"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["ICMIntent"] != null) ICMIntent = (uint)o["ICMIntent"];
            if (o["ICMMethod"] != null) ICMMethod = (uint)o["ICMMethod"];
            if (o["InfFilename"] != null) InfFilename = (string)o["InfFilename"];
            if (o["InfSection"] != null) InfSection = (string)o["InfSection"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["InstalledDisplayDrivers"] != null) InstalledDisplayDrivers = (string)o["InstalledDisplayDrivers"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["MaxMemorySupported"] != null) MaxMemorySupported = (uint)o["MaxMemorySupported"];
            if (o["MaxNumberControlled"] != null) MaxNumberControlled = (uint)o["MaxNumberControlled"];
            if (o["MaxRefreshRate"] != null) MaxRefreshRate = (uint)o["MaxRefreshRate"];
            if (o["MinRefreshRate"] != null) MinRefreshRate = (uint)o["MinRefreshRate"];
            if (o["Monochrome"] != null) Monochrome = (bool)o["Monochrome"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfColorPlanes"] != null) NumberOfColorPlanes = (ushort)o["NumberOfColorPlanes"];
            if (o["NumberOfVideoPages"] != null) NumberOfVideoPages = (uint)o["NumberOfVideoPages"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["ProtocolSupported"] != null) ProtocolSupported = (ushort)o["ProtocolSupported"];
            if (o["ReservedSystemPaletteEntries"] != null) ReservedSystemPaletteEntries = (uint)o["ReservedSystemPaletteEntries"];
            if (o["SpecificationVersion"] != null) SpecificationVersion = (uint)o["SpecificationVersion"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
            if (o["SystemPaletteEntries"] != null) SystemPaletteEntries = (uint)o["SystemPaletteEntries"];
            if (o["TimeOfLastReset"] != null) TimeOfLastReset = (string)o["TimeOfLastReset"];
            if (o["VideoArchitecture"] != null) VideoArchitecture = (ushort)o["VideoArchitecture"];
            if (o["VideoMemoryType"] != null) VideoMemoryType = (ushort)o["VideoMemoryType"];
            if (o["VideoMode"] != null) VideoMode = (ushort)o["VideoMode"];
            if (o["VideoModeDescription"] != null) VideoModeDescription = (string)o["VideoModeDescription"];
            if (o["VideoProcessor"] != null) VideoProcessor = (string)o["VideoProcessor"];

        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_DesktopMonitor
    {
        public ushort Availability;
        public uint Bandwidth;
        public string Caption;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public string Description;
        public string DeviceID;
        public ushort DisplayType;
        public bool ErrorCleared;
        public string ErrorDescription;
        public string InstallDate;
        public bool IsLocked;
        public uint LastErrorCode;
        public string MonitorManufacturer;
        public string MonitorType;
        public string Name;
        public uint PixelsPerXLogicalInch;
        public uint PixelsPerYLogicalInch;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public uint ScreenHeight;
        public uint ScreenWidth;
        public string Status;
        public ushort StatusInfo;
        public string SystemCreationClassName;
        public string SystemName;
        public Win32_DesktopMonitor(ManagementObject o)
        {
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["Bandwidth"] != null) Bandwidth = (uint)o["Bandwidth"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["DisplayType"] != null) DisplayType = (ushort)o["DisplayType"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["IsLocked"] != null) IsLocked = (bool)o["IsLocked"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["MonitorManufacturer"] != null) MonitorManufacturer = (string)o["MonitorManufacturer"];
            if (o["MonitorType"] != null) MonitorType = (string)o["MonitorType"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["PixelsPerXLogicalInch"] != null) PixelsPerXLogicalInch = (uint)o["PixelsPerXLogicalInch"];
            if (o["PixelsPerYLogicalInch"] != null) PixelsPerYLogicalInch = (uint)o["PixelsPerYLogicalInch"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["ScreenHeight"] != null) ScreenHeight = (uint)o["ScreenHeight"];
            if (o["ScreenWidth"] != null) ScreenWidth = (uint)o["ScreenWidth"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class Win32_PointingDevice
    {
        public ushort Availability;
        public string Caption;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public string Description;
        public string DeviceID;
        public ushort DeviceInterface;
        public uint DoubleSpeedThreshold;
        public bool ErrorCleared;
        public string ErrorDescription;
        public ushort Handedness;
        public string HardwareType;
        public string InfFileName;
        public string InfSection;
        public string InstallDate;
        public bool IsLocked;
        public uint LastErrorCode;
        public string Manufacturer;
        public string Name;
        public byte NumberOfButtons;
        public string PNPDeviceID;
        public ushort PointingType;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public uint QuadSpeedThreshold;
        public uint Resolution;
        public uint SampleRate;
        public string Status;
        public ushort StatusInfo;
        public uint Synch;
        public string SystemCreationClassName;
        public string SystemName;
        public Win32_PointingDevice(ManagementObject o)
        {
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["DeviceInterface"] != null) DeviceInterface = (ushort)o["DeviceInterface"];
            if (o["DoubleSpeedThreshold"] != null) DoubleSpeedThreshold = (uint)o["DoubleSpeedThreshold"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["Handedness"] != null) Handedness = (ushort)o["Handedness"];
            if (o["HardwareType"] != null) HardwareType = (string)o["HardwareType"];
            if (o["InfFileName"] != null) InfFileName = (string)o["InfFileName"];
            if (o["InfSection"] != null) InfSection = (string)o["InfSection"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["IsLocked"] != null) IsLocked = (bool)o["IsLocked"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Manufacturer"] != null) Manufacturer = (string)o["Manufacturer"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfButtons"] != null) NumberOfButtons = (byte)o["NumberOfButtons"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PointingType"] != null) PointingType = (ushort)o["PointingType"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["QuadSpeedThreshold"] != null) QuadSpeedThreshold = (uint)o["QuadSpeedThreshold"];
            if (o["Resolution"] != null) Resolution = (uint)o["Resolution"];
            if (o["SampleRate"] != null) SampleRate = (uint)o["SampleRate"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["Synch"] != null) Synch = (uint)o["Synch"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
        }
    }
    public class Win32_BaseBoard
    {
        public string Caption;
        public string[] ConfigOptions;
        public string CreationClassName;
        public float Depth;
        public string Description;
        public float Height;
        public bool HostingBoard;
        public bool HotSwappable;
        public string InstallDate;
        public string Manufacturer;
        public string Model;
        public string Name;
        public string OtherIdentifyingInfo;
        public string PartNumber;
        public bool PoweredOn;
        public string Product;
        public bool Removable;
        public bool Replaceable;
        public string RequirementsDescription;
        public bool RequiresDaughterBoard;
        public string SerialNumber;
        public string SKU;
        public string SlotLayout;
        public bool SpecialRequirements;
        public string Status;
        public string Tag;
        public string Version;
        public float Weight;
        public float Width;
        public Win32_BaseBoard(ManagementObject o)
        {
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ConfigOptions"] != null) ConfigOptions = (string[])o["ConfigOptions"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["Depth"] != null) Depth = (float)o["Depth"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["Height"] != null) Height = (float)o["Height"];
            if (o["HostingBoard"] != null) HostingBoard = (bool)o["HostingBoard"];
            if (o["HotSwappable"] != null) HotSwappable = (bool)o["HotSwappable"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["Manufacturer"] != null) Manufacturer = (string)o["Manufacturer"];
            if (o["Model"] != null) Model = (string)o["Model"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["OtherIdentifyingInfo"] != null) OtherIdentifyingInfo = (string)o["OtherIdentifyingInfo"];
            if (o["PartNumber"] != null) PartNumber = (string)o["PartNumber"];
            if (o["PoweredOn"] != null) PoweredOn = (bool)o["PoweredOn"];
            if (o["Product"] != null) Product = (string)o["Product"];
            if (o["Removable"] != null) Removable = (bool)o["Removable"];
            if (o["Replaceable"] != null) Replaceable = (bool)o["Replaceable"];
            if (o["RequirementsDescription"] != null) RequirementsDescription = (string)o["RequirementsDescription"];
            if (o["RequiresDaughterBoard"] != null) RequiresDaughterBoard = (bool)o["RequiresDaughterBoard"];
            if (o["SerialNumber"] != null) SerialNumber = (string)o["SerialNumber"];
            if (o["SKU"] != null) SKU = (string)o["SKU"];
            if (o["SlotLayout"] != null) SlotLayout = (string)o["SlotLayout"];
            if (o["SpecialRequirements"] != null) SpecialRequirements = (bool)o["SpecialRequirements"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["Tag"] != null) Tag = (string)o["Tag"];
            if (o["Version"] != null) Version = (string)o["Version"];
            if (o["Weight"] != null) Weight = (float)o["Weight"];
            if (o["Width"] != null) Width = (float)o["Width"];

        }
    }
    public class Win32_USBHub
    {
        public ushort Availability;
        public string Caption;
        public byte ClassCode;
        public uint ConfigManagerErrorCode;
        public bool ConfigManagerUserConfig;
        public string CreationClassName;
        public byte CurrentAlternateSettings;
        public byte CurrentConfigValue;
        public string Description;
        public string DeviceID;
        public bool ErrorCleared;
        public string ErrorDescription;
        public bool GangSwitched;
        public string InstallDate;
        public uint LastErrorCode;
        public string Name;
        public byte NumberOfConfigs;
        public byte NumberOfPorts;
        public string PNPDeviceID;
        public ushort[] PowerManagementCapabilities;
        public bool PowerManagementSupported;
        public byte ProtocolCode;
        public string Status;
        public ushort StatusInfo;
        public byte SubclassCode;
        public string SystemCreationClassName;
        public string SystemName;
        public ushort USBVersion;
        public Win32_USBHub(ManagementObject o)
        {
            if (o["Availability"] != null) Availability = (ushort)o["Availability"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["ClassCode"] != null) ClassCode = (byte)o["ClassCode"];
            if (o["ConfigManagerErrorCode"] != null) ConfigManagerErrorCode = (uint)o["ConfigManagerErrorCode"];
            if (o["ConfigManagerUserConfig"] != null) ConfigManagerUserConfig = (bool)o["ConfigManagerUserConfig"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["CurrentAlternateSettings"] != null) CurrentAlternateSettings = (byte)o["CurrentAlternateSettings"];
            if (o["CurrentConfigValue"] != null) CurrentConfigValue = (byte)o["CurrentConfigValue"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["DeviceID"] != null) DeviceID = (string)o["DeviceID"];
            if (o["ErrorCleared"] != null) ErrorCleared = (bool)o["ErrorCleared"];
            if (o["ErrorDescription"] != null) ErrorDescription = (string)o["ErrorDescription"];
            if (o["GangSwitched"] != null) GangSwitched = (bool)o["GangSwitched"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["LastErrorCode"] != null) LastErrorCode = (uint)o["LastErrorCode"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["NumberOfConfigs"] != null) NumberOfConfigs = (byte)o["NumberOfConfigs"];
            if (o["NumberOfPorts"] != null) NumberOfPorts = (byte)o["NumberOfPorts"];
            if (o["PNPDeviceID"] != null) PNPDeviceID = (string)o["PNPDeviceID"];
            if (o["PowerManagementCapabilities"] != null) PowerManagementCapabilities = (ushort[])o["PowerManagementCapabilities"];
            if (o["PowerManagementSupported"] != null) PowerManagementSupported = (bool)o["PowerManagementSupported"];
            if (o["ProtocolCode"] != null) ProtocolCode = (byte)o["ProtocolCode"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["StatusInfo"] != null) StatusInfo = (ushort)o["StatusInfo"];
            if (o["SubclassCode"] != null) SubclassCode = (byte)o["SubclassCode"];
            if (o["SystemCreationClassName"] != null) SystemCreationClassName = (string)o["SystemCreationClassName"];
            if (o["SystemName"] != null) SystemName = (string)o["SystemName"];
            if (o["USBVersion"] != null) USBVersion = (ushort)o["USBVersion"];
        }
    }
    public class Win32_CodecFile
    {
        public uint AccessMask;
        public bool Archive;
        public string Caption;
        public bool Compressed;
        public string CompressionMethod;
        public string CreationClassName;
        public string CreationDate;
        public string CSCreationClassName;
        public string CSName;
        public string Description;
        public string Drive;
        public string EightDotThreeFileName;
        public bool Encrypted;
        public string EncryptionMethod;
        public string Extension;
        public string FileName;
        public ulong FileSize;
        public string FileType;
        public string FSCreationClassName;
        public string FSName;
        public string Group;
        public bool Hidden;
        public string InstallDate;
        public ulong InUseCount;
        public string LastAccessed;
        public string LastModified;
        public string Manufacturer;
        public string Name;
        public string Path;
        public bool Readable;
        public string Status;
        public bool System;
        public string Version;
        public bool Writeable;
        public Win32_CodecFile(ManagementObject o)
        {
            if (o["AccessMask"] != null) AccessMask = (uint)o["AccessMask"];
            if (o["Archive"] != null) Archive = (bool)o["Archive"];
            if (o["Caption"] != null) Caption = (string)o["Caption"];
            if (o["Compressed"] != null) Compressed = (bool)o["Compressed"];
            if (o["CompressionMethod"] != null) CompressionMethod = (string)o["CompressionMethod"];
            if (o["CreationClassName"] != null) CreationClassName = (string)o["CreationClassName"];
            if (o["CreationDate"] != null) CreationDate = (string)o["CreationDate"];
            if (o["CSCreationClassName"] != null) CSCreationClassName = (string)o["CSCreationClassName"];
            if (o["CSName"] != null) CSName = (string)o["CSName"];
            if (o["Description"] != null) Description = (string)o["Description"];
            if (o["Drive"] != null) Drive = (string)o["Drive"];
            if (o["EightDotThreeFileName"] != null) EightDotThreeFileName = (string)o["EightDotThreeFileName"];
            if (o["Encrypted"] != null) Encrypted = (bool)o["Encrypted"];
            if (o["EncryptionMethod"] != null) EncryptionMethod = (string)o["EncryptionMethod"];
            if (o["Extension"] != null) Extension = (string)o["Extension"];
            if (o["FileName"] != null) FileName = (string)o["FileName"];
            if (o["FileSize"] != null) FileSize = (ulong)o["FileSize"];
            if (o["FileType"] != null) FileType = (string)o["FileType"];
            if (o["FSCreationClassName"] != null) FSCreationClassName = (string)o["FSCreationClassName"];
            if (o["FSName"] != null) FSName = (string)o["FSName"];
            if (o["Group"] != null) Group = (string)o["Group"];
            if (o["Hidden"] != null) Hidden = (bool)o["Hidden"];
            if (o["InstallDate"] != null) InstallDate = (string)o["InstallDate"];
            if (o["InUseCount"] != null) InUseCount = (ulong)o["InUseCount"];
            if (o["LastAccessed"] != null) LastAccessed = (string)o["LastAccessed"];
            if (o["LastModified"] != null) LastModified = (string)o["LastModified"];
            if (o["Manufacturer"] != null) Manufacturer = (string)o["Manufacturer"];
            if (o["Name"] != null) Name = (string)o["Name"];
            if (o["Path"] != null) Path = (string)o["Path"];
            if (o["Readable"] != null) Readable = (bool)o["Readable"];
            if (o["Status"] != null) Status = (string)o["Status"];
            if (o["System"] != null) System = (bool)o["System"];
            if (o["Version"] != null) Version = (string)o["Version"];
            if (o["Writeable"] != null) Writeable = (bool)o["Writeable"];
        }
    }
}
