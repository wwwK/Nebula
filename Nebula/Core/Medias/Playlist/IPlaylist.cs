using System;

namespace Nebula.Core.Medias.Playlist
{
    public interface IPlaylist
    {
        string   Name          { get; set; }
        string   Description   { get; set; }
        int      MediasCount   { get; }
        TimeSpan TotalDuration { get; }

        void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1);
        void RemoveMedia(IMediaInfo mediaInfo);
        IMediaInfo GetMedia(int index);
        IMediaInfo GetRandomMedia(params IMediaInfo[] excludedMedias);
    }
}