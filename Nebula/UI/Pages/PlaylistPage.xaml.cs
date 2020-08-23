using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Events;
using Nebula.Core.Medias.Playlist;
using Nebula.Pages.Dialogs;

namespace Nebula.UI.Pages
{
    public partial class PlaylistPage : Page
    {
        public PlaylistPage()
        {
            InitializeComponent();

            DataContext = this;
            Focus();
        }

        public IPlaylist Playlist { get; private set; }

        public MediasCollectionPages Medias { get; private set; }

        private void RemoveMedia(IMediaInfo mediaInfo)
        {
            Playlist.RemoveMedia(mediaInfo);
            if (NebulaClient.MediaPlayer.Queue.IsQueued(mediaInfo))
                NebulaClient.MediaPlayer.Queue.Remove(mediaInfo);
            Medias.Remove(mediaInfo);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!(e.ExtraData is IPlaylist playlist))
                return;
            Playlist = playlist;
            PlaylistLogo.Source = new BitmapImage(playlist.Thumbnail);
            PlaylistTitle.Text = playlist.Name;
            PlaylistDescription.Text = playlist.Description;
            PlaylistAuthor.Text = string.Format(NebulaClient.GetLocString("By"), playlist.Author);
            ;
            PlaylistMediaCount.Text =
                $"{string.Format(NebulaClient.GetLocString("PlaylistTitles"), playlist.MediasCount)} - {string.Format(NebulaClient.GetLocString("PlaylistTotalDuration"), playlist.TotalDuration)}";
            Medias = new MediasCollectionPages(playlist.Medias);
            Medias.PageChanged += OnPageChanged;
            CurrentPageText.Text = $"{Medias.CurrentPage + 1}/{Playlist.Medias.TotalPages}";
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Medias.PageChanged -= OnPageChanged;
        }

        private void OnPageChanged(object sender, MediaCollectionPageChangedEventArgs e)
        {
            CurrentPageText.Text = $"{Medias.CurrentPage + 1}/{Playlist.Medias.TotalPages}";
        }

        private async void OnPlayClick(object sender, RoutedEventArgs e)
        {
            AppBarButton clicked = (AppBarButton) sender;
            if (clicked.DataContext is IMediaInfo mediaInfo)
                await NebulaClient.MediaPlayer.OpenMedia(mediaInfo, true);
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            AppBarButton clicked = (AppBarButton) sender;
            if (clicked.DataContext is IMediaInfo mediaInfo)
                RemoveMedia(mediaInfo);
        }

        private async void OnDeletePlaylistClick(object sender, RoutedEventArgs e)
        {
            ContentDialogResult result =
                await NebulaMessageBox.ShowYesNo("DeletePlaylist", "DeletePlaylistMsg", Playlist.Name);
            if (result == ContentDialogResult.Primary)
            {
                NebulaClient.Playlists.RemovePlaylist(Playlist);
                NebulaClient.Navigate(typeof(PlaylistsPage));
            }
        }

        private async void OnEditPlaylistClick(object sender, RoutedEventArgs e)
        {
            PlaylistEditDialog dialog = new PlaylistEditDialog(PlaylistEditDialogAction.EditPlaylist, Playlist);
            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string oldname = Playlist.Name;
                Playlist.Name = dialog.PlaylistName.Text;
                Playlist.Description = dialog.PlaylistDescription.Text;
                Playlist.Author = dialog.PlaylistAuthor.Text;
                Playlist.Thumbnail = string.IsNullOrWhiteSpace(dialog.PlaylistThumbnail.Text)
                    ? null
                    : new Uri(dialog.PlaylistThumbnail.Text);
                if (oldname != Playlist.Name)
                    NebulaClient.Playlists.RenamePlaylist(oldname, Playlist);
                else
                    NebulaClient.Playlists.SavePlaylist(Playlist);
                NebulaClient.Navigate(typeof(PlaylistPage), Playlist);
            }
        }

        private void OnSearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                if (Medias.Count == Playlist.MediasCount)
                    return;
                Medias.UpdateMedias();
                return;
            }

            Medias.Clear();
            foreach (IMediaInfo mediaInfo in Playlist)
            {
                if (mediaInfo.Title.ToLower().Contains(SearchBox.Text.ToLower()))
                    Medias.Add(mediaInfo);
            }
        }

        private void OnBackPageClick(object sender, RoutedEventArgs e)
        {
            Medias.PreviousPage();
        }

        private void OnForwardPageClick(object sender, RoutedEventArgs e)
        {
            Medias.NextPage();
        }

        private void OnSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
                Medias.UpdateMedias();
        }
    }
}