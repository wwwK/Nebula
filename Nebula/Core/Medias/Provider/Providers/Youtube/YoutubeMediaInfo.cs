using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Nebula.Core.Medias.Provider.Providers.Youtube
{
    public class YoutubeMediaInfo : IMediaInfo
    {
        public YoutubeMediaInfo(string id, string ownerId, string title, string description, string author,
                                string thumbnail, TimeSpan duration, DateTime uploadDate)
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
        
        public YoutubeMediaInfo(XmlElement element)
        {
            Id = element.GetAttribute("Id");
            OwnerId = element.GetAttribute("OwnerId");
            Title = element.GetAttribute("Title");
            Description = element.GetAttribute("Description");
            Author = element.GetAttribute("Author");
            ThumbnailUrl = element.GetAttribute("Thumbnail");
            Duration = TimeSpan.FromSeconds(double.Parse(element.GetAttribute("Duration")));
        }

        public YoutubeMediaInfo()
        {
            
        }

        public string Id { get; set; }

        public string OwnerId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string ThumbnailUrl { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime UploadDate { get; }

        public async Task<IArtistInfo> GetArtistInfo()
        {
            IArtistInfo artistInfo = await NebulaClient.GetMediaProvider<YoutubeMediaProvider>().GetArtistInfo(OwnerId);
            return artistInfo;
        }

        public async Task<Uri> GetAudioStreamUri()
        {
            StreamManifest streamManifest = await NebulaClient
                                                  .GetMediaProvider<YoutubeMediaProvider>().Youtube.Videos.Streams
                                                  .GetManifestAsync(new VideoId(Id));
            return new Uri(streamManifest.GetAudioOnly().WithHighestBitrate().Url);
        }

        public async Task<Uri> GetAudioVideoStreamUri()
        {
            StreamManifest streamManifest = await NebulaClient
                                                  .GetMediaProvider<YoutubeMediaProvider>().Youtube.Videos.Streams
                                                  .GetManifestAsync(new VideoId(Id));
            return new Uri(streamManifest.GetMuxed().WithHighestVideoQuality().Url);
        }

        public async Task<Uri> GetVideoStreamUri()
        {
            StreamManifest streamManifest = await NebulaClient
                                                  .GetMediaProvider<YoutubeMediaProvider>().Youtube.Videos.Streams
                                                  .GetManifestAsync(new VideoId(Id));
            return new Uri(streamManifest.GetVideo().WithHighestVideoQuality().Url);
        }

        public IMediaProvider GetMediaProvider()
        {
            return NebulaClient.GetMediaProvider<YoutubeMediaProvider>();
        }
    }
}