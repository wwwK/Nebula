using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AngleSharp.Common;
using ModernWpf.Media.Animation;
using Nebula.Core;
using Nebula.Core.Medias;

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

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer.Width = ActualWidth;
            ScrollViewer.Height = ActualHeight - CommandBar.ActualHeight;
        }

        public ObservableCollection<IMediaInfo> Medias { get; } = new ObservableCollection<IMediaInfo>();

        private async void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Medias.Clear();
                IAsyncEnumerable<IMediaInfo> medias = NebulaClient.Search(SearchBox.Text, 0, 1);
                await foreach (IMediaInfo mediaInfo in medias)
                    Application.Current.Dispatcher.Invoke(() => { Medias.Add(mediaInfo); });
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
            if (e.ChangedButton == MouseButton.Left && sender is Image image &&
                image.DataContext is IMediaInfo mediaInfo)
                NebulaClient.MediaPlayer.Open(mediaInfo);
        }
    }
}