using System;
using Nebula.Core.Medias.Playlist;

namespace Nebula.Core.Medias.Player.Events
{
    public class PlaylistChangedEventArgs : EventArgs
    {
        public PlaylistChangedEventArgs(IPlaylist old, IPlaylist @new)
        {
            OldPlaylist = old;
            NewPlaylist = @new;
        }

        public IPlaylist OldPlaylist { get; }
        public IPlaylist NewPlaylist { get; }
    }
}