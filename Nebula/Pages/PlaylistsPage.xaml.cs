using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using Nebula.Core;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.UI;
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

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer.Width = ActualWidth;
            ScrollViewer.Height = ActualHeight - CommandBar.ActualHeight;
        }

        private void OnPanelMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid && grid.Parent is Grid rootGrid &&
                rootGrid.Name == "ItemRootPanel" & rootGrid.IsMouseOver)
            {
                ControlUtils.ApplyBlur(grid, 4, TimeSpan.FromSeconds(0), TimeSpan.Zero);
                if (rootGrid.Children[1] is AppBarButton button)
                    button.Visibility = Visibility.Visible;
            }
        }

        private void OnPanelMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid && grid.Parent is Grid rootGrid &&
                rootGrid.Name == "ItemRootPanel" && !rootGrid.IsMouseOver)
            {
                ControlUtils.RemoveBlur(grid, TimeSpan.FromSeconds(0), TimeSpan.Zero);
                if (rootGrid.Children[1] is AppBarButton button)
                    button.Visibility = Visibility.Collapsed;
            }
        }

        private void OnPlayClicked(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton button && button.DataContext is IPlaylist playlist)
                NebulaClient.MediaPlayer.OpenPlaylist(playlist);
        }

        private void OnPlaylistElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid && grid.DataContext is IPlaylist playlist)
                NebulaClient.Navigate(typeof(PlaylistPage), playlist, new DrillInNavigationTransitionInfo());
        }
    }
}