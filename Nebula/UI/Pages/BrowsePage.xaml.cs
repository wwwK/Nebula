using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using Nebula.Core;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.Medias.Playlist.Playlists;
using Nebula.Core.UI.Dialogs;
using Page = ModernWpf.Controls.Page;

namespace Nebula.UI.Pages
{
    public partial class BrowsePage : Page
    {
        public BrowsePage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private IMediaInfo                       CurrentRightClick { get; set; }
        public  ObservableCollection<IMediaInfo> Medias            { get; } = new ObservableCollection<IMediaInfo>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!(e.ExtraData is string searchQuery))
                return;
            SearchBox.Text = searchQuery;
            Search(searchQuery);
        }

        public async void Search(string query)
        {
            NebulaClient.Session.AddBrowserSearch(SearchBox.Text);
            Medias.Clear();
            IAsyncEnumerable<IMediaInfo> medias = NebulaClient.Search(query);
            await foreach (IMediaInfo mediaInfo in medias)
                Medias.Add(mediaInfo);
        }

        private void OnSearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            Search(SearchBox.Text);
            SearchBox.IsSuggestionListOpen = false;
        }

        private async void OnMediaItemAuthorMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBlock clicked) || !(clicked.DataContext is IMediaInfo mediaInfo))
                return;
            IArtistInfo artistInfo = await mediaInfo.GetArtistInfo();
            UnknownPlaylist playlist = await UnknownPlaylist.FromArtist(artistInfo, false);
            NebulaClient.Navigate(typeof(PlaylistPage), playlist, new DrillInNavigationTransitionInfo());
        }

        private async void OnMediaItemMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Image image) || !(image.DataContext is IMediaInfo mediaInfo))
                return;
            if (e.ChangedButton == MouseButton.Left)
                await NebulaClient.MediaPlayer.OpenMedia(mediaInfo, true);
            else if (e.ChangedButton == MouseButton.Right)
                CurrentRightClick = mediaInfo;
        }

        private void OnAddToListeningSessionClicked(object sender, RoutedEventArgs e)
        {
            NebulaClient.MediaPlayer.Queue.Enqueue(CurrentRightClick);
        }

        private async void OnMenuPlayClicked(object sender, RoutedEventArgs e)
        {
            await NebulaClient.MediaPlayer.OpenMedia(CurrentRightClick);
        }

        private void OnSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            switch (args.Reason)
            {
                case AutoSuggestionBoxTextChangeReason.UserInput:
                    sender.ItemsSource = NebulaClient.Session.GetSearchHistory();
                    break;
                case AutoSuggestionBoxTextChangeReason.SuggestionChosen:
                    Search(SearchBox.Text);
                    break;
            }
        }

        private void OnAddToPlaylistSubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem item))
                return;
            if (item.Items.Count > 2)
            {
                for (int i = item.Items.Count - 1; i > 1; i--)
                    item.Items.RemoveAt(i);
            }

            foreach (IPlaylist playlist in NebulaClient.Playlists.GetPlaylists())
                item.Items.Add(new MenuItem {Header = playlist.Name, Tag = playlist});
        }

        private async void OnAddToPlaylistClick(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is MenuItem item))
                return;
            switch (item.Tag)
            {
                case IPlaylist playlist:
                {
                    if (playlist.Contains(CurrentRightClick))
                    {
                        ContentDialogResult result = await NebulaMessageBox.ShowYesNo("MediaAlreadyExists",
                            "MediaAlreadyExistsMsg",
                            CurrentRightClick.Title);
                        if (result == ContentDialogResult.Primary)
                            playlist.AddMedia(CurrentRightClick);
                    }
                    else
                        playlist.AddMedia(CurrentRightClick);
                }
                    break;
                case "CREATE_PLAYLIST":
                {
                    PlaylistCreationDialog dialog = new PlaylistCreationDialog();
                    ContentDialogResult result = await dialog.ShowDialogAsync();
                    if (result == ContentDialogResult.Primary)
                        dialog.CreatedPlaylist?.AddMedia(CurrentRightClick);
                }
                    break;
            }
        }
    }
}