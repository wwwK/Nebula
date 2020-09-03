using System;
using Nebula.Shared.Packets;

namespace Nebula.Shared
{
    public interface IUser
    {
        int    Id           { get; }
        string Name         { get; }
        string ThumbnailUrl { get; }
    }
}