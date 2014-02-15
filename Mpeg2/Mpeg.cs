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
    public class Mpeg2 : LOCALIZED_DATA
    {
        private List<IBloc> blocs = new List<IBloc>();
        private List<GOP> gops = new List<GOP>();
        public List<SequenceHeader> seqHead = new List<SequenceHeader>();
        public List<IBloc> videoStream = new List<IBloc>();
        private long position;
        public List<IBloc> Blocs
        {
            get { return blocs; }
            set { blocs = value; }
        }
        public List<GOP> Gops
        {
            get { return gops; }
            set { gops = value; }
        }
        public string PositionInFile
        {
            get
            {
                return position.ToString();
            }
        }
        BinaryFileReader sw;
        //http://dvd.sourceforge.net/dvdinfo/mpeghdrs.html
        //000001ba : 13 octets
        //000001bb : longueur sur les octets 4 et 5
        // 0xC0 - 0xDF MPEG-1 or MPEG-2 audio stream 
        //0xE0 - 0xEF MPEG-1 or MPEG-2 video stream 
        public int[] DecodeGop(byte[] b)
        {
            int[] data = new int[4];
            data[0] = (b[0] & 0x7c) / 4;
            data[1] = (b[0] & 0x03) * 16 + (b[1] & 0xF0) / 16;
            data[2] = (b[1] & 0x07) * 8 + (b[2] & 0xE0) / 0xE0;
            data[3] = (b[2] & 0x1F) * 2 + (b[3] & 0x80) / 0x80;
            return data;
        }
        public Mpeg2(string Filename)
        {
            Picture CurrentPicture = null;
            GOP CurrentGroup = null;
            SequenceHeader CurrentSequence = null;
            VideoStream CurrentStream = null;
            byte[] buffer = new byte[30];
            sw = new BinaryFileReader(Filename, FileMode.Open, FileAccess.Read, FileShare.Read, false);
            while (sw.Position <= sw.Length)
            {
                if (sw.Position > 50000000)
                    break;
                #region analyze
                if (sw.Position >= sw.Length)
                    break;
                byte[] dat = sw.ReadBytes(4);
                int startExtra = 0;
                while (!((dat[0] == 0x00) && ((dat[1] == 0x00) && (dat[2] == 0x01))) && (sw.Position < sw.Length))
                {
                    sw.Position -= 3;
                    dat = sw.ReadBytes(4);
                    startExtra += 1;
                }
                if (startExtra > 0)
                {
                    sw.Position -= startExtra;
                    if (CurrentStream != null)
                        CurrentStream.unknown = sw.ReadBytes(startExtra);
                }
                IBloc b = new Bloc();
                b.Offset = sw.Position - 4;
                b.Header = dat;
                try
                {
                    #region Case
                    switch (b.HeaderCode)
                    {
                        case 0x00:
                            #region Picture
                            b.Data = sw.ReadBytes(4);
                            CurrentPicture = new Picture(b);
                            if (CurrentPicture.ImageType != "I")
                            {
                                int k = sw.ReadByte();
                            }
                            if (CurrentGroup != null)
                            {
                                CurrentGroup.Pictures.Add(CurrentPicture);
                                CurrentGroup.tempOrder.Add(CurrentPicture.Temporal_sequence_number);
                            }
                            break;
                            #endregion
                        case 0xb2://user buffer
                            blocs.Add(b);
                            break;
                        case 0xb3:
                            #region Sequence header
                            b.Data = sw.ReadBytes(8);
                            SequenceHeader sq = new SequenceHeader(b);
                            seqHead.Add(sq);
                            blocs.Add(sq);
                            CurrentSequence = sq;
                            if (sq.Load_intra_quantiser_matrix)
                            {
                            }
                            if (sq.Load_non_intra_quantiser_matrix)
                            {
                            }
                            break;
                            #endregion
                        case 0xb4:
                            break;
                        case 0xb5:
                            #region extension
                            int aa = sw.ReadByte();
                            aa = (aa & 0xF0) / 16;
                            sw.Position--;
                            switch (aa)
                            {
                                case 1:
                                    buffer = sw.ReadBytes(6);
                                    b.Data = buffer;
                                    break;
                                case 2:
                                    buffer = sw.ReadBytes(13);// ???
                                    b.Data = buffer;
                                    break;
                                case 8:
                                    buffer = sw.ReadBytes(5);
                                    b.Data = buffer;
                                    break;
                            }
                            Extension ex = new Extension(b);
                            ex.extensionType = aa;
                            int id = ex.detail();
                            if (id > 0)
                            {
                                buffer = sw.ReadBytes(id);
                            }
                            switch (aa)
                            {
                                case 1:
                                    if (CurrentSequence != null)
                                        CurrentSequence.SequenceExtension = ex;
                                    break;
                                case 4:
                                    break;
                                case 8:
                                    if (CurrentPicture != null)
                                        CurrentPicture.pictureExtension = ex;
                                    break;
                            }
                            //                           blocs.Add(ex);
                            break;
                            #endregion
                        case 0xb7:
                            // end of buffer
                            return;
                        case 0xb8:
                            #region   Group of pictures
                            b.Data = sw.ReadBytes(4);
                            CurrentGroup = new GOP(b);
                            if (CurrentSequence != null)
                            {
                                CurrentGroup.framerate = CurrentSequence.FrameRate;
                                CurrentGroup.aspectRatio = CurrentSequence.AspectRatio;
                                CurrentGroup.s = CurrentSequence.VideoSize;
                            }
                            CurrentSequence.gops.Add(CurrentGroup);
                            gops.Add(CurrentGroup);
                            break;
                            #endregion
                        case 0xb9:
                            break;
                        case 0xba:
                            #region Pack header length 10
                            buffer = sw.ReadBytes(10);
                            if ((buffer[9] & 0x7) != 0x0)//pack_stuffing_length -- A 3 bit integer specifying the number of stuffing bytes which follow this field.
                            {
                                int u = (buffer[9] & 0x7);
                                byte[] stuff = sw.ReadBytes(u);
                            }
                            b.Data = buffer;
                            PackHeader p = new PackHeader(b);
                            blocs.Add(p);
                            break;
                            #endregion
                        case 0xbb:
                            #region System header
                            // http://www.mpucoder.com/DVD/sys_hdr.html 
                            buffer = sw.ReadBytes(sw.ReadShort());
                            b.Data = buffer;
                            SystemHeader sh = new SystemHeader(b);
                            blocs.Add(sh);
                            break;
                            #endregion
                        case 0xbc://program stream map
                            break;
                        case 0xbd://Private stream 1                          
                        case 0xbf://Private stream 2
                        case 0xbe://padding stream
                            #region padding stream
                            buffer = sw.ReadBytes(sw.ReadShort());
                            b.Data = buffer;
                            blocs.Add(b);
                            break;
                            #endregion
                        case 0xc0://Audio
                        case 0xe0://video
                            #region Data stream
                            if (sw.Position >= 0x1200E)
                            {
                            }
                            int si = sw.ReadShort();
                            byte[] extensionBuffer = sw.ReadBytes(3);//extension
                            b.Data = buffer;
                            CurrentStream = new VideoStream(b, extensionBuffer);
                            if (CurrentStream.PTS_DTS_flags != 0)
                            {
                                switch (CurrentStream.PTS_DTS_flags)
                                {
                                    case 0x02:
                                        CurrentStream.PTS_DTS_Buffer.Add(sw.ReadBytes(5));
                                        break;
                                    case 0x01:
                                    case 0x03:
                                        CurrentStream.PTS_DTS_Buffer.Add(sw.ReadBytes(5));
                                        CurrentStream.PTS_DTS_Buffer.Add(sw.ReadBytes(5));
                                        break;
                                }
                                CurrentStream.PTS_DTS_Decode();
                            }
                            if (CurrentStream.ESCR_flag)
                            {
                                sw.ReadBytes(6);
                            }
                            if (CurrentStream.ES_rate_flag)
                            {
                                sw.ReadBytes(2);
                            }
                            if (CurrentStream.additional_copy_info_flag)
                                sw.ReadBytes(1);
                            if (CurrentStream.PES_CRC_flag)
                                sw.ReadBytes(2);
                            if (CurrentStream.PES_extension_flag)
                            {
                                byte ext = (byte) sw.ReadByte();
                                if ((ext & 0x80) == 0x80)//PES private buffer flag
                                {
                                }
                                if ((ext & 0x40) == 0x40)//pack header field flag
                                {
                                }
                                if ((ext & 0x20) == 0x20)//program packet sequence counter flag
                                {
                                    sw.ReadBytes(2);
                                }
                                if ((ext & 0x10) == 0x10)//P-STD buffer flag
                                {
                                    byte[] P_STD_buffer = sw.ReadBytes(2);
                                    bool bufScale = (P_STD_buffer[0] & 0x20) == 0x020;
                                    int bufSize = (P_STD_buffer[0] & 0x1F) * 256 + P_STD_buffer[1];
                                }
                                if ((ext & 0x01) == 0x01)//PES extension flag 2
                                {
                                }
                            }
                            if (CurrentStream.PES_header_data_length > 0)
                            {
                                CurrentStream.headerData = sw.ReadBytes(CurrentStream.PES_header_data_length);
                            }
                            long pos = sw.Position - 3 - CurrentStream.PES_header_data_length;
                            CurrentStream.payload = sw.ReadBytes(si);
                            sw.Seek(pos, SeekOrigin.Begin);
                            videoStream.Add(CurrentStream);
                            blocs.Add(CurrentStream);
                            break;
                            #endregion
                        default:
                            #region Slice. Ends with new marker
                            long start = sw.Position;
                            if ((b.HeaderCode >= 0x01) && (b.HeaderCode <= 0xAF))
                            {
                                List<byte> bts = new List<byte>();
                                int endBlock = sw.ReadByte();
                                while (sw.Position < sw.Length)
                                {
                                    #region Get buffer until 0x00 0x00
                                    bts.Add((byte)endBlock);
                                    endBlock = sw.ReadByte();
                                    start = sw.Position;
                                    if (endBlock == 00)
                                    {
                                        byte[] code = new byte[2];
                                        sw.Read(code, 0, 2);
                                        if ((code[0] == 0x00) && (code[1] == 0x01))
                                        {
                                            sw.Position -= 3;
                                            break;
                                        }
                                        else
                                        {
                                            bts.Add((byte)endBlock);
                                            sw.Position -= 2;
                                        }
                                    }
                                    #endregion
                                }
                                b.Data = bts.ToArray();
                                Slice sl = new Slice(b);
                                if (CurrentPicture != null)
                                {
                                    CurrentPicture.Slices.Add(sl);
                                    if (sl.HeaderCode != CurrentPicture.Slices.Count)
                                    {
                                    }
                                }
                            }
                            else
                            {
                            }
                            #endregion
                            break;
                    }
                    #endregion
                }
                catch (Exception e) { }
                #endregion
            }
            position = sw.Position;
            sw.Close();
            int images = 0;
            foreach (GOP g in gops)
            {
                images += g.Pictures.Count;
            }
            float sec = images / 25;
        }
    }
    public class Bloc : LOCALIZED_DATA, IBloc
    {
        private byte[] data;
        private byte[] header;
        private long offset;
        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }
        public byte[] Header
        {
            get { return header; }
            set { header = value; }
        }
        public long HeaderCode
        {
            get
            {
                if ((header[0] != 0) || (header[1] != 0) || (header[2] != 0x01))
                    return -1;
                return header[3];
            }
        }
        public long Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public List<byte> extraData = new List<byte>();
        public override string ToString()
        {
            string type = HeaderType;
            #region Type expansion
            #endregion
            return type + " " + offset.ToString("X8"); ;
        }

        public string HeaderType
        {
            get
            {
                string type = "";
                switch (HeaderCode)
                {
                    case 0x00:
                        type = "Picture";
                        break;
                    case 0xB0:
                        type = "Reserved ";
                        break;
                    case 0xB1:
                        type = "Reserved";
                        break;
                    case 0xB2:
                        type = "User data";
                        break;
                    case 0xB3:
                        type = "Sequence header";
                        break;
                    case 0xB4:
                        type = "Sequence error";
                        break;
                    case 0xB5:
                        type = "Extension";
                        break;
                    case 0xB6:
                        type = "Reserved";
                        break;
                    case 0xB7:
                        type = "Sequence end";
                        break;
                    case 0xB8:
                        type = "Group of Pictures";
                        break;
                    case 0xB9:
                        type = "Program end (terminates a program stream)";
                        break;
                    case 0xBA:
                        type = "Pack header";
                        break;
                    case 0xBB:
                        type = "System Header";
                        break;
                    case 0xBC:
                        type = "Program Stream Map";
                        break;
                    case 0xBD:
                        type = "Private stream 1";
                        break;
                    case 0xBE:
                        type = "Padding stream";
                        break;
                    case 0xBF:
                        type = "Private stream 2";
                        break;
                    case 0xF0:
                        type = "ECM Stream";
                        break;
                    case 0xF1:
                        type = "EMM Stream";
                        break;
                    case 0xF2:
                        type = "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Annex A or ISO/IEC 13818-6_DSMCC_stream";
                        break;
                    case 0xF3:
                        type = "ISO/IEC_13522_stream";
                        break;
                    case 0xF4:
                        type = "ITU-T Rec. H.222.1 type A";
                        break;
                    case 0xF5:
                        type = "ITU-T Rec. H.222.1 type B";
                        break;
                    case 0xF6:
                        type = "ITU-T Rec. H.222.1 type C";
                        break;
                    case 0xF7:
                        type = "ITU-T Rec. H.222.1 type D";
                        break;
                    case 0xF8:
                        type = "ITU-T Rec. H.222.1 type E";
                        break;
                    case 0xF9:
                        type = "Ancillary stream";
                        break;
                    case 0xFA - 0xFE:
                        type = "Reserved";
                        break;
                    case 0xFF:
                        type = "Program Stream Directory";
                        break;
                    default:
                        if ((HeaderCode >= 0x01) && (HeaderCode <= 0xAF))
                            type = "Slice " + HeaderCode.ToString("X2");
                        if ((HeaderCode >= 0xC0) && (HeaderCode <= 0xDf))
                            type = "MPEG-1 or MPEG-2 audio stream";

                        if ((HeaderCode >= 0xE0) && (HeaderCode <= 0xEf))
                            type = "MPEG-1 or MPEG-2 video stream";
                        break;
                }
                return type;
            }
        }
    }
    /// <summary>
    /// Fully decoded
    /// </summary>
    public class SequenceHeader : Bloc
    {
        private string[] AspectRatioData = new string[] { "Forbidden", "1:1", "4/3", "16:9", "2.21:1" };
        private string[] FrameRateData = new string[] { "Forbidden", "23.976", "24", "25", "29,97", "30", "50", "59,94", "60" };

        public List<GOP> gops = new List<GOP>();
        public Extension SequenceExtension;
        private Size s;
        private string aspectRatio;
        private string framerate;
        private int bitRate;
        private int VBV_buffer_size;
        private bool constrained_parameters_flag;
        private bool load_intra_quantiser_matrix;
        private bool load_non_intra_quantiser_matrix;
        #region Properties
        public Size Size(byte[] data)
        {
            byte mid = data[1];
            int w = data[0] * 16 + (mid & 0xf0) / 16;
            int h = (mid & 0x0f) * 256 + data[2];
            return new Size(w, h); ;
        }
        public string FindAspectRatio(byte b)
        {
            int a = (b & 0xF0) / 16;
            return AspectRatioData[a];
        }
        public string FindFrameRate(byte b)
        {
            int a = (b & 0x0F);
            return FrameRateData[a];
        }
        public Size VideoSize
        {
            get { return s; }
            set { s = value; }
        }
        public string AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; }
        }
        public string FrameRate
        {
            get { return framerate; }
            set { framerate = value; }
        }
        public int BitRate
        {
            get { return bitRate; }
            set { bitRate = value; }
        }
        #endregion
        public bool Constrained_parameters_flag
        {
            get { return constrained_parameters_flag; }
            set { constrained_parameters_flag = value; }
        }
        public bool Load_intra_quantiser_matrix
        {
            get { return load_intra_quantiser_matrix; }
            set { load_intra_quantiser_matrix = value; }
        }
        public bool Load_non_intra_quantiser_matrix
        {
            get { return load_non_intra_quantiser_matrix; }
            set { load_non_intra_quantiser_matrix = value; }
        }
        public SequenceHeader(IBloc b)
        {
            Data = b.Data;
            Offset = b.Offset;
            Header = b.Header;
            s = Size(Data);
            framerate = FindFrameRate(Data[3]);
            aspectRatio = FindAspectRatio(Data[3]);
            bitRate = (Data[4] * 256 + Data[5]) * 4 + (Data[6] & 0xC0 / 0xC0);
            VBV_buffer_size = ((Data[6] & 0x1F) << 5) + ((Data[7] & 0xF8) >> 3);
            constrained_parameters_flag = ((Data[7] & 0x04) == 0x04);
            load_intra_quantiser_matrix = ((Data[7] & 0x02) == 0x02);
            load_non_intra_quantiser_matrix = ((Data[7] & 0x01) == 0x01);
        }
        public override string ToString()
        {
            return base.ToString() + " " + s.ToString() + " " + aspectRatio + " " + framerate; ;
        }
    }
    public class GOP : Bloc
    {
        public Size s;
        public string aspectRatio;
        public string framerate;
        public bool dropframeflag;
        public int hour;
        public int minute;
        public int second;
        public int frame;
        private bool closedGOP;
        private bool brokenGOP;
        private List<Picture> pictures = new List<Picture>();
        #region Properties
        public int NumberOfSlices
        {
            get
            {
                int sum = 0;
                foreach (Picture p in pictures)
                    sum += p.Slices.Count;
                return sum;
            }
        }
        public string Time
        {
            get
            {
                return hour.ToString("d2") + ":" + minute.ToString("d2") + ":" + second.ToString("d2") + "FR" + frame.ToString("d2");
            }
        }
        public bool ClosedGOP
        {
            get { return closedGOP; }
            set { closedGOP = value; }
        }
        public bool BrokenGOP
        {
            get { return brokenGOP; }
            set { brokenGOP = value; }
        }
        public List<Picture> Pictures
        {
            get { return pictures; }
            set { pictures = value; }
        }
        #endregion
        public List<int> tempOrder = new List<int>();
        private int[] DecodeGop(byte[] b)
        {
            int[] data = new int[4];
            hour = (b[0] & 0x7c) / 4;
            minute = (b[0] & 0x03) * 16 + (b[1] & 0xF0) / 16;
            second = (b[1] & 0x07) * 8 + (b[2] & 0xE0) / 0x20;
            frame = (b[2] & 0x1F) * 2 + (b[3] & 0x80) / 0x80;
            return data;
        }
        public override string ToString()
        {
            int frate = int.Parse(framerate);
            float secondes = (float)pictures.Count / (float)frate;
            string cl = "";
            if (closedGOP)
                cl = "closed GOP";
            return "GOP " + Offset.ToString("X") + " " + cl + " " + Time + " " + pictures.Count + " images soit " + NumberOfSlices.ToString() + " slices et " + secondes.ToString() + " secondes ";
        }
        public GOP(IBloc bl)
        {
            Data = bl.Data;
            Offset = bl.Offset;
            Header = bl.Header;
            DecodeGop(Data);
        }
    }
    public class Extension : Bloc
    {
        string extT = "";
        public int extensionType;
        public Extension(IBloc b)
        {
            Data = b.Data;
            Offset = b.Offset;
            Header = b.Header;
        }
        public int detail()
        {
            int toRead = 0;
            try
            {
                switch (extensionType)
                {
                    case 1:
                        extT = "Sequence_Extension";
                        int profileAndLevel = ((Data[0] & 0x0F) << 4) + ((Data[1] & 0xF0) >> 4);
                        bool progressive_sequence = ((Data[1] & 0x08) == 0x08);
                        int chroma_format = (Data[1] & 0x06) / 2;
                        int horizontal_size_extension = (Data[1] & 0x01) * 2 + (Data[2] & 0x0C) / 0x0C;
                        int vertical_size_extension = (Data[2] & 0x60) / 0x60;
                        int bit_rate_extension;
                        int vbv_buffer_size_extension;
                        int low_delay;
                        int frame_rate_extension_n;
                        int frame_rate_extension_d;
                        break;
                    case 2:
                        extT = "Sequence_Display_Extension";
                        int video_format;
                        bool color_description_flag = false;
                        if (color_description_flag)
                            toRead = 3;
                        int color_primaries;
                        int transfer_characteristics;
                        int matrix_coefficients;
                        int display_horizontal_size;
                        int display_vertical_size;

                        break;
                    case 8:
                        extT = "Picture_Coding_Extension";
                        int intra_DC_precision = (Data[2] & 0x0C) >> 2;
                        int picture_structure = (Data[2] & 0x03);
                        bool Top_Field_First = ((Data[3] & 0x80) == 0x80);
                        bool frame_pred_frame_dct = ((Data[3] & 0x40) == 0x40);
                        bool concealment_motion_vectors = ((Data[3] & 0x20) == 0x20);
                        bool q_scale_type = ((Data[3] & 0x10) == 0x10);
                        bool intra_vlc_format = ((Data[3] & 0x08) == 0x08);
                        bool alternate_scan = ((Data[3] & 0x04) == 0x04);
                        bool Repeat_First_Field = ((Data[3] & 0x02) == 0x02);
                        bool chroma_420_type = ((Data[3] & 0x01) == 0x01);
                        bool progressive_frame = ((Data[4] & 0x80) == 0x80);
                        bool composite_display = ((Data[4] & 0x40) == 0x40);
                        if (composite_display)
                        {
                            toRead = 2;
                            bool v_axis = ((Data[4] & 0x20) == 0x20);
                            int field_sequence;
                            bool sub_carrier;
                            int burst_amplitude;
                            int sub_carrier_phase;
                        }
                        int[,] f_code = new int[2, 2];
                        f_code[0, 0] = Data[0] & 0x0F;// forward horizontal
                        f_code[0, 1] = (Data[1] & 0xF0) >> 4; //forward vertical
                        f_code[1, 0] = Data[1] & 0x0F;//backward horizontal
                        f_code[1, 1] = (Data[2] & 0x0F) >> 4; //backward vertical

                        break;
                }
            }
            catch { }
            return toRead;
        }
        public override string ToString()
        {

            return extT + " " + base.ToString();
        }
    }
    public class VideoStream : Bloc
    {
        public IBloc PackHeader;
        public byte[] extensionData;
        public byte[] headerData;
        public byte[] payload;
        public byte[] unknown;
        public int PES_scrambling_control;
        public bool PES_priority;
        public bool data_alignment_indicator, copyright, original_copy;
        public int PTS_DTS_flags;
        public bool ESCR_flag, ES_rate_flag, DSM_trick_mode_flag, additional_copy_info_flag, PES_CRC_flag, PES_extension_flag;
        public int PES_header_data_length;
        public List<byte[]> PTS_DTS_Buffer = new List<byte[]>();
        public long PTS;
        public long DTS;
        public void PTS_DTS_Decode()
        {
            PTS = DecodePts_dts(PTS_DTS_Buffer[0]);
            if (PTS_DTS_Buffer.Count == 2)
            {
                DTS = DecodePts_dts(PTS_DTS_Buffer[1]);
            }
        }
        private long DecodePts_dts(byte[] pts)
        {
            long p = (pts[0] & 0x0E << 29) + ((pts[1] & 0x7F) << 21) + ((pts[2] & 0xFE) << 14) + (pts[3] << 7) + ((pts[4] & 0xFE) >> 1);
            return p / 90;
        }
        public VideoStream(IBloc bl, byte[] extensionData)
        {
            Data = bl.Data;
            Offset = bl.Offset;
            Header = bl.Header;
            this.extensionData = extensionData;
            PES_scrambling_control = (extensionData[0] & 0x30) >> 4;
            PES_priority = (extensionData[0] & 0x08) == 0x08;
            data_alignment_indicator = (extensionData[0] & 0x04) == 0x04;
            copyright = (extensionData[0] & 0x02) == 0x02;
            original_copy = (extensionData[0] & 0x01) == 0x01;
            PTS_DTS_flags = (extensionData[1] & 0xc0) >> 6;
            ESCR_flag = (extensionData[1] & 0x20) == 0x20;
            ES_rate_flag = (extensionData[1] & 0x20) == 0x20;
            DSM_trick_mode_flag = (extensionData[1] & 0x08) == 0x08;
            additional_copy_info_flag = (extensionData[1] & 0x04) == 0x04;
            PES_CRC_flag = (extensionData[1] & 0x02) == 0x02;
            PES_extension_flag = (extensionData[1] & 0x01) == 0x01;
            PES_header_data_length = extensionData[2];
        }
        public override string ToString()
        {
            string uk = "";
            if (unknown != null)
                uk = " Unknown " + unknown.Length.ToString();
            return base.ToString() + " Payload " + payload.Length.ToString() + uk;
        }
    }
    public class SystemHeader : Bloc
    {
        public long rate_bound; // 22-bit unsigned integer. Must be greater than or equal to (>=) the maximum value of the program_mux_rate coded in any pack of the program stream. For DVD-Video this value should be 25200 decimal. 
        public long audio_bound;// 6-bit unsigned integer, ranging from 0 to 32 inclusive. Must be greater than or equal to (>=) the maximum number of audio streams in the program stream. ISO 13818-1 states this should be the MPEG audio streams, but DVD-Video counts all audio streams. For DVD-Video this should be the number of audio streams of any type, from 0 to 8 inclusive. 
        public bool fixed_flag;// 1-bit boolean. If TRUE (1) the program stream is multiplexed at a fixed bitrate. For DVD-Video this flag should be FALSE (0). 
        public bool CSPS_flag;// 1-bit boolean. If TRUE (1) the program stream meets the requirements of a "Constrained System parameter program Stream". For DVD-Video this flag must be FALSE (0). 
        public bool system_audio_lock_flag;// 1-bit boolean. TRUE (1) indicates that there is a specified constant rational relationship between the audio sampling rate and the system_clock_frequency (27MHz). For DVD-Video this flag should be TRUE (1). 
        public bool system_video_lock_flag;// 1-bit boolean. TRUE (1) indicates that there is a specified constant rational relationship between the video picture rate and the system_clock_frequency (27MHz). For DVD-Video this flag should be TRUE (1).  The PAL/SECAM ratio is 1080000 system clocks (3600 90KHz clocks) per displayed picture. The NTSC ratio is 900900 system clocks (3003 90KHz clocks) per displayed picture. This rate differs slightly from the nominal rate for NTSC, but is fixed, and consistent with ITU-601. 
        public long video_bound;// 5-bit unsigned integer, ranging from 0 to 16 inclusive. Must be greater than or equal to (>=) the maximum number of video streams in the program stream. For DVD-Video this value will always be 1. 
        public bool packet_rate_restriction_flag;// 1-bit boolean. If CSPS_flag is TRUE (1) this specifies which restraint is applicable to the packet rate, otherwise the flag has no meaning. For DVD-Video this flag must be FALSE (0). 
        public int header_length;
        public List<Stream_bound_Entry> stes = new List<Stream_bound_Entry>();
        public SystemHeader(IBloc bl)
        {
            Data = bl.Data;
            Offset = bl.Offset;
            Header = bl.Header;
            #region rate bound
            int a = Data[0] & 0x7F;
            int b = Data[1];
            int c = Data[2] & 0xFE;
            rate_bound = c + (b + a * 256) * 256;
            rate_bound = rate_bound >> 1;
            #endregion
            #region audio bound
            audio_bound = (Data[3] & 0xFC) >> 2;
            #endregion
            fixed_flag = ((Data[3] & 0x02) == 0x02);
            CSPS_flag = ((Data[3] & 0x01) == 0x01);
            system_audio_lock_flag = ((Data[4] & 0x80) == 0x80);
            system_video_lock_flag = ((Data[4] & 0x40) == 0x40);
            #region video bound
            video_bound = Data[4] & 0x1F;
            #endregion
            packet_rate_restriction_flag = ((Data[5] & 0x80) == 0x80);
            if (Data.Length > 6)
            {
                int i = 6;
                while (i < Data.Length)
                {
                    Stream_bound_Entry ste = new Stream_bound_Entry();
                    ste.code = Data[i];
                    ste.P_STD_buffer_bound_scale = ((Data[i + 1] & 0x20) == 0x20);
                    ste.P_STD_buffer_size_bound = Data[i + 2] + (Data[i + 1] & 0x1F) * 256;
                    stes.Add(ste);
                    switch (Data[i])
                    {
                        case 0xB9:
                            break;
                        case 0xB8:
                            break;
                        case 0xBD:
                            break;
                        case 0xBF:
                            break;
                        default:
                            break;
                    }
                    i += 3;
                }
            }
        }
        public override string ToString()
        {
            return base.ToString() + " " + rate_bound.ToString() + " video " + video_bound.ToString() + " audio " + audio_bound.ToString();
        }
    }
    public class Stream_bound_Entry: LOCALIZED_DATA
    {
        public int code;
        public bool P_STD_buffer_bound_scale;// False (0) indicates the multiplier is 128, TRUE (1) indicates the multiplier is 1024
        public long P_STD_buffer_size_bound;// with multiplier
        public override string ToString()
        {
            return code.ToString("X2") + " " + P_STD_buffer_bound_scale.ToString() + " " + P_STD_buffer_size_bound.ToString();
        }
    }
    public class PackHeader : Bloc
    {
        /*SCR and SCR_ext together are the System Clock Reference, a counter driven at 27MHz, used as a reference to synchronize streams. The clock is divided by 300 (to match the 90KHz clocks such as pTS/dTS), the quotient is SCR (33 bits), the remainder is SCR_ext (9 bits) 
Program_Mux_Rate -- This is a 22 bit integer specifying the rate at which the program stream target decoder receives the program Stream during the pack in which it is included. The value of program_mux_rate is measured in units of 50 bytes/second. The value 0 is forbidden. */
        private double src;
        private int program_Mux_Rate;
        #region Properties
        public double SRC
        {
            get { return src; }
            set { src = value; }
        }
        public int Program_Mux_Rate
        {
            get { return program_Mux_Rate; }
            set { program_Mux_Rate = value; }
        }
        #endregion
        private void ComputeSrc()
        {
            /*			SCR=ret_bit_value(2,4,false);		// SCR(32..30)
			scr_val=(SCR>3)?4294967296.0:0.0;	// caz pt. SCR[32]=1 -> overflow
			SCR=SCR<<30;
			SCR|=ret_bit_value(6,7,true)<<28;	// SCR(29,28)
			SCR|=ret_value(1)<<20;				// SCR(27..20)
			SCR|=ret_bit_value(0,4,false)<<15;	// SCR(19..15)
			SCR|=ret_bit_value(6,7,true)<<13;	// SCR(14,13)
			SCR|=ret_value(1)<<5;				// SCR(12..5)
			SCR|=ret_bit_value(0,4,false);		// SCR(4..0)
			scr_val+=Convert.ToDouble(SCR);
			scr_val*=300.0;
			SCR=ret_bit_value(6,7,true)<<7;		// SCR_ext(8,7)
			SCR|=ret_bit_value(0,6,true);		// SCR_ext(6..0)
			scr_val+=Convert.ToDouble(SCR);
*/
        }
        public PackHeader(IBloc bl)
        {
            Data = bl.Data;
            Offset = bl.Offset;
            Header = bl.Header;
            // computes System Clock Reference : to be checked !!
            long SRC30 = (Data[0] & 0x38) << 30;// 3 bits
            long SRC15 = ((Data[0] & 0x03) << 28) + (Data[1] << 20) + ((Data[2] & 0xF8) << 15);// 15 bits
            long SRC0 = ((Data[2] & 0x03) << 13) + (Data[3] << 5) + ((Data[4] & 0xF8) >> 3); // 14 bits
            src = Convert.ToDouble(SRC30 + SRC15 + SRC0);
            src *= 300;
            int SRCEXT = ((Data[4] & 0x03) << 7) + ((Data[5] & 0xFE) >> 1);
            double k = Convert.ToDouble(SRCEXT);
            src += k;
            src = src / 27000.0;
            program_Mux_Rate = ((Data[6] * 256 + Data[7]) << 6) + ((Data[8] & 0xFC) >> 2);
            //     program_Mux_Rate = program_Mux_Rate * 50;//bytes per second  
        }
        public override string ToString()
        {
            return base.ToString() + " " + src.ToString() + " " + program_Mux_Rate.ToString();
        }
    }
    public class Picture : Bloc
    {
        #region Properties
        public List<IBloc> Slices
        {
            get { return slices; }
            set { slices = value; }
        }
        public string ImageType
        {
            get { return imageType; }
            set { imageType = value; }
        }
        public int Temporal_sequence_number
        {
            get { return temperal_sequence_number; }
            set { temperal_sequence_number = value; }
        }
        #endregion
        private List<IBloc> slices = new List<IBloc>();
        public Extension pictureExtension;
        private int temperal_sequence_number;

        public int VBV_delay;
        private string imageType;

        public Picture(IBloc bl)
        {
            Data = bl.Data;
            Offset = bl.Offset;
            Header = bl.Header;
            int ai = (Data[1] & 56) / 8;
            switch (ai)
            {
                case 1:
                    imageType = "I";
                    break;
                case 2:
                    imageType = "P";
                    break;
                case 3:
                    imageType = "B";
                    break;
                case 4:
                    imageType = "D";
                    break;
            }
            temperal_sequence_number = (Data[0] << 2) + ((Data[1] & 0xC0) >> 6);
            VBV_delay = ((Data[1] & 0x07) * 256 + Data[2]) * 8 + (Data[3] & 0xF8) / 8;
        }
        public override string ToString()
        {
            temperal_sequence_number = (Data[0] << 2) + ((Data[1] & 0xC0) >> 6);
            return imageType + " " + temperal_sequence_number.ToString() + " " + base.ToString();
        }
    }
    public class Slice : Bloc
    {
        private int quantiserScaleCode;
        private bool sliceExtensionFlag;

        public int QuantiserScaleCode
        {
            get { return quantiserScaleCode; }
            set { quantiserScaleCode = value; }
        }
        public bool SliceExtensionFlag
        {
            get { return sliceExtensionFlag; }
            set { sliceExtensionFlag = value; }
        }
        public Slice(IBloc bl)
        {
            Data = bl.Data;
            Offset = bl.Offset;
            Header = bl.Header;
            quantiserScaleCode = (Data[0] & 0xF8) / 8;
            sliceExtensionFlag = ((Data[0] & 0x04) / 4 == 1);
        }
        public override string ToString()
        {
            return base.ToString() + " " + Data.Length.ToString("X") + " " + sliceExtensionFlag.ToString() + " " + quantiserScaleCode.ToString();
        }
    }
    public interface IBloc
    {
        byte[] Header { get; set; }
        long HeaderCode { get; }
        long Offset { get; set; }
        byte[] Data { get; set; }
    }
 
}
