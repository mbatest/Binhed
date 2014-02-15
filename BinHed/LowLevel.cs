using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Windows.Forms;
using System.Text;

namespace BinHed
{
 /*
    class Low
    {
        public static int Maint(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage : ReadFile <FileName>");
                return 1;
            }

            if (!System.IO.File.Exists(args[0]))
            {
                Console.WriteLine("File " + args[0] + " not found.");
                return 1;
            }

            byte[] buffer = new byte[128];
            FileReader fr = new FileReader();

            if (fr.Open(args[0]))
            {

                // Assume that an ASCII file is being read
                ASCIIEncoding Encoding = new ASCIIEncoding();

                int bytesRead;
                do
                {
                    bytesRead = fr.Read(buffer, 0, buffer.Length);
                    string content = Encoding.GetString(buffer, 0, bytesRead);
                    Console.Write("{0}", content);
                }
                while (bytesRead > 0);

                fr.Close();
                return 0;
            }
            else
            {
                Console.WriteLine("Failed to open requested file");
                return 1;
            }
        }
        public Low(string[] args)
        {
            byte[] buf = new byte[128];
            FileReader fr = new FileReader();
            string str = "PHYSICALDRIVE0";
            if (fr.Open(@"\\.\" + str))
            {
                ASCIIEncoding Encoding = new ASCIIEncoding();

                int bytesRead;
                do
                {
                    bytesRead = fr.Read(buf, 0, buf.Length);
                    string content = Encoding.GetString(buf, 0, bytesRead);
                    Console.Write("{0}", content);
                }
                while (bytesRead > 0);
                fr.Close();
            }
            string driveLetter = args[0].Substring(0, 1).ToUpper() + ":";
            driveLetter = "PHYSICALDRIVE0";
            IntPtr hFile = CreateFile("\\\\.\\A", WinntConst.GENERIC_READ | WinntConst.FILE_SHARE_READ, 0, IntPtr.Zero, WinntConst.OPEN_EXISTING, 0, IntPtr.Zero);
            SafeFileHandle _hdev = CreateFileR(driveLetter);
            if (_hdev.IsClosed | _hdev.IsInvalid)
            {
                Console.WriteLine("Error opening device");
                return;
            }
     //       MessageBox.Show("ouvert");
            Console.WriteLine("DeviceIoControl - Version One");
            Console.WriteLine("IOCTL_DVD_START_SESSION");
            bool result = false;
            int bytesReturned = 0;
            //           int sessionId = 0;
            //           result = DeviceIoControl(_hdev, CTL_CODE(0x00000033, 0x0400, 0, 1), IntPtr.Zero, 0, (IntPtr)sessionId, Marshal.SizeOf(sessionId), out bytesReturned, IntPtr.Zero);
            IntPtr buffer = Marshal.AllocHGlobal(sizeof(int));
            result = DeviceIoControl(_hdev, CTL_CODE(0x00000033, 0x0400, 0, 1), IntPtr.Zero, 0, buffer, sizeof(int), out bytesReturned, IntPtr.Zero);
            int sessionId = Marshal.ReadInt32(buffer);
            Marshal.FreeHGlobal(buffer);
            if (result == false)
            {
                int error_code = Marshal.GetLastWin32Error();
                Console.WriteLine("Result: " + result);
                Console.WriteLine("error code: " + error_code);
            }
            else
            {
                Console.WriteLine("Result: " + result); Console.WriteLine("BytesReturned: " + bytesReturned);
                Console.WriteLine("SessionId: " + sessionId); Console.WriteLine("sizeof(SessionId): " + Marshal.SizeOf(sessionId));
            }
            Console.WriteLine("IOCTL_DVD_READ_STRUCTURE");
            Console.WriteLine("Skipping...");
            Console.WriteLine("IOCTL_DVD_END_SESSION");
            bytesReturned = 0;
            result = DeviceIoControl(_hdev, CTL_CODE(0x00000033, 0x0403, 0, 1), new IntPtr(sessionId), Marshal.SizeOf(sessionId), IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);
            if (result == false)
            {
                int error_code = Marshal.GetLastWin32Error();
                Console.WriteLine("error code: " + error_code);
                Console.WriteLine("Result: " + result);
            }
            else { Console.WriteLine("Result: " + result); Console.WriteLine("BytesReturned: " + bytesReturned); }
            Console.WriteLine("\nDeviceIoControl - Version Two");
            Console.WriteLine("IOCTL_DVD_START_SESSION");
            result = false; uint bytesReturned2 = 0; sessionId = -10;
            NativeOverlapped nativeOverlapped = new NativeOverlapped();
            result = DeviceIoControlAlt(_hdev, EIOControlCode.DvdStartSession, 0, 0, sessionId, (uint)Marshal.SizeOf(sessionId), ref bytesReturned2, ref nativeOverlapped);
            if (result == false)
            {
                int error_code = Marshal.GetLastWin32Error();
                Console.WriteLine("Result: " + result);
                Console.WriteLine("error code: " + error_code);
            }
            else
            {
                Console.WriteLine("Result: " + result);
                Console.WriteLine("BytesReturned: " + bytesReturned2);
                Console.WriteLine("SessionId: " + sessionId);
                Console.WriteLine("sizeof(SessionId): " + Marshal.SizeOf(sessionId));
            }
            Console.WriteLine("IOCTL_DVD_READ_STRUCTURE");
            Console.WriteLine("Skipping...");
            Console.WriteLine("IOCTL_DVD_END_SESSION");
            bytesReturned2 = 0;
            result = DeviceIoControlAlt(_hdev, EIOControlCode.DvdEndSession, sessionId, (uint)Marshal.SizeOf(sessionId), 0, 0, ref bytesReturned2, ref nativeOverlapped);
            if (result == false)
            {
                int error_code = Marshal.GetLastWin32Error(); Console.WriteLine("Result: " + result);
                Console.WriteLine("error code: " + error_code);
            }
            else
            {
                Console.WriteLine("Result: " + result);
                Console.WriteLine("BytesReturned: " + bytesReturned2);
            }
            _hdev.Close();
        }
        public static int CTL_CODE(int DeviceType, int Function, int Method, int Access) { return (((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method)); }
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        public static SafeFileHandle CreateFileR(string device)
        {
            string str = device.EndsWith(@"\") ? device.Substring(0, device.Length - 1) : device;
            return new SafeFileHandle(CreateFile(@"\\.\" + str, WinntConst.GENERIC_READ, WinntConst.FILE_SHARE_READ, IntPtr.Zero, WinntConst.OPEN_EXISTING, WinntConst.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero), true);
        }
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeviceIoControl([In] SafeFileHandle hDevice, [In] int dwIoControlCode, [In] IntPtr lpInBuffer, [In] int nInBufferSize, [Out] IntPtr lpOutBuffer, [In] int nOutBufferSize, out int lpBytesReturned, [In] IntPtr lpOverlapped);
        internal class WinntConst
        {            // Fields       
            internal static uint FILE_ATTRIBUTE_NORMAL = 0x80;
            internal static uint FILE_SHARE_READ = 1;
            internal static uint GENERIC_READ = 0x80000000;
            internal static uint OPEN_EXISTING = 3;
        }
        // Other tagCode for DeviceIoControl from pinvoke.net        
        [Flags]
        public enum EIOControlCode : uint
        {            // DVD            
            DvdReadStructure = (EFileDevice.Dvd << 16) | (0x0450 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DvdStartSession = (EFileDevice.Dvd << 16) | (0x0400 << 2) | EMethod.Buffered | (FileAccess.Read << 14),
            DvdEndSession = (EFileDevice.Dvd << 16) | (0x0403 << 2) | EMethod.Buffered | (FileAccess.Read << 14)
        };
        [Flags]
        public enum EFileDevice : uint { Dvd = 0x00000033, }
        [Flags]
        public enum EMethod : uint { Buffered = 0, InDirect = 1, OutDirect = 2, Neither = 3 }
        [DllImport("Kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControlAlt(Microsoft.Win32.SafeHandles.SafeFileHandle hDevice, EIOControlCode IoControlCode, [MarshalAs(UnmanagedType.AsAny)][In] object InBuffer, uint nInBufferSize, [MarshalAs(UnmanagedType.AsAny)][Out] object OutBuffer, uint nOutBufferSize, ref uint pBytesReturned, [In] ref System.Threading.NativeOverlapped Overlapped);
    }
    class LowLevel
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct OVERLAPPED
        {
            public uint Internal;
            public uint InternalHigh;
            public uint Offset;
            public uint OffsetHigh;
            public int hEvent;
        }

        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr CreateFile(
                    string lpFileName,
                    uint dwDesiredAccess,
                    uint dwShareMode,
                    IntPtr lpSecurityAttributes,
                    uint dwCreationDisposition,
                    uint dwFlagsAndAttributes,
                    IntPtr hTemplateFile);

        [DllImport("kernel32", SetLastError = true)]
        public static extern Boolean CloseHandle(int handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean ReadFile(
        IntPtr hFile,
        Byte[] buffer,
        UInt32 BytesToRead,
        ref UInt32 BytedRead,
        ref OVERLAPPED OverLapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            [Out] IntPtr lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            IntPtr lpOverlapped);

        static int EIGHT_K = 8192;
        static int FIVE_TWELVE_BYTES = 512;
        static uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = (0x40000000);
        static uint OPEN_EXISTING = 3;
        static uint FILE_SHARE_READ = 0x00000001;
        static uint FILE_SHARE_WRITE = 0x00000002;

        private IntPtr OpenVolume(string DeviceName)
        {
            IntPtr hDevice;
            hDevice = CreateFile(
                @"\\.\" + DeviceName,
                GENERIC_READ,
                FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                0,
                IntPtr.Zero);
            if ((int)hDevice == -1)
            {
                throw new Exception(Marshal.GetLastWin32Error().ToString());
            }
            return hDevice;
        }
        public LowLevel()
        {

        }
    }
    class FileReader
    {
        const uint GENERIC_READ = 0x80000000;
        const uint OPEN_EXISTING = 3;
        IntPtr handle;

        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe IntPtr CreateFile(
              string FileName,                    // file nameIndex
              uint DesiredAccess,                 // access channel
              uint ShareMode,                     // share channel
              uint SecurityAttributes,            // Security Attributes
              uint CreationDisposition,           // how to createDate
              uint FlagsAndAttributes,            // file attributes
              int hTemplateFile                   // handle to template file
              );

        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool ReadFile(
              IntPtr hFile,                       // handle to file
              void* pBuffer,                      // codeBuffer codeBuffer
              int NumberOfBytesToRead,            // number of bytes to read
              int* pNumberOfBytesRead,            // number of bytes read
              int Overlapped                      // overlapped codeBuffer
              );

        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool CloseHandle(
              IntPtr hObject   // handle to object
              );

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

        public unsafe int Read(byte[] buffer, int index, int count)
        {
            int n = 0;
            fixed (byte* p = buffer)
            {
                if (!ReadFile(handle, p + index, count, &n, 0))
                    return 0;
            }
            return n;
        }

        public bool Close()
        {
            // close file handle
            return CloseHandle(handle);
        }
    }
    */
}