using System;
using System.IO;
using System.Windows;
using Nebula.Core.Data;
using Nebula.Core.Medias.Playlist.Events;

namespace Nebula.Core.Medias.Playlist
{
    public interface IPlaylist : IDataLoadable, IDataSaveable
    {
        string           Name          { get; set; }
        string           Description   { get; set; }
        string           Author        { get; set; }
        Uri              Thumbnail     { get; set; }
        object           Tag           { get; set; }
        MediasCollection Medias        { get; set; }
        IDataFile        File          { get; set; }
        int              MediasCount   { get; }
        TimeSpan         TotalDuration { get; }

        event EventHandler<PlaylistMediaAddedEventArgs> MediaAdded;

        bool Contains(IMediaInfo mediaInfo);
        void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1);
        void AddMedias(IMediaInfo[] mediaInfo);
        void RemoveMedia(IMediaInfo mediaInfo);
        void RemoveMedias(params IMediaInfo[] medias);
        IMediaInfo GetMedia(int index);
    }
}