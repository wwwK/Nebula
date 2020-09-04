using System;
using Nebula.Server.Commands;
using Nebula.Server.Extensions;
using Nebula.Server.Users;
using static Nebula.Server.ServerApp;

namespace Nebula.Server.SharedSession.Commands
{
    public class SharedSessionCreateRoomCommand : ICommand
    {
        public string CommandPrefix { get; } = "create";
        public string Usage         { get; } = "ss create <RoomName> <RoomMaxUsers> optional:<RoomPassword>";
        public int    MinimumArgs   { get; } = 2;

        public bool CanUse(NebulaUser user)
        {
            return user.IsServer();
        }

        public void Execute(NebulaUser user, params string[] args)
        {
            string roomName = args[0];
            int maxUsers = int.Parse(args[1]);
            string roomPassword = "";
            if (args.Length > 2)
                roomPassword = args[2];
            if (ServerApp.Server.SharedSessionsManager.CreateRoom(new SharedSessionRoom(ServerApp.Server.ServerUser, roomName, roomPassword, "", maxUsers)))
                WriteLine($"Successfully created shared session room '{roomName}'", ConsoleColor.Green);
            else
                WriteLine($"Failed to create created shared session room '{roomName}'", ConsoleColor.Red);
        }
    }
}