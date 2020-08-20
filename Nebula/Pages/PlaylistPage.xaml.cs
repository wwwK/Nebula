using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Playlist;
using Nebula.Pages.Dialogs;

namespace Nebula.Pages
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

        public ObservableCollection<IMediaInfo> Medias { get; } = new ObservableCollection<IMediaInfo>();

        private int CurrentPage { get; set; }


        private void PreviousPage()
        {
            if (CurrentPage == 0)
                CurrentPage = Playlist.Medias.TotalPages - 1;
            else
                CurrentPage--;
            RefreshMedias();
        }

        private void ForwardPage()
        {
            if (CurrentPage + 1 == Playlist.Medias.TotalPages)
                CurrentPage = 0;
            else
                CurrentPage++;
            RefreshMedias();
        }

        private void RefreshMedias()
        {
            CurrentPageText.Text = $"{CurrentPage + 1}/{Playlist.Medias.TotalPages}";
            Medias.Clear();
            foreach (IMediaInfo mediaInfo in Playlist.Medias.GetMediasFromPage(CurrentPage))
                Medias.Add(mediaInfo);
        }

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

            if (e.ExtraData is IPlaylist playlist)
            {
                Playlist = playlist;
                PlaylistLogo.Source = new BitmapImage(playlist.Thumbnail);
                PlaylistTitle.Text = playlist.Name;
                PlaylistDescription.Text = playlist.Description;
                PlaylistAuthor.Text = string.Format(NebulaClient.GetLocString("By"), playlist.Author);
                ;
                PlaylistMediaCount.Text =
                    $"{string.Format(NebulaClient.GetLocString("PlaylistTitles"), playlist.MediasCount)} - {string.Format(NebulaClient.GetLocString("PlaylistTotalDuration"), playlist.TotalDuration)}";
                RefreshMedias();
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer.Width = ActualWidth - 7;
            ScrollViewer.Height = (ActualHeight - PlaylistInfoPanel.ActualHeight) - 10;
            PlaylistCommandBar.Margin = new Thickness(150 + 15,
                155 - (PlaylistCommandBar.ActualHeight), 0, 0);
        }

        private async void OnPlayClick(object sender, RoutedEventArgs e)
        {
            AppBarButton clicked = (AppBarButton) sender;
            if (clicked.DataContext is IMediaInfo mediaInfo)
                await NebulaClient.MediaPlayer.Open(mediaInfo, true);
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
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    if (Medias.Count != Playlist.MediasCount)
                    {
                        Medias.Clear();
                        foreach (IMediaInfo mediaInfo in Playlist)
                            Medias.Add(mediaInfo);
                    }

                    return;
                }

                Medias.Clear();
                foreach (IMediaInfo mediaInfo in Playlist)
                {
                    if (mediaInfo.Title.ToLower().Contains(SearchBox.Text.ToLower()))
                        Medias.Add(mediaInfo);
                }
            }
        }

        private void OnBackPageClick(object sender, RoutedEventArgs e)
        {
            PreviousPage();
        }

        private void OnForwardPageClick(object sender, RoutedEventArgs e)
        {
            ForwardPage();
        }

        private void OnSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
                RefreshMedias();
        }
    }
}