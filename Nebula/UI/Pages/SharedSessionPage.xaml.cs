using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Nebula.Core;
using Nebula.Core.Extensions;
using Nebula.Net.Packets.BOTH;
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
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            DataContext = NebulaClient.SharedSession;
            NebulaClient.SharedSession.Messages.CollectionChanged += OnMessageAddedChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NebulaClient.SharedSession.Messages.CollectionChanged += OnMessageAddedChanged;
        }

        private void OnMessageAddedChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;
            MessageList.UpdateLayout();
            MessageList.GetChildOfType<ScrollViewer>()?.ScrollToBottom();
        }

        private void OnLeaveClick(object sender, RoutedEventArgs e)
        {
            NebulaClient.SharedSession.Leave();
            NebulaClient.Network.SendPacket(new SharedSessionLeavePacket());
        }

        private void OnSendClick(object sender, RoutedEventArgs e)
        {
            SendMessage(TextMessage.Text);
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            NebulaClient.SharedSession.ClearMessages();
        }

        private void OnTextMessageKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage(TextMessage.Text);
        }

        private void SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;
            NebulaClient.Network.SendPacket(new SharedSessionUserMessagePacket {Message = message});
            TextMessage.Text = string.Empty;
        }
    }
}