using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LiteNetLib;
using Nebula.Shared.Packets.C2S;
using Nebula.Shared.Packets.S2C;
using Nebula.Shared.SharedSession;
using static Nebula.Server.NebulaServer;

namespace Nebula.Server.SharedSession
{
    public class SharedSessionsManager
    {
        public SharedSessionsManager()
        {
            PacketProcessor.SubscribeReusable<SharedSessionRoomCreationRequest, NetPeer>(OnReceiveRoomCreationRequest);
            PacketProcessor.SubscribeReusable<SharedSessionsListRequest, NetPeer>(OnReceiveSessionsListRequest);
            SharedSessionRoom room = new SharedSessionRoom(null, "Test", "", "", 5);
            Rooms.Add(room.Id, room);
        }

        private Dictionary<Guid, SharedSessionRoom> Rooms { get; } = new Dictionary<Guid, SharedSessionRoom>();

        private bool ValidateString(string value, int minLenght, int maxLenght)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length >= minLenght && value.Length <= maxLenght;
        }

        private void OnReceiveRoomCreationRequest(SharedSessionRoomCreationRequest request, NetPeer peer)
        {
            if (!IsConnected(peer) || !ValidateString(request.RoomName, 3, 60) && request.RoomMaxUsers < 1)
                return;
            SharedSessionRoom room =
                new SharedSessionRoom(GetUserById(peer.Id), request.RoomName, request.RoomPassword, request.RoomThumbnail, request.RoomMaxUsers);
            Rooms.Add(room.Id, room);
        }

        private void OnReceiveSessionsListRequest(SharedSessionsListRequest request, NetPeer peer)
        {
            SharedSessionsListResponse response = new SharedSessionsListResponse {Sessions = new string[Rooms.Count]};
            for (int i = Rooms.Count; i-- > 0;)
                response.Sessions[i] = Rooms.ElementAt(i).Value.ToString();
            SendPacket(response, peer);
        }
    }
}