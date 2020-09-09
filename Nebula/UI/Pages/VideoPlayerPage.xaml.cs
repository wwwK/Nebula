using System.Windows.Navigation;
using Nebula.Core;
using Page = ModernWpf.Controls.Page;

namespace Nebula.UI.Pages
{
    public partial class VideoPlayerPage : Page
    {
        public VideoPlayerPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SimpleGrid.Children.Add(NebulaClient.MediaPlayer.MediaElement);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SimpleGrid.Children.Clear();
        }
    }
}