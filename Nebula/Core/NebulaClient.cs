using System;
using System.Collections.Generic;
using System.Windows;
using ModernWpf.Media.Animation;
using Nebula.Core.Discord;
using Nebula.Core.Medias;
using Nebula.Core.Medias.Player;
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
        public static  DiscordRpc           Discord        { get; }

        static NebulaClient()
        {
            MediaProviders.Add(new YoutubeMediaProvider());
            MainWindow = Application.Current.MainWindow as MainWindow;
            MediaPlayer = new MediaPlayer();
            Updater = new NebulaUpdater();
            Discord = new DiscordRpc();
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
    }
}