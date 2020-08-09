using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ModernWpf.Media.Animation;
using Nebula.Core.Events;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Player;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.Medias.Provider;
using Nebula.Core.Medias.Provider.Providers.Youtube;
using Nebula.Core.Updater;
using Nebula.Pages;

namespace Nebula.Core
{
    public static class NebulaClient
    {
        private static List<IMediaProvider> MediaProviders { get; } = new List<IMediaProvider>();
        private static MainWindow           MainWindow     { get; }
        public static  MediaPlayer          MediaPlayer    { get; }
        public static  NebulaUpdater        Updater        { get; }
        public static  NebulaSession        Session        { get; }
        public static  NebulaSettings       Settings       { get; }
        public static  PlaylistsManager     Playlists      { get; }

        public static event EventHandler<NebulaAppLoopEventArgs> Tick;

        internal static CancellationTokenSource CancellationTokenSource { get; }

        static NebulaClient()
        {
            MainWindow = Application.Current.MainWindow as MainWindow;
            Settings = new NebulaSettings();
            MediaPlayer = new MediaPlayer();
            Updater = new NebulaUpdater();
            Playlists = new PlaylistsManager();
            Session = new NebulaSession(); //Needs to be latest

            MediaProviders.Add(new YoutubeMediaProvider());

            CancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => AppTick(CancellationTokenSource.Token, 500));
        }

        public static void Navigate(Type type)
        {
            if (type != null && MainWindow.ContentFrame.CurrentSourcePageType != type)
                MainWindow.ContentFrame.Navigate(type);
        }

        public static void Navigate(Type type, object arg)
        {
            MainWindow.ContentFrame.Navigate(type, arg);
        }

        public static void Navigate(Type type, object arg, NavigationTransitionInfo transitionInfo)
        {
            MainWindow.ContentFrame.Navigate(type, arg, transitionInfo);
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
                MessageBox.Show(e.StackTrace);
                throw;
            }
        }
    }
}