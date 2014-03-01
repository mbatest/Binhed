using System;
using System.Collections.Generic;
using System.IO;
using Utils;
using System.Text;

namespace BluRay
{
    public class MPLS : LOCALIZED_DATA
    {
        private string header;
        private int list_pos;
        private int mark_pos;
        private int ext_pos;
        private MPLS_AI app_info;
        private short list_count;
        private short sub_count;
        private short mark_count;
        private string name;
        private List<MPLS_PI> play_item;
        private List<MPLS_SUB> sub_path;
        private List<MPLS_PLM> play_mark;
        private List<CLPI> clips = new List<CLPI>();
        private string longName;
        #region Private Methods
        /*       MPLS_STN _parse_stn(BitStreamReader bs)
        {
            MPLS_STN stn = new MPLS_STN();

            // SkipBit STN len
            int len = bs.ReadShort();
            int pos = bs.Position;
            // SkipBit 2 reserved bytes
            bs.SkipBit(16);
            #region stream numbers
            stn.num_video = bs.ReadByte();
            stn.num_audio = bs.ReadByte();
            stn.num_pg = bs.ReadByte();
            stn.num_ig = bs.ReadByte();
            stn.num_secondary_audio = bs.ReadByte();
            stn.num_secondary_video = bs.ReadByte();
            stn.num_pip_pg = bs.ReadByte();
            #endregion
            // 5 reserve bytes
            bs.SkipBit(40);

            #region Primary Video Streams
            if (stn.num_video > 0)
            {
                stn.video = new List<MPLS_STREAM>();
                for (int ii = 0; ii < stn.num_video; ii++)
                {
                    stn.video.Add(new MPLS_STREAM(bs));
                }
            }
            #endregion
            #region Primary Audio Streams
            if (stn.num_audio > 0)
            {
                stn.audio = new List<MPLS_STREAM>();
                for (int ii = 0; ii < stn.num_audio; ii++)
                {
                    stn.audio.Add(new MPLS_STREAM(bs));
                }
            }
            #endregion
            #region Presentation Graphic Streams
            if ((stn.num_pg > 0) || (stn.num_pip_pg) > 0)
            {
                stn.pg = new List<MPLS_STREAM>();
                for (int ii = 0; ii < (stn.num_pg + stn.num_pip_pg); ii++)
                {
                    stn.pg.Add(new MPLS_STREAM(bs));
                }
            }
            #endregion
            #region Interactive Graphic Streams
            if (stn.num_ig > 0)
            {
                stn.ig = new List<MPLS_STREAM>();
                for (int ii = 0; ii < stn.num_ig; ii++)
                {
                    stn.ig.Add(new MPLS_STREAM(bs));
                }
            }
            #endregion
            #region Secondary audio
            if (stn.num_secondary_audio > 0)
            {
                stn.secondary_audio = new List<MPLS_STREAM>();
                for (int ii = 0; ii < stn.num_secondary_audio; ii++)
                {
                    MPLS_STREAM ssa = new MPLS_STREAM(bs);
                    stn.secondary_audio.Add(ssa);
                    // Read Secondary Audio Extra Attributes
                    ssa.sa_num_primary_audio_ref = bs.ReadByte();
                    bs.SkipBit(8);
                    if (ssa.sa_num_primary_audio_ref > 0)
                    {
                        ssa.sa_primary_audio_ref = new List<byte>();
                        for (int jj = 0; jj < ssa.sa_num_primary_audio_ref; jj++)
                        {
                            ssa.sa_primary_audio_ref.Add(bs.ReadByte());
                        }
                        if ((ssa.sa_num_primary_audio_ref % 2) == 0)
                        {
                            bs.SkipBit(8);
                        }
                    }
                }
            }
            #endregion
            # region Secondary Video Streams
             if (stn.num_secondary_video > 0)
            {
                stn.secondary_video = new List<MPLS_STREAM>();
                for (int ii = 0; ii < stn.num_secondary_video; ii++)
                {
                    MPLS_STREAM ssb = new MPLS_STREAM(bs);
                    stn.secondary_video.Add(ssb);
                    // Read Secondary Video Extra Attributes
                    sv_num_secondary_audio_ref = bs.ReadByte();
                    bs.SkipBit(8);
                    if (sv_num_secondary_audio_ref > 0)
                    {
                        sv_secondary_audio_ref = new List<byte>();
                        for (int jj = 0; jj < sv_num_secondary_audio_ref; jj++)
                        {
                            sv_secondary_audio_ref.Add(bs.ReadByte());
                        }
                        if ((sv_num_secondary_audio_ref % 2) == 0)
                        {
                            bs.SkipBit(8);
                        }
                    }
                    sv_num_pip_pg_ref = bs.ReadByte();
                    bs.SkipBit(8);
                    if (sv_num_pip_pg_ref > 0)
                    {
                        sv_pip_pg_ref = new List<byte>();
                        for (int jj = 0; jj < sv_num_pip_pg_ref; jj++)
                        {
                            sv_pip_pg_ref.Add(bs.ReadByte());
                        }
                        if ((sv_num_pip_pg_ref % 2) == 0)
                        {
                            bs.SkipBit(8);
                        }
                    }

                }
            }
             #endregion
             bs.bitPosition = (pos + len) * 8;
            return stn;
        }
        MPLS_PI _parse_playitem(BitStreamReader bs)
        {
            MPLS_PI pi = new MPLS_PI();
            string clip_id;//[6],
            string codec_id;//[5];
            byte stc_id;
            // PlayItem Length
            int len = bs.ReadShort();
            int pos = bs.Position ;
            // Primary Clip identifer
            clip_id = bs.ReadString(5);
            codec_id = bs.ReadString(4);
            if (codec_id != "M2TS")
            {
                //      //fprintf(stderr, "Incorrect CodecIdentifier (%s)\n", codec_id);
            }
            // SkipBit reserved 11 bits
            bs.SkipBit(11);

            pi.is_multi_angle = bs.ReadBool();

            pi.connection_condition = (byte)bs.ReadIntFromBits(4);
            if (pi.connection_condition != 0x01 &&
                pi.connection_condition != 0x05 &&
                pi.connection_condition != 0x06)
            {

                //       //fprintf(stderr, "Unexpected connection condition %02x\n",                 pi.connection_condition);
            }

            stc_id = bs.ReadByte();
            pi.in_time = bs.ReadInteger();
            pi.out_time = bs.ReadInteger();
            pi.uo_mask = new BD_UO_MASK(bs.ReadBytes(8));
            pi.random_access_flag = bs.ReadBool();
            bs.SkipBit(7);
            pi.still_mode = bs.ReadByte();
            if (pi.still_mode == 0x01)
            {
                pi.still_time = bs.ReadShort();
            }
            else
            {
                bs.SkipBit(16);
            }

            pi.angle_count = 1;
            if (pi.is_multi_angle)
            {
                pi.angle_count = bs.ReadByte();
                if (pi.angle_count < 1)
                {
                    pi.angle_count = 1;
                }
                bs.SkipBit(6);
                pi.is_different_audio = bs.ReadBool();
                pi.is_seamless_angle = bs.ReadBool();
            }
            pi.clip = new List<MPLS_CLIP>();
            MPLS_CLIP clp = new MPLS_CLIP(clip_id, codec_id,stc_id);
            pi.clip.Add(clp);
            for (int ii = 1; ii < pi.angle_count; ii++)
            {
                clp = new MPLS_CLIP(bs);
                pi.clip.Add(clp);
            }
            pi.stn = new MPLS_STN(bs);
            // Seek past any unused items
            bs.bitPosition = (pos + len) * 8;
            return pi;
        }*/
        void Parse_playlistmark(BitStreamReader bs)
        {
            bs.BitPosition = mark_pos * 8;
            // SkipBit the length field, I don't use it
            bs.SkipBit(32);
            // Then get the number of marks
            mark_count = bs.ReadShort();
            play_mark = new List<MPLS_PLM>();
            for (int ii = 0; ii < mark_count; ii++)
            {
                MPLS_PLM plmi = new MPLS_PLM(bs);
                play_mark.Add(plmi);
            }
        }
        void Parse_playlist(BitStreamReader bs)
        {
            bs.BitPosition = list_pos * 8;
            // SkipBit playlist length
            int playlist_length = bs.ReadInteger();// bs.SkipBit(32);
            // SkipBit reserved bytes
            bs.SkipBit(16);
            list_count = bs.ReadShort();
            sub_count = bs.ReadShort();
            play_item = new List<MPLS_PI>();
            for (int ii = 0; ii < list_count; ii++)
            {
                MPLS_PI pi = new MPLS_PI(bs);
                play_item.Add(pi);
            }
            if (sub_count > 0)
            {
                sub_path = new List<MPLS_SUB>();
                for (int ii = 0; ii < sub_count; ii++)
                {
                    MPLS_SUB sub = new MPLS_SUB(bs);
                }
            }
        }
        #endregion
        #region Properties
        public string Header
        {
            get { return header; }
            set { header = value; }
        }
        public MPLS_AI App_info
        {
            get { return app_info; }
            set { app_info = value; }
        }
        public List<MPLS_PI> Play_item
        {
            get { return play_item; }
            set { play_item = value; }
        }
        public List<MPLS_SUB> Sub_path
        {
            get { return sub_path; }
            set { sub_path = value; }
        }
        public List<MPLS_PLM> Play_mark
        {
            get { return play_mark; }
            set { play_mark = value; }
        }
        public List<CLPI> Clips
        {
            get { return clips; }
            set { clips = value; }
        }
        public string LongName
        {
            get { return longName; }
            set { longName = value; }
        }
        #endregion
        public override string ToString()
        {
            return name;
        }
        public MPLS(string fileName)
        {
            longName = fileName;
            name = Path.GetFileNameWithoutExtension(fileName);
            BitStreamReader bs = new BitStreamReader(fileName, true);
            header = bs.ReadString(8);
            list_pos = bs.ReadInteger();
            mark_pos = bs.ReadInteger();
            ext_pos = bs.ReadInteger();
            // SkipBit 160 reserved bits
            bs.SkipBit(160);
            app_info = new MPLS_AI(bs);
            Parse_playlist(bs);
            Parse_playlistmark(bs);
            #region Date
            bs.BitPosition = 8 * ext_pos;
            byte[] buffer = bs.ReadBytes(0x18);
            string g2 = bs.ReadString(4);// BufferConvert.ReadString(buffer, 0, 4);//Normalement PLEX
            bs.ReadBytes(0x3d);
            string date = bs.ReadString(10);// BufferConvert.ReadString(buffer, 0x3d, 10);
            buffer = new byte[66];
            bs.ReadBytes(0xFD);
            for (int j = 0; j < play_item.Count; j++)
            {
                bs.ReadBytes(0x08);
                //    To be checked   ???
                int jour = bs.ReadBCD(); // BufferConvert.ReadBCD(buffer[index]);
                int heure = bs.ReadBCD(); //BufferConvert.ReadBCD(buffer[index + 1]);
                int min = bs.ReadBCD(); //BufferConvert.ReadBCD(buffer[index + 2]);
                int sec = bs.ReadBCD(); //BufferConvert.ReadBCD(buffer[index + 3]);*/
                bs.ReadBytes(0x0e - 4);
                play_item[j].Date = bs.ReadString(10);
                bs.ReadBytes(0x22);
            }
            #endregion
            bs.Close();
        }
    }
    #region Mpls
    public class MPLS_STREAM : LOCALIZED_DATA
    {
        private string[] audioCodec = new string[] { "AC3", "DTS", "Dolby Lossless", "DD+", "DTS-HD", "DTS-HD XLL", "DD+", "DTS-HD" };
        private string[] frequency = new string[] { "-", "48 Khz", "-", "-", "96Khz", "192Khz", "", "", "", "", "", "", "", "", "", "" };
        private string[] surround = new string[] { "-", "Mono", "-", "Stereo", "-", "-", "Multi Channel", "-", "-", "-", "-", "-", "ST+", "-", "-" };
        private string[] videoType = new string[] { "-", "480i", "576i", "480", "1080i", "720p", "1080p", "576p" };
        private string[] frameRate = new string[] { "-", "23,976", "24", "25", "29,97", "-", "50", "59,94" };
        private string[] aspectList = new string[] { "", "", "4:3", "16:9" };
        private short pid;
        private byte subpath_id;
        private byte subclip_id;
        private byte char_code;

        private byte stream_type;
        private byte coding_type;
        private byte format;
        private byte rate;
        private string lang;//[4];

        // Secondary audio specific fields
        private byte sa_num_primary_audio_ref;
        private byte sv_num_secondary_audio_ref;
        private byte sv_num_pip_pg_ref;
        private List<byte> sa_primary_audio_ref;
        private List<byte> sv_secondary_audio_ref;
        private List<byte> sv_pip_pg_ref;
        #region Properties
        public short Pid
        {
            get { return pid; }
            set { pid = value; }
        }
        public byte Subpath_id
        {
            get { return subpath_id; }
            set { subpath_id = value; }
        }
        public byte Subclip_id
        {
            get { return subclip_id; }
            set { subclip_id = value; }
        }
        public byte Char_code
        {
            get { return char_code; }
            set { char_code = value; }
        }
        public string Stream_Type
        {
            get
            {
                switch (stream_type)
                {
                    case 1: return "PlayItem";
                    case 2: case 4: return "SubPath";
                    case 3: return "IMPiP";
                    default: return "---";
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
                    case 0x1b:
                        return frameRate[rate];
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
                        return frequency[rate];
                    case 0x90:
                    case 0x91:
                    case 0xa0:
                    case 0x92:
                    default: return "";
                }
            }
        }
        public string Format
        {
            get
            {
                switch (coding_type)
                {
                    case 0x01:
                    case 0x02:
                    case 0xea:
                    case 0x1b:
                        return videoType[format];
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
                        return surround[format];
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
        public List<byte> Sa_primary_audio_ref
        {
            get { return sa_primary_audio_ref; }
            set { sa_primary_audio_ref = value; }
        }
        public List<byte> Sv_secondary_audio_ref
        {
            get { return sv_secondary_audio_ref; }
            set { sv_secondary_audio_ref = value; }
        }
        public List<byte> Sv_pip_pg_ref
        {
            get { return sv_pip_pg_ref; }
            set { sv_pip_pg_ref = value; }
        }
        #endregion
        public long position;
        public long length;
        public MPLS_STREAM(BitStreamReader bs)
        {
            int len = bs.ReadByte();
            int pos = (int) bs.Position;
            position = bs.Position;
            stream_type = bs.ReadByte();
            /*         MPID = BufferConvert.ReadShortInteger(buf1, 1);
            SPid = BufferConvert.ReadShortInteger(buf1, 3); ;
            SCid = BufferConvert.ReadShortInteger(buf1, 5);
            SPID = BufferConvert.ReadShortInteger(buf1, 7);
 */
            #region
            switch (stream_type)
            {
                case 1:// Play Item
                    pid = bs.ReadShort();
                    break;
                case 2: // SubPath
                case 4:
                    subpath_id = bs.ReadByte();
                    subclip_id = bs.ReadByte();
                    pid = bs.ReadShort();
                    break;
                case 3://IM PIP
                    subpath_id = bs.ReadByte();
                    pid = bs.ReadShort();
                    break;
                default:
                    // //fprintf(stderr, "unrecognized stream type %02x\n", s.stream_type);
                    break;
            };
            #endregion
            bs.BitPosition = (pos + len) * 8;

            len = bs.ReadByte();
            pos = (int)bs.Position;
            lang = "";
            coding_type = bs.ReadByte();
            #region
            switch (coding_type)
            {
                case 0x01:
                case 0x02:
                case 0xea:
                case 0x1b:
                    format = (byte)bs.ReadIntFromBits(4);
                    rate = (byte)bs.ReadIntFromBits(4);
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
                    format = (byte)bs.ReadIntFromBits(4);
                    rate = (byte)bs.ReadIntFromBits(4);
                    lang = bs.ReadString(3);
                    break;
                case 0x90:
                case 0x91:
                    lang = bs.ReadString(3);
                    break;
                case 0x92:
                    char_code = bs.ReadByte();
                    lang = bs.ReadString(3);
                    break;
                default:
                    //    //fprintf(stderr, "unrecognized coding type %02x\n", s.coding_type);
                    break;

            };
            #endregion
            length = bs.Position - position;
            bs.BitPosition = (pos + len) * 8;
        }
        public void SecondaryAudio(BitStreamReader bs)
        {
            // Read Secondary Audio Extra Attributes
            sa_num_primary_audio_ref = bs.ReadByte();
            bs.SkipBit(8);
            if (sa_num_primary_audio_ref > 0)
            {
                sa_primary_audio_ref = new List<byte>();
                for (int jj = 0; jj < sa_num_primary_audio_ref; jj++)
                {
                    sa_primary_audio_ref.Add(bs.ReadByte());
                }
                if ((sa_num_primary_audio_ref % 2) == 0)
                {
                    bs.SkipBit(8);
                }
            }

        }
        public void SecondaryVideo(BitStreamReader bs)
        {
             // Read Secondary Video Extra Attributes
            sv_num_secondary_audio_ref = bs.ReadByte();
            bs.SkipBit(8);
            if (sv_num_secondary_audio_ref > 0)
            {
                sv_secondary_audio_ref = new List<byte>();
                for (int jj = 0; jj < sv_num_secondary_audio_ref; jj++)
                {
                    sv_secondary_audio_ref.Add(bs.ReadByte());
                }
                if ((sv_num_secondary_audio_ref % 2) == 0)
                {
                    bs.SkipBit(8);
                }
            }
            sv_num_pip_pg_ref = bs.ReadByte();
            bs.SkipBit(8);
            if (sv_num_pip_pg_ref > 0)
            {
                sv_pip_pg_ref = new List<byte>();
                for (int jj = 0; jj < sv_num_pip_pg_ref; jj++)
                {
                    sv_pip_pg_ref.Add(bs.ReadByte());
                }
                if ((sv_num_pip_pg_ref % 2) == 0)
                {
                    bs.SkipBit(8);
                }
            }

        }
    }
    public class MPLS_STN : LOCALIZED_DATA
    {
        private byte num_video;
        private byte num_audio;
        private byte num_pg;
        private byte num_ig;
        private byte num_secondary_audio;
        private byte num_secondary_video;
        private byte num_pip_pg;
        private List<MPLS_STREAM> video;
        private List<MPLS_STREAM> audio;
        private List<MPLS_STREAM> pg;
        private List<MPLS_STREAM> ig;
        private List<MPLS_STREAM> secondary_audio;
        private List<MPLS_STREAM> secondary_video;
        public long position;
        public long length;
        #region Properties
        public byte Num_video
        {
            get { return num_video; }
            set { num_video = value; }
        }
        public byte Num_audio
        {
            get { return num_audio; }
            set { num_audio = value; }
        }
        public byte Num_pg
        {
            get { return num_pg; }
            set { num_pg = value; }
        }
        public byte Num_ig
        {
            get { return num_ig; }
            set { num_ig = value; }
        }
        public byte Num_secondary_audio
        {
            get { return num_secondary_audio; }
            set { num_secondary_audio = value; }
        }
        public byte Num_secondary_video
        {
            get { return num_secondary_video; }
            set { num_secondary_video = value; }
        }
        public byte Num_pip_pg
        {
            get { return num_pip_pg; }
            set { num_pip_pg = value; }
        }
        public List<MPLS_STREAM> Video
        {
            get { return video; }
            set { video = value; }
        }
        public List<MPLS_STREAM> Audio
        {
            get { return audio; }
            set { audio = value; }
        }
        public List<MPLS_STREAM> Pg
        {
            get { return pg; }
            set { pg = value; }
        }
        public List<MPLS_STREAM> Ig
        {
            get { return ig; }
            set { ig = value; }
        }
        public List<MPLS_STREAM> Secondary_audio
        {
            get { return secondary_audio; }
            set { secondary_audio = value; }
        }
        public List<MPLS_STREAM> Secondary_video
        {
            get { return secondary_video; }
            set { secondary_video = value; }
        }
        #endregion
        public MPLS_STN(BitStreamReader bs)
        {
            position = bs.Position;
            int len = bs.ReadShort();
            int pos = (int) bs.Position;
            // SkipBit 2 reserved bytes
            bs.SkipBit(16);
            #region stream numbers
            num_video = bs.ReadByte();
            num_audio = bs.ReadByte();
            num_pg = bs.ReadByte();
            num_ig = bs.ReadByte();
            num_secondary_audio = bs.ReadByte();
            num_secondary_video = bs.ReadByte();
            num_pip_pg = bs.ReadByte();
            #endregion
            // 5 reserve bytes
            bs.SkipBit(40);
            #region Primary Video Streams
            if (num_video > 0)
            {
                video = new List<MPLS_STREAM>();
                for (int ii = 0; ii < num_video; ii++)
                {
                    video.Add(new MPLS_STREAM(bs));
                }
            }
            #endregion
            #region Primary Audio Streams
            if (num_audio > 0)
            {
                audio = new List<MPLS_STREAM>();
                for (int ii = 0; ii < num_audio; ii++)
                {
                    audio.Add(new MPLS_STREAM(bs));
                }
            }
            #endregion
            #region Presentation Graphic Streams
            if ((num_pg > 0) || (num_pip_pg) > 0)
            {
                pg = new List<MPLS_STREAM>();
                for (int ii = 0; ii < (num_pg + num_pip_pg); ii++)
                {
                    pg.Add(new MPLS_STREAM(bs));
                }
            }
            #endregion
            #region Interactive Graphic Streams
            if (num_ig > 0)
            {
                ig = new List<MPLS_STREAM>();
                for (int ii = 0; ii < num_ig; ii++)
                {
                    ig.Add(new MPLS_STREAM(bs));
                }
            }
            #endregion
            #region Secondary audio
            if (num_secondary_audio > 0)
            {
                secondary_audio = new List<MPLS_STREAM>();
                for (int ii = 0; ii < num_secondary_audio; ii++)
                {
                    MPLS_STREAM ssa = new MPLS_STREAM(bs);
                    secondary_audio.Add(ssa);
                    ssa.SecondaryAudio(bs);
                }
            }
            #endregion
            # region Secondary Video Streams
            if (num_secondary_video > 0)
            {
                secondary_video = new List<MPLS_STREAM>();
                for (int ii = 0; ii < num_secondary_video; ii++)
                {
                    MPLS_STREAM ssb = new MPLS_STREAM(bs);
                    secondary_video.Add(ssb);
                    ssb.SecondaryVideo(bs);
                }
            }
            #endregion
            length = bs.Position - position;
            bs.BitPosition = (pos + len) * 8;
        }
    }
    public class MPLS_CLIP : LOCALIZED_DATA
    {
        private string clip_id;//[6];
        private string codec_id;//[5];
        private byte stc_id;
        public long position;
        public long length;

        #region Properties
        public string Clip_id
        {
            get { return clip_id; }
            set { clip_id = value; }
        }
        public string Codec_id
        {
            get { return codec_id; }
            set { codec_id = value; }
        }
        public byte Stc_id
        {
            get { return stc_id; }
            set { stc_id = value; }
        }
        #endregion
        public MPLS_CLIP(BitStreamReader bs)
        {
            position = bs.Position;
            Clip_id = bs.ReadString(5);
            Codec_id = bs.ReadString(4);
            if (Codec_id != "M2TS")
            {
                //          //fprintf(stderr, "Incorrect CodecIdentifier (%s)\n", pi.clip[ii].codec_id);
            }
            Stc_id = bs.ReadByte();
            length = bs.Position - position;
        }
        public MPLS_CLIP(string clip_id, string codec_id, byte stc_id)
        {
            this.Clip_id = clip_id;
            this.Codec_id = codec_id;
            this.Stc_id = stc_id;
        }
    }
    public class MPLS_PI : LOCALIZED_DATA
    {
        private string clip_id;//[6],
        private string codec_id;//[5];
        private bool is_multi_angle;
        private int in_time;
        private int out_time;
        private BD_UO_MASK uo_mask;
        private bool random_access_flag;
        private byte still_mode;
        private short still_time;
        private byte angle_count;
        private bool is_different_audio;
        private bool is_seamless_angle;
        private List<MPLS_CLIP> clip;
        private MPLS_STN stn;
        private string date;
        private string name;
        private byte connection_condition;
        byte stc_id;
        public long position;
        public long length;
        #region Properties
        public string Clip_id
        {
            get { return clip_id; }
            set { clip_id = value; }
        }
        public byte Stc_id
        {
            get { return stc_id; }
            set { stc_id = value; }
        }
        public string Codec_id
        {
            get { return codec_id; }
            set { codec_id = value; }
        }
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        public bool Is_multi_angle
        {
            get { return is_multi_angle; }
            set { is_multi_angle = value; }
        }
        public byte Connection_condition
        {
            get { return connection_condition; }
            set { connection_condition = value; }
        }
        public float In_time
        {
            get { return (float) in_time/45000; }
        }
        public float Out_time
        {
            get { return (float)out_time/45000; }
        }
        public BD_UO_MASK Uo_mask
        {
            get { return uo_mask; }
            set { uo_mask = value; }
        }
        public bool Random_access_flag
        {
            get { return random_access_flag; }
            set { random_access_flag = value; }
        }
        public byte Still_mode
        {
            get { return still_mode; }
            set { still_mode = value; }
        }
        public short Still_time
        {
            get { return still_time; }
            set { still_time = value; }
        }
        public byte Angle_count
        {
            get { return angle_count; }
            set { angle_count = value; }
        }
        public bool Is_different_audio
        {
            get { return is_different_audio; }
            set { is_different_audio = value; }
        }
        public bool Is_seamless_angle
        {
            get { return is_seamless_angle; }
            set { is_seamless_angle = value; }
        }
        public List<MPLS_CLIP> Clips
        {
            get { return clip; }
            set { clip = value; }
        }
        public MPLS_STN STN
        {
            get { return stn; }
            set { stn = value; }
        }
        #endregion
        public MPLS_PI(BitStreamReader bs)
        {
            position = bs.Position;
            // PlayItem Length
            int len = bs.ReadShort();
            int pos = (int)bs.Position;
            // Primary Clip identifer
            clip_id = bs.ReadString(5);
            codec_id = bs.ReadString(4);
            if (codec_id != "M2TS")
            {
                //      //fprintf(stderr, "Incorrect CodecIdentifier (%s)\n", codec_id);
            }
            // SkipBit reserved 11 bits
            bs.SkipBit(11);
            is_multi_angle = bs.ReadBool();
            connection_condition = (byte)bs.ReadIntFromBits(4);
            if (connection_condition != 0x01 &&
                connection_condition != 0x05 &&
                connection_condition != 0x06)
            {

                //       //fprintf(stderr, "Unexpected connection condition %02x\n",                 connection_condition);
            }

            stc_id = bs.ReadByte();
            in_time = bs.ReadInteger();
            out_time = bs.ReadInteger();
            uo_mask = new BD_UO_MASK(bs.ReadBytes(8));
            random_access_flag = bs.ReadBool();
            bs.SkipBit(7);
            still_mode = bs.ReadByte();
            if (still_mode == 0x01)
            {
                still_time = bs.ReadShort();
            }
            else
            {
                bs.SkipBit(16);
            }

            angle_count = 1;
            if (is_multi_angle)
            {
                angle_count = bs.ReadByte();
                if (angle_count < 1)
                {
                    angle_count = 1;
                }
                bs.SkipBit(6);
                is_different_audio = bs.ReadBool();
                is_seamless_angle = bs.ReadBool();
            }
            clip = new List<MPLS_CLIP>();
            MPLS_CLIP clp = new MPLS_CLIP(clip_id, codec_id, stc_id);
            clip.Add(clp);
            for (int ii = 1; ii < angle_count; ii++)
            {
                clp = new MPLS_CLIP(bs);
                clip.Add(clp);
            }
            stn = new MPLS_STN(bs);
            // Seek past any unused items
            length = bs.Position - position;
            bs.BitPosition = (pos + len) * 8;

        }
        private string FormatTime(float time)
        {
            string s = "";
            double t = time / 45000;
            int sec = (int)Math.Floor(t);
            int dec = (int)((t - sec) * 1000);
            int min = sec / 60;
            sec = sec % 60;
            int hour = min / 60;
            min = min % 60;
            s = hour.ToString("D2") + ":" + min.ToString("D2") + ":" + sec.ToString("D2") + "." + dec.ToString("D3");
            return s;
        }

    }
    public class MPLS_PLM : LOCALIZED_DATA// Année et date ??
    {
        private byte mark_id;
        private byte mark_type;
        private short play_item_ref;
        private int time;
        private short entry_es_pid;
        private int duration;
        public long position;
        public long length;
        #region Properties
        public byte Mark_id
        {
            get { return mark_id; }
            set { mark_id = value; }
        }
        public string Mark_Type
        {
            get
            {
                switch (mark_type)
                {
                    case 1:
                        return "entry mark";
                    case 2:
                        return "link point";
                    default:
                        return "";
                }
            }
        }
        public short Play_item_ref
        {
            get { return play_item_ref; }
            set { play_item_ref = value; }
        }
        public float Time
        {
            get { return (float)time/45000; }
        }
        public short Entry_es_pid
        {
            get { return entry_es_pid; }
            set { entry_es_pid = value; }
        }
        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }
        #endregion
        public MPLS_PLM(BitStreamReader bs)
        {
            position = bs.Position;
            mark_id = bs.ReadByte();
            mark_type = bs.ReadByte();
            play_item_ref = bs.ReadShort();
            time = bs.ReadInteger();
            entry_es_pid = bs.ReadShort();
            duration = bs.ReadInteger();
            length = bs.Position - position;
        }
    }
    public class MPLS_AI : LOCALIZED_DATA
    {
        private byte playback_type;
        private short playback_count;
        private BD_UO_MASK uo_mask;
        private bool random_access_flag;
        private bool audio_mix_flag;
        private bool lossless_bypass_flag;
        public long position;
        public long length;

        #region Properties
        public string Playback_type
        {
            get
            {
                switch (playback_type)
                {
                    case 1: return "Sequential";
                    case 2: return "Random";
                    case 3: return "Shuffle";
                    default: return "";
                }
            }
        }
        public short Playback_count
        {
            get { return playback_count; }
            set { playback_count = value; }
        }
        public BD_UO_MASK Uo_mask
        {
            get { return uo_mask; }
            set { uo_mask = value; }
        }
        public bool Random_access_flag
        {
            get { return random_access_flag; }
            set { random_access_flag = value; }
        }
        public bool Audio_mix_flag
        {
            get { return audio_mix_flag; }
            set { audio_mix_flag = value; }
        }
        public bool Lossless_bypass_flag
        {
            get { return lossless_bypass_flag; }
            set { lossless_bypass_flag = value; }
        }
        #endregion
        public MPLS_AI(BitStreamReader bs)
        {
            position = bs.Position;
            int pos = (int)bs.Position;
            int len = bs.ReadInteger();

            // Reserved
            bs.SkipBit(8);
            playback_type = bs.ReadByte();
            if (playback_type == 2 || playback_type == 3)
            {
                playback_count = bs.ReadShort();
            }
            else
            {
                // Reserved
                bs.SkipBit(16);
            }
            uo_mask = new BD_UO_MASK(bs.ReadBytes(8));
            random_access_flag = bs.ReadBool();
            audio_mix_flag = bs.ReadBool();
            lossless_bypass_flag = bs.ReadBool();
            // Reserved
            bs.SkipBit(13);
            length = bs.Position - position;
            bs.BitPosition = (pos + len) * 8;
         }
    }
    public class MPLS_SUB_PI : LOCALIZED_DATA
    {
        private byte connection_condition;
        private bool is_multi_clip;
        private int in_time;
        private int out_time;
        private short sync_play_item_id;
        private int sync_pts;
        private byte clip_count;
        private List<MPLS_CLIP> clip;
        public long position;
        public long length;
        #region Properties
        public byte Connection_condition
        {
            get { return connection_condition; }
            set { connection_condition = value; }
        }
        public bool Is_multi_clip
        {
            get { return is_multi_clip; }
            set { is_multi_clip = value; }
        }
        public int In_time
        {
            get { return in_time; }
            set { in_time = value; }
        }
        public int Out_time
        {
            get { return out_time; }
            set { out_time = value; }
        }
        public short Sync_play_item_id
        {
            get { return sync_play_item_id; }
            set { sync_play_item_id = value; }
        }
        public int Sync_pts
        {
            get { return sync_pts; }
            set { sync_pts = value; }
        }
        public byte Clip_count
        {
            get { return clip_count; }
            set { clip_count = value; }
        }
        public List<MPLS_CLIP> Clip
        {
            get { return clip; }
            set { clip = value; }
        }
        #endregion
        public MPLS_SUB_PI(BitStreamReader bs)
        {
            int len, ii;
            int pos;
            string clip_id;
            string codec_id;
            byte stc_id;
            position = bs.Position;
            // PlayItem Length
            len = bs.ReadShort();
            pos = (int)bs.Position;

            // Primary Clip identifer
            clip_id = bs.ReadString(5);
            codec_id = bs.ReadString(4);
            if (codec_id != "M2TS")
            {
                //          //fprintf(stderr, "Incorrect CodecIdentifier (%s)\n", pi.clip[ii].codec_id);
            }

            bs.SkipBit(27);

            connection_condition = (byte)bs.ReadIntFromBits(4);

            if (connection_condition != 0x01 &&
                connection_condition != 0x05 &&
                connection_condition != 0x06)
            {

                //      //fprintf(stderr, "Unexpected connection condition %02x\n",                 connection_condition);
            }
            is_multi_clip = bs.ReadBool();
            stc_id = bs.ReadByte();
            in_time = bs.ReadInteger();
            out_time = bs.ReadInteger();
            sync_play_item_id = bs.ReadShort();
            sync_pts = bs.ReadInteger();
            clip_count = 1;
            if (is_multi_clip)
            {
                clip_count = bs.ReadByte();
                if (clip_count < 1)
                {
                    clip_count = 1;
                }
            }
            clip = new List<MPLS_CLIP>();
            MPLS_CLIP clp = new MPLS_CLIP(clip_id,codec_id,stc_id);
            clip.Add(clp);
            for (ii = 1; ii < clip_count; ii++)
            {
                clp = new MPLS_CLIP(bs);
                clip.Add(clp);
            }
            // Seek to end of subpath
            length = bs.Position - position;
            bs.BitPosition = (pos + len) * 8;
        }
    }
    public class MPLS_SUB : LOCALIZED_DATA
    {
        private byte type;
        private bool is_repeat;
        private List<MPLS_SUB_PI> sub_play_item;
        private byte sub_playitem_count;
        public long position;
        public long length;
        #region Properties
        public byte Type
        {
            get { return type; }
            set { type = value; }
        }
        public bool Is_repeat
        {
            get { return is_repeat; }
            set { is_repeat = value; }
        }
        public byte Sub_playitem_count
        {
            get { return sub_playitem_count; }
            set { sub_playitem_count = value; }
        }
        public List<MPLS_SUB_PI> Sub_play_item
        {
            get { return sub_play_item; }
            set { sub_play_item = value; }
        }
        #endregion
        public MPLS_SUB(BitStreamReader bs)
        {
            position = bs.Position;
            int len = bs.ReadInteger();
            int pos = (int)bs.Position;

            bs.SkipBit(8);
            Type = bs.ReadByte();
            bs.SkipBit(15);
            Is_repeat = bs.ReadBool();
            bs.SkipBit(8);
            sub_playitem_count = bs.ReadByte();

            sub_play_item = new List<MPLS_SUB_PI>();
            for (int ii = 0; ii < sub_playitem_count; ii++)
            {
                MPLS_SUB_PI spp = new MPLS_SUB_PI(bs);
                sub_play_item.Add(spp);
            }
            length = bs.Position - position;
            // Seek to end of subpath
            bs.BitPosition = (pos + len) * 8;

        }
    }
    #endregion
}
