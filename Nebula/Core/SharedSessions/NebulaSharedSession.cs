using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using LiteNetLib;
using ModernWpf.Media.Animation;
using Nebula.Core.UI.Dialogs;
using Nebula.Net.Packets;
using Nebula.Net.Packets.BOTH;
using Nebula.Net.Packets.C2S;
using Nebula.Net.Packets.S2C;
using Nebula.UI.Pages;

namespace Nebula.Core.SharedSessions
{
    public class NebulaSharedSession
    {
        public NebulaSharedSession()
        {
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionJoinResponse, NetPeer>(OnReceiveSharedSessionJoinResponse);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionUserJoinedPacket, NetPeer>(OnReceiveSharedSessionUserJoin);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionUserLeftPacket, NetPeer>(OnReceiveSharedSessionUserLeft);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionUserMessagePacket, NetPeer>(OnReceiveSessionUserMessagePacket);
        }

        public ObservableCollection<UserInfo>             Users             { get; } = new ObservableCollection<UserInfo>();
        public ObservableCollection<SharedSessionMessage> Messages          { get; } = new ObservableCollection<SharedSessionMessage>();
        public Guid                                       Id                { get; private set; }
        public string                                     Name              { get; set; }
        public string                                     ThumbnailUrl      { get; set; }
        public int                                        MaxUsers          { get; private set; }
        public bool                                       PasswordProtected { get; private set; }
        public bool                                       IsSessionActive   { get; private set; }
        public int                                        UsersCount        => Users?.Count ?? 0;

        private void SetSession(SharedSessionInfo sessionInfo, UserInfo[] users = null)
        {
            if (!NebulaClient.Network.IsConnected || sessionInfo == SharedSessionInfo.Empty)
            {
                IsSessionActive = false;
                return;
            }

            Id = sessionInfo.Id;
            Name = sessionInfo.Name;
            MaxUsers = sessionInfo.MaximumUsers;
            PasswordProtected = sessionInfo.PasswordProtected;
            Users.Clear();
            Messages.Clear();
            if (users != null && users.Length > 0)
            {
                foreach (UserInfo user in users)
                    Users.Add(user);
            }

            IsSessionActive = true;
            NebulaClient.Navigate(typeof(SharedSessionPage), null, new DrillInNavigationTransitionInfo());
        }

        public void Leave()
        {
            if (!IsSessionActive)
                return;
            NebulaClient.Network.SendPacket(new SharedSessionLeavePacket());
            IsSessionActive = false;
            NebulaClient.Navigate(typeof(SharedSessionsPage));
        }

        public UserInfo FindUser(Predicate<UserInfo> predicate)
        {
            foreach (UserInfo userInfo in Users)
            {
                if (predicate(userInfo))
                    return userInfo;
            }

            return UserInfo.Empty;
        }

        public UserInfo FindUserById(int id)
        {
            return FindUser(info => info.Id == id);
        }

        public bool IsUserPresent(UserInfo userInfo)
        {
            return FindUserById(userInfo.Id) != UserInfo.Empty;
        }

        public void AddMessage(SharedSessionMessage message)
        {
            if (!IsSessionActive)
                return;
            Messages.Add(message);
            
        }

        public void ClearMessages()
        {
            if (!IsSessionActive)
                return;
            Messages.Clear();
        }

        private void OnReceiveSharedSessionJoinResponse(SharedSessionJoinResponse response, NetPeer peer)
        {
            NebulaClient.BeginInvoke(async () =>
            {
                switch (response.ResponseCode)
                {
                    case 0:
                        SetSession(response.Session, response.Users);
                        break;
                    case 10:
                        await NebulaMessageBox.ShowOk("SharedSessionCantJoin", "Session does not exists");
                        break;
                    case 11:
                        break;
                    case 12:
                        await NebulaMessageBox.ShowOk("SharedSessionCantJoin", "SharedSessionWrongPassword");
                        break;
                }
            });
        }

        private void OnReceiveSharedSessionUserJoin(SharedSessionUserJoinedPacket response, NetPeer peer)
        {
            if (!IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() =>
            {
                if (IsUserPresent(response.User))
                    return;
                Users.Add(response.User);
                AddMessage(new SharedSessionMessage(FindUserById(response.User.Id), NebulaClient.GetLocString("SharedSessionMessageUserJoined"), "#10b513"));
            });
        }

        private void OnReceiveSharedSessionUserLeft(SharedSessionUserLeftPacket response, NetPeer peer)
        {
            if (!IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() =>
            {
                UserInfo listUser = FindUserById(response.User.Id);
                if (listUser == UserInfo.Empty)
                    return;
                Users.Remove(listUser);
                AddMessage(new SharedSessionMessage(listUser, NebulaClient.GetLocString("SharedSessionMessageUserLeft"), "#fc0000"));
            });
        }

        private void OnReceiveSessionUserMessagePacket(SharedSessionUserMessagePacket packet, NetPeer peer)
        {
            if (!IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() => AddMessage(new SharedSessionMessage(FindUserById(packet.User.Id), packet.Message)));
        }
    }
}