using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Nebula.Core.Data;
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

        public NebulaPlaylist()
        {
        }

        public string           Name          { get; set; }
        public string           Description   { get; set; }
        public string           Author        { get; set; }
        public bool             AutoSave      { get; set; } = true;
        public Uri              Thumbnail     { get; set; }
        public object           Tag           { get; set; }
        public MediasCollection Medias        { get; set; } = new MediasCollection();
        public IDataFile        File          { get; set; }
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

        public bool OnLoad(IDataMember member)
        {
            Name = member.GetString("Name");
            Description = member.GetString("Description");
            Author = member.GetString("Author");
            string thumbnail = member.GetString("Thumbnail");
            if (thumbnail.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                Thumbnail = Uri.TryCreate(thumbnail, UriKind.RelativeOrAbsolute, out Uri thumbnailUri) ? thumbnailUri : new Uri("https://i.imgur.com/Od5XogD.png");
            }
            else
                Thumbnail = new Uri(Path.Combine(NebulaClient.Playlists.ThumbnailCacheDirectory.FullName,
                    thumbnail));

            foreach (IDataMember dataMember in member.GetChilds())
            {
                Type type = Type.GetType(dataMember.GetString("ProviderType"));
                if (type == null)
                    continue;
                object instance = Activator.CreateInstance(type);
                if (!(instance is IMediaInfo mediaInfo))
                    continue;
                mediaInfo.OnLoad(dataMember);
                Medias.Add(mediaInfo);
            }

            return true;
        }

        public bool OnSave(IDataMember member)
        {
            member.SetValue("Name", Name);
            member.SetValue("Description", Description);
            member.SetValue("Author", Author);
            if (Thumbnail != null)
            {
                if (Thumbnail.ToString().StartsWith("http"))
                    member.SetValue("Thumbnail", Thumbnail.ToString());
                else
                {
                    string thumbnailFileName =
                        $"{Name}_thumbnail{Path.GetExtension(Thumbnail.LocalPath)}";
                    member.SetValue("Thumbnail", thumbnailFileName);
                    string filePath = Path.Combine(NebulaClient.Playlists.ThumbnailCacheDirectory.FullName, thumbnailFileName);
                    if (!System.IO.File.Exists(filePath))
                        System.IO.File.Copy(Thumbnail.LocalPath, filePath);
                }
            }

            foreach (IMediaInfo mediaInfo in Medias)
                member.CreateChild("Media", mediaInfo);
            return true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}