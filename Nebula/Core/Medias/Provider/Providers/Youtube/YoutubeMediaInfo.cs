using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Nebula.Core.Medias.Provider.Providers.Youtube
{
    public class YoutubeMediaInfo : IMediaInfo
    {
        public YoutubeMediaInfo()
        {
        }

        public YoutubeMediaInfo(string id, string ownerId, string title, string description, string author, string thumbnail, TimeSpan duration, DateTime uploadDate)
        {
            Id = id;
            OwnerId = ownerId;
            Title = title;
            Description = description;
            Author = author;
            ThumbnailUrl = thumbnail;
            Duration = duration;
            UploadDate = uploadDate;
        }

        public string Id { get; }

        public string OwnerId { get; }

        public string Title { get; }

        public string Description { get; }

        public string Author { get; }

        public string ThumbnailUrl { get; }

        public TimeSpan Duration { get; }

        public DateTime UploadDate { get; }

        public async Task<IArtistInfo> GetArtistInfo()
        {
            IArtistInfo artistInfo = await NebulaClient.GetMediaProvider<YoutubeMediaProvider>().GetArtistInfo(OwnerId);
            return artistInfo;
        }

        public async Task<Uri> GetStreamUri()
        {
            StreamManifest streamManifest = await NebulaClient.GetMediaProvider<YoutubeMediaProvider>().Youtube.Videos.Streams.GetManifestAsync(new VideoId(Id));
            return new Uri(streamManifest.GetAudioOnly().WithHighestBitrate().Url);
        }
    }
}