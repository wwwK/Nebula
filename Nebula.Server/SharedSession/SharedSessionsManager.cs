using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using Nebula.Server.Extensions;
using Nebula.Server.SharedSession.Commands;
using Nebula.Server.Users;
using Nebula.Shared.Packets;
using Nebula.Shared.Packets.C2S;
using Nebula.Shared.Packets.S2C;

namespace Nebula.Server.SharedSession
{
    public class SharedSessionsManager
    {
        public SharedSessionsManager()
        {
            NebulaServer.CommandsManager.RegisterCommand(new SharedSessionsSubCommand());
            NebulaServer.PacketProcessor.SubscribeReusable<SharedSessionRoomCreationRequest, NetPeer>(OnReceiveRoomCreationRequest);
            NebulaServer.PacketProcessor.SubscribeReusable<SharedSessionsListRequest, NetPeer>(OnReceiveSessionsListRequest);
            NebulaServer.PacketProcessor.SubscribeReusable<SharedSessionJoinRequest, NetPeer>(OnReceiveSessionJoinRequest);
            SharedSessionRoom room = new SharedSessionRoom(null, "Test", "", "", 5);
            Rooms.Add(room.Id, room);
        }

        private Dictionary<Guid, SharedSessionRoom> Rooms { get; } = new Dictionary<Guid, SharedSessionRoom>();

        public IEnumerable<SharedSessionRoom> GetRooms()
        {
            return Rooms.Select(sharedSessionRoom => sharedSessionRoom.Value);
        }

        public SharedSessionRoom GetRoom(Guid guid)
        {
            return Rooms.ContainsKey(guid) ? Rooms[guid] : null;
        }

        public bool CreateRoom(SharedSessionRoom room)
        {
            if (Rooms.ContainsKey(room.Id))
                return false;
            Rooms.Add(room.Id, room);
            return true;
        }

        private bool ValidateString(string value, int minLenght, int maxLenght)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length >= minLenght && value.Length <= maxLenght;
        }

        private void OnReceiveRoomCreationRequest(SharedSessionRoomCreationRequest request, NetPeer peer)
        {
            if (!NebulaServer.IsConnected(peer) || !ValidateString(request.RoomName, 3, 60) || request.RoomMaxUsers <= 1)
                return;
            SharedSessionRoom room =
                new SharedSessionRoom(NebulaServer.GetUserById(peer.Id), request.RoomName, request.RoomPassword, request.RoomThumbnail, request.RoomMaxUsers);
            CreateRoom(room);
        }

        private void OnReceiveSessionsListRequest(SharedSessionsListRequest request, NetPeer peer)
        {
            SharedSessionsListResponse response = new SharedSessionsListResponse {Sessions = new string[Rooms.Count]};
            for (int i = Rooms.Count; i-- > 0;)
                response.Sessions[i] = Rooms.ElementAt(i).Value.ToString();
            NebulaServer.SendPacket(TestPacket.Create(), peer);
            NebulaServer.SendPacket(response, peer);
        }

        private void OnReceiveSessionJoinRequest(SharedSessionJoinRequest request, NetPeer peer)
        {
            if (!NebulaServer.IsConnected(peer))
                return;
            NebulaUser user = NebulaServer.GetUserById(peer.Id);
            Guid parseId = Guid.Parse(request.Id);
            SharedSessionRoom room = GetRoom(parseId);
            SharedSessionJoinResponse response = new SharedSessionJoinResponse {RoomId = request.Id};
            if (room == null)
                response.ResponseCode = 10;
            else
            {
                if (room.IsUserPresent(user))
                    response.ResponseCode = 11;
                else if (room.PasswordProtected && !room.VerifyPassword(request.Password))
                    response.ResponseCode = 12;
                else
                {
                    room.AddUser(user);
                    response.ResponseCode = 0;
                    response.RoomName = room.Name;
                    response.MaxUsers = room.MaxUsers;
                    response.PasswordProtected = room.PasswordProtected;
                    if (room.UsersCount > 0)
                    {
                        response.Users = new string[room.UsersCount];
                        int index = 0;
                        foreach (NebulaUser roomUser in room.GetUsers())
                        {
                            response.Users[index] = roomUser.ToInfoPacketString();
                            index++;
                        }
                    }
                }
            }

            NebulaServer.SendPacket(response, peer);
        }
    }
}