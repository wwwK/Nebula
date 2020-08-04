using System;
using System.Collections.Generic;

namespace Nebula.Core.Medias.Playlist
{
    public class NebulaPlaylist : IPlaylist
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MediasCount { get; }
        public TimeSpan TotalDuration { get; }

        public void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1)
        {
            throw new NotImplementedException();
        }

        public void RemoveMedia(IMediaInfo mediaInfo)
        {
            throw new NotImplementedException();
        }

        public IMediaInfo GetMedia(int index)
        {
            throw new NotImplementedException();
        }

        public IMediaInfo GetRandomMedia(params IMediaInfo[] excludedMedias)
        {
            throw new NotImplementedException();
        }

        private List<IMediaInfo> MediasList { get; } = new List<IMediaInfo>();

    }
}