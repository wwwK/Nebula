using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Nebula.Core.Settings.Groups;

namespace Nebula.Core.Settings.Extentions
{
    public static class AppearanceExts
    {
        public static Uri GetBackgroundImageUri(this AppearanceSettingsGroup settingsGroup)
        {
            return string.IsNullOrWhiteSpace(settingsGroup.BackgroundImage)
                ? null
                : new Uri(settingsGroup.BackgroundImage);
        }

        public static ImageSource GetBackgroundImageSource(this AppearanceSettingsGroup settingsGroup)
        {
            try
            {
                Uri uri = settingsGroup.GetBackgroundImageUri();
                if (uri == null || uri.ToString().StartsWith("file") && !File.Exists(settingsGroup.BackgroundImage))
                    return null;
                return new BitmapImage(uri);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Stretch GetBackgroundImageStretch(this AppearanceSettingsGroup settingsGroup)
        {
            return Enum.TryParse(settingsGroup.BackgroundImageStretch, out Stretch stretch) ? stretch : Stretch.UniformToFill;
        }
    }
}