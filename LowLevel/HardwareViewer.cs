using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LowLevel
{
    public partial class HardwareViewer : UserControl
    {
        public HardwareViewer()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            DiskAccess dk = new DiskAccess();
            #region processor
            TreeNode processor = new TreeNode(dk.processor.Name);
            treeView1.Nodes.Add(processor);
            processor.Nodes.Add(dk.processor.Caption);
            processor.Nodes.Add("Manufacturer :" + dk.processor.Manufacturer);
            processor.Nodes.Add("Max clock speed : " + ((float)dk.processor.MaxClockSpeed / 1000).ToString() + "Ghz");
            processor.Nodes.Add("Number of Cores : " + dk.processor.NumberOfCores.ToString());
            processor.Nodes.Add("Number of logical processors : " + dk.processor.NumberOfLogicalProcessors.ToString());
            processor.Nodes.Add("L2 cache size : " + dk.processor.L2CacheSize.ToString());
            processor.Nodes.Add("L3 cache size : " + dk.processor.L3CacheSize.ToString());
            processor.Nodes.Add("Address width : " + dk.processor.AddressWidth.ToString());
            processor.Nodes.Add("Processor Id : " + dk.processor.ProcessorId.ToString());
            processor.Nodes.Add("Revision : " + dk.processor.Revision.ToString());
            #endregion
            #region motherboard
            TreeNode mboard = new TreeNode(dk.motherBoard.Caption);
            treeView1.Nodes.Add(mboard);
            mboard.Nodes.Add("Primary bus type : " + dk.motherBoard.PrimaryBusType);
            mboard.Nodes.Add("Secondary bus type : " + dk.motherBoard.SecondaryBusType);
            TreeNode baseboard = new TreeNode(dk.motherBoard.Caption);
            treeView1.Nodes.Add(baseboard);
            baseboard.Nodes.Add(dk.baseBoard.Manufacturer);
            baseboard.Nodes.Add(dk.baseBoard.Product);
            baseboard.Nodes.Add(dk.baseBoard.SerialNumber);
            TreeNode bios = new TreeNode(dk.bios.Caption);
            bios.Nodes.Add("Serial number :" + dk.bios.SerialNumber);
            bios.Nodes.Add("Language :" + dk.bios.CurrentLanguage);
            bios.Nodes.Add("SMBios version :" + dk.bios.SMBIOSBIOSVersion);
            bios.Nodes.Add("Major Version :" + dk.bios.SMBIOSMajorVersion);
            bios.Nodes.Add("Minor Version :" + dk.bios.SMBIOSMinorVersion);
            treeView1.Nodes.Add(bios);
            #endregion
            #region memory
            TreeNode memDevice = new TreeNode("Memory Devices");
            foreach (Win32_MemoryDevice md in dk.memDevice)
            {
                TreeNode tm = new TreeNode(md.DeviceID);
                memDevice.Nodes.Add(tm);
            }
            treeView1.Nodes.Add(memDevice);
            #endregion
            #region Physical disks
            TreeNode pDisks = new TreeNode("Physical Disks");
            treeView1.Nodes.Add(pDisks);
            foreach (Win32_DiskDrive dd in dk.physicalDisks)
            {
                TreeNode hd = new TreeNode(dd.Caption);
                pDisks.Nodes.Add(hd);
                hd.Nodes.Add(dd.DeviceID);
                hd.Nodes.Add(dd.Description);
                hd.Nodes.Add(dd.Manufacturer);
                hd.Nodes.Add(dd.PNPDeviceID);
                if (dd.CapabilityDescriptions != null)
                {
                    TreeNode cap = new TreeNode("Capabilities");
                    hd.Nodes.Add(cap);
                    foreach (string s in dd.CapabilityDescriptions)
                        cap.Nodes.Add(s);
                }
                if (dd.MediaType != null) hd.Nodes.Add(dd.MediaType);
                if (dd.InterfaceType != null) hd.Nodes.Add(dd.InterfaceType);
                if (dd.Model != null) hd.Nodes.Add(dd.Model);
                if (dd.FirmwareRevision != null) hd.Nodes.Add(dd.FirmwareRevision);
                if (dd.serialNumber != null) hd.Nodes.Add(dd.serialNumber);
                hd.Nodes.Add("Total heads " + dd.TotalHeads.ToString());
                hd.Nodes.Add("Total cylinders " + dd.TotalCylinders.ToString());
                hd.Nodes.Add("Total tracks " + dd.TotalTracks.ToString());
                hd.Nodes.Add("Total Sectors " + dd.TotalSectors.ToString());
                hd.Nodes.Add("Tracks per cylinder" + dd.TracksPerCylinder.ToString());
                hd.Nodes.Add("Size " + dd.Size.ToString() + " bytes");
                hd.Nodes.Add("Partitions :" + dd.Partitions.ToString());
            }
            #endregion
            #region Logical disks disks
            TreeNode lDisks = new TreeNode("Logical Disks");
            treeView1.Nodes.Add(lDisks);
            foreach (Win32_LogicalDisk dd in dk.logDisks)
            {
                TreeNode hd = new TreeNode(dd.DeviceID);
                lDisks.Nodes.Add(hd);
                hd.Nodes.Add(dd.ToString());
                hd.Nodes.Add("File system " + dd.FileSystem);
                hd.Nodes.Add("Size " + dd.Size.ToString());
                hd.Nodes.Add("Free space " + dd.FreeSpace.ToString());
                if (dd.VolumeSerialNumber != null) hd.Nodes.Add("Serial number " + dd.VolumeSerialNumber.ToString());
                if (dd.VolumeName != null) hd.Nodes.Add("Volume name " + dd.VolumeName.ToString());
                hd.Nodes.Add("Description " + dd.Description);
                hd.Nodes.Add("Drive type :" + dd.DriveType.ToString());
                hd.Nodes.Add("Media type :" + dd.MediaType.ToString());
            }
            #endregion
            #region Volumes
            TreeNode volumes = new TreeNode("Volumes");
            treeView1.Nodes.Add(volumes);
            foreach(Win32_Volume vol in dk.volumes)
            {
                TreeNode v = new TreeNode(vol.Caption);
                volumes.Nodes.Add(v);
                v.Nodes.Add("Serial number : " + vol.SerialNumber.ToString());
                v.Nodes.Add(vol.FileSystem);
                v.Nodes.Add(vol.Label);
                v.Nodes.Add("Size : " + vol.Capacity.ToString());
                v.Nodes.Add("Free space : " + vol.FreeSpace.ToString());
                v.Nodes.Add("Block size : " + vol.BlockSize);
                v.Nodes.Add("Device id : " + vol.DeviceID);
            }
            TreeNode part = new TreeNode("Partitions");
            treeView1.Nodes.Add(part);
            foreach (Win32_DiskPartition vol in dk.partitions)
            {
                TreeNode v = new TreeNode(vol.Caption);
                part.Nodes.Add(v);
                v.Nodes.Add("Device id : " + vol.DeviceID);
                v.Nodes.Add("Type : " + vol.Type);
                v.Nodes.Add("Number of blocks : " + vol.NumberOfBlocks.ToString());
                v.Nodes.Add("Size : " + vol.Size.ToString());
                v.Nodes.Add("Starting offset : " + vol.StartingOffset.ToString());
            }
            #endregion
            #region cd rom drives
            TreeNode cdRom = new TreeNode("Cd rom drives");
            treeView1.Nodes.Add(cdRom);
            foreach (Win32_CDROMDrive cd in dk.cdRomDrives)
            {
                TreeNode tn = new TreeNode(cd.Caption);
                cdRom.Nodes.Add(tn);
                tn.Nodes.Add(cd.MediaType);
                tn.Nodes.Add(cd.SerialNumber);
                TreeNode cap = new TreeNode("Capabilities");
                tn.Nodes.Add(cap);
                foreach (string s in cd.CapabilityDescriptions)
                    cap.Nodes.Add(s);
            }
            #endregion
            #region Computer system
            TreeNode CompSys = new TreeNode("Computer");
            treeView1.Nodes.Add(CompSys);
            CompSys.Nodes.Add("Name : " + dk.computerSystem.Caption);
            CompSys.Nodes.Add(dk.computerSystem.SystemType);
            CompSys.Nodes.Add(dk.computerSystem.Description);
            CompSys.Nodes.Add(dk.computerSystem.Manufacturer);
            CompSys.Nodes.Add(dk.computerSystem.Model);
            CompSys.Nodes.Add(dk.computerSystem.BootupState);
            TreeNode roles = new TreeNode("Roles");
            CompSys.Nodes.Add(roles);
            foreach (string s in dk.computerSystem.Roles)
            {
                roles.Nodes.Add(s);
            }
            #endregion
            #region OS
            TreeNode os = new TreeNode("Operating System");
            treeView1.Nodes.Add(os);
            os.Nodes.Add(dk.os.OSArchitecture);
            os.Nodes.Add(dk.os.SerialNumber);
            os.Nodes.Add(dk.os.Caption);
            os.Nodes.Add(dk.os.Version);
            os.Nodes.Add(dk.os.CSDVersion);
            os.Nodes.Add(dk.os.Manufacturer);
            os.Nodes.Add("Build type :" + dk.os.BuildType);
            os.Nodes.Add("Build number :" + dk.os.BuildNumber);
            os.Nodes.Add("Boot device :" + dk.os.BootDevice);
            os.Nodes.Add("Windows directory :" + dk.os.WindowsDirectory);
            os.Nodes.Add("System directory :" + dk.os.SystemDirectory);
            os.Nodes.Add("Install date :" + dk.os.InstallDate);
            os.Nodes.Add("Last boot up :" + dk.os.LastBootUpTime);
            os.Nodes.Add("Max number of processes :" + dk.os.MaxNumberOfProcesses);
            os.Nodes.Add("Total visible memory :" + dk.os.TotalVisibleMemorySize);
            os.Nodes.Add("Total virtual memory :" + dk.os.TotalVirtualMemorySize);
            #endregion
            #region Video
            TreeNode vid = new TreeNode("Video");
            treeView1.Nodes.Add(vid);
            foreach (Win32_VideoController vCont in dk.videoController)
            {
                TreeNode cont = new TreeNode(vCont.Caption);
                vid.Nodes.Add(cont);
                cont.Nodes.Add(vCont.DeviceID);
                if (vCont.AdapterDACType != null) cont.Nodes.Add("Adapter DAC Type" + vCont.AdapterDACType);
                cont.Nodes.Add("Adapter RAM" + vCont.AdapterRAM.ToString());
                cont.Nodes.Add(vCont.PNPDeviceID);
                cont.Nodes.Add(vCont.DriverVersion);
                cont.Nodes.Add(vCont.DriverDate);
                cont.Nodes.Add(vCont.InfFilename);
                cont.Nodes.Add(vCont.InfSection);
                cont.Nodes.Add(vCont.InstalledDisplayDrivers);
                if (vCont.CurrentHorizontalResolution != 0) cont.Nodes.Add("Current Horizontal Resolution " + vCont.CurrentHorizontalResolution.ToString());
                if (vCont.CurrentVerticalResolution != 0) cont.Nodes.Add("Current Vertical Resolution " + vCont.CurrentHorizontalResolution.ToString());
                if (vCont.CurrentBitsPerPixel != 0) cont.Nodes.Add("Current Bits Per Pixel " + vCont.CurrentBitsPerPixel.ToString());
                if (vCont.CurrentNumberOfColors != 0) cont.Nodes.Add("Current NumberOfColor " + vCont.CurrentNumberOfColors.ToString());
                if (vCont.CurrentRefreshRate != 0) cont.Nodes.Add("Current Refresh Rate " + vCont.CurrentRefreshRate.ToString());
            }
            TreeNode scr = new TreeNode("Display");
            treeView1.Nodes.Add(scr);
            foreach (Win32_DesktopMonitor dkt in dk.desktops)
            {
                TreeNode mon = new TreeNode(dkt.Caption);
                scr.Nodes.Add(mon);
                mon.Nodes.Add(dkt.DeviceID);
                mon.Nodes.Add("Pixels Per X Logical Inch" + dkt.PixelsPerXLogicalInch.ToString());
                mon.Nodes.Add("Pixels Per Y Logical Inch" + dkt.PixelsPerYLogicalInch.ToString());
            }
            #endregion
            #region Keyboard
            TreeNode kbd = new TreeNode("Keyboard");
            treeView1.Nodes.Add(kbd);
            kbd.Nodes.Add(dk.kbd.Name);
            kbd.Nodes.Add(dk.kbd.PNPDeviceID);
            kbd.Nodes.Add(dk.kbd.Description);
            #endregion
            #region Pointing devices
            TreeNode poin = new TreeNode("Pointing device");
            treeView1.Nodes.Add(poin);
            foreach (Win32_PointingDevice p in dk.pointing)
            {
                TreeNode pN = new TreeNode(p.Description);
                poin.Nodes.Add(pN);
                pN.Nodes.Add(p.Name);
                pN.Nodes.Add(p.PNPDeviceID);
                pN.Nodes.Add(p.Description);
            }
            #endregion
            #region fan
            TreeNode fan = new TreeNode("Fan");
            treeView1.Nodes.Add(fan);
            fan.Nodes.Add(dk.fan.Caption);
            fan.Nodes.Add(dk.fan.DeviceID);
            #endregion
            #region Network
            TreeNode net = new TreeNode("Network");
            treeView1.Nodes.Add(net);
            foreach (Win32_NetworkAdapter netw in dk.network)
            {
                TreeNode tn = new TreeNode(netw.Name);
                net.Nodes.Add(tn);
                tn.Nodes.Add("Description : " + netw.Description);
                if (netw.AdapterType != null) tn.Nodes.Add("Type : " + netw.AdapterType);
                if (netw.AdapterTypeID != 0) tn.Nodes.Add("Type ID : " + netw.AdapterTypeID);
                if (netw.Manufacturer != null) tn.Nodes.Add("Manufacturer : " + netw.Manufacturer);
                if (netw.PNPDeviceID != null) tn.Nodes.Add("PNP device ID : " + netw.PNPDeviceID);
                if (netw.ServiceName != null) tn.Nodes.Add("Service name : " + netw.ServiceName);
                tn.Nodes.Add("Time of last reset : " + netw.TimeOfLastReset);
                if (netw.MACAddress != null)                    tn.Nodes.Add("Mac address : " + netw.MACAddress);
            }
            #endregion
            #region Codecs
            if (dk.codecs.Count > 0)
            {
                TreeNode cod = new TreeNode("Codecs");
                treeView1.Nodes.Add(cod);
                foreach (Win32_CodecFile p in dk.codecs)
                {
                    TreeNode pN = new TreeNode(p.FileName);
                    cod.Nodes.Add(pN);
                    pN.Nodes.Add(p.FileType);
                    pN.Nodes.Add(p.FileSize.ToString());
                    pN.Nodes.Add(p.Version);
                    pN.Nodes.Add(p.InstallDate);
                    pN.Nodes.Add(p.Manufacturer);
                    pN.Nodes.Add(p.Group);
                    if (p.Description != null) pN.Nodes.Add(p.Description);
                }
            }
            #endregion
            #region USB
            TreeNode usb = new TreeNode("Usb hubs");
            treeView1.Nodes.Add(usb);
            foreach (Win32_USBHub p in dk.usb)
            {
                TreeNode pN = new TreeNode(p.Caption);
                usb.Nodes.Add(pN);
                pN.Nodes.Add(p.PNPDeviceID);
            }
            #endregion
        }
    }
}
