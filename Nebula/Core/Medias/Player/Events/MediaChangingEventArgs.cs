using System.ComponentModel;

namespace Nebula.Core.Medias.Player.Events
{
    public class MediaChangingEventArgs : CancelEventArgs
    {
        public MediaChangingEventArgs(IMediaInfo oldMedia, IMediaInfo newMedia)
        {
            OldMedia = oldMedia;
            NewMedia = newMedia;
        }
        
        /// <summary>
        /// Previous media <see cref="IMediaInfo"/>. This can be null if there was no previous media.
        /// </summary>
        public IMediaInfo OldMedia { get; }
        
        /// <summary>
        /// The new <see cref="IMediaInfo"/> going to be played. This can't be null.
        /// </summary>
        public IMediaInfo NewMedia { get; }
    }
}