using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class PlaybackPositionChangedEventArgs : EventArgs
    {
        public PlaybackPositionChangedEventArgs(TimeSpan position)
        {
            Position = position;
        }

        /// <summary>
        /// Playback Position.
        /// </summary>
        public TimeSpan Position { get; }
    }
}