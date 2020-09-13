using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Nebula.Core.Files.Converters;

namespace Nebula.Core.Files
{
    public static class FilesManager
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true,
            IgnoreReadOnlyProperties = true,
            Converters =
            {
                new MediasCollectionConverter(),
                new MediaInfoConverter(),
                new TimeSpanJsonConverter()
            }
        };

        public static async Task SaveJsonAsync(string filePath, object obj)
        {
            if (obj == null)
                return;
            await SaveJsonAsync(filePath, obj, obj.GetType());
        }

        public static async Task SaveJsonAsync(string filePath, object obj, Type type)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            await using FileStream fileStream = File.OpenWrite(filePath);
            await JsonSerializer.SerializeAsync(fileStream, obj, type, SerializerOptions);
        }

        public static async Task<T> LoadJsonAsync<T>(string filePath)
        {
            if (!File.Exists(filePath))
                return default;
            await using FileStream fileStream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<T>(fileStream, SerializerOptions);
        }

        public static string ReplaceInvalidChars(string filename)
        {
            return string.Join("", filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}