using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class PlaybackRepeatChangedEventArgs : EventArgs
    {
        public PlaybackRepeatChangedEventArgs(bool repeat)
        {
            Repeat = repeat;
        }
        
        public bool Repeat { get; }
    }
}