using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class PlaybackVolumeChangedEventArgs : EventArgs
    {
        public PlaybackVolumeChangedEventArgs(int oldVolume, int newVolume)
        {
            OldVolume = oldVolume;
            NewVolume = newVolume;
        }

        /// <summary>
        /// Previous volume value.
        /// </summary>
        public int OldVolume { get; }

        /// <summary>
        /// New volume value.
        /// </summary>
        public int NewVolume { get; }
    }
}