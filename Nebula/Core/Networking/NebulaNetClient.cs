using System;
using System.Windows;
using LiteNetLib;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Provider;
using Nebula.Core.Networking.Events;
using Nebula.Core.SharedSessions;
using Nebula.Net;
using Nebula.Net.Packets;
using Nebula.Net.Packets.BOTH;
using Nebula.Net.Packets.C2S;
using Nebula.Net.Packets.S2C;

namespace Nebula.Core.Networking
{
    public class NebulaNetClient : BaseNetManager
    {
        private const string OfficialIp   = "35.181.56.125";
        private const int    OfficialPort = 9080;

        public NebulaNetClient()
        {

        }

        private NetPeer ServerPeer  { get; set; }
        public  bool    IsConnected => ServerPeer != null;

        public event EventHandler<ConnectedToServerEventArgs>      Connected;
        public event EventHandler<DisconnectedFromServerEventArgs> Disconnected;

        public void Connect()
        {
            if (IsConnected)
                return;
            NetManager.Start();
            if (NebulaClient.Settings.Server.ConnectToCustomServer)
                NetManager.Connect(NebulaClient.Settings.Server.ServerIp, NebulaClient.Settings.Server.ServerPort,
                    NebulaClient.Settings.Server.ServerConnectionKey);
            else
                NetManager.Connect(OfficialIp, OfficialPort, string.Empty);
        }

        public void Disconnect()
        {
            if (!IsConnected)
                return;
            NetManager.Stop(true);
            NebulaClient.SharedSession.Leave();
        }

        public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            if (!IsConnected)
                return;
            PacketProcessor.Send(ServerPeer, packet, deliveryMethod);
        }

        public override void OnPeerConnected(NetPeer peer)
        {
            ServerPeer = peer;
            SendPacket(new UserInfoPacket
            {
                UserInfo = new UserInfo
                {
                    Id = -1,
                    Username = NebulaClient.Settings.UserProfile.Username,
                    AvatarUrl = NebulaClient.Settings.UserProfile.Avatar,
                }
            });
            if (Connected != null)
                NebulaClient.Invoke(() => Connected.Invoke(this, new ConnectedToServerEventArgs(peer)));
        }

        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            ServerPeer = null;
            NebulaClient.BeginInvoke(() =>
            {
                NebulaClient.Notifications.NotifyOk("", "ServerConnectionLost", "#ff0000");
                Disconnected?.Invoke(this, new DisconnectedFromServerEventArgs(peer, disconnectInfo));
                if (!NebulaClient.SharedSession.IsSessionActive)
                    return;
                NebulaClient.SharedSession.Leave();
            });
        }

        public override void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketProcessor.ReadAllPackets(reader, this);
        }
    }
}