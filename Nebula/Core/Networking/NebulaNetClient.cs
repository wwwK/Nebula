using System.Net;
using System.Windows;
using LiteNetLib;
using LiteNetLib.Utils;
using Nebula.Core.Dialogs;
using Nebula.Shared.Packets;
using Nebula.Shared.Packets.S2C;

namespace Nebula.Core.Networking
{
    public class NebulaNetClient
    {
        public NebulaNetClient()
        {
            ClientListener = new EventBasedNetListener();
            PacketProcessor = new NetPacketProcessor();
            Client = new NetManager(ClientListener) {UnsyncedEvents = true, AutoRecycle = true, UnconnectedMessagesEnabled = true};
            ClientListener.PeerConnectedEvent += OnPeerConnectedEvent;
            ClientListener.PeerDisconnectedEvent += OnPeerDisconnectedEvent;
            ClientListener.NetworkReceiveEvent += OnNetworkReceiveEvent;
        }

        private NetManager            Client          { get; }
        private EventBasedNetListener ClientListener  { get; }
        public NetPacketProcessor    PacketProcessor { get; }
        private NetPeer               ServerPeer      { get; set; }
        public  bool                  IsConnected     => ServerPeer != null;

        public void Connect()
        {
            if (IsConnected)
                return;
            Client.Start();
            Client.Connect(NebulaClient.Settings.General.ServerIp, NebulaClient.Settings.General.ServerPort, "");
        }

        public void Disconnect()
        {
            if (!IsConnected)
                return;
        }

        public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            if (!IsConnected)
                return;
            PacketProcessor.Send(ServerPeer, packet, deliveryMethod);
        }

        private void OnPeerConnectedEvent(NetPeer peer)
        {
            ServerPeer = peer;
            SendPacket(new UserInfosPacket
            {
                Name = NebulaClient.Settings.UserProfile.Username,
                ThumbnailUrl = ""
            });
            MessageBox.Show("Connected");
        }

        private void OnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            ServerPeer = null;
        }

        private void OnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketProcessor.ReadAllPackets(reader, peer);
        }
    }
}