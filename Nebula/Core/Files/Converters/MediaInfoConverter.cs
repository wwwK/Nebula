using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using EasySharp.Reflection;
using EasySharp.Reflection.Fast;
using Nebula.Core.Medias;

namespace Nebula.Core.Files.Converters
{
    public class MediaInfoConverter : JsonConverter<IMediaInfo>
    {
        private static readonly Dictionary<string, FastPropertyInfo<IMediaInfo, object>> Properties = FastAccessor.GetObject<IMediaInfo>(true, false,
            p => !p.HasAttribute<JsonIgnoreAttribute>(), f => !f.HasAttribute<JsonIgnoreAttribute>()).GetPropertiesAsDictionary();

        public override IMediaInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                return default;
            reader.Read(); //Skip token
            reader.Read(); //Skin PropertyName
            string providerType = reader.GetString();
            if (string.IsNullOrWhiteSpace(providerType))
                return default;
            IMediaInfo mediaInfo = Activator.CreateInstance(Type.GetType(providerType) ?? throw new JsonException("Invalid Provider Type")) as IMediaInfo;
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.PropertyName)
                    break;
                string property = reader.GetString();
                if (!Properties.ContainsKey(property))
                    continue;
                FastPropertyInfo<IMediaInfo, object> info = Properties[property];
                reader.Read();
                info.Set(mediaInfo, JsonSerializer.Deserialize(ref reader, info.Info.PropertyType, options));
            }

            return mediaInfo;
        }

        public override void Write(Utf8JsonWriter writer, IMediaInfo value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("ProviderType", value.GetType().FullName);
            foreach (KeyValuePair<string, FastPropertyInfo<IMediaInfo, object>> keyValuePair in Properties)
            {
                writer.WritePropertyName(keyValuePair.Key);
                JsonSerializer.Serialize(writer, keyValuePair.Value.Get(value), keyValuePair.Value.Info.PropertyType, options);
            }

            writer.WriteEndObject();
        }
    }
}