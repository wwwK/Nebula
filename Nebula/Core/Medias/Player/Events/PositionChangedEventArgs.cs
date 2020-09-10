using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class PositionChangedEventArgs : EventArgs
    {
        public PositionChangedEventArgs(TimeSpan position)
        {
            Position = position;
        }

        public TimeSpan Position { get; }
    }
}