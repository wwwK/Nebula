using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.Medias.Playlist.Playlists;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace Nebula.Core.Medias.Provider.Providers.Youtube
{
    public class YoutubeMediaProvider : IMediaProvider
    {
        public YoutubeMediaProvider()
        {
        }

        public string Url         { get; } = "https://www.youtube.com/";
        public string Name        { get; } = "Youtube";
        public string NameColorEx { get; } = "#ff0000";

        public YoutubeClient Youtube { get; } = new YoutubeClient();

        private IMediaInfo VideoToMediaInfo(Video video)
        {
            return new YoutubeMediaInfo(video.Id.Value, video.ChannelId.Value, video.Title,
                video.Description, video.Author, video.Thumbnails.LowResUrl, video.Duration, video.UploadDate.DateTime);
        }

        public async IAsyncEnumerable<IMediaInfo> SearchMedias(string query, params object[] args)
        {
            if (args == null || args.Length < 2)
            {
                await foreach (Video video in Youtube.Search.GetVideosAsync(query, 0,
                    NebulaClient.Settings.General.SearchMaxPages))
                    yield return VideoToMediaInfo(video);
            }
            else if (args[0] is int startPage && args[1] is int pageCount)
            {
                await foreach (Video video in Youtube.Search.GetVideosAsync(query, startPage, pageCount))
                    yield return VideoToMediaInfo(video);
            }
            else
                throw new ArgumentException(
                    "Missing and/or wrong arguments. Supported arguments are null or int(startPage), int(pageCount)");
        }

        public async IAsyncEnumerable<IMediaInfo> GetArtistMedias(string query, params object[] args)
        {
            await foreach (Video video in Youtube.Channels.GetUploadsAsync(new ChannelId(query)))
                yield return VideoToMediaInfo(video);
        }

        public async Task<IMediaInfo> GetMediaInfo(string query, params object[] args)
        {
            return VideoToMediaInfo(await Youtube.Videos.GetAsync(new VideoId(query)));
        }

        public async Task<IArtistInfo> GetArtistInfo(string query, params object[] args)
        {
            Channel channel = await Youtube.Channels.GetAsync(new ChannelId(query));
            return new YoutubeArtistInfo(channel.Id, channel.Title, channel.Url, channel.LogoUrl);
        }

        public async Task<IPlaylist> GetPlaylist(string query, params object[] args)
        {
            YoutubeExplode.Playlists.Playlist playlist = await Youtube.Playlists.GetAsync(query.Trim());
            string thumbnail = playlist.Thumbnails?.MediumResUrl;
            NebulaPlaylist nebulaPlaylist = new NebulaPlaylist(playlist.Title, playlist.Description, playlist.Author, thumbnail == null ? null : new Uri(thumbnail));
            nebulaPlaylist.AutoSave = false;
            await foreach (Video video in Youtube.Playlists.GetVideosAsync(playlist.Id))
                nebulaPlaylist.AddMedia(VideoToMediaInfo(video));
            nebulaPlaylist.AutoSave = true;
            NebulaClient.Playlists.SavePlaylist(nebulaPlaylist);
            return nebulaPlaylist;
        }
    }
}