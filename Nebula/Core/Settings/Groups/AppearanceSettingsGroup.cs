using System;

namespace Nebula.Core.Settings.Groups
{
    public class AppearanceSettingsGroup : ISettingsGroup
    {
        private string _backgroundImage        = "";
        private string _backgroundImageStretch = "UniformToFill";

        public AppearanceSettingsGroup()
        {
        }

        public string GroupName { get; } = "Appearance";

        public string BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                _backgroundImage = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public string BackgroundImageStretch
        {
            get => _backgroundImageStretch;
            set
            {
                _backgroundImageStretch = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler SettingsChanged;
    }
}