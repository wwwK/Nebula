using System;

namespace Nebula.Server
{
    public static class ServerApp
    {
        public static NebulaServer Server { get; private set; }

        private static void Main(string[] args)
        {
            Server = new NebulaServer();
            Server.StartServer();
            Server.CommandsManager.HandleConsoleCommands();
        }

        public static void WriteLine(string content, ConsoleColor color = ConsoleColor.White)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {content}");
            Console.ResetColor();
            Console.Write("Server > ");
        }

        public static void Clear()
        {
            Console.Clear();
        }
    }
}