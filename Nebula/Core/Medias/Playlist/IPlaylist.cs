﻿using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Nebula.Core.Medias.Playlist.Events;

namespace Nebula.Core.Medias.Playlist
{
    public interface IPlaylist : IEnumerable<IMediaInfo>
    {
        string   Name          { get; set; }
        string   Description   { get; set; }
        string   Author        { get; }
        Uri      Thumbnail     { get; }
        int      MediasCount   { get; }
        TimeSpan TotalDuration { get; }

        event EventHandler<PlaylistMediaAddedEventArgs> MediaAdded;

        void AddMedia(IMediaInfo mediaInfo, int insertIndex = -1);
        void AddMedias(IMediaInfo[] mediaInfo);
        void RemoveMedia(IMediaInfo mediaInfo);
        void RemoveMedias(params IMediaInfo[] medias);
        IMediaInfo GetMedia(int index);
    }
}