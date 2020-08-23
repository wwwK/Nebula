using System;
using Nebula.Shared;

namespace Nebula.Server.Lobby
{
    public class ServerLobby : INebulaLobby
    {
        private string _name;
        private string _thumbnailUrl;
        
        public ServerLobby(Guid guid, string name, string thumbnailUrl)
        {
            Guid = guid.ToString();
            Name = name;
            ThumbnailUrl = thumbnailUrl;
        }

        public string Guid         { get; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
            }
        }

        public string ThumbnailUrl
        {
            get => _thumbnailUrl;
            set
            {
                _thumbnailUrl = value;
            }
        }
    }
}