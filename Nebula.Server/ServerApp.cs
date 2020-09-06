using System;
using System.Threading.Tasks;

namespace Nebula.Server
{
    public static class ServerApp
    {
        public static NebulaServer Server { get; private set; }
        public static bool         IsUnix { get; private set; }

        private static void Main(string[] args)
        {
            IsUnix = Environment.OSVersion.Platform == PlatformID.Unix;
            Server = new NebulaServer();
            Server.StartServer();
            CommandsManager.HandleConsoleCommands();
        }

        public static void WriteLine(string content, ConsoleColor color = ConsoleColor.White)
        {
            if (!IsUnix)
                Console.SetCursorPosition(0, Console.CursorTop);
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {content}");
            Console.ResetColor();
            if (!IsUnix)
                Console.Write("Server > ");
        }

        public static void Clear()
        {
            Console.Clear();
        }
    }
}