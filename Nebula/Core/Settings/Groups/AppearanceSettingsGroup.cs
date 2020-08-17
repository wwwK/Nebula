using System;
using System.Xml;

namespace Nebula.Core.Settings.Groups
{
    public class AppearanceSettingsGroup : ISettingsGroup
    {
        public string GroupName { get; } = "Appearance";

        public event EventHandler SettingsChanged;

        public void Save(XmlElement document)
        {

        }

        public void Load(XmlElement document)
        {
        }
    }
}