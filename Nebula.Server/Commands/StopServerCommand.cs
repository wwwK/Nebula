using System;
using Nebula.Server.Extensions;
using Nebula.Server.Users;
using static Nebula.Server.NebulaServer;

namespace Nebula.Server.Commands
{
    public class StopServerCommand : ICommand
    {
        public string CommandPrefix { get; } = "stop";
        public string Usage         { get; } = "stop";
        public int    MinimumArgs   { get; } = 0;

        public bool CanUse(NebulaUser user)
        {
            return user.IsServer();
        }

        public void Execute(NebulaUser user, params string[] args)
        {
            ServerApp.Server.StopServer();
        }
    }
}