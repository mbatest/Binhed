using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Text;
using Utils;
//Voir BdEdit pour détails
namespace BluRay
{
    [Serializable]
    [DefaultPropertyAttribute("Clip")]
    public class PlayItem
    {
        bool isMultiAngle;
        int connectionCondition;
        string codec_id;

        private string année;
        private string name;
        private float inTime;//en millisecondes
        private float outTime;//en millisecondes
        private int rS;
        private byte[] uO1;
        #region STN
        private List<VideoSegment> vids;
        private List<AudioSegment> auds;
        public List<Segment> ig;
        public List<Segment> pg;
        public List<VideoSegment> secondary_video;
        public List<AudioSegment> secondary_audio;
        #endregion
        public PlayItem(byte[] buffer)
        {
            BitStreamReader bs = new BitStreamReader(buffer, true);
            Name = bs.ReadString(5);
            codec_id = bs.ReadString(4);

            bs.skip(11);
            isMultiAngle = bs.ReadBool();
            connectionCondition = bs.ReadIntFromBits(4);

            RS = bs.ReadByte();// stc_id
            //       bs.BitPosition -= 8;
            InTime = (float)bs.ReadInteger() / 45000; //0x0D
            OutTime = (float)bs.ReadInteger() / 45000;
            UO1 = bs.ReadBytes(8);
            UO1_flags = new BD_UO_MASK(UO1);
            //            int a = bs.ReadInteger();
            bool random_access_flag = bs.ReadBool();
            bs.skip(7);
            byte still_mode = bs.ReadByte();
            if (still_mode == 0x01)
            {
                short still_time = bs.ReadShort();
            }
            else
            {
                bs.skip(16);
            }
            int angle_count = 1;
            if (isMultiAngle)
            {
                angle_count = bs.ReadByte();
                if (angle_count < 1)
                {
                    angle_count = 1;
                }
                bs.skip(6);
                bool is_different_audio = bs.ReadBool();
                bool is_seamless_angle = bs.ReadBool();
            }
            int blockLength = bs.ReadShort();
            int unknown = bs.ReadShort();
            int start = 0x25;
            bs.BitPosition = 0x25 * 8;
            #region segment list
            byte[] streamNumbers = new byte[6];
            for (int w = 0; w < 6; w++)
            {
                streamNumbers[w] = bs.ReadByte();
                start++;
            }
            #endregion
            start = 0x31;
            #region streams
            #region first segments are video
            if (streamNumbers[0] > 0)
            {
                VideoSegments = new List<VideoSegment>();
                for (int w = 0; w < streamNumbers[0]; w++)
                {
                    try
                    {
                        VideoSegments.Add(new VideoSegment(buffer, ref start));
                    }
                    catch (Exception e) { }
                }
            }
            #endregion
            #region audio segments
            if (streamNumbers[1] > 0)
            {
                AudioSegments = new List<AudioSegment>();
                for (int w = 0; w < streamNumbers[1]; w++)
                {
                    AudioSegments.Add(new AudioSegment(buffer, ref start));
                }
            }
            #endregion
            #region ig segments
            if (streamNumbers[2] > 0)
            {
                ig = new List<Segment>();
                for (int w = 0; w < streamNumbers[1]; w++)
                {
                    ig.Add(new Segment(buffer, ref start));
                }
            }
            #endregion
            #region pg segments
            if (streamNumbers[3] > 0)
            {
                pg = new List<Segment>();
                for (int w = 0; w < streamNumbers[1]; w++)
                {
                    pg.Add(new Segment(buffer, ref start));
                }
            }
            #endregion
            #region secondary video
            if (streamNumbers[4] > 0)
            {
                VideoSegments = new List<VideoSegment>();
                for (int w = 0; w < streamNumbers[0]; w++)
                {
                    try
                    {
                        VideoSegments.Add(new VideoSegment(buffer, ref start));
                    }
                    catch (Exception e) { }
                }
            }
            #endregion
            #region secondary audio segments
            if (streamNumbers[5] > 0)
            {
                AudioSegments = new List<AudioSegment>();
                for (int w = 0; w < streamNumbers[1]; w++)
                {
                    AudioSegments.Add(new AudioSegment(buffer, ref start));
                }
            }
            #endregion
            #endregion
        }
        public PlayItem()
        {
        }
        #region Properties
        public string Codec_id
        {
            get { return codec_id; }
            set { codec_id = value; }
        }
        public BD_UO_MASK UO1_flags;
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public List<VideoSegment> VideoSegments
        {
            get { return vids; }
            set { vids = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public List<AudioSegment> AudioSegments
        {
            get { return auds; }
            set { auds = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Clips")]
        public byte[] UO1
        {
            get { return uO1; }
            set { uO1 = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Clips")]
        public int RS
        {
            get { return rS; }
            set { rS = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Clips")]
        public float OutTime
        {
            get { return outTime; }
            set { outTime = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Clips")]
        public float InTime
        {
            get { return inTime; }
            set { inTime = value; }
        }
        [CategoryAttribute("Clips"), DescriptionAttribute("Clips")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [CategoryAttribute("Date"), DescriptionAttribute("Date")]
        public string Année
        {
            get { return année; }
            set { année = value; }
        }
        public string date;
        [CategoryAttribute("Raw Data"), DescriptionAttribute("Clips")]
         private byte[] data2;
        [CategoryAttribute("Raw Data 2"), DescriptionAttribute("Clips")]
        public byte[] Data2
        {
            get { return data2; }
            set { data2 = value; }
        }
        public int ConnectionCondition
        {
            get { return connectionCondition; }
            set { connectionCondition = value; }
        }
        public bool IsMultiAngle
        {
            get { return isMultiAngle; }
            set { isMultiAngle = value; }
        }
        #endregion
    }
    public struct PlayItemMark
    {
        [CategoryAttribute("Clips"), DescriptionAttribute("Clips")]
        public short EntryPId
        {
            get { return entry_es_pid; }
            set { entry_es_pid = value; }
        }
        [CategoryAttribute("Clips"), DescriptionAttribute("Clips")]
        public float MarkTime
        {
            get { return (float) mark_time/45000; }
         }
        [CategoryAttribute("Clips"), DescriptionAttribute("Clips")]
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
        [CategoryAttribute("Clips"), DescriptionAttribute("Clips")]
        public short Play_Item_ref
        {
            get { return play_item_ref; }
            set { play_item_ref = value; }
        }
        public byte mark_id;
        public byte mark_type;
        public short play_item_ref;
        public int mark_time;
        public short entry_es_pid;
        public int duration;
    }

    [DefaultPropertyAttribute("Clip Info")]
    public class Clpi
    {
        List<EntryPoint> coarse = new List<EntryPoint>();
        List<EntryPoint> fine = new List<EntryPoint>();
        int recordingRate;
        int packetsNumber;
        string header;
        string hdmv;
        float inTime;
        float outTime;
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public List<EntryPoint> Coarse
        {
            get { return coarse; }
            set { coarse = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public List<EntryPoint> Fine
        {
            get { return fine; }
            set { fine = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public int RecordingRate
        {
            get { return recordingRate; }
            set { recordingRate = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public int PacketsNumber
        {
            get { return packetsNumber; }
            set { packetsNumber = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public string Header
        {
            get { return header; }
            set { header = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public string Hdmv
        {
            get { return hdmv; }
            set { hdmv = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public float InTime
        {
            get { return inTime; }
            set { inTime = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public float OutTime
        {
            get { return outTime; }
            set { outTime = value; }
        }
        public Clpi(string fileName)
        {
            CLPI cl = new CLPI(fileName);
            BinaryFileReader FS = new BinaryFileReader(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, false);
            /*            byte[] buffer = new byte[bs.Length];
                        bs.Read(buffer, 0, (int)bs.Length);*/
            FS.Position = 0;
            header = FS.ReadString(8);
            if (header.IndexOf("HDMV") != 0)
                return;
            // 0x08 00 00 00 ec : impact sur sequence info ??
            // 0x0c 00 00 01 06
            int x08 = FS.ReadInteger();// ConvertBuffer.ReadInteger(buffer, 0x08);
            int endOfTimeData = FS.ReadInteger(); // ConvertBuffer.ReadInteger(buffer, 0x0c);
            int endStreams = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, 0x10);
            // 0x10 adresse fin description stream
            // 0x14 fin des blocs de 4 octets
            int end4ByteBlocks = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, 0x14);
            int AddressSizeLastBlock = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, 0x18);
            // 0x18 adresse quatre octets plus loin ?*
            // 0x20 - 27 : 00
            // 0x28
            // 0x2C
            // 0x34 recording rate (codage ?)
            FS.Position = 0x34;
            recordingRate = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, 0x34);
            // 0x38 nombre de paquets
            packetsNumber = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, 0x38);
            // 0x40 -> 0xbb 00
            FS.Position = 0xbb;
            int xbb = FS.ReadShort(); //ConvertBuffer.ReadInteger(buffer, 0xbb);
            // Oxbd 1e80
            int xbd = FS.ReadShort(); //ConvertBuffer.ReadInteger(buffer, 0xbd);
            // 0xbf HDMV
            hdmv = FS.ReadString(4); //ConvertBuffer.ReadString(buffer, 0xbf, 4);
            if (hdmv != "HDMV")
            {

            }
            FS.Position = 0xdc;
            int xdc = FS.ReadShort();// ConvertBuffer.ReadShort(buffer, 0xdc);
            if (xdc > 0)
            {
                int xde = FS.ReadInteger(); // ConvertBuffer.ReadInteger(buffer, 0xde);
                string name = FS.ReadString(9); //ConvertBuffer.ReadString(buffer, 0xe2, 9);
                // 0xe2 clip suivant ? sur 9 octets (numéro sur 5 puis codec)
                FS.Position = x08;
                int U6 = FS.ReadInteger(); // ConvertBuffer.ReadInteger(buffer, x08);//0xec)
            }
            // 0x100 début et fin du clip 
            int sequenceInfoNumber = FS.ReadShort(); //ConvertBuffer.ReadShort(buffer, 0xe0);// adresse à vérifier
            int start = 0xf2;
            FS.Position = 0xf2;
            for (int u = 0; u < sequenceInfoNumber; u++)
            {
                int sequenceATCStart = FS.ReadInteger(); // ConvertBuffer.ReadInteger(buffer, start);
                int num = FS.ReadByte(); // ConvertBuffer.ReadByteInteger(buffer, start + 4);
                int offset = FS.ReadByte(); // ConvertBuffer.ReadByteInteger(buffer, start + 5);
                start += 0x0D;// 0xf8 - OxFd ???
            }
            FS.Position = endOfTimeData - 8;
            inTime = (float)FS.ReadInteger() / 45000;// (float)ConvertBuffer.ReadInteger(buffer, endOfTimeData - 8) / 45000;
            outTime = (float)FS.ReadInteger() / 45000;//(float)ConvertBuffer.ReadInteger(buffer, endOfTimeData - 4) / 45000;
            int x106 = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, 0x106);
            int x10a = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, 0x10a);
            int x10e = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, 0x10e);
            int numberStreams = FS.ReadByte(); //ConvertBuffer.ReadByteInteger(buffer, endOfTimeData + 0xC);
            start = endOfTimeData + 0x0e;
            FS.Position = endOfTimeData + 0x0e;
            for (int u = 0; u < numberStreams; u++)
            {
                byte[] loc = FS.ReadBytes(0x5);// new byte[0x18];
                //   Buffer.BlockCopy(buffer, start, loc, 0, 0x18);
                int ratio = FS.ReadByte();// ConvertBuffer.ReadByteInteger(loc, 5);//0x30 = 16:9  0x20 4:33
                // à détailler
                FS.ReadBytes(0x18 - 6);
                start += 0x18;
            }
            start = endStreams;
            FS.Position = endStreams;
            int Ux = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, start);
            start += 4;
            int EPType = FS.ReadShort(); //ConvertBuffer.ReadShort(buffer, start);
            // EPType 00 01
            start += 2;
            start += 2;
            FS.Position += 2;
            int PID = FS.ReadShort(); //ConvertBuffer.ReadShort(buffer, start);
            start += 2;
            int Unk = FS.ReadShort(); //ConvertBuffer.ReadShort(buffer, start);
            start += 2;
            int EPCoarseNumber = FS.ReadShort() / 4; //ConvertBuffer.ReadShort(buffer, start) / 4;
            start += 2;
            int EPFineNumber = FS.ReadShort(); //ConvertBuffer.ReadShort(buffer, start);
            start += 2;
            int EPmapStart = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, start);
            start += 4;
            int EPTableLength = FS.ReadInteger(); //ConvertBuffer.ReadInteger(buffer, start);
            start += 4;
            for (int i = 0; i < EPCoarseNumber; i++)
            {
                // Blocs de huit octets : ref to xxxxx, pTS EP xx, SPN EP sur 4 octets
                byte[] loc = FS.ReadBytes(4);// new byte[8];
                EntryPoint ep = new EntryPoint();
                ep.SPN = FS.ReadInteger(); //ConvertBuffer.ReadInteger(loc, 4);
                start += 8;
                coarse.Add(ep);
            }
            // Table fine EP
            for (int i = 0; i < EPFineNumber; i++)
            {
                // blocs de 4 octets de la forme 39 db 00 98 (pTS EP + SPN EP)
                // on sépare le premier "3" (I end ?)
                // 9db /2 = 4ED
                // 9db % 2 = 1 -> 1 00 98
                EntryPoint ep = new EntryPoint();
                byte[] loc = FS.ReadBytes(2);// new byte[4];
                ep.Iend = loc[0] / 0x0F;
                ep.PTS = (loc[0] & 0x0F) * 256 + loc[1];
                ep.PTS /= 2;
                ep.SPN = FS.ReadShort();// ConvertBuffer.ReadShort(loc, 2);
                ep.SPN = (loc[1] % 2) * 65536 + ep.SPN;
                fine.Add(ep);
                start += 4;
            }
            FS.Position = AddressSizeLastBlock;
            int sizeLastBlock = FS.ReadInteger();// ConvertBuffer.ReadInteger(buffer, AddressSizeLastBlock);
        }
    }
    public class EntryPoint
    {
        int refTo;
        int iend;
        int pTS;
        int sPN;
        public int RefTo
        {
            get { return refTo; }
            set { refTo = value; }
        }
        public int Iend
        {
            get { return iend; }
            set { iend = value; }
        }
        public int PTS
        {
            get { return pTS; }
            set { pTS = value; }
        }
        public int SPN
        {
            get { return sPN; }
            set { sPN = value; }
        }
    }

}
