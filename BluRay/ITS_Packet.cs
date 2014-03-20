using System;
namespace BluRay
{
    public interface ITS_Packet
    {
        TS_AdaptationField AdaptationField { get; set; }
        TS_Packet_Header Header { get; set; }
        Utils.ELEMENTARY_TYPE PointerField { get; set; }
        Utils.ELEMENTARY_TYPE Splice_countdown { get; set; }
        string ToString();
    }
}
