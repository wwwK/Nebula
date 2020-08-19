using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class PlaybackShuffleChangedEventArgs : EventArgs
    {
        public PlaybackShuffleChangedEventArgs(bool shuffle)
        {
            Shuffle = shuffle;
        }

        /// <summary>
        /// True if shuffle is enabled, false otherwise.
        /// </summary>
        public bool Shuffle { get; }
    }
}