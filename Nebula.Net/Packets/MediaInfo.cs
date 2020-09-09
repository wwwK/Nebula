using LiteNetLib.Utils;

namespace Nebula.Net.Packets
{
    public struct MediaInfo : INetSerializable
    {
        public string Id       { get; set; }
        public string Title    { get; set; }
        public string Provider { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(Title);
            writer.Put(Provider);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetString();
            Title = reader.GetString();
            Provider = reader.GetString();
        }

        public static MediaInfo Empty => new MediaInfo {Id = string.Empty, Title = string.Empty};

        public static bool operator ==(MediaInfo media1, MediaInfo media2)
        {
            return media1.Id == media2.Id;
        }

        public static bool operator !=(MediaInfo media1, MediaInfo media2)
        {
            return media1.Id != media2.Id;
        }
    }
}