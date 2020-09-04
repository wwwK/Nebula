using System;
using System.Text;
using Nebula.Server.Commands;
using Nebula.Server.Extensions;
using Nebula.Server.Users;
using static Nebula.Server.ServerApp;

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
            foreach (SharedSessionRoom room in ServerApp.Server.SharedSessionsManager.GetRooms())
            {
                builder.AppendLine($"\t\t - {room.Id}");
                builder.AppendLine($"\t\t\t - Owner: {room.Owner?.Username ?? "NULL"}");
                builder.AppendLine($"\t\t\t - Name: {room.Name}");
                builder.AppendLine($"\t\t\t - Users: {room.UsersCount}/{room.MaximumUsers}");
                builder.AppendLine($"\t\t\t - Protected: {room.PasswordProtected}");
                count++;
            }

            WriteLine(builder.ToString(), ConsoleColor.Green);
            WriteLine($"--- Total Rooms: {count}", ConsoleColor.Green);
        }
    }
}