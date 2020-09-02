using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LiteNetLib;
using Nebula.Core;
using Nebula.Shared.Packets.C2S;
using Nebula.Shared.Packets.S2C;
using Nebula.Shared.SharedSession;
using Page = ModernWpf.Controls.Page;

namespace Nebula.UI.Pages
{
    public partial class SharedSessionsPage : Page
    {
        public SharedSessionsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionsListResponse, NetPeer>(OnReceiveSharedSessions);
            if (!NebulaClient.Network.IsConnected)
                NebulaClient.Network.Connect();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NebulaClient.Network.PacketProcessor.RemoveSubscription<SharedSessionsListResponse>();
        }

        private void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            NebulaClient.Network.SendPacket(new SharedSessionsListRequest());
        }

        private void OnReceiveSharedSessions(SharedSessionsListResponse response, NetPeer peer)
        {
            NebulaClient.Invoke(() =>
            {
                foreach (string sessionStr in response.Sessions)
                {
                    SharedSessionInfo sessionInfo = SharedSessionInfo.FromString(sessionStr);
                    if (sessionInfo == null)
                        continue;
                    ListView.Items.Add(sessionInfo);
                } 
            });
        }
    }
}