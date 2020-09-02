using System;

namespace Nebula.Shared.SharedSession
{
    public class SharedSessionInfo : ISharedSession
    {
        private SharedSessionInfo(Guid id, string name, string thumbnailUrl, bool passwordProtected, int usersCount, int maxUsers)
        {
            Id = id;
            Name = name;
            ThumbnailUrl = thumbnailUrl;
            PasswordProtected = passwordProtected;
            UsersCount = usersCount;
            MaxUsers = maxUsers;
        }

        public Guid   Id                { get; }
        public string Name              { get; }
        public string ThumbnailUrl      { get; }
        public bool   PasswordProtected { get; }
        public int    UsersCount        { get; }
        public int    MaxUsers          { get; }

        public static SharedSessionInfo FromString(string value)
        {
            string[] split = value.Split('@');
            if (split.Length != 6)
                return null;
            return new SharedSessionInfo(Guid.Parse(split[0]), split[1], split[2],
                bool.Parse(split[3]), int.Parse(split[4]), int.Parse(split[5]));
        }
    }
}