using Nebula.Server.Users;

namespace Nebula.Server.Commands
{
    public interface ICommand
    {
        public string CommandPrefix { get; }
        public string Usage         { get; }
        public int    MinimumArgs   { get; }

        bool CanUse(NebulaUser user);
        void Execute(NebulaUser user, params string[] args);
    }
}