using System;
using System.Collections;

namespace Nebula.Shared.SharedSession
{
    public interface ISharedSession
    {
        Guid   Id           { get; }
        string Name         { get; }
        string ThumbnailUrl { get; }
        int    UsersCount   { get; }
        int    MaxUsers     { get; }
    }
}