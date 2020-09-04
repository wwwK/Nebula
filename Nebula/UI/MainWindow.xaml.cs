using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Settings.Extentions;
using Nebula.UI.Pages;

namespace Nebula.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or Sets the background image
        /// </summary>
        public ImageSource BackgroundWallpaper
        {
            get => ImageBackground.Source;
            set => ImageBackground.Source = value;
        }

        /// <summary>
        /// Enable or Disable the ability to interact with the specified elements if they are behind the window title bar.
        /// </summary>
        /// <param name="value">Enable or Disable interaction</param>
        /// <param name="elements">Elements</param>
        private void SetHitTestVisibleInChrome(bool value, params IInputElement[] elements)
        {
            foreach (IInputElement element in elements)
                WindowChrome.SetIsHitTestVisibleInChrome(element, value);
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs args)
        {
            SetHitTestVisibleInChrome(true, SearchBox, HomeButton, BrowseButton, PlaylistsButton,
                RecentlyListenedButton, SettingsButton);
            BackgroundWallpaper = NebulaClient.Settings.Appearance.GetBackgroundImageSource();
            ImageBackground.Stretch = NebulaClient.Settings.Appearance.GetBackgroundImageStretch();
        }

        private void OnNavViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
                NebulaClient.Navigate(typeof(SettingsPage));
            else if (args.InvokedItemContainer is NavigationViewItem navItem && navItem.Tag is Type type)
            {
                if (type == typeof(SharedSessionsPage) && NebulaClient.SharedSession.IsSessionActive)
                    NebulaClient.Navigate(typeof(SharedSessionPage));
                else
                    NebulaClient.Navigate(type);
            }
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
            if (e.Key != Key.Enter)
                return;
            NebulaClient.Navigate(typeof(BrowsePage), SearchBox.Text);
            SearchBox.IsSuggestionListOpen = false;
        }

        private void OnSettingsClicked(object sender, RoutedEventArgs e)
        {
            NebulaClient.Navigate(typeof(SettingsPage));
        }
    }
}