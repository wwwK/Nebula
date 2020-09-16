using System;
using System.Threading.Tasks;
using System.Xml;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Nebula.Core.Medias.Provider.Providers.Youtube
{
    public class YoutubeMediaInfo : IMediaInfo
    {
        private string _id;
        private string _ownerId;

        public YoutubeMediaInfo(string id, string ownerId, string title, string description, string author,
                                string thumbnail, TimeSpan duration, DateTime uploadDate)
        {
            _id = id;
            _ownerId = ownerId;
            Title = title;
            Description = description;
            Author = author;
            ThumbnailUrl = thumbnail;
            Duration = duration;
            UploadDate = uploadDate;
        }

        public YoutubeMediaInfo(XmlElement element)
        {
            _id = element.GetAttribute("Id");
            _ownerId = element.GetAttribute("OwnerId");
            Title = element.GetAttribute("Title");
            Description = element.GetAttribute("Description");
            Author = element.GetAttribute("Author");
            ThumbnailUrl = element.GetAttribute("Thumbnail");
            Duration = TimeSpan.FromSeconds(double.Parse(element.GetAttribute("Duration")));
        }

        public YoutubeMediaInfo()
        {
        }


        string IMediaInfo.Id
        {
            get => _id;
            set => _id = value;
        }

        string IMediaInfo.OwnerId
        {
            get => _ownerId;
            set => _ownerId = value;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string   ThumbnailUrl { get; set; }
        public TimeSpan Duration     { get; set; }
        public DateTime UploadDate   { get; }

        public async Task<IArtistInfo> GetArtistInfo()
        {
            IArtistInfo artistInfo = await NebulaClient.GetMediaProvider<YoutubeMediaProvider>().GetArtistInfo(_ownerId);
            return artistInfo;
        }

        public async Task<Uri> GetAudioStreamUri()
        {
            StreamManifest streamManifest = await NebulaClient
                                                  .GetMediaProvider<YoutubeMediaProvider>().Youtube.Videos.Streams
                                                  .GetManifestAsync(new VideoId(_id));
            return new Uri(streamManifest.GetAudioOnly().WithHighestBitrate().Url);
        }

        public async Task<Uri> GetAudioVideoStreamUri()
        {
            StreamManifest streamManifest = await NebulaClient
                                                  .GetMediaProvider<YoutubeMediaProvider>().Youtube.Videos.Streams
                                                  .GetManifestAsync(new VideoId(_id));
            return new Uri(streamManifest.GetMuxed().WithHighestVideoQuality().Url);
        }

        public async Task<Uri> GetVideoStreamUri()
        {
            StreamManifest streamManifest = await NebulaClient
                                                  .GetMediaProvider<YoutubeMediaProvider>().Youtube.Videos.Streams
                                                  .GetManifestAsync(new VideoId(_id));
            return new Uri(streamManifest.GetVideo().WithHighestVideoQuality().Url);
        }

        public IMediaProvider GetMediaProvider()
        {
            return NebulaClient.GetMediaProvider<YoutubeMediaProvider>();
        }
    }
}