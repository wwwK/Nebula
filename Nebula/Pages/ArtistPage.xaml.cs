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
using Nebula.Pages.Dialogs;

namespace Nebula.Pages
{
    public partial class ArtistPage : Page
    {
        public ArtistPage()
        {
            InitializeComponent();

            DataContext = this;
        }

        public IArtistInfo ArtistInfo { get; private set; }

        public ObservableCollection<IMediaInfo> Medias { get; } = new ObservableCollection<IMediaInfo>();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.ExtraData is IArtistInfo artistInfo)
            {
                ArtistInfo = artistInfo;
                ArtistLogo.Source = new BitmapImage(new Uri(artistInfo.LogoUrl));
                ArtistTitle.Text = artistInfo.Title;
                ArtistUrl.Text = artistInfo.Url;
                int max = 20;
                int current = 0;
                await foreach (IMediaInfo mediaInfo in artistInfo.GetMedias())
                {
                    if (current >= max)
                        break;
                    Medias.Add(mediaInfo);
                    current++;
                }
            }
        }

        private void OnArtistUrlMouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", ArtistUrl.Text);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollViewer.Width = ActualWidth - 7;
            ScrollViewer.Height = (ActualHeight - ArtistInfoPanel.ActualHeight) - 10;
        }

        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            AppBarButton clicked = (AppBarButton) sender;
            if (clicked.DataContext is IMediaInfo mediaInfo)
                NebulaClient.MediaPlayer.Open(mediaInfo);
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
        }
    }
}