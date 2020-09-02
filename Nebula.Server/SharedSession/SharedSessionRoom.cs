using System;
using System.Collections.Generic;
using Nebula.Shared.SharedSession;

namespace Nebula.Server.SharedSession
{
    public class SharedSessionRoom : ISharedSession
    {
        public SharedSessionRoom(NebulaUser owner, string name, string password, string thumbnailUrl, int maxUsers)
        {
            Id = Guid.NewGuid();
            Owner = owner;
            Name = name;
            Password = password;
            ThumbnailUrl = thumbnailUrl;
            MaxUsers = maxUsers;
        }

        private List<NebulaUser> Users             { get; } = new List<NebulaUser>();
        public  Guid             Id                { get; }
        public  NebulaUser       Owner             { get; set; }
        public  string           Name              { get; set; }
        public  string           Password          { get; set; }
        public  string           ThumbnailUrl      { get; set; }
        public  int              MaxUsers          { get; } = 4;
        public  int              UsersCount        => Users.Count;
        public  bool             PasswordProtected => !string.IsNullOrWhiteSpace(Password);

        public override string ToString()
        {
            return $"{Id.ToString()}@{Name}@{ThumbnailUrl}@{PasswordProtected}@{UsersCount}@{MaxUsers}";
        }
    }
}