using System.IO;
using System.Windows;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.Medias.Provider.Providers.Youtube;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Attributes;
using Nebula.UI.Controls;

namespace Nebula.Core.UI.Dialogs
{
    public class PlaylistImportDialog : DialogDataContent
    {
        private static readonly DataContentCache Cache = DataContentCache.BuildCache<PlaylistImportDialog>();

        public PlaylistImportDialog()
        {
            Title = NebulaClient.GetLocString("PlaylistImport");
            PrimaryButtonText = NebulaClient.GetLocString("Import");
            Cache.PrepareFor(this);
        }

        [DataProperty(typeof(PathSelectorControl), "PlaylistPathUrl", args: new object[] {"Playlist files (*.playlist) | *.playlist"})]
        public string Path { get; set; }

        public override async void OnPrimaryClick()
        {
            if (string.IsNullOrWhiteSpace(Path))
                return;
            IPlaylist playlist;
            if (Path.Contains("http") && Path.Contains("youtube"))
            {
                playlist = await NebulaClient.GetMediaProvider<YoutubeMediaProvider>().GetPlaylist(Path);
                NebulaClient.Playlists.AddPlaylist(playlist);
            }
            else
            {
                playlist = NebulaClient.Playlists.LoadPlaylist(new FileInfo(Path));
                NebulaClient.Playlists.SavePlaylist(playlist);
            }
            
            await NebulaMessageBox.ShowOk("PlaylistImport", "PlaylistImported", playlist.Name);
        }

        public override DataContentCache GetCache()
        {
            return Cache;
        }
    }
}