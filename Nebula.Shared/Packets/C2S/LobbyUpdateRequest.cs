namespace Nebula.Shared.Packets.C2S
{
    public class LobbyUpdateRequest
    {
        public string LobbyName      { get; set; }
        public string LobbyThumbnail { get; set; }
        public string LobbyPassword  { get; set; }
    }
}