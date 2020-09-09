using System;

namespace Nebula.Core.Medias.Player.Events
{
    public class StateChangedEventArgs : EventArgs
    {
        public StateChangedEventArgs(MediaPlayerState oldState, MediaPlayerState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        public MediaPlayerState OldState { get; }
        public MediaPlayerState NewState { get; }
    }
}