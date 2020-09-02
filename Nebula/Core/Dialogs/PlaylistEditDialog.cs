using System;
using System.Windows.Controls;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Attributes;
using Nebula.UI.Controls;
using Nebula.UI.Pages;

namespace Nebula.Core.Dialogs
{
    public class PlaylistEditDialog : DialogDataContent
    {
        private static readonly DataContentCache Cache = DataContentCache.BuildCache(typeof(PlaylistEditDialog));

        public PlaylistEditDialog(IPlaylist playlist)
        {
            Playlist = playlist;
            Title = NebulaClient.GetLocString("EditPlaylist");
            PrimaryButtonText = NebulaClient.GetLocString("Edit");
            Name = playlist.Name;
            Description = playlist.Description;
            Author = playlist.Author;
            string thumbnailUri = playlist.Thumbnail.ToString();
            Thumbnail = thumbnailUri.StartsWith("file") ? playlist.Thumbnail.LocalPath : thumbnailUri;
            Cache.PrepareFor(this);
        }

        public IPlaylist Playlist { get; }

        [DataProperty(typeof(TextBox), "TextProperty", "Name")]
        public string Name { get; set; }

        [DataProperty(typeof(TextBox), "TextProperty", "Description")]
        public string Description { get; set; }

        [DataProperty(typeof(TextBox), "TextProperty", "Author")]
        public string Author { get; set; }

        [DataProperty(typeof(PathSelectorControl), "TextProperty", "Thumbnail")]
        public string Thumbnail { get; set; }

        public override DataContentCache GetCache()
        {
            return Cache;
        }

        public override void OnPrimaryClick()
        {
            string oldName = Playlist.Name;
            Playlist.Name = Name;
            Playlist.Description = Description;
            Playlist.Author = Author;
            Playlist.Thumbnail = string.IsNullOrWhiteSpace(Thumbnail) ? null : new Uri(Thumbnail);
            if (oldName != Playlist.Name)
                NebulaClient.Playlists.RenamePlaylist(oldName, Playlist);
            else
                NebulaClient.Playlists.SavePlaylist(Playlist);
            NebulaClient.Navigate(typeof(PlaylistPage), Playlist);
        }
    }
}