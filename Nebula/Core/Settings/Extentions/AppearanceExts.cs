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

        public static ImageBrush GetBackgroundImageBrush(this AppearanceSettingsGroup settingsGroup)
        {
            try
            {
                Uri uri = settingsGroup.GetBackgroundImageUri();
                if (uri == null || uri.ToString().StartsWith("file") && !File.Exists(settingsGroup.BackgroundImage))
                    return null;
                ImageBrush imageBrush = new ImageBrush(new BitmapImage(uri));
                imageBrush.Stretch = Enum.TryParse(settingsGroup.BackgroundImageStretch, out Stretch stretch)
                    ? stretch
                    : Stretch.UniformToFill;
                return imageBrush;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}