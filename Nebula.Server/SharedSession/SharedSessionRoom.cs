using System;
using System.Collections.Generic;
using LiteNetLib;
using Nebula.Net.Packets;
using Nebula.Net.Packets.S2C;
using Nebula.Server.Users;

namespace Nebula.Server.SharedSession
{
    public class SharedSessionRoom
    {
        public SharedSessionRoom(NebulaUser owner, string name, string password, string thumbnailUrl, int maxUsers)
        {
            Id = Guid.NewGuid();
            Owner = owner;
            Name = name;
            Password = password;
            ThumbnailUrl = thumbnailUrl;
            MaximumUsers = maxUsers;
        }

        private List<NebulaUser> Users             { get; } = new List<NebulaUser>();
        public  Guid             Id                { get; }
        public  NebulaUser       Owner             { get; set; }
        public  string           Name              { get; set; }
        public  string           Password          { get; set; }
        public  string           ThumbnailUrl      { get; set; }
        public  int              MaximumUsers      { get; } = 4;
        public  int              UsersCount        => Users.Count;
        public  bool             PasswordProtected => !string.IsNullOrWhiteSpace(Password);

        public bool IsFull()
        {
            return UsersCount >= MaximumUsers;
        }

        public bool IsUserPresent(NebulaUser user)
        {
            return Users.Contains(user);
        }

        public bool IsOwner(NebulaUser user)
        {
            return Owner == user;
        }

        public bool VerifyPassword(string password)
        {
            return Password == password;
        }

        public IEnumerable<NebulaUser> GetUsers()
        {
            return Users;
        }

        public void AddUser(NebulaUser user)
        {
            Users.Add(user);
            user.SharedSessionRoom = this;
            SendToAll(new SharedSessionUserJoinedPacket {User = user.AsUserInfo()}, nebulaUser => user != nebulaUser);
        }

        public void RemoveUser(NebulaUser user)
        {
            if (!IsUserPresent(user))
                return;
            Users.Remove(user);
            user.SharedSessionRoom = null;
            SendToAll(new SharedSessionUserLeftPacket() {User = user.AsUserInfo()});
        }

        public void SetReady(NebulaUser user)
        {
            user.IsPlayReady = true;
            CheckReadyState();
        }

        public void SetAllUnReady()
        {
            foreach (NebulaUser nebulaUser in Users)
                nebulaUser.IsPlayReady = false;
        }

        public bool IsReady(NebulaUser user)
        {
            return user.IsPlayReady;
        }

        public void SendToAll<T>(T packet, Predicate<NebulaUser> predicate = null) where T : class, new()
        {
            foreach (NebulaUser nebulaUser in Users.ToArray())
            {
                if (predicate == null)
                    ServerApp.Server.SendPacket(packet, nebulaUser.Peer);
                else if (predicate(nebulaUser))
                    ServerApp.Server.SendPacket(packet, nebulaUser.Peer);
            }
        }

        public SharedSessionInfo AsSessionInfo()
        {
            SharedSessionInfo sessionInfo = new SharedSessionInfo
            {
                Id = Id,
                Name = Name,
                CurrentUsers = UsersCount,
                MaximumUsers = MaximumUsers,
                PasswordProtected = PasswordProtected
            };
            return sessionInfo;
        }

        public UserInfo[] AsUsersArray()
        {
            UserInfo[] users = new UserInfo[UsersCount];
            int index = 0;
            foreach (NebulaUser nebulaUser in Users)
                users[index++] = nebulaUser.AsUserInfo();
            return users;
        }

        private void CheckReadyState()
        {
            bool allReady = true;
            foreach (NebulaUser nebulaUser in GetUsers())
            {
                if (nebulaUser.IsPlayReady)
                    continue;
                allReady = false;
                break;
            }

            if (allReady)
                SendToAll(new SharedSessionStartPlayingPacket());
        }
    }
}