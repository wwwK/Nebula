using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ModernWpf.Media.Animation;
using Nebula.Core;
using Nebula.Core.Settings;
using Nebula.Core.UI.Content;
using Nebula.UI.Pages.Settings;
using Page = ModernWpf.Controls.Page;

namespace Nebula.UI.Pages
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void NavigateTo(string pageTag)
        {
            string[] split = pageTag.Split('>');

            SimplePanelDataContent dataContent = split[0] switch
            {
                "GENERAL"    => NebulaClient.Settings.General,
                "SERVER"     => NebulaClient.Settings.Server,
                "PROFILE"    => NebulaClient.Settings.UserProfile,
                "APPEARANCE" => null,
                "PRIVACY"    => null,
                _            => null
            };

            if (dataContent != null)
            {
                SettingsNavFrame.Navigate(typeof(NebulaPage), dataContent,
                    new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft});
            }
            else
            {
                switch (split[0])
                {
                    case "APPEARANCE":
                        SettingsNavFrame.Navigate(typeof(AppearancePage),
                            new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft});
                        break;
                    case "PRIVACY":
                        SettingsNavFrame.Navigate(typeof(PrivacyPage),
                            new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft});
                        break;
                    case "ABOUT":
                        SettingsNavFrame.Navigate(typeof(AboutPage),
                            new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft});
                        break;
                }
            }

            Header.Text = (MenuListView.SelectedItem as ListViewItem)?.Content.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.ExtraData is string page)
                NavigateTo(page);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SettingsNavFrame.Content = null;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is ListViewItem item)
                NavigateTo(item.Tag.ToString());
        }

        private async void OnSaveSettingsClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button))
                return;
            button.IsEnabled = false;
            await NebulaSettings.SaveSettingsAsync();
            button.IsEnabled = true;
        }
    }
}