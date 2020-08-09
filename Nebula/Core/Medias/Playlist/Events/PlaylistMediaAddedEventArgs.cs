using System;

namespace Nebula.Core.Medias.Playlist.Events
{
    public class PlaylistMediaAddedEventArgs : EventArgs
    {
        public PlaylistMediaAddedEventArgs(IPlaylist playlist, IMediaInfo mediaInfo, int mediaIndex)
        {
            Playlist = playlist ?? throw new ArgumentNullException(nameof(playlist));
            AddedMedia = mediaInfo ?? throw new ArgumentNullException(nameof(mediaInfo));
            MediaIndex = mediaIndex;
        }

        public IPlaylist  Playlist   { get; }
        public IMediaInfo AddedMedia { get; }
        public int        MediaIndex { get; }
    }
}