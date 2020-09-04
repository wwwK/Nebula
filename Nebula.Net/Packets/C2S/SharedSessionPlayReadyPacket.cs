namespace Nebula.Net.Packets.C2S
{
    public class SharedSessionPlayReadyPacket
    {
        public int ToAvoidEmptyPacket { get; set; } = 0;
    }
}