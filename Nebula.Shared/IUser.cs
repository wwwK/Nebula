using System;

namespace Nebula.Shared
{
    public interface IUser
    {
        int    Id           { get; }
        string Name         { get; }
        string ThumbnailUrl { get; }
    }
}