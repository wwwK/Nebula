using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ModernWpf.Media.Animation;
using Nebula.Core;
using Nebula.Pages.Settings;
using Page = ModernWpf.Controls.Page;

namespace Nebula.Pages
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
            switch (split[0])
            {
                case "GENERAL":
                    SettingsNavFrame.Navigate(typeof(GeneralPage), null,
                        new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft});
                    break;
                case "APPEARANCE":
                    SettingsNavFrame.Navigate(typeof(AppearancePage), null,
                        new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft});
                    break;
                case "PRIVACY":
                    SettingsNavFrame.Navigate(typeof(PrivacyPage), null,
                        new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft});
                    break;
                case "ABOUT":
                    SettingsNavFrame.Navigate(typeof(AboutPage), split.Length == 2 ? split[1] : null,
                        new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft});
                    break;
            }

            Header.Text = (MenuListView.SelectedItem as ListViewItem)?.Content.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.ExtraData is string page)
                NavigateTo(page);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is ListViewItem item)
                NavigateTo(item.Tag.ToString());
        }

        private void OnSaveSettingsClick(object sender, RoutedEventArgs e)
        {
            NebulaClient.Settings.SaveSettings();
        }
    }
}