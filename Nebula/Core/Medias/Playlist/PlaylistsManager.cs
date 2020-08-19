using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Nebula.Core.Medias.Playlist.Events;

namespace Nebula.Core.Medias.Playlist
{
    public class PlaylistsManager : IEnumerable<IPlaylist>
    {
        private const string PlaylistsFolderName               = "playlists";
        private const string PlaylistsThumbnailCacheFolderName = "thumbnails_cache";

        public PlaylistsManager()
        {
            PlaylistsDirectory =
                new DirectoryInfo(Path.Combine(NebulaClient.Settings.SettingsDirectory.FullName, PlaylistsFolderName));
            ThumbnailCacheDirectory =
                new DirectoryInfo(Path.Combine(PlaylistsDirectory.FullName, PlaylistsThumbnailCacheFolderName));
            PlaylistsDirectory.Create();
            ThumbnailCacheDirectory.Create();
            LoadPlaylists();
        }

        private List<IPlaylist> Playlists               { get; } = new List<IPlaylist>();
        private DirectoryInfo   PlaylistsDirectory      { get; }
        private DirectoryInfo   ThumbnailCacheDirectory { get; }

        public List<IPlaylist> GetPlaylists()
        {
            return Playlists.ToList();
        }

        public void AddPlaylist(IPlaylist playlist)
        {
            if (Playlists.Contains(playlist))
                return;
            Playlists.Add(playlist);
            playlist.MediaAdded += OnPlaylistMediaAdded;
        }

        public void RemovePlaylist(IPlaylist playlist)
        {
            if (!Playlists.Contains(playlist))
                return;
            Playlists.Remove(playlist);
            DeletePlaylist(playlist);
            playlist.MediaAdded -= OnPlaylistMediaAdded;
        }

        public IEnumerator<IPlaylist> GetEnumerator()
        {
            return Playlists.GetEnumerator();
        }

        private void LoadPlaylists()
        {
            foreach (FileInfo fileInfo in PlaylistsDirectory.GetFiles())
            {
                try
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
                        thumbnailUri = new Uri(Path.Combine(ThumbnailCacheDirectory.FullName,
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
                    Playlists.Add(playlist);
                }
                catch
                {
                }
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
                    string filePath = Path.Combine(ThumbnailCacheDirectory.FullName, thumbnailFileName);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void OnPlaylistMediaAdded(object sender, PlaylistMediaAddedEventArgs e)
        {
            if (NebulaClient.MediaPlayer.CurrentPlaylist == e.Playlist && !NebulaClient.MediaPlayer.Queue.IsEmpty)
                NebulaClient.MediaPlayer.Queue.Enqueue(e.AddedMedia);
        }
    }
}