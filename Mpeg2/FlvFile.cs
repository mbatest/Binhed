using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
namespace VideoFiles
{
    public class FlvFile:LOCALIZED_DATA
    {
        ELEMENTARY_TYPE signature;
        ELEMENTARY_TYPE version;
        ELEMENTARY_TYPE flags;
        ELEMENTARY_TYPE headerSize;
        public ELEMENTARY_TYPE Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        public ELEMENTARY_TYPE Version
        {
            get { return version; }
            set { version = value; }
        }
        public ELEMENTARY_TYPE Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        public ELEMENTARY_TYPE HeaderSize
        {
            get { return headerSize; }
            set { headerSize = value; }
        }
        List<PacketHeader> pks = new List<PacketHeader>();
        public FlvFile(string FileName)
        {
            BitStreamReader sw = new BitStreamReader(FileName, true);
            LengthInFile = sw.Position;
            signature = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 3);
            version = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            flags = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            headerSize = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            while (sw.Position<sw.Length)
            {
                PacketHeader pk = new PacketHeader(sw);
                pks.Add(pk);
            }
            sw.Close();
            return;

        }
    }
    public class PacketHeader : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE start_last_packet;

        public ELEMENTARY_TYPE Start_last_packet
        {
            get { return start_last_packet; }
            set { start_last_packet = value; }
        }
        ELEMENTARY_TYPE packet_type;

        public ELEMENTARY_TYPE Packet_type
        {
            get { return packet_type; }
            set { packet_type = value; }
        }
        ELEMENTARY_TYPE payload_size;

        public ELEMENTARY_TYPE Payload_size
        {
            get { return payload_size; }
            set { payload_size = value; }
        }
        ELEMENTARY_TYPE timestamp_lower;

        public ELEMENTARY_TYPE Timestamp_lower
        {
            get { return timestamp_lower; }
            set { timestamp_lower = value; }
        }
        ELEMENTARY_TYPE timestamp_upper;

        public ELEMENTARY_TYPE Timestamp_upper
        {
            get { return timestamp_upper; }
            set { timestamp_upper = value; }
        }
        ELEMENTARY_TYPE stream_ID;

        public ELEMENTARY_TYPE Stream_ID
        {
            get { return stream_ID; }
            set { stream_ID = value; }
        }
        ELEMENTARY_TYPE payload_data;

        public ELEMENTARY_TYPE Payload_data
        {
            get { return payload_data; }
            set { payload_data = value; }
        }
        public PacketHeader(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            start_last_packet = new ELEMENTARY_TYPE(sw, 0, typeof(int)); ;
            packet_type = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            payload_size = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 3);
            timestamp_lower = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 3);
            timestamp_upper = new ELEMENTARY_TYPE(sw, 0, typeof(byte));
            stream_ID = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 3);
            sw.Position += Vint.BytesToInt((byte[])payload_size.Value);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
}
