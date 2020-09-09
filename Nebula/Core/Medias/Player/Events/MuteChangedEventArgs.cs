using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class MuteChangedEventArgs : EventArgs
    {
        public MuteChangedEventArgs(bool muted)
        {
            IsMuted = muted;
        }

        /// <summary>
        /// True if the sound is muted, false otherwise.
        /// </summary>
        public bool IsMuted { get; }
    }
}