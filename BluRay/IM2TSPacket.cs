using System;
namespace BluRay
{
    public interface IM2TSPacket
    {
        int PacketNumber { get; set; }
        int PID { get; set; }
        string ToString();
    }
}
