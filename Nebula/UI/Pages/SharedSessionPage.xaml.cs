using System.Windows;
using System.Windows.Navigation;
using Nebula.Core;
using Nebula.Net.Packets.C2S;
using Page = ModernWpf.Controls.Page;

namespace Nebula.UI.Pages
{
    public partial class SharedSessionPage : Page
    {
        public SharedSessionPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NebulaClient.SharedSession == null)
                return;
            DataContext = NebulaClient.SharedSession;
        }

        private void OnLeaveClick(object sender, RoutedEventArgs e)
        {
            NebulaClient.SharedSession.Leave();
            NebulaClient.Network.SendPacket(new SharedSessionLeavePacket());
        }
    }
}