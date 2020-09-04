using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using Nebula.Net.Packets;
using Nebula.Net.Packets.BOTH;
using Nebula.Net.Packets.C2S;
using Nebula.Net.Packets.S2C;
using Nebula.Server.Events;
using Nebula.Server.SharedSession.Commands;
using Nebula.Server.Users;

namespace Nebula.Server.SharedSession
{
    public class SharedSessionsManager
    {
        public SharedSessionsManager(NebulaServer server)
        {
            Server = server;
            Server.CommandsManager.RegisterCommand(new SharedSessionsSubCommand());
            Server.PacketProcessor.SubscribeReusable<SharedSessionCreationRequest, NebulaUser>(OnReceiveRoomCreationRequest);
            Server.PacketProcessor.SubscribeReusable<SharedSessionsPollRequest, NebulaUser>(OnReceiveSessionsPollRequest);
            Server.PacketProcessor.SubscribeReusable<SharedSessionJoinRequest, NebulaUser>(OnReceiveSessionJoinRequest);
            Server.PacketProcessor.SubscribeReusable<SharedSessionPlayMediaPacket, NebulaUser>(OnReceivePlayMediaPacket);
            Server.PacketProcessor.SubscribeReusable<SharedSessionPlayReadyPacket, NebulaUser>(OnReceivePlayReadyPacket);
            Server.PacketProcessor.SubscribeReusable<SharedSessionLeavePacket, NebulaUser>(OnReceiveSessionLeavePacket);
            Server.PacketProcessor.SubscribeReusable<SharedSessionPausePacket, NebulaUser>(OnReceiveSessionPausePacket);
            Server.PacketProcessor.SubscribeReusable<SharedSessionResumePacket, NebulaUser>(OnReceiveSessionResumePacket);
            Server.PacketProcessor.SubscribeReusable<SharedSessionPositionChangedPacket, NebulaUser>(OnReceiveSessionPositionChangedPacket);
            Server.UserDisconnected += OnUserDisconnected;
        }

        private Dictionary<Guid, SharedSessionRoom> Rooms  { get; } = new Dictionary<Guid, SharedSessionRoom>();
        public  NebulaServer                        Server { get; }

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

        private void OnReceiveRoomCreationRequest(SharedSessionCreationRequest request, NebulaUser user)
        {
            if (!ValidateString(request.Name, 3, 60) || request.Size <= 1)
                return;

            SharedSessionRoom room =
                new SharedSessionRoom(user, request.Name, request.Password, "", request.Size);
            CreateRoom(room);
        }

        private void OnReceiveSessionsPollRequest(SharedSessionsPollRequest request, NebulaUser user)
        {
            SharedSessionsPollResponse response = new SharedSessionsPollResponse {Sessions = new SharedSessionInfo[Rooms.Count]};
            for (int i = Rooms.Count; i-- > 0;)
                response.Sessions[i] = Rooms.ElementAt(i).Value.AsSessionInfo();
            Server.SendPacket(response, user.Peer);
        }

        private void OnReceiveSessionJoinRequest(SharedSessionJoinRequest request, NebulaUser user)
        {
            SharedSessionRoom room = GetRoom(request.Session.Id);
            SharedSessionJoinResponse response = new SharedSessionJoinResponse();
            if (room == null)
                response.ResponseCode = 10;
            else
            {
                if (room.IsUserPresent(user))
                    response.ResponseCode = 11;
                else if (room.PasswordProtected && !room.VerifyPassword(request.Password))
                    response.ResponseCode = 12;
                else if (room.IsFull())
                    response.ResponseCode = 13;
                else
                {
                    room.AddUser(user);
                    response.ResponseCode = 0;
                    response.Session = room.AsSessionInfo();
                    response.Users = room.AsUsersArray();
                }
            }

            Server.SendPacket(response, user.Peer);
        }

        private void OnReceiveSessionPausePacket(SharedSessionPausePacket packet, NebulaUser user)
        {
            if (user.SharedSessionRoom == null || !user.SharedSessionRoom.IsUserPresent(user))
            {
                Server.HandleBadPacket(user);
                return;
            }

            user.SharedSessionRoom.SendToAll(packet, nebulaUser => nebulaUser != user);
        }

        private void OnReceiveSessionResumePacket(SharedSessionResumePacket packet, NebulaUser user)
        {
            if (user.SharedSessionRoom == null || !user.SharedSessionRoom.IsUserPresent(user))
            {
                Server.HandleBadPacket(user);
                return;
            }

            user.SharedSessionRoom.SendToAll(packet, nebulaUser => nebulaUser != user);
        }

        private void OnReceivePlayMediaPacket(SharedSessionPlayMediaPacket request, NebulaUser user)
        {
            if (user.SharedSessionRoom == null || !user.SharedSessionRoom.IsUserPresent(user))
            {
                Server.HandleBadPacket(user);
                return;
            }

            user.SharedSessionRoom.SetAllUnReady();
            user.SharedSessionRoom.SendToAll(request, nebulaUser => nebulaUser != user);
            user.SharedSessionRoom.SetReady(user);
        }

        private void OnReceivePlayReadyPacket(SharedSessionPlayReadyPacket request, NebulaUser user)
        {
            if (user.SharedSessionRoom == null || !user.SharedSessionRoom.IsUserPresent(user))
            {
                Server.HandleBadPacket(user);
                return;
            }

            user.SharedSessionRoom.SetReady(user);
        }

        private void OnReceiveSessionPositionChangedPacket(SharedSessionPositionChangedPacket packet, NebulaUser user)
        {
            if (user.SharedSessionRoom == null || !user.SharedSessionRoom.IsUserPresent(user))
            {
                Server.HandleBadPacket(user);
                return;
            }

            user.SharedSessionRoom.SendToAll(packet, nebulaUser => nebulaUser != user);
        }

        private void OnReceiveSessionLeavePacket(SharedSessionLeavePacket packet, NebulaUser user)
        {
            if (user.SharedSessionRoom == null || !user.SharedSessionRoom.IsUserPresent(user))
            {
                Server.HandleBadPacket(user);
                return;
            }

            user.SharedSessionRoom.RemoveUser(user);
        }

        private void OnUserDisconnected(object sender, UserDisconnectedEventArgs e)
        {
            if (e.User == null)
                return;
            if (e.User.IsInRoom())
                e.User.SharedSessionRoom.RemoveUser(e.User);
        }
    }
}