using System;
using System.IO;
using Nebula.Core.Medias.Playlist;

namespace Nebula.Core
{
    public class NebulaSettings
    {
        public const string SettingsFolderName = "Nebula";
        public const string SettingsFileName   = "NebulaSettings.json";

        public NebulaSettings()
        {
            SettingsDirectory =
                new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    SettingsFolderName));
            LoadSettings();
        }

        public DirectoryInfo SettingsDirectory { get; }

        private void LoadSettings()
        {
            if (!SettingsDirectory.Exists)
                SettingsDirectory.Create();
        }

        public void SavePlaylist(IPlaylist playlist)
        {
            
        }
    }
}