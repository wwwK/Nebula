using System;
using LiteNetLib;
using Nebula.Server.Users;

namespace Nebula.Server.Events
{
    public class UserDisconnectedEventArgs : EventArgs
    {
        public UserDisconnectedEventArgs(NebulaUser user, DisconnectInfo disconnectInfo)
        {
            User = user;
            DisconnectInfo = disconnectInfo;
        }

        public NebulaUser     User           { get; }
        public DisconnectInfo DisconnectInfo { get; }
    }
}