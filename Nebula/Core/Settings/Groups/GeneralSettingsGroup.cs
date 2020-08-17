using System;
using System.Xml;
using EasySharp.Reflection;
using EasySharp.Reflection.Fast;
using Nebula.Core.Extensions;

namespace Nebula.Core.Settings.Groups
{
    public class GeneralSettingsGroup : ISettingsGroup
    {
        private readonly FastPropertyInfo<GeneralSettingsGroup, bool> _fastMediaKeyEnabled =
            FastAccessor.GetProperty<GeneralSettingsGroup, bool>("MediaKeyEnabled");

        private int  _mediaKeySoundIncDecValue = 5;
        private int  _defaultSoundLevel        = 50;
        private bool _mediaKeyEnabled          = true;

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

        public event EventHandler SettingsChanged;

        public void Save(XmlElement element)
        {
            element.SetAttribute(nameof(DefaultSoundLevel), DefaultSoundLevel.ToString());
            element.SetAttribute(nameof(MediaKeyEnabled), MediaKeyEnabled.ToString());
            element.SetAttribute(nameof(MediaKeySoundIncDecValue), MediaKeySoundIncDecValue.ToString());
        }

        public void Load(XmlElement element)
        {
            DefaultSoundLevel = element.GetIntAttribute(nameof(DefaultSoundLevel), 50);
            MediaKeyEnabled = element.GetBoolAttribute(nameof(MediaKeyEnabled), true);
            MediaKeySoundIncDecValue = element.GetIntAttribute(nameof(MediaKeySoundIncDecValue), 5);
        }
    }
}