using System.Collections.Generic;

namespace Nebula.Core.Medias.Provider.Providers.Youtube
{
    public class YoutubeArtistInfo : IArtistInfo
    {
        public YoutubeArtistInfo(string id, string title, string url, string logoUrl)
        {
            Id = id;
            Title = title;
            Url = url;
            LogoUrl = logoUrl;
        }
        
        public string Id { get; }
        
        public string Title { get; }
        
        public string Url { get; }

        public string LogoUrl { get; }

        public IAsyncEnumerable<IMediaInfo> GetMedias()
        {
            return NebulaClient.GetMediaProvider<YoutubeMediaProvider>().GetArtistMedias(Id);
        }
    }
}