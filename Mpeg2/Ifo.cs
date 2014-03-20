using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;
using Utils;
using System.Diagnostics;
namespace VideoFiles
{
    public class VideoAttributes : LOCALIZED_DATA
    {
        #region
        private string name;
        private string coding_Mode;
        private string standard;
        private string aspect;
        private string resolution;
        private string movieType;
        #endregion
        #region Properties
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Coding_Mode
        {
            get { return coding_Mode; }
            set { coding_Mode = value; }
        }
        public string Standard
        {
            get { return standard; }
            set { standard = value; }
        }
        public string Aspect
        {
            get { return aspect; }
            set { aspect = value; }
        }
        public string Resolution
        {
            get { return resolution; }
            set { resolution = value; }
        }
        public string MovieType
        {
            get { return movieType; }
            set { movieType = value; }
        }
        #endregion
        public VideoAttributes(string N, byte[] b)
        {
            name = N;
            DecodeVideo(b);
        }
        public VideoAttributes(BitStreamReader sw)
        {
        }
        private void DecodeVideo(byte[] buf)
        {
            switch ((buf[0] & 0xc0) >> 6)
            {
                case 0: coding_Mode = "Mpeg-1";
                    break;
                case 1: coding_Mode = "Mpeg-2";
                    break;
            }
            switch ((buf[0] & 0x30) >> 4)
            {
                case 0: standard = "NTSC";
                    break;
                case 1: standard = "PAL";
                    break;
            }
            switch ((buf[0] & 0x0c) >> 2)
            {
                case 0: aspect = "4:3";
                    break;
                case 3: aspect = "16:9";
                    break;
            }
            switch (buf[1] & 0x30)
            {
                case 0: resolution = "720x576";
                    if (standard == "NTSC")
                        resolution = "720x480";
                    break;
                case 1: resolution = "704x576";
                    break;
            }
            if ((buf[1] & 0x01) == 1)
                movieType = "Film";
            else
                movieType = "Camera";
        }
        public override string ToString()
        {
            return name + " " + coding_Mode + " " + standard;
        }
    }
    public class AudioAttributes : LOCALIZED_DATA
    {
        private string name;
        private string coding_Mode;
        private string sample_rate;
        private string language_type;
        private string application_Mode;
        private string language_code;
        private string quantization;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Coding_Mode
        {
            get { return coding_Mode; }
            set { coding_Mode = value; }
        }
        public string Language_type
        {
            get { return language_type; }
            set { language_type = value; }
        }
        public string Application_Mode
        {
            get { return application_Mode; }
            set { application_Mode = value; }
        }
        public string Language_code
        {
            get { return language_code; }
            set { language_code = value; }
        }
        public string Sample_rate
        {
            get { return sample_rate; }
            set { sample_rate = value; }
        }
        public string Quantization
        {
            get { return quantization; }
            set { quantization = value; }
        }
        public AudioAttributes(string N, byte[] b)
        {
            name = N;
           // PositionOfStructureInFile
            DecodeAudio(b);
        }
        public AudioAttributes(BitStreamReader sw)
        {
        }
        private void DecodeAudio(byte[] buf)
        {
            #region coding mode
            switch ((buf[0] & 0xE) >> 1)
            {
                case 0:
                    coding_Mode = "AC3";
                    break;
                case 2:
                    coding_Mode = "Mpeg-1";
                    break;
                case 3: coding_Mode = "Mpeg-2ext";
                    break;
                case 4: coding_Mode = "LPCM";
                    break;
                case 6: coding_Mode = " DTS";
                    break;
            }
            #endregion
            if ((buf[0] & 0x10) == 0x10)
            {
            }
            bool lg = true;
            if (((buf[0] & 0x0C) >> 2) == 0)
                lg = false;
            if (coding_Mode == "LPCM")
            {
            }
            switch ((buf[1] & 0x30) >> 2) 
            { 
                case 0:
                sample_rate = "48 kbps";
                    break;
                case 1:
                sample_rate = "96 kbps";
                    break;
            }
            if (lg)
            {
                byte[] b = new byte[2];
                Buffer.BlockCopy(buf, 2, b, 0, 2);
                language_code = Encoding.Default.GetString(b);
            }
        }
    }
    public class VMG_IFO : LOCALIZED_DATA
    {
        #region VMG IFO
        #region Attributes
        ELEMENTARY_TYPE header;
        ELEMENTARY_TYPE last_sector_of_VMG_set;
        ELEMENTARY_TYPE last_sector_of_IFO;
        ELEMENTARY_TYPE versionnumber;
        ELEMENTARY_TYPE vmg_category;
        ELEMENTARY_TYPE number_of_volumes;
        ELEMENTARY_TYPE volume_number;
        ELEMENTARY_TYPE side_ID;
        ELEMENTARY_TYPE number_of_title_sets;
        ELEMENTARY_TYPE provider_ID;
        ELEMENTARY_TYPE vmg_POS;
        ELEMENTARY_TYPE end_byte_address_of_VMGI_MAT;
        ELEMENTARY_TYPE start_address_of_First_Play_program_chain;
        ELEMENTARY_TYPE start_sector_of_Menu_VOB;
        ELEMENTARY_TYPE sector_pointer_to_table_of_titles;
        ELEMENTARY_TYPE sector_pointer_to_Menu_Program_Chain_table;
        ELEMENTARY_TYPE sector_pointer_to_VMG_PTL_MAIT;
        ELEMENTARY_TYPE sector_pointer_to_VMG_VTS_ATRT;
        ELEMENTARY_TYPE sector_pointer_to_VMG_TXTDT_MG;
        ELEMENTARY_TYPE sector_pointer_to_VMGM_C_ADT;
        ELEMENTARY_TYPE sector_pointer_to_VMGM_VOBU_ADMAP;
        ELEMENTARY_TYPE video_attributes_of_VMGM_VOBS;
        ELEMENTARY_TYPE number_of_audio_streams_in_VMGM_VOBS;
        ELEMENTARY_TYPE audio_attributes_of_VMGM_VOBS;
        ELEMENTARY_TYPE number_of_subpicture_streams_in_VMGM_VOBS;
        ELEMENTARY_TYPE subpicture_attributes_of_VMGM_VOBS;
        ELEMENTARY_TYPE reserved;
        #endregion
        #region Properties
        public ELEMENTARY_TYPE Header
        {
            get { return header; }
            set { header = value; }
        }
        public ELEMENTARY_TYPE Last_sector_of_VMG_set
        {
            get { return last_sector_of_VMG_set; }
            set { last_sector_of_VMG_set = value; }
        }
        public ELEMENTARY_TYPE Last_sector_of_IFO
        {
            get { return last_sector_of_IFO; }
            set { last_sector_of_IFO = value; }
        }
        public ELEMENTARY_TYPE Versionnumber
        {
            get { return versionnumber; }
            set { versionnumber = value; }
        }
        public ELEMENTARY_TYPE Vmg_category
        {
            get { return vmg_category; }
            set { vmg_category = value; }
        }
        public ELEMENTARY_TYPE Number_of_volumes
        {
            get { return number_of_volumes; }
            set { number_of_volumes = value; }
        }
        public ELEMENTARY_TYPE Volume_number
        {
            get { return volume_number; }
            set { volume_number = value; }
        }
        public ELEMENTARY_TYPE Side_ID
        {
            get { return side_ID; }
            set { side_ID = value; }
        }
        public ELEMENTARY_TYPE Number_of_title_sets
        {
            get { return number_of_title_sets; }
            set { number_of_title_sets = value; }
        }
        public ELEMENTARY_TYPE Provider_ID
        {
            get { return provider_ID; }
            set { provider_ID = value; }
        }
        public ELEMENTARY_TYPE Vmg_POS
        {
            get { return vmg_POS; }
            set { vmg_POS = value; }
        }
        public ELEMENTARY_TYPE End_byte_address_of_VMGI_MAT
        {
            get { return end_byte_address_of_VMGI_MAT; }
            set { end_byte_address_of_VMGI_MAT = value; }
        }
        public ELEMENTARY_TYPE Start_address_of_First_Play_program_chain
        {
            get { return start_address_of_First_Play_program_chain; }
            set { start_address_of_First_Play_program_chain = value; }
        }
        public ELEMENTARY_TYPE Start_sector_of_Menu_VOB
        {
            get { return start_sector_of_Menu_VOB; }
            set { start_sector_of_Menu_VOB = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_table_of_titles
        {
            get { return sector_pointer_to_table_of_titles; }
            set { sector_pointer_to_table_of_titles = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_Menu_Program_Chain_table
        {
            get { return sector_pointer_to_Menu_Program_Chain_table; }
            set { sector_pointer_to_Menu_Program_Chain_table = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VMG_PTL_MAIT
        {
            get { return sector_pointer_to_VMG_PTL_MAIT; }
            set { sector_pointer_to_VMG_PTL_MAIT = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VMG_VTS_ATRT
        {
            get { return sector_pointer_to_VMG_VTS_ATRT; }
            set { sector_pointer_to_VMG_VTS_ATRT = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VMG_TXTDT_MG
        {
            get { return sector_pointer_to_VMG_TXTDT_MG; }
            set { sector_pointer_to_VMG_TXTDT_MG = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VMGM_C_ADT
        {
            get { return sector_pointer_to_VMGM_C_ADT; }
            set { sector_pointer_to_VMGM_C_ADT = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VMGM_VOBU_ADMAP
        {
            get { return sector_pointer_to_VMGM_VOBU_ADMAP; }
            set { sector_pointer_to_VMGM_VOBU_ADMAP = value; }
        }
        public ELEMENTARY_TYPE Video_attributes_of_VMGM_VOBS
        {
            get { return video_attributes_of_VMGM_VOBS; }
            set { video_attributes_of_VMGM_VOBS = value; }
        }
        public ELEMENTARY_TYPE Number_of_audio_streams_in_VMGM_VOBS
        {
            get { return number_of_audio_streams_in_VMGM_VOBS; }
            set { number_of_audio_streams_in_VMGM_VOBS = value; }
        }
        public ELEMENTARY_TYPE Audio_attributes_of_VMGM_VOBS
        {
            get { return audio_attributes_of_VMGM_VOBS; }
            set { audio_attributes_of_VMGM_VOBS = value; }
        }
        public ELEMENTARY_TYPE Number_of_subpicture_streams_in_VMGM_VOBS
        {
            get { return number_of_subpicture_streams_in_VMGM_VOBS; }
            set { number_of_subpicture_streams_in_VMGM_VOBS = value; }
        }
        public ELEMENTARY_TYPE Subpicture_attributes_of_VMGM_VOBS
        {
            get { return subpicture_attributes_of_VMGM_VOBS; }
            set { subpicture_attributes_of_VMGM_VOBS = value; }
        }
        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
        #endregion
        #endregion
        public VMG_IFO(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            header = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 12);
            last_sector_of_VMG_set = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sw.Position=0x1C;
            last_sector_of_IFO = new ELEMENTARY_TYPE(sw, 0, typeof(int)); 
            versionnumber = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            Vmg_category = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]),4);
            number_of_volumes = new ELEMENTARY_TYPE(sw, 0, typeof(short));;
            volume_number = new ELEMENTARY_TYPE(sw, 0, typeof(short));;
            side_ID = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]),1);
            sw.Position = 0x3E;
            number_of_title_sets = new ELEMENTARY_TYPE(sw, 0, typeof(short));;
            Provider_ID = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 32);
            sw.Position = 0x60;
            vmg_POS = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 8);
            sw.Position = 0x80;
            end_byte_address_of_VMGI_MAT = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            start_address_of_First_Play_program_chain = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            sw.Position = 0xC0;
            start_sector_of_Menu_VOB = new ELEMENTARY_TYPE(sw, 0, typeof(int));;
            sector_pointer_to_table_of_titles = new ELEMENTARY_TYPE(sw, 0, typeof(int));;
            sector_pointer_to_Menu_Program_Chain_table = new ELEMENTARY_TYPE(sw, 0, typeof(int));;
            sector_pointer_to_VMG_PTL_MAIT = new ELEMENTARY_TYPE(sw, 0, typeof(int));;
            sector_pointer_to_VMG_VTS_ATRT = new ELEMENTARY_TYPE(sw, 0, typeof(int));;
            sector_pointer_to_VMG_TXTDT_MG = new ELEMENTARY_TYPE(sw, 0, typeof(int));;
            sector_pointer_to_VMGM_C_ADT = new ELEMENTARY_TYPE(sw, 0, typeof(int));;
            sector_pointer_to_VMGM_VOBU_ADMAP = new ELEMENTARY_TYPE(sw, 0, typeof(int));;
            sw.Position = 0x100;
            #region video and audio attributes
            video_attributes_of_VMGM_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]),2);
            number_of_audio_streams_in_VMGM_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(short));;
            audio_attributes_of_VMGM_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]),8 * 8);
            #endregion
            sw.Position = 0x154; ;
            number_of_subpicture_streams_in_VMGM_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(short));;
            subpicture_attributes_of_VMGM_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]),6);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class TS_IFO : LOCALIZED_DATA
    {
        private ELEMENTARY_TYPE last_sector_of_title_set;
        private ELEMENTARY_TYPE last_sector_of_IFO_TS;
        private ELEMENTARY_TYPE vTS_category;
        #region TS IFO
        public ELEMENTARY_TYPE Header
        {
            get { return header; }
            set { header = value; }
        }
        public ELEMENTARY_TYPE Last_sector_of_title_set
        {
            get { return last_sector_of_title_set; }
            set { last_sector_of_title_set = value; }
        }
        public ELEMENTARY_TYPE Last_sector_of_IFO_TS
        {
            get { return last_sector_of_IFO_TS; }
            set { last_sector_of_IFO_TS = value; }
        }
        byte[] version_number_TS;
        public ELEMENTARY_TYPE VTS_category
        {
            get { return vTS_category; }
            set { vTS_category = value; }
        }
        private ELEMENTARY_TYPE end_byte_address_of_VTS_MAT;
        private ELEMENTARY_TYPE start_sector_of_Menu_Vob;
        private ELEMENTARY_TYPE sector_pointer_to_VTS_PTT_SRPT;
        private ELEMENTARY_TYPE sector_pointer_to_VTS_PGCI;
        private ELEMENTARY_TYPE sector_pointer_to_VTSM_PGCI_UT;
        private ELEMENTARY_TYPE sector_pointer_to_VTS_TMAPTI;
        private ELEMENTARY_TYPE sector_pointer_to_VTS_C_ADT;
        private ELEMENTARY_TYPE sector_pointer_to_VTS_VOBU_ADMAP;
        private ELEMENTARY_TYPE number_of_audio_streams_in_VTSM_VOBS;
        private ELEMENTARY_TYPE number_of_subpicture_streams_in_VTSM_VOBS;
        private ELEMENTARY_TYPE number_of_audio_streams_in_VTS_VOBS;
        private ELEMENTARY_TYPE number_of_subpicture_streams_in_VTS_VOBS;
        private ELEMENTARY_TYPE sector_pointer_to_VTSM_C_ADT;
        private ELEMENTARY_TYPE sector_pointer_to_VTSM_VOBU_ADMAP;
        private ELEMENTARY_TYPE header;
        public ELEMENTARY_TYPE End_byte_address_of_VTS_MAT
        {
            get { return end_byte_address_of_VTS_MAT; }
            set { end_byte_address_of_VTS_MAT = value; }
        }
        public ELEMENTARY_TYPE Start_sector_of_Menu_Vob
        {
            get { return start_sector_of_Menu_Vob; }
            set { start_sector_of_Menu_Vob = value; }
        }
        public ELEMENTARY_TYPE start_sector_of_Title_Vob;
        public ELEMENTARY_TYPE Sector_pointer_to_VTS_PTT_SRPT
        {
            get { return sector_pointer_to_VTS_PTT_SRPT; }
            set { sector_pointer_to_VTS_PTT_SRPT = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VTS_PGCI
        {
            get { return sector_pointer_to_VTS_PGCI; }
            set { sector_pointer_to_VTS_PGCI = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VTSM_PGCI_UT
        {
            get { return sector_pointer_to_VTSM_PGCI_UT; }
            set { sector_pointer_to_VTSM_PGCI_UT = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VTS_TMAPTI
        {
            get { return sector_pointer_to_VTS_TMAPTI; }
            set { sector_pointer_to_VTS_TMAPTI = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VTSM_C_ADT
        {
            get { return sector_pointer_to_VTSM_C_ADT; }
            set { sector_pointer_to_VTSM_C_ADT = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VTSM_VOBU_ADMAP
        {
            get { return sector_pointer_to_VTSM_VOBU_ADMAP; }
            set { sector_pointer_to_VTSM_VOBU_ADMAP = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VTS_C_ADT
        {
            get { return sector_pointer_to_VTS_C_ADT; }
            set { sector_pointer_to_VTS_C_ADT = value; }
        }
        public ELEMENTARY_TYPE Sector_pointer_to_VTS_VOBU_ADMAP
        {
            get { return sector_pointer_to_VTS_VOBU_ADMAP; }
            set { sector_pointer_to_VTS_VOBU_ADMAP = value; }
        }
        ELEMENTARY_TYPE video_attributes_of_VTSM_VOBS;

        public ELEMENTARY_TYPE Video_attributes_of_VTSM_VOBS
        {
            get { return video_attributes_of_VTSM_VOBS; }
            set { video_attributes_of_VTSM_VOBS = value; }
        }

        public ELEMENTARY_TYPE Number_of_audio_streams_in_VTSM_VOBS
        {
            get { return number_of_audio_streams_in_VTSM_VOBS; }
            set { number_of_audio_streams_in_VTSM_VOBS = value; }
        }
        private ELEMENTARY_TYPE audio_attributes_of_VTSM_VOBS;

        public ELEMENTARY_TYPE Audio_attributes_of_VTSM_VOBS
        {
            get { return audio_attributes_of_VTSM_VOBS; }
            set { audio_attributes_of_VTSM_VOBS = value; }
        }

        public ELEMENTARY_TYPE Number_of_subpicture_streams_in_VTSM_VOBS
        {
            get { return number_of_subpicture_streams_in_VTSM_VOBS; }
            set { number_of_subpicture_streams_in_VTSM_VOBS = value; }
        }
        byte[] subpicture_attributes_of_VTSM_VOBS;
        byte[] reserved_TS;
        byte[] video_attributes_of_VTS_VOBS;

        public byte[] Video_attributes_of_VTS_VOBS
        {
            get { return video_attributes_of_VTS_VOBS; }
            set { video_attributes_of_VTS_VOBS = value; }
        }

        public ELEMENTARY_TYPE Number_of_audio_streams_in_VTS_VOBS
        {
            get { return number_of_audio_streams_in_VTS_VOBS; }
            set { number_of_audio_streams_in_VTS_VOBS = value; }
        }
        ELEMENTARY_TYPE audio_attributes_of_VTS_VOBS;

        public ELEMENTARY_TYPE Audio_attributes_of_VTS_VOBS
        {
            get { return audio_attributes_of_VTS_VOBS; }
            set { audio_attributes_of_VTS_VOBS = value; }
        }

        public ELEMENTARY_TYPE Number_of_subpicture_streams_in_VTS_VOBS
        {
            get { return number_of_subpicture_streams_in_VTS_VOBS; }
            set { number_of_subpicture_streams_in_VTS_VOBS = value; }
        }
        ELEMENTARY_TYPE subpicture_attributes_of_VTS_VOBS;

        public ELEMENTARY_TYPE Subpicture_attributes_of_VTS_VOBS
        {
            get { return subpicture_attributes_of_VTS_VOBS; }
            set { subpicture_attributes_of_VTS_VOBS = value; }
        }
        ELEMENTARY_TYPE multichannel_extension;

        public ELEMENTARY_TYPE Multichannel_extension
        {
            get { return multichannel_extension; }
            set { multichannel_extension = value; }
        }
        #endregion

        public TS_IFO(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            header = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 12);
            last_sector_of_title_set = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sw.Position = 0x1C;
            last_sector_of_IFO_TS = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sw.Position = 0x20;
            version_number_TS = sw.ReadBytes(2);
            VTS_category = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            sw.Position = 0x80;
            end_byte_address_of_VTS_MAT = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sw.Position = 0xC0;
            // nbInstructions sector 0x800
            start_sector_of_Menu_Vob = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            start_sector_of_Title_Vob = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sector_pointer_to_VTS_PTT_SRPT = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sector_pointer_to_VTS_PGCI = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sector_pointer_to_VTSM_PGCI_UT = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sector_pointer_to_VTS_TMAPTI = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sector_pointer_to_VTSM_C_ADT = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sector_pointer_to_VTSM_VOBU_ADMAP = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sector_pointer_to_VTS_C_ADT = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sector_pointer_to_VTS_VOBU_ADMAP = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sw.Position = 0x100;
            video_attributes_of_VTSM_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            number_of_audio_streams_in_VTSM_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            audio_attributes_of_VTSM_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 8 * 8);

                  sw.Position = 0x154;
                  number_of_subpicture_streams_in_VTSM_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                  subpicture_attributes_of_VTSM_VOBS = sw.ReadBytes(6);
                    
                  sw.Position = 0x200;

                  video_attributes_of_VTS_VOBS = sw.ReadBytes(2);

                  number_of_audio_streams_in_VTS_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                  audio_attributes_of_VTS_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 8 * 8);
                 
                  sw.Position = 0x254;
                  number_of_subpicture_streams_in_VTS_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                  subpicture_attributes_of_VTS_VOBS = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 32 * 6);

                  sw.Position = 0x318;
                  multichannel_extension = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 8 * 24);
 //                 version = ((version_number_TS[1] & 0xF0) >> 4).ToString() + "." + (version_number_TS[1] & 0x0F).ToString();

            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class TT_SRPT : LOCALIZED_DATA
    {
        byte[] data;
        private ELEMENTARY_TYPE  number_of_angles;
        private ELEMENTARY_TYPE  number_of_chapters;
        private ELEMENTARY_TYPE title_type;
        private ELEMENTARY_TYPE parental_management_mask;
        private ELEMENTARY_TYPE video_Title_Set_number;
        private ELEMENTARY_TYPE title_number_within_VTS;
        private ELEMENTARY_TYPE start_sector;
        public string Type
        {
            get
            {
                string s = ((byte)title_type.Value).ToString("x")+ " : ";
                int a = (((byte)title_type.Value) & (byte)0x40)/(256*16);
                if (a == 1)
                {
                    s += " not sequential,";
                }
                a = (((byte)title_type.Value) & (byte)0x3C) / 4;
                s += " Jump/Link/Call commands";
                switch (a)
                {
                    case 0x0:
                        s += " none";
                        break;
                    case 0x0001:
                        s += " invalid";
                        break;
                    case 0x3:
                        s += " only in button";
                        break;
                    case 0x5:
                        s += "  only in pre/post";
                        break;
                    case 0x7:
                        s += "  in button and pre/post";
                        break;
                    case 0x9:
                        s += "  only in cell";
                        break;
                    case 0xb:
                        s += "  in cell and button";
                        break;
                    case 0xd:
                        s += "  in cell and pre/post";
                        break;
                    case 0xf:
                        s += " in all places";
                        break;
                }
                return s;
            }
            set { object x = value; }
        }
        public ELEMENTARY_TYPE Title_type
        {
            get { return title_type; }
            set { title_type = value; }
        } 
        public ELEMENTARY_TYPE  Number_of_angles
        {
            get { return number_of_angles; }
            set { number_of_angles = value; }
        }
        public ELEMENTARY_TYPE  Number_of_chapters
        {
            get { return number_of_chapters; }
            set { number_of_chapters = value; }
        }
        public ELEMENTARY_TYPE Parental_management_mask
        {
            get { return parental_management_mask; }
            set { parental_management_mask = value; }
        } 
        public ELEMENTARY_TYPE Video_Title_Set_number
        {
            get { return video_Title_Set_number; }
            set { video_Title_Set_number = value; }
        }
        public ELEMENTARY_TYPE Title_number_within_VTS
        {
            get { return title_number_within_VTS; }
            set { title_number_within_VTS = value; }
        }
        public ELEMENTARY_TYPE Start_sector
        {
            get { return start_sector; }
            set { start_sector = value; }
        }
        public TT_SRPT(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            title_type =  new ELEMENTARY_TYPE(sw,0,typeof(byte));// sw.ReadByte();
            number_of_angles = new ELEMENTARY_TYPE(sw,0,typeof(byte));
            number_of_chapters = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            parental_management_mask = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            video_Title_Set_number = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            title_number_within_VTS = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            start_sector = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
        public override string ToString()
        {
            return "Title structure";
        }
    }
    public class VMGM_LU : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE numberProgramChains;
        ELEMENTARY_TYPE pgc_Category;
        ELEMENTARY_TYPE offset;
        List<VMGM_PGC> vmgm_pgc = new List<VMGM_PGC>();
        ELEMENTARY_TYPE end_address;

        public ELEMENTARY_TYPE NumberProgramChains
        {
            get { return numberProgramChains; }
            set { numberProgramChains = value; }
        }
        public ELEMENTARY_TYPE End_address
        {
            get { return end_address; }
            set { end_address = value; }
        }
        public ELEMENTARY_TYPE PGC_Category
        {
            get { return pgc_Category; }
            set { pgc_Category = value; }
        }
        public ELEMENTARY_TYPE Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public List<VMGM_PGC> Vmgm_pgc
        {
            get { return vmgm_pgc; }
            set { vmgm_pgc = value; }
        }

        public VMGM_LU(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            numberProgramChains = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            sw.Position += 2;
            end_address = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            PGC_Category = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sw.Position = (int)PositionOfStructureInFile + (int)offset.Value;
            for (int i = 0; i < (short)numberProgramChains.Value; i++)
            {
                VMGM_PGC v = new VMGM_PGC(sw);
                vmgm_pgc.Add(v);
                if (sw.Position > (int)PositionOfStructureInFile + (int)end_address.Value)
                    break;
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class VMGM_PGCI_UT : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE numberLanguageUnits;
        ELEMENTARY_TYPE isoLanguage;
        ELEMENTARY_TYPE offset;
        List<VMGM_LU> vmgm_pgc = new List<VMGM_LU>();
        ELEMENTARY_TYPE end_address;

        ELEMENTARY_TYPE has_menu;

        public ELEMENTARY_TYPE NumberLanguageUnits
        {
            get { return numberLanguageUnits; }
            set { numberLanguageUnits = value; }
        }
        public ELEMENTARY_TYPE End_address
        {
            get { return end_address; }
            set { end_address = value; }
        }

        public ELEMENTARY_TYPE IsoLanguage
        {
            get { return isoLanguage; }
            set { isoLanguage = value; }
        }
        public ELEMENTARY_TYPE Has_menu
        {
            get { return has_menu; }
            set { has_menu = value; }
        }
        public ELEMENTARY_TYPE Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public List<VMGM_LU> Vmgm_LU
        {
            get { return vmgm_pgc; }
            set { vmgm_pgc = value; }
        }
        public VMGM_PGCI_UT(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            numberLanguageUnits = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            sw.Position += 2;
            end_address = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            IsoLanguage = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 2);
            sw.Position += 1;
            has_menu = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            sw.Position = (int)PositionOfStructureInFile + (int)offset.Value;
            for (int i = 0; i < (short)numberLanguageUnits.Value; i++)
            {
                VMGM_LU v = new VMGM_LU(sw);
                vmgm_pgc.Add(v);
                if (sw.Position > (int)PositionOfStructureInFile + (int)end_address.Value)
                    break;
            }

            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class PGC_AST_CTL : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE b;

        public ELEMENTARY_TYPE B
        {
            get { return b; }
            set { b = value; }
        }
        public PGC_AST_CTL(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            b = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            sw.Position++;
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class PGC_SPST_CTL : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE b;
        public ELEMENTARY_TYPE B
        {
            get { return b; }
            set { b = value; }
        }
        public PGC_SPST_CTL(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            b = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]),4);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class VMGM_PGC : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE playback;
        ELEMENTARY_TYPE number_Programs;
        ELEMENTARY_TYPE number_Cells;
        List<PGC_AST_CTL> pgc_ast_ctrl = new List<PGC_AST_CTL>();
        List<PGC_SPST_CTL> pgc_spst_ctrl = new List<PGC_SPST_CTL>();
        ELEMENTARY_TYPE palettes;
        ELEMENTARY_TYPE offsetToCommands;
        ELEMENTARY_TYPE offsetToProgramMap;
        ELEMENTARY_TYPE offsetToCellPlayback;
        ELEMENTARY_TYPE offsetToCellPosition;

        public ELEMENTARY_TYPE Number_Programs
        {
            get { return number_Programs; }
            set { number_Programs = value; }
        }
        public ELEMENTARY_TYPE Number_Cells
        {
            get { return number_Cells; }
            set { number_Cells = value; }
        }
        public ELEMENTARY_TYPE Playback
        {
            get { return playback; }
            set { playback = value; }
        }
        public List<PGC_AST_CTL> Pgc_ast_ctrl
        {
            get { return pgc_ast_ctrl; }
            set { pgc_ast_ctrl = value; }
        }
        public List<PGC_SPST_CTL> Pgc_spst_ctrl
        {
            get { return pgc_spst_ctrl; }
            set { pgc_spst_ctrl = value; }
        }
        public ELEMENTARY_TYPE Palettes
        {
            get { return palettes; }
            set { palettes = value; }
        }

        public ELEMENTARY_TYPE OffsetToCommands
        {
            get { return offsetToCommands; }
            set { offsetToCommands = value; }
        }
        public ELEMENTARY_TYPE OffsetToProgramMap
        {
            get { return offsetToProgramMap; }
            set { offsetToProgramMap = value; }
        }
        public ELEMENTARY_TYPE OffsetToCellPlayback
        {
            get { return offsetToCellPlayback; }
            set { offsetToCellPlayback = value; }
        }
        public ELEMENTARY_TYPE OffsetToCellPosition
        {
            get { return offsetToCellPosition; }
            set { offsetToCellPosition = value; }
        }
        ELEMENTARY_TYPE nb_pre_commands;
        ELEMENTARY_TYPE nb_post_commands;
        ELEMENTARY_TYPE nb_cell_commands;

        public ELEMENTARY_TYPE Nb_pre_commands
        {
            get { return nb_pre_commands; }
            set { nb_pre_commands = value; }
        }
        public ELEMENTARY_TYPE Nb_post_commands
        {
            get { return nb_post_commands; }
            set { nb_post_commands = value; }
        }
        public ELEMENTARY_TYPE Nb_cell_commands
        {
            get { return nb_cell_commands; }
            set { nb_cell_commands = value; }
        }
        ELEMENTARY_TYPE end_adress;

        public ELEMENTARY_TYPE End_adress
        {
            get { return end_adress; }
            set { end_adress = value; }
        }
        ELEMENTARY_TYPE pre_commands;

        public ELEMENTARY_TYPE Pre_commands
        {
            get { return pre_commands; }
            set { pre_commands = value; }
        }
        ELEMENTARY_TYPE post_commands;

        public ELEMENTARY_TYPE Post_commands
        {
            get { return post_commands; }
            set { post_commands = value; }
        }
        ELEMENTARY_TYPE cell_commands;

        public ELEMENTARY_TYPE Cell_commands
        {
            get { return cell_commands; }
            set { cell_commands = value; }
        }
        public VMGM_PGC(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            sw.Position += 2;
            Number_Programs = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            Number_Cells = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            Playback = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 4);
            sw.Position += 4;
            for (int i = 0; i < 8; i++)
                pgc_ast_ctrl.Add(new PGC_AST_CTL(sw));
            for (int i = 0; i < 32; i++)
                pgc_spst_ctrl.Add(new PGC_SPST_CTL(sw));
            sw.Position += 8;
            palettes = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 4 * 16);
            offsetToCommands = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            offsetToProgramMap = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            OffsetToCellPlayback = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            OffsetToCellPosition = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            if((short)offsetToCommands.Value!=0)
            {
                sw.Position = (int)PositionOfStructureInFile + (short)offsetToCommands.Value;
                nb_pre_commands = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                nb_post_commands = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                nb_cell_commands = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                end_adress = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                pre_commands = new ELEMENTARY_TYPE(sw,0,typeof(byte[]),(short)nb_pre_commands.Value * 8) ;
                 post_commands = new ELEMENTARY_TYPE(sw,0,typeof(byte[]),(short)nb_post_commands.Value * 8);
                 cell_commands = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (short)nb_cell_commands.Value * 8);
               sw.Position += (int)PositionOfStructureInFile + (short)end_adress.Value;
            }
            if ((short)offsetToProgramMap.Value != 0)
            {
            }
            if ((short)offsetToCellPlayback.Value != 0)
            {
            }
            if ((short)offsetToCellPosition.Value != 0)
            {
            }
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }

    public class  VMGM_C : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE cellId;
        ELEMENTARY_TYPE vobId;
        ELEMENTARY_TYPE startSector;
        ELEMENTARY_TYPE endSector;
        public ELEMENTARY_TYPE VobId
        {
            get { return vobId; }
            set { vobId = value; }
        }
        public ELEMENTARY_TYPE CellId
        {
            get { return cellId; }
            set { cellId = value; }
        }
        public ELEMENTARY_TYPE StartSector
        {
            get { return startSector; }
            set { startSector = value; }
        }
        public ELEMENTARY_TYPE EndSector
        {
            get { return endSector; }
            set { endSector = value; }
        }

        public VMGM_C(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            VobId = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            cellId = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            sw.Position++;
            startSector = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            endSector = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class VMGM_C_ADT : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE number_VOB_ID;
        List<VMGM_C> vmgn_c = new List<VMGM_C>();

        public ELEMENTARY_TYPE Number_VOB_ID
        {
            get { return number_VOB_ID; }
            set { number_VOB_ID = value; }
        }
        public List<VMGM_C> VMGM_C
        {
            get { return vmgn_c; }
            set { vmgn_c = value; }
        }
        public  VMGM_C_ADT(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            number_VOB_ID = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            sw.Position += 6;
            for (int i = 0; i < (short) number_VOB_ID.Value;i++ )
            {
                VMGM_C v = new VMGM_C(sw);
                vmgn_c.Add(v);
            }
             LengthInFile = sw.Position - PositionOfStructureInFile;

        }
    }
    public class IFO: LOCALIZED_DATA
    {
        #region Attributes
        //     the region code is stored in the file "VIDEO_TS.IFO" (table "VMGM_MAT"), byte offsets 34 and 35.[12]
        private byte regionCode;
        byte[] version_number;
        BitStreamReader sw;
        TS_IFO ts_ifo;
        VMG_IFO vmg;
        VMGM_PGCI_UT vmgpgci;
        VMGM_C_ADT vmgm_c_adt;
        VMGM_PGC first_pgc;
        #region video info
        private List<VideoAttributes> video = new List<VideoAttributes>();
        #endregion
        #region audio info
        private List<AudioAttributes> audio = new List<AudioAttributes>();
        #endregion
        private string version;
        private List<TT_SRPT> titles = new List<TT_SRPT>();
        #endregion
 
        #region Properties
        public VMG_IFO VMG_IFO
        {
            get { return vmg; }
            set { vmg = value; }
        }
        public TS_IFO TS_IFO
        {
            get { return ts_ifo; }
            set { ts_ifo = value; }
        }
        public VMGM_PGCI_UT VMG_PGCI
        {
            get { return vmgpgci; }
            set { vmgpgci = value; }
        }
        public VMGM_C_ADT VMGM_C_ADT
        {
            get { return vmgm_c_adt; }
            set { vmgm_c_adt = value; }
        }
        public VMGM_PGC First_pgc
        {
            get { return first_pgc; }
            set { first_pgc = value; }
        }
        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        public List<VideoAttributes> Video
        {
            get { return video; }
            set { video = value; }
        }
        public List<AudioAttributes> Audio
        {
            get
            {
                return audio;
            }
            set { audio = value; }
        }
        public List<TT_SRPT> Titles
        {
            get { return titles; }
            set { titles = value; }
        }
        public List<string> RegionCode
        {
            get
            {
                string[] reg = new string[]{ "United States, Canada, Bermuda, U.S. territories",
                        "Europe (except Russia, Ukraine, and Belarus), Middle East, Egypt, Japan, South Africa, Swaziland, Lesotho, Greenland, French Overseas departments and territories",
                        "Southeast Asia, South Korea, Republic of China (Taiwan), Hong Kong, Macau", 
                        "Mexico, Central America, Caribbean, South America, Australia, Oceania",
                        "India, Afghanistan, Sri Lanka, Ukraine, Belarus, Russia, Africa (except Egypt, South Africa, Swaziland, and Lesotho), Central and South Asia, Mongolia, North Korea",
                        "People's Republic of China",
                        "Reserved for future use (found in use on protected screener copies of MPAA-related DVDs and media copies of pre-releases in Asia)",
                        "International venues such as aircraft, cruise ships, etc.","ALL"};

                List<string> regions = new List<string>();
                if (regionCode == 0) regions.Add("worldwide");
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (((regionCode >> i) & 0x01) == 0x00)
                            regions.Add(reg[i]);
                    }
                }
                return regions;
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Filename"></param>
        public IFO(string Filename)
        {
            //http://dvd.sourceforge.net/dvdinfo/ifo.html 
            sw = new BitStreamReader(Filename, true);
            ELEMENTARY_TYPE header = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 12);
            sw.Position = 0;
            switch ((string)header.Value)
            {
                case "DVDVIDEO-VMG_IFO"://in VIDEO_TS.IFO
                case "DVDVIDEO-VMG": 
                    #region
                    vmg = new VMG_IFO(sw);
                    version = ((((byte[])vmg.Versionnumber.Value)[1] & 0xF0) >> 4).ToString() + "." + ((((byte[])vmg.Versionnumber.Value)[1] & 0x0F).ToString());
                    regionCode = ((byte[])vmg.Vmg_category.Value)[1];
                    video.Add(new VideoAttributes("VMGM_VOBS", (byte[])vmg.Video_attributes_of_VMGM_VOBS.Value));
                    AddAudio((short)vmg.Number_of_audio_streams_in_VMGM_VOBS.Value, (byte[])vmg.Audio_attributes_of_VMGM_VOBS.Value, "VMGM_VOBS");
                    ParseTableOfTitles();
                    sw.Position = (int)vmg.Start_address_of_First_Play_program_chain.Value;
                    First_pgc = new VMGM_PGC(sw);
                    ReadMenu(((int)vmg.Sector_pointer_to_Menu_Program_Chain_table.Value));
                    //   the region code is stored in the file "VIDEO_TS.IFO" (table "VMGM_MAT"), byte offsets 34 and 35.[12]
                    sw.Position = (int)vmg.Sector_pointer_to_VMGM_C_ADT.Value * 0x800;
                    vmgm_c_adt = new VMGM_C_ADT(sw);
                    #endregion
                    break;
                case "DVDVIDEO-VTS":
                    ts_ifo = new TS_IFO(sw);
                    video.Add(new VideoAttributes("VTSM_VOBS",(byte[]) ts_ifo.Video_attributes_of_VTSM_VOBS.Value));
                    video.Add(new VideoAttributes("VTS_VOBS", (byte[])ts_ifo.Video_attributes_of_VTS_VOBS));
                    AddAudio((short)ts_ifo.Number_of_audio_streams_in_VTSM_VOBS.Value, (byte[])ts_ifo.Audio_attributes_of_VTSM_VOBS.Value, "VTSM_VOBS");
                    AddAudio((short)ts_ifo.Number_of_audio_streams_in_VTS_VOBS.Value, (byte[])ts_ifo.Audio_attributes_of_VTS_VOBS.Value, "VTS_VOBS");
                    ReadMenu(((int)ts_ifo.Sector_pointer_to_VTSM_PGCI_UT.Value));
                break;
            }
        }
        private void ParseTableOfTitles()
        {
            sw.Position = (int)VMG_IFO.Sector_pointer_to_table_of_titles.Value * 0x800;
            int nbTitles = sw.ReadByte() * 256 + sw.ReadByte();// BufferConvert.ByteToInteger(b);
            sw.Position += 6;
            for (int i = 0; i < nbTitles; i++)
            {
                TT_SRPT ts = new TT_SRPT(sw);
                titles.Add(ts);
            }
        }
        private void ReadMenu(int x)
        {
            sw.Position = x*0x800;
            vmgpgci = new VMGM_PGCI_UT(sw);
        }
        private void ReadTitlesAndChapters()
        {
            sw.Position = ((int)ts_ifo.Sector_pointer_to_VTS_PTT_SRPT.Value) * 0x800;
        }  
        private byte[] ReadSector(int sector_pointer)
        {
            // Ifo sector is 0x800 bytes
            sw.Position = sector_pointer * 0x800;
            return sw.ReadBytes(0x800);
        }
        private void AddAudio(int number_of_audio_streams, byte[] audio_attributes, string name)
        {
            for (int i = 0; i < number_of_audio_streams; i++)
            {
                byte[] b = new byte[8];
                if (i * 8 < audio_attributes.Length)
                {
                    Buffer.BlockCopy(audio_attributes, i * 8, b, 0, 8);
                    audio.Add(new AudioAttributes(name, b));
                }
            }
        }
     }
}
