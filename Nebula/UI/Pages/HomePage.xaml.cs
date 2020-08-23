using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Navigation;
using Nebula.Core;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Events;
using Page = ModernWpf.Controls.Page;

namespace Nebula.UI.Pages
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public MediasCollectionPages Medias { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Medias = NebulaClient.MediaPlayer.Queue.GetPages();
            Medias.PageChanged += OnPageChanged;
            Username.Text = NebulaClient.GetLocString("HomeWelcome", NebulaClient.Settings.UserProfile.Username);
            TitlesDuration.Text = Medias.Medias.Count > 0 ? $"{NebulaClient.GetLocString("PlaylistTitles", Medias.Medias.Count)}" : "";
            PagesCount.Text = Medias.TotalPages != 0 ? $"{Medias.CurrentPage + 1}/{Medias.TotalPages}" : "1/1";
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Medias.PageChanged -= OnPageChanged;
        }

        private void OnPageChanged(object sender, MediaCollectionPageChangedEventArgs e)
        {
            PagesCount.Text = Medias.TotalPages != 0 ? $"{Medias.CurrentPage + 1}/{Medias.TotalPages}" : "1/1";
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            Medias.PreviousPage();
        }

        private void OnForwardClick(object sender, RoutedEventArgs e)
        {
            Medias.NextPage();
        }
    }
}