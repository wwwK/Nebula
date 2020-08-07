using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using ModernWpf.Controls;

namespace Nebula.Pages.Dialogs
{
    public partial class CreatePlaylistDialog : ContentDialog
    {
        public CreatePlaylistDialog()
        {
            InitializeComponent();
        }

        private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
        }

        private void OnPlaylistThumbnailMouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true && File.Exists(dialog.FileName))
                PlaylistThumbnail.Text = dialog.FileName;
        }
    }
}