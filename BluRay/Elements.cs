using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
// http://hirntier.blogspot.com/2010/02/avchd-timecode-update.html
// http://owl.phy.queensu.ca/~phil/exiftool/TagNames/M2TS.html
// http://etherguidesystems.com/Help/SDOs/MPEG/
namespace BluRay
{
    public class TS_Packet_Header : LOCALIZED_DATA
    {
        #region Private data
        private ELEMENTARY_TYPE m2TS_Copy_Permission_Indicator;//first two bits
        private ELEMENTARY_TYPE m2TSArrivalTime;// First four bytes - 30 bits
        ELEMENTARY_TYPE syncByte;
        ELEMENTARY_TYPE flags;
        ELEMENTARY_TYPE pid;
        ELEMENTARY_TYPE flags2;
        byte data { get { return (byte)flags.Value; } }
        byte data2 { get { return (byte)flags2.Value; } }
        #endregion
        #region Properties
        public ELEMENTARY_TYPE M2TS_Copy_Permission_Indicator { get { return m2TS_Copy_Permission_Indicator; } }
        public ELEMENTARY_TYPE M2TSArrivalTime { get { return m2TSArrivalTime; } }
        public ELEMENTARY_TYPE SyncByte { get { return syncByte; } }
        public ELEMENTARY_TYPE Flags { get { return flags; } }
        public bool Transport_Error_Indicator { get { return (data & 0x80) == 0x80; } }
        public bool Payload_Unit_Start_Indicator { get { return (data & 0x40) == 0x40; } }
        public bool Transport_Priority { get { return (data & 0x20) == 0x20; } }
        public ELEMENTARY_TYPE PidByte { get { return pid; } }
        public int PID { get { return ((data & 0x1F) << 8) + (byte)pid.Value; } }//13 bits
        public ELEMENTARY_TYPE Flags2 { get { return flags2; } }
        public int Transport_Scrambling_Control { get { return ((data2 & 0xD0) >> 6); } }// 2 bits
        public bool Has_Adaptation_field { get { return (data2 & 0x20) == 0x20; } }//1 bit
        public bool ContainsPayload { get { return ((data2 & 0x10) == 0x10); } }
        public int ContinuityCounter { get { return data2 & 0x0F; } }//4bits
        #endregion
        public TS_Packet_Header(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            // 4 premier bytes dans M2TS
            if (Utils.Utils.PacketSize == 0xC0)
            {
                m2TS_Copy_Permission_Indicator = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
                m2TSArrivalTime = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            }
            syncByte = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            while ((byte)syncByte.Value != 0x47)
                syncByte = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            flags = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            pid = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            flags2 = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class TS_AdaptationField : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE adaptation_field_Length;
        ELEMENTARY_TYPE flags;

        private byte data { get { return (byte)flags.Value; } }
        TS_PCR pcr;
        TS_PCR opcr;
        ELEMENTARY_TYPE spliceCountdown;
        #region Properties
        public ELEMENTARY_TYPE Adaptation_field_Length
        {
            get { return adaptation_field_Length; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
        }
        public bool Discontinuity_indicator { get { return (data & 0x80) == 0x80; } }
        public bool Random_Access_indicator { get { return (data & 0x40) == 0x40; ; } }
        public bool Elementary_stream_priority_indicator { get { return (data & 0x20) == 0x40; } }
        public bool PCR_flag { get { return (data & 0x10) == 0x10; } }
        public bool OPCR_flag { get { return (data & 0x08) == 0x08; } }
        public bool Splicing_point_flag { get { return (data & 0x04) == 0x04; } }
        public bool Transport_private_data_flag { get { return (data & 0x02) == 0x02; } }
        public bool Adaptation_field_extension_flag { get { return (data & 0x01) == 0x01; } }
        public TS_PCR Program_clock_reference { get { return pcr; } }
        public TS_PCR Original_Program_clock_reference { get { return opcr; } }
        public ELEMENTARY_TYPE SpliceCountdown { get { return spliceCountdown; } }
        ELEMENTARY_TYPE privateData;
        public ELEMENTARY_TYPE PrivateData
        {
            get { return privateData; }
            set { privateData = value; }
        }
        #endregion
        public TS_AdaptationField(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            adaptation_field_Length = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            flags = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            if (PCR_flag)
            {
                pcr = new TS_PCR(sw);
            }
            if (OPCR_flag)
            {
                opcr = new TS_PCR(sw);

            }
            if (Splicing_point_flag)
            {
                spliceCountdown = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            }
         //   if (Transport_private_data_flag)
            {
              //  privateData = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (byte)adaptation_field_Length.Value);
            }
            if (Adaptation_field_extension_flag)
            {

            }
            sw.Position = adaptation_field_Length.PositionOfStructureInFile + (byte)adaptation_field_Length.Value;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class TS_Packet : LOCALIZED_DATA, BluRay.ITS_Packet
    {
        private String blockType;
        protected String BlockType
        {
            get { return blockType; }
            set { blockType = value; }
        }
        TS_Packet_Header header;
        TS_AdaptationField adaptationField;
        ELEMENTARY_TYPE splice_countdown;
        ELEMENTARY_TYPE pointerField;
        public string BlockNumber { get { return (PositionOfStructureInFile / Utils.Utils.PacketSize).ToString(); } }
        public TS_Packet_Header Header
        {
            get { return header; }
            set { header = value; }
        }
        public TS_AdaptationField AdaptationField
        {
            get { return adaptationField; }
            set { adaptationField = value; }
        }
        public ELEMENTARY_TYPE Splice_countdown
        {
            get { return splice_countdown; }
            set { splice_countdown = value; }
        }
        public ELEMENTARY_TYPE PointerField
        {
            get { return pointerField; }
            set { pointerField = value; }
        }
        public TS_Packet(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            header = new TS_Packet_Header(sw);
            if (header.Has_Adaptation_field)
            {
                adaptationField = new TS_AdaptationField(sw);
            }
            LengthInFile = Utils.Utils.PacketSize;
        }
        public override string ToString()
        {
            long blockNumber = PositionOfStructureInFile / Utils.Utils.PacketSize;
            return BlockType + " " + blockNumber.ToString(); ;
        }
    }
    public class TS_PCR : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE pCR_Counter;
        ELEMENTARY_TYPE program_Clock_Reference_Extension;
        long endTime;
        double st;
        double tt;

        public ELEMENTARY_TYPE PCR_Counter        {            get { return pCR_Counter; }        }
        public ELEMENTARY_TYPE Program_Clock_Reference_Extension        {            get { return program_Clock_Reference_Extension; }        }
        public long EndTime { get { return endTime; } }
        public string St { get { return st.ToString(); } }
        public string Tt        {            get { return tt.ToString(); }        }
        public TS_PCR(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            pCR_Counter = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            program_Clock_Reference_Extension = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            long pcrBase = (int)pCR_Counter.Value;
            short pcrExt = (short)program_Clock_Reference_Extension.Value;
            endTime = 300 * (2 * pcrBase + (pcrExt >> 15)) + (pcrExt & 0x01ff);
            st = (double)300 * (2 * pcrBase + (pcrExt >> 15)) / (double)90000;
            tt = (double)(pcrExt & 0x01ff) / (double)27000;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class TS_Specific_Program_Information : TS_Packet
    {
        ELEMENTARY_TYPE table_ID;

        ELEMENTARY_TYPE section_length_data;
        ELEMENTARY_TYPE table_Id_extension;//2 bytes
        ELEMENTARY_TYPE section_Number;
        ELEMENTARY_TYPE last_Section_Number;
        ELEMENTARY_TYPE version;
        private byte data { get { return (byte)flags.Value; } }
        ELEMENTARY_TYPE flags;
        public string Table_Type { get { return ((Table_Types)(Byte)Table_ID.Value).ToString(); } }
        public ELEMENTARY_TYPE Table_ID { get { return table_ID; } }
        public ELEMENTARY_TYPE Flags { get { return flags; } }
        public ELEMENTARY_TYPE Section_length_data { get { return section_length_data; } }
        public ELEMENTARY_TYPE Transport_Stream_Id { get { return table_Id_extension; } }
        public ELEMENTARY_TYPE Version { get { return version; } }
        public int Version_Number { get { return ((byte)version.Value & 0x3E) >> 1; } }
        public ELEMENTARY_TYPE Section_Number { get { return section_Number; } }
        public ELEMENTARY_TYPE Last_Section_Number { get { return last_Section_Number; } }
        public bool Section_Syntax_Indicator { get { return (data & 0x80) == 0x80; } }
        public int Section_length { get { return (byte)section_length_data.Value + (data & 0x03) * 256; } }
        public bool Current_Or_Next_Indicator { get { return ((byte)version.Value & 0x01) == 0x01; } }
        public TS_Specific_Program_Information(BitStreamReader sw)
            : base(sw)
        {
            if (Header.Payload_Unit_Start_Indicator)
            {
                PointerField = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            }
            table_ID = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            flags = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            section_length_data = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            table_Id_extension = new ELEMENTARY_TYPE(sw, 0, typeof(short));//The PAT uses this for the transport stream identifier and the PMT uses this for the Program number
            version = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            section_Number = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            last_Section_Number = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    /// <summary>
    /// The PAT lists all programs available in  the transport stream. Each program has a unique PID (16 bit value)
    /// </summary>
    public class TS_Program_Association_Table : TS_Specific_Program_Information
    {
        List<TS_Program> programs = new List<TS_Program>();
        ELEMENTARY_TYPE crc;
        public ELEMENTARY_TYPE Crc
        {
            get { return crc; }
            set { crc = value; }
        }
        public List<TS_Program> Programs { get { return programs; } }

        public TS_Program_Association_Table(BitStreamReader sw)
            : base(sw)
        {
            BlockType = "Program Association Table";
            long u = sw.Position - Section_length_data.PositionOfStructureInFile;
            while (sw.Position < Section_length_data.PositionOfStructureInFile+ Section_length  - 4)
            {
                TS_Program p = new TS_Program(sw);
                programs.Add(p);
            }
            crc = new ELEMENTARY_TYPE(sw, 0, typeof(uint));
            sw.Position = PositionOfStructureInFile + Utils.Utils.PacketSize;
            LengthInFile = sw.Position - PositionOfStructureInFile;

        }

    }

    public class TS_Program_Map_Table : TS_Specific_Program_Information
    {
        ELEMENTARY_TYPE pcr_PID;
        ELEMENTARY_TYPE proginfoLength;
        ELEMENTARY_TYPE crc;
        List<TS_ElementaryStream> elementaryStreams = new List<TS_ElementaryStream>();
        List<TSProgramInfoDescriptor> program_descriptors = new List<TSProgramInfoDescriptor>();
        public ELEMENTARY_TYPE Pcr_PID { get { return pcr_PID; } }
        public int PCR_PID { get { byte[] b = (byte[])pcr_PID.Value; return ((b[0] & 0x1F) << 8) + b[1]; } }
        public int? PCR_PID_Value
        {
            get
            {
                if (PCR_PID == 0x1fff) return null;
                else return PCR_PID;
            }
        }
        public ELEMENTARY_TYPE ProginfoLength { get { return proginfoLength; } }
        public int Prog_info_length { get { byte[] b = (byte[])proginfoLength.Value; return ((b[0] & 0x0F) << 8) + b[1]; } }
        public List<TSProgramInfoDescriptor> Program_descriptors { get { return program_descriptors; } }
        public List<TS_ElementaryStream> ElementaryStreams { get { return elementaryStreams; } }
        public ELEMENTARY_TYPE Crc { get { return crc; } }
        public TS_Program_Map_Table(BitStreamReader sw)
            : base(sw)
        {
            BlockType = "Program Map Table";
            pcr_PID = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            proginfoLength = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            long u = proginfoLength.PositionOfStructureInFile;
            #region Program Info Descriptor List:
            while (sw.Position < u + Prog_info_length)
            {
                TSProgramInfoDescriptor pdes = new TSProgramInfoDescriptor(sw);
                program_descriptors.Add(pdes);
            }
            #endregion
            #region Elementary Stream Info: http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/M2TS.html
            u = Section_length_data.PositionOfStructureInFile; ;
            while (sw.Position < u + Section_length - 4  /*CRC*/ )
            {
                TS_ElementaryStream el = new TS_ElementaryStream(sw);
                elementaryStreams.Add(el);
            }
            #endregion
            sw.Position = PositionOfStructureInFile + Utils.Utils.PacketSize;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class TS_Network_Information_Table : TS_Specific_Program_Information
    {
        public TS_Network_Information_Table(BitStreamReader sw)
            : base(sw)
        {
            BlockType = "Network Information Table";
            sw.Position = PositionOfStructureInFile + Utils.Utils.PacketSize;
        }
    }
    public class TSProgramInfoDescriptor : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE tag;
        ELEMENTARY_TYPE length;
        ELEMENTARY_TYPE data;
        public ELEMENTARY_TYPE Tag { get { return tag; } }
        public ELEMENTARY_TYPE Length { get { return length; } }
        public ELEMENTARY_TYPE Data { get { return data; } set { data = value; } }
        public string TagValue
        {
            get
            {
                switch ((byte)tag.Value)
                {
                    case 0x02: return "Video stream header parameters for ITU-T Rec. H.262, ISO/IEC 13818-2 and ISO/IEC 11172-2";
                    case 0x03: return "Audio stream header parameters for ISO/IEC 13818-3 and ISO/IEC 11172-3";
                    case 0x04: return "Hierarchy for stream selection";
                    case 0x05: return "Registration of private formats";
                    case 0x06: return "Data stream alignment for packetized video and audio sync point";
                    case 0x07: return "Target background grid defines total display area size";
                    case 0x08: return "Video Window defines position in display area";
                    case 0x09: return "Conditional access system and EMM/ECM PID";
                    case 0x0A: return "ISO 639 language and audio type";
                    case 0x0B: return "System clock external reference";
                    case 0x0C: return "Multiplex buffer utilization bounds";
                    case 0x0D: return "Copyright identification system and reference";
                    case 0x0E: return "Maximum bit rate";
                    case 0x0F: return "Private data indicator";
                    case 0x10: return "Smoothing buffer";
                    case 0x11: return "STD video buffer leak control";
                    case 0x12: return "IBP video I-frame indicator";
                    case 0x13: return "ISO/IEC13818-6 DSM CC carousel identifier";
                    case 0x14: return "ISO/IEC13818-6 DSM CC association tag";
                    case 0x15: return "ISO/IEC13818-6 DSM CC deferred association tag";
                    case 0x16: return "ISO/IEC13818-6 DSM CC Reserved.";
                    case 0x17: return "DSM CC NPT reference";
                    case 0x18: return "DSM CC NPT endpoint";
                    case 0x19: return "DSM CC stream mode";
                    case 0x1A: return "DSM CC stream event";
                    case 0x1B: return "Video stream header parameters for ISO/IEC 14496-2 (MPEG-4 H.263 based)";
                    case 0x1C: return "Audio stream header parameters for ISO/IEC 14496-3 (MPEG-4 LOAS multi-format framed)";
                    case 0x1D: return "IOD parameters for ISO/IEC 14496-1";
                    case 0x1E: return "SL parameters for ISO/IEC 14496-1";
                    case 0x1F: return "FMC parameters for ISO/IEC 14496-1";
                    case 0x20: return "External ES identifier for ISO/IEC 14496-1";
                    case 0x21: return "MuxCode for ISO/IEC 14496-1";
                    case 0x22: return "FMX Buffer Size for ISO/IEC 14496-1";
                    case 0x23: return "Multiplex Buffer for ISO/IEC 14496-1";
                    case 0x24: return "Content labeling for ISO/IEC 14496-1";
                    case 0x25: return "Metadata pointer";
                    case 0x26: return "Metadata";
                    case 0x27: return "Metadata STD";
                    case 0x28: return "Video stream header parameters for ITU-T Rec. H.264 and ISO/IEC 14496-10";
                    case 0x29: return "ISO/IEC 13818-11 IPMP (DRM)";
                    case 0x2A: return "Timing and HRD for ITU-T Rec. H.264 and ISO/IEC 14496-10";
                    case 0x2B: return "Audio stream header parameters for ISO/IEC 13818-7 ADTS AAC";
                    case 0x2C: return "FlexMux Timing for ISO/IEC 14496-1";
                    case 0x2D: return "Text stream header parameters for ISO/IEC 14496";
                    case 0x2E: return "Audio extension stream header parameters for ISO/IEC 14496-3 (MPEG-4 LOAS multi-format framed)";
                    case 0x2F: return "Video auxiliary stream header parameters";
                    case 0x30: return "Video scalable stream header parameters";
                    case 0x31: return "Video multi stream header parameters";
                    case 0x35: return "Program stereoscopic (3D) information";
                    case 0x36: return "Video stereoscopic (3D) information";
                    default: return "";
                }
            }
        }
        public TSProgramInfoDescriptor(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            tag = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            length = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            switch ((byte)tag.Value)
            {
                case 0x88: //HDMV Copy Control Descriptor
                    data = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (byte)length.Value);
                    break;
                case 0x25: //Metadata pointer
                case 5://Registration Descriptor
                default:
                    data = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (byte)length.Value);
                    break;
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;

        }
    }
    public class TS_ElementaryStream : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE stream_Type;
        ELEMENTARY_TYPE pid_Data;
        ELEMENTARY_TYPE prog_info_length;
        private List<TS_Descriptor> descriptors;
        List<TS_DataStream> packets = new List<TS_DataStream>();

        public List<TS_DataStream> Packets
        {
            get { return packets; }
            set { packets = value; }
        }
        public ELEMENTARY_TYPE Stream_Type { get { return stream_Type; } }
        public ELEMENTARY_TYPE PID_DATA { get { return pid_Data; } }
        public ELEMENTARY_TYPE Prog_info_length { get { return prog_info_length; } }
        public int PID { get { byte[] bb = (byte[])pid_Data.Value; return ((bb[0] & 0x1F) << 8) + bb[1]; } }
        public List<TS_Descriptor> Descriptors { get { return descriptors; } }
        public string Stream
        {
            get
            {
                switch ((byte)stream_Type.Value)
                {
                    case 0x0: return "Reserved";
                    case 0x1: return "MPEG-1 Video";
                    case 0x2: return "MPEG-2 Video";
                    case 0x3: return "MPEG-1 Audio";
                    case 0x4: return "MPEG-2 Audio";
                    case 0x5: return "ISO 13818-1 private sections";
                    case 0x6: return "ISO 13818-1 PES private data";
                    case 0x7: return "ISO 13522 MHEG";
                    case 0x8: return "ISO 13818-1 DSM-CC";
                    case 0x9: return "ISO 13818-1 auxiliary";
                    case 0xa: return "ISO 13818-6 multi-protocol encap";
                    case 0xb: return "ISO 13818-6 DSM-CC U-N msgs";
                    case 0xc: return "ISO 13818-6 stream descriptors";
                    case 0xd: return "ISO 13818-6 sections";
                    case 0xe: return "ISO 13818-1 auxiliary";
                    case 0xf: return "MPEG-2 AAC Audio";
                    case 0x10: return "MPEG-4 Video";
                    case 0x11: return "MPEG-4 LATM AAC Audio";
                    case 0x12: return "MPEG-4 generic";
                    case 0x13: return "ISO 14496-1 SL-packetized";
                    case 0x14: return "ISO 13818-6 Synchronized Download Protocol";
                    case 0x1b: return "H.264 Video";
                    case 0x80: return "DigiCipher II Video";
                    case 0x81: return "A52/AC-3 Audio";
                    case 0x82: return "HDMV DTS Audio";
                    case 0x83: return "LPCM Audio";
                    case 0x84: return "SDDS Audio";
                    case 0x85: return "ATSC Program ID";
                    case 0x86: return "DTS-HD Audio";
                    case 0x87: return "E-AC-3 Audio";
                    case 0x8a: return "DTS Audio";
                    case 0x91: return "A52b/AC-3 Audio";
                    case 0x92: return "DVD_SPU vls Subtitle";
                    case 0x94: return "SDDS Audio";
                    case 0xa0: return "MSCODEC Video";
                    case 0xea: return "Private ES (VC-1)";
                    default: return "";
                }
            }
        }
        public String Stream_Category
        {
            get
            {
                switch ((byte)stream_Type.Value)
                {
                    case 0x1: return "Video";
                    case 0x2: return "Video";
                    case 0x3: return "Audio";
                    case 0x4: return "Audio";
                    case 0x5: return "private sections";
                    case 0x6: return "PE private data";
                    case 0x7: return "MHEG";
                    case 0x8: return "DSM-CC";
                    case 0x9: return "auxiliary";
                    case 0xa: return "multi-protocol encap";
                    case 0xc: return "stream descriptors";
                    case 0xd: return "sections";
                    case 0xe: return "auxiliary";
                    case 0xf: return "Audio";
                    case 0x10: return "Video";
                    case 0x11: return "Audio";
                    case 0x12: return "generic";
                    case 0x1b: return "Video";
                    case 0x80: return "Video";
                    case 0x81: return "Audio";
                    case 0x82: return "Audio";
                    case 0x83: return "Audio";
                    case 0x84: return "Audio";
                    case 0x86: return "Audio";
                    case 0x87: return "Audio";
                    case 0x8a: return "Audio";
                    case 0x91: return "Audio";
                    case 0x92: return "Subtitle";
                    case 0x94: return "Audio";
                    case 0xa0: return "Video";
                    default: return "";
                }
            }
        }
        private int Prog_length { get { byte[] b = (byte[])prog_info_length.Value; return ((b[0]) & (byte)0x03) * 256 + b[1]; } }
        public TS_ElementaryStream(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            stream_Type = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            pid_Data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            prog_info_length = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            if (Prog_length > 0)
            {
                long v = prog_info_length.PositionOfStructureInFile;
                descriptors = new List<TS_Descriptor>();
                while (sw.StreamPosition < v + Prog_length)
                {
                    TS_Descriptor ds = new TS_Descriptor(sw);
                    descriptors.Add(ds);
                }
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return Stream;
        }
    }
    public class TS_Descriptor : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE tag;
        ELEMENTARY_TYPE length;
        ELEMENTARY_TYPE data;
        ELEMENTARY_TYPE format;
        public ELEMENTARY_TYPE Tag { get { return tag; } }
        public ELEMENTARY_TYPE Length { get { return length; } }
        public ELEMENTARY_TYPE Format { get { return format; } }        
        public ELEMENTARY_TYPE Data { get { return data; } }
        public string Tag_Description
        {
            //http://www.tsreader.com/tsreader/descriptors.html
            get
            {
                switch ((byte)tag.Value)
                {
                    case 0x02: return "Video Stream";
                    case 0x03: return "Audio Stream";
                    case 0x04: return "Hierarchy";
                    case 0x05: return "Registration";
                    case 0x06: return "Data Stream Alignment";
                    case 0x07: return "Target Background Grid";
                    case 0x08: return "Video Window";
                    case 0x09: return "Conditional Access";
                    case 0x0a: return "ISO 639 Language";
                    case 0x0b: return "System Clock";
                    case 0x0c: return "Multiplex Buffer Utilization";
                    case 0x0d: return "Copyright Descriptor";
                    case 0x0e: return "Maximum Bitrate";
                    case 0x0f: return "Private Data Indicator";
                    case 0x10: return "Smoothing Buffer";
                    case 0x11: return "STD";
                    case 0x12: return "IBP";
                    case 0x40: return "Network Name";
                    case 0x41: return "Service List";
                    case 0x42: return "Stuffing";
                    case 0x43: return "Satellite Delivery";
                    case 0x44: return "Cable Delivery";
                    case 0x45: return "VBI Data";
                    case 0x46: return "VBI Teletext";
                    case 0x47: return "Bouquet Name";
                    case 0x48: return "Service";
                    case 0x49: return "Country Availability";
                    case 0x4a: return "Linkage";
                    case 0x4b: return "NVOD Reference";
                    case 0x4c: return "Time Shifted Service";
                    case 0x4d: return "Short Event";
                    case 0x4e: return "Extended Event";
                    case 0x4f: return "Time Shifted Event";
                    case 0x50: return "Component";
                    case 0x51: return "Mosaic";
                    case 0x52: return "Stream Indentifier";
                    case 0x53: return "Conditional Access";
                    case 0x54: return "Content";
                    case 0x55: return "Parental Rating";
                    case 0x56: return "Teletext";
                    case 0x57: return "Telephone";
                    case 0x58: return "Local Time Offset";
                    case 0x59: return "Subtitling";
                    case 0x5a: return "Terrestrial Delivery";
                    case 0x5b: return "Multi Lingual Network Name";
                    case 0x5c: return "Multi Lingual Bouquet Name";
                    case 0x5d: return "Multi Lingual Service Name";
                    case 0x5e: return "Multi Lingual Component Name";
                    case 0x5f: return "Private Data Specifier";
                    case 0x60: return "Service Move";
                    case 0x61: return "Short Smoothing Buffer";
                    case 0x62: return "Frequency List";
                    case 0x63: return "Partial Transport Stream";
                    case 0x64: return "Data Broadcast";
                    case 0x65: return "CA Systen";
                    case 0x66: return "Data Broadcast ID";
                    case 0x67: return "Transport Stream";
                    case 0x68: return "DSNG";
                    case 0x69: return "PDC";
                    case 0x6a: return "AC-3 Audio";
                    case 0x6b: return "Ancilliary Data";
                    case 0x6c: return "Cell List";
                    case 0x6d: return "Cell Frequency Link";
                    case 0x6e: return "Announcement Support";
                    case 0x73: return "DTS Audio";
                    case 0x83: return "Logical Channel Number";
                    case 0x80: return "Stuffing";
                    case 0x81: return "AC-3 Audio";
                    case 0x82: return "Frame Rate";
                    case 0x84: return "Component Name";
                    case 0x90: return "Frequency Spec";
                    case 0x91: return "Modulation Parameters";
                    case 0x92: return "Transport Stream ID";
                    case 0xc0: return "Banner Override";
                    case 0x86: return "Caption Service";
                    case 0x87: return "Content Advisory";
                    case 0xa0: return "Extended Channel Name";
                    case 0xa1: return "Service Location";
                    case 0xa2: return "Time-Shifted Service";
                    case 0xa3: return "Component Name";
                    default: return "Unknown";
                }

            }
        }
        public TS_Descriptor(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            tag = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            length = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            switch ((byte)tag.Value)
            {
                case 0x05:
                    // Format identifier
                    format = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
                    data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (byte)length.Value - 4);
                    break;
                default:
                    data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (byte)length.Value);
                    break;
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
            TS_File.Desc.Add(this);
        }
    }
    public class TS_Program : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE progNb;
        ELEMENTARY_TYPE pid_Data;
        private string program_Name;
        List<TS_Packet> packets = new List<TS_Packet>();
        private int program_Map_PID;
        public ELEMENTARY_TYPE Program_Number { get { return progNb; } }
        public ELEMENTARY_TYPE Pid_Data { get { return pid_Data; } }
        public int ProgramNumber { get { return (short)progNb.Value; } }
        public string Program_Name
        {
            get { return program_Name; }
            set { program_Name = value; }
        }
        public int Program_Map_PID
        {
            get { return program_Map_PID; }
            set { program_Map_PID = value; }
        }
        public List<TS_Packet> Packets { get { return packets; } }
        public TS_Program(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            progNb = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            pid_Data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            LengthInFile = sw.Position - PositionOfStructureInFile;
            byte[] bb = (byte[])pid_Data.Value;
            program_Map_PID = ((bb[0] & 0x1F) << 8) + bb[1];
        }
        public TS_Program(int id, string pType)
        {
            program_Map_PID = id;
            program_Name = pType;
        }
        public override string ToString()
        {
            return program_Name;
        }
    }
    public class TS_DataPacket:TS_Packet
    {
        ELEMENTARY_TYPE packetStart;
        ELEMENTARY_TYPE streamId;
        ELEMENTARY_TYPE packetLength;
        ELEMENTARY_TYPE markers;
        ELEMENTARY_TYPE flags;
        ELEMENTARY_TYPE pes_header_length;
        ELEMENTARY_TYPE pts;
        ELEMENTARY_TYPE dts;
         public ELEMENTARY_TYPE PacketStart
        {
            get { return packetStart; }
            set { packetStart = value; }
        }

        public ELEMENTARY_TYPE StreamId
        {
            get { return streamId; }
            set { streamId = value; }
        }

        public ELEMENTARY_TYPE PacketLength
        {
            get { return packetLength; }
            set { packetLength = value; }
        }
        public ELEMENTARY_TYPE Markers
        {
            get { return markers; }
            set { markers = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public ELEMENTARY_TYPE Pes_header_length
        {
            get { return pes_header_length; }
            set { pes_header_length = value; }
        }
        public ELEMENTARY_TYPE Presentation_Time_Stamp { get { return pts; } }
        public ELEMENTARY_TYPE Decoding_Time_Stamp { get { return dts; } }
        List<TS_Network_Abstraction_Layer> nals = new List<TS_Network_Abstraction_Layer>();
        ELEMENTARY_TYPE extension;
        public ELEMENTARY_TYPE Extension
        {
            get { return extension; }
            set { extension = value; }
        }
        byte[] extensionData { get { return (byte[])extension.Value; } }
        public int PES_scrambling_control { get { return (extensionData[0] & 0x30) >> 4; } }
        public bool PES_priority { get { return (extensionData[0] & 0x08) == 0x08; } }
        public bool data_alignment_indicator { get { return (extensionData[0] & 0x04) == 0x04; } }
        public bool copyright { get { return (extensionData[0] & 0x02) == 0x02; } }
        public bool original_copy { get { return (extensionData[0] & 0x01) == 0x01; } }
        public int PTS_DTS_flags { get { return (extensionData[1] & 0xc0) >> 6; } }
        public bool ESCR_flag { get { return (extensionData[1] & 0x20) == 0x20; } }
        public bool ES_rate_flag { get { return (extensionData[1] & 0x20) == 0x20; } }
        public bool DSM_trick_mode_flag { get { return (extensionData[1] & 0x08) == 0x08; } }
        public bool additional_copy_info_flag { get { return (extensionData[1] & 0x04) == 0x04; } }
        public bool PES_CRC_flag { get { return (extensionData[1] & 0x02) == 0x02; } }
        public bool PES_extension_flag { get { return (extensionData[1] & 0x01) == 0x01; } }
        public int PES_header_data_length { get { return extensionData[2]; } }
   //     public long PTS_Value { get { return (pts[0] & 0x0E << 29) + ((pts[1] & 0x7F) << 21) + ((pts[2] & 0xFE) << 14) + (pts[3] << 7) + ((pts[4] & 0xFE) >> 1); } }
    //    public long DTS_Value { get { return (dts[0] & 0x0E << 29) + ((dts[1] & 0x7F) << 21) + ((dts[2] & 0xFE) << 14) + (dts[3] << 7) + ((dts[4] & 0xFE) >> 1); } }

        public List<TS_Network_Abstraction_Layer> Nals
        {
            get { return nals; }
            set { nals = value; }
        }

        public int pts_dts { get { return (byte)flags.Value & 0xC0; } }
        public string Stream_Type
        {
            get
            {
                if (streamId != null)
                {
                    if ((byte)streamId.Value == 0xBD) return "Private_stream";
                    if ((byte)streamId.Value == 0xBE) return "Padding_stream";
                    if ((byte)streamId.Value == 0xBF) return "Private_stream";
                    if ((byte)streamId.Value >= 0xF0) return "Mpeg_audio_stream";
                    if ((byte)streamId.Value >= 0xE0) return "Mpeg_video_stream";
                    else return "";
                }
                else return "";
            }
        }
        public bool StartPacket { get { byte b = (byte)streamId.Value; return (b==0xC0 ||b==0xE0);} }
        private string date;

        public string Date        {            get { return date; }        }
        public TS_DataPacket(BitStreamReader sw)
            : base(sw)
        {
            if (Header.Payload_Unit_Start_Indicator)
            {
                #region Cas du premier paquet
                packetStart = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 3);

                if (((byte[])packetStart.Value)[2] == 1)
                {
                    streamId = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
                    packetLength = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    markers = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
                    flags = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
                    pes_header_length = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
                    switch (pts_dts)
                    {
                        case 0x00:
                            break;
                        case 0x01:
                            break;
                        case 0x80:
                            pts = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 5);
                            break;
                        case 0xC0:
                            pts = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 5);
                            dts = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 5);
                            break;
                    }
                }
                #endregion
            }
            else
            {

            }
            #region Find Nals
            while (sw.Position < PositionOfStructureInFile + Utils.Utils.PacketSize)
            {
                int bx = sw.ReadInteger();
                while ((bx != 1) & (sw.Position < PositionOfStructureInFile + Utils.Utils.PacketSize))
                {
                    sw.Position -= 3;
                    bx = sw.ReadInteger();
                }
                if (bx==1)
                {
                    sw.Position -= 4;
                    long end;
                    TS_Network_Abstraction_Layer nal = new TS_Network_Abstraction_Layer(sw, PositionOfStructureInFile + Utils.Utils.PacketSize, out end);
                    nals.Add(nal);
                    if (nal.Date != "")
                    {
                        date = nal.Date;
                    }
                    sw.Position = end;
                }
                else break;
            }
            #endregion
            ELEMENTARY_TYPE crc_nalx = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 4);
        }
        public override string ToString()
        {
            string s = BlockType;
            if(Stream_Type!=null)
                s += " " +Stream_Type;
            return s ;
        }
    }
    public class TS_VideoPacket : TS_DataPacket
    {
        public TS_VideoPacket(BitStreamReader sw)
            : base(sw)
        {
            BlockType = "Video Packet";
            sw.Position = PositionOfStructureInFile + Utils.Utils.PacketSize;
        }
    }
    public class TS_AudioPacket : TS_DataPacket
    {
        public TS_AudioPacket(BitStreamReader sw)
            : base(sw)
        {
            BlockType = "Audio Packet";
            sw.Position = PositionOfStructureInFile + Utils.Utils.PacketSize;
        }
    }
    public class TS_Network_Abstraction_Layer : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE nalCode;
        ELEMENTARY_TYPE tag;
        ELEMENTARY_TYPE guid;
        ELEMENTARY_TYPE mpdm;
        ELEMENTARY_TYPE nbTags;
        List<TS_Tag> tags = new List<TS_Tag>();
        int nal_Unit_Type { get { return (byte)nalCode.Value & 0x1F; } }
        string primary_Pic_Type;
        string date = "";
        ELEMENTARY_TYPE dt;
        ELEMENTARY_TYPE text;
        TS_Sequence_Parameter_Set sp;
        TS_Picture_Parameter_Set pp;
        string[] data_strings;

        public ELEMENTARY_TYPE NalCode { get { return nalCode; } }
        public byte TagId { get { return (byte)tag.Value; } }
        public ELEMENTARY_TYPE Tag { get { return tag; } }
        public ELEMENTARY_TYPE Guid { get { return guid; } }
        public ELEMENTARY_TYPE MPDM { get { return mpdm; } }
        public ELEMENTARY_TYPE NbTags { get { return nbTags; } }
        public List<TS_Tag> Tags { get { return tags; } }
        public string NalType
        {
            get
            {
                switch (nal_Unit_Type)
                {
                    case 0: return "Unspecified";
                    case 1: return "Coded slice";
                    case 2: return "Data Partition A";
                    case 3: return "Data Partition B";
                    case 4: return "Data Partition C";
                    case 5: return "IDR (coded slice)";
                    case 6: return "Supplemental enhancement information ";
                    case 7: return "SPS sequence parameter set";
                    case 8: return "PPS picture parameter set";
                    case 9: return "Acces unit delimiter";
                    case 10: return "EoS end of sequence";
                    case 11: return "EoS end of stream";
                    default: return "";
                }
            }
        }
        public string Date { get { return date; } }
        public ELEMENTARY_TYPE Data { get { return dt; } }
        public ELEMENTARY_TYPE Text { get { return text; } }
        public TS_Picture_Parameter_Set Picture_Parameter_Set { get { return pp; } }
        public TS_Sequence_Parameter_Set Sequence_parameter_set { get { return sp; } }
        public string ImageDimension { get { return sp.Image_Width.ToString() + "x" + sp.Image_Height.ToString(); } }
        public string[] Data_strings { get { return data_strings; } }
        public string Primary_Pic_Type { get { return primary_Pic_Type; } }
        public TS_Network_Abstraction_Layer(BitStreamReader sw, long max, out long end)
        {
            //http://www.ietf.org/rfc/rfc6190.txt
            PositionOfStructureInFile = sw.Position;
            ELEMENTARY_TYPE marker = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 4);
            #region Finds the length of the NAL
            int bx = sw.ReadInteger();
            while ((bx != 1) & (sw.Position < max))
            {
                sw.Position -= 3;
                bx = sw.ReadInteger();
            }
            if (bx == 1)
            {
                end = sw.Position - 4;
            }
            else
            {
                end = max;
            }
            #endregion
            sw.Position = PositionOfStructureInFile + 4;
            nalCode = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            int dataLength = (int)(end - sw.Position);
            LengthInFile = 5 + dataLength;
            switch (nal_Unit_Type)
            {
                case 1://non-IDR coded slice                         
                    dt = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), dataLength);
                    break;
                case 2://Coded slice data partition A
                    break;
                case 3://Coded slice data partition B
                    break;
                case 4://Coded slice data partition C
                    dt = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), dataLength);
                    break;
                case 5: // Slice
                    dt = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), dataLength);
                    break;
                case 6:
                    #region Reads SEI
                    tag = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
                    switch ((byte)tag.Value)
                    {
                        case 0:
                            break;
                        case 5:
                            sw.Position++;
                            guid = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 0x10);
                            sw.Position -= 0x10;
                            Guid gu = sw.ReadGuid();
                            Guid uuid_iso_iec_11578 = new System.Guid("05FBC6B9-5A80-40E5-A22A-AB4020267E26");
                            Guid x = new System.Guid("608cee17-4df8-d911-8cd6-0800200c9a66");
                            Guid y = new System.Guid("139FB1A9-446A-4DEC-8CBF-65B1E12D2CFD");
                            Guid guid_ = new Guid(new byte[] { 0x17, 0xee, 0x8c, 0x60, 0xf8, 0x4d, 0x11, 0xd9, 0x8c, 0xd6, 0x08, 0x00, 0x20, 0x0c, 0x9a, 0x66 });
                            if (gu == guid_)
                            {
                                #region Find tags
                                mpdm = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 4);
                                nbTags = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
                                for (int u = 0; u < (byte)nbTags.Value; u++)
                                {
                                    TS_Tag T = new TS_Tag(sw);
                                    tags.Add(T);
                                    if (T.TagId == 0x18)
                                        date = T.Date;
                                    if (T.TagId == 0x19)
                                        date += T.Date;
                                }
                                #endregion
                            }
                            else
                            {
                                sw.Position += 2;
                                dataLength = (int)(end - sw.Position);
                                text = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, dataLength);
                                string txt = (string)text.Value;
                                long position = sw.Position;
                                if (sw.Position == max)
                                {
                                    TS_Packet_Header hd = new TS_Packet_Header(sw);

                                    while (hd.ContinuityCounter > 0)
                                    {
                                        dataLength = Utils.Utils.PacketSize - 4;
                                        ELEMENTARY_TYPE t = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);
                                        txt += (string)t.Value;
                                        hd = new TS_Packet_Header(sw);
                                    }
                                    data_strings = txt.Split(' ');
                                }
                                //  sw.Position = position;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                    #endregion
                case 0x07: //SPS http://www.cardinalpeak.com/blog/?p=878 h264bitstream
                    #region Sequence_Parameter_Set
                    try
                    {
                        sp = new TS_Sequence_Parameter_Set(sw);
                        sp.LengthInFile = dataLength; 
                    }
                    catch { }
                    break;
                    #endregion
                case 0x08: //PPS
                    #region Picture_Parameter_Set
                    try
                    {
                        pp = new TS_Picture_Parameter_Set(sw, dataLength);
                        pp.LengthInFile = dataLength;
                    }
                    catch { }
                    break;
                    #endregion
                case 9: // Acces unit delimiter
                    dt = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
                    int xx = ((byte)dt.Value & 0xE0) >> 4;
                    // à revoir
                    string[] primary_pic_types = new string[] { "I", "I, P", "I, P, B", "SI", "SI, SP", "I, SI", "I, SI, P, SP", "I, SI, P, SP, B" };
                    primary_Pic_Type = primary_pic_types[xx];
                    break;
                case 10:
                case 11:
                    break;
                default:
                    break;
            }
            //      LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class TS_Tag : LOCALIZED_DATA
    {
        /*
AVCHD metadata are stored in an SEI message of mPID 5 (user buffer unregistered). These messages start with a GUID (indicating the mPID of the buffer). The rest is not specified in the H.264 spec. For AVCHD metadata the buffer structure is as follows:
1. The 16 byte GUID, which consists of the bytes
0x17 0xee 0x8c 0x60 0xf8 0x4d 0x11 0xd9 0x8c 0xd6 0x08 0x00 0x20 0x0c 0x9a 0x66
2. 4 bytes
0x4d 0x44 0x50 0x4d which are "MDPM" in ASCII.
3. One byte, which specifies the number of tags to follow
4. Each tag begins with one byte specifying the tag mPID followed by 4 bytes of buffer.
The date and time are stored in tags 0x18 and 0x19.
Tag 0x18 starts with an unknown byte. I saw values between 0x02 and 0xff in various files. 
It seems however that it has a constant value for all frames in a file. The 3 remaining bytes are the year and the month in BCD coding (0x20 0x09 0x08 means August 2009).
The 4 bytes in tag 0x19 are the day, hour, minute and second (also BCD coded). -> temps universel ?
There are more informations stored in this SEI message, check here for a list.
If you want to make further research on this, you can download gmerlin-avdecoder from CVS, open the file lib/parse_h264.cCode 
and uncomment the following line (at the very beginning):
*/
        ELEMENTARY_TYPE tag;
        ELEMENTARY_TYPE tagValue;
        public ELEMENTARY_TYPE Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public ELEMENTARY_TYPE TagValue
        {
            get { return tagValue; }
            set { tagValue = value; }
        }
        public int TagId { get { return (byte)tag.Value; } }
        enum tagNames { DateTimeOriginal = 0x0018, DateTimeOriginal_2 = 0x0019, Camera1 = 0x0070, Camera2 = 0x0071, Shutter = 0x007f, ExposureTime = 0x00a0, FNumber = 0x00a1, ExposureProgram = 0x00a2, BrightnessValue = 0x00a3, ExposureCompensation = 0x00a4, MaxApertureValue = 0x00a5, Flash = 0x00a6, CustomRendered = 0x00a7, WhiteBalance = 0x00a8, FocalLengthIn35mmFormat = 0x00a9, SceneCaptureType = 0x00aa, GPSVersionID = 0x00b0, GPSLatitudeRef = 0x00b1, GPSLatitude = 0x00b2, GPSLongitudeRef = 0x00b5, GPSLongitude = 0x00b6, GPSAltitudeRef = 0x00b9, GPSAltitude = 0x00ba, GPSTimeStamp = 0x00bb, GPSStatus = 0x00be, GPSMeasureMode = 0x00bf, GPSDOP = 0x00c0, GPSSpeedRef = 0x00c1, GPSSpeed = 0x00c2, GPSTrackRef = 0x00c3, GPSTrack = 0x00c4, GPSImgDirectionRef = 0x00c5, GPSImgDirection = 0x00c6, GPSMapDatum = 0x00c7, MakeModel = 0x00e0, RecInfo = 0x00e1, Model = 0x00e4, Model2 = 0x00e5, Model3 = 0x00e6, FrameInfo = 0x00ee };
        private string date = "";
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        private string make = "";
        public string Make
        {
            get { return make; }
            set { make = value; }
        }
        public string unused = "";
        private string model = "";
        public string Model
        {
            get { return model; }
            set { model = value; }
        }
        private int FromBCD(byte b)
        {
            int digit1 = b >> 4;
            int digit2 = b & 0x0f;
            return digit1 * 10 + digit2;
        }
        public TS_Tag(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            tag = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            tagValue = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 4);
            LengthInFile = sw.Position - PositionOfStructureInFile;
            byte[] buf_loc = (byte[])tagValue.Value;
            #region Decode tags
            if ((buf_loc[0] == 0xff) && (buf_loc[1] == 0xff) && (buf_loc[2] == 0xff) && (buf_loc[3] == 0xff))
            {
                unused = "unused";
                return;
            }
            switch ((byte)tag.Value)
            {
                case 0x13:
                    break;
                case 0x18:
                    // It's the "unknown byte" that 0x18 starts with. 5 bits are counting the half hours difference from GMT and bit six is the sign.
                    int year = FromBCD(buf_loc[1]) * 100 + FromBCD(buf_loc[2]);
                    int month = FromBCD(buf_loc[3]);
                    date = year.ToString() + ":" + month.ToString() + ":";
                    break;
                case 0x19:
                    int jour = FromBCD(buf_loc[0]);
                    int heure = FromBCD(buf_loc[1]);
                    int min = FromBCD(buf_loc[2]);
                    int sec = FromBCD(buf_loc[3]);
                    date += jour.ToString() + " " + heure.ToString() + ":" + min.ToString() + ":" + sec.ToString();
                    break;
                case 0xb4:
                    break;
                case 0xe0:
                    if ((buf_loc[0] == 0x01) && (buf_loc[1] == 0x03))
                    {
                        make = "Panasonic";
                    }
                    if ((buf_loc[0] == 0x01) && (buf_loc[1] == 0x08))
                    {
                        make = "Sony";
                    }
                    break;
                case 0xe1:// RecInfo
                    break;
                case 0xe2://??
                    break;
                case 0xe3://??
                    break;
                case 0xe4://Model
                    model = Encoding.Default.GetString(buf_loc);
                    break;
                case 0xe5://Model
                    model = Encoding.Default.GetString(buf_loc);
                    break;
                case 0xe6://Model
                    model = Encoding.Default.GetString(buf_loc);
                    break;
                case 0xe8:
                    break;
                case 0xe9:
                    break;
                case 0xea:
                    break;
                case 0xeb:
                    break;
                case 0x70://Camera 1
                    break;
                case 0x71://Camera 2
                    break;
                case 0x7f://Shutter
                    break;
                default:
                    break;
            }

            #endregion
        }
        public override string ToString()
        {
            byte tag_ = (byte)tag.Value;
            return tag_.ToString("X2") + " " + (tagNames)tag_ + " " + unused + " " + date + " " + make;
        }
    }

    public class TS_Picture_Parameter_Set: LOCALIZED_DATA
    {
        private int pic_parameter_set_id;
        private int seq_parameter_set_id;
        public int Pic_parameter_set_id        {            get { return pic_parameter_set_id; }        }
        public int Seq_parameter_set_id        {            get { return seq_parameter_set_id; }        }
        private bool entropy_coding_mode_flag;

        public bool Entropy_coding_mode_flag        {            get { return entropy_coding_mode_flag; }        }
        private bool pic_order_present_flag;

        public bool Pic_order_present_flag        {            get { return pic_order_present_flag; }        }
        private int num_slice_groups_minus1;

        public int Num_slice_groups_minus1        {            get { return num_slice_groups_minus1; }        }
        private int slice_group_map_type;

        public int Slice_group_map_type        {            get { return slice_group_map_type; }        }
        private int[] run_length_minus1 = new int[8]; // up to num_slice_groups_minus1, which is <= 7 in Baseline and Extended, 0 otheriwse

        public int[] Run_length_minus1        {            get { return run_length_minus1; }        }
        private int[] top_left = new int[8];

        public int[] Top_left        {            get { return top_left; }        }
        private int[] bottom_right = new int[8];

        public int[] Bottom_right        {            get { return bottom_right; }        }
        private bool slice_group_change_direction_flag;

        public bool Slice_group_change_direction_flag        {            get { return slice_group_change_direction_flag; }        }
        private int slice_group_change_rate_minus1;

        public int Slice_group_change_rate_minus1        {            get { return slice_group_change_rate_minus1; }        }
        private int pic_size_in_map_units_minus1;

        public int Pic_size_in_map_units_minus1        {            get { return pic_size_in_map_units_minus1; }        }
        private int[] slice_group_id = new int[256]; // FIXME what size?

        public int[] Slice_group_id        {            get { return slice_group_id; }        }
        private int num_ref_idx_l0_active_minus1;

        public int Num_ref_idx_l0_active_minus1        {            get { return num_ref_idx_l0_active_minus1; }        }
        private int num_ref_idx_l1_active_minus1;

        public int Num_ref_idx_l1_active_minus1        {            get { return num_ref_idx_l1_active_minus1; }        }
        private bool weighted_pred_flag;

        public bool Weighted_pred_flag        {            get { return weighted_pred_flag; }        }
        private int weighted_bipred_idc;

        public int Weighted_bipred_idc        {            get { return weighted_bipred_idc; }        }
        private int pic_init_qp_minus26;

        public int Pic_init_qp_minus26        {            get { return pic_init_qp_minus26; }        }
        private int pic_init_qs_minus26;

        public int Pic_init_qs_minus26        {            get { return pic_init_qs_minus26; }        }
        private int chroma_qp_index_offset;

        public int Chroma_qp_index_offset        {            get { return chroma_qp_index_offset; }        }
        private bool deblocking_filter_control_present_flag;

        public bool Deblocking_filter_control_present_flag        {            get { return deblocking_filter_control_present_flag; }        }
        private bool constrained_intra_pred_flag;

        public bool Constrained_intra_pred_flag        {            get { return constrained_intra_pred_flag; }        }
        private bool redundant_pic_cnt_present_flag;

        public bool Redundant_pic_cnt_present_flag        {            get { return redundant_pic_cnt_present_flag; }        }
        private bool transform_8x8_mode_flag;

        public bool Transform_8x8_mode_flag        {            get { return transform_8x8_mode_flag; }        }
        private bool pic_scaling_matrix_present_flag;

        public bool Pic_scaling_matrix_present_flag        {            get { return pic_scaling_matrix_present_flag; }        }
        private bool[] pic_scaling_list_present_flag = new bool[8];

        public bool[] Pic_scaling_list_present_flag        {            get { return pic_scaling_list_present_flag; }        }
        private int[] scalingList4x4 = new int[4*4];

        public int[] ScalingList4x4        {            get { return scalingList4x4; }        }
        private bool[] useDefaultScalingMatrix4x4Flag = new bool[6];

        public bool[] UseDefaultScalingMatrix4x4Flag        {            get { return useDefaultScalingMatrix4x4Flag; }        }
        private int[] scalingList8x8 = new int[8*8];

        public int[] ScalingList8x8        {            get { return scalingList8x8; }        }
        private bool[] useDefaultScalingMatrix8x8Flag = new bool[2];

        public bool[] UseDefaultScalingMatrix8x8Flag        {            get { return useDefaultScalingMatrix8x8Flag; }        }
        private int second_chroma_qp_index_offset;

        public int Second_chroma_qp_index_offset        {            get { return second_chroma_qp_index_offset; }        }

        public TS_Picture_Parameter_Set(BitStreamReader sw, int datalength)
        {
            PositionOfStructureInFile = sw.Position;
            pic_parameter_set_id = sw.ReadGolomb();
            seq_parameter_set_id = sw.ReadGolomb();
            entropy_coding_mode_flag = sw.ReadBool();
            pic_order_present_flag = sw.ReadBool();
            num_slice_groups_minus1 = sw.ReadGolomb();
            if (num_slice_groups_minus1 > 0)
            {
                #region
                slice_group_map_type = sw.ReadGolomb();
                if (slice_group_map_type == 0)
                {
                    for (int i_group = 0; i_group <= num_slice_groups_minus1; i_group++)
                    {
                        run_length_minus1[i_group] = sw.ReadGolomb();
                    }
                }
                else if (slice_group_map_type == 2)
                {
                    for (int i_group = 0; i_group < num_slice_groups_minus1; i_group++)
                    {
                        top_left[i_group] = sw.ReadGolomb();
                        bottom_right[i_group] = sw.ReadGolomb();
                    }
                }
                else if (slice_group_map_type == 3 ||
                         slice_group_map_type == 4 ||
                         slice_group_map_type == 5)
                {
                    slice_group_change_direction_flag = sw.ReadBool();
                    slice_group_change_rate_minus1 = sw.ReadGolomb();
                }
                else if (slice_group_map_type == 6)
                {
                    pic_size_in_map_units_minus1 = sw.ReadGolomb();
                    for (int i = 0; i <= pic_size_in_map_units_minus1; i++)
                    {
                        //                   slice_group_id[index] = sw.u(b, ceil(log2(num_slice_groups_minus1 + 1))); // was u(v)
                    }
                }
                #endregion
            }
            num_ref_idx_l0_active_minus1 = sw.ReadGolomb();
            num_ref_idx_l1_active_minus1 = sw.ReadGolomb();
            weighted_pred_flag = sw.ReadBool();
            weighted_bipred_idc = sw.ReadIntFromBits(2);
            pic_init_qp_minus26 = sw.se();
            pic_init_qs_minus26 = sw.se();
            chroma_qp_index_offset = sw.se();
            deblocking_filter_control_present_flag = sw.ReadBool();
            constrained_intra_pred_flag = sw.ReadBool();
            redundant_pic_cnt_present_flag = sw.ReadBool();
            long current = sw.Position - PositionOfStructureInFile;
            if (current < datalength)
            {
                transform_8x8_mode_flag = sw.ReadBool();
                pic_scaling_matrix_present_flag = sw.ReadBool();
                if (pic_scaling_matrix_present_flag)
                {
                    for (int i = 0; i < 6 + 2 * (transform_8x8_mode_flag ? 1 : 0); i++)
                    {
                        pic_scaling_list_present_flag[i] = sw.ReadBool();
                        if (pic_scaling_list_present_flag[i])
                        {
                            if (i < 6)
                            {
                                read_scaling_list(sw, ScalingList4x4, i, 16, UseDefaultScalingMatrix4x4Flag[i]);

                            }
                            else
                            {
                                read_scaling_list(sw, ScalingList8x8, i - 6, 64, UseDefaultScalingMatrix8x8Flag[i - 6]);
                            }
                        }
                    }
                }
                second_chroma_qp_index_offset = sw.se();
            }
            current = sw.Position - PositionOfStructureInFile;
            int leftover = 8 - (int) sw.CurrentBit;
            sw.ReadBitRange(leftover);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        void read_scaling_list(BitStreamReader sw, int[] scalingList, int k, int sizeOfScalingList, bool useDefaultScalingMatrixFlag)
        {
            // Probablement incorrect
            int lastScale = 8;
            int nextScale = 8;
            for (int j = 0; j < sizeOfScalingList; j++)
            {
                if (nextScale != 0)
                {
                    int delta_scale = sw.ReadGolomb();
                    nextScale = (lastScale + delta_scale + 256) % 256;
                    useDefaultScalingMatrixFlag = (j == 0) && (nextScale == 0);
                }
                scalingList[j] = (nextScale == 0) ? lastScale : nextScale;
                lastScale = scalingList[j];
            }
        }
    }
    public class TS_Sequence_Parameter_Set : LOCALIZED_DATA
    {
        #region Members
        ELEMENTARY_TYPE profile_idc;
        ELEMENTARY_TYPE level_idc;
        int width;
        int height;
        private int seq_parameter_set_id;
        private int chroma_format_idc;
        private bool residual_colour_transform_flag;
        private int bit_depth_luma_minus8;
        private int bit_depth_chroma_minus8;
        public bool constraint_set0_flag;
        public bool constraint_set1_flag;
        public bool constraint_set2_flag;
        public bool constraint_set3_flag;
        public int reserved_zero_4bits;
        private int log2_max_frame_num_minus4;
        private int pic_order_cnt_type;
        private int log2_max_pic_order_cnt_lsb_minus4;
        private bool delta_pic_order_always_zero_flag;
        private int offset_for_non_ref_pic;
        private int offset_for_top_to_bottom_field;
        private int num_ref_frames_in_pic_order_cnt_cycle;
        private int[] offset_for_ref_frame = new int[256];
        private int num_ref_frames;
        private bool gaps_in_frame_num_value_allowed_flag;
        private int pic_width_in_mbs_minus1;
        private int pic_height_in_map_units_minus1;
        private bool frame_mbs_only_flag;
        private bool mb_adaptive_frame_field_flag;
        private bool direct_8x8_inference_flag;
        private bool frame_cropping_flag;
        private int frame_crop_left_offset;
        private int frame_crop_right_offset;
        private int frame_crop_top_offset;
        private int frame_crop_bottom_offset;
        private bool vui_parameters_present_flag;
        private bool qpprime_y_zero_transform_bypass_flag;
        private bool seq_scaling_matrix_present_flag;
        private bool[] seq_scaling_list_present_flag = new bool[4 * 4];
        private int[] scalingList4x4 = new int[4 * 4];
        private int[] scalingList8x8 = new int[8 * 8];
        private bool[] useDefaultScalingMatrix4x4Flag = new bool[4 * 4];
        private bool[] useDefaultScalingMatrix8x8Flag = new bool[8 * 8];
        #endregion
        #region Properties
        public ELEMENTARY_TYPE Profile_idc { get { return profile_idc; } }
        public ELEMENTARY_TYPE Level_idc { get { return level_idc; } }
        public string Profile_IDC
        {
            get
            {
                {
                    switch ((byte)Profile_idc.Value)
                    {
                        case 44: return "CAVLC 4:4:4 Intra";
                        case 66: return "Baseline";
                        case 77: return "Main";
                        case 83: return "Scalable Baseline";
                        case 86: return "Scalable High";
                        case 88: return "Extended";
                        case 100: return "High";
                        case 110: return "High 10";
                        case 118: return "Multiview High";
                        case 122: return "High 4:2:2";
                        case 128: return "Stereo High";
                        case 144: return "High 4:4:4";
                        case 244: return "High 4:4:4 Predictive";
                        default: return "Unknown";
                    }
                }
            }
        }
        public int Image_Width { get { return width; } }
        public int Image_Height { get { return height; } }
       public int Seq_parameter_set_id { get { return seq_parameter_set_id; } }
        public int Chroma_format_idc        {            get { return chroma_format_idc; }        }
        public bool Residual_colour_transform_flag        {            get { return residual_colour_transform_flag; }        }
        public int Bit_depth_luma_minus8        {            get { return bit_depth_luma_minus8; }        }
        public int Bit_depth_chroma_minus8        {            get { return bit_depth_chroma_minus8; }            set { bit_depth_chroma_minus8 = value; }        }
        public bool Qpprime_y_zero_transform_bypass_flag        {            get { return qpprime_y_zero_transform_bypass_flag; }        }
        public bool Seq_scaling_matrix_present_flag        {            get { return seq_scaling_matrix_present_flag; }        }
        public bool[] Seq_scaling_list_present_flag        {            get { return seq_scaling_list_present_flag; }        }
        public int[] ScalingList4x4        {            get { return scalingList4x4; }        }
        public bool[] UseDefaultScalingMatrix4x4Flag        {            get { return useDefaultScalingMatrix4x4Flag; }        }
        public int[] ScalingList8x8        {            get { return scalingList8x8; }        }
        public bool[] UseDefaultScalingMatrix8x8Flag        {            get { return useDefaultScalingMatrix8x8Flag; }        }
        public int Log2_max_frame_num_minus4        {            get { return log2_max_frame_num_minus4; }        }
        public int Pic_order_cnt_type        {            get { return pic_order_cnt_type; }        }
        public int Log2_max_pic_order_cnt_lsb_minus4        {            get { return log2_max_pic_order_cnt_lsb_minus4; }        }
        public bool Delta_pic_order_always_zero_flag        {            get { return delta_pic_order_always_zero_flag; }        }
        public int Offset_for_non_ref_pic        {            get { return offset_for_non_ref_pic; }        }
        public int Offset_for_top_to_bottom_field        {            get { return offset_for_top_to_bottom_field; }        }
        public int Num_ref_frames_in_pic_order_cnt_cycle        {            get { return num_ref_frames_in_pic_order_cnt_cycle; }        }
        public int[] Offset_for_ref_frame        {            get { return offset_for_ref_frame; }        }
        public int Num_ref_frames        {            get { return num_ref_frames; }        }
        public bool Gaps_in_frame_num_value_allowed_flag        {            get { return gaps_in_frame_num_value_allowed_flag; }        }
        public int Pic_width_in_mbs_minus1        {            get { return pic_width_in_mbs_minus1; }        }
        public int Pic_height_in_map_units_minus1        {            get { return pic_height_in_map_units_minus1; }        }
        public bool Frame_mbs_only_flag        {            get { return frame_mbs_only_flag; }        }
        public bool Mb_adaptive_frame_field_flag        {            get { return mb_adaptive_frame_field_flag; }        }
        public bool Direct_8x8_inference_flag        {            get { return direct_8x8_inference_flag; }        }
        public bool Frame_cropping_flag        {            get { return frame_cropping_flag; }        }
        public int Frame_crop_left_offset        {            get { return frame_crop_left_offset; }        }
        public int Frame_crop_right_offset        {            get { return frame_crop_right_offset; }        }
        public int Frame_crop_top_offset        {            get { return frame_crop_top_offset; }        }
        public int Frame_crop_bottom_offset        {            get { return frame_crop_bottom_offset; }        }
        public bool Vui_parameters_present_flag        {            get { return vui_parameters_present_flag; }        }
        VUI vui;
        HRD hrd;
        public VUI Vui        {            get { return vui; }        }
        public HRD Hrd        {            get { return hrd; }        }
        #endregion
        #region Sub components
        public class VUI : LOCALIZED_DATA
        {
            static float[] Avc_PixelAspectRatio = new float[]
            {
                (float)1, //Reserved
                (float)1,
                (float)12/(float)11,
                (float)10/(float)11,
                (float)16/(float)11,
                (float)40/(float)33,
                (float)24/(float)11,
                (float)20/(float)11,
                (float)32/(float)11,
                (float)80/(float)33,
                (float)18/(float)11,
                (float)15/(float)11,
                (float)64/(float)33,
                (float)160/(float)99,
                (float)4/(float)3,
                (float)3/(float)2,
                (float)2,
            };
            string[] Avc_slice_type = new string[] { "P", "B", "I", "SP", "SI", "P", "B", "I", "SP", "SI" };
            string[] vc_pic_struct = new string[]{    "frame",    "top field",    "bottom field",    "top field, bottom field",    "bottom field, top field",    "top field, bottom field, top field repeated",
    "bottom field, top field, bottom field repeated",    "frame doubling",    "frame tripling"};
            string[] Avc_ct_type = new string[] { "Progressive", "Interlaced", "Unknown", "Reserved" };
            string[] Avc_Colorimetry_format_idc = new string[] { "monochrome", "4:2:0", "4:2:2", "4:4:4" };
            private bool aspect_ratio_info_present_flag;
            private int aspect_ratio_idc;
            private int sar_width;
            private int sar_height;
            private bool overscan_info_present_flag;
            public bool overscan_appropriate_flag;
            private bool video_signal_type_present_flag;
            private int video_format;
            private bool video_full_range_flag;
            private bool colour_description_present_flag;
            private int colour_primaries;
            private int transfer_characteristics;
            private int matrix_coefficients;
            private bool chroma_loc_info_present_flag;
            private int chroma_sample_loc_type_top_field;
            private int chroma_sample_loc_type_bottom_field;
            private bool timing_info_present_flag;
            private int num_units_in_tick;
            private int time_scale;
            private bool fixed_frame_rate_flag;
            private bool nal_hrd_parameters_present_flag;
            private bool vcl_hrd_parameters_present_flag;
            private bool low_delay_hrd_flag;
            private bool pic_struct_present_flag;
            private bool bitstream_restriction_flag;
            private bool motion_vectors_over_pic_boundaries_flag;
            private int max_bytes_per_pic_denom;
            private int max_bits_per_mb_denom;
            private int log2_max_mv_length_horizontal;
            private int log2_max_mv_length_vertical;
            private int num_reorder_frames;
            private int max_dec_frame_buffering;
            #region Properties
            public bool Aspect_ratio_info_present_flag { get { return aspect_ratio_info_present_flag; } }
            public float Pixel_Aspect_ratio {get{ return Avc_PixelAspectRatio[aspect_ratio_idc];}}
            public int Aspect_ratio_idc { get { return aspect_ratio_idc; } }
            public int Sar_width { get { return sar_width; } }
            public int Sar_height { get { return sar_height; } }
            public bool Overscan_info_present_flag { get { return overscan_info_present_flag; } }
            public bool Video_signal_type_present_flag { get { return video_signal_type_present_flag; } }
            public int Video_format_int { get { return video_format; } }
            public string Video_Format
            {
                get
                {
                    switch (video_format)
                    {

                        case 0: return "Component";
                        case 1: return "PAL";
                        case 2: return "NTSC";
                        case 3: return "SECAM";
                        case 4: return "MAC";
                        default: return "";
                    };
                }
            }
            public bool Video_full_range_flag { get { return video_full_range_flag; } }
            public bool Colour_description_present_flag { get { return colour_description_present_flag; } }
            public int Colour_primaries { get { return colour_primaries; } }
            public int Transfer_characteristics { get { return transfer_characteristics; } }
            public int Matrix_coefficients { get { return matrix_coefficients; } }
            public bool Chroma_loc_info_present_flag { get { return chroma_loc_info_present_flag; } }
            public int Chroma_sample_loc_type_top_field { get { return chroma_sample_loc_type_top_field; } }
            public int Chroma_sample_loc_type_bottom_field { get { return chroma_sample_loc_type_bottom_field; } }
            public bool Timing_info_present_flag { get { return timing_info_present_flag; } }
            public int Num_units_in_tick { get { return num_units_in_tick; } }
            public int Time_scale { get { return time_scale; } }
            public bool Fixed_frame_rate_flag { get { return fixed_frame_rate_flag; } }
            public bool Nal_hrd_parameters_present_flag { get { return nal_hrd_parameters_present_flag; } }
            public bool Vcl_hrd_parameters_present_flag { get { return vcl_hrd_parameters_present_flag; } }
            public bool Low_delay_hrd_flag { get { return low_delay_hrd_flag; } }
            public bool Pic_struct_present_flag { get { return pic_struct_present_flag; } }
            public bool Bitstream_restriction_flag { get { return bitstream_restriction_flag; } }
            public bool Motion_vectors_over_pic_boundaries_flag { get { return motion_vectors_over_pic_boundaries_flag; } }
            public int Max_bytes_per_pic_denom { get { return max_bytes_per_pic_denom; } }
            public int Max_bits_per_mb_denom { get { return max_bits_per_mb_denom; } }
            public int Log2_max_mv_length_horizontal { get { return log2_max_mv_length_horizontal; } }
            public int Log2_max_mv_length_vertical { get { return log2_max_mv_length_vertical; } }
            public int Num_reorder_frames { get { return num_reorder_frames; } }
            public int Max_dec_frame_buffering { get { return max_dec_frame_buffering; } }
            #endregion
            public VUI(BitStreamReader sw)
            {
                PositionOfStructureInFile = sw.Position;
                aspect_ratio_info_present_flag = sw.ReadBool();
                if (aspect_ratio_info_present_flag)
                {
                    aspect_ratio_idc = sw.ReadIntFromBits(1);
                    if (aspect_ratio_idc == 255 /*SAR_Extended*/)
                    {
                        sar_width = sw.ReadIntFromBits(2);//bs_read_u(b, 16);
                        sar_height = sw.ReadIntFromBits(2);// bs_read_u(b, 16);
                    }
                }
                overscan_info_present_flag = sw.ReadBool();
                if (overscan_info_present_flag)
                {
                    overscan_appropriate_flag = sw.ReadBool();
                }
                video_signal_type_present_flag = sw.ReadBool();
                if (video_signal_type_present_flag)
                {
                    video_format = sw.RangeToInt(sw.ReadBitRange(3));//bs_read_u(b, 3);
                    video_full_range_flag = sw.ReadBool();
                    colour_description_present_flag = sw.ReadBool();
                    if (colour_description_present_flag)
                    {
                        colour_primaries = sw.ReadIntFromBits(1);
                        transfer_characteristics = sw.ReadIntFromBits(1);
                        matrix_coefficients = sw.ReadIntFromBits(1);
                    }
                }
                chroma_loc_info_present_flag = sw.ReadBool();
                if (chroma_loc_info_present_flag)
                {
                    chroma_sample_loc_type_top_field = sw.ReadGolomb();
                    chroma_sample_loc_type_bottom_field = sw.ReadGolomb();
                }
                timing_info_present_flag = sw.ReadBool();
                if (timing_info_present_flag)
                {
                    num_units_in_tick = sw.ReadIntFromBits(4);// bs_read_u(b, 32);
                    time_scale = sw.ReadIntFromBits(4);//bs_read_u(b, 32);
                    fixed_frame_rate_flag = sw.ReadBool();
                }
                nal_hrd_parameters_present_flag = sw.ReadBool();
                if (nal_hrd_parameters_present_flag)
                {
                    HRD h = new HRD(sw);
                }
                vcl_hrd_parameters_present_flag = sw.ReadBool();
                if (vcl_hrd_parameters_present_flag)
                {
                    HRD h1 = new HRD(sw);
                }
                if (nal_hrd_parameters_present_flag || vcl_hrd_parameters_present_flag)
                {
                    low_delay_hrd_flag = sw.ReadBool();
                }
                pic_struct_present_flag = sw.ReadBool();
                bitstream_restriction_flag = sw.ReadBool();
                if (bitstream_restriction_flag)
                {
                    motion_vectors_over_pic_boundaries_flag = sw.ReadBool();
                    max_bytes_per_pic_denom = sw.ReadGolomb();
                    max_bits_per_mb_denom = sw.ReadGolomb();
                    log2_max_mv_length_horizontal = sw.ReadGolomb();
                    log2_max_mv_length_vertical = sw.ReadGolomb();
                    num_reorder_frames = sw.ReadGolomb();
                    max_dec_frame_buffering = sw.ReadGolomb();
                }
                LengthInFile = sw.Position - PositionOfStructureInFile;
            }
        }
        public class HRD : LOCALIZED_DATA
        {
            public int cpb_cnt_minus1;
            public int bit_rate_scale;
            public int cpb_size_scale;
            public int[] bit_rate_value_minus1; // up to cpb_cnt_minus1, which is <= 31
            public int[] cpb_size_value_minus1;
            public bool[] cbr_flag;
            public int initial_cpb_removal_delay_length_minus1;
            public int cpb_removal_delay_length_minus1;
            public int dpb_output_delay_length_minus1;
            public int time_offset_length;
            public HRD(BitStreamReader sw)
            {
                PositionOfStructureInFile = sw.Position;
                int SchedSelIdx;
                cpb_cnt_minus1 = sw.ReadGolomb();
                //          hrd.bit_rate_scale = bs_read_u( 4);
                //          hrd.cpb_size_scale = bs_read_u( 4);
                for (SchedSelIdx = 0; SchedSelIdx <= cpb_cnt_minus1; SchedSelIdx++)
                {
                    //    hrd.bit_rate_value_minus1[SchedSelIdx] = bitreader.ReadGolomb();
                    //   hrd.cpb_size_value_minus1[SchedSelIdx] = bitreader.ReadGolomb();
                    //   hrd.cbr_flag[SchedSelIdx] = bitreader.ReadBool();
                }
                /*           hrd.initial_cpb_removal_delay_length_minus1 = bs_read_u( 5);
                           hrd.cpb_removal_delay_length_minus1 = bs_read_u( 5);
                           hrd.dpb_output_delay_length_minus1 = bs_read_u( 5);
                           hrd.time_offset_length = bs_read_u( 5);*/
                LengthInFile = sw.Position - PositionOfStructureInFile;
            }
        }
        #endregion
        public TS_Sequence_Parameter_Set(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            profile_idc = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            constraint_set0_flag = sw.ReadBool();
            constraint_set1_flag = sw.ReadBool();
            constraint_set2_flag = sw.ReadBool();
            constraint_set3_flag = sw.ReadBool();
            sw.ReadBitRange(4);  /* all 0's reserved_zero_4bits */
            level_idc = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            seq_parameter_set_id = sw.ReadGolomb(); ;
            if ((byte)Profile_idc.Value == 0x64 || (byte)Profile_idc.Value == 0x6E ||
                (byte)Profile_idc.Value == 0x7A || (byte)Profile_idc.Value == 0x90)
            {
                chroma_format_idc = sw.ReadGolomb();
                if (chroma_format_idc == 3)
                {
                    residual_colour_transform_flag = sw.ReadBool();
                }
                bit_depth_luma_minus8 = sw.ReadGolomb();
                bit_depth_chroma_minus8 = sw.ReadGolomb();
                qpprime_y_zero_transform_bypass_flag = sw.ReadBool();
                seq_scaling_matrix_present_flag = sw.ReadBool();
                if (seq_scaling_matrix_present_flag)
                {
                    #region
                    for (int i = 0; i < 6; i++)
                    {
                        seq_scaling_list_present_flag[i] = sw.ReadBool();
                        if (seq_scaling_list_present_flag[i])
                        {
                            if (i < 6)
                            {
                                read_scaling_list(sw, scalingList4x4, i, 16, useDefaultScalingMatrix4x4Flag[i]);
                            }
                            else
                            {
                                read_scaling_list(sw, scalingList8x8, i - 6, 64, useDefaultScalingMatrix8x8Flag[i - 6]);
                            }
                        }
                    }
                    #endregion
                }
            }
            log2_max_frame_num_minus4 = sw.ReadGolomb();
            pic_order_cnt_type = sw.ReadGolomb();
            if (pic_order_cnt_type == 0)
            {
                log2_max_pic_order_cnt_lsb_minus4 = sw.ReadGolomb();
            }
            else if (pic_order_cnt_type == 1)
            {
                #region
                delta_pic_order_always_zero_flag = sw.ReadBool();
                offset_for_non_ref_pic = sw.se();
                offset_for_top_to_bottom_field = sw.se();
                num_ref_frames_in_pic_order_cnt_cycle = sw.ReadGolomb();
                for (int i = 0; i < num_ref_frames_in_pic_order_cnt_cycle; i++)
                {
                    offset_for_ref_frame[i] = sw.se();
                }
                #endregion
            }
            num_ref_frames = sw.ReadGolomb();
            gaps_in_frame_num_value_allowed_flag = sw.ReadBool();
            pic_width_in_mbs_minus1 = sw.ReadGolomb();
            width = (pic_width_in_mbs_minus1 + 1) * 16;
            pic_height_in_map_units_minus1 = sw.ReadGolomb();
            frame_mbs_only_flag = sw.ReadBool();
            int rect = 0;
            if (frame_mbs_only_flag) rect = 1;
            height = (pic_height_in_map_units_minus1 + 1) * 16 * (2 - rect);
            if (!frame_mbs_only_flag)
            {
                mb_adaptive_frame_field_flag = sw.ReadBool();
            }
            direct_8x8_inference_flag = sw.ReadBool();
            frame_cropping_flag = sw.ReadBool();
            if (frame_cropping_flag)
            {
                frame_crop_left_offset = sw.ReadGolomb();
                frame_crop_right_offset = sw.ReadGolomb();
                frame_crop_top_offset = sw.ReadGolomb();
                frame_crop_bottom_offset = sw.ReadGolomb();
            }
            vui_parameters_present_flag = sw.ReadBool();
            if (vui_parameters_present_flag)
            {
                vui = new VUI(sw);
            }
            int leftover = 8 - (int)sw.CurrentBit;
            sw.ReadBitRange(leftover);
            //          read_rbsp_trailing_bits(h, b);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public static void read_scaling_list(BitStreamReader sw, int[] scalingList, int k, int sizeOfScalingList, bool useDefaultScalingMatrixFlag)
        {
            // Probablement incorrect
            int lastScale = 8;
            int nextScale = 8;
            for (int j = 0; j < sizeOfScalingList; j++)
            {
                if (nextScale != 0)
                {
                    int delta_scale = sw.ReadGolomb();
                    nextScale = (lastScale + delta_scale + 256) % 256;
                    useDefaultScalingMatrixFlag = (j == 0) && (nextScale == 0);
                }
                scalingList[j] = (nextScale == 0) ? lastScale : nextScale;
                lastScale = scalingList[j];
            }
        }
    }
    public class TS_DataStream:LOCALIZED_DATA
    {
        private List<TS_Packet> packets;
        public List<TS_Packet> Packets        {            get { return packets; }        }
        public TS_DataStream()
        {
            packets = new List<TS_Packet>();
        }
        public void AddPacket(TS_Packet p)
        {
            packets.Add(p);
        }
    }
    enum Table_Types { Program_association_section, Conditional_access_section, TS_program_map_section, TS_description_section }
}
