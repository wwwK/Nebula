using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.Settings;
using Nebula.Core.Settings.Extentions;
using Nebula.Core.Settings.Groups;
using YoutubeExplode.Playlists;

namespace Nebula.Core.Settings
{
    public class NebulaSettings
    {
        private const string SettingsFolderName = "Nebula";
        private const string SettingsFileName   = "NebulaSettings.xml";

        public NebulaSettings()
        {
            SettingsDirectory =
                new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    SettingsFolderName));
            SettingsFileInfo = new FileInfo(Path.Combine(SettingsDirectory.FullName, SettingsFileName));
            LoadSettings();
            General.SettingsChanged += OnSettingsChanged;
            Privacy.SettingsChanged += OnSettingsChanged;
            Appearance.SettingsChanged += OnSettingsChanged;
        }

        public DirectoryInfo           SettingsDirectory { get; }
        public FileInfo                SettingsFileInfo  { get; }
        public GeneralSettingsGroup    General           { get; }      = new GeneralSettingsGroup();
        public PrivacySettingsGroup    Privacy           { get; }      = new PrivacySettingsGroup();
        public AppearanceSettingsGroup Appearance        { get; }      = new AppearanceSettingsGroup();
        public bool                    AutoSave          { get; set; } = false;

        private void LoadSettings()
        {
            SettingsDirectory.Create();
            if (!File.Exists(SettingsFileInfo.FullName))
                SaveSettings();
            else
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(SettingsFileInfo.FullName);
                foreach (XmlElement element in xmlDocument.DocumentElement.ChildNodes)
                {
                    if (element.Name == General.GroupName)
                        General.Load(element);
                    else if (element.Name == Privacy.GroupName)
                        Privacy.Load(element);
                    else if (element.Name == Appearance.GroupName)
                        Appearance.Load(element);
                }
            }
        }

        public void SaveSettings()
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement rootElement = xmlDocument.CreateElement(nameof(NebulaSettings));
            xmlDocument.AppendChild(rootElement);

            XmlElement generalElement = xmlDocument.CreateElement(General.GroupName);
            XmlElement privacyElement = xmlDocument.CreateElement(Privacy.GroupName);
            XmlElement appearanceElement = xmlDocument.CreateElement(Appearance.GroupName);

            General.Save(generalElement);
            Privacy.Save(privacyElement);
            Appearance.Save(appearanceElement);

            rootElement.AppendChild(generalElement);
            rootElement.AppendChild(privacyElement);
            rootElement.AppendChild(appearanceElement);

            xmlDocument.Save(SettingsFileInfo.FullName);
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            if (AutoSave)
                SaveSettings();

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
                if (Appearance.DisplayMode != mainWindow.NavView.DisplayMode.ToString())
                    mainWindow.SetViewMode(Appearance.DisplayMode);
                mainWindow.BackgroundBrush = Appearance.GetBackgroundImageBrush();
            }
            else if (sender == Privacy)
            {
            }
        }

        /*

        private string ToBase64(BitmapImage bitmapImage)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using MemoryStream memStream = new MemoryStream();
            encoder.Save(memStream);
            return Convert.ToBase64String(memStream.ToArray());
        }

        private BitmapImage FromBase64(string base64)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64);
            MemoryStream memoryStream = new MemoryStream(byteBuffer) {Position = 0};
            BitmapImage bitmapImage = new BitmapImage {StreamSource = memoryStream};
            memoryStream.Close();
            return bitmapImage;
        } */
    }
}