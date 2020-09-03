using System;
using Nebula.Server.Commands;
using Nebula.Server.Extensions;
using Nebula.Server.Users;

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
            if (NebulaServer.SharedSessionsManager.CreateRoom(new SharedSessionRoom(NebulaServer.ServerUser, roomName, roomPassword, "", maxUsers)))
                NebulaServer.WriteLine($"Successfully created shared session room '{roomName}'", ConsoleColor.Green);
            else
                NebulaServer.WriteLine($"Failed to create created shared session room '{roomName}'", ConsoleColor.Red);
        }
    }
}