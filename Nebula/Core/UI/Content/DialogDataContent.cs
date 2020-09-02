using System.Threading.Tasks;
using System.Windows.Input;
using ModernWpf.Controls;
using Nebula.UI.Dialogs;

namespace Nebula.Core.UI.Content
{
    public class DialogDataContent : SimplePanelDataContent
    {
        public string Title               { get; set; }
        public string PrimaryButtonText   { get; set; }
        public string SecondaryButtonText { get; set; }
        public string CloseButtonText     { get; set; } = NebulaClient.GetLocString("Cancel");

        public virtual void OnPrimaryClick()
        {
        }

        public virtual void OnSecondaryClick()
        {
        }

        public virtual void OnCloseClick()
        {
        }

        public async Task<ContentDialogResult> ShowDialogAsync()
        {
            NebulaDialog dialog = new NebulaDialog(this);
            return await dialog.ShowAsync();
        }
    }
}