using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Player.Events;
using Nebula.Core.UI;

namespace Nebula
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NavView.PaneOpening += (sender, args) => UpdateAppTitleMargin(sender);
            NavView.PaneClosing += (sender, args) => UpdateAppTitleMargin(sender);
            NebulaClient.MediaPlayer.MediaChanged += OnMediaChanged;
            NebulaClient.MediaPlayer.PlaybackPositionChanged += PlaybackPositionChanged;
            NebulaClient.MediaPlayer.PlaybackVolumeChanged += OnPlaybackVolumeChanged;
            NebulaClient.MediaPlayer.PlaybackMuteChanged += OnPlaybackMuteChanged;
            NebulaClient.MediaPlayer.RepeatChanged += OnPlaybackRepeatChanged;
            NebulaClient.MediaPlayer.ShuffleChanged += OnPlaybackShuffleChanged;
            FrameTracker = new FrameNavigationTracker(ContentFrame);
        }

        private FrameNavigationTracker FrameTracker { get; }

        private void UpdateAppTitleMargin(NavigationView sender)
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24;
            Thickness currMargin = AppTitle.Margin;

            if ((sender.DisplayMode == NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                sender.DisplayMode == NavigationViewDisplayMode.Minimal)
                AppTitle.Margin = new Thickness(smallLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            else
                AppTitle.Margin = new Thickness(largeLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs args)
        {
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top,
                TitleBar.GetSystemOverlayRightInset(this), currMargin.Bottom);
        }

        private void OnNavViewDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = sender.DisplayMode == NavigationViewDisplayMode.Minimal
                ? new Thickness(sender.CompactPaneLength * 2, currMargin.Top, currMargin.Right, currMargin.Bottom)
                : new Thickness(sender.CompactPaneLength, currMargin.Top, currMargin.Right, currMargin.Bottom);
            UpdateAppTitleMargin(sender);
        }

        private void OnNavViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                //
            }
            else if (args.InvokedItemContainer is NavigationViewItem navItem)
                NebulaClient.Navigate(navItem.Tag as Type);
        }

        private void OnNavViewBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            ContentFrame.GoBack();
        }

        private void OnMediaProgressOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            NebulaClient.MediaPlayer.Position =
                TimeSpan.FromSeconds(e.GetPosition(MediaProgress).X /
                                     (MediaProgress.ActualWidth / MediaProgress.Maximum));
        }

        private void OnMediaProgressMouseMove(object sender, MouseEventArgs e)
        {
            TimeSpan currentMouseTimePos =
                TimeSpan.FromSeconds(Mouse.GetPosition(MediaProgress).X /
                                     (MediaProgress.ActualWidth / MediaProgress.Maximum));
            MediaProgress.ToolTip = currentMouseTimePos.ToString("hh\\:mm\\:ss");
        }

        private void OnPlaybackVolumeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            NebulaClient.MediaPlayer.Volume = (int) e.NewValue;
        }

        private void OnPlaybackMuteChanged(object sender, PlaybackMuteChangedEventArgs e)
        {
            if (e.IsMuted)
            {
                MediaMute.Icon = new SymbolIcon(Symbol.Mute);
                MediaMute.Label = "Unmute";
            }
            else
            {
                MediaMute.Icon = new SymbolIcon(Symbol.Volume);
                MediaMute.Label = "Mute";
            }
        }

        private void OnPlaybackRepeatChanged(object sender, PlaybackRepeatChangedEventArgs e)
        {
            PlaybackRepeat.IsChecked = e.Repeat;
        }

        private void OnPlaybackShuffleChanged(object sender, PlaybackShuffleChangedEventArgs e)
        {
            PlaybackShuffle.IsChecked = e.Shuffle;
        }

        private void OnMediaChanged(object sender, MediaChangedEventArgs e)
        {
            MediaTitle.Text = Truncate(e.NewMedia.Title, 50);
            MediaProgress.Minimum = 0.0;
            MediaProgress.Maximum = e.NewMedia.Duration.TotalSeconds;
            PlaybackVolume.Value = NebulaClient.MediaPlayer.Volume;
            PlaybackPlay.Icon = new SymbolIcon(Symbol.Pause);
            PlaybackPlay.Label = "Pause";
        }

        private void PlaybackPositionChanged(object sender, TimeSpan e)
        {
            MediaProgress.Value = e.TotalSeconds;
        }

        private void OnPlaybackVolumeChanged(object sender, PlaybackVolumeChangedEventArgs e)
        {
            PlaybackVolume.Value = e.NewVolume;
        }

        private void OnPlaybackPlayClicked(object sender, RoutedEventArgs e)
        {
            AppBarButton playBtn = (AppBarButton) sender;
            if (NebulaClient.MediaPlayer.IsPaused)
            {
                playBtn.Icon = new SymbolIcon(Symbol.Pause);
                playBtn.Label = "Pause";
                NebulaClient.MediaPlayer.Resume();
            }
            else
            {
                playBtn.Icon = new SymbolIcon(Symbol.Play);
                playBtn.Label = "Play";
                NebulaClient.MediaPlayer.Pause();
            }
        }

        private void OnPlaybackRepeatClicked(object sender, RoutedEventArgs e)
        {
            NebulaClient.MediaPlayer.Repeat = !NebulaClient.MediaPlayer.Repeat;
        }

        private void OnPlaybackShuffleClicked(object sender, RoutedEventArgs e)
        {
            NebulaClient.MediaPlayer.Shuffle = !NebulaClient.MediaPlayer.Shuffle;
        }

        private void OnPlaybackMuteClicked(object sender, RoutedEventArgs e)
        {
            NebulaClient.MediaPlayer.IsMuted = !NebulaClient.MediaPlayer.IsMuted;
        }

        public static string Truncate(string value, int maxChars) //Todo: move to helper class
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }
    }
}