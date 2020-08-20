using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Medias.Playlist;

namespace Nebula.Pages.Dialogs
{
    public partial class PlaylistImportDialog : ContentDialog
    {
        public PlaylistImportDialog()
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

        private void OnFromFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Playlist files (*.playlist) | *.playlist",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true && File.Exists(dialog.FileName))
                PlaylistPath.Text = dialog.FileName;
        }
    }
}