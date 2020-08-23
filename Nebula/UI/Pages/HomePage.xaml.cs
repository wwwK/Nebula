using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Nebula.Core;
using Nebula.Core.Extensions;
using Nebula.Core.Medias;
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

        private MediasCollectionManager          QueueManager { get; set; }
        public  ObservableCollection<IMediaInfo> Medias       { get; } = new MediasCollection();

        private void RefreshMedias()
        {
            Medias.Clear();
            PagesCount.Text = QueueManager.TotalPages != 0 ? $"{QueueManager.CurrentPage + 1}/{QueueManager.TotalPages}" : "1/1";
            foreach (IMediaInfo mediaInfo in QueueManager.GetMediasFromPage(QueueManager.CurrentPage))
                Medias.Add(mediaInfo);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            QueueManager = NebulaClient.MediaPlayer.Queue.GetQueueManager();
            Username.Text = NebulaClient.GetLocString("HomeWelcome", NebulaClient.Settings.UserProfile.Username);
            if (QueueManager.Medias.Count > 0)
                TitlesDuration.Text = $"{NebulaClient.GetLocString("PlaylistTitles", QueueManager.Medias.Count)}";
            RefreshMedias();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*
            ListeningSessionPanel.Height = ActualHeight - UserPanel.ActualHeight - (30 + 15); // 30 = Top & bot margin of Userpanel. 15 = bottomn margin
            ScrollViewer.Height = ListeningSessionPanel.ActualHeight - (ListeningSessionTitle.ActualHeight + PageControls.ActualHeight + 5);
            System.Diagnostics.Debug.Print(ListeningSessionPanel.ActualHeight + "");
            System.Diagnostics.Debug.Print(ScrollViewer.ActualHeight + ""); */
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            QueueManager.PreviousPage();
            RefreshMedias();
        }

        private void OnForwardClick(object sender, RoutedEventArgs e)
        {
            QueueManager.NextPage();
            RefreshMedias();
        }
    }
}