using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Medias.Playlist;

namespace Nebula.Pages.Dialogs
{
    public partial class PlaylistEditDialog : ContentDialog
    {
        public PlaylistEditDialog(PlaylistEditDialogAction action, IPlaylist playlist = null)
        {
            InitializeComponent();
            Action = action;
            if (action == PlaylistEditDialogAction.CreatePlaylist)
            {
                Title = NebulaClient.GetLocString("CreatePlaylist");
                PrimaryButtonText = NebulaClient.GetLocString("Create");
            }
            else
            {
                Title = NebulaClient.GetLocString("EditPlaylist");
                PrimaryButtonText = NebulaClient.GetLocString("Edit");
            }

            if (playlist != null && action == PlaylistEditDialogAction.EditPlaylist)
            {
                PlaylistName.Text = playlist.Name;
                PlaylistDescription.Text = playlist.Description;
                PlaylistAuthor.Text = playlist.Author;
                string thumbnailUri = playlist.Thumbnail.ToString();
                PlaylistThumbnail.Text = thumbnailUri.StartsWith("file") ? playlist.Thumbnail.LocalPath : thumbnailUri;
            }
        }

        public PlaylistEditDialogAction Action { get; }

        public IPlaylist Playlist { get; }

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