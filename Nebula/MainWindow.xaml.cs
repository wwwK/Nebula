﻿using System;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shell;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Extensions;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Player.Events;
using Nebula.Core.Settings.Extentions;
using Nebula.Core.UI;
using Nebula.Pages;

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
            NebulaClient.MediaPlayer.PlaybackPaused += OnPlaybackPaused;
            NebulaClient.MediaPlayer.PlaybackResumed += OnPlaybackResumed;
            NebulaClient.MediaPlayer.RepeatChanged += OnPlaybackRepeatChanged;
            NebulaClient.MediaPlayer.ShuffleChanged += OnPlaybackShuffleChanged;
            FrameTracker = new FrameNavigationTracker(ContentFrame);
        }

        private FrameNavigationTracker FrameTracker { get; }

        public ImageBrush BackgroundBrush
        {
            get => ContentFrame.Background as ImageBrush;
            set
            {
                if (value == null)
                    ContentFrame.Background = new SolidColorBrush(Colors.Black);
                else
                    ContentFrame.Background = value;
            }
        }

        public void SetViewMode(string displayMode)
        {
            SetViewMode(Enum.TryParse(NebulaClient.Settings.Appearance.DisplayMode,
                out NavigationViewPaneDisplayMode mode)
                ? mode
                : NavigationViewPaneDisplayMode.Left);
        }

        public void SetViewMode(NavigationViewPaneDisplayMode displayMode)
        {
            NavView.PaneDisplayMode = displayMode;
            NavView.IsTitleBarAutoPaddingEnabled = false;
            if (displayMode == NavigationViewPaneDisplayMode.Top)
            {
                AppTitle.Visibility = Visibility.Collapsed;
                SettingsButton.Visibility = Visibility.Visible;
                ControlBoxRectangle.Visibility = Visibility.Visible;
                LibraryHeader.Visibility = Visibility.Collapsed;
                NavView.IsSettingsVisible = false;
                NavView.AlwaysShowHeader = false;
                SearchBox.Width = 250;
                SetHitTestVisibleInChrome(true, SearchBox, HomeButton, BrowseButton, PlaylistsButton,
                    RecentlyListenedButton, SettingsButton);
            }
            else
            {
                AppTitle.Visibility = Visibility.Visible;
                SettingsButton.Visibility = Visibility.Collapsed;
                LibraryHeader.Visibility = Visibility.Visible;
                ControlBoxRectangle.Visibility = Visibility.Collapsed;
                NavView.IsSettingsVisible = true;
                NavView.AlwaysShowHeader = true;
                SearchBox.Width = 200;
                SetHitTestVisibleInChrome(false, SearchBox, HomeButton, BrowseButton, PlaylistsButton,
                    RecentlyListenedButton, SettingsButton);
                UpdateAppTitleMargin(NavView);
            }

            UpdateMediaInfoWidth();
        }

        private void SetHitTestVisibleInChrome(bool value, params IInputElement[] elements)
        {
            foreach (IInputElement element in elements)
                WindowChrome.SetIsHitTestVisibleInChrome(element, value);
        }

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
            SetViewMode(NebulaClient.Settings.Appearance.DisplayMode);
            BackgroundBrush = NebulaClient.Settings.Appearance.GetBackgroundImageBrush();
        }

        private void OnPaneOpened(NavigationView sender, object obj)
        {
            UpdateMediaInfoWidth();
        }

        private void OnPaneClosed(NavigationView sender, object obj)
        {
            UpdateMediaInfoWidth();
        }

        private void OnNavViewDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = sender.DisplayMode == NavigationViewDisplayMode.Minimal
                ? new Thickness(sender.CompactPaneLength * 2, currMargin.Top, currMargin.Right, currMargin.Bottom)
                : new Thickness(sender.CompactPaneLength, currMargin.Top, currMargin.Right, currMargin.Bottom);
            UpdateAppTitleMargin(sender);
            UpdateMediaInfoWidth();
        }

        private void OnNavViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NebulaClient.Navigate(typeof(SettingsPage));
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


        private void OnPlaybackPaused(object sender, PlaybackPausedEventArgs e)
        {
            PlaybackPlay.Icon = new SymbolIcon(Symbol.Play);
            PlaybackPlay.Label = NebulaClient.GetLocString("Play");
        }

        private void OnPlaybackResumed(object sender, PlaybackResumedEventArgs e)
        {
            PlaybackPlay.Icon = new SymbolIcon(Symbol.Pause);
            PlaybackPlay.Label = NebulaClient.GetLocString("Pause");
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
                MediaMute.Label = NebulaClient.GetLocString("Unmute");
            }
            else
            {
                MediaMute.Icon = new SymbolIcon(Symbol.Volume);
                MediaMute.Label = NebulaClient.GetLocString("Mute");
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
            MediaTitle.Text = e.NewMedia.Title;
            MediaAuthor.Text = e.NewMedia.Author;
            MediaThumbnail.Height = CBar.ActualHeight;
            MediaThumbnail.Source = new BitmapImage(new Uri(e.NewMedia.ThumbnailUrl));
            MediaProgress.Minimum = 0.0;
            MediaProgress.Maximum = e.NewMedia.Duration.TotalSeconds;
            PlaybackVolume.Value = NebulaClient.MediaPlayer.Volume;
            PlaybackPlay.Icon = new SymbolIcon(Symbol.Pause);
            PlaybackPlay.Label = NebulaClient.GetLocString("Pause");
            UpdateMediaInfoWidth();
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
            if (NebulaClient.MediaPlayer.IsPaused)
                NebulaClient.MediaPlayer.Resume();
            else
                NebulaClient.MediaPlayer.Pause();
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

        private void OnPlaybackForwardClicked(object sender, RoutedEventArgs e)
        {
            NebulaClient.MediaPlayer.Forward(true);
        }

        private void OnPlaybackBackwardClicked(object sender, RoutedEventArgs e)
        {
            NebulaClient.MediaPlayer.Backward(true);
        }

        private void OnSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                sender.ItemsSource = NebulaClient.Session.GetSearchHistory();
            else if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
                NebulaClient.Navigate(typeof(BrowsePage), SearchBox.Text);
        }

        private void OnSearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NebulaClient.Navigate(typeof(BrowsePage), SearchBox.Text);
                SearchBox.IsSuggestionListOpen = false;
            }
        }

        private void UpdateMediaInfoWidth()
        {
            Point relativePoint = PlaybackShuffle.TransformToAncestor(CBarControls).Transform(new Point(0, 0));
            double availableSpace = relativePoint.X - MediaThumbnail.ActualWidth - 10;
            MediaInfosPanel.Width = availableSpace > 1 ? availableSpace : 1;
        }

        public static string Truncate(string value, int maxChars) //Todo: move to helper class
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMediaInfoWidth();
        }

        private void OnSettingsClicked(object sender, RoutedEventArgs e)
        {
            NebulaClient.Navigate(typeof(SettingsPage));
        }
    }
}