using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nebula.Net.Packets.S2C;
using Nebula.Server.Commands;
using Nebula.Server.Extensions;
using Nebula.Server.Users;
using static Nebula.Server.ServerApp;

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

                    string[] commandArgs = new string[argsLenght];
                    CopyArray(args, commandArgs, 1);
                    command.Execute(user, commandArgs);
                }
            }
        }

        public void SendCommandUsage(NebulaUser user, ICommand command)
        {
            if (user.IsServer())
                WriteLine($"---- Command Usage '{command.CommandPrefix}': {command.Usage}", ConsoleColor.Green);
            else
                ServerApp.Server.SendPacket(new CommandUsagePacket {CommandPrefix = command.CommandPrefix, CommandUsage = command.Usage}, user.Peer);
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

        public static void HandleConsoleCommands()
        {
            if (!IsUnix)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Server > ");
            }

            string value = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(value))
                ServerApp.Server.CommandsManager.ExecuteCommands(ServerApp.Server.ServerUser, value.Replace("Server > ", "").SplitWithoutQuotes());
            HandleConsoleCommands();
        }
    }
}