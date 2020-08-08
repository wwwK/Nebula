using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Nebula.Core.Medias.Playlist
{
    public class NebulaPlaylist : IPlaylist
    {
        private static Random _random = new Random();

        public NebulaPlaylist(string name, string description, string author, BitmapImage thumbnail = null,
                              List<IMediaInfo> medias = null)
        {
            Name = name;
            Description = description;
            Author = author;
            Thumbnail = thumbnail ??
                        new BitmapImage(new Uri("pack://application:,,/Resources/default_playlist_thumbnail.png"));
            if (medias != null)
                AddMedias(medias.ToArray());
        }

        public string      Name          { get; set; }
        public string      Description   { get; set; }
        public string      Author        { get; }
        public BitmapImage Thumbnail     { get; }
        public TimeSpan    TotalDuration { get; private set; } = TimeSpan.Zero;
        public int         MediasCount   => MediaList.Count;

        private List<IMediaInfo> MediaList { get; } = new List<IMediaInfo>();

        public void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1)
        {
            if (insertIndex >= 0)
                MediaList.Insert(insertIndex, mediaInfo);
            else
                MediaList.Add(mediaInfo);
            Save();
        }

        public void AddMedias(IMediaInfo[] medias) //Todo: bad way of doing that
        {
            MediaList.AddRange(medias);
            UpdateTotalDuration();
            Save();
        }

        public void RemoveMedia(IMediaInfo mediaInfo)
        {
            if (!MediaList.Contains(mediaInfo))
                return;
            MediaList.Remove(mediaInfo);
            UpdateTotalDuration();
            Save();
        }

        public void RemoveMedias(params IMediaInfo[] medias) //Todo: bad way of doing that
        {
            throw new NotImplementedException();
        }

        public IMediaInfo GetMedia(int index)
        {
            if (index < 0 || index > MediasCount - 1)
                throw new ArgumentOutOfRangeException(nameof(index));
            return MediaList[index];
        }

        private void Save()
        {
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerator<IMediaInfo> GetEnumerator()
        {
            return MediaList.GetEnumerator();
        }

        private void UpdateTotalDuration()
        {
            TotalDuration = TimeSpan.Zero;
            foreach (IMediaInfo mediaInfo in MediaList)
                TotalDuration += mediaInfo.Duration;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}