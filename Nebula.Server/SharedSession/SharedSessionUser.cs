using System;
using LiteNetLib;
using Nebula.Shared;

namespace Nebula.Server.SharedSession
{
    public class SharedSessionUser : IUser
    {
        public SharedSessionUser(NetPeer peer, string name, string thumbnailUrl)
        {
            Peer = peer ?? throw new ArgumentNullException(nameof(peer));
            Id = Peer.Id;
            Name = string.IsNullOrWhiteSpace(name) ? "NebulaUser" : name;
            ThumbnailUrl = thumbnailUrl;
        }

        public int     Id           { get; }
        public NetPeer Peer         { get; }
        public string  Name         { get; }
        public string  ThumbnailUrl { get; }
        public bool    IsPlayReady  { get; }
    }
}