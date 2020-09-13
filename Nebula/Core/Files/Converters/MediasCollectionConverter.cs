using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nebula.Core.Medias;

namespace Nebula.Core.Files.Converters
{
    public class MediasCollectionConverter : JsonConverter<MediasCollection>
    {
        public override MediasCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            MediasCollection collection = new MediasCollection();
            if (reader.TokenType != JsonTokenType.StartArray)
                return collection;
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    if (JsonSerializer.Deserialize(ref reader, typeof(IMediaInfo), options) is IMediaInfo mediaInfo)
                        collection.Add(mediaInfo);
                }

                reader.Read();
            }

            return collection;
        }

        public override void Write(Utf8JsonWriter writer, MediasCollection value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (IMediaInfo mediaInfo in value)
                JsonSerializer.Serialize(writer, mediaInfo, options);
            writer.WriteEndArray();
        }
    }
}