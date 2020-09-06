using System;
using System.Windows.Controls;
using ModernWpf.Controls;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.Settings.Groups
{
    public class GeneralSettingsGroup : SimplePanelDataContent, ISettingsGroup
    {
        private static readonly DataContentCache Cache = DataContentCache.BuildCache<GeneralSettingsGroup>();

        private int  _searchMaxPages            = 1;
        private int  _mediaKeySoundIncDecValue  = 5;
        private int  _defaultSoundLevel         = 50;
        private int  _playlistMaxMediasPerPages = 25;
        private bool _mediaKeyEnabled           = true;

        public GeneralSettingsGroup()
        {
        }

        public string             GroupName { get; } = "General";
        public event EventHandler SettingsChanged;

        [DataProperty(typeof(NumberBox), "SettingsDefaultSoundLevel", args: new object[] {1, 0, 100})]
        public int DefaultSoundLevel
        {
            get => _defaultSoundLevel;
            set
            {
                _defaultSoundLevel = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        [DataCategory("Search")]
        [DataProperty(typeof(NumberBox), "SettingsSearchMaxPages", args: new object[] {1, 1, 50})]
        public int SearchMaxPages
        {
            get => _searchMaxPages;
            set
            {
                _searchMaxPages = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        [DataCategory("Playlists")]
        [DataProperty(typeof(NumberBox), "SettingsMaxElementsPerPage", args: new object[] {1, 1, 100})]
        public int PlaylistMaxMediasPerPage
        {
            get => _playlistMaxMediasPerPages;
            set
            {
                _playlistMaxMediasPerPages = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        [DataCategory("Keyboard")]
        [DataProperty(typeof(CheckBox), "SettingsKeyboardMediaEnable")]
        public bool MediaKeyEnabled
        {
            get => _mediaKeyEnabled;
            set
            {
                _mediaKeyEnabled = value;
                SettingsChanged?.Invoke(this, new EventArgs());
            }
        }

        [DataProperty(typeof(NumberBox), "SettingsKeyboardMediaSoundIncDecValue", args: new object[] {1, 1, 100})]
        public int MediaKeySoundIncDecValue
        {
            get => _mediaKeySoundIncDecValue;
            set
            {
                _mediaKeySoundIncDecValue = value;
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