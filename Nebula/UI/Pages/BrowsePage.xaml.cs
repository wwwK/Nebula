using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using AngleSharp.Common;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using Nebula.Core;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Playlist;
using Nebula.Pages;
using Nebula.Pages.Dialogs;
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

            //NebulaClient.MediaPlayer.OpenUri(new Uri("http://185.52.127.155/fr/30001/mp3_128.mp3"));
            if (e.ExtraData is string searchQuery)
            {
                SearchBox.Text = searchQuery;
                Search(searchQuery);
            }
        }

        public async void Search(string query)
        {
            NebulaClient.Session.AddBrowserSearch(SearchBox.Text);
            Medias.Clear();
            IAsyncEnumerable<IMediaInfo> medias = NebulaClient.Search(query);
            await foreach (IMediaInfo mediaInfo in medias)
                NebulaClient.BeginInvoke(() => { Medias.Add(mediaInfo); }); //Todo: BeginInvoke > Invoke
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
            TextBlock clicked = (TextBlock) sender;
            if (clicked.DataContext is IMediaInfo mediaInfo)
                NebulaClient.Navigate(typeof(ArtistPage), await mediaInfo.GetArtistInfo(),
                    new DrillInNavigationTransitionInfo());
        }

        private async void OnMediaItemMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image && image.DataContext is IMediaInfo mediaInfo)
            {
                if (e.ChangedButton == MouseButton.Left)
                    await NebulaClient.MediaPlayer.OpenMedia(mediaInfo, true);
                else if (e.ChangedButton == MouseButton.Right)
                    CurrentRightClick = mediaInfo;
            }
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
                    PlaylistEditDialog dialog = new PlaylistEditDialog(PlaylistEditDialogAction.CreatePlaylist);
                    ContentDialogResult result = await dialog.ShowAsync(ContentDialogPlacement.Popup);
                    if (result == ContentDialogResult.Primary)
                    {
                        NebulaPlaylist playlist = new NebulaPlaylist(dialog.PlaylistName.Text,
                            dialog.PlaylistDescription.Text,
                            dialog.PlaylistAuthor.Text,
                            string.IsNullOrWhiteSpace(dialog.PlaylistThumbnail.Text)
                                ? null
                                : new Uri(dialog.PlaylistThumbnail.Text));
                        playlist.AddMedia(CurrentRightClick);
                        NebulaClient.Playlists.AddPlaylist(playlist);
                    }
                }
                    break;
            }
        }
    }
}