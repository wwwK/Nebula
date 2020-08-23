using System;
using System.Collections.Generic;
using LiteNetLib;
using Nebula.Shared.Packets.C2S;

namespace Nebula.Server.Lobby
{
    public static class LobbiesManager
    {
        public static  bool                          IsInitialized { get; private set; }
        private static Dictionary<Guid, ServerLobby> Lobbies       { get; } = new Dictionary<Guid, ServerLobby>();

        public static void Init()
        {
            if (IsInitialized)
                return;
            App.NebulaServer.PacketProcessor.SubscribeReusable<LobbyCreationRequest, NetPeer>(OnReceiveCreationRequest);
            App.NebulaServer.PacketProcessor.SubscribeReusable<LobbyUpdateRequest, NetPeer>(OnReceiveUpdateRequest);
        }

        public static void CreateLobby(ServerLobby lobby)
        {
            if (!Guid.TryParse(lobby.Guid, out Guid lobbyGuid) || Lobbies.ContainsKey(lobbyGuid))
                return;
            Lobbies.Add(lobbyGuid, lobby);
        }

        public static void RemoveLobby(ServerLobby lobby)
        {
        }

        private static void OnReceiveCreationRequest(LobbyCreationRequest request, NetPeer peer)
        {
        }

        private static void OnReceiveUpdateRequest(LobbyUpdateRequest request, NetPeer peer)
        {
        }
    }
}