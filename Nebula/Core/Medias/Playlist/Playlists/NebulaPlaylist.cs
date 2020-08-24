using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nebula.Core.Medias.Playlist.Events;

namespace Nebula.Core.Medias.Playlist.Playlists
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

        public string           Name          { get; set; }
        public string           Description   { get; set; }
        public string           Author        { get; set; }
        public bool             AutoSave      { get; set; } = true;
        public Uri              Thumbnail     { get; set; }
        public object           Tag           { get; set; }
        public MediasCollection Medias        { get; } = new MediasCollection();
        public TimeSpan         TotalDuration => Medias.TotalDuration;
        public int              MediasCount   => Medias.Count;

        public event EventHandler<PlaylistMediaAddedEventArgs> MediaAdded;

        public bool Contains(IMediaInfo mediaInfo)
        {
            return Medias.Any(media => media.Id == mediaInfo.Id);
        }

        public void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1)
        {
            if (insertIndex >= 0)
                Medias.Insert(insertIndex, mediaInfo);
            else
                Medias.Add(mediaInfo);
            if (AutoSave)
                Save();
            MediaAdded?.Invoke(this, new PlaylistMediaAddedEventArgs(this, mediaInfo, insertIndex));
        }

        public void AddMedias(IMediaInfo[] medias)
        {
            Medias.AddRange(medias);
            if (AutoSave)
                Save();
        }

        public void RemoveMedia(IMediaInfo mediaInfo)
        {
            if (!Medias.Contains(mediaInfo))
                return;
            Medias.Remove(mediaInfo);
            if (AutoSave)
                Save();
        }

        public void RemoveMedias(params IMediaInfo[] medias) //Todo: bad way of doing that
        {
            Medias.RemoveRange(medias);
            if (AutoSave)
                Save();
        }

        public IMediaInfo GetMedia(int index)
        {
            if (index < 0 || index > MediasCount - 1)
                throw new ArgumentOutOfRangeException(nameof(index));
            return Medias[index];
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
            return Medias.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}