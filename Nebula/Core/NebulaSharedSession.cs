using System;
using System.Collections.Generic;
using System.Windows;
using Nebula.Shared.Packets;
using Nebula.Shared.SharedSession;

namespace Nebula.Core
{
    public class NebulaSharedSession : ISharedSession
    {
        public NebulaSharedSession(Guid id, string name, string thumbnailUrl, ICollection<UserInfosPacket> users, int maxUsers, bool passwordProtected)
        {
            Id = id;
            Name = name;
            ThumbnailUrl = thumbnailUrl;
            MaxUsers = maxUsers;
            PasswordProtected = passwordProtected;
            if (users != null && users.Count > 0)
            {
                UsersCount = users.Count;
                Users.AddRange(users);
            }
        }

        private List<UserInfosPacket> Users             { get; } = new List<UserInfosPacket>();
        public  Guid                  Id                { get; }
        public  string                Name              { get; set; }
        public  string                ThumbnailUrl      { get; set; }
        public  int                   UsersCount        { get; }
        public  int                   MaxUsers          { get; }
        public  bool                  PasswordProtected { get; }
    }
}