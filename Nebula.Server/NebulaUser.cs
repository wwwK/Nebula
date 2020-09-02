﻿using System.Collections.Generic;
using LiteNetLib;
using Nebula.Shared;

namespace Nebula.Server
{
    public class NebulaUser : IUser
    {
        public NebulaUser(NetPeer peer, string name = "", string thumbnailUrl = "")
        {
            Peer = peer;
            Id = peer.Id;
            Name = name;
            ThumbnailUrl = thumbnailUrl;
        }

        private Dictionary<string, object> Tags         { get; } = new Dictionary<string, object>();
        public  NetPeer                    Peer         { get; }
        public  int                        Id           { get; }
        public  string                     Name         { get; set; }
        public  string                     ThumbnailUrl { get; set; }

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

        public bool RemoveTag(string key)
        {
            return Tags.Remove(key);
        }
    }
}