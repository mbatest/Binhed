using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.ComponentModel;
using Utils;
using System.Diagnostics;
namespace BluRay
{
    //http://hirntier.blogspot.com/2010/02/avchd-timecode-update.html
    // http://owl.phy.queensu.ca/~phil/exiftool/TagNames/M2TS.html
    // Trouver les types de frames I B P ???
    public class M2TS : LOCALIZED_DATA
    {
        [CategoryAttribute("M2TS"), DescriptionAttribute("List of image dates")]
        public List<string> Dates
        {
            get { return dates; }
        }
        List<DataStream> videostreams = new List<DataStream>();
        public List<DataStream> Videostreams
        {
            get { return videostreams; }
            set { videostreams = value; }
        }
        List<DataStream> audiostreams = new List<DataStream>();
        public List<DataStream> Audiostreams
        {
            get { return audiostreams; }
            set { audiostreams = value; }
        }
        DataStream CurrentVideoStream;
        DataStream CurrentAudioStream;
        List<string> dates = new List<string>();
        private List<Program> programs = new List<Program>();
        private List<int> progIds = new List<int>();
        public List<Program> Programs
        {
            get { return programs; }
            set { programs = value; }
        }
        List<IM2TSPacket> blocs = new List<IM2TSPacket>();
        List<byte> tags = new List<byte>();
        public M2TS(string Filename)
        {
            BinaryFileReader sw = new BinaryFileReader(Filename, FileMode.Open, FileAccess.Read, FileShare.Read, false);
            byte[] buffer;
            while (sw.Position < sw.Length)
            {

                #region First bloc, Program association table;
                long offset = sw.Position;
                buffer = sw.ReadBytes(0xC0);   //Packet size 192 (0xC0) bytes
                PAT pat = new PAT(buffer, offset);
                blocs.Add(pat);
                Program p = new Program(pat.PID, "Program Association Table", pat);
                programs.Add(p);
                progIds.Add(p.PID);
                foreach (Program u in pat.Programs)
                {
                    programs.Add(u);
                    progIds.Add(u.PID);
                }
                PMT pmt = new PMT(sw.ReadBytes(0xC0), offset);
                p = new Program(pmt.PCR_PID, "PCR", null);
                programs.Add(p);
                progIds.Add(p.PID);
                foreach (ElementaryStream u in pmt.ElementaryStreams)
                {
                    programs.Add(new Program(u.PID, u.Stream, null));
                    progIds.Add(u.PID);
                }
                blocs.Add(pmt);
                NIT Nit = new NIT(sw.ReadBytes(0xC0), offset);
                blocs.Add(Nit);
                int ind = progIds.IndexOf(Nit.PID);
                if (ind < 0)
                {
                    p = new Program(Nit.PID, "Network Information Table", Nit);
                    programs.Add(p);
                    progIds.Add(p.PID);
                }
                {
                    programs[ind].Blocs.Add(Nit);
                }
             
                while ((sw.Position < sw.Length))
                {
                    offset = sw.Position;
                    if (sw.Position >= 0x780)
                    {
                    }
                    if (offset >= 0x6C0)
                    {
                    }
                     buffer = sw.ReadBytes(0xC0);
                    int PID = FinPid(buffer);
                    string t = null;
                    foreach (Program pp in programs)
                    {
                        if (pp.PID == PID)
                        {
                            t = pp.ProgType;
                            break;
                        }
                    }
                    IM2TSPacket bl = null;
                   switch (PID)
                    {
                        case 0x000:
                            bl = new PAT(buffer, offset);
                            break;
                        case 0x100:
                            bl = new PMT(buffer, offset);
                            break;
                        case 0x01F:
                            bl = new NIT(buffer, offset);
                            break;
                        default:
                            #region Generic case
                            switch (t)
                            {
                                case "PCR":
                                    bl = new PCR(buffer, offset);
                                    break;
                                case "Video":
                                    bl = new VideoPacket(buffer, offset);
                                    if (((VideoPacket)bl).FrameDate != null)
                                        dates.Add(((VideoPacket)bl).FrameDate);
                                    if (((VideoPacket)bl).startPacket)
                                    {
                                        CurrentVideoStream = new DataStream();
                                        videostreams.Add(CurrentVideoStream);
                                    }
                                    if (CurrentVideoStream != null)
                                        CurrentVideoStream.AddPacket(bl);
                                    break;
                                case "Audio":
                                    try
                                    {
                                        if (offset >= 0xF7E6C0)
                                        {
                                        }
                                        bl = new AudioPacket(buffer, offset);
                                        if (((AudioPacket)bl).startPacket)
                                        {
                                            CurrentAudioStream = new DataStream();
                                            audiostreams.Add(CurrentAudioStream);
                                        }
                                        if (CurrentAudioStream != null)
                                            CurrentAudioStream.AddPacket(bl);
                                    }
                                    catch (Exception ex) { }
                                    break;
                                default:
                                    bl = new M2TSPacket(buffer, offset);
                                    break;
                            }
                            #endregion
                            break;
                    }
                    blocs.Add(bl);
                    Trace.WriteLine(bl);
                    ind = progIds.IndexOf(bl.PID);
                    if (ind < 0)
                    {
                        string blocType = "";
                        p = new Program(bl.PID, blocType, bl);
                        programs.Add(p);
                        progIds.Add(bl.PID);
                    }
                    else
                    {
                        programs[ind].Blocs.Add(bl);
                    }
                }
                #endregion
            }
             FileInfo f = new FileInfo(Filename);
            //          long frameSize = f.Length / blocNb.Count;
        }
        private int FinPid(byte[] b)
        {
            int syncByte = b[4];
            return ((b[5] & 0x1F) << 8) + b[6];
        }

     }
    /*
     * Access unit delimiter 0x00000109
     * 0x000001 NAL start prefix 
    0 
    1.2  NAL Ref IDC
    3.7  Nal Unit type
     * 0 Unspecified
     * 1 Coded slice
     * 2 Data Partition A
     * 3 Data Partition B
     * 4 Data Partition C
     * 5 IDR (coded slice)
     * 6 SEI
     * 7 SPS sequence parameter ser
     * 8 PPS picture ...
     * 9 Acces unit delimiter
     * 10 EoS end of sequence
     * 11 EoS end of stream
     * 
     */
    /**
   *   NAL Units start code: 00 00 01 X Y
   *   X =  IDR Picture NAL Units (25, 45, 65)
   *   X = Non IDR Picture NAL Units (01, 21, 41, 61) ; 01 = buffer-frames, 41 = p-frames
   *   since frames can be splitted over multiple NAL Units only count the NAL Units with Y > 0x80
   **/
    public class NAL : LOCALIZED_DATA
    {
        private List<MAttribute> attributes;
        public List<MAttribute> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
        string date;

        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        int code;
        public byte[] startCode;
        public int offset;
        public int blocNumber;
        public int tag;
        public byte[] buffer;
        public List<Tag> tags = new List<Tag>();
        public string NalType
        {
            get
            {
                string t = "";
                if ((code >= 0xe0) && (code <= 0xef))
                    return  "Video stream";
                else
                if ((code >= 0xc0) && (code <= 0xcf))
                    return  "Audio stream";
                else
                switch(code)
                {
                    case 0: t= "Unspecified";
                        break;
                    case 1: t= "Coded slice";
                        break;
                    case 2: t= "Data Partition A";
                        break;
                    case 3: t= "Data Partition B";
                        break;
                    case 4: t= "Data Partition C";
                        break;
                    case 5: t= "IDR (coded slice)";
                        break;
                    case 6: t= "SEI";
                        break;
                    case 7: t= "SPS sequence parameter set";
                        break;
                    case 8: t= "PPS picture parameter set";
                        break;
                    case 9: t= "Acces unit delimiter";
                        break;
                    case 10: t= "EoS end of sequence";
                        break;
                    case 11: t = "EoS end of stream";
                        break;
                    default :
                        break;
                }
                return t;
            }
        }
        public NAL(int offset, int blocNumber, byte[] s, byte[] data)
        {
            int offCode = 4;
            startCode = s;
            if ((startCode[offCode] >= 0xC0))
                code = startCode[offCode];
            else
                code = startCode[offCode] & 0x1F;
            this.buffer = data;
            this.offset = offset;
            this.blocNumber = blocNumber;
            switch (code)
            {
                case 5: // Slice
                    break;
                case 9: // Acces unite delimiter
                    // To decode
                    break;
                case 6: // SEI
                    tag = data[0];
                    AddAttribute(new MAttribute("Tag", tag));
                    switch (tag)
                    {
                        case 5:
                            byte[] b = new byte[0x10];
                            Buffer.BlockCopy(buffer, 2, b, 0, 0x10);
                            Guid gu = new Guid(b);
                            Guid guid = new Guid(new byte[] { 0x17, 0xee, 0x8c, 0x60, 0xf8, 0x4d, 0x11, 0xd9, 0x8c, 0xd6, 0x08, 0x00, 0x20, 0x0c, 0x9a, 0x66 });
                            if (gu == guid)
                            {
                                #region Find tags
                                /*
       AVCHD metadata are stored in an SEI message of mPID 5 (user buffer unregistered). These messages start with a GUID (indicating the mPID of the buffer). The rest is not specified in the H.264 spec. For AVCHD metadata the buffer structure is as follows:
       1. The 16 byte GUID, which consists of the bytes
       0x17 0xee 0x8c 0x60 0xf8 0x4d 0x11 0xd9 0x8c 0xd6 0x08 0x00 0x20 0x0c 0x9a 0x66
       2. 4 bytes
       0x4d 0x44 0x50 0x4d which are "MDPM" in ASCII.
       3. One byte, which specifies the number of tags to follow
       4. Each tag begins with one byte specifying the tag mPID followed by 4 bytes of buffer.
       The date and time are stored in tags 0x18 and 0x19.
       Tag 0x18 starts with an unknown byte. I saw values between 0x02 and 0xff in various files. It seems however that it has a constant value for all frames in a file. The 3 remaining bytes are the year and the month in BCD coding (0x20 0x09 0x08 means August 2009).
       The 4 bytes in tag 0x19 are the day, hour, minute and second (also BCD coded). -> temps universel ?
       There are more informations stored in this SEI message, check here for a list.
       If you want to make further research on this, you can download gmerlin-avdecoder from CVS, open the file lib/parse_h264.cCode and uncomment the following line (at the very beginning):
        */

                                b = new byte[0x4];
                                Buffer.BlockCopy(buffer, 0x12, b, 0, 0x4);
                                string Mpdm = Encoding.Default.GetString(b);
                                int i = 0x016;
                                int nb = buffer[i];
                                i++;
                                for (int u = 0; u < nb; u++)
                                {
                                    int tagId = buffer[i];
                                    byte[] tg = new byte[4];
                                    Buffer.BlockCopy(buffer, i + 1, tg, 0, 4);
                                    Tag T = new Tag(tagId, tg);
                                    tags.Add(T);
                                    if (tagId == 0x18)
                                        date = T.date;
                                    if (tagId == 0x19)
                                        date += T.date;

                                    i += 5;
                                }
                                AddAttribute(new MAttribute("Date", date));
                                #endregion
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case 0x07: //SPS http://www.cardinalpeak.com/blog/?p=878 h264bitstream
                    try
                    {
                        SPS sp = new SPS(buffer);
                        AddAttribute(new MAttribute("Sequence parameters set", sp));
                    }
                    catch { }
                    break;
                case 0x08: //PPS
                    try
                    {
                        PPS pp = new PPS(buffer);
                        AddAttribute(new MAttribute("Picture parameters set", pp));
                    }
                    catch { }
                    break;
            }
        }      
        public override string ToString()
        {
            return NalType + " Bloc " + blocNumber.ToString() + " Offset " + offset.ToString();
        }
        private void AddAttribute(MAttribute attr)
        {
            if (attributes == null)
                attributes = new List<MAttribute>();
            attributes.Add(attr);
        }
    }
    public class SPS : LOCALIZED_DATA
    {
        private int profile_idc;
        public int Profile_idc
        {
            get { return profile_idc; }
            set { profile_idc = value; }
        }
        public bool constraint_set0_flag;
        public bool constraint_set1_flag;
        public bool constraint_set2_flag;
        public bool constraint_set3_flag;
        public int reserved_zero_4bits;
        public int level_idc;
        public int seq_parameter_set_id;
        public int chroma_format_idc;
        public bool residual_colour_transform_flag;
        public int bit_depth_luma_minus8;
        public int bit_depth_chroma_minus8;
        public bool qpprime_y_zero_transform_bypass_flag;
        public bool seq_scaling_matrix_present_flag;
        public bool[] seq_scaling_list_present_flag = new bool[6];
        public int[] ScalingList4x4 = new int[6];
        public bool[] UseDefaultScalingMatrix4x4Flag = new bool[6];
        public int[] ScalingList8x8 = new int[2];
        public bool[] UseDefaultScalingMatrix8x8Flag = new bool[2];
        public int log2_max_frame_num_minus4;
        public int pic_order_cnt_type;
        public int log2_max_pic_order_cnt_lsb_minus4;
        public bool delta_pic_order_always_zero_flag;
        public int offset_for_non_ref_pic;
        public int offset_for_top_to_bottom_field;
        public int num_ref_frames_in_pic_order_cnt_cycle;
        public int[] offset_for_ref_frame = new int[256];
        public int num_ref_frames;
        public bool gaps_in_frame_num_value_allowed_flag;
        public int pic_width_in_mbs_minus1;
        public int pic_height_in_map_units_minus1;
        public bool frame_mbs_only_flag;
        public bool mb_adaptive_frame_field_flag;
        public bool direct_8x8_inference_flag;
        public bool frame_cropping_flag;
        public int frame_crop_left_offset;
        public int frame_crop_right_offset;
        public int frame_crop_top_offset;
        public int frame_crop_bottom_offset;
        public bool vui_parameters_present_flag;
        BitStreamReader bitreader;
        VUI vui = new VUI();
        HRD hrd = new HRD();
        class VUI
        {
            public bool aspect_ratio_info_present_flag;
            public int aspect_ratio_idc;
            public int sar_width;
            public int sar_height;
            public bool overscan_info_present_flag;
            public bool overscan_appropriate_flag;
            public bool video_signal_type_present_flag;
            public int video_format;
            public bool video_full_range_flag;
            public bool colour_description_present_flag;
            public int colour_primaries;
            public int transfer_characteristics;
            public int matrix_coefficients;
            public bool chroma_loc_info_present_flag;
            public int chroma_sample_loc_type_top_field;
            public int chroma_sample_loc_type_bottom_field;
            public bool timing_info_present_flag;
            public int num_units_in_tick;
            public int time_scale;
            public bool fixed_frame_rate_flag;
            public bool nal_hrd_parameters_present_flag;
            public bool vcl_hrd_parameters_present_flag;
            public bool low_delay_hrd_flag;
            public bool pic_struct_present_flag;
            public bool bitstream_restriction_flag;
            public bool motion_vectors_over_pic_boundaries_flag;
            public int max_bytes_per_pic_denom;
            public int max_bits_per_mb_denom;
            public int log2_max_mv_length_horizontal;
            public int log2_max_mv_length_vertical;
            public int num_reorder_frames;
            public int max_dec_frame_buffering;
        }
        struct HRD
        {
            public int cpb_cnt_minus1;
            public int bit_rate_scale;
            public int cpb_size_scale;
            public int[] bit_rate_value_minus1; // up to cpb_cnt_minus1, which is <= 31
            public int[] cpb_size_value_minus1 ;
            public bool[] cbr_flag;
            public int initial_cpb_removal_delay_length_minus1;
            public int cpb_removal_delay_length_minus1;
            public int dpb_output_delay_length_minus1;
            public int time_offset_length;
        }
        public SPS(byte[] buffer)
        {
            bitreader = new BitStreamReader(buffer, false);
            int i;
            profile_idc = bitreader.ReadByte();//.ReadIntFromBits(8) ??
            constraint_set0_flag = bitreader.ReadBool();
            constraint_set1_flag = bitreader.ReadBool();
            constraint_set2_flag = bitreader.ReadBool();
            constraint_set3_flag = bitreader.ReadBool();
            bitreader.ReadBitRange(4);  /* all 0's reserved_zero_4bits */
            level_idc = bitreader.ReadIntFromBits(8);
            seq_parameter_set_id = bitreader.ReadGolomb(); ;
            if (profile_idc == 100 || profile_idc == 110 ||
                profile_idc == 122 || profile_idc == 144)
            {
                chroma_format_idc = bitreader.ReadGolomb();
                if (chroma_format_idc == 3)
                {
                    residual_colour_transform_flag = bitreader.ReadBool();
                }
                bit_depth_luma_minus8 = bitreader.ReadGolomb();
                bit_depth_chroma_minus8 = bitreader.ReadGolomb();
                qpprime_y_zero_transform_bypass_flag = bitreader.ReadBool();
                seq_scaling_matrix_present_flag = bitreader.ReadBool();
                if (seq_scaling_matrix_present_flag)
                {
                    for (i = 0; i < 6; i++)
                    {
                        seq_scaling_list_present_flag[i] = bitreader.ReadBool();
                        if (seq_scaling_list_present_flag[i])
                        {
                            if (i < 6)
                            {
                                read_scaling_list(ScalingList4x4, i, 16, UseDefaultScalingMatrix4x4Flag[i]);
                            }
                            else
                            {
                                read_scaling_list(ScalingList8x8, i - 6, 64, UseDefaultScalingMatrix8x8Flag[i - 6]);
                            }
                        }
                    }
                }
            }
            log2_max_frame_num_minus4 = bitreader.ReadGolomb();
            pic_order_cnt_type = bitreader.ReadGolomb();
            if (pic_order_cnt_type == 0)
            {
                log2_max_pic_order_cnt_lsb_minus4 = bitreader.ReadGolomb();
            }
            else if (pic_order_cnt_type == 1)
            {
                delta_pic_order_always_zero_flag = bitreader.ReadBool();
                offset_for_non_ref_pic = bitreader.se();
                offset_for_top_to_bottom_field = bitreader.se();
                num_ref_frames_in_pic_order_cnt_cycle = bitreader.ReadGolomb();
                for (i = 0; i < num_ref_frames_in_pic_order_cnt_cycle; i++)
                {
                    offset_for_ref_frame[i] = bitreader.se();
                }
            }
            num_ref_frames = bitreader.ReadGolomb();
            gaps_in_frame_num_value_allowed_flag = bitreader.ReadBool();
            pic_width_in_mbs_minus1 = bitreader.ReadGolomb();
            pic_height_in_map_units_minus1 = bitreader.ReadGolomb();
            frame_mbs_only_flag = bitreader.ReadBool();
            if (!frame_mbs_only_flag)
            {
                mb_adaptive_frame_field_flag = bitreader.ReadBool();
            }
            direct_8x8_inference_flag = bitreader.ReadBool();
            frame_cropping_flag = bitreader.ReadBool();
            if (frame_cropping_flag)
            {
                frame_crop_left_offset = bitreader.ReadGolomb();
                frame_crop_right_offset = bitreader.ReadGolomb();
                frame_crop_top_offset = bitreader.ReadGolomb();
                frame_crop_bottom_offset = bitreader.ReadGolomb();
            }
            vui_parameters_present_flag = bitreader.ReadBool();
            if (vui_parameters_present_flag)
            {
                read_vui_parameters();
            }
            //          read_rbsp_trailing_bits(h, b);
        }
        void read_scaling_list(int[] scalingList, int k, int sizeOfScalingList, bool useDefaultScalingMatrixFlag)
        {
            int j;
            // Probablement incorrect
            int lastScale = 8;
            int nextScale = 8;
            for (j = 0; j < sizeOfScalingList; j++)
            {
                if (nextScale != 0)
                {
                    int delta_scale = bitreader.ReadGolomb();
                    nextScale = (lastScale + delta_scale + 256) % 256;
                    useDefaultScalingMatrixFlag = (j == 0) && (nextScale == 0);
                }
                scalingList[j] = (nextScale == 0) ? lastScale : nextScale;
                lastScale = scalingList[j];
            }
        }
        void read_vui_parameters()
        {
            vui.aspect_ratio_info_present_flag = bitreader.ReadBool();
            if (vui.aspect_ratio_info_present_flag)
            {
                vui.aspect_ratio_idc = bitreader.ReadIntFromBits(1);
                if (vui.aspect_ratio_idc == 255 /*SAR_Extended*/)
                {
                    vui.sar_width = bitreader.ReadIntFromBits(2);//bs_read_u(b, 16);
                    vui.sar_height = bitreader.ReadIntFromBits(2);// bs_read_u(b, 16);
                }
            }
            vui.overscan_info_present_flag = bitreader.ReadBool();
            if (vui.overscan_info_present_flag)
            {
                vui.overscan_appropriate_flag = bitreader.ReadBool();
            }
            vui.video_signal_type_present_flag = bitreader.ReadBool();
            if (vui.video_signal_type_present_flag)
            {
                vui.video_format = bitreader.RangeToInt( bitreader.ReadBitRange(3));//bs_read_u(b, 3);
                vui.video_full_range_flag = bitreader.ReadBool();
                vui.colour_description_present_flag = bitreader.ReadBool();
                if (vui.colour_description_present_flag)
                {
                    vui.colour_primaries = bitreader.ReadIntFromBits(1);
                    vui.transfer_characteristics = bitreader.ReadIntFromBits(1);
                    vui.matrix_coefficients = bitreader.ReadIntFromBits(1);
                }
            }
            vui.chroma_loc_info_present_flag = bitreader.ReadBool();
            if (vui.chroma_loc_info_present_flag)
            {
                vui.chroma_sample_loc_type_top_field = bitreader.ReadGolomb();
                vui.chroma_sample_loc_type_bottom_field = bitreader.ReadGolomb();
            }
            vui.timing_info_present_flag = bitreader.ReadBool();
            if (vui.timing_info_present_flag)
            {
                vui.num_units_in_tick = bitreader.ReadIntFromBits(4);// bs_read_u(b, 32);
                vui.time_scale = bitreader.ReadIntFromBits(4);//bs_read_u(b, 32);
                vui.fixed_frame_rate_flag = bitreader.ReadBool();
            }
            vui.nal_hrd_parameters_present_flag = bitreader.ReadBool();
            if (vui.nal_hrd_parameters_present_flag)
            {
                read_hrd_parameters();
            }
            vui.vcl_hrd_parameters_present_flag = bitreader.ReadBool();
            if (vui.vcl_hrd_parameters_present_flag)
            {
                 read_hrd_parameters();
            }
            if (vui.nal_hrd_parameters_present_flag || vui.vcl_hrd_parameters_present_flag)
            {
                vui.low_delay_hrd_flag = bitreader.ReadBool();
            }
            vui.pic_struct_present_flag = bitreader.ReadBool();
            vui.bitstream_restriction_flag = bitreader.ReadBool();
            if (vui.bitstream_restriction_flag)
            {
                vui.motion_vectors_over_pic_boundaries_flag = bitreader.ReadBool();
                vui.max_bytes_per_pic_denom = bitreader.ReadGolomb();
                vui.max_bits_per_mb_denom = bitreader.ReadGolomb();
                vui.log2_max_mv_length_horizontal = bitreader.ReadGolomb();
                vui.log2_max_mv_length_vertical = bitreader.ReadGolomb();
                vui.num_reorder_frames = bitreader.ReadGolomb();
                vui.max_dec_frame_buffering = bitreader.ReadGolomb();
            }
        }
        void read_hrd_parameters()
        {
            int SchedSelIdx;

            hrd.cpb_cnt_minus1 = bitreader.ReadGolomb();
  //          hrd.bit_rate_scale = bs_read_u( 4);
  //          hrd.cpb_size_scale = bs_read_u( 4);
            for (SchedSelIdx = 0; SchedSelIdx <= hrd.cpb_cnt_minus1; SchedSelIdx++)
            {
                hrd.bit_rate_value_minus1[SchedSelIdx] = bitreader.ReadGolomb();
                hrd.cpb_size_value_minus1[SchedSelIdx] = bitreader.ReadGolomb();
                hrd.cbr_flag[SchedSelIdx] = bitreader.ReadBool();
            }
 /*           hrd.initial_cpb_removal_delay_length_minus1 = bs_read_u( 5);
            hrd.cpb_removal_delay_length_minus1 = bs_read_u( 5);
            hrd.dpb_output_delay_length_minus1 = bs_read_u( 5);
            hrd.time_offset_length = bs_read_u( 5);*/
        }
    }
    public class PPS : LOCALIZED_DATA
    {
        public int pic_parameter_set_id;
        public int seq_parameter_set_id;
        public bool entropy_coding_mode_flag;
        public bool pic_order_present_flag;
        public int num_slice_groups_minus1;
        public int slice_group_map_type;
        public int[] run_length_minus1 = new int[8]; // up to num_slice_groups_minus1, which is <= 7 in Baseline and Extended, 0 otheriwse
        public int[] top_left = new int[8];
        public int[] bottom_right = new int[8];
        public bool slice_group_change_direction_flag;
        public int slice_group_change_rate_minus1;
        public int pic_size_in_map_units_minus1;
        public int[] slice_group_id = new int[256]; // FIXME what size?
        public int num_ref_idx_l0_active_minus1;
        public int num_ref_idx_l1_active_minus1;
        public bool weighted_pred_flag;
        public int weighted_bipred_idc;
        public int pic_init_qp_minus26;
        public int pic_init_qs_minus26;
        public int chroma_qp_index_offset;
        public bool deblocking_filter_control_present_flag;
        public bool constrained_intra_pred_flag;
        public bool redundant_pic_cnt_present_flag;
        public bool transform_8x8_mode_flag;
        public bool pic_scaling_matrix_present_flag;
        public bool[] pic_scaling_list_present_flag = new bool[8];
        public int[] ScalingList4x4 = new int[6];
        public bool[] UseDefaultScalingMatrix4x4Flag = new bool[6];
        public int[] ScalingList8x8 = new int[2];
        public bool[] UseDefaultScalingMatrix8x8Flag = new bool[2];
        public int second_chroma_qp_index_offset;
        public PPS(byte[] buffer)
        {            int i;
            int i_group;
            BitStreamReader bitreader = new BitStreamReader(buffer, false);
            pic_parameter_set_id = bitreader.ReadGolomb();
            seq_parameter_set_id = bitreader.ReadGolomb();
            entropy_coding_mode_flag = bitreader.ReadBool();
            pic_order_present_flag = bitreader.ReadBool();
            num_slice_groups_minus1 = bitreader.ReadGolomb();
            if (num_slice_groups_minus1 > 0)
            {
                slice_group_map_type = bitreader.ReadGolomb();
                if (slice_group_map_type == 0)
                {
                    for (i_group = 0; i_group <= num_slice_groups_minus1; i_group++)
                    {
                        run_length_minus1[i_group] = bitreader.ReadGolomb();
                    }
                }
                else if (slice_group_map_type == 2)
                {
                    for (i_group = 0; i_group < num_slice_groups_minus1; i_group++)
                    {
                        top_left[i_group] = bitreader.ReadGolomb();
                        bottom_right[i_group] = bitreader.ReadGolomb();
                    }
                }
                else if (slice_group_map_type == 3 ||
                         slice_group_map_type == 4 ||
                         slice_group_map_type == 5)
                {
                    slice_group_change_direction_flag = bitreader.ReadBool();
                    slice_group_change_rate_minus1 = bitreader.ReadGolomb();
                }
                else if (slice_group_map_type == 6)
                {
                    pic_size_in_map_units_minus1 = bitreader.ReadGolomb();
                    for (i = 0; i <= pic_size_in_map_units_minus1; i++)
                    {
     //                   slice_group_id[index] = bitreader.u(b, ceil(log2(num_slice_groups_minus1 + 1))); // was u(v)
                    }
                }
            }
            num_ref_idx_l0_active_minus1 = bitreader.ReadGolomb();
            num_ref_idx_l1_active_minus1 = bitreader.ReadGolomb();
            weighted_pred_flag = bitreader.ReadBool();
            weighted_bipred_idc = bitreader.ReadIntFromBits(2);
            pic_init_qp_minus26 = bitreader.se();
            pic_init_qs_minus26 = bitreader.se();
            chroma_qp_index_offset = bitreader.se();
            deblocking_filter_control_present_flag = bitreader.ReadBool();
            constrained_intra_pred_flag = bitreader.ReadBool();
            redundant_pic_cnt_present_flag = bitreader.ReadBool();
            /*
            if (more_rbsp_data(h, b))
            {
                transform_8x8_mode_flag = bitreader.ReadBool();
                pic_scaling_matrix_present_flag = bitreader.ReadBool();
                if (pic_scaling_matrix_present_flag)
                {
                    for (index = 0; index < 6 + 2 * transform_8x8_mode_flag; index++)
                    {
                        pic_scaling_list_present_flag[index] = bitreader.ReadBool();
                        if (pic_scaling_list_present_flag[index])
                        {
                            if (index < 6)
                            {
         //                       read_scaling_list(b, ScalingList4x4[index], 16, UseDefaultScalingMatrix4x4Flag[index]);
                            }
                            else
                            {
          //                      read_scaling_list(b, ScalingList8x8[index - 6], 64, UseDefaultScalingMatrix8x8Flag[index - 6]);
                            }
                        }
                    }
                }
                second_chroma_qp_index_offset = bitreader.se;
            }
              */
            //                read_rbsp_trailing_bits(h, b);
        }
    }
    public class MTable : M2TSPacket
    {
        public int Table_ID
        {
            get { return table_ID; }
        }
        int PointerField; //Present if payload_unit_start_indicator bit is set in the TS header bytes. Gives the number of bytes from the end of this field to the start of payload buffer
        public int table_ID;
        public MTable(byte[] b, long o)
            : base(b, o)
        {
            blocType = "Network Information Table";
            if (payload_Unit_Start_Indicator)
            {
                PointerField = breader.ReadByte();
            }
            table_ID = breader.ReadByte();
        }
   }
    public class NIT : MTable
    {
        public NIT(byte[] b, long o)
            : base(b, o)
        {
            blocType = "Network Information Table";
        }
    }
    public class PAT : MTable
    {
        bool Section_Syntax_Indicator;
        // 3 bits 011
        int section_length;
        public int transport_Stream_Id;//2 bytes
        int reserved; // 2 bits 00
        int version_Number; //5 bits
        bool current_Next_Indicator; // 1bit
        int Section_Number;
        int Last_Section_Number;
        List<Program> ints = new List<Program>();
        public List<Program> Programs
        {
            get { return ints; }
            set { ints = value; }
        }
        public PAT(byte[] b, long o)
            : base(b, o)
        {
            blocType = "Program Association Table";
            int flags = breader.ReadByte();
            Section_Syntax_Indicator = (flags & 0x80) == 0x80;
            section_length = breader.ReadByte() + (flags & 0x03) * 256;
            transport_Stream_Id = breader.ReadByte() *256 + breader.ReadByte();
            flags = breader.ReadByte();
            version_Number = flags & 0x3E;
            current_Next_Indicator = (flags & 0x01)==0x01;
            Section_Number = breader.ReadByte();
            Last_Section_Number = breader.ReadByte();
            long u = breader.Position;
            while (breader.Position <  u + section_length - 9)
            {
                int ProgNb = breader.ReadShort();
                byte[] bb = breader.ReadBytes(2);
                int ProgId = ((bb[0] & 0x1F) << 8) + bb[1];
                string bt = "";
                if (ints.Count == 0)
                    bt = "Network Information Table";
                else bt = "Program MapTable";
                Program p = new Program(ProgNb, ProgId, bt, null);
                ints.Add(p);
             }
            // int crc
        }
        public override string ToString()
        {
            return m2TSArrivalTime.ToString() + " " + PID.ToString("X") + " " + blocType + " " + continuityCounter.ToString() + " " + offset.ToString("X2");
        }
    } 
    public class PMT : MTable
    {
        public int PCR_PID
        {
            get { return pCR_PID; }
            set { pCR_PID = value; }
        }
        public List<ElementaryStream> ElementaryStreams
        {
            get { return elementaryStreams; }
        }

        bool Section_Syntax_Indicator;
        // 3 bits 111
        int section_length;
        public int program_Num;//2 bytes
        int reserved; // 2 bits 00
        int version_Number; //5 bits
        bool current_Next_Indicator; // 1bit
        int Section_Number;
        int Last_Section_Number;
        // 3 bits
        private int pCR_PID; // PID of general timecode stream, or 0x1FFF

        // 4 bits
        int Program_info_length;// 2 +2+10 Sum size of following program descriptors. First two bits must be zero
        List<ElementaryStream> elementaryStreams = new List<ElementaryStream>();
        public PMT(byte[] b, long o)
            : base(b, o)
        {
            blocType = "Program Map Table";
            int flags = breader.ReadByte();
            Section_Syntax_Indicator = (flags & 0x80) == 0x80;
            section_length = breader.ReadByte() + (flags & 0x03) * 256;
            program_Num = breader.ReadByte() * 256 + breader.ReadByte();
            flags = breader.ReadByte();
            version_Number = flags & 0x3E;
            current_Next_Indicator = (flags & 0x01) == 0x01;
            Section_Number = breader.ReadByte();
            Last_Section_Number = breader.ReadByte();
            pCR_PID = ((breader.ReadByte() & 0x1F) << 8) + breader.ReadByte();
            int progInfoLength = ((breader.ReadByte() & 0x0F) << 8) + breader.ReadByte();
            long u = breader.Position;
            //Program Info Descriptor List: 
            while (breader.Position < u + progInfoLength)
            {
                int tag = breader.ReadByte();
                int length = breader.ReadByte();
                byte[] dt = breader.ReadBytes(length);
                switch (tag)
                {
                    case 5://Registration Descriptor
                        string s = Encoding.Default.GetString(dt);
                        break;
                    case   0x88: //HDMV Copy Control Descriptor
                        s = Encoding.Default.GetString(dt);
                        break;
                }
                
            }
            // Elementary Stream Info:
            u = breader.Position;
            while (breader.Position < 0x0b + section_length - 4 /*CRC*/ )
            {
                int streamType = breader.ReadByte(); // 1b  H.264/AVC Video (as defined in ITU-T Rec. H.264 | ISO/IEC 14496-10 Video)
                // 0x81 AC-3 (ATSC A/53B audio)

                int elPid = ((breader.ReadByte() & 0x1F) << 8) + breader.ReadByte();
                ElementaryStream st = new ElementaryStream(streamType, elPid);
                elementaryStreams.Add(st);
                int elprogInfoLength = ((breader.ReadByte() & 0x0F) << 8) + breader.ReadByte();
                if (elprogInfoLength > 0)
                {
                   long v = breader.Position;
                    while (breader.Position < v+elprogInfoLength)
                    {
                        int tag = breader.ReadByte();
                        int length = breader.ReadByte();
                        byte[] dt = breader.ReadBytes(length);
                        st.descriptors.Add(new Descriptor(tag, dt));
                    }

                }
            }
        }
        public override string ToString()
        {
            return m2TSArrivalTime.ToString() + " " + PID.ToString("X") + " " + blocType + " " + continuityCounter.ToString() + " " + offset.ToString("X2");
        }
    }
    public class PCR : M2TSPacket
    {
        private int pCR_Counter;
        private int program_Clock_Reference_Extension;

        public int PCR_Counter
        {
            get { return pCR_Counter; }
            set { pCR_Counter = value; }
        }
        public int Program_Clock_Reference_Extension
        {
            get { return program_Clock_Reference_Extension; }
            set { program_Clock_Reference_Extension = value; }
        }
        public PCR(byte[] buf, long o)
            : base(buf, o)
        {
            BitStreamReader bs = new BitStreamReader(buf, true);
            long pcrBase = (long)bs.ReadLongIntegerFromBits(32);
            int pcrExt = bs.ReadShort();
            long endTime = 300 * (2 * pcrBase + (pcrExt >> 15)) + (pcrExt & 0x01ff);
            double st = (double)300 * (2 * pcrBase + (pcrExt >> 15)) / (double)90000;
            double tt = (double)(pcrExt & 0x01ff) / (double)27000;
            program_Clock_Reference_Extension = pcrBuff[5];
        }
    }
    public class DataStream : LOCALIZED_DATA
    {
        private List<IM2TSPacket> packets = new List<IM2TSPacket>();
        private List<NAL> nals = new List<NAL>();

        public List<IM2TSPacket> Packets
        {
            get { return packets; }
            set { packets = value; }
        }
        public List<NAL> Nals
        {
            get { return nals; }
            set { nals = value; }
        }
        public void AddPacket(IM2TSPacket packet)
        {
            packets.Add(packet);//erroné
            DataPacket dp = (DataPacket)packet;
            if (dp.Nals != null)
                foreach (NAL n in dp.Nals)
                    nals.Add(n);
        }
    }
    public class VideoPacket : DataPacket
    {
        public List<byte[]> PTS_DTS_Buffer = new List<byte[]>();
        private long pTS;
        private long dTS;
        public long PTS
        {
            get { return pTS; }
            set { pTS = value; }
        } 
        public long DTS
        {
            get { return dTS; }
            set { dTS = value; }
        }
        public VideoPacket(byte[] b, long o)
            : base(b, o)
        {
            blocType = "Video stream";
            if ((b[11] & 0xE0) == 0xE0) //reads all data in 0xE0 NAL
            {
                breader.BitPosition = 12 * 8;
                int PacketLength = breader.ReadShort();
                byte[] extensionData = breader.ReadBytes(3);
                int PES_scrambling_control = (extensionData[0] & 0x30) >> 4;
                bool PES_priority = (extensionData[0] & 0x08) == 0x08;
                bool data_alignment_indicator = (extensionData[0] & 0x04) == 0x04;
                bool copyright = (extensionData[0] & 0x02) == 0x02;
                bool original_copy = (extensionData[0] & 0x01) == 0x01;
                int PTS_DTS_flags = (extensionData[1] & 0xc0) >> 6;
                bool ESCR_flag = (extensionData[1] & 0x20) == 0x20;
                bool ES_rate_flag = (extensionData[1] & 0x20) == 0x20;
                bool DSM_trick_mode_flag = (extensionData[1] & 0x08) == 0x08;
                bool additional_copy_info_flag = (extensionData[1] & 0x04) == 0x04;
                bool PES_CRC_flag = (extensionData[1] & 0x02) == 0x02;
                bool PES_extension_flag = (extensionData[1] & 0x01) == 0x01;
                int PES_header_data_length = extensionData[2];
                if (PTS_DTS_flags != 0)
                {
                    switch (PTS_DTS_flags)
                    {
                        case 0x02:
                            PTS_DTS_Buffer.Add(breader.ReadBytes(5));
                            break;
                        case 0x01:
                        case 0x03:
                            PTS_DTS_Buffer.Add(breader.ReadBytes(5));
                            PTS_DTS_Buffer.Add(breader.ReadBytes(5));
                            break;
                    }
                    PTS_DTS_Decode();
                }

            }
        }
        public void PTS_DTS_Decode()
        {

            pTS = DecodePts_dts(PTS_DTS_Buffer[0]);
            if (PTS_DTS_Buffer.Count == 2)
            {
                dTS = DecodePts_dts(PTS_DTS_Buffer[1]);
            }
        }
        private long DecodePts_dts(byte[] pts)
        {
            long p = (pts[0] & 0x0E << 29) + ((pts[1] & 0x7F) << 21) + ((pts[2] & 0xFE) << 14) + (pts[3] << 7) + ((pts[4] & 0xFE) >> 1);
            return p; //  90 khz;
        }
    }
    public class DataPacket : M2TSPacket
    {
        public bool startPacket = false;
        private List<NAL> nals;

        public List<NAL> Nals
        {
            get { return nals; }
            set { nals = value; }
        }
        string date;
        public string FrameDate
        {
            get { return date; }
            set { date = value; }
        }
        public DataPacket(byte[] b, long o)
            : base(b, o)
        {
            int vd = findStream(b, breader.Position);
            if ((vd > 0) & (vd + 4 < b.Length))
            {
                breader.BitPosition = vd * 8;

                try
                {
                    byte[] nalTag = breader.ReadBytes(4);
                    //             NAL n = new NAL(vd, this.PacketNumber, nalTag, new byte[0]);
                    if (((nalTag[3] & 0xE0) == 0xE0) || ((nalTag[3] & 0xF0) == 0xF0))
                        startPacket = true;
                }
                catch { }
            }
            #region Look for NAL
            List<int> nalOccurences = findNals(b, breader.Position);
            for (int k = 0; k < nalOccurences.Count;k++ )
            {
                int j = nalOccurences[k];
                int end = 0xC0 -1;
                if (k < nalOccurences.Count - 1)
                    end = nalOccurences[k + 1];
                breader.BitPosition = j * 8;

                byte[] nalTag = breader.ReadBytes(5);
                int length = end - j - 5;
                byte[] data = null;
                if (length >= 4)
                {
                    data = breader.ReadBytes(length);
                   NAL n = new NAL(j, this.PacketNumber, nalTag, data);
                    if (n.NalType == "Video stream")
                        startPacket = true;
                    if (n.Date != null)
                        date = n.Date;
                    AddNal(n);
                }

            }
            #endregion
        }
        private void AddNal(NAL nal)
        {
            if (nals == null)
            {
                nals = new List<NAL>();
            }
            nals.Add(nal);
        }
        public override string ToString()
        {
            return m2TSArrivalTime.ToString() + " " + PID.ToString("X") + " " + blocType + " " + continuityCounter.ToString() + " " + offset.ToString("X2") + FrameDate ;
        }
        private int findStream(byte[] haystack, long start)
        {
            byte[] needle = new byte[] { 0x00, 0x00, 0x01 };// ou { 0x00, 0x00, 0x00, 0x01 };
            List<int> occurences = new List<int>();
            int index = -1;
            for (int i = (int) start; i < haystack.Length; i++)
            {
                if (needle[0] == haystack[i])
                {
                    bool found = true;
                    int j, k;
                    for (j = 0, k = i; j < needle.Length; j++, k++)
                    {
                        if (k >= haystack.Length || needle[j] != haystack[k])
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {
                        occurences.Add(i - 1);
                        index = i ;
                        break;
                    }
                }
            }
            return index;
        }
        private List<int> findNals(byte[] haystack, long start)
        {
            byte[] needle = new byte[] { 0x00, 0x00, 0x00, 0x01 };// ou {  0x00, 0x00, 0x01 };
            List<int> occurences = new List<int>();
            for (int i = (int)start; i < haystack.Length; i++)
            {
                if (needle[0] == haystack[i])
                {
                    bool found = true;
                    int j, k;
                    for (j = 0, k = i; j < needle.Length; j++, k++)
                    {
                        if (k >= haystack.Length || needle[j] != haystack[k])
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {
                        occurences.Add(i);
                        i = k;
                    }
                }
            }
            return occurences;
        }
    }
    public class AudioPacket : DataPacket
    {
        public AudioPacket(byte[] b, long o)
            : base(b, o)
        {
            blocType = "Audio stream";
            if ((b[11] & 0xF0) ==0xF0)//reads all data in 0xF0 NAL
            {
//                startPacket = true;
            }
        }
    }
    public class M2TSPacket : LOCALIZED_DATA, BluRay.IM2TSPacket
    {
        #region common header
        private int pID;
        int packetNumber;
        private List<MAttribute> attributes;

        protected List<MAttribute> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
        protected BitStreamReader breader;
        public int PacketNumber { get {return packetNumber; } set{packetNumber=value;} }

        public int m2TS_Copy_Permission_Indicator;//first two bits
        public long m2TSArrivalTime;// First four bytes - 30 bits
        public int syncByte;//byte n° 4 : must be 0x47
        public bool transport_Error_Indicator;
        public bool payload_Unit_Start_Indicator;
        public bool transport_Priority;

        public int PID
        {
            get { return pID; }
            set { pID = value; }
        }
        public int transport_Scrambling_Control; // 2 bits
        public int Adaptation_field; //2 bits
        public int continuityCounter; // four bits
        #endregion
        public string blocType;
        #region Adaptation Field Format
        protected byte[] pcrBuff;
        protected byte[] opcrBuf ;
        protected bool Discontinuity_indicator; // Set to 1 if current TS packet is in a discontinuity state with respect to either the continuity counter or the program clock reference
        protected bool Random_Access_indicator; //Set to 1 if the PES packet in this TS packet starts a video/audio sequence
        protected bool Elementary_stream_priority_indicator;// 1 = higher priority
        protected bool PCR_flag; //1 means adaptation field does contain a pcrBuff field
        protected bool OPCR_flag;//1 means adaptation field does contain an opcrBuf field
        protected bool Splicing_point_flag;//1 means presence of splice countdown field in adaptation field
        protected bool Transport_private_data_flag;// 1 means presence of private buffer bytes in adaptation field
        protected bool Adaptation_field_extension_flag;//1 means presence of adaptation field extension
        #endregion
        byte[] header;
        byte[] program;
        protected byte[] buffer;
        protected long offset;
        protected List<Tag> tags = new List<Tag>();
        public long position;
        public long length;
        public M2TSPacket()
        {
        }
        public M2TSPacket(byte[] buffer, long offset)
        {
            position = offset;
            length = buffer.Length;
            blocType = "Standard";
            packetNumber = (int) offset / 0xC0;
            this.buffer = buffer;
            this.offset = offset;
            breader = new BitStreamReader(buffer, true);
            #region header
            DecodeHeader();
            #endregion
            int Adaptation_field_Length = 0;
            switch (Adaptation_field)
            {
                case 1: // no adaptation fields, payload only
                    break;
                case 2:// adaptation field only
                case 3: //adaptation field and payload
                    Adaptation_field_Length = breader.ReadByte();
                     break;
             }
            if (Adaptation_field_Length > 0)
            {
                byte data = breader.ReadByte();
                AddAttribute(new MAttribute("Discontinuity_indicator", (data & 0x80) == 0x80));
                Discontinuity_indicator = (data & 0x80) == 0x80; // Set to 1 if current TS packet is in a discontinuity state with respect to either the continuity counter or the program clock reference
                AddAttribute(new MAttribute("Random_Access_indicator", (data & 0x40) == 0x40));
                Random_Access_indicator = (data & 0x40) == 0x40; //Set to 1 if the PES packet in this TS packet starts a video/audio sequence
                AddAttribute(new MAttribute("Elementary_stream_priority_indicator", (data & 0x20) == 0x40));
                Elementary_stream_priority_indicator = (data & 0x20) == 0x40;// 1 = higher priority
                AddAttribute(new MAttribute("PCR_flag", (data & 0x10) == 0x10));
                PCR_flag = (data & 0x10) == 0x10; //1 means adaptation field does contain a pcrBuff field
                AddAttribute(new MAttribute("OPCR_flag", (data & 0x08) == 0x08));
                OPCR_flag = (data & 0x08) == 0x08;//1 means adaptation field does contain an opcrBuf field
                AddAttribute(new MAttribute("Splicing_point_flag", (data & 0x04) == 0x04));
                Splicing_point_flag = (data & 0x04) == 0x04;//1 means presence of splice countdown field in adaptation field
                AddAttribute(new MAttribute("Transport_private_data_flag", (data & 0x02) == 0x02));
                Transport_private_data_flag = (data & 0x02) == 0x02;// 1 means presence of private buffer bytes in adaptation field
                Adaptation_field_extension_flag = (data & 0x01) == 0x01;//1 means presence of adaptation field extension
                if (PCR_flag)
                {                   
                    pcrBuff = breader.ReadBytes(6);
                }
                if (OPCR_flag)
                {
                    Buffer.BlockCopy(buffer, 0x0A, pcrBuff, 0, 6);
                }
                if (Splicing_point_flag)
                {
                    int Splice_Countdown = breader.ReadByte();
                }
                if (Adaptation_field_extension_flag)
                {
                }

            }
        }
        public override string ToString()
        {
            return m2TSArrivalTime.ToString() + " " + pID.ToString("X") + " " + blocType + " " + continuityCounter.ToString() + " " + offset.ToString("X2");
        }
        /// <summary>
        /// First eight bytes
        /// </summary>
        private void DecodeHeader()
        {
            m2TS_Copy_Permission_Indicator = breader.ReadIntFromBits(2);
      //      byte[] c = breader.ReadBytes(4); 
      //      m2TS_Copy_Permission_Indicator = (c[0] & 0xC0); // 2 bits
      //      c[0] = (byte)(c[0] & 0x3F);
            m2TSArrivalTime = breader.ReadIntFromBits(30);// sur 30 bits
            syncByte = breader.ReadByte();
            byte data = breader.ReadByte(); //byte 6
            transport_Error_Indicator = (data & 0x80) == 0x80;
            payload_Unit_Start_Indicator = (data & 0x40) == 0x40;
            transport_Priority = (data & 0x20) == 0x20;
            pID = ((data & 0x1F) << 8) + breader.ReadByte();
            data = breader.ReadByte(); //byte 7
            transport_Scrambling_Control = ((data & 0xD0) >> 6);
            Adaptation_field = ((data & 0x30) >> 4);
            continuityCounter = data & 0x0F;
        }
        private int GetPID(byte[] b)
        {
            return ((b[0] & 0x3F) << 8) + b[1];
        }
        private void AddAttribute(MAttribute attr)
        {
            if (attributes == null)
                attributes = new List<MAttribute>();
            attributes.Add(attr);
        }
    }
    public class ElementaryStream : LOCALIZED_DATA
    {
        int streamType; 
        int elPid;
        public string Stream
        {
            get{
             switch (streamType)
            {
                case 0x1b: return "Video";
                case 0x81: return "Audio";
               default: return "";
            }
       }}
        public int StreamType
        {
            get { return streamType; }
            set { streamType = value; }
        }

        public List<Descriptor> descriptors = new List<Descriptor>();
        public int PID
        {
            get { return elPid; }
            set { elPid = value; }
        }
        int elprogInfoLength;
        public ElementaryStream(int stType, int pid)
        {
            streamType = stType;
            elPid = pid;
        }
        public override string ToString()
        { 
            string str = "";
            switch (streamType)
            {
                case 0x1b: str = "H.264/AVC Video (as defined in ITU-T Rec. H.264 | ISO/IEC 14496-10 Video)";
                    break;
                case 0x81: str = "AC-3 (ATSC A/53B audio)";
                    break;
            }
            return str + " " + PID.ToString();
        }
    }
    public class Descriptor : LOCALIZED_DATA
    {
        int tag;
        byte[] data;
        public Descriptor(int t, byte[] dt)
        {
            tag = t;
            data = dt;
            int length = dt.Length;
            #region decode
            switch (tag)
            {
                case 5://Registration Descriptor
                    string s = Encoding.Default.GetString(dt, 0, 1);
                    int i = 1;
                    while ((i < length) && (dt[i] != 0xff))
                    {
                        s += Encoding.Default.GetString(dt, i, 1);
                        i++;
                    }
                    i++;
                    if (i < length)
                    {
                        int format = dt[i++];
                        int video_format = (dt[i] & 0xF0) >> 4;
                        int frame_rate = dt[i++] & 0x0F;
                        int aspect_ratio = dt[i] & 0xF0;
                        // chaine terminée par ff puis 3 octets
                        // 1B
                        // 43         video_format: 4: 1080i - SMPTE 274M,    frame_rate: 3: 25
                        // 3f           aspect_ratio: 3: 16:9
                    }
                    break;
                case 0x88: //HDMV Copy Control Descriptor
                    s = Encoding.Default.GetString(dt);
                    break;
                case 0x81: /*AC-3 (ATSC) Audio Descriptor : 04 38 0F 00 1F 00
   O4 :    sample_rate_code: 0: 48 KHz  et       bsid: 4: (Non standard bsid)
       bit_rate_code: 14: Exact Bitrate: 384 Kbit/s
       surround_mode: 0: Not indicated
       bsmod: 0
       num_channels: 7:  Audio Coding Mode: 3/2
       full_svc: True
       langcod: 0
       mainid: 0
       priority: 3: Not specified
       language_flag: False
       language_flag2: False
                                        * et http://rmworkshop.com/dvd_info/related_info/ac3hdr.html
*/
                    break;
            }
            #endregion

        }
    }
    public class Program : LOCALIZED_DATA
    {
        string blocType;
        public string BlocType
        {
          get { return blocType; }
          set { blocType = value; }
        }
        private List<IM2TSPacket> blocs = new List<IM2TSPacket>();

        public List<IM2TSPacket> Blocs
        {
            get { return blocs; }
            set { blocs = value; }
        }
        public int programNumber;
        public int PID;
        private string progType;

        public string ProgType
        {
            get { return progType; }
            set { progType = value; }
        }
        public Program(int id, string pType, IM2TSPacket b)
        {
            PID = id;
            progType = pType;
            if (b != null)
                blocs.Add(b);
            
            blocType = pType;
        }
        public Program(int number, int id, string pType, IM2TSPacket b)
        {
            programNumber = number;
            PID = id;
            progType = pType;
            if (b != null)
                blocs.Add(b);
        }
        public override string ToString()
        {
            return PID.ToString("x4") + " " + progType;
        }
    }
    public class MAttribute : LOCALIZED_DATA
    {
        string name;
        object value;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public MAttribute(string n, object o)
        {
            name = n;
            value = o;
        }
        public override string ToString()
        {
            return name +" "+ value.ToString();
        }
    }
    public class Tag : LOCALIZED_DATA
    {
        int tag;
        enum tagNames { DateTimeOriginal = 0x0018, DateTimeOriginal_2 = 0x0019, Camera1 = 0x0070, Camera2 = 0x0071, Shutter = 0x007f, ExposureTime = 0x00a0, FNumber = 0x00a1, ExposureProgram = 0x00a2, BrightnessValue = 0x00a3, ExposureCompensation = 0x00a4, MaxApertureValue = 0x00a5, Flash = 0x00a6, CustomRendered = 0x00a7, WhiteBalance = 0x00a8, FocalLengthIn35mmFormat = 0x00a9, SceneCaptureType = 0x00aa, GPSVersionID = 0x00b0, GPSLatitudeRef = 0x00b1, GPSLatitude = 0x00b2, GPSLongitudeRef = 0x00b5, GPSLongitude = 0x00b6, GPSAltitudeRef = 0x00b9, GPSAltitude = 0x00ba, GPSTimeStamp = 0x00bb, GPSStatus = 0x00be, GPSMeasureMode = 0x00bf, GPSDOP = 0x00c0, GPSSpeedRef = 0x00c1, GPSSpeed = 0x00c2, GPSTrackRef = 0x00c3, GPSTrack = 0x00c4, GPSImgDirectionRef = 0x00c5, GPSImgDirection = 0x00c6, GPSMapDatum = 0x00c7, MakeModel = 0x00e0, RecInfo = 0x00e1, Model = 0x00e4, Model2 = 0x00e5, Model3 = 0x00e6, FrameInfo = 0x00ee };
        string tagName;
        byte[] buffer;
        public string date = "";
        public string make = "";
        public string unused = "";
        public string model = "";
        private int FromBCD(byte b)
        {
            int digit1 = b >> 4;
            int digit2 = b & 0x0f;
            return digit1 * 10 + digit2;
        }
        public Tag(int t, byte[] d)
        {
            tag = t;
            buffer = d;
            #region Decode tags
            if ((buffer[0] == 0xff) && (buffer[1] == 0xff) && (buffer[2] == 0xff) && (buffer[3] == 0xff))
            {
                unused = "unused";
                return;
            }

            switch (t)
            {
                case 0x18:
                    int yd = (buffer[1] >> 4) * 1000;
                    int ye = buffer[2] & 0x0f;
                    int year = yd + ye;
                    int month = buffer[3];
                    date = year.ToString() + ":" + month.ToString() + ":";
                    break;
                case 0x19:
                    int jour = FromBCD(buffer[0]);
                    int heure = FromBCD(buffer[1]);
                    int min = FromBCD(buffer[2]);
                    int sec = FromBCD(buffer[3]);
                    date += jour.ToString() + " " + heure.ToString() + ":" + min.ToString() + ":" + sec.ToString();
                    break;
                case 0xe0:
                    if ((buffer[0] == 0x01) && (buffer[1] == 0x03))
                    {
                        make = "Panasonic";
                    }
                    if ((buffer[0] == 0x01) && (buffer[1] == 0x08))
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
                    model = Encoding.Default.GetString(buffer);
                    break;
                case 0xe5://Model
                    model = Encoding.Default.GetString(buffer);
                    break;
                case 0xe6://Model
                    model = Encoding.Default.GetString(buffer);
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
            return tag.ToString("X2") + " " + (tagNames)tag + " " + unused + " " + date + " " + make;
        }
    }
}
 