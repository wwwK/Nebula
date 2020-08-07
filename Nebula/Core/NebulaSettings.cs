using System;

namespace Nebula.Core
{
    public class NebulaSettings
    {
        private const string SettingsFolderName = "Nebula";
        private const string SettingsFileName   = "NebulaSettings.json";

        public NebulaSettings()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
    }
}