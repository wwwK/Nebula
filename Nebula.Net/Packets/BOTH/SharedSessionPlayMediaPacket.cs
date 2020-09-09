namespace Nebula.Net.Packets.BOTH
{
    public class SharedSessionPlayMediaPacket
    {
        public UserInfo  UserInfo  { get; set; } = UserInfo.Empty;
        public MediaInfo MediaInfo { get; set; } = MediaInfo.Empty;
        public bool      PlayVideo { get; set; } = false;
    }
}