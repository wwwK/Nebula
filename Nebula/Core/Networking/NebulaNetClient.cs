using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Documents;
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
            PacketProcessor.RegisterNestedType(UserInfosPacket.Serialize, UserInfosPacket.Deserialize);
            PacketProcessor.SubscribeReusable<SharedSessionJoinResponse, NetPeer>(OnReceiveSharedSessionJoinResponse);
            PacketProcessor.SubscribeReusable<TestPacket, NetPeer>(OnReceive);
        }

        private void OnReceive(TestPacket arg1, NetPeer arg2)
        {
            MessageBox.Show(arg1.Users?.Length + "");
        }

        private NetManager            Client          { get; }
        private EventBasedNetListener ClientListener  { get; }
        public  NetPacketProcessor    PacketProcessor { get; }
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
        }

        private void OnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            ServerPeer = null;
            NebulaClient.SetSharedSession(null);
        }

        private void OnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketProcessor.ReadAllPackets(reader, peer);
        }

        private void OnReceiveSharedSessionJoinResponse(SharedSessionJoinResponse response, NetPeer peer)
        {
            switch (response.ResponseCode)
            {
                case 0:
                {
                    Guid id = Guid.Parse(response.RoomId);
                    List<UserInfosPacket> users;
                    if (response.CurrentUsers > 0 && response.Users != null)
                    {
                        users = new List<UserInfosPacket>(response.CurrentUsers);
                        foreach (string userString in response.Users)
                        {
                            UserInfosPacket userInfo = UserInfosPacket.FromString(userString);
                            if (userInfo == null)
                                continue;
                            users.Add(userInfo);
                        }
                    }
                    else
                        users = new List<UserInfosPacket>();

                    NebulaSharedSession session = new NebulaSharedSession(id, response.RoomName, "", users, response.MaxUsers, response.PasswordProtected);
                    NebulaClient.SetSharedSession(session);
                    MessageBox.Show("Success");
                }
                    break;
                case 10:
                    NebulaMessageBox.ShowOk("SharedSessionCantJoin", "Session does not exists");
                    break;
                case 11:
                    break;
                case 12:
                    MessageBox.Show("DD");
                    NebulaMessageBox.ShowOk("SharedSessionCantJoin", "SharedSessionWrongPassword");
                    break;
            }
        }
    }
}