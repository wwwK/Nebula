using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Medias.Playlist;
using Nebula.Pages.Dialogs;
using Page = ModernWpf.Controls.Page;

namespace Nebula.Pages
{
    public partial class PlaylistsPage : Page
    {
        public PlaylistsPage()
        {
            InitializeComponent();

            DataContext = this;
            RefreshPlaylists();
        }

        public List<IPlaylist> Playlists { get; private set; }

        private async void OnCreatePlaylistClicked(object sender, RoutedEventArgs e)
        {
            CreatePlaylistDialog dialog = new CreatePlaylistDialog {Title = "Create Playlist"};
            ContentDialogResult result = await dialog.ShowAsync(ContentDialogPlacement.Popup);
            if (result == ContentDialogResult.Primary)
            {
                NebulaPlaylist playlist = new NebulaPlaylist(dialog.PlaylistName.Text, dialog.PlaylistDescription.Text,
                    dialog.PlaylistAuthor.Text, string.IsNullOrWhiteSpace(dialog.PlaylistThumbnail.Text) ? null : new BitmapImage(new Uri(dialog.PlaylistThumbnail.Text)));
                NebulaClient.Playlists.AddPlaylist(playlist);
                RefreshPlaylists();
            }
        }

        private void RefreshPlaylists()
        {
            Playlists?.Clear();
            Playlists = NebulaClient.Playlists.GetPlaylists();
            PlaylistsElements.ItemsSource = Playlists;
        }
    }
}