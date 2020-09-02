namespace Nebula.Shared.Packets.C2S
{
    public class SharedSessionRoomCreationRequest
    {
        public string RoomName      { get; set; }
        public string RoomThumbnail { get; set; }
        public string RoomPassword  { get; set; }
        public int    RoomMaxUsers  { get; set; }
    }
}