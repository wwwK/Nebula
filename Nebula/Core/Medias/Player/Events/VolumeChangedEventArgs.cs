using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class VolumeChangedEventArgs : EventArgs
    {
        public VolumeChangedEventArgs(double oldVolume, double newVolume)
        {
            OldVolume = oldVolume;
            NewVolume = newVolume;
        }

        /// <summary>
        /// Previous volume value.
        /// </summary>
        public double OldVolume { get; }

        /// <summary>
        /// New volume value.
        /// </summary>
        public double NewVolume { get; }
    }
}