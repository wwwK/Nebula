using System;
using System.Collections.Generic;

namespace Nebula.Core.Medias.Playlist
{
    public class NebulaPlaylist : IPlaylist
    {
        private static Random _random = new Random();

        public NebulaPlaylist(string name, string description, string author, List<IMediaInfo> medias = null)
        {
            Name = name;
            Description = description;
            Author = author;
            if (medias != null)
                AddMedias(medias.ToArray());
        }

        public string   Name          { get; set; }
        public string   Description   { get; set; }
        public string   Author        { get; }
        public TimeSpan TotalDuration { get; private set; } = TimeSpan.Zero;
        public int      MediasCount   => MediaList.Count;

        private List<IMediaInfo> MediaList { get; } = new List<IMediaInfo>();

        public void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1)
        {
            if (insertIndex >= 0)
                MediaList.Insert(insertIndex, mediaInfo);
            else
                MediaList.Add(mediaInfo);
            TotalDuration += mediaInfo.Duration;
        }

        public void AddMedias(IMediaInfo[] medias) //Todo: bad way of doing that
        {
            foreach (IMediaInfo mediaInfo in medias)
                AddMedia(mediaInfo);
        }

        public void RemoveMedia(IMediaInfo mediaInfo)
        {
            if (!MediaList.Contains(mediaInfo))
                return;
            MediaList.Remove(mediaInfo);
            TotalDuration -= mediaInfo.Duration;
        }

        public void RemoveMedias(params IMediaInfo[] medias) //Todo: bad way of doing that
        {
            foreach (IMediaInfo mediaInfo in medias)
                RemoveMedia(mediaInfo);
        }

        public IMediaInfo GetMedia(int index)
        {
            if (index < 0 || index > MediasCount - 1)
                throw new ArgumentOutOfRangeException(nameof(index));
            return MediaList[index];
        }

        public IMediaInfo GetRandomMedia()
        {
            return MediaList[_random.Next(MediasCount)];
        }
    }
}