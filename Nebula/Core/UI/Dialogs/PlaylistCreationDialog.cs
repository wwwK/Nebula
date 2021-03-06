﻿using System;
using System.Windows.Controls;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.Medias.Playlist.Playlists;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Attributes;
using Nebula.UI.Controls;

namespace Nebula.Core.UI.Dialogs
{
    public class PlaylistCreationDialog : DialogDataContent
    {
        private static readonly DataContentCache Cache = DataContentCache.BuildCache<PlaylistCreationDialog>();

        public PlaylistCreationDialog()
        {
            Title = NebulaClient.GetLocString("CreatePlaylist");
            PrimaryButtonText = NebulaClient.GetLocString("Create");
            Cache.PrepareFor(this);
        }

        public IPlaylist CreatedPlaylist { get; private set; }

        [DataProperty(typeof(TextBox), "Name")]
        public string Name { get; set; }

        [DataProperty(typeof(TextBox), "Description")]
        public string Description { get; set; }

        [DataProperty(typeof(TextBox), "Author")]
        public string Author { get; set; }

        [DataProperty(typeof(PathSelectorControl), "Thumbnail", args: new object[] {"Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"})]
        public string Thumbnail { get; set; }

        public override DataContentCache GetCache()
        {
            return Cache;
        }

        public override void OnPrimaryClick()
        {
            Uri thumbnailUri = string.IsNullOrWhiteSpace(Thumbnail) ? null : new Uri(Thumbnail);
            CreatedPlaylist = new NebulaPlaylist(Name, Description, Author, thumbnailUri);
            NebulaClient.Playlists.AddPlaylist(CreatedPlaylist);
        }
    }
}