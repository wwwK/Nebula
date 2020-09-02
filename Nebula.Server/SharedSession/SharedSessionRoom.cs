using System;
using System.Collections.Generic;
using Nebula.Shared.SharedSession;

namespace Nebula.Server.SharedSession
{
    public class SharedSessionRoom : ISharedSession
    {
        public SharedSessionRoom(string name, string password, string thumbnailUrl, int maxUsers)
        {
            Id = Guid.NewGuid();
            Name = name;
            Password = password;
            ThumbnailUrl = thumbnailUrl;
            MaxUsers = maxUsers;
        }

        private List<SharedSessionUser> Users        { get; } = new List<SharedSessionUser>();
        public  Guid                    Id           { get; set; }
        public  string                  Name         { get; set; }
        public  string                  Password     { get; }
        public  string                  ThumbnailUrl { get; set; }
        public  int                     UsersCount   => Users.Count;
        public  int                     MaxUsers     { get; } = 4;
    }
}