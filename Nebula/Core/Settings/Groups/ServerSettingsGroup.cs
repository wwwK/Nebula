using System;
using System.Windows.Controls;
using ModernWpf.Controls;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.Settings.Groups
{
    public class ServerSettingsGroup : SimplePanelDataContent, ISettingsGroup
    {
        private static readonly DataContentCache Cache = DataContentCache.BuildCache<ServerSettingsGroup>();
        private static          bool             CacheInitialized { get; set; }

        private int    _serverPort          = 9080;
        private bool   _connectCustomServer = false;
        private string _serverIp            = "127.0.0.1";
        private string _serverConnKey       = "";

        public ServerSettingsGroup()
        {
        }

        public string             GroupName { get; } = "Server";
        public event EventHandler SettingsChanged;

        [DataProperty(typeof(CheckBox), "SettingsServerConnectPrivate")]
        public bool ConnectToCustomServer
        {
            get => _connectCustomServer;
            set
            {
                _connectCustomServer = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        [DataProperty(typeof(TextBox), "SettingsServerIp")]
        public string ServerIp
        {
            get => _serverIp;
            set
            {
                _serverIp = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        [DataProperty(typeof(NumberBox), "SettingsServerPort")]
        public int ServerPort
        {
            get => _serverPort;
            set
            {
                _serverPort = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        [DataProperty(typeof(TextBox), "SettingsServerConnectionKey")]
        public string ServerConnectionKey
        {
            get => _serverConnKey;
            set
            {
                _serverConnKey = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public void OnSettingsLoaded()
        {
            Cache.PrepareFor(this);
        }

        public override DataContentCache GetCache()
        {
            return Cache;
        }
    }
}