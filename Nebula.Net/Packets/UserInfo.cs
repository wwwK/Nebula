using LiteNetLib.Utils;

namespace Nebula.Net.Packets
{
    public struct UserInfo : INetSerializable
    {
        public int    Id        { get; set; }
        public string Username  { get; set; }
        public string AvatarUrl { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(Username);
            writer.Put(AvatarUrl);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();
            Username = reader.GetString();
            AvatarUrl = reader.GetString();
        }

        public static UserInfo Empty => new UserInfo {Id = -1, Username = string.Empty, AvatarUrl = string.Empty};

        public static bool operator ==(UserInfo user1, UserInfo user2)
        {
            return user1.Id == user2.Id;
        }

        public static bool operator !=(UserInfo user1, UserInfo user2)
        {
            return user1.Id != user2.Id;
        }
    }
}