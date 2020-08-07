using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AngleSharp.Common;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using Nebula.Core;
using Nebula.Core.Medias;
using Page = ModernWpf.Controls.Page;

namespace Nebula.Pages
{
    public partial class BrowsePage : Page
    {
        public BrowsePage()
        {
            InitializeComponent();
            DataContext = this;
            SizeChanged += OnSizeChanged;
        }

        private IMediaInfo CurrentRightClick { get; set; }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer.Width = ActualWidth;
            ScrollViewer.Height = ActualHeight - CommandBar.ActualHeight;
        }

        public ObservableCollection<IMediaInfo> Medias { get; } = new ObservableCollection<IMediaInfo>();

        private async void Search(string query)
        {
            NebulaClient.Session.AddBrowserSearch(SearchBox.Text);
            Medias.Clear();
            IAsyncEnumerable<IMediaInfo> medias = NebulaClient.Search(query, 0, 1);
            await foreach (IMediaInfo mediaInfo in medias)
                NebulaClient.BeginInvoke(() => { Medias.Add(mediaInfo); }); //Todo: BeginInvoke > Invoke
        }

        private async void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search(SearchBox.Text);
            }
        }

        private async void OnMediaItemAuthorMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock clicked = (TextBlock) sender;
            if (clicked.DataContext is IMediaInfo mediaInfo)
                NebulaClient.Navigate(typeof(ArtistPage), await mediaInfo.GetArtistInfo(),
                    new DrillInNavigationTransitionInfo());
        }

        private void OnMediaItemMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image && image.DataContext is IMediaInfo mediaInfo)
            {
                if (e.ChangedButton == MouseButton.Left)
                    NebulaClient.MediaPlayer.Open(mediaInfo);
                else if (e.ChangedButton == MouseButton.Right)
                    CurrentRightClick = mediaInfo;
            }
        }

        private void OnAddToListeningSessionClicked(object sender, RoutedEventArgs e)
        {
            NebulaClient.MediaPlayer.Session.AddMedia(CurrentRightClick);
        }

        private void OnMenuPlayClicked(object sender, RoutedEventArgs e)
        {
            NebulaClient.MediaPlayer.Open(CurrentRightClick);
        }

        private void OnSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                sender.ItemsSource = NebulaClient.Session.GetSearchHistory();
            else if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
                Search(SearchBox.Text);
        }
    }
}