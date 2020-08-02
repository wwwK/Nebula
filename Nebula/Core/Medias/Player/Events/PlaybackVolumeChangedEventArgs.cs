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

        public int OldVolume { get; }

        public int NewVolume { get; }
    }
}