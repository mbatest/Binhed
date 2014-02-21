using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Management;

namespace LowLevel
{
    public class DiskAccess
    {
        public DiskAccess()
        {
            RawAccess r = new RawAccess();
            Drives();
        }
        public DriveInfo[] allDrives = DriveInfo.GetDrives();
        public Win32_Battery battery;
        public Win32_BIOS bios;
        public Win32_BootConfiguration boot;
        public List<Win32_CDROMDrive> cdRomDrives= new List<Win32_CDROMDrive>();
        public Win32_ComputerSystem computerSystem;
        public List<Win32_DesktopMonitor> desktops = new List<Win32_DesktopMonitor>();
        public List<Win32_DiskDrive> physicalDisks = new List<Win32_DiskDrive>();
        public List<Win32_DiskPartition> partitions = new List<Win32_DiskPartition>();
        public Win32_Fan fan;
        public Win32_Keyboard kbd;
        public List<Win32_LogicalDisk> logDisks = new List<Win32_LogicalDisk>();
        public List<Win32_MemoryArray> memArr = new List<Win32_MemoryArray>();
        public List<Win32_MemoryDevice> memDevice = new List<Win32_MemoryDevice>();
        public Win32_MotherboardDevice motherBoard;
        public Win32_BaseBoard baseBoard;
        public Win32_Processor processor;
        public List<Win32_NetworkAdapter> network = new List<Win32_NetworkAdapter>();
        public Win32_OperatingSystem os;
        public List<Win32_VideoController> videoController = new List<Win32_VideoController>();
        public List<Win32_Volume> volumes = new List<Win32_Volume>();
        public List<Win32_CodecFile> codecs = new List<Win32_CodecFile>();
        public List<Win32_PointingDevice> pointing = new List<Win32_PointingDevice>();
        public List<Win32_USBHub> usb = new List<Win32_USBHub>();
        public void Drives()
        {
            #region DriveInfo
            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("Drive {0}", d.Name);
                Console.WriteLine(" File type: {0}", d.DriveType);
                if (d.IsReady == true)
                {
                    Console.WriteLine(" Volume label: {0}", d.VolumeLabel);
                    Console.WriteLine(" File system: {0}", d.DriveFormat);
                    Console.WriteLine(
                        " Available space to current user:{0, 15} bytes",
                        d.AvailableFreeSpace);

                    Console.WriteLine(
                        " Total available space:          {0, 15} bytes",
                        d.TotalFreeSpace);

                    Console.WriteLine(
                        " Total size of drive:            {0, 15} bytes ",
                        d.TotalSize);
                }
            }
            #endregion
            ManagementObjectSearcher res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_DiskDrive"));
            foreach (ManagementObject o in res.Get())
            {
                physicalDisks.Add(new Win32_DiskDrive(o));
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_CDROMDrive"));
            foreach (ManagementObject o in res.Get())
            {
                cdRomDrives.Add(new Win32_CDROMDrive(o)); ;
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_ComputerSystem"));
            foreach (ManagementObject o in res.Get())
            {
                computerSystem = new Win32_ComputerSystem(o);
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_Battery"));
            foreach (ManagementObject o in res.Get())
            {
                battery = new Win32_Battery(o);
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_BIOS"));
            foreach (ManagementObject o in res.Get())
            {
                bios = new Win32_BIOS(o);
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_BootConfiguration"));
            foreach (ManagementObject o in res.Get())
            {
                boot = new Win32_BootConfiguration(o);
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_Processor"));
            foreach (ManagementObject o in res.Get())
            {
                processor = new Win32_Processor(o);
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_MotherboardDevice"));
            foreach (ManagementObject o in res.Get())
            {
                motherBoard = new Win32_MotherboardDevice(o);
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_BaseBoard"));
            foreach (ManagementObject o in res.Get())
            {
                baseBoard = new Win32_BaseBoard(o);
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_OperatingSystem"));
            foreach (ManagementObject o in res.Get())
            {
                os = new Win32_OperatingSystem(o);
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_DiskPartition"));
          /*  if(res.Get()!=null)
                foreach (ManagementObject o in res.Get())
                {
                    partitions.Add(new Win32_DiskPartition(o));
                }*/
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_LogicalDisk"));
            foreach (ManagementObject o in res.Get())
            {
                logDisks.Add(new Win32_LogicalDisk(o));
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_MemoryArray"));
            foreach (ManagementObject o in res.Get())
            {
                memArr.Add(new Win32_MemoryArray(o));
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_MemoryDevice"));
            foreach (ManagementObject o in res.Get())
            {
                memDevice.Add(new Win32_MemoryDevice(o));
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_Fan"));
            foreach (ManagementObject o in res.Get())
            {
                fan = new Win32_Fan(o);
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_DesktopMonitor"));
            foreach (ManagementObject o in res.Get())
            {
                desktops.Add(new Win32_DesktopMonitor(o));
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_Keyboard"));
            foreach (ManagementObject o in res.Get())
            {
                kbd = new Win32_Keyboard(o);
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_VideoController"));
            foreach (ManagementObject o in res.Get())
            {
                videoController.Add( new Win32_VideoController(o));
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_NetworkAdapter"));
            foreach (ManagementObject o in res.Get())
            {
                network.Add(new Win32_NetworkAdapter(o));
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_Volume"));
            foreach (ManagementObject o in res.Get())
            {
                volumes.Add(new Win32_Volume(o));
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_PointingDevice"));
            foreach (ManagementObject o in res.Get())
            {
                pointing.Add(new Win32_PointingDevice(o));
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_USBHub"));
            foreach (ManagementObject o in res.Get())
            {
                usb.Add(new Win32_USBHub(o));
            }
            res = new ManagementObjectSearcher(new WqlObjectQuery("SELECT * FROM Win32_CodecFile"));
            foreach (ManagementObject o in res.Get())
            {
                codecs.Add(new Win32_CodecFile(o));
            }
        }
    }
 }