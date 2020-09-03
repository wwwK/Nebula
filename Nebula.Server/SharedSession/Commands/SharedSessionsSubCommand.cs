using Nebula.Server.Commands;
using Nebula.Server.Extensions;
using Nebula.Server.Users;

namespace Nebula.Server.SharedSession.Commands
{
    public class SharedSessionsSubCommand : SubCommand
    {
        public SharedSessionsSubCommand()
        {
            SubManager.RegisterCommand(new SharedSessionCreateRoomCommand());
            SubManager.RegisterCommand(new SharedSessionListRoomsCommand());
        }

        public override string CommandPrefix { get; } = "ss";
        public override string Usage         { get; } = "ss <create:list:del>";
        public override int    MinimumArgs   { get; } = 1;

        public override bool CanUse(NebulaUser user)
        {
            return user.IsServer();
        }
    }
}