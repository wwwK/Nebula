using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class PlaybackRepeatChangedEventArgs : EventArgs
    {
        public PlaybackRepeatChangedEventArgs(bool repeat)
        {
            Repeat = repeat;
        }

        /// <summary>
        /// True if repeat is enabled, false otherwise
        /// </summary>
        public bool Repeat { get; }
    }
}