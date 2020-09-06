using LiteNetLib;

namespace Nebula.Core.Networking.Events
{
    public class DisconnectedFromServerEventArgs
    {
        public DisconnectedFromServerEventArgs(NetPeer serverPeer, DisconnectInfo disconnectInfo)
        {
            ServerPeer = serverPeer;
            DisconnectInfo = disconnectInfo;
        }

        public NetPeer        ServerPeer     { get; }
        public DisconnectInfo DisconnectInfo { get; }
    }
}