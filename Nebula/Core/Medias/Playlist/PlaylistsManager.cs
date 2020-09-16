using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Nebula.Core.Data.Xml;
using Nebula.Core.Extensions;
using Nebula.Core.Medias.Playlist.Events;
using Nebula.Core.Medias.Playlist.Playlists;
using Nebula.Core.Settings;

namespace Nebula.Core.Medias.Playlist
{
    public class PlaylistsManager : IEnumerable<IPlaylist>
    {
        private const string PlaylistsFolderName               = "playlists";
        private const string PlaylistsThumbnailCacheFolderName = "thumbnails_cache";

        public PlaylistsManager()
        {
            PlaylistsDirectory =
                new DirectoryInfo(Path.Combine(NebulaSettings.SettingsDirectory.FullName, PlaylistsFolderName));
            ThumbnailCacheDirectory =
                new DirectoryInfo(Path.Combine(PlaylistsDirectory.FullName, PlaylistsThumbnailCacheFolderName));
            PlaylistsDirectory.Create();
            ThumbnailCacheDirectory.Create();
        }

        private List<IPlaylist> Playlists               { get; } = new List<IPlaylist>();
        public  DirectoryInfo   PlaylistsDirectory      { get; }
        public  DirectoryInfo   ThumbnailCacheDirectory { get; }

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

        public void LoadPlaylists()
        {
            Playlists.Clear();
            foreach (FileInfo fileInfo in PlaylistsDirectory.GetFiles())
            {
                if (LoadPlaylist(fileInfo) is { } playlist)
                    AddPlaylist(playlist);
            }
        }

        public IPlaylist LoadPlaylist(FileInfo fileInfo)
        {
            try
            {
                if (!fileInfo.Exists || !fileInfo.FullName.EndsWith(".playlist"))
                    return default;
                NebulaPlaylist playlist = new NebulaPlaylist {File = new XmlDataFile(fileInfo)};
                if (playlist.File.Load(playlist))
                    return playlist;
                NebulaClient.Notifications.NotifyError("ErrorFailedToLoadPlaylist", Path.GetFileNameWithoutExtension(fileInfo.FullName));
                return default;
            }
            catch
            {
                NebulaClient.Notifications.NotifyError("ErrorFailedToLoadPlaylist", Path.GetFileNameWithoutExtension(fileInfo.FullName));
            }

            return default;
        }

        public void SavePlaylist(IPlaylist playlist)
        {
            playlist.Name = playlist.Name.ReplaceInvalidPathChars();
            playlist.File ??= new XmlDataFile(new FileInfo(Path.Combine(PlaylistsDirectory.FullName, $"{playlist.Name}.playlist")));
            if (!playlist.File.Save(playlist))
                NebulaClient.Notifications.NotifyError("ErrorFailedToSavePlaylist", playlist.Name);
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
            if (NebulaClient.MediaPlayer.CurrentPlaylist == e.Playlist && !NebulaClient.MediaPlayer.MediaQueue.IsEmpty)
                NebulaClient.MediaPlayer.MediaQueue.Enqueue(e.AddedMedia);
        }
    }
}