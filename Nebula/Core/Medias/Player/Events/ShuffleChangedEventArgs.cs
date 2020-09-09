using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class ShuffleChangedEventArgs : EventArgs
    {
        public ShuffleChangedEventArgs(bool shuffle)
        {
            Shuffle = shuffle;
        }

        /// <summary>
        /// True if shuffle is enabled, false otherwise.
        /// </summary>
        public bool Shuffle { get; }
    }
}