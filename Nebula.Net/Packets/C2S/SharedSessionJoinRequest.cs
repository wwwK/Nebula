namespace Nebula.Net.Packets.C2S
{
    public class SharedSessionJoinRequest
    {
        public SharedSessionInfo Session  { get; set; }
        public string            Password { get; set; }
    }
}