using LiteNetLib.Utils;

namespace Nebula.Shared.Packets
{
    public class UserInfosPacket
    {
        public int    Id           { get; set; }
        public string Name         { get; set; }
        public string ThumbnailUrl { get; set; }

        public static UserInfosPacket FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            string[] split = value.Split('@');
            if (split.Length != 3)
                return null;
            return new UserInfosPacket {Id = int.Parse(split[0]), Name = split[1], ThumbnailUrl = split[2]};
        }

        public static void Serialize(NetDataWriter writer, UserInfosPacket packet)
        {
            writer.Put(packet.Id);
            writer.Put(packet.Name);
            writer.Put(packet.ThumbnailUrl);
        }

        public static UserInfosPacket Deserialize(NetDataReader reader)
        {
            return new UserInfosPacket {Id = reader.GetInt(), Name = reader.GetString(), ThumbnailUrl = reader.GetString()};
        }
    }
}