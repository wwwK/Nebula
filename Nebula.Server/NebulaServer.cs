using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Nebula.Server.SharedSession;
using Nebula.Shared.Packets;
using Nebula.Shared.Packets.C2S;

namespace Nebula.Server
{
    public static class NebulaServer
    {
        private static Dictionary<int, NebulaUser> ConnectedUsers        { get; }      = new Dictionary<int, NebulaUser>();
        private static int                         MaxUsers              { get; set; } = 1000;
        public static  NetManager                  Server                { get; }
        public static  EventBasedNetListener       ServerListener        { get; }
        public static  SharedSessionsManager       SharedSessionsManager { get; }
        public static  NetPacketProcessor          PacketProcessor       { get; }

        static NebulaServer()
        {
            ServerListener = new EventBasedNetListener();
            PacketProcessor = new NetPacketProcessor();
            Server = new NetManager(ServerListener) {UnsyncedEvents = true, AutoRecycle = true};
            SharedSessionsManager = new SharedSessionsManager();
            ServerListener.ConnectionRequestEvent += OnConnectionRequestEvent;
            ServerListener.PeerConnectedEvent += OnPeerConnectedEvent;
            ServerListener.PeerDisconnectedEvent += OnPeerDisconnectedEvent;
            ServerListener.NetworkErrorEvent += OnNetworkErrorEvent;
            ServerListener.NetworkReceiveEvent += OnNetworkReceive;
            PacketProcessor.SubscribeReusable<UserInfosPacket, NetPeer>(OnReceiveUserInfos);
        }

        private static void Main(string[] args)
        {
            WriteLine("Starting Server...", ConsoleColor.Yellow);
            Server.Start(9080);
            WriteLine($"Server started ! Listening on port {Server.LocalPort}", ConsoleColor.Green);
            ReadLine();
        }

        public static NebulaUser FindUser(Predicate<NebulaUser> predicate)
        {
            for (int i = ConnectedUsers.Count; i-- > 0;)
            {
                NebulaUser nebulaUser = ConnectedUsers[i];
                if (predicate(nebulaUser))
                    return nebulaUser;
            }

            return null;
        }

        public static NebulaUser GetUserById(int id)
        {
            return ConnectedUsers.ContainsKey(id) ? ConnectedUsers[id] : null;
        }

        public static bool IsConnected(NetPeer peer)
        {
            return ConnectedUsers.ContainsKey(peer.Id);
        }

        public static NebulaUser GetUserByName(string name)
        {
            return FindUser(user => user.Name == name);
        }

        public static NebulaUser GetUserByPeer(NetPeer peer)
        {
            return FindUser(user => user.Peer == peer);
        }

        public static void SendPacket<T>(T packet, NetPeer user, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            PacketProcessor.Send(user, packet, method);
        }

        public static void WriteLine(string content, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(content);
            Console.ResetColor();
        }

        public static void ReadLine()
        {
            string value = Console.ReadLine();
            if (value == "stop")
            {
                WriteLine("Stopping Server...", ConsoleColor.Yellow);
                Server.Stop(true);
                WriteLine("Server Stopped !", ConsoleColor.Red);
                Environment.Exit(0);
                return;
            }

            ReadLine();
        }

        private static void OnConnectionRequestEvent(ConnectionRequest request)
        {
            WriteLine($"Connection Request from '{request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port}'", ConsoleColor.Cyan);
            if (Server.ConnectedPeersCount >= MaxUsers)
            {
                request.Reject(NetDataWriter.FromString("Server Full "));
                WriteLine($"Refused Connection from '{request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port}'. Reason: Server Full", ConsoleColor.DarkRed);
                return;
            }

            request.Accept();
        }

        private static void OnPeerConnectedEvent(NetPeer peer)
        {
            WriteLine($"Client Connected '{peer.EndPoint.Address}:{peer.EndPoint.Port}'", ConsoleColor.Cyan);
            ConnectedUsers.Add(peer.Id, new NebulaUser(peer));
        }

        private static void OnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            WriteLine($"Client Disconnected '{peer.EndPoint.Address}:{peer.EndPoint.Port}'. Reason: {disconnectInfo.Reason} ({disconnectInfo.SocketErrorCode})",
                ConsoleColor.Cyan);
            NebulaUser user = GetUserById(peer.Id);
            if (user == null)
                return;
            ConnectedUsers.Remove(user.Id);
        }

        private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketProcessor.ReadAllPackets(reader, peer);
        }

        private static void OnNetworkErrorEvent(IPEndPoint endpoint, SocketError socketError)
        {
            WriteLine($"Network Error '{socketError.ToString()}' from {endpoint.Address}:{endpoint.Port}", ConsoleColor.Red);
        }

        private static void OnReceiveUserInfos(UserInfosPacket infosPacket, NetPeer peer)
        {
            NebulaUser user = GetUserById(peer.Id);
            if (user == null)
            {
                peer.Disconnect(NetDataWriter.FromString("User not registered server side"));
                WriteLine($"Received user informations from '{peer.EndPoint.Address}:{peer.EndPoint.Port}' but the peer is not registered.", ConsoleColor.Red);
                return;
            }

            user.Name = infosPacket.Name;
            user.ThumbnailUrl = infosPacket.ThumbnailUrl;
        }
    }
}