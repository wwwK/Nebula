using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using Nebula.Core;
using Nebula.Core.Extensions;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.Medias.Provider.Providers.Youtube;
using Nebula.Core.UI;
using Nebula.Pages.Dialogs;
using Page = ModernWpf.Controls.Page;

namespace Nebula.UI.Pages
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

        private void RefreshPlaylists()
        {
            Playlists?.Clear();
            Playlists = NebulaClient.Playlists.GetPlaylists();
            PlaylistsElements.ItemsSource = Playlists;
        }

        private async void OnCreatePlaylistClicked(object sender, RoutedEventArgs e)
        {
            PlaylistEditDialog dialog = new PlaylistEditDialog(PlaylistEditDialogAction.CreatePlaylist);
            ContentDialogResult result = await dialog.ShowAsync(ContentDialogPlacement.Popup);
            if (result == ContentDialogResult.Primary)
            {
                NebulaPlaylist playlist = new NebulaPlaylist(dialog.PlaylistName.Text, dialog.PlaylistDescription.Text,
                    dialog.PlaylistAuthor.Text,
                    string.IsNullOrWhiteSpace(dialog.PlaylistThumbnail.Text)
                        ? null
                        : new Uri(dialog.PlaylistThumbnail.Text));
                NebulaClient.Playlists.AddPlaylist(playlist);
                RefreshPlaylists();
            }
        }

        private async void OnImportPlaylistClicked(object sender, RoutedEventArgs e)
        {
            PlaylistImportDialog dialog = new PlaylistImportDialog();
            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string playlistPath = dialog.PlaylistPath.Text;
                if (string.IsNullOrWhiteSpace(playlistPath))
                    return;
                await NebulaMessageBox.ShowOk("PlaylistImport", "PlaylistImporting");
                IPlaylist playlist;
                if (playlistPath.Contains("http") && playlistPath.Contains("youtube"))
                {
                    playlist = await NebulaClient.GetMediaProvider<YoutubeMediaProvider>().GetPlaylist(playlistPath);
                    NebulaClient.Playlists.AddPlaylist(playlist);
                    NebulaClient.Playlists.SavePlaylist(playlist);
                }
                else
                {
                    playlist = NebulaClient.Playlists.LoadPlaylist(new FileInfo(playlistPath));
                    NebulaClient.Playlists.SavePlaylist(playlist);
                }

                await NebulaMessageBox.ShowOk("PlaylistImport", "PlaylistImported", playlist.Name);
                RefreshPlaylists();
            }
        }

        private void OnPanelMouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is Grid grid) || !(grid.Parent is Grid rootGrid) || !(rootGrid.Name == "ItemRootPanel" & rootGrid.IsMouseOver))
                return;
            grid.ApplyBlur(4, TimeSpan.FromSeconds(0), TimeSpan.Zero);
            if (rootGrid.Children[1] is AppBarButton button)
                button.Visibility = Visibility.Visible;
        }

        private void OnPanelMouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender is Grid grid) || !(grid.Parent is Grid rootGrid) || rootGrid.Name != "ItemRootPanel" || rootGrid.IsMouseOver)
                return;
            grid.RemoveBlur(TimeSpan.FromSeconds(0), TimeSpan.Zero);
            if (rootGrid.Children[1] is AppBarButton button)
                button.Visibility = Visibility.Collapsed;
        }

        private void OnPlayClicked(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton button && button.DataContext is IPlaylist playlist)
                NebulaClient.MediaPlayer.OpenPlaylist(playlist, true);
        }

        private void OnPlaylistElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid && grid.DataContext is IPlaylist playlist)
                NebulaClient.Navigate(typeof(PlaylistPage), playlist, new DrillInNavigationTransitionInfo());
        }
    }
}