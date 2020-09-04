using LiteNetLib;

namespace Nebula.Net
{
    public class NetClient
    {
        public NetClient(NetPeer peer)
        {
            Peer = peer;
        }

        public NetPeer Peer       { get; }
        public int     BadPackets { get; set; }
        public int     Id         => Peer?.Id ?? -1;
    }
}