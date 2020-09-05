namespace Nebula.Net.Packets.BOTH
{
    public class SharedSessionPlayMediaPacket
    {
        public UserInfo UserInfo  { get; set; } = UserInfo.Empty;
        public string   MediaId   { get; set; }
        public string   MediaName { get; set; }
        public string   Provider  { get; set; }
    }
}