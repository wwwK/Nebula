using System.Threading.Tasks;
using ModernWpf.Controls;
using Nebula.Core;


namespace Nebula.Pages.Dialogs
{
    public partial class NebulaMessageBox : ContentDialog
    {
        public NebulaMessageBox(string titleKey, string text, string primaryKey = null, string secondaryKey = null,
                                string cancelKey = null)
        {
            InitializeComponent();
            Title = NebulaClient.GetLocString(titleKey);
            MessageBoxText.Text = text;
            if (!string.IsNullOrWhiteSpace(primaryKey))
                PrimaryButtonText = NebulaClient.GetLocString(primaryKey);
            if (!string.IsNullOrWhiteSpace(secondaryKey))
                SecondaryButtonText = NebulaClient.GetLocString(secondaryKey);
            if (!string.IsNullOrWhiteSpace(cancelKey))
                CloseButtonText = NebulaClient.GetLocString(cancelKey);
        }

        public static async Task<ContentDialogResult> ShowYesNo(string titleKey, string text, params object[] format)
        {
            return await Show(titleKey, text, "ButtonYes", null, "ButtonNo", format);
        }

        public static async Task<ContentDialogResult> ShowOk(string titleKey, string text, params object[] format)
        {
            return await Show(titleKey, text, null, null, "ButtonOk", format);
        }

        public static async Task<ContentDialogResult> Show(string titleKey, string text, string primary = null,
                                                           string secondary = null,
                                                           string cancel = null, params object[] format)
        {
            string msg = NebulaClient.GetLocString(text);
            if (format != null && format.Length > 0)
                msg = string.Format(msg, format);
            NebulaMessageBox messageBox = new NebulaMessageBox(titleKey, msg, primary, secondary, cancel);
            return await messageBox.ShowAsync();
        }
    }
}