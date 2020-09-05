using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Nebula.Net;
using Nebula.Net.Packets.C2S;
using Nebula.Server.Commands;
using Nebula.Server.Events;
using Nebula.Server.SharedSession;
using Nebula.Server.Users;
using static Nebula.Server.ServerApp;

namespace Nebula.Server
{
    public class NebulaServer : BaseNetManager
    {
        public NebulaServer()
        {
            SharedSessionsManager = new SharedSessionsManager(this);
            PacketProcessor.SubscribeReusable<UserInfoPacket, NebulaUser>(OnReceiveUserInfos);
            CommandsManager.RegisterCommand(new StopServerCommand());
            CommandsManager.RegisterCommand(new ClearConsoleCommand());
        }

        private Dictionary<int, NebulaUser> ConnectedUsers        { get; } = new Dictionary<int, NebulaUser>();
        public  CommandsManager             CommandsManager       { get; } = new CommandsManager();
        public  SharedSessionsManager       SharedSessionsManager { get; }
        public  NebulaServerUser            ServerUser            { get; } = new NebulaServerUser();
        public  int                         MaximumConnections    { get; } = 1000;
        public  int                         MaximumUserBadPackets { get; } = 100;
        public  string                      ServerKey             { get; } = String.Empty;

        public event EventHandler<UserDisconnectedEventArgs> UserDisconnected;

        public void StartServer(int port = 9080)
        {
            WriteLine("Starting Server...", ConsoleColor.Yellow);
            if (NetManager.Start(IPAddress.Any, IPAddress.IPv6Any, port))
                WriteLine($"Server started ! Listening on port {NetManager.LocalPort}", ConsoleColor.Green);
            else
                WriteLine("Failed to start server.", ConsoleColor.Red);
        }

        public void StopServer()
        {
            WriteLine("Stopping Server...", ConsoleColor.Yellow);
            NetManager.Stop(true);
            WriteLine("Server Stopped ! Press any key to exit.", ConsoleColor.Red);
            Console.ReadLine();
            Environment.Exit(0);
        }

        public bool IsConnected(int userId)
        {
            return ConnectedUsers.ContainsKey(userId);
        }

        public NebulaUser GetUser(int userId)
        {
            if (!IsConnected(userId))
                return null;
            return ConnectedUsers[userId];
        }

        public void HandleBadPacket(NebulaUser user)
        {
            user.BadPackets++;
            if (user.BadPackets >= MaximumUserBadPackets)
                user.Peer.Disconnect(NetDataWriter.FromString($"Too many bad packets ({user.BadPackets})"));
            WriteLine($"Handled bad packet for user '{user.Id}'. User has a total of {user.BadPackets} bad packets.", ConsoleColor.Red);
        }

        public override void OnConnectionRequest(ConnectionRequest request)
        {
            WriteLine($"Connection Request from '{request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port}'", ConsoleColor.Cyan);
            if (NetManager.ConnectedPeersCount >= MaximumConnections)
            {
                request.Reject(NetDataWriter.FromString("Server Full"));
                WriteLine($"Refused Connection from '{request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port}'. Reason: Server Full", ConsoleColor.DarkRed);
                return;
            }

            if (!String.IsNullOrWhiteSpace(ServerKey))
                request.AcceptIfKey(ServerKey);
            else
                request.Accept();
        }

        public override void OnPeerConnected(NetPeer peer)
        {
            ConnectedUsers.Add(peer.Id, new NebulaUser(peer));
            WriteLine($"Client Connected '{peer.EndPoint.Address}:{peer.EndPoint.Port}'", ConsoleColor.Cyan);
        }

        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            NebulaUser user = GetUser(peer.Id);
            if (user != null)
                ConnectedUsers.Remove(user.Id);
            UserDisconnected?.Invoke(this, new UserDisconnectedEventArgs(user, disconnectInfo));
            WriteLine($"Client Disconnected '{peer.EndPoint.Address}:{peer.EndPoint.Port}'. Reason: {disconnectInfo.Reason} ({disconnectInfo.SocketErrorCode})",
                ConsoleColor.Red);
        }

        public override void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            WriteLine($"Network Error '{socketError.ToString()}' from {endPoint.Address}:{endPoint.Port}", ConsoleColor.Red);
        }

        public override void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            NebulaUser user = GetUser(peer.Id);
            if (user == null)
            {
                peer.Disconnect(NetDataWriter.FromString("User not registered server side"));
                WriteLine($"Received packet from '{peer.EndPoint.Address}:{peer.EndPoint.Port}' but the user is not registered.", ConsoleColor.Red);
                return;
            }

            try
            {
                PacketProcessor.ReadAllPackets(reader, user);
            }
            catch
            {
                HandleBadPacket(user);
            }
        }

        private void OnReceiveUserInfos(UserInfoPacket infosPacket, NebulaUser user)
        {
            if (user.Username == infosPacket.UserInfo.Username && user.AvatarUrl == infosPacket.UserInfo.AvatarUrl)
                HandleBadPacket(user);
            else
            {
                user.Username = infosPacket.UserInfo.Username;
                user.AvatarUrl = infosPacket.UserInfo.AvatarUrl;
            }
        }
    }
}