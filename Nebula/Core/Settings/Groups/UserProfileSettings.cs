using System;
using System.Windows.Controls;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.Settings.Groups
{
    public class UserProfileSettings : SimplePanelDataContent, ISettingsGroup
    {
        private static readonly DataContentCache Cache = DataContentCache.BuildCache<UserProfileSettings>();

        private string _username = "NebulaUser";
        private string _avatar   = "";

        public UserProfileSettings()
        {
        }

        public string             GroupName { get; } = "UserProfile";
        public event EventHandler SettingsChanged;

        [DataProperty(typeof(TextBox), "Username")]
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        [DataProperty(typeof(TextBox), "ProfileAvatar")]
        public string Avatar
        {
            get => _avatar;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.StartsWith("http"))
                    return;
                _avatar = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public override DataContentCache GetCache()
        {
            return Cache;
        }

        public void OnSettingsLoaded()
        {
            Cache.PrepareFor(this);
        }
    }
}