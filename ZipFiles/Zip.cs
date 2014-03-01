using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Utils;

namespace ZipFiles
{
    public class Zip : LOCALIZED_DATA
    {
       List<ZipHeader> zips = new List<ZipHeader>();

       public List<ZipHeader> Zip_Components
       {
           get { return zips; }
           set { zips = value; }
       }
       public Zip(string path)
        {
            List<byte[]> dataBuffers = new List<byte[]>();
            List<string> uncompressedText = new List<string>();
            BitStreamReader sw = new BitStreamReader(path, false);
            byte[] buffer = new byte[30];
            byte[] LengthData = new byte[2];
            byte[] intData = new byte[4];
            zips = new List<ZipHeader>();
            while (sw.Position < sw.Length)
            {
                ZipHeader z = new ZipHeader(sw);
                zips.Add(z);
             }
        }
    }
    public class ZipHeader : LOCALIZED_DATA
    {
        /* A.  Local file header:

               local file header signature     4 bytes  (0x04034b50)
               version needed to extract       2 bytes
               general purpose bit flag        2 bytes
               compression method              2 bytes
               last mod file time              2 bytes
               last mod file date              2 bytes
               crc-32                          4 bytes
               compressed size                 4 bytes
               uncompressed size               4 bytes
               file name length                2 bytes
               extra field length              2 bytes

               file name (variable size)
               extra field (variable size)
       */
        ELEMENTARY_TYPE header_signature, version_made_by, version_needed, bit_flag, compression_method,
            last_modified_time, last_modified_date, crc_32, compressed_size, uncompressed_size,
            file_name_length, extra_field_length, file_name, extra_field, comment_length, data_buffer, comment;

        ELEMENTARY_TYPE disk_number_start, internal_file_attributes, external_files_attributes, relative_offset_local_header;
        ELEMENTARY_TYPE number_of_this_disk, number_of_the_disk_with_the_start_of_the_central_directory, total_number_of_entries_in_the_central_directory_on_this_disk
            , total_number_of_entries_in_the_central_directory, size_of_the_central_directory, offset_of_start_of_central_directory_with_respect_to_the_starting_disk_number,
            zIP_file_comment_length_, zIP_file_comment;
        private string text;
        List<Extra_field> extra_fields;
        #region Properties
        public ELEMENTARY_TYPE Header_signature
        {
            get { return header_signature; }
            set { header_signature = value; }
        }
        public ELEMENTARY_TYPE Version_needed
        {
            get { return version_needed; }
            set { version_needed = value; }
        }
        public ELEMENTARY_TYPE Version_made_by
        {
            get { return version_made_by; }
            set { version_made_by = value; }
        }
        public ELEMENTARY_TYPE Bit_flag
        {
            get { return bit_flag; }
            set { bit_flag = value; }
        }
        public ELEMENTARY_TYPE Compression_method
        {
            get { return compression_method; }
            set { compression_method = value; }
        }
        public ELEMENTARY_TYPE Last_modified_time
        {
            get { return last_modified_time; }
            set { last_modified_time = value; }
        }
        public ELEMENTARY_TYPE Last_modified_date
        {
            get { return last_modified_date; }
            set { last_modified_date = value; }
        }
        public ELEMENTARY_TYPE Crc_32
        {
            get { return crc_32; }
            set { crc_32 = value; }
        }
        public ELEMENTARY_TYPE Compressed_size
        {
            get { return compressed_size; }
            set { compressed_size = value; }
        }
        public ELEMENTARY_TYPE Uncompressed_size
        {
            get { return uncompressed_size; }
            set { uncompressed_size = value; }
        } 
        public ELEMENTARY_TYPE File_name_length
        {
            get { return file_name_length; }
            set { file_name_length = value; }
        }
        public ELEMENTARY_TYPE Extra_field_length
        {
            get { return extra_field_length; }
            set { extra_field_length = value; }
        }
        public ELEMENTARY_TYPE Comment_length
        {
            get { return comment_length; }
            set { comment_length = value; }
        }
        public ELEMENTARY_TYPE Disk_number_start
        {
            get { return disk_number_start; }
            set { disk_number_start = value; }
        }
        public ELEMENTARY_TYPE Internal_file_attributes
        {
            get { return internal_file_attributes; }
            set { internal_file_attributes = value; }
        }
        public ELEMENTARY_TYPE External_files_attributes
        {
            get { return external_files_attributes; }
            set { external_files_attributes = value; }
        }
        public ELEMENTARY_TYPE Relative_offset_local_header
        {
            get { return relative_offset_local_header; }
            set { relative_offset_local_header = value; }
        }
        public ELEMENTARY_TYPE File_name
        {
            get { return file_name; }
            set { file_name = value; }
        }
        public ELEMENTARY_TYPE Extra_field
        {
            get { return extra_field; }
            set { extra_field = value; }
        }
        public List<Extra_field> Extra_fields
        {
            get { return extra_fields; }
            set { extra_fields = value; }
        }
        public ELEMENTARY_TYPE Data_buffer
        {
            get { return data_buffer; }
            set { data_buffer = value; }
        }
        public ELEMENTARY_TYPE Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        public ELEMENTARY_TYPE Number_of_this_disk
        {
            get { return number_of_this_disk; }
            set { number_of_this_disk = value; }
        }
        public ELEMENTARY_TYPE Number_of_the_disk_with_the_start_of_the_central_directory
        {
            get { return number_of_the_disk_with_the_start_of_the_central_directory; }
            set { number_of_the_disk_with_the_start_of_the_central_directory = value; }
        }
        public ELEMENTARY_TYPE Total_number_of_entries_in_the_central_directory_on_this_disk
        {
            get { return total_number_of_entries_in_the_central_directory_on_this_disk; }
            set { total_number_of_entries_in_the_central_directory_on_this_disk = value; }
        }
        public ELEMENTARY_TYPE Total_number_of_entries_in_the_central_directory
        {
            get { return total_number_of_entries_in_the_central_directory; }
            set { total_number_of_entries_in_the_central_directory = value; }
        }
        public ELEMENTARY_TYPE Size_of_the_central_directory
        {
            get { return size_of_the_central_directory; }
            set { size_of_the_central_directory = value; }
        }
        public ELEMENTARY_TYPE Offset_of_start_of_central_directory_with_respect_to_the_starting_disk_number
        {
            get { return offset_of_start_of_central_directory_with_respect_to_the_starting_disk_number; }
            set { offset_of_start_of_central_directory_with_respect_to_the_starting_disk_number = value; }
        }
        public ELEMENTARY_TYPE ZIP_file_comment_length
        {
            get { return zIP_file_comment_length_; }
            set { zIP_file_comment_length_ = value; }
        }
        public ELEMENTARY_TYPE ZIP_file_comment
        {
            get { return zIP_file_comment; }
            set { zIP_file_comment = value; }
        }
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public string Compression_Detail
        {
            get
            {
                int a = 100;
                if (compression_method != null)
                    a = (short)compression_method.Value;
                switch ((short)compression_method.Value)
                {
                    case 0: return "The file is stored (no compression)";
                    case 1: return "The file is Shrunk";
                    case 2: return "The file is Reduced with compression factor 1";
                    case 3: return "The file is Reduced with compression factor 2";
                    case 4: return "The file is Reduced with compression factor 3";
                    case 5: return "The file is Reduced with compression factor 4";
                    case 6: return "The file is Imploded";
                    case 7: return "Reserved for Tokenizing compression algorithm";
                    case 8: return "The file is Deflated";
                    case 9: return "Enhanced Deflating using Deflate64(tm)";
                    case 10: return "PKWARE Data Compression Library Imploding (old IBM TERSE)";
                    case 11: return "Reserved by PKWARE";
                    case 12: return "File is compressed using BZIP2 algorithm";
                    case 13: return "Reserved by PKWARE";
                    case 14: return "LZMA (EFS)";
                    case 15: return "Reserved by PKWARE";
                    case 16: return "Reserved by PKWARE";
                    case 17: return "Reserved by PKWARE";
                    case 18: return "File is compressed using IBM TERSE (new)";
                    case 19: return "IBM LZ77 z Architecture (PFS)";
                    case 97: return "WavPack compressed data";
                    case 98: return "PPMd version I, Rev 1";
                    default: return "";
                }
            }
        }
        public string Made_by
        {
            get
            {
                int a = 20;
                if (version_made_by != null)
                    a = (short)version_made_by.Value;
                switch (a)
                {
                    case 0: return "MS-DOS and OS/2 (FAT / VFAT / FAT32 file systems)";
                    case 1: return "Amiga";
                    case 2: return "OpenVMS";
                    case 3: return "UNIX";
                    case 4: return "VM/CMS";
                    case 5: return "Atari ST";
                    case 6: return "OS/2 H.P.F.S.";
                    case 7: return "Macintosh";
                    case 8: return "Z-System";
                    case 9: return "CP/M";
                    case 10: return "Windows NTFS";
                    case 11: return "MVS (OS/390 Z/OS) ";
                    case 12: return "VSE";
                    case 13: return "Acorn Risc       ";
                    case 14: return "VFAT";
                    case 15: return "alternate MVS      ";
                    case 16: return "BeOS";
                    case 17: return "Tandem";
                    case 18: return "OS/400";
                    case 19: return "OS/X (Darwin) ";
                    default: return "unused";
                }
            }
        }
        #endregion
        public ZipHeader(BitStreamReader sw)
        {
            int ZipCode = 0x04034b50;
            int CentralDirectory = 0x02014b50;
            int centralEnd = 0x06054b50;
            PositionOfStructureInFile = sw.Position;
            header_signature = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            if ((int)header_signature.Value == centralEnd)
            {
                number_of_this_disk = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                number_of_the_disk_with_the_start_of_the_central_directory = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                total_number_of_entries_in_the_central_directory_on_this_disk = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                total_number_of_entries_in_the_central_directory = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                size_of_the_central_directory = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                offset_of_start_of_central_directory_with_respect_to_the_starting_disk_number = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                zIP_file_comment_length_ = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                if((short)zIP_file_comment_length_.Value>0)
                    zIP_file_comment = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)zIP_file_comment_length_.Value);

                return;
            }
            if ((int)header_signature.Value == CentralDirectory)
            {
                version_made_by = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            }
            version_needed = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            bit_flag = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            compression_method = new ELEMENTARY_TYPE(sw, 0, typeof(short));//8
            last_modified_time = new ELEMENTARY_TYPE(sw, 0, typeof(short));//
            last_modified_date = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            crc_32 = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            compressed_size = new ELEMENTARY_TYPE(sw, 0, typeof(int));//18
            uncompressed_size = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            file_name_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            extra_field_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if ((int)header_signature.Value == CentralDirectory)
            {
                comment_length = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                disk_number_start = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                internal_file_attributes = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                external_files_attributes = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                relative_offset_local_header = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            }
            file_name = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)file_name_length.Value);
            if ((short)extra_field_length.Value > 0)
            {
                long start = sw.Position;
                extra_fields = new List<Extra_field>();
                while (sw.Position < start + (short)extra_field_length.Value)
                {
                    extra_fields.Add(new ZipFiles.Extra_field(sw));
                }
            }
            if ((int)header_signature.Value == ZipCode)
            {
                data_buffer  = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]),(int)compressed_size.Value);
                text = Deflate((int)uncompressed_size.Value, (byte[])data_buffer.Value);
            }
            if (comment_length != null)
                comment = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (short)comment_length.Value);

            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        private string Deflate(int uncompSize, byte[] dataBuffer)
        {
            byte[] uncomp = new byte[uncompSize];
            MemoryStream ms = new MemoryStream(dataBuffer);
            DeflateStream df = new DeflateStream(ms, CompressionMode.Decompress);
            df.Read(uncomp, 0, uncompSize);
            return Encoding.Default.GetString(uncomp);
        }
        public override string ToString()
        {
            string s = "";
            switch ((int)header_signature.Value)
            {
                case 0x04034b50:
                    s = "File : ";
                    break;
                case 0x02014b50:
                    s = "Central directory";
                    break;
                case 0x06054b50:
                    s = "End directory";
                    break;
            }
            if (file_name != null)
                s += ((string)file_name.Value).ToString();
            return s;
        }
    }
    public class Extra_field : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE header, data_size, buffer;

        public ELEMENTARY_TYPE Header
        {
            get { return header; }
            set { header = value; }
        }
        public ELEMENTARY_TYPE Data_size
        {
            get { return data_size; }
            set { data_size = value; }
        }
        public ELEMENTARY_TYPE Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }
        public Extra_field(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            header = new ELEMENTARY_TYPE(sw, 0, typeof(ushort));
            data_size = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            buffer = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (short)data_size.Value);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            switch ((ushort)header.Value)
            {
                case 0x0001: return "Zip64 extended information extra field";
                case 0x0007: return "AV Info";
                case 0x0008: return "Reserved for extended language encoding data (PFS)";
                case 0x0009: return "OS/2";
                case 0x000a: return "NTFS ";
                case 0x000c: return "OpenVMS";
                case 0x000d: return "UNIX";
                case 0x000e: return "Reserved for file stream and fork descriptors";
                case 0x000f: return "Patch Descriptor";
                case 0x0014: return "PKCS#7 Store for X.509 Certificates";
                case 0x0015: return "X.509 Certificate ID and Signature for individual file";
                case 0x0016: return "X.509 Certificate ID for Central Directory";
                case 0x0017: return "Strong Encryption Header";
                case 0x0018: return "Record Management Controls";
                case 0x0019: return "PKCS#7 Encryption Recipient Certificate List";


                case 0x0065: return "IBM S/390 (Z390), AS/400 (I400) attributes - uncompressed";
                case 0x0066: return "Reserved for IBM S/390 (Z390), AS/400 (I400) attributes - compressed";
                case 0x4690: return "POSZIP 4690 (reserved) ";
                case 0x07c8: return "Macintosh";
                case 0x2605: return "ZipIt Macintosh";
                case 0x2705: return "ZipIt Macintosh 1.3.5+";
                case 0x2805: return "ZipIt Macintosh 1.3.5+";
                case 0x334d: return "Info-ZIP Macintosh";
                case 0x4341: return "Acorn/SparkFS ";
                case 0x4453: return "Windows NT security descriptor (binary ACL)";
                case 0x4704: return "VM/CMS";
                case 0x470f: return "MVS";
                case 0x4b46: return "FWKCS MD5 (see below)";
                case 0x4c41: return "OS/2 access control list (text ACL)";
                case 0x4d49: return "Info-ZIP OpenVMS";
                case 0x4f4c: return "Xceed original location extra field";
                case 0x5356: return "AOS/VS (ACL)";
                case 0x5455: return "extended timestamp";
                case 0x554e: return "Xceed unicode extra field";
                case 0x5855: return "Info-ZIP UNIX (original, also OS/2, NT, etc)";
                case 0x6375: return "Info-ZIP Unicode Comment Extra Field";
                case 0x6542: return "BeOS/BeBox";
                case 0x7075: return "Info-ZIP Unicode Path Extra Field";
                case 0x756e: return "ASi UNIX";
                case 0x7855: return "Info-ZIP UNIX (new)";
                case 0xa220: return "Microsoft Open Packaging Growth Hint";
                default: return "Unknown";
            }
        }
    }

    public class Gzip : LOCALIZED_DATA
    {
        private string os;
        private string compression;
        public Gzip(string path)
        {
            List<byte[]> dataBuffers = new List<byte[]>();
            List<string> uncompressedText = new List<string>();
            FileStream sw = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] buffer = new byte[30];
            byte[] LengthData = new byte[2];
            byte[] intData = new byte[4];
            while (sw.Position < sw.Length)
            {
                sw.Read(buffer, 0, 2);//header
                sw.Read(buffer, 0, 1);//Compression
                switch (buffer[0])
                {
                    case 0x08:
                        compression = "Deflate";
                        break;
                }
                sw.Read(buffer, 0, 1);//Flags

                sw.Read(buffer, 0, 4);//Datetime
                sw.Read(buffer, 0, 1);//XLEN
                sw.Read(buffer, 0, 1);//Operating system               
                switch (buffer[0])
                {
                    case 0:
                        os = " FAT filesystem (MS-DOS, OS/2, NT/Win32)";
                        break;
                    case 1:
                        os = "Amiga";
                        break;
                    case 2:
                        os = "VMS (or OpenVMS)";
                        break;
                    case 3:
                        os = "Unix";
                        break;
                    case 4:
                        os = "VM/CMS";
                        break;
                    case 5:
                        os = "Atari TOS";
                        break;
                    case 6:
                        os = "HPFS filesystem (OS/2, NT)";
                        break;
                    case 7:
                        os = "Macintosh";
                        break;
                    case 8:
                        os = "Z-System";
                        break;
                    case 9:
                        os = "CP/M";
                        break;
                    case 10:
                        os = "TOPS-20";
                        break;
                    case 11:
                        os = "NTFS filesystem (NT)";
                        break;
                    case 12:
                        os = "QDOS";
                        break;
                    case 13:
                        os = "Acorn RISCOS";
                        break;
                    case 255:
                        os = "unknown";
                        break;

                }
                sw.Read(buffer, 0, 1);//CRC


            }
        }
    }
}