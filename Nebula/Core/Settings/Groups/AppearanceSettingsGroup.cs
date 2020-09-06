using System;
using System.Windows.Controls;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.Settings.Groups
{
    public class AppearanceSettingsGroup : ISettingsGroup
    {
        private static readonly DataContentCache Cache = DataContentCache.BuildCache<AppearanceSettingsGroup>();

        private string _backgroundImage        = "";
        private string _backgroundImageStretch = "UniformToFill";

        public AppearanceSettingsGroup()
        {
        }

        public string             GroupName { get; } = "Appearance";
        public event EventHandler SettingsChanged;

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
    }
}