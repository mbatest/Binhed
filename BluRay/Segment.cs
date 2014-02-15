using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Utils;

namespace BluRay
{
    [DefaultPropertyAttribute("Segment")]
    public class VideoSegment : Segment
    {
        #region Properties
        [CategoryAttribute("Codec"), DescriptionAttribute("Codec")]
        public string Codec
        {
            get
            {
                int low = codecData[0] & 0x0F;
                int high = (codecData[0] & 0xF0) >> 4;
                switch (codecData[0])
                {
                    //MPEG 2 ??
                    case 0x1B:
                        return "AVC";
                    case 0xEA:
                        return "VC-1";
                    default:
                        return "";
                }
            }
        }
        [CategoryAttribute("Codec"), DescriptionAttribute("Resolution")]
        public string Resolution
        {
            get
            {
                int high = (codecData[1] & 0xF0) >> 4;
                return resolution[high];
            }
        }
        [CategoryAttribute("Codec"), DescriptionAttribute("FrameRate")]
        public string FrameRate
        {
            get
            {
                int low = codecData[1] & 0x0F;
                return frameRate[low];
            }
        }
        #endregion
        public VideoSegment()
        {
        }
        public VideoSegment(byte[] buffer, ref int start) : base(buffer,ref start)
        {
        }
  }
    [DefaultPropertyAttribute("Segment")]
    public class AudioSegment : Segment
    {
        #region Properties
        [CategoryAttribute("Codec"), DescriptionAttribute("Codec")]
        public string Details
        {
            get
            {
                int low = codecData[0] & 0x0F;
                int high = (codecData[0] & 0xF0) >> 4;
                return ((bd_stream_type_e)codecData[0]).ToString();// Codec[low];
            }
        }
        [CategoryAttribute("Codec"), DescriptionAttribute("Frequency")]
        public string Frequency
        {
            get
            {
                int low = codecData[1] & 0x0F;
                int high = (codecData[1] & 0xF0) >> 4;
                return frequency[low];
            }
        }
        [CategoryAttribute("Codec"), DescriptionAttribute("Surround")]
        public string Surround
        {
            get
            {
                int low = codecData[1] & 0x0F;
                int high = (codecData[1] & 0xF0) >> 4;
                return surround[high];
            }
        }
        [CategoryAttribute("Language"), DescriptionAttribute("Langue")]
        public string Language
        {
            get { return Encoding.Default.GetString(lg); }
        }
        #endregion
        public AudioSegment()
        {
        }
        public AudioSegment(byte[] buffer, ref int start)
            : base(buffer, ref start)
        {
       }
    }
    public class Segment
    {
        #region Members
        public static string[] audioCodec = new string[] { "DD", "DTS", "Dolby Lossless", "DD+", "DTS-HD", "DTS-HD XLL", "DD+", "DTS-HD" };
        public static string[] frequency = new string[] { "-", "48 Khz", "-", "-", "96Khz", "192Khz", "", "", "", "", "", "", "", "", "", "" };
        public static string[] surround = new string[] { "-", "Mono", "-", "Stereo", "-", "-", "Multi Channel", "-", "-", "-", "-", "-", "ST+", "-", "-" };
        public static string[] resolution = new string[] { "-", "480i", "576i", "480", "1080i", "720p", "1080p", "576p" };
        public static string[] frameRate = new string[] { "-", "23,976", "24", "25", "29,97", "-", "50", "59,94" };
        public byte[] codecData;
        public byte[] lg;
        public int mPID;
        public int nb;
        public int sPid;
        public int sCid;
        public int sPID;
        public byte[] data = new byte[0x10];
        [CategoryAttribute("Item Type"), DescriptionAttribute("Item Type")]
        public string ItemType
        {
            get
            {
                return "PlayItem";
            }
        }
        [CategoryAttribute("Index"), DescriptionAttribute("Segments")]
        public int Nb
        {
            get { return nb; }
            set { nb = value; }
        }
        [CategoryAttribute("Segment Type"), DescriptionAttribute("Segment type")]
        public int MPID
        {
            get { return mPID; }
            set { mPID = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public int SPid
        {
            get { return sPid; }
            set { sPid = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public int SCid
        {
            get { return sCid; }
            set { sCid = value; }
        }
        [CategoryAttribute("Unknown"), DescriptionAttribute("Segments")]
        public int SPID
        {
            get { return sPID; }
            set { sPID = value; }
        }
        #endregion
        public Segment()
        {
        }
        public Segment(byte[] buffer, ref int start)
        {
            int l = buffer[start]; //first block length
            byte[] buf1 = new byte[l];
            Buffer.BlockCopy(buffer, start + 1, buf1, 0, l);
            Nb = buf1[0];
            MPID = ConvertBuffer.ReadShortInteger(buf1, 1);
            SPid = ConvertBuffer.ReadShortInteger(buf1, 3); ;
            SCid = ConvertBuffer.ReadShortInteger(buf1, 5);
            SPID = ConvertBuffer.ReadShortInteger(buf1, 7);
            //first block length
            start += buf1.Length + 1;
            int l2 = buffer[start];
            byte[] buf2 = new byte[l2];
            Buffer.BlockCopy(buffer, start + 1, buf2, 0, l2);
            //encoding
            codecData = new byte[2];
            Buffer.BlockCopy(buf2, 0, codecData, 0, 2);
            lg = new byte[3];
            Buffer.BlockCopy(buf2, 2, lg, 0, 3);// nothing ??
            start += buf2.Length + 1;
        }
    }
}
