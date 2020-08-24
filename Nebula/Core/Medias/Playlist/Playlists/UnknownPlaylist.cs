using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nebula.Core.Medias.Playlist.Events;

namespace Nebula.Core.Medias.Playlist.Playlists
{
    public class UnknownPlaylist : IPlaylist
    {
        public UnknownPlaylist()
        {
        }

        public string           Name          { get; set; }
        public string           Description   { get; set; }
        public string           Author        { get; set; }
        public Uri              Thumbnail     { get; set; }
        public object           Tag           { get; set; }
        public MediasCollection Medias        { get; } = new MediasCollection();
        public int              MediasCount   => Medias.Count;
        public TimeSpan         TotalDuration => Medias.TotalDuration;

        public event EventHandler<PlaylistMediaAddedEventArgs> MediaAdded;

        public bool Contains(IMediaInfo mediaInfo)
        {
            return Medias.Any(media => media.Id == mediaInfo.Id);
        }

        public void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1)
        {
            if (Contains(mediaInfo))
                return;
            if (insertIndex == -1)
                Medias.Add(mediaInfo);
            else
                Medias.Insert(insertIndex, mediaInfo);
        }

        public void AddMedias(IMediaInfo[] mediaInfo)
        {
            Medias.AddRange(mediaInfo);
        }

        public void RemoveMedia(IMediaInfo mediaInfo)
        {
            if (!Contains(mediaInfo))
                return;
            Medias.Remove(mediaInfo);
        }

        public void RemoveMedias(params IMediaInfo[] medias)
        {
            Medias.RemoveRange(medias);
        }

        public IMediaInfo GetMedia(int index)
        {
            if (index < 0 || index > MediasCount - 1)
                throw new ArgumentOutOfRangeException(nameof(index));
            return Medias[index];
        }

        public IEnumerator<IMediaInfo> GetEnumerator()
        {
            return Medias.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static async Task<UnknownPlaylist> FromArtist(IArtistInfo artistInfo, bool loadMedias = true)
        {
            UnknownPlaylist playlist = new UnknownPlaylist
            {
                Name = artistInfo.Title,
                Description = artistInfo.Url,
                Thumbnail = new Uri(artistInfo.LogoUrl),
                Tag = artistInfo
            };
            if (!loadMedias)
                return playlist;
            await foreach (IMediaInfo mediaInfo in artistInfo.GetMedias())
                playlist.AddMedia(mediaInfo);
            return playlist;
        }
    }
}