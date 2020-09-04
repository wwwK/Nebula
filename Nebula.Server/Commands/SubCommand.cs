using Nebula.Server.Users;

namespace Nebula.Server.Commands
{
    public abstract class SubCommand : ICommand
    {
        protected       CommandsManager SubManager    { get; } = new CommandsManager();
        public abstract string          CommandPrefix { get; }
        public abstract string          Usage         { get; }
        public abstract int             MinimumArgs   { get; }
        public abstract bool CanUse(NebulaUser user);

        public void Execute(NebulaUser user, params string[] args)
        {
            SubManager.ExecuteCommands(user, args);
        }
    }
}