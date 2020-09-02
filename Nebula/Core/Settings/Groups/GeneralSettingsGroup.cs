using System;
using System.Text.Json.Serialization;
using System.Xml;
using EasySharp.Reflection;
using EasySharp.Reflection.Fast;
using Nebula.Core.Extensions;

namespace Nebula.Core.Settings.Groups
{
    public class GeneralSettingsGroup : ISettingsGroup
    {
        private int    _searchMaxPages            = 1;
        private int    _mediaKeySoundIncDecValue  = 5;
        private int    _defaultSoundLevel         = 50;
        private int    _playlistMaxMediasPerPages = 25;
        private int    _serverPort                = 9080;
        private bool   _mediaKeyEnabled           = true;
        private string _serverIp                  = "127.0.0.1";

        public GeneralSettingsGroup()
        {
        }

        public string GroupName { get; } = "General";

        public bool MediaKeyEnabled
        {
            get => _mediaKeyEnabled;
            set
            {
                _mediaKeyEnabled = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public int MediaKeySoundIncDecValue
        {
            get => _mediaKeySoundIncDecValue;
            set
            {
                _mediaKeySoundIncDecValue = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public int DefaultSoundLevel
        {
            get => _defaultSoundLevel;
            set
            {
                _defaultSoundLevel = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public int SearchMaxPages
        {
            get => _searchMaxPages;
            set
            {
                _searchMaxPages = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public int PlaylistMaxMediasPerPage
        {
            get => _playlistMaxMediasPerPages;
            set
            {
                _playlistMaxMediasPerPages = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public string ServerIp
        {
            get => _serverIp;
            set
            {
                _serverIp = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public int ServerPort
        {
            get => _serverPort;
            set
            {
                _serverPort = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler SettingsChanged;
    }
}