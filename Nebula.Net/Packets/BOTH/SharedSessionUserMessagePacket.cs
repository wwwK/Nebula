namespace Nebula.Net.Packets.BOTH
{
    public class SharedSessionUserMessagePacket
    {
        public UserInfo User    { get; set; } = UserInfo.Empty;
        public string   Message { get; set; }
    }
}