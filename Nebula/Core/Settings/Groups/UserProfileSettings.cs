using System;

namespace Nebula.Core.Settings.Groups
{
    public class UserProfileSettings : ISettingsGroup
    {
        private string _username = "NebulaUser";
        private string _avatar   = "";

        public UserProfileSettings()
        {
        }

        public string GroupName { get; } = "UserProfile";

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

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

        public event EventHandler SettingsChanged;
    }
}