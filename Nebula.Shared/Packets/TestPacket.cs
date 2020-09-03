namespace Nebula.Shared.Packets
{
    public class TestPacket
    {
        public UserInfosPacket[] Users { get; set; }

        public static TestPacket Create()
        {
            UserInfosPacket[] users = new[]
            {
                new UserInfosPacket {Id = 1, Name = "Doria,", ThumbnailUrl = ""},
                new UserInfosPacket {Id = 1, Name = "Doria,", ThumbnailUrl = ""},
                new UserInfosPacket {Id = 1, Name = "Doria,", ThumbnailUrl = ""},
                new UserInfosPacket {Id = 1, Name = "Doria,", ThumbnailUrl = ""},
                new UserInfosPacket {Id = 1, Name = "Doria,", ThumbnailUrl = ""},
                new UserInfosPacket {Id = 1, Name = "Doria,", ThumbnailUrl = ""},
            };
            return new TestPacket {Users = users};
        }
    }
}