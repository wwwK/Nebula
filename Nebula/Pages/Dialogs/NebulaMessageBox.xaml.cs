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
            PrimaryButtonText = primaryKey == null
                ? NebulaClient.GetLocString("ButtonYes")
                : NebulaClient.GetLocString(primaryKey);
            SecondaryButtonText = secondaryKey == null
                ? null
                : NebulaClient.GetLocString(secondaryKey);
            CloseButtonText = cancelKey == null
                ? NebulaClient.GetLocString("ButtonNo")
                : NebulaClient.GetLocString(cancelKey);
        }

        public static async Task<ContentDialogResult> ShowYesNo(string titleKey, string text, params object[] format)
        {
            return await Show(titleKey, text, null, null, null, format);
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