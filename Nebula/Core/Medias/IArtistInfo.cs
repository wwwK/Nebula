using System.Collections.Generic;

namespace Nebula.Core.Medias
{
    public interface IArtistInfo
    {
        /// <summary>
        /// Artist Id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Artist Title
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Artist Url
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Artist Logo Url
        /// </summary>
        public string LogoUrl { get; }

        /// <summary>
        /// Returns Artist's Medias
        /// </summary>
        /// <returns>IMediaInfo</returns>
        public IAsyncEnumerable<IMediaInfo> GetMedias();
    }
}