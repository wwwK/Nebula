using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Playlist;

namespace Nebula.Pages
{
    public partial class PlaylistPage : Page
    {
        public PlaylistPage()
        {
            InitializeComponent();

            DataContext = this;
        }

        public IPlaylist Playlist { get; private set; }

        public ObservableCollection<IMediaInfo> Medias { get; } = new ObservableCollection<IMediaInfo>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.ExtraData is IPlaylist playlist)
            {
                Playlist = playlist;
                PlaylistLogo.Source = new BitmapImage(playlist.Thumbnail);
                PlaylistTitle.Text = playlist.Name;
                PlaylistDescription.Text = playlist.Description;
                PlaylistAuthor.Text = $"By {playlist.Name}";
                PlaylistMediaCount.Text = $"{playlist.MediasCount} Title(s) | {playlist.TotalDuration}";
                foreach (IMediaInfo media in playlist)
                    Medias.Add(media);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer.Width = ActualWidth - 7;
            ScrollViewer.Height = (ActualHeight - PlaylistInfoPanel.ActualHeight) - 10;
        }

        private async void OnPlayClick(object sender, RoutedEventArgs e)
        {
            AppBarButton clicked = (AppBarButton) sender;
            if (clicked.DataContext is IMediaInfo mediaInfo)
                await NebulaClient.MediaPlayer.Open(mediaInfo);
        }

        private void OnRemoveClick(object sender, RoutedEventArgs e)
        {
            AppBarButton clicked = (AppBarButton) sender;
            if (clicked.DataContext is IMediaInfo mediaInfo)
            {
                Playlist.RemoveMedia(mediaInfo);
                if (NebulaClient.MediaPlayer.Queue.IsQueued(mediaInfo))
                    NebulaClient.MediaPlayer.Queue.Remove(mediaInfo);
                Medias.Remove(mediaInfo);
            }
        }
    }
}