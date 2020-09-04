using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Nebula.Net.Packets;

namespace Nebula.Net
{
    public abstract class BaseNetManager : INetEventListener
    {
        protected BaseNetManager()
        {
            NetManager = new NetManager(this) {UnsyncedEvents = true, AutoRecycle = true};
            PacketProcessor = new NetPacketProcessor();
            PacketProcessor.RegisterNestedType<SharedSessionInfo>();
            PacketProcessor.RegisterNestedType<UserInfo>();
        }

        public NetManager         NetManager      { get; }
        public NetPacketProcessor PacketProcessor { get; }

        public void SendPacket<T>(T packet, NetPeer user, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            PacketProcessor.Send(user, packet, method);
        }

        public virtual void OnPeerConnected(NetPeer peer)
        {
        }

        public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
        }

        public virtual void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public virtual void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketProcessor.ReadAllPackets(reader, peer);
        }

        public virtual void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public virtual void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public virtual void OnConnectionRequest(ConnectionRequest request)
        {
        }
    }
}