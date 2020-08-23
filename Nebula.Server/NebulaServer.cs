using LiteNetLib;
using LiteNetLib.Utils;

namespace Nebula.Server
{
    public class NebulaServer
    {
        public NebulaServer()
        {
            ServerListener = new EventBasedNetListener();
            PacketProcessor = new NetPacketProcessor();
            Server = new NetManager(ServerListener) {UnsyncedEvents = true};
            ServerListener.NetworkReceiveEvent += OnNetworkReceive;
        }

        public NetManager            Server          { get; }
        public EventBasedNetListener ServerListener  { get; }
        public NetPacketProcessor    PacketProcessor { get; }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketProcessor.ReadAllPackets(reader, peer);
        }
    }
}