using System.Collections.Generic;
using LiteNetLib;
using Nebula.Net;
using Nebula.Net.Packets;
using Nebula.Server.SharedSession;

namespace Nebula.Server.Users
{
    public class NebulaUser : NetClient
    {
        public NebulaUser(NetPeer peer, string username = "", string avatarUrl = "") : base(peer)
        {
            Username = username;
            AvatarUrl = avatarUrl;
        }

        private Dictionary<string, object> Tags              { get; } = new Dictionary<string, object>();
        public  string                     Username          { get; set; }
        public  string                     AvatarUrl         { get; set; }
        public  SharedSessionRoom          SharedSessionRoom { get; set; }
        public  bool                       IsPlayReady       { get; set; } = false;

        public bool IsInRoom()
        {
            return SharedSessionRoom != null && SharedSessionRoom.IsUserPresent(this);
        }

        public T RegisterTag<T>(string key, T tag) where T : class
        {
            if (Tags.ContainsKey(key))
                return default;
            Tags.Add(key, tag);
            return tag;
        }

        public T GetTag<T>(string key) where T : class
        {
            if (!Tags.ContainsKey(key))
                return default;
            return Tags[key] as T;
        }

        public bool HasTag(string key)
        {
            return Tags.ContainsKey(key);
        }

        public bool RemoveTag(string key)
        {
            return Tags.Remove(key);
        }

        public UserInfo AsUserInfo()
        {
            return new UserInfo
            {
                Id = Id,
                Username = Username,
                AvatarUrl = AvatarUrl
            };
        }
    }
}