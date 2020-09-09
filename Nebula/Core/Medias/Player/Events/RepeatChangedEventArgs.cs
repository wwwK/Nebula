using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class RepeatChangedEventArgs : EventArgs
    {
        public RepeatChangedEventArgs(bool repeat)
        {
            Repeat = repeat;
        }

        /// <summary>
        /// True if repeat is enabled, false otherwise
        /// </summary>
        public bool Repeat { get; }
    }
}