using System;
using System.Windows.Media;
using System.Xml;
using Nebula.Core.Extensions;

namespace Nebula.Core.Settings.Groups
{
    public class AppearanceSettingsGroup : ISettingsGroup
    {
        private string _displayMode            = "Top";
        private string _backgroundImage        = "";
        private string _backgroundImageStretch = "UniformToFill";

        public AppearanceSettingsGroup()
        {
        }

        public string GroupName { get; } = "Appearance";

        public string DisplayMode
        {
            get => _displayMode;
            set
            {
                _displayMode = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

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

        public void Save(XmlElement element)
        {
            element.SetAttribute(nameof(DisplayMode), DisplayMode);
            element.SetAttribute(nameof(BackgroundImage), BackgroundImage);
            element.SetAttribute(nameof(BackgroundImageStretch), BackgroundImageStretch);
        }

        public void Load(XmlElement element)
        {
            DisplayMode = element.GetStringAttribute(nameof(DisplayMode), "Left");
            BackgroundImage = element.GetStringAttribute(nameof(BackgroundImage), "");
            BackgroundImageStretch = element.GetStringAttribute(nameof(BackgroundImageStretch), "UniformToFill");
        }
    }
}