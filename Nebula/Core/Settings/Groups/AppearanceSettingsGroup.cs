using System;
using System.Xml;
using Nebula.Core.Extensions;

namespace Nebula.Core.Settings.Groups
{
    public class AppearanceSettingsGroup : ISettingsGroup
    {
        private string _displayMode      = "Left";

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

        public event EventHandler SettingsChanged;

        public void Save(XmlElement element)
        {
            element.SetAttribute(nameof(DisplayMode), DisplayMode);
        }

        public void Load(XmlElement element)
        {
            DisplayMode = element.GetStringAttribute(nameof(DisplayMode), "Left");
        }
    }
}