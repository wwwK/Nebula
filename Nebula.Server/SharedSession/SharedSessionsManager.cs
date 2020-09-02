using System;
using System.Collections.Generic;
using LiteNetLib;
using Nebula.Shared.Packets.C2S;

namespace Nebula.Server.SharedSession
{
    public class SharedSessionsManager
    {
        public SharedSessionsManager()
        {
            ServerApp.PacketProcessor.SubscribeReusable<SharedSessionRoomCreationRequest, NetPeer>(OnReceiveRoomCreationRequest);
        }

        private Dictionary<Guid, SharedSessionRoom> Rooms { get; } = new Dictionary<Guid, SharedSessionRoom>();

        private bool ValidateString(string value, int minLenght, int maxLenght)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length >= minLenght && value.Length <= maxLenght;
        }

        private void OnReceiveRoomCreationRequest(SharedSessionRoomCreationRequest request, NetPeer peer)
        {
            if (!ValidateString(request.RoomName, 3, 60) && request.RoomMaxUsers < 1)
                return;
            SharedSessionRoom room = new SharedSessionRoom(request.RoomName, request.RoomPassword, request.RoomThumbnail, request.RoomMaxUsers);
            Rooms.Add(room.Id, room);
        }
    }
}