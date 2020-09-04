namespace Nebula.Net.Packets.C2S
{
    public class SharedSessionsPollRequest
    {
        public int ToAvoidEmptyPacket { get; set; } = 0;
    }
}