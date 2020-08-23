using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using EasySharp.Misc;
using EasySharp.Units;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Updater;
using Nebula.Pages.Dialogs;
using Page = ModernWpf.Controls.Page;

namespace Nebula.UI.Pages.Settings
{
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();

            AboutNebula.Text = string.Format(NebulaClient.GetLocString("AboutNebula"),
                Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (UpdateDownloadStatus.Visibility == Visibility.Collapsed)
            {
                CheckForUpdateButton.IsEnabled = false;
                UpdateDownloadStatus.Visibility = Visibility.Visible;
            }

            EUnit unit = EUnit.Byte;
            UpdateDownloadStatus.Text =
                $"Downloading Update file '{NebulaClient.Updater.CurrentDownloadedFile}' {new ByteUnit(e.BytesReceived).ToHumanReadable(unit)} / {new ByteUnit(e.TotalBytesToReceive).ToHumanReadable(unit)} {e.ProgressPercentage}%";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NebulaClient.Updater.WebClient.DownloadProgressChanged += OnDownloadProgressChanged;
            if (e.ExtraData is string str && str == "downloadUpdate")
                CheckForUpdateButton.IsEnabled = false;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NebulaClient.Updater.WebClient.DownloadProgressChanged -= OnDownloadProgressChanged;
        }

        private async void OnCheckForUpdateClick(object sender, RoutedEventArgs e)
        {
            CheckForUpdateButton.IsEnabled = false;
            await NebulaClient.CheckForUpdate(false);
            CheckForUpdateButton.IsEnabled = true;
        }
    }
}