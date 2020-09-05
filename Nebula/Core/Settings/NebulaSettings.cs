using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;
using Nebula.Core.Settings.Extentions;
using Nebula.Core.Settings.Groups;
using Nebula.UI;

#pragma warning disable 4014

namespace Nebula.Core.Settings
{
    public class NebulaSettings
    {
        private const string SettingsFileName = "NebulaSettings.json";

        public static readonly DirectoryInfo SettingsDirectory =
            new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Nebula"));

        public NebulaSettings()
        {
            if (!SettingsDirectory.Exists)
                SettingsDirectory.Create();
        }

        [JsonIgnore] public bool                    AutoSave    { get; set; } = false;
        public              GeneralSettingsGroup    General     { get; set; } = new GeneralSettingsGroup();
        public              PrivacySettingsGroup    Privacy     { get; set; } = new PrivacySettingsGroup();
        public              AppearanceSettingsGroup Appearance  { get; set; } = new AppearanceSettingsGroup();
        public              UserProfileSettings     UserProfile { get; set; } = new UserProfileSettings();

        public void OnSettingsLoaded()
        {
            General.SettingsChanged += OnSettingsChanged;
            Privacy.SettingsChanged += OnSettingsChanged;
            Appearance.SettingsChanged += OnSettingsChanged;
            UserProfile.SettingsChanged += OnSettingsChanged;
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (AutoSave)
                SaveSettingsAsync();

            if (sender == General)
            {
                if (General.MediaKeyEnabled && !NebulaClient.KeyboardHooker.IsHooked)
                    NebulaClient.KeyboardHooker.Hook();
                else if (!General.MediaKeyEnabled && NebulaClient.KeyboardHooker.IsHooked)
                    NebulaClient.KeyboardHooker.UnHook();
            }
            else if (sender == Appearance)
            {
                MainWindow mainWindow = NebulaClient.MainWindow;
                mainWindow.BackgroundWallpaper = Appearance.GetBackgroundImageSource();
                mainWindow.ImageBackground.Stretch = Appearance.GetBackgroundImageStretch();
            }
            else if (sender == Privacy)
            {
            }
            else if (sender == UserProfile)
            {
            }
        }

        /// <summary>
        /// Load Settings from file
        /// </summary>
        /// <returns><see cref="NebulaSettings"/></returns>
        public static NebulaSettings LoadSettings()
        {
            string file = Path.Combine(SettingsDirectory.FullName, SettingsFileName);
            if (!File.Exists(file))
                Task.Run(async () => await SaveSettingsAsync(new NebulaSettings())).Wait();
            NebulaSettings settings = JsonSerializer.Deserialize<NebulaSettings>(File.ReadAllText(Path.Combine(SettingsDirectory.FullName, SettingsFileName)),
                new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    WriteIndented = true,
                    IgnoreReadOnlyProperties = true
                });
            settings.OnSettingsLoaded();
            return settings;
        }

        /// <summary>
        /// Save <see cref="NebulaSettings"/> to file
        /// </summary>
        public static async Task SaveSettingsAsync(NebulaSettings settings = null)
        {
            string file = Path.Combine(SettingsDirectory.FullName, SettingsFileName);
            if (File.Exists(file))
                File.Delete(file);
            await using FileStream fs = File.OpenWrite(file);
            await JsonSerializer.SerializeAsync(fs, settings ?? NebulaClient.Settings, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = true,
                IgnoreReadOnlyProperties = true
            });
        }
    }
}