using System;
using Nebula.Core.Medias.Playlist;

namespace Nebula.Core.Medias.Player.Events
{
    public class MediaChangedEventArgs : EventArgs
    {
        public MediaChangedEventArgs(IPlaylist playlist, IMediaInfo oldMedia, IMediaInfo newMedia)
        {
            Playlist = playlist;
            OldMedia = oldMedia;
            NewMedia = newMedia;
        }

        /// <summary>
        /// Currently Played <see cref="IPlaylist"/>. This can be null if no playlist is being played.
        /// </summary>
        public IPlaylist Playlist { get; }

        /// <summary>
        /// Previous media <see cref="IMediaInfo"/>. This can be null if there was no previous media.
        /// </summary>
        public IMediaInfo OldMedia { get; }

        /// <summary>
        /// The new <see cref="IMediaInfo"/> being played. This can't be null.
        /// </summary>
        public IMediaInfo NewMedia { get; }
    }
}