using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;
// http://www.faqs.org/patents/app/20090317067
// http://git.videolan.org/?p=libbluray.git;a=tree;f=src/libbluray;hb=HEAD

namespace BluRay
{
    #region Indexbdmv
    public class INDEX_BDMV : LOCALIZED_DATA
    {
        public string header;
        public INDX_APP_INFO app_info;
        public INDX_PLAY_ITEM first_play;
        public INDX_PLAY_ITEM top_menu;
        public INDX_HDMV_OBJ hdmv;
        public short num_titles;
        public List<INDX_TITLE> titles;
        public string fileName;
        public INDEX_BDMV(string fileName)
        {
            this.fileName = fileName;
            BitStreamReader bs = new BitStreamReader(fileName, true);
            header = bs.ReadString(8);
            if (!header.Contains("INDX"))
            {
                return;
            }
            indexes_start = bs.ReadInteger();
            extension_data_start = bs.ReadInteger();
            bs.BitPosition = 0x28 * 8;
            app_info = new INDX_APP_INFO(bs);// _parse_app_info(bs);
            bs.BitPosition = indexes_start * 8;
            Parse_index(bs);
            bs.Close();
            MovieObject = new MOBJ_BDMV(Path.GetDirectoryName(bs.Name) + "\\MovieObject.bdmv");
             string dir = Path.GetDirectoryName(bs.Name) + "\\PLAYLIST\\";
            foreach (string fn in Directory.GetFiles(dir))
            {
                MPLS mp = new MPLS(fn);
                list.Add(mp);
            }
            #region Handles clips
            dir = Path.GetDirectoryName(bs.Name) + "\\CLIPINF\\";
            foreach (string fn in Directory.GetFiles(dir))
            {
                CLPI clp = new CLPI(fn);
                clips.Add(clp);
            }
            #endregion
            
        }
        #region extensions
        List<string> mpls = new List<string>();
        public MOBJ_BDMV MovieObject;
        public List<MPLS> list = new List<MPLS>();
        public List<CLPI> clips = new List<CLPI>();
        public List<CLPI> Clips
        {
            get { return clips; }
            set { clips = value; }
        }
        public List<MPLS> MplsList
        {
            get { return list; }
            set { list = value; }
        }
        #endregion
        int indexes_start;
        int extension_data_start;
        void Parse_index(BitStreamReader bs)
        {
            int ndex_len = bs.ReadInteger();
            /* TODO: check if goes to extension data area or EOF */
            first_play = new INDX_PLAY_ITEM(bs);// _parse_playback_obj(bs);
            top_menu = new INDX_PLAY_ITEM(bs);//_parse_playback_obj(bs);
            num_titles = bs.ReadShort();
            titles = new List<INDX_TITLE>(); ;
            for (int i = 0; i < num_titles; i++)
            {
                INDX_TITLE title = new INDX_TITLE(bs);
                titles.Add(title);
            }
        }
        bool Parse_header(BitStreamReader bs)
        {
            return true;
        }
        private void extension(BitStreamReader bs)
        {
            int lengthExtensionData = bs.ReadInteger();
            int lengthFirstblock = bs.ReadInteger();
            bs.ReadBytes(4);
            int ID1 = bs.ReadShort();
            int ID2 = bs.ReadShort();
            int st = bs.ReadInteger();
            int lg = bs.ReadInteger();
            //           bs.Position = extensionData + lengthFirstblock;
            String idex = bs.ReadString(4);
            bs.ReadBytes(4);//0181]An area "reserved" having a data length of 32 bits is arranged subsequent to the field TypeIndicator
            int TableOfPlayListStartAddress = bs.ReadInteger();
            bs.ReadInteger();
            bs.BitPosition += (TableOfPlayListStartAddress * 8);
            int nb = bs.ReadShort();
            for (int u = 0; u < nb; u++)
            {
                string n = bs.ReadString(5);
                mpls.Add(n + ".mpls");
                bs.ReadByte();
                int rank = bs.ReadInteger();
                titles[rank].Name = n;
            }
        }
    }
    public class INDX_BASE : LOCALIZED_DATA
    {
        public List<long> FieldPosition = new List<long>();
        public long position;
        public long length;
    }
    public class INDX_APP_INFO: INDX_BASE
    {
        #region Private members
        private bool initial_output_mode_preference;//: 1; /* 0 - 2D, 1 - 3D */
        private bool content_exist_flag;// 1;
        private indx_video_format video_format;
        private indx_frame_rate frame_rate;
        private byte[] user_data;
         #endregion
        #region Properties
        public string Mode2d
        {
            get
            {
                if (initial_output_mode_preference)
                    return "SS";
                else
                    return "2D";
            }
        }
        public bool Stereo
        {
            get
            {
                return (content_exist_flag);
            }
        }
        public string VideoFormat
        {
            get
            {
                switch (video_format)
                {
                    case indx_video_format.indx_video_480i: return "480i";
                    case indx_video_format.indx_video_576i: return "576i";
                    case indx_video_format.indx_video_480p: return "480p";
                    case indx_video_format.indx_video_1080i: return "1080i";
                    case indx_video_format.indx_video_720p: return "720p";
                    case indx_video_format.indx_video_1080p: return "1080p";
                    case indx_video_format.indx_video_576p: return "576p";
                    default: return "";
                }

            }
        }
        public string FrameRate
        {
            get
            {
                switch (frame_rate)
                {
                    case indx_frame_rate.indx_fps_23_976: return "23,976";
                    case indx_frame_rate.indx_fps_24: return "24";
                    case indx_frame_rate.indx_fps_25: return "25";
                    case indx_frame_rate.indx_fps_29_97: return "29,97";
                    case indx_frame_rate.indx_fps_50: return "50";
                    case indx_frame_rate.indx_fps_59_94: return "59,94";
                    default: return "-";
                }
            }
        }
        public byte[] User_data
        {
            get { return user_data; }
            set { user_data = value; }
        }
        #endregion
        public INDX_APP_INFO(BitStreamReader bs)
        {
            position = bs.Position;
            int len = bs.ReadInteger();
            bs.SkipBit(1);
            FieldPosition.Add(bs.BitPosition);
            initial_output_mode_preference = bs.ReadBool();
            FieldPosition.Add(bs.BitPosition);
            content_exist_flag = bs.ReadBool();
            bs.SkipBit(5);
            FieldPosition.Add(bs.BitPosition);
            video_format = (indx_video_format)bs.ReadIntFromBits(4);
            FieldPosition.Add(bs.BitPosition);
            frame_rate = (indx_frame_rate)bs.ReadIntFromBits(4);
            FieldPosition.Add(bs.BitPosition);
            user_data = bs.ReadBytes(0x20);
            length = bs.Position - position;
        }
    }
    public class INDX_BDJ_OBJ : INDX_PLAY_ITEM
    {
        private indx_bdj_playback_type playback_type;
        private string name;
        #region Properties
        public string Playback_Type
        {
            get
            {
                switch (playback_type)
                {
                    case indx_bdj_playback_type.indx_bdj_playback_type_movie: return "Movie";
                    case indx_bdj_playback_type.indx_bdj_playback_type_interactive: return "Interactive";
                    default: return "";

                }
            }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion
        public INDX_BDJ_OBJ(BitStreamReader bs)
        {
            position = bs.Position;
            FieldPosition.Add(bs.BitPosition);
            playback_type = (indx_bdj_playback_type)bs.ReadIntFromBits(2);
            bs.SkipBit(14);
            FieldPosition.Add(bs.BitPosition);
            name = bs.ReadString(5);
            bs.SkipBit(8);
            length = bs.Position - position;
        }
    }
    public class INDX_HDMV_OBJ : INDX_PLAY_ITEM
    {
        public indx_hdmv_playback_type playback_type;
        public string Playback_Type
        {
            get
            {
                switch (playback_type)
                {
                    case indx_hdmv_playback_type.indx_hdmv_playback_type_movie: return "Movie";
                    case indx_hdmv_playback_type.indx_hdmv_playback_type_interactive: return "Interactive";
                    default: return "";
                }
            }
        }
        public short id_ref;
        public INDX_HDMV_OBJ(BitStreamReader bs)
        {
            position = bs.Position;
            int a = bs.ReadIntFromBits(2);
            FieldPosition.Add(bs.BitPosition);
            playback_type = (indx_hdmv_playback_type)a;
            FieldPosition.Add(bs.BitPosition);
            permitted = bs.ReadIntFromBits(2);
            bs.SkipBit(12);
            FieldPosition.Add(bs.BitPosition);
            id_ref = (short)bs.ReadIntFromBits(16);
            bs.SkipBit(32);
            length = bs.Position - position;
        }
    }
    public class INDX_PLAY_ITEM : INDX_BASE
    {
        protected int permitted;
        private indx_object_type object_type;
        /*union {*/
        private INDX_BDJ_OBJ bdj;
        private INDX_HDMV_OBJ hdmv;
        #region Properties
        public string Permitted
        {
            get
            {
                switch (permitted)
                {
                    case 0x0: return "Permitted";
                    case 0x2: return "Prohibited 1";
                    case 0x1: return "Prohibited 2";
                    default: return "";
                }
            }
        }
        public string Object_Type
        {
            get
            {
                switch (object_type)
                {
                    case indx_object_type.indx_object_type_hdmv: return "HDMV";
                    case indx_object_type.indx_object_type_bdj: return "BDJ";
                    default: return "";
                }
            }
        }
        public INDX_BDJ_OBJ Bdj
        {
            get { return bdj; }
            set { bdj = value; }
        }
        public INDX_HDMV_OBJ Hdmv
        {
            get { return hdmv; }
            set { hdmv = value; }
        }
        #endregion
        /*};*/
        public INDX_PLAY_ITEM() { }
        public INDX_PLAY_ITEM(BitStreamReader bs)
        {
            position = bs.Position;
            int a = bs.ReadIntFromBits(2);
            FieldPosition.Add(bs.BitPosition);
            object_type = (indx_object_type)a;
            FieldPosition.Add(bs.BitPosition);
            permitted = bs.ReadIntFromBits(2);
            bs.SkipBit(28);
            if (object_type == indx_object_type.indx_object_type_hdmv)
            {
                hdmv = new INDX_HDMV_OBJ(bs);
            }
            else
            {
                bdj = new INDX_BDJ_OBJ(bs);
            }
            length = bs.Position - position;
        }
    }
    public class INDX_TITLE : INDX_BASE
    {
        private string name;// ???
        private indx_object_type object_type;
        private int access_type; // 2
        /*union {*/
        private INDX_BDJ_OBJ bdj;
        private INDX_HDMV_OBJ hdmv;
        /*};*/
        #region Properties
        public string Permitted
        {
            get
            {
                switch (access_type)
                {
                    case 0x0: return "Permitted";
                    case 0x2: return "Prohibited 1";
                    case 0x1: return "Prohibited 2";
                    default: return "";
                }
            }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Object_type
        {
            get
            {
                switch (object_type)
                {
                    case indx_object_type.indx_object_type_hdmv: return "HDMV";
                    case indx_object_type.indx_object_type_bdj: return "BDJ";
                    default: return "";
                }
            }
        }
        public INDX_BDJ_OBJ Bdj
        {
            get { return bdj; }
            set { bdj = value; }
        }
        public INDX_HDMV_OBJ Hdmv
        {
            get { return hdmv; }
            set { hdmv = value; }
        }
        #endregion
        public INDX_TITLE(BitStreamReader bs)
        {
            position = bs.Position;
            FieldPosition.Add(bs.BitPosition);
            object_type = (indx_object_type)bs.ReadIntFromBits(2);
            FieldPosition.Add(bs.BitPosition);
            access_type = bs.ReadIntFromBits(2);
            bs.SkipBit(28);
            if (object_type == indx_object_type.indx_object_type_hdmv)
            {
                hdmv = new INDX_HDMV_OBJ(bs);
            }
            else
            {
                bdj = new INDX_BDJ_OBJ(bs);
            }
            length = bs.Position - position;
        }
        public INDX_TITLE()
        {
        }
    } ;
    #endregion
    #region enums
    public enum indx_object_type
    {
        indx_object_type_hdmv = 1,
        indx_object_type_bdj = 2,
    }
    public enum indx_video_format
    {
        indx_video_format_ignored,
        indx_video_480i,
        indx_video_576i,
        indx_video_480p,
        indx_video_1080i,
        indx_video_720p,
        indx_video_1080p,
        indx_video_576p,
    }
    public enum indx_frame_rate
    {
        indx_fps_reserved1,
        indx_fps_23_976,
        indx_fps_24,
        indx_fps_25,
        indx_fps_29_97,
        indx_fps_reserved2,
        indx_fps_50,
        indx_fps_59_94,
    }
    public enum indx_hdmv_playback_type
    {
        indx_hdmv_playback_type_movie = 0,
        indx_hdmv_playback_type_interactive = 1,
    }
    public enum indx_bdj_playback_type
    {
        indx_bdj_playback_type_movie = 2,
        indx_bdj_playback_type_interactive = 3,
    }
    #endregion
}
