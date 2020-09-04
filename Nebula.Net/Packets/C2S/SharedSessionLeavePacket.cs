namespace Nebula.Net.Packets.C2S
{
    public class SharedSessionLeavePacket
    {
        public int ToAvoidEmptyPacket { get; set; } = 0;
    }
}