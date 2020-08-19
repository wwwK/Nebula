using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class PlaybackMuteChangedEventArgs : EventArgs
    {
        public PlaybackMuteChangedEventArgs(bool muted)
        {
            IsMuted = muted;
        }

        /// <summary>
        /// True if the sound is muted, false otherwise.
        /// </summary>
        public bool IsMuted { get; }
    }
}