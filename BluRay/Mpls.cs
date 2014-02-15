using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using Utils;

namespace BluRay
{
    [DefaultPropertyAttribute("Playing list")]
    public class Mpls
    {
        private List<PlayItem> clipsData = new List<PlayItem>();
        private List<PlayItemMark> playItemMarks = new List<PlayItemMark>();
        private int UserMaskTable;
        private int UserMaskTable2;
        private string date;
        private string name;
        private string headerString;
        private List<Clpi> clips = new List<Clpi>();
        public List<PlayItem> ClipsData
        {
            get { return clipsData; }
            set { clipsData = value; }
        }
        [CategoryAttribute("Clips"), DescriptionAttribute("Clips")]
        public List<Clpi> Clips
        {
            get { return clips; }
            set { clips = value; }
        }
        [CategoryAttribute("Header"), DescriptionAttribute("En tête")]
        public string HeaderString
        {
            get { return headerString; }
            set { headerString = value; }
        }
        public int UserMask_1
        {
            get { return UserMaskTable; }
            set { UserMaskTable = value; }
        }
        public int UserMask_2
        {
            get { return UserMaskTable2; }
            set { UserMaskTable2 = value; }
        }
        [CategoryAttribute("Date"), DescriptionAttribute("Date")]
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        [CategoryAttribute("Clips"), DescriptionAttribute("Play Mode")]
        public string PlayMode
        {
            get
            {
                switch (playMode)
                {
                    case 1: return "Sequential";
                    case 2: return "Random";
                    case 3: return "Shuffle";
                    default: return "";
                }
            }
        }
        int playMode;
        public int uk71;
        public int offsetToClipData;
        public int lengthOfClipData;
        public int offsetToEndoFClipdata;
        public int clipDataLength;
        public bool random_access_flag;
        public bool audio_mix_flag;
        public bool lossless_bypass_flag;
            
        public Mpls(string fileName)
        {
            MPLS mp = new MPLS(fileName);
            BinaryFileReader FS = new BinaryFileReader(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, false);
            name = Path.GetFileNameWithoutExtension(fileName);
            byte[] buffer = new byte[0x45];
            headerString = FS.ReadString(8);
            if (headerString.IndexOf("MPLS") != 0)
                return;
            #region 3 offsets
            offsetToClipData = FS.ReadInteger();//0x08
            long startOfClipData = offsetToClipData + FS.Position - 1;
            lengthOfClipData = FS.ReadInteger();//0x0c
            offsetToEndoFClipdata = FS.ReadInteger();//0x10
            long endOfClipData = offsetToClipData + FS.Position - 1;
            #endregion
            byte[] uk1 = FS.ReadBytes(20);//Reserved 20 bytes
            #region All that unknown : App Info Play list
            int blength = FS.ReadInteger();
            int end = (int) FS.Position + blength;
            FS.ReadByte(); // Reserved
            playMode  = FS.ReadByte();//0x2D :  1 : Sequential 2 : Random 3 : Shuffle
            if ((playMode == 2) || (playMode == 3))
                uk71 = FS.ReadShort();
            else
                FS.ReadShort();
            UserMaskTable = FS.ReadInteger();//ConvertBuffer.ReadInteger(buffer, 0x30); //16 Flags
            UserMaskTable2 = FS.ReadInteger();//ConvertBuffer.ReadInteger(buffer, 0x34);
            #endregion
            int uk8 = FS.ReadInteger();//ConvertBuffer.ReadInteger(buffer, 0x38);
            // Octet 0x38, flags : 8 random access 4 audio mix 2 bypass mixer
            random_access_flag = (uk8&0x80)==0x80;
            audio_mix_flag = (uk8 & 0x40) == 0x40;
            lossless_bypass_flag = (uk8 & 0x40) == 0x40;
            clipDataLength = FS.ReadShort();//ConvertBuffer.ReadShort(buffer, 0x3c); //clip buffer length
            FS.ReadShort();
            int nbOfClips = FS.ReadShort(); //ConvertBuffer.ReadShort(buffer, 0x40);// number of clipsData
            int nbOfSubClips = FS.ReadShort();
            for (int i = 0; i < nbOfClips; i++)
            {
                int PlayItemLength = FS.ReadShort();
                PlayItem c = new PlayItem(FS.ReadBytes(PlayItemLength));
                clipsData.Add(c);
            }
            for (int i = 0; i < nbOfSubClips; i++)
            {
            }
    //        bs.Seek(lengthOfClipData, SeekOrigin.Begin);
            int blkLength = FS.ReadInteger();// bs.Read(buffer, 0, 4);//Bloc length            
            int nbPlayMarks = FS.ReadShort();// bs.Read(buffer, 0, 2);//number of items
            buffer = new byte[0xE];
            for (int i = 0; i < nbPlayMarks; i++)
            {
                PlayItemMark plmi = new PlayItemMark();
                plmi.mark_id = FS.ReadByte();
                plmi.mark_type = FS.ReadByte();
                plmi.play_item_ref = (short) FS.ReadShort();
                plmi.mark_time = FS.ReadInteger();
                plmi.entry_es_pid = (short) FS.ReadShort();
                plmi.duration = FS.ReadInteger();
               playItemMarks.Add(plmi);
            }
            #region PlayList marks
            FS.Seek(offsetToEndoFClipdata, SeekOrigin.Begin);
            buffer = FS.ReadBytes(0x18);
            string g2 = FS.ReadString(4);// ConvertBuffer.ReadString(buffer, 0, 4);//Normalement PLEX
            FS.ReadBytes(0x3d);
            date = FS.ReadString(10);// ConvertBuffer.ReadString(buffer, 0x3d, 10);
            buffer = new byte[66];
            FS.ReadBytes(0xFD);
            for (int j = 0; j < nbOfClips; j++)
            {
                FS.ReadBytes(0x08);
                //       ???
                int jour = FS.FromBCD(); // ConvertBuffer.ReadBCD(buffer[index]);
                int heure = FS.FromBCD(); //ConvertBuffer.ReadBCD(buffer[index + 1]);
                int min = FS.FromBCD(); //ConvertBuffer.ReadBCD(buffer[index + 2]);
                int sec = FS.FromBCD(); //ConvertBuffer.ReadBCD(buffer[index + 3]);*/
                FS.ReadBytes(0x0a);
                clipsData[j].Année = FS.ReadString(10);
                 FS.ReadBytes(0x22);
            }
            #endregion
            FS.Close();
            foreach (PlayItem cp in clipsData)
            {
                string s1 = cp.Name + ".CLPI";
                string s = Path.GetDirectoryName(FS.Name).Replace("PLAYLIST", "CLIPINF\\");
                s += s1; 
                Clpi clp = new Clpi(s);
                clips.Add(clp);
            }
        }
        public override string ToString()
        {
            return name;
        }
    }

}
