using System;

namespace Nebula.Core.Settings.Groups
{
    public class UserProfileSettings : ISettingsGroup
    {
        private string _username = "NebulaUser";

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

        public event EventHandler SettingsChanged;
    }
}