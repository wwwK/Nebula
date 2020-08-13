using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Playlist;
using YoutubeExplode.Playlists;

namespace Nebula.Core
{
    public class NebulaSettings
    {
        public const string SettingsFolderName                = "Nebula";
        public const string PlaylistsFolderName               = "playlists";
        public const string PlaylistsThumbnailCacheFolderName = "thumbnails_cache";
        public const string SettingsFileName                  = "NebulaSettings.json";

        public NebulaSettings()
        {
            SettingsDirectory =
                new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    SettingsFolderName));
            PlaylistsDirectory = new DirectoryInfo(Path.Combine(SettingsDirectory.FullName, PlaylistsFolderName));
            PlaylistsThumbnailSCacheDirectory =
                new DirectoryInfo(Path.Combine(PlaylistsDirectory.FullName, PlaylistsThumbnailCacheFolderName));
            LoadSettings();
        }

        public DirectoryInfo SettingsDirectory                 { get; }
        public DirectoryInfo PlaylistsDirectory                { get; }
        public DirectoryInfo PlaylistsThumbnailSCacheDirectory { get; }

        private void LoadSettings()
        {
            if (!SettingsDirectory.Exists)
                SettingsDirectory.Create();
            if (!PlaylistsDirectory.Exists)
                PlaylistsDirectory.Create();
            if (!PlaylistsThumbnailSCacheDirectory.Exists)
                PlaylistsThumbnailSCacheDirectory.Create();
        }

        public IEnumerable<IPlaylist> LoadPlaylists() //Todo: provider load support by type
        {
            foreach (FileInfo fileInfo in PlaylistsDirectory.GetFiles())
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileInfo.FullName);
                if (xmlDocument.DocumentElement == null)
                    continue;
                string playListName = xmlDocument.DocumentElement.GetAttribute("Name");
                string playListDescription = xmlDocument.DocumentElement.GetAttribute("Description");
                string playListAuthor = xmlDocument.DocumentElement.GetAttribute("Author");
                string thumbnail = xmlDocument.DocumentElement.GetAttribute("Thumbnail");
                Uri thumbnailUri;
                if (thumbnail.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!Uri.TryCreate(thumbnail, UriKind.RelativeOrAbsolute, out thumbnailUri))
                        thumbnailUri = new Uri("https://i.imgur.com/Od5XogD.png");
                }
                else
                    thumbnailUri = new Uri(Path.Combine(PlaylistsThumbnailSCacheDirectory.FullName,
                        xmlDocument.DocumentElement.GetAttribute("Thumbnail")));

                NebulaPlaylist playlist =
                    new NebulaPlaylist(playListName, playListDescription, playListAuthor,
                        thumbnailUri) {AutoSave = false};
                foreach (XmlElement child in xmlDocument.DocumentElement.ChildNodes)
                {
                    Type type = Type.GetType(child.GetAttribute("ProviderType"));
                    if (type == null)
                        continue;
                    object instance = Activator.CreateInstance(type, child);
                    if (instance is IMediaInfo mediaInfo)
                        playlist.AddMedia(mediaInfo);
                }

                playlist.AutoSave = true;
                yield return playlist;
            }
        }

        public void SavePlaylist(IPlaylist playlist)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement rootElement = xmlDocument.CreateElement(nameof(NebulaPlaylist));
            rootElement.SetAttribute("Name", playlist.Name);
            rootElement.SetAttribute("Description", playlist.Description);
            rootElement.SetAttribute("Author", playlist.Author);
            if (playlist.Thumbnail != null)
            {
                if (playlist.Thumbnail.ToString().StartsWith("http"))
                    rootElement.SetAttribute("Thumbnail", playlist.Thumbnail.ToString());
                else
                {
                    string thumbnailFileName =
                        $"{playlist.Name}_thumbnail{Path.GetExtension(playlist.Thumbnail.LocalPath)}";
                    rootElement.SetAttribute("Thumbnail", thumbnailFileName);
                    string filePath = Path.Combine(PlaylistsThumbnailSCacheDirectory.FullName, thumbnailFileName);
                    if (!File.Exists(filePath))
                        File.Copy(playlist.Thumbnail.LocalPath, filePath);
                }
            }

            xmlDocument.AppendChild(rootElement);
            foreach (IMediaInfo mediaInfo in playlist)
            {
                XmlElement mediaElement = xmlDocument.CreateElement("Media");
                mediaElement.SetAttribute("ProviderType", mediaInfo.GetType().FullName);
                mediaElement.SetAttribute("Id", mediaInfo.Id);
                mediaElement.SetAttribute("OwnerId", mediaInfo.OwnerId);
                mediaElement.SetAttribute("Title", mediaInfo.Title);
                mediaElement.SetAttribute("Description", mediaInfo.Description);
                mediaElement.SetAttribute("Author", mediaInfo.Author);
                mediaElement.SetAttribute("Thumbnail", mediaInfo.ThumbnailUrl);
                mediaElement.SetAttribute("Duration", mediaInfo.Duration.TotalSeconds.ToString());
                rootElement.AppendChild(mediaElement);
            }

            xmlDocument.Save(Path.Combine(PlaylistsDirectory.FullName, playlist.Name + ".playlist"));
        }

        public void DeletePlaylist(IPlaylist playlist)
        {
            string filePath = $"{Path.Combine(PlaylistsDirectory.FullName, playlist.Name)}.playlist";
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public void RenamePlaylist(string oldName, IPlaylist playlist)
        {
            string filePath = $"{Path.Combine(PlaylistsDirectory.FullName, oldName)}.playlist";
            if (File.Exists(filePath))
                File.Delete(filePath);
            SavePlaylist(playlist);
        }

        private string ToBase64(BitmapImage bitmapImage)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using MemoryStream memStream = new MemoryStream();
            encoder.Save(memStream);
            return Convert.ToBase64String(memStream.ToArray());
        }

        private BitmapImage FromBase64(string base64)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64);
            MemoryStream memoryStream = new MemoryStream(byteBuffer) {Position = 0};
            BitmapImage bitmapImage = new BitmapImage {StreamSource = memoryStream};
            memoryStream.Close();
            return bitmapImage;
        }
    }
}