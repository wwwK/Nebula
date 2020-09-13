using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Events;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.UI.Dialogs;
using PlaylistEditDialog = Nebula.Core.UI.Dialogs.PlaylistEditDialog;

namespace Nebula.UI.Pages
{
    public partial class PlaylistPage : Page
    {
        private bool _isLoaded = false;

        public PlaylistPage()
        {
            InitializeComponent();

            DataContext = this;
            Focus();
        }

        public IPlaylist Playlist { get; private set; }

        public MediasCollectionPages Medias { get; private set; }

        private bool IsFullyLoaded
        {
            get => _isLoaded;
            set
            {
                _isLoaded = value;
                PlayThis.IsEnabled = value;
            }
        }

        private void RemoveMedia(IMediaInfo mediaInfo)
        {
            Playlist.RemoveMedia(mediaInfo);
            if (NebulaClient.MediaPlayer.MediaQueue.IsQueued(mediaInfo))
                NebulaClient.MediaPlayer.MediaQueue.Remove(mediaInfo);
            Medias.Remove(mediaInfo);
        }

        private void UpdateMedias()
        {
            Medias.UpdateMedias();
            PlaylistMediaCount.Text =
                $"{string.Format(NebulaClient.GetLocString("PlaylistTitles"), Medias.Collection.Count)} - {string.Format(NebulaClient.GetLocString("PlaylistTotalDuration"), Medias.Collection.TotalDuration)}";
            CurrentPageText.Text = $"{Medias.CurrentPage + 1}/{Playlist.Medias.TotalPages}";
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!(e.ExtraData is IPlaylist playlist))
                return;
            Playlist = playlist;
            PlaylistLogo.Source = new BitmapImage(playlist.Thumbnail);
            PlaylistTitle.Text = playlist.Name;
            PlaylistDescription.Text = playlist.Description;
            PlaylistAuthor.Text = string.Format(NebulaClient.GetLocString("By"), playlist.Author);
            Medias = new MediasCollectionPages(playlist.Medias);
            Medias.PageChanged += OnPageChanged;
            UpdateMedias();
            if (playlist.Tag is IArtistInfo artistInfo)
            {
                PlaylistAuthor.Text = string.Empty;
                int count = 0;
                await foreach (IMediaInfo mediaInfo in artistInfo.GetMedias())
                {
                    playlist.AddMedia(mediaInfo);
                    count++;
                    if (count >= NebulaClient.Settings.General.PlaylistMaxMediasPerPage)
                    {
                        UpdateMedias();
                        count = 0;
                    }
                }

                UpdateMedias();
            }

            IsFullyLoaded = true;
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

        private async void OnListenPlaylistClick(object sender, RoutedEventArgs e)
        {
            await NebulaClient.MediaPlayer.OpenPlaylist(Playlist);
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
            await new PlaylistEditDialog(Playlist).ShowDialogAsync();
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
            foreach (IMediaInfo mediaInfo in Playlist.Medias)
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