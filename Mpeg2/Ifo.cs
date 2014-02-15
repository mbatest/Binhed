using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;
using Utils;
using System.Diagnostics;
namespace Mpeg2Files
{
    public class VideoAttributes : LOCALIZED_DATA
    {
        private string name;
        private string coding_Mode;
        private string standard;
        private string aspect;
        private string resolution;
        private string movieType;

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
        public VideoAttributes(string N, byte[] b)
        {
            name = N;
            DecodeVideo(b);
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
    public class AudioAtributes : LOCALIZED_DATA
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
        public AudioAtributes(string N, byte[] b)
        {
            name = N;
            DecodeAudio(b);
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
    public class TitleStructure : LOCALIZED_DATA
    {
        byte[] data;
        private int number_of_angles;
        private int number_of_chapters;
        private byte title_type;

        public byte Title_type
        {
            get { return title_type; }
            set { title_type = value; }
        }
 
        public int Number_of_angles
        {
            get { return number_of_angles; }
            set { number_of_angles = value; }
        }
 
        public int Number_of_chapters
        {
            get { return number_of_chapters; }
            set { number_of_chapters = value; }
        }
        public byte[] parental_management_mask;
        public int Video_Title_Set_number;
        public int title_number_within_VTS;
        public long start_sector;
        public TitleStructure(byte[] b)
        {
            data = b;
            title_type = b[0];
            number_of_angles = b[1];
            number_of_chapters = b[2] * 256 + b[3];

            Video_Title_Set_number = b[6];
            title_number_within_VTS = b[7];
            start_sector = (((b[8] * 256) + b[9]) * 256 + b[10]) * 256 + b[11];
        }
        public override string ToString()
        {
            return Video_Title_Set_number.ToString() + " " + title_number_within_VTS.ToString() + " " + number_of_chapters.ToString() + " " + start_sector.ToString();
        }
    }
    public class Ifo: LOCALIZED_DATA
    {
        public string Header
        {
            get { return header; }
            set { header = value; }
        }
        public List<VideoAttributes> Video
        {
            get { return video; }
            set { video = value; }
        }
        public List<AudioAtributes> Audio
        {
            get
            {
                return audio;
            }
            set { audio = value; }
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
                     for(int i = 0; i<7;i++)
                     {
                         if (((regionCode >> i) & 0x01) == 0x00)
                             regions.Add(reg[i]);
                     }
                 }
                 return regions;
             }
         }
  //     the region code is stored in the file "VIDEO_TS.IFO" (table "VMGM_MAT"), byte offsets 34 and 35.[12]
        private byte regionCode;
        private string header;
        BitStreamReader sw;

        public string version;
        public List<TitleStructure> titles = new List<TitleStructure>();
        byte[] version_number;
        #region video info
        private List<VideoAttributes> video = new List<VideoAttributes>();
        #endregion
        #region audio info
        private List<AudioAtributes> audio = new List<AudioAtributes>();
        #endregion
        #region VMG IFO
        int last_sector_of_VMG_set;
        int last_sector_of_IFO;
        byte[] VMG_category;
        int number_of_volumes;
        int volume_number;
        int side_ID;
        int number_of_title_sets;
        public string Provider_ID;
        byte[] VMG_POS;
        int end_byte_address_of_VMGI_MAT;
        int start_address_of_First_Play_program_chain;
        int start_sector_of_Menu_VOB;
        int sector_pointer_to_table_of_titles;
        int sector_pointer_to_Menu_Program_Chain_table;
        int sector_pointer_to_VMG_PTL_MAIT;
        int sector_pointer_to_VMG_VTS_ATRT;
        int sector_pointer_to_VMG_TXTDT_MG;
        int sector_pointer_to_VMGM_C_ADT;
        int sector_pointer_to_VMGM_VOBU_ADMAP;
        byte[] video_attributes_of_VMGM_VOBS;
        int number_of_audio_streams_in_VMGM_VOBS;
        byte[] audio_attributes_of_VMGM_VOBS;
        int number_of_subpicture_streams_in_VMGM_VOBS;
        byte[] subpicture_attributes_of_VMGM_VOBS;
        byte[] reserved;
        #endregion
        #region TS IFO
        int DVDVIDEO_VTS;
        int last_sector_of_title_set;
        int last_sector_of_IFO_TS;
        byte[] version_number_TS;
        int VTS_category;
        int end_byte_address_of_VTS_MAT;
        int start_sector_of_Menu_Vob;
        int start_sector_of_Title_Vob;
        int sector_pointer_to_VTS_PTT_SRPT;
        int sector_pointer_to_VTS_PGCI;
        int sector_pointer_to_VTSM_PGCI_UT;
        int sector_pointer_to_VTS_TMAPTI;
        int sector_pointer_to_VTSM_C_ADT;
        byte[] sector_pointer_to_VTSM_VOBU_ADMAP;
        int sector_pointer_to_VTS_C_ADT;
        int sector_pointer_to_VTS_VOBU_ADMAP;
        byte[] video_attributes_of_VTSM_VOBS;
        int number_of_audio_streams_in_VTSM_VOBS;
        byte[] audio_attributes_of_VTSM_VOBS;
        int number_of_subpicture_streams_in_VTSM_VOBS;
        byte[] subpicture_attributes_of_VTSM_VOBS;
        byte[] reserved_TS;
        byte[] video_attributes_of_VTS_VOBS;
        int number_of_audio_streams_in_VTS_VOBS;
        byte[] audio_attributes_of_VTS_VOBS;
        int number_of_subpicture_streams_in_VTS_VOBS;
        byte[] subpicture_attributes_of_VTS_VOBS;
        byte[] multichannel_extension;
        #endregion
        public Ifo(string Filename)
        {
            sw = new BitStreamReader(Filename, false);
            byte[] buffer = sw.ReadBytes(12);
            header = Encoding.Default.GetString(buffer);
            switch (header)
            {
                case "DVDVIDEO-VMG":
                    last_sector_of_VMG_set = sw.ReadInteger();
                    sw.ReadBytes(0x0C);
                    last_sector_of_IFO = sw.ReadInteger();
                    version_number = sw.ReadBytes(2);

                    VMG_category = sw.ReadBytes(4);
                    regionCode = VMG_category[1];
                    number_of_volumes = sw.ReadShort();
                    volume_number = sw.ReadShort();
                    side_ID = sw.ReadByte();
                    sw.Position= 0x3E;
                    number_of_title_sets = sw.ReadShort();
                    sw.Position = 0x40;
                    Provider_ID = sw.ReadString(32);
                    VMG_POS = sw.ReadBytes(8);
                    sw.Position = 0x80;
                    end_byte_address_of_VMGI_MAT = sw.ReadInteger();
                    start_address_of_First_Play_program_chain = sw.ReadInteger();
                    sw.Position = 0xC0;
                    start_sector_of_Menu_VOB = sw.ReadInteger();
                    sector_pointer_to_table_of_titles = sw.ReadInteger();
                    sector_pointer_to_Menu_Program_Chain_table = sw.ReadInteger();
                    sector_pointer_to_VMG_PTL_MAIT = sw.ReadInteger();
                    sector_pointer_to_VMG_VTS_ATRT = sw.ReadInteger();
                    sector_pointer_to_VMG_TXTDT_MG = sw.ReadInteger();
                    sector_pointer_to_VMGM_C_ADT = sw.ReadInteger();
                    sector_pointer_to_VMGM_VOBU_ADMAP = sw.ReadInteger();
                    sw.Position = 0x100;
                    #region video and audio attributes
                    video_attributes_of_VMGM_VOBS = sw.ReadBytes(2);
                    number_of_audio_streams_in_VMGM_VOBS = sw.ReadShort();
                    audio_attributes_of_VMGM_VOBS = sw.ReadBytes(8 * 8);
                    #endregion
                    sw.Position =0x154;;
                    number_of_subpicture_streams_in_VMGM_VOBS = sw.ReadShort();
                    subpicture_attributes_of_VMGM_VOBS = sw.ReadBytes(6);
                    version = ((version_number[1] & 0xF0) >> 4).ToString() + "." + (version_number[1] & 0x0F).ToString();
                    video.Add(new VideoAttributes("VMGM_VOBS", video_attributes_of_VMGM_VOBS));
                    AddAudio(number_of_audio_streams_in_VMGM_VOBS, audio_attributes_of_VMGM_VOBS, "VMGM_VOBS");
                    sw.Position = start_address_of_First_Play_program_chain;
                //    byte[] First_Play = sw.ReadBytes(end_byte_address_of_VMGI_MAT - start_address_of_First_Play_program_chain);
               //     ParseTableOfTitles();
                //    ReadMenu();
                 //   byte[] VMGM_C_ADT = ReadSector(sector_pointer_to_VMGM_C_ADT);
                 //   the region code is stored in the file "VIDEO_TS.IFO" (table "VMGM_MAT"), byte offsets 34 and 35.[12]
                    break;
                case "DVDVIDEO-VTS":
                    last_sector_of_title_set = sw.ReadInteger();
                    sw.Position = 0x1C;
                    last_sector_of_IFO_TS = sw.ReadInteger();
                    version_number_TS = sw.ReadBytes(2);
                    VTS_category = sw.ReadInteger();
                    sw.Position = 0x80;
                    end_byte_address_of_VTS_MAT = sw.ReadInteger();
                    sw.Position = 0xC0;
                    // nbInstructions sector 0x800
                    start_sector_of_Menu_Vob = sw.ReadInteger();
                    start_sector_of_Title_Vob = sw.ReadInteger();
                    sector_pointer_to_VTS_PTT_SRPT = sw.ReadInteger();
                    sector_pointer_to_VTS_PGCI = sw.ReadInteger();
                    sector_pointer_to_VTSM_PGCI_UT = sw.ReadInteger();
                    sector_pointer_to_VTS_TMAPTI = sw.ReadInteger();
                    sector_pointer_to_VTSM_C_ADT = sw.ReadInteger();
                    sector_pointer_to_VTSM_VOBU_ADMAP = sw.ReadBytes(4);
                    sector_pointer_to_VTS_C_ADT = sw.ReadInteger();
                    sector_pointer_to_VTS_VOBU_ADMAP = sw.ReadInteger();
                    sw.Position = 0x100;
                    video_attributes_of_VTSM_VOBS = sw.ReadBytes(2);
                    number_of_audio_streams_in_VTSM_VOBS = sw.ReadShort();
                    audio_attributes_of_VTSM_VOBS = sw.ReadBytes(8 * 8);
                    video.Add(new VideoAttributes("VTSM_VOBS", video_attributes_of_VTSM_VOBS));
                    AddAudio(number_of_audio_streams_in_VTSM_VOBS, audio_attributes_of_VTSM_VOBS, "VTSM_VOBS");
                    sw.Position = 0x154;
                    number_of_subpicture_streams_in_VTSM_VOBS = sw.ReadShort();
                    subpicture_attributes_of_VTSM_VOBS = sw.ReadBytes(6);
                    
                    sw.Position = 0x200;

                    video_attributes_of_VTS_VOBS = sw.ReadBytes(2);
                    video.Add(new VideoAttributes("VTS_VOBS", video_attributes_of_VTS_VOBS));
                    number_of_audio_streams_in_VTS_VOBS = sw.ReadShort();
                    audio_attributes_of_VTS_VOBS = sw.ReadBytes(8 * 8);
                    AddAudio(number_of_audio_streams_in_VTS_VOBS, audio_attributes_of_VTS_VOBS, "VTS_VOBS");

                    sw.Position = 0x200;
                    number_of_subpicture_streams_in_VTS_VOBS = sw.ReadShort();
                    subpicture_attributes_of_VTS_VOBS = sw.ReadBytes(32 * 6);

                    sw.Position = 0x318;
                    multichannel_extension = sw.ReadBytes(8 * 24);
                    version = ((version_number_TS[1] & 0xF0) >> 4).ToString() + "." + (version_number_TS[1] & 0x0F).ToString();
                    break;
            }
        }
        private void ParseTableOfTitles()
        {
            byte[] table_of_titles = ReadSector(sector_pointer_to_table_of_titles);
  //          byte[] b = new byte[2];
  //          Buffer.BlockCopy(table_of_titles, 0, b, 0, 2);
            int nbTitles = table_of_titles[0] * 256 + table_of_titles[1];// BufferConvert.ByteToInteger(b);
            for (int i = 0; i < nbTitles; i++)
            {
                byte[] b = new byte[12];
                Buffer.BlockCopy(table_of_titles, 8 + i * 12, b, 0, 12);
                TitleStructure ts = new TitleStructure(b);
                titles.Add(ts);
            }
        }
        private void ReadMenu()
        {
            byte[] data = ReadSector(sector_pointer_to_Menu_Program_Chain_table);

        }
        private void ReadTitlesAndChapters()
        {
            byte[] data = ReadSector(sector_pointer_to_VTS_PTT_SRPT);
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
                Buffer.BlockCopy(audio_attributes, i * 8, b, 0, 8);
                audio.Add(new AudioAtributes(name, b));
            }
        }
     }
}
