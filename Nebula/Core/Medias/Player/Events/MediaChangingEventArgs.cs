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
        /// Previous media played, this can be null if there was no previous media.
        /// </summary>
        public IMediaInfo OldMedia { get; }
        
        /// <summary>
        /// The new media being played, this can't be null
        /// </summary>
        public IMediaInfo NewMedia { get; }
    }
}