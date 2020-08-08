using System.Collections.Generic;
using System.Linq;

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
        }

        public void RemovePlaylist(IPlaylist playlist)
        {
            if (!Playlists.Contains(playlist))
                return;
            Playlists.Remove(playlist);
        }

        public void SavePlaylists()
        {
            
        }
    }
}