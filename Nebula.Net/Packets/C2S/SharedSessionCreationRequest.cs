namespace Nebula.Net.Packets.C2S
{
    public class SharedSessionCreationRequest
    {
        public string Name     { get; set; }
        public string Password { get; set; }
        public int    Size     { get; set; }
    }
}