using System.Windows;
using LiteNetLib;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Provider;
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
        public NebulaNetClient()
        {
            PacketProcessor.SubscribeReusable<SharedSessionPlayMediaPacket, NetPeer>(OnReceivePlayMediaPacket);
            PacketProcessor.SubscribeReusable<SharedSessionStartPlayingPacket, NetPeer>(OnReceivePlayPacket);
            PacketProcessor.SubscribeReusable<SharedSessionPausePacket, NetPeer>(OnReceiveSessionPausePacket);
            PacketProcessor.SubscribeReusable<SharedSessionResumePacket, NetPeer>(OnReceiveSessionResumePacket);
            PacketProcessor.SubscribeReusable<SharedSessionPositionChangedPacket, NetPeer>(OnReceiveSessionPositionChangedPacket);
        }

        private NetPeer ServerPeer  { get; set; }
        public  bool    IsConnected => ServerPeer != null;

        public void Connect()
        {
            if (IsConnected)
                return;
            NetManager.Start();
            NetManager.Connect(NebulaClient.Settings.General.ServerIp, NebulaClient.Settings.General.ServerPort, "");
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
        }

        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            ServerPeer = null;
            NebulaClient.BeginInvoke(() =>
            {
                NebulaClient.Notifications.NotifyOk("", "ServerConnectionLost", "#ff0000");
                if (!NebulaClient.SharedSession.IsSessionActive)
                    return;
                NebulaClient.SharedSession.Leave();
            });
        }

        private void OnReceivePlayMediaPacket(SharedSessionPlayMediaPacket packet, NetPeer peer)
        {
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            NebulaClient.BeginInvoke(async () =>
            {
                NebulaClient.SharedSession.AddMessage(new SharedSessionMessage(packet.UserInfo,
                    NebulaClient.GetLocString("SharedSessionMessagePlayMedia", packet.MediaName), "#ffee00"));
                IMediaProvider provider = NebulaClient.GetMediaProviderByName(packet.Provider);
                if (provider == null)
                {
                    Disconnect();
                    return;
                }

                IMediaInfo mediaInfo = await provider.GetMediaInfo(packet.MediaId);
                if (mediaInfo == null)
                    return;
                await NebulaClient.MediaPlayer.OpenMedia(mediaInfo, true, false, true);
                SendPacket(new SharedSessionPlayReadyPacket()); //Todo: maybe move in media player class
            });
        }

        private void OnReceivePlayPacket(SharedSessionStartPlayingPacket packet, NetPeer peer)
        {
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() => NebulaClient.MediaPlayer.Play());
        }

        private void OnReceiveSessionPausePacket(SharedSessionPausePacket packet, NetPeer peer)
        {
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() => NebulaClient.MediaPlayer.Pause(true));
        }

        private void OnReceiveSessionResumePacket(SharedSessionResumePacket packet, NetPeer peer)
        {
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() => NebulaClient.MediaPlayer.Resume(true));
        }

        private void OnReceiveSessionPositionChangedPacket(SharedSessionPositionChangedPacket packet, NetPeer peer)
        {
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() => NebulaClient.MediaPlayer.SetPosition(packet.NewPosition, true));
        }
    }
}