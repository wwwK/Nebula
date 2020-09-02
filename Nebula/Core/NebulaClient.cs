using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using EasySharp.Windows.Hookers;
using EasySharp.Windows.Hookers.Keyboard;
using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using Nebula.Core.Events;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Player;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.Medias.Provider;
using Nebula.Core.Medias.Provider.Providers.Youtube;
using Nebula.Core.Networking;
using Nebula.Core.Settings;
using Nebula.Core.Updater;
using Nebula.Pages;
using Nebula.Pages.Dialogs;
using Nebula.UI;
using Nebula.UI.Pages;

namespace Nebula.Core
{
    public static class NebulaClient
    {
        private static List<IMediaProvider> MediaProviders { get; } = new List<IMediaProvider>();
        public static  MainWindow           MainWindow     { get; }
        public static  MediaPlayer          MediaPlayer    { get; }
        public static  NebulaUpdater        Updater        { get; }
        public static  NebulaSession        Session        { get; }
        public static  NebulaSettings       Settings       { get; }
        public static  NebulaNetClient      Network        { get; }
        public static  PlaylistsManager     Playlists      { get; }
        public static  KeyboardHooker       KeyboardHooker { get; }

        public static event EventHandler<NebulaAppLoopEventArgs> Tick;

        internal static CancellationTokenSource CancellationTokenSource { get; }

        static NebulaClient()
        {
            MainWindow = Application.Current.MainWindow as MainWindow;
            Settings = NebulaSettings.LoadSettings(); //Needs to be first 
            MediaPlayer = new MediaPlayer();
            Updater = new NebulaUpdater();
            Playlists = new PlaylistsManager();
            KeyboardHooker = new KeyboardHooker();
            Network = new NebulaNetClient();
            Session = new NebulaSession(); //Needs to be latest

            MediaProviders.Add(new YoutubeMediaProvider());

            KeyboardHooker.KeyDown += OnGlobalKeyDown;
            if (Settings.General.MediaKeyEnabled)
                KeyboardHooker.Hook();

            CancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => AppTick(CancellationTokenSource.Token, 500));
#if RELEASE
            CheckForUpdate(true);
#endif
        }

        private static void OnGlobalKeyDown(object sender, KeyboardKeyDownEventArgs e)
        {
            switch (e.Key)
            {
                case EVirtualKeys.MEDIA_PLAY_PAUSE when MediaPlayer.IsPaused:
                    MediaPlayer.Resume();
                    e.Handled = true;
                    break;
                case EVirtualKeys.MEDIA_PLAY_PAUSE when !MediaPlayer.IsPaused:
                    MediaPlayer.Pause();
                    e.Handled = true;
                    break;
                case EVirtualKeys.MEDIA_STOP:
                    MediaPlayer.Stop();
                    e.Handled = true;
                    break;
                case EVirtualKeys.MEDIA_NEXT_TRACK:
                    MediaPlayer.Forward(true);
                    e.Handled = true;
                    break;
                case EVirtualKeys.VOLUME_MUTE:
                    MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
                    e.Handled = true;
                    break;
                case EVirtualKeys.VOLUME_UP when MediaPlayer.Volume < 100:
                    MediaPlayer.Volume += Settings.General.MediaKeySoundIncDecValue;
                    e.Handled = true;
                    break;
                case EVirtualKeys.VOLUME_DOWN when MediaPlayer.Volume > 0:
                    MediaPlayer.Volume -= Settings.General.MediaKeySoundIncDecValue;
                    e.Handled = true;
                    break;
            }
        }

        public static void Navigate(Type type)
        {
            if (type != null && MainWindow.ContentFrame.CurrentSourcePageType != type)
                MainWindow.ContentFrame.Navigate(type, null, GetNavigationAnimation(type));
        }

        public static void Navigate(Type type, object arg)
        {
            MainWindow.ContentFrame.Navigate(type, arg, GetNavigationAnimation(type));
        }

        public static void Navigate(Type type, object arg, NavigationTransitionInfo transitionInfo)
        {
            MainWindow.ContentFrame.Navigate(type, arg, transitionInfo);
        }

        private static NavigationTransitionInfo GetNavigationAnimation(Type type)
        {
            Type currentPageType = MainWindow.ContentFrame.CurrentSourcePageType;
            List<Type> types = new List<Type> //Todo: this should be cached
            {
                typeof(HomePage),
                typeof(BrowsePage),
                typeof(PlaylistsPage),
                typeof(SettingsPage)
            };
            int curIndex = types.IndexOf(currentPageType);
            int newIndex = types.IndexOf(type);
            return newIndex < curIndex
                ? new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromRight}
                : new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft};
        }

        public static Page GetCurrentPage()
        {
            return MainWindow.ContentFrame.Content as Page;
        }

        public static async Task CheckForUpdate(bool silentIfUpToDate)
        {
            UpdateCheckResult checkResult = await Updater.CheckForUpdate();
            switch (checkResult)
            {
                case UpdateCheckResult.Failed when !silentIfUpToDate:
                    await NebulaMessageBox.ShowOk("UpdateCheckFailed", "UpdateCheckFailedMsg");
                    break;
                case UpdateCheckResult.UpdateAvailable:
                {
                    ContentDialogResult result =
                        await NebulaMessageBox.ShowYesNo("UpdateCheckAvailable", "UpdateCheckAvailableMsg");
                    if (result == ContentDialogResult.Primary)
                    {
                        Updater.DownloadUpdate();
                        Navigate(typeof(SettingsPage), "ABOUT");
                    }
                }
                    break;
                case UpdateCheckResult.UpToDate when !silentIfUpToDate:
                {
                    ContentDialogResult result =
                        await NebulaMessageBox.ShowYesNo("UpdateCheckUpToDate", "UpdateCheckUpToDateMsg");
                    if (result == ContentDialogResult.Primary)
                    {
                        Updater.DownloadUpdate();
                        Navigate(typeof(SettingsPage), "ABOUT");
                    }
                }
                    break;
            }
        }

        public static TProvider GetMediaProvider<TProvider>() where TProvider : IMediaProvider
        {
            foreach (IMediaProvider mediaProvider in MediaProviders)
            {
                if (mediaProvider is TProvider provider)
                    return provider;
            }

            return default;
        }

        public static async IAsyncEnumerable<IMediaInfo> Search(string searchQuery, params object[] args)
        {
            foreach (IMediaProvider provider in MediaProviders)
            {
                await foreach (IMediaInfo mediaInfo in provider.SearchMedias(searchQuery, args))
                    yield return mediaInfo;
            }
        }

        public static string GetLocString(string key, params object[] format)
        {
            if (format == null || format.Length == 0)
                return Resources.nebula.ResourceManager.GetString(key) ?? string.Empty;
            return string.Format(Resources.nebula.ResourceManager.GetString(key) ?? $"UNKNOWN_KEY({key})", format);
        }

        public static void Invoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        public static void BeginInvoke(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }

        private static async void AppTick(CancellationToken token, int delay)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(delay, token);
                    Invoke(() => Tick?.Invoke(Application.Current, new NebulaAppLoopEventArgs()));
                }
            }
            catch (OperationCanceledException e)
            {
            }
        }
    }
}