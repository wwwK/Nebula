using System;
using Nebula.Server.Extensions;
using Nebula.Server.Users;
using static Nebula.Server.NebulaServer;

namespace Nebula.Server.Commands
{
    public class ClearConsoleCommand : ICommand
    {
        public string CommandPrefix { get; } = "clear";
        public string Usage         { get; } = "clear";
        public int    MinimumArgs   { get; } = 0;

        public bool CanUse(NebulaUser user)
        {
            return user.IsServer();
        }

        public void Execute(NebulaUser user, params string[] args)
        {
            ServerApp.Clear();
        }
    }
}