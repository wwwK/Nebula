using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using Nebula.Core;
using Nebula.Core.Extensions;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.UI.Dialogs;
using Page = ModernWpf.Controls.Page;
using PlaylistImportDialog = Nebula.Core.UI.Dialogs.PlaylistImportDialog;

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
            await new PlaylistCreationDialog().ShowDialogAsync();
            RefreshPlaylists();
        }

        private async void OnImportPlaylistClicked(object sender, RoutedEventArgs e)
        {
            await new PlaylistImportDialog().ShowDialogAsync();
            RefreshPlaylists();
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

        private async void OnPlayClicked(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton button && button.DataContext is IPlaylist playlist)
                await NebulaClient.MediaPlayer.OpenPlaylist(playlist);
        }

        private void OnPlaylistElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid && grid.DataContext is IPlaylist playlist)
                NebulaClient.Navigate(typeof(PlaylistPage), playlist, new DrillInNavigationTransitionInfo());
        }
    }
}