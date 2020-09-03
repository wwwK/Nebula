using System;
using System.Text;
using Nebula.Server.Commands;
using Nebula.Server.Extensions;
using Nebula.Server.Users;

namespace Nebula.Server.SharedSession.Commands
{
    public class SharedSessionListRoomsCommand : ICommand
    {
        public string CommandPrefix { get; } = "list";
        public string Usage         { get; } = "ss list";
        public int    MinimumArgs   { get; } = 0;

        public bool CanUse(NebulaUser user)
        {
            return user.IsServer();
        }

        public void Execute(NebulaUser user, params string[] args)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("--- Shared Sessions List :");
            int count = 0;
            foreach (SharedSessionRoom room in NebulaServer.SharedSessionsManager.GetRooms())
            {
                builder.AppendLine($"\t - {room.Id}");
                builder.AppendLine($"\t\t Name: {room.Name}");
                builder.AppendLine($"\t\t Users: {room.UsersCount}/{room.MaxUsers}");
                builder.AppendLine($"\t\t Protected: {room.PasswordProtected}");
                count++;
            }

            builder.AppendLine($"--- Total Rooms: {count}");

            NebulaServer.WriteLine(builder.ToString(), ConsoleColor.Green);
        }
    }
}