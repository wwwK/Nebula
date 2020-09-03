using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using LiteNetLib;
using Nebula.Core;
using Nebula.Core.Dialogs;
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

        private async void OnCreateRoomClick(object sender, RoutedEventArgs e)
        {
            await new SharedSessionRoomCreationDialog().ShowDialogAsync();
        }

        private void OnReceiveSharedSessions(SharedSessionsListResponse response, NetPeer peer)
        {
            NebulaClient.Invoke(() =>
            {
                ListView.Items.Clear();
                foreach (string sessionStr in response.Sessions)
                {
                    SharedSessionInfo sessionInfo = SharedSessionInfo.FromString(sessionStr);
                    if (sessionInfo == null)
                        continue;
                    ListView.Items.Add(sessionInfo);
                }
            });
        }

        private async void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(ListView.SelectedItem is SharedSessionInfo info))
                return;
            SharedSessionJoinRequest request = new SharedSessionJoinRequest {Id = info.Id.ToString(), Password = String.Empty};
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