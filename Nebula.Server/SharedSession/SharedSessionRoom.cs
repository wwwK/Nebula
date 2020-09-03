using System;
using System.Collections.Generic;
using Nebula.Server.Users;
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
            //Todo send new user
            Users.Add(user);
        }

        public void RemoveUser(NebulaUser user)
        {
            if (!IsUserPresent(user))
                return;
            Users.Remove(user);
            //Todo send user leave
        }

        public override string ToString()
        {
            return $"{Id.ToString()}@{Name}@{ThumbnailUrl}@{PasswordProtected}@{UsersCount}@{MaxUsers}";
        }
    }
}