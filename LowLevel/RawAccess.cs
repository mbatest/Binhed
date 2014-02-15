using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LowLevel
{
    public enum PARTITION_STYLE
    {
        PARTITION_STYLE_MBR = 0,
        PARTITION_STYLE_GPT = 1,
        PARTITION_STYLE_RAW = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _PARTITION_INFORMATION_EX
    {
        PARTITION_STYLE PartitionStyle;
        Int64 StartingOffset;
        Int64 PartitionLength;
        Int32 PartitionNumber;
        bool RewritePartition;
        Partition_Information obj; //union

    }

    [StructLayout(LayoutKind.Explicit)]
    struct Partition_Information
    {
        [FieldOffset(0)]
        PARTITION_INFORMATION_MBR Mbr;

        [FieldOffset(0)]
        PARTITION_INFORMATION_GPT Gpt;
    }


    //http://msdn.microsoft.com/en-us/library/aa365450(VS.85).aspx
    [StructLayout(LayoutKind.Sequential)]
    public struct PARTITION_INFORMATION_MBR
    {
        byte PartitionType;
        bool BootIndicator;
        bool RecognizedPartition;
        Int32 HiddenSectors;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PARTITION_INFORMATION_GPT
    {
        string PartitionType; //GUID
        string PartitionId;    //GUID
        Int64 Attributes;
        char[] Name;
    }
    public class RawAccess
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            uint lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            int hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(
            IntPtr hDevice, // call CreateFile()
            Int32 dwIoControlCode,
            IntPtr lpInBuffer,
            Int32 nInBufferSize,
            IntPtr lpOutBuffer,
            Int32 nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped
            );
        public RawAccess()
        {
            Open(@"\\.\PhysicalDrive0");
            byte[] inB = new byte[50];
            byte[] outB = new byte[50];
            uint lb;
            if (DeviceIoControl(handle, 0, IntPtr.Zero, 50, IntPtr.Zero, 50, out lb, IntPtr.Zero))
            {
            }

        }
        [DllImport("kernel32", SetLastError = true)]
        static extern  bool CloseHandle(
              IntPtr hObject   // handle to object
              );
        IntPtr handle;
        const uint GENERIC_READ = 0x80000000;
        const uint OPEN_EXISTING = 3;
        public bool Open(string FileName)
        {
            // open the existing file for reading          
            handle = CreateFile(
                  FileName,
                  GENERIC_READ,
                  0,
                  0,
                  OPEN_EXISTING,
                  0,
                  0);

            if (handle != IntPtr.Zero)
                return true;
            else
                return false;
        }
        public bool Close()
        {
            // close file handle
            return CloseHandle(handle);
        }

    }
}
