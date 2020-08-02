using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class PlaybackMuteChangedEventArgs : EventArgs
    {
        public PlaybackMuteChangedEventArgs(bool muted)
        {
            IsMuted = muted;
        }

        public bool IsMuted { get; }
    }
}