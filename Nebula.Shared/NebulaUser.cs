using System;

namespace Nebula.Shared
{
    public class NebulaUser
    {
        Guid   Guid         { get; }
        string Name         { get; }
        string ThumbnailUrl { get; }
    }
}