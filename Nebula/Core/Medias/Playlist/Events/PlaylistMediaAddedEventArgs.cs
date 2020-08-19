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

        /// <summary>
        /// Modified Playlist.
        /// </summary>
        public IPlaylist  Playlist   { get; }
        
        /// <summary>
        /// New Media.
        /// </summary>
        public IMediaInfo AddedMedia { get; }
        
        /// <summary>
        /// New Media Insert Index.
        /// </summary>
        public int        MediaIndex { get; }
    }
}