using System;
using LiteNetLib;
using LiteNetLib.Utils;
using Nebula.Server.SharedSession;

namespace Nebula.Server
{
    public static class ServerApp
    {
        private static NetManager            Server                { get; }
        private static EventBasedNetListener ServerListener        { get; }
        private static SharedSessionsManager SharedSessionsManager { get; }
        public static  NetPacketProcessor    PacketProcessor       { get; }

        static ServerApp()
        {
            ServerListener = new EventBasedNetListener();
            PacketProcessor = new NetPacketProcessor();
            Server = new NetManager(ServerListener) {UnsyncedEvents = true};
            SharedSessionsManager = new SharedSessionsManager();
            ServerListener.NetworkReceiveEvent += OnNetworkReceive;
        }

        private static void Main(string[] args)
        {
        }

        private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            PacketProcessor.ReadAllPackets(reader, peer);
        }

        public static void ReadLine()
        {
            string value = Console.ReadLine();
            if (value == "stop")
            {
                Server.Stop(true);
                Environment.Exit(0);
                return;
            }

            ReadLine();
        }
    }
}