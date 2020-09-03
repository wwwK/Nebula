using System;
using System.Collections.Generic;
using Nebula.Server.Commands;
using Nebula.Server.Extensions;
using Nebula.Server.Users;
using Nebula.Shared.Packets.S2C;

namespace Nebula.Server
{
    public class CommandsManager
    {
        public CommandsManager()
        {
        }

        private Dictionary<string, ICommand> Commands { get; } = new Dictionary<string, ICommand>();

        public bool RegisterCommand(ICommand command)
        {
            string commandPrefix = command.CommandPrefix.ToLower();
            if (Commands.ContainsKey(commandPrefix))
                return false;
            Commands.Add(commandPrefix, command);
            return true;
        }

        public void ExecuteCommands(NebulaUser user, params string[] args)
        {
            if (args == null || args.Length < 1)
                return;
            string commandPrefix = args[0];
            if (Commands.TryGetValue(commandPrefix, out ICommand command) && command.CanUse(user))
            {
                if (args.Length == 1 && command.MinimumArgs == 0)
                    command.Execute(user);
                else
                {
                    int argsLenght = args.Length - 1;
                    if (argsLenght < command.MinimumArgs)
                    {
                        SendCommandUsage(user, command);
                        return;
                    }

                    string[] commandArgs = new string[args.Length - 1];
                    CopyArray(args, commandArgs, 1);
                    command.Execute(user, commandArgs);
                }
            }
        }

        public void SendCommandUsage(NebulaUser user, ICommand command)
        {
            if (user.IsServer())
                NebulaServer.WriteLine($"---- Command Usage '{command.CommandPrefix}': {command.Usage}", ConsoleColor.Green);
            else
                NebulaServer.SendPacket(new CommandUsagePacket {CommandPrefix = command.CommandPrefix, CommandUsage = command.Usage}, user.Peer);
        }

        private void CopyArray(string[] source, string[] destination, int startIndex)
        {
            if (startIndex > source.Length || startIndex < 0)
                return;
            int index = startIndex;
            for (int i = 0; i < destination.Length; i++)
            {
                if (i > source.Length - 1)
                    break;
                destination[i] = source[index++];
            }
        }
    }
}