using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using LiteNetLib;
using Nebula.Core;
using Nebula.Core.Networking.Events;
using Nebula.Core.UI.Dialogs;
using Nebula.Net.Packets;
using Nebula.Net.Packets.C2S;
using Nebula.Net.Packets.S2C;
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
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionsPollResponse, NetPeer>(OnReceiveSharedSessions);
            NebulaClient.Network.Connected += OnNetworkConnected;
            if (!NebulaClient.Network.IsConnected)
                NebulaClient.Network.Connect();
            else
                NebulaClient.Network.SendPacket(new SharedSessionsPollRequest());
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NebulaClient.Network.PacketProcessor.RemoveSubscription<SharedSessionsPollResponse>();
            NebulaClient.Network.Connected -= OnNetworkConnected;
        }

        private void OnNetworkConnected(object sender, ConnectedToServerEventArgs e)
        {
            NebulaClient.Network.SendPacket(new SharedSessionsPollRequest());
        }

        private void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            NebulaClient.Network.SendPacket(new SharedSessionsPollRequest());
        }

        private async void OnCreateRoomClick(object sender, RoutedEventArgs e)
        {
            await new SharedSessionRoomCreationDialog().ShowDialogAsync();
        }

        private void OnReceiveSharedSessions(SharedSessionsPollResponse response, NetPeer peer)
        {
            NebulaClient.Invoke(() =>
            {
                ListView.Items.Clear();
                foreach (SharedSessionInfo sessionInfo in response.Sessions)
                    ListView.Items.Add(sessionInfo);
            });
        }

        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(ListView.SelectedItem is SharedSessionInfo info))
                return;
            SharedSessionJoinRequest request = new SharedSessionJoinRequest {Session = info, Password = String.Empty};
            if (info.PasswordProtected)
            {
                SharedSessionJoinDialog dialog = new SharedSessionJoinDialog();
                await dialog.ShowDialogAsync();
                request.Password = dialog.RoomPassword;
            }

            NebulaClient.Network.SendPacket(request);
        }
    }
}