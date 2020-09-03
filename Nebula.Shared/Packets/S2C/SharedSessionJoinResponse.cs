namespace Nebula.Shared.Packets.S2C
{
    public class SharedSessionJoinResponse
    {
        public string   RoomId            { get; set; }
        public string   RoomName          { get; set; }
        public int      MaxUsers          { get; set; }
        public bool     PasswordProtected { get; set; }
        public string[] Users             { get; set; }
        public int      ResponseCode      { get; set; }
        public bool     Success           => ResponseCode == 0;
        public int      CurrentUsers      => Users?.Length ?? 0;
    }
}