using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nebula.Core.Medias.Playlist.Events;

namespace Nebula.Core.Medias.Playlist
{
    public class NebulaPlaylist : IPlaylist
    {
        public NebulaPlaylist(string name, string description, string author, Uri thumbnail = null,
                              List<IMediaInfo> medias = null)
        {
            Name = name ?? " ";
            Description = description ?? " ";
            Author = author ?? " ";
            Thumbnail = thumbnail ?? new Uri("https://i.imgur.com/Od5XogD.png");
            if (medias != null)
                AddMedias(medias.ToArray());
        }

        public string   Name          { get; set; }
        public string   Description   { get; set; }
        public string   Author        { get; set; }
        public bool     AutoSave      { get; set; } = true;
        public Uri      Thumbnail     { get; set; }
        public TimeSpan TotalDuration { get; private set; } = TimeSpan.Zero;
        public int      MediasCount   => MediaList.Count;

        public event EventHandler<PlaylistMediaAddedEventArgs> MediaAdded;

        private List<IMediaInfo> MediaList { get; } = new List<IMediaInfo>();

        public bool Contains(IMediaInfo mediaInfo)
        {
            return MediaList.Any(media => media.Id == mediaInfo.Id);
        }

        public void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1)
        {
            if (insertIndex >= 0)
                MediaList.Insert(insertIndex, mediaInfo);
            else
                MediaList.Add(mediaInfo);
            UpdateTotalDuration();
            if (AutoSave)
                Save();
            MediaAdded?.Invoke(this, new PlaylistMediaAddedEventArgs(this, mediaInfo, insertIndex));
        }

        public void AddMedias(IMediaInfo[] medias)
        {
            MediaList.AddRange(medias);
            UpdateTotalDuration();
            if (AutoSave)
                Save();
        }

        public void RemoveMedia(IMediaInfo mediaInfo)
        {
            if (!MediaList.Contains(mediaInfo))
                return;
            MediaList.Remove(mediaInfo);
            UpdateTotalDuration();
            if (AutoSave)
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
            NebulaClient.Playlists.SavePlaylist(this);
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