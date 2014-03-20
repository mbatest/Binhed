using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Utils;
/*Packets are 188 bytes (0xBC) in length, but the communication medium may add some error correction bytes to the packet. 
 * ISDB-T and DVB-T/C/S uses 204 bytes (0xCC) and ATSC 8-VSB, 208 bytes (0xD0) as the size of emission packets 
 * 192 (0xC0) for m2ts  adding a 4-byte timecode (TC) to standard 188-byte packets
 */
namespace BluRay
{
    public class TS_File : LOCALIZED_DATA
    {
        static List<TS_Descriptor> desc = new List<TS_Descriptor>();
        static List<TS_AdaptationField> adapt = new List<TS_AdaptationField>();
        private StreamType streamType = BluRay.StreamType.TS;
        public String Stream_Type { get { return (streamType).ToString(); } }
        public static List<TS_Descriptor> Desc { get { return TS_File.desc; } }
        public List<TS_DataStream> Videostreams { get { return videostreams; } }
        public List<TS_DataStream> Audiostreams { get { return audiostreams; } }
        public List<ITS_Packet> Packets { get { return packets; } }
        public List<TS_Program> Programs { get { return programs; } }
        private List<TS_ElementaryStream> streams = new List<TS_ElementaryStream>();
        public List<TS_ElementaryStream> Streams { get { return streams; } }
        public List<string> Dates { get { return dates; } }
        List<TS_DataStream> audiostreams = new List<TS_DataStream>();
        List<TS_DataStream> videostreams = new List<TS_DataStream>();
        List<string> dates = new List<string>();
        private List<TS_Program> programs = new List<TS_Program>();
        TS_DataStream CurrentVideoStream;
        TS_DataStream CurrentAudioStream;
        private List<int> progIds = new List<int>();
        List<IM2TSPacket> blocs = new List<IM2TSPacket>();
        List<byte> tags = new List<byte>();
        public List<byte> Tags
        {
            get { return tags; }
            set { tags = value; }
        }
        List<ITS_Packet> packets = new List<ITS_Packet>();
        public TS_File(string FileName)
        {
            Utils.Utils.PacketSize = 0xBC;
            streamType = StreamType.TS;
            #region Identifies type of stream
            BitStreamReader sw = new BitStreamReader(FileName, true);
            byte b = sw.ReadByte();
            if (b != 0x47)
            {
                Utils.Utils.PacketSize = 0xC0;
                streamType = StreamType.M2TS;
            }
            long PacketNumber = sw.Length / Utils.Utils.PacketSize;
            sw.Position = 0;
            #endregion
            #region Reads first packet to create data structures
            TS_Program pat = new TS_Program(0, "Program Association Table");
            programs.Add(pat);
            TS_Program_Association_Table first = new TS_Program_Association_Table(sw);
            pat.Packets.Add(first);
            packets.Add(first);
            int Program_Map_PID = 0;
            foreach (TS_Program p in first.Programs)
            {
                programs.Add(p);
                if (p.ProgramNumber == 1)
                {
                    Program_Map_PID = p.Program_Map_PID;
                    p.Program_Name = "Program Map Table";
                }
            }
            int PCR_PID = 0xff;
            #endregion
            while (!sw.Eof)
            {
                long blockstart = sw.Position;
                TS_Packet_Header hd = new TS_Packet_Header(sw);
                int pid = hd.PID;
                sw.Position = blockstart;
                TS_Packet p = null;
                if (pid == 0x000)
                {
                    p = new TS_Program_Association_Table(sw);
                }
                else if (pid == 0x001)
                {
                    //Conditional access table
                }
                else if (pid == Program_Map_PID)
                {

                    p = new TS_Program_Map_Table(sw);
                    if (streams.Count == 0)
                    {
                        TS_Program_Map_Table pmt = (TS_Program_Map_Table)p;
                        if (pmt.PCR_PID_Value != null)
                        {
                            switch (streamType)
                            {
                                case StreamType.M2TS:// Pourquoi 
                                    PCR_PID = (int)pmt.PCR_PID_Value;
                                    programs.Add(new TS_Program(PCR_PID, "Program Clock Reference"));
                                    break;
                                case StreamType.TS:
                                    break;
                            }
                        }
                        foreach (TS_ElementaryStream el in pmt.ElementaryStreams)
                        {
                            streams.Add(el);
                            programs.Add(new TS_Program(el.PID, el.Stream));
                        }
                    }
                }
                else if (pid == PCR_PID)// PCR_Packets
                {
                    p = new TS_Packet(sw);
                }
                else
                {
                    #region Data packets
                    bool ok = false;
                    foreach (TS_ElementaryStream el in streams)
                    {
                        if(pid==100)
                        { }
                        if (el.PID == pid)
                        {
                            switch (el.Stream_Category)
                            {
                                case "Video":
                                    #region Packets Video
                                    p = new TS_VideoPacket(sw);
                                    string d = ((TS_DataPacket)p).Date;
                                    if ((d != null) & (d != ""))
                                        dates.Add(((TS_DataPacket)p).Date + ", block : " + ((TS_DataPacket)p).BlockNumber);
                                    if (((TS_VideoPacket)p).Header.Payload_Unit_Start_Indicator)
                                    {
                                        CurrentVideoStream = new TS_DataStream();
                                        el.Packets.Add(CurrentVideoStream);
                                    }
                                    if (CurrentVideoStream != null)
                                        CurrentVideoStream.AddPacket((TS_VideoPacket)p);
                                    ok = true;
                                    break;
                                    #endregion
                                case "Audio":
                                    #region Audio Packets
                                    p = new TS_AudioPacket(sw);
                                    if (((TS_AudioPacket)p).Header.Payload_Unit_Start_Indicator)
                                    {
                                        CurrentAudioStream = new TS_DataStream();
                                        el.Packets.Add(CurrentAudioStream);
                                    }
                                    if (CurrentAudioStream != null)
                                        CurrentAudioStream.AddPacket((TS_AudioPacket)p);
                                    ok = true;
                                    #endregion
                                    break;
                            }
                        }
                    }
                    #endregion
                    if (!ok)
                    {
                        p = new TS_Packet(sw);
                    }
                }
                #region Add packet to program
                TS_Program pr = null;
                foreach (TS_Program prog in programs)
                {
                    if (prog.Program_Map_PID == pid)
                    {
                        pr = prog;
                        prog.Packets.Add(p); break;
                    }
                }
                if (pr == null)
                {
                    TS_Program pro = new TS_Program(p.Header.PID, "Inconnu");
                    programs.Add(pro);
                    pro.Packets.Add(p);
                }
                packets.Add(p);
                #endregion
                sw.Position = ((TS_Packet)p).PositionOfStructureInFile + Utils.Utils.PacketSize;
                if (packets.Count > 10000)
                    //                   sw.Position = 0x1951f60;
                    break;
            }
            LengthInFile = sw.Length;
        }
    }
    public enum StreamType { TS, M2TS }
}
