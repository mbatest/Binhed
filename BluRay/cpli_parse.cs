using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace BluRay
{
    public class CLPI : LOCALIZED_DATA
    {
        private string type_indicator;

        private Int32 sequence_info_start_addr;
        private Int32 program_info_start_addr;
        private Int32 cpi_start_addr;
        private Int32 clip_mark_start_addr;
        private Int32 ext_data_start_addr;
        private CLPI_CLIP_INFO clip;
        private CLPI_SEQ_INFO sequence;
        private CLPI_CPI cpi;
        private CLPI_PROG_INFO program;
        string name;
        private string longName;
        #region Properties
        public string Header
        {
            get { return type_indicator; }
            set { type_indicator = value; }
        }
        public CLPI_CLIP_INFO Clip
        {
            get { return clip; }
            set { clip = value; }
        }
        public CLPI_SEQ_INFO Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }
        public CLPI_PROG_INFO Program
        {
            get { return program; }
            set { program = value; }
        }
        public CLPI_CPI Cpi
        {
            get { return cpi; }
            set { cpi = value; }
        }
        public string LongName
        {
            get { return longName; }
            set { longName = value; }
        }
        #endregion
        // SkipBit clip mark & extension data
        public CLPI(string fileName)
        {
            longName = fileName;
            name = Path.GetFileNameWithoutExtension(fileName);
            BitStreamReader bs = new BitStreamReader(fileName, true);
            #region Header
            type_indicator = bs.ReadString(8);
            if (!type_indicator.Contains("HDMV"))
                return;
            sequence_info_start_addr = bs.ReadInteger();
            program_info_start_addr = bs.ReadInteger();
            cpi_start_addr = bs.ReadInteger();
            clip_mark_start_addr = bs.ReadInteger();
            ext_data_start_addr = bs.ReadInteger();
            #endregion
            clip = new CLPI_CLIP_INFO(bs);
            sequence = new CLPI_SEQ_INFO(bs, sequence_info_start_addr);
            program = new CLPI_PROG_INFO(bs, program_info_start_addr);// Parse_program(bs);
            cpi = new CLPI_CPI(bs, cpi_start_addr);
            bs.Close();
        }
        public override string ToString()
        {
            return name;
        }
    }
    #region cpli
    public class BD_UO_MASK : LOCALIZED_DATA
    {
        public bool menu_call;// 1
        public bool title_search;// 1
        public bool chapter_search;// 1
        public bool time_search;// 1
        public bool skip_to_next_point;// 1
        public bool skip_to_prev_point;// 1
        public bool play_firstplay;// 1
        public bool stop;// 1
        public bool pause_on;// 1
        public bool pause_off;// 1
        public bool still;// 1
        public bool forward;// 1
        public bool backward;// 1
        public bool resume;// 1
        public bool move_up;// 1
        public bool move_down;// 1
        public bool move_left;// 1
        public bool move_right;// 1
        public bool select;// 1
        public bool activate;// 1
        public bool select_and_activate;// 1
        public bool primary_audio_change;// 1
        public bool angle_change;// 1
        public bool popup_on;// 1
        public bool popup_off;// 1
        public bool pg_enable_disable;// 1
        public bool pg_change;// 1
        public bool secondary_video_enable_disable;// 1
        public bool secondary_video_change;// 1
        public bool secondary_audio_enable_disable;// 1
        public bool secondary_audio_change;// 1
        public bool pip_pg_change;// 1
        private int uo1;
        private int uo2;

        public int UO1
        {
            get { return uo1; }
         }

        public int UO2
        {
            get { return uo2; }
        }
        public BD_UO_MASK(byte[] buff)
        {
            BitStreamReader bs = new BitStreamReader(buff, true);
            uo1 = bs.ReadInteger();
            uo2 = bs.ReadInteger();
            bs.BitPosition = 0;
            menu_call = bs.ReadBool();
            title_search = bs.ReadBool();
            chapter_search = bs.ReadBool();
            time_search = bs.ReadBool();
            skip_to_next_point = bs.ReadBool();
            skip_to_prev_point = bs.ReadBool();
            play_firstplay = bs.ReadBool();
            stop = bs.ReadBool();
            pause_on = bs.ReadBool();
            pause_off = bs.ReadBool();
            still = bs.ReadBool();
            forward = bs.ReadBool();
            backward = bs.ReadBool();
            resume = bs.ReadBool();
            move_up = bs.ReadBool();
            move_down = bs.ReadBool();
            move_left = bs.ReadBool();
            move_right = bs.ReadBool();
            select = bs.ReadBool();
            activate = bs.ReadBool();
            select_and_activate = bs.ReadBool();
            primary_audio_change = bs.ReadBool();
            bs.SkipBit(1);
            angle_change = bs.ReadBool();
            popup_on = bs.ReadBool();
            popup_off = bs.ReadBool();
            pg_enable_disable = bs.ReadBool();
            pg_change = bs.ReadBool();
            secondary_video_enable_disable = bs.ReadBool();
            secondary_video_change = bs.ReadBool();
            secondary_audio_enable_disable = bs.ReadBool();
            secondary_audio_change = bs.ReadBool();
            bs.SkipBit(1);
            pip_pg_change = bs.ReadBool();
        }
    }
    public class CLPI_STC_SEQ : LOCALIZED_DATA
    {
        private Int16 pcr_pid;
        private Int32 spn_stc_start;
        private float presentation_start_time;
        private float presentation_end_time;
        #region Properties
        public Int16 Pcr_pid
        {
            get { return pcr_pid; }
            set { pcr_pid = value; }
        }
        public float Presentation_start_time
        {
            get { return presentation_start_time / 45000; }
         }
        public float Presentation_end_time
        {
            get { return presentation_end_time / 45000; }
        }
        #endregion
        public CLPI_STC_SEQ(BitStreamReader bs)
        {
            pcr_pid = bs.ReadShort();
            spn_stc_start = bs.ReadInteger();
            presentation_start_time = bs.ReadInteger();
            presentation_end_time = bs.ReadInteger();

        }
    }
    public class CLPI_ATC_SEQ : LOCALIZED_DATA
    {
        public Int32 spn_atc_start;
        public byte num_stc_seq;
        public byte offset_stc_id;
        private List<CLPI_STC_SEQ> stc_seq;
        #region Properties
        public List<CLPI_STC_SEQ> Stc_seq
        {
            get { return stc_seq; }
            set { stc_seq = value; }
        }
        #endregion
        public CLPI_ATC_SEQ(BitStreamReader bs)
        {
            spn_atc_start = bs.ReadInteger();
            num_stc_seq = bs.ReadByte();
            offset_stc_id = bs.ReadByte();
            stc_seq = new List<CLPI_STC_SEQ>();
            for (int jj = 0; jj < num_stc_seq; jj++)
            {
                CLPI_STC_SEQ seq = new CLPI_STC_SEQ(bs);
                stc_seq.Add(seq);
            }

        }
    }
    public class CLPI_SEQ_INFO : LOCALIZED_DATA
    {
        public byte num_atc_seq;
        private List<CLPI_ATC_SEQ> atc_seq;
        #region Properties
        public List<CLPI_ATC_SEQ> Atc_seq
        {
            get { return atc_seq; }
            set { atc_seq = value; }
        }
        #endregion
        public CLPI_SEQ_INFO(BitStreamReader bs, int sequence_info_start_addr)
        {
            bs.BitPosition = 8 * sequence_info_start_addr;

            // SkipBit the length field, and a reserved byte
            bs.SkipBit(5 * 8);
            // Then get the number of sequences
            num_atc_seq = bs.ReadByte();
            atc_seq = new List<CLPI_ATC_SEQ>();
            for (int ii = 0; ii < num_atc_seq; ii++)
            {
                CLPI_ATC_SEQ seq = new CLPI_ATC_SEQ(bs);
                atc_seq.Add(seq);
            }
        }
    }
    public struct CLPI_TS_TYPE
    {
        private byte validity;
        private string format_id;
        #region Properties
        public byte Validity
        {
            get { return validity; }
            set { validity = value; }
        }
        public string Format_id
        {
            get { return format_id; }
            set { format_id = value; }
        }
        #endregion
    }
    public class CLPI_ATC_DELTA : LOCALIZED_DATA
    {
        private int delta;
        private string file_id;
        private string file_code;

        public int Delta
        {
            get { return delta; }
            set { delta = value; }
        }
        public string File_id
        {
            get { return file_id; }
            set { file_id = value; }
        }
        public string File_code
        {
            get { return file_code; }
            set { file_code = value; }
        }
        public CLPI_ATC_DELTA(BitStreamReader bs)
        {
            delta = bs.ReadInteger();
            file_id = bs.ReadString(5);
            file_code = bs.ReadString(4);
            bs.SkipBit(8);
        }
    }
    public class CLPI_CLIP_INFO : LOCALIZED_DATA
    {
        private byte application_type;
        private int ts_recording_rate;
        private CLPI_TS_TYPE ts_type_info;
        private byte atc_delta_count;
        private List<CLPI_ATC_DELTA> atc_delta;
        private bool is_atc_delta; // Arrival Time Clock
        private int num_source_packets;
        public byte clip_stream_type;
        #region Properties
        public bool Is_atc_delta
        {
            get { return is_atc_delta; }
            set { is_atc_delta = value; }
        }
        public int Num_source_packets
        {
            get { return num_source_packets; }
            set { num_source_packets = value; }
        }
        public string Application_Type
        {
            get
            {
                switch (application_type)
                {
                    case 0: return "reserved";
                    case 1: return "Main TS for a movie";
                    case 2: return "Main TS for a time based slide-show";
                    case 3: return "Main TS for a browsable slide-show";
                    case 4: return "Sub TS for a browsable slide-show";
                    case 5: return "Sub TS for a text subtitle";
                    case 6: return "Sub TS for a one or more elementary streams path";
                    case 7: return "Sub TS for a sub-path of a stereoscopic movie";
                    case 8: return "Sub TS for a sub-path of a stereoscopic IG menu";
                    case 9: return "Main TS for a movie (AVCHD 1080p50/60)";
                    default: return "";
                }
            }
        }
        public int Ts_recording_rate
        {
            get { return ts_recording_rate; }
            set { ts_recording_rate = value; }
        }
        public CLPI_TS_TYPE Ts_type_info
        {
            get { return ts_type_info; }
            set { ts_type_info = value; }
        }
        public List<CLPI_ATC_DELTA> Atc_delta
        {
            get { return atc_delta; }
            set { atc_delta = value; }
        }
        #endregion
        public CLPI_CLIP_INFO(BitStreamReader bs)
        {
            bs.BitPosition = 0x28 * 8;
            int ClipInfoLength = bs.ReadInteger();
            // reserved
            bs.SkipBit(16);
            clip_stream_type = bs.ReadByte();
            application_type = bs.ReadByte();
            // SkipBit reserved 31 bits
            bs.SkipBit(31);
            is_atc_delta = bs.ReadBool();//0x30 last bit
            Ts_recording_rate = bs.ReadInteger();//0x34
            num_source_packets = bs.ReadInteger();//0x38
            // SkipBit reserved 128 bytes
            bs.SkipBit(128 * 8);
            // ts type info block
            int len = bs.ReadShort();
            int pos = bs.Position;
            if (len > 0)
            {
                ts_type_info.Validity = bs.ReadByte();
                ts_type_info.Format_id = bs.ReadString(4);
                // Seek past the stuff we don't know anything about
                bs.BitPosition = (pos + len) * 8;
            }
            if (is_atc_delta)
            {
                // SkipBit reserved bytes
                bs.SkipBit(8);
                atc_delta_count = bs.ReadByte();
                atc_delta = new List<CLPI_ATC_DELTA>();
                for (int ii = 0; ii < atc_delta_count; ii++)
                {
                    CLPI_ATC_DELTA cpa = new CLPI_ATC_DELTA(bs);
                    atc_delta.Add(cpa);
                }
            }

        }
    }
    public class CLPI_PROG_STREAM : LOCALIZED_DATA
    {
        private string[] audioCodec = new string[] { "AC3", "DTS", "Dolby Lossless", "DD+", "DTS-HD", "DTS-HD XLL", "DD+", "DTS-HD" };
        private string[] frequency = new string[] { "-", "48 Khz", "-", "-", "96Khz", "192Khz", "", "", "", "", "", "", "", "", "", "" };
        private string[] surround = new string[] { "-", "Mono", "-", "Stereo", "-", "-", "Multi Channel", "-", "-", "-", "-", "-", "ST+", "-", "-" };
        private string[] videoType = new string[] { "-", "480i", "576i", "480", "1080i", "720p", "1080p", "576p" };
        private string[] frameRate = new string[] { "-", "23,976", "24", "25", "29,97", "-", "50", "59,94" };
        private string[] aspectList = new string[] { "", "", "4:3", "16:9" };
        private short pid;
        public byte coding_type;
        public byte format;
        public byte rate;
        public byte aspect;
        public bool oc_flag;
        public byte char_code;
        #region Properties
        private string lang;
        public string Format
        {
            get
            {
                switch (coding_type)
                {
                    case 0x01:
                    case 0x02:
                    case 0xea:
                    case 0x1b: return videoType[format];
                    case 0x03:
                    case 0x04:
                    case 0x80:
                    case 0x81:
                    case 0x82:
                    case 0x83:
                    case 0x84:
                    case 0x85:
                    case 0x86:
                    case 0xa1:
                    case 0xa2: return surround[format];
                    case 0x90:
                    case 0x91:
                    case 0xa0:
                    case 0x92:
                    default: return "";
                }
            }
        }
        public string Coding
        {
            get
            {
                switch (coding_type)
                {
                    case 0x01: return "VIDEO MPEG1";
                    case 0x02: return "VIDEO MPEG2";
                    case 0x03: return "AUDIO MPEG1";
                    case 0x04: return "AUDIO MPEG2";
                    case 0x80: return "AUDIO LPCM";
                    case 0x81: return "AUDIO AC3";
                    case 0x82: return "AUDIO DTS";
                    case 0x83: return "AUDIO TRUHD";
                    case 0x84: return "AUDIO AC3PLUS";
                    case 0x85: return "AUDIO DTSHD";
                    case 0x86: return "AUDIO DTSHD MASTER";
                    case 0xea: return "VIDEO VC1";
                    case 0x1b: return "VIDEO H264";
                    case 0x90: return "SUB PG";
                    case 0x91: return "SUB IG";
                    case 0x92: return "SUB TEXT";
                    case 0xa1: return "AUDIO AC3PLUS SECONDARY";
                    case 0xa2: return "AUDIO DTSHD SECONDARY";
                    default: return "";
                }
            }
        }
        public string FrameRate
        {
            get
            {
                switch (coding_type)
                {
                    case 0x01:
                    case 0x02:
                    case 0xea:
                    case 0x1b: return frameRate[rate];
                    case 0x03:
                    case 0x04:
                    case 0x80:
                    case 0x81:
                    case 0x82:
                    case 0x83:
                    case 0x84:
                    case 0x85:
                    case 0x86:
                    case 0xa1:
                    case 0xa2: return frequency[rate];
                    case 0x90:
                    case 0x91:
                    case 0xa0:
                    case 0x92:
                    default: return "";
                }
            }
        }
        public string Aspect
        {
            get
            {
                switch (coding_type)
                {
                    case 0x01:
                    case 0x02:
                    case 0xea:
                    case 0x1b: return aspectList[aspect];
                    case 0x03:
                    case 0x04:
                    case 0x80:
                    case 0x81:
                    case 0x82:
                    case 0x83:
                    case 0x84:
                    case 0x85:
                    case 0x86:
                    case 0xa1:
                    case 0xa2:
                    case 0x90:
                    case 0x91:
                    case 0xa0:
                    case 0x92:
                    default: return "";
                }
            }
        }
        public string Lang
        {
            get { return lang; }
            set { lang = value; }
        }
        public short PID
        {
            get { return pid; }
            set { pid = value; }
        }
        public string Details
        {
            get
            {
                string s = "";
                s = Format + " " + FrameRate + " " + Aspect;
                
                return s;
            }
        }
        #endregion
        public CLPI_PROG_STREAM(BitStreamReader bs)
        {
            pid = bs.ReadShort();
            int len = bs.ReadByte();
            int pos = bs.Position;
            coding_type = bs.ReadByte();
            switch (coding_type)
            {
                #region Read data
                case 0x01:
                case 0x02:
                case 0xea:
                case 0x1b:
                    format = (byte)bs.ReadIntFromBits(4);
                    rate = (byte)bs.ReadIntFromBits(4);
                    aspect = (byte)bs.ReadIntFromBits(4);
                    bs.SkipBit(2);
                    oc_flag = bs.ReadBool();
                    bs.SkipBit(1);
                    break;

                case 0x03:
                case 0x04:
                case 0x80:
                case 0x81:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x85:
                case 0x86:
                case 0xa1:
                case 0xa2:
                    format = (byte)bs.ReadIntFromBits(4); ;
                    rate = (byte)bs.ReadIntFromBits(4); ;
                    lang = bs.ReadString(3);
                    break;

                case 0x90:
                case 0x91:
                case 0xa0:
                    lang = bs.ReadString(3);
                    break;

                case 0x92:
                    char_code = bs.ReadByte();
                    lang = bs.ReadString(3);
                    break;
                default:
                    break;
                #endregion
            };
            // SkipBit over any padding
            bs.BitPosition = 8 * (pos + len);
         }
    }
    public class CLPI_PROG : LOCALIZED_DATA
    {
        public Int32 spn_program_sequence_start;
        private Int16 program_map_pid;
        public byte num_streams;
        public byte num_groups;
        private List<CLPI_PROG_STREAM> streams;
        #region Properties
        public Int16 Program_map_pid
        {
            get { return program_map_pid; }
            set { program_map_pid = value; }
        }
        public List<CLPI_PROG_STREAM> Streams
        {
            get { return streams; }
            set { streams = value; }
        }
        #endregion
        public CLPI_PROG(BitStreamReader bs)
        {
            spn_program_sequence_start = bs.ReadInteger();
            program_map_pid = bs.ReadShort();
            num_streams = bs.ReadByte();
            num_groups = bs.ReadByte();
            streams = new List<CLPI_PROG_STREAM>();
            for (int jj = 0; jj < num_streams; jj++)
            {
                CLPI_PROG_STREAM ps = new CLPI_PROG_STREAM(bs);
                streams.Add(ps);
            }
        }
    }
    public class CLPI_PROG_INFO : LOCALIZED_DATA
    {
        public byte num_prog;
        private List<CLPI_PROG> progs;
        #region Properties
        public List<CLPI_PROG> Progs
        {
            get { return progs; }
            set { progs = value; }
        }
        #endregion
        public CLPI_PROG_INFO(BitStreamReader bs, int program_info_start_addr)
        {
            bs.BitPosition = 8 * program_info_start_addr;
            // SkipBit the length field, and a reserved byte
            bs.SkipBit(5 * 8);
            // Then get the number of sequences
            num_prog = bs.ReadByte();

            progs = new List<CLPI_PROG>();
            for (int ii = 0; ii < num_prog; ii++)
            {
                CLPI_PROG prog = new CLPI_PROG(bs);
                progs.Add(prog);
            }
        }
    }
    public class CLPI_EP_COARSE : LOCALIZED_DATA
    {
        private int ref_ep_fine_id;
        private int pts_ep;
       private int spn_ep;
       #region Properties
       public int Ref_ep_fine_id
        {
            get { return ref_ep_fine_id; }
            set { ref_ep_fine_id = value; }
        }
        public int Pts_ep
        {
            get { return pts_ep; }
            set { pts_ep = value; }
        }

        public int Spn_ep
        {
            get { return spn_ep; }
            set { spn_ep = value; }
        }
       #endregion
        public CLPI_EP_COARSE(BitStreamReader bs)
        {
            ref_ep_fine_id = bs.ReadIntFromBits(18);
            pts_ep = bs.ReadIntFromBits(14);
            spn_ep = bs.ReadInteger();
        }
    }
    public class CLPI_EP_FINE : LOCALIZED_DATA
    {
        private bool is_angle_change_point;
       private byte i_end_position_offset;
       private int pts_ep;
        private int spn_ep;
        #region Properties
        public bool Is_angle_change_point
        {
            get { return is_angle_change_point; }
            set { is_angle_change_point = value; }
        }
         public byte I_end_position_offset
        {
            get { return i_end_position_offset; }
            set { i_end_position_offset = value; }
        }
        public int Pts_ep
        {
            get { return pts_ep; }
            set { pts_ep = value; }
        }
        public int Spn_ep
        {
            get { return spn_ep; }
            set { spn_ep = value; }
        }
        #endregion
        public CLPI_EP_FINE(BitStreamReader bs)
        {
            is_angle_change_point = bs.ReadBool();
            i_end_position_offset = (byte)bs.ReadIntFromBits(3);
            pts_ep = bs.ReadIntFromBits(11);
            spn_ep = bs.ReadIntFromBits(17);
        }
    } ;
    public class CLPI_EP_MAP_ENTRY : LOCALIZED_DATA
    {
        private short pid;
        private byte ep_stream_type;
        public int num_ep_coarse;
        public int num_ep_fine;
        public Int32 ep_map_stream_start_addr;
        private List<CLPI_EP_COARSE> coarse;
        private List<CLPI_EP_FINE> fine;
        #region Properties
        public byte Ep_stream_type
        {
            get { return ep_stream_type; }
            set { ep_stream_type = value; }
        }
        public short PID
        {
            get { return pid; }
            set { pid = value; }
        }
        public List<CLPI_EP_COARSE> Coarse
        {
            get { return coarse; }
            set { coarse = value; }
        }
        public List<CLPI_EP_FINE> Fine
        {
            get { return fine; }
            set { fine = value; }
        }
        #endregion
        public CLPI_EP_MAP_ENTRY(BitStreamReader bs, int ep_map_pos)
        {
            pid = bs.ReadShort();
            bs.SkipBit(10);
            ep_stream_type = (byte)bs.ReadIntFromBits(4);
            num_ep_coarse = bs.ReadIntFromBits(16);
            num_ep_fine = bs.ReadIntFromBits(18);
            ep_map_stream_start_addr = bs.ReadInteger() + ep_map_pos;
        }
        public void _parse_ep_map_stream(BitStreamReader bs)
        {
            bs.BitPosition = 8 * ep_map_stream_start_addr;
            int fine_start = bs.ReadInteger();

            coarse = new List<CLPI_EP_COARSE>();
            for (int ii = 0; ii < num_ep_coarse; ii++)
            {
                CLPI_EP_COARSE coar = new CLPI_EP_COARSE(bs);
                coarse.Add(coar);
            }
            bs.BitPosition = 8 * (ep_map_stream_start_addr + fine_start);
            fine = new List<CLPI_EP_FINE>();
            for (int ii = 0; ii < num_ep_fine; ii++)
            {
                CLPI_EP_FINE fin = new CLPI_EP_FINE(bs);
                fine.Add(fin);
            }
        }
    }
    public class CLPI_CPI : LOCALIZED_DATA
    {
        private List<CLPI_EP_MAP_ENTRY> entry;
        private byte type;
        // ep_map
        public byte num_stream_pid;
        #region Properties
        public byte Type
        {
            get { return type; }
            set { type = value; }
        }
        public List<CLPI_EP_MAP_ENTRY> Entry
        {
            get { return entry; }
            set { entry = value; }
        }
        #endregion
        public CLPI_CPI(BitStreamReader bs, int cpi_start_addr)
        {
            bs.BitPosition = 8 * cpi_start_addr;
            int len = bs.ReadInteger();
            if (len == 0)
            {
                return;
            }

            bs.SkipBit(12);
            type = (byte)bs.ReadIntFromBits(4);
            int ep_map_pos = bs.Position;

            // EP Map starts here
            bs.SkipBit(8);
            num_stream_pid = bs.ReadByte();

            entry = new List<CLPI_EP_MAP_ENTRY>();
            for (int ii = 0; ii < num_stream_pid; ii++)
            {
                CLPI_EP_MAP_ENTRY ent = new CLPI_EP_MAP_ENTRY(bs, ep_map_pos);
                entry.Add(ent);
            }
            for (int ii = 0; ii < num_stream_pid; ii++)
            {
                CLPI_EP_MAP_ENTRY ent = entry[ii];
                ent._parse_ep_map_stream(bs);//            _parse_ep_map_stream(bs, ref entry );
                entry[ii] = ent;
            }

        }
    }
    #endregion

}
