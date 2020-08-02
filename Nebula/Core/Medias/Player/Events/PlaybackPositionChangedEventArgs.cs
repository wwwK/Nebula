using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class PlaybackPositionChangedEventArgs : EventArgs
    {
        public PlaybackPositionChangedEventArgs(TimeSpan position)
        {
            Position = position;
        }

        public TimeSpan Position { get; }
    }
}