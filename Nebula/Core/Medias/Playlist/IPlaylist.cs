using System;

namespace Nebula.Core.Medias.Playlist
{
    public interface IPlaylist
    {
        string   Name          { get; set; }
        string   Description   { get; set; }
        string   Author        { get; }
        int      MediasCount   { get; }
        TimeSpan TotalDuration { get; }

        void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1);
        void AddMedias(IMediaInfo[] mediaInfo);
        void RemoveMedia(IMediaInfo mediaInfo);
        void RemoveMedias(params IMediaInfo[] medias);
        IMediaInfo GetMedia(int index);
        IMediaInfo GetRandomMedia();
    }
}