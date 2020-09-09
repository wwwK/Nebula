using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Extensions;
using Nebula.Core.Medias.Player;
using Nebula.Core.Medias.Player.Events;
using Nebula.Core.Medias.Provider;
using Unosquare.FFME.Common;
using MediaPlayer = Nebula.Core.Medias.Player.MediaPlayer;

namespace Nebula.UI.Controls
{
    public partial class MediaPlayerControl : UserControl
    {
        public MediaPlayerControl()
        {
            InitializeComponent();
            MediaPlayer.MediaChanged += OnMediaChanged;
            MediaPlayer.StateChanged += OnPlayerStateChanged;
            MediaPlayer.MuteChanged += OnPlayerMuteChanged;
            MediaPlayer.MediaElement.PositionChanged += OnPositionChanged;
            MediaPlayer.VolumeChanged += (sender, args) => PlaybackVolume.Value = args.NewVolume;
            MediaPlayer.ShuffleChanged += (sender, args) => PlaybackShuffle.IsChecked = args.Shuffle;
            MediaPlayer.RepeatChanged += (sender, args) => PlaybackRepeat.IsChecked = args.Repeat;
            PlaybackVolume.ValueChanged += (sender, args) => MediaPlayer.Volume = args.NewValue;
            PlaybackShuffle.Click += (sender, args) => MediaPlayer.Shuffle = !MediaPlayer.Shuffle;
            PlaybackRepeat.Click += (sender, args) => MediaPlayer.Repeat = !MediaPlayer.Repeat;
            PlaybackMute.Click += (sender, args) => MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
            PlaybackBack.Click += async (sender, args) => await MediaPlayer.Backward();
            PlaybackForward.Click += async (sender, args) => await MediaPlayer.Forward(true);
            PlaybackVolume.Value = MediaPlayer.Volume;
        }

        private MediaPlayer MediaPlayer => NebulaClient.MediaPlayer;

        private void UpdateMediaInfoWidth()
        {
            Point relativePoint = PlaybackShuffle.TransformToAncestor(RootGrid).Transform(new Point(0, 0));
            double availableSpace = relativePoint.X - MediaThumbnail.ActualWidth - 10;
            MediaInfoPanel.Width = availableSpace > 1 ? availableSpace : 1;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PlaybackPosition.Width =
                ActualWidth - (PlaybackPositionText.ActualWidth + PlaybackRemaining.ActualWidth + 20); // 20 = Margin for Left & Right pos + Margin for progress
            UpdateMediaInfoWidth();
        }

        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.IsPaused)
                MediaPlayer.Resume();
            else
                MediaPlayer.Pause();
        }

        private void OnPlaybackProgressOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MediaPlayer.SetPosition(e.GetPosition(PlaybackPosition).X / (PlaybackPosition.ActualWidth / PlaybackPosition.Maximum));
        }

        private void OnPlaybackProgressMouseMove(object sender, MouseEventArgs e)
        {
            TimeSpan currentMouseTimePos =
                TimeSpan.FromSeconds(Mouse.GetPosition(PlaybackPosition).X /
                                     (PlaybackPosition.ActualWidth / PlaybackPosition.Maximum));
            PlaybackPosition.ToolTip = currentMouseTimePos.ToFormattedHMS();
        }

        private void OnMediaChanged(object sender, MediaChangedEventArgs e)
        {
            IMediaProvider mediaProvider = e.NewMedia.GetMediaProvider();
            MediaThumbnail.Source = new BitmapImage(new Uri(e.NewMedia.ThumbnailUrl));
            MediaTitle.Text = e.NewMedia.Title;
            MediaAuthor.Text = e.NewMedia.Author;
            MediaProvider.Text = mediaProvider.Name;
            MediaProvider.Foreground = new SolidColorBrush((Color) (ColorConverter.ConvertFromString(mediaProvider.NameColorEx) ?? Colors.Gray));
            PlaybackPosition.Minimum = 0;
            PlaybackPosition.Maximum = e.NewMedia.Duration.TotalSeconds;
            PlaybackRemaining.Text = e.NewMedia.Duration.ToString();
            PlaybackVolume.Value = NebulaClient.MediaPlayer.Volume;
            PlaybackPlay.Icon = new SymbolIcon(Symbol.Pause);
            PlaybackPlay.Label = NebulaClient.GetLocString("Pause");
            UpdateMediaInfoWidth();
        }

        private void OnPlayerMuteChanged(object sender, MuteChangedEventArgs e)
        {
            if (e.IsMuted)
            {
                PlaybackMute.Icon = new SymbolIcon(Symbol.Mute);
                PlaybackMute.Label = NebulaClient.GetLocString("Unmute");
            }
            else
            {
                PlaybackMute.Icon = new SymbolIcon(Symbol.Volume);
                PlaybackMute.Label = NebulaClient.GetLocString("Mute");
            }
        }

        private void OnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            PlaybackPosition.Value = e.Position.TotalSeconds;
            PlaybackPositionText.Text = e.Position.ToFormattedHMS();
            if (MediaPlayer.CurrentMedia != null)
                PlaybackRemaining.Text = (MediaPlayer.CurrentMedia.Duration - e.Position).ToFormattedHMS();
        }

        private void OnPlayerStateChanged(object sender, StateChangedEventArgs e)
        {
            switch (e.NewState)
            {
                case MediaPlayerState.Paused:
                    PlaybackPlay.Icon = new SymbolIcon(Symbol.Play);
                    PlaybackPlay.Label = NebulaClient.GetLocString("Play");
                    break;
                case MediaPlayerState.Playing:
                    PlaybackPlay.Icon = new SymbolIcon(Symbol.Pause);
                    PlaybackPlay.Label = NebulaClient.GetLocString("Pause");
                    break;
            }
        }
    }
}