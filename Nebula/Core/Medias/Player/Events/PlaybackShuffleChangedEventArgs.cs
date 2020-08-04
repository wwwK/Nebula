using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class PlaybackShuffleChangedEventArgs : EventArgs
    {
        public PlaybackShuffleChangedEventArgs(bool shuffle)
        {
            Shuffle = shuffle;
        }

        public bool Shuffle { get; }
    }
}