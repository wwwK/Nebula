using System;
using LiteNetLib;

namespace Nebula.Core.Networking.Events
{
    public class ConnectedToServerEventArgs : EventArgs
    {
        public ConnectedToServerEventArgs(NetPeer serverPeer)
        {
            ServerPeer = serverPeer;
        }

        public NetPeer ServerPeer { get; }
    }
}