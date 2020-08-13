using System.Collections.Generic;
using System.Linq;
using Nebula.Core.Medias.Playlist.Events;

namespace Nebula.Core.Medias.Playlist
{
    public class PlaylistsManager
    {
        public PlaylistsManager()
        {
            LoadPlaylists();
        }

        private List<IPlaylist> Playlists { get; set; }

        private void LoadPlaylists()
        {
            Playlists = new List<IPlaylist>();
            foreach (IPlaylist playlist in NebulaClient.Settings.LoadPlaylists())
                Playlists.Add(playlist);
        }

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
            NebulaClient.Settings.DeletePlaylist(playlist);
            playlist.MediaAdded -= OnPlaylistMediaAdded;
        }

        public void SavePlaylists()
        {
        }

        private void OnPlaylistMediaAdded(object sender, PlaylistMediaAddedEventArgs e)
        {
            if (NebulaClient.MediaPlayer.CurrentPlaylist == e.Playlist && !NebulaClient.MediaPlayer.Queue.IsEmpty)
                NebulaClient.MediaPlayer.Queue.Enqueue(e.AddedMedia);
        }
    }
}