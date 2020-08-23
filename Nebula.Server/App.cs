using System;
using Nebula.Server.Lobby;

namespace Nebula.Server
{
    public static class App
    {
        private static void Main(string[] args)
        {
            NebulaServer = new NebulaServer();
            LobbiesManager.Init();
        }

        public static NebulaServer NebulaServer { get; private set; }

        public static void ReadLine()
        {
            string value = Console.ReadLine();
            if (value == "stop")
            {
            }
        }
    }
}