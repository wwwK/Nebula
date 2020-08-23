namespace Nebula.Shared.Packets.C2S
{
    public class LobbyCreationRequest
    {
        public string LobbyName      { get; set; }
        public string LobbyThumbnail { get; set; }
        public string LobbyPassword  { get; set; }
        public int    LobbyMaxUsers  { get; set; }
    }
}