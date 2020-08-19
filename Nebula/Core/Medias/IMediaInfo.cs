using System;
using System.Threading.Tasks;

namespace Nebula.Core.Medias
{
    /// <summary>
    /// Provide Media infos to implement by media provider results.
    /// </summary>
    public interface IMediaInfo
    {
        /// <summary>
        /// Media ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Media Owner Id ( Channel, Profile )
        /// </summary>
        public string OwnerId { get; }

        /// <summary>
        /// Media Name
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Media Author
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Media Description
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Thumbnail Url
        /// </summary>
        public string ThumbnailUrl { get; }

        /// <summary>
        /// Media Total Duration
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Media Upload Date
        /// </summary>
        DateTime UploadDate { get; }

        /// <summary>
        /// Returns media's artist's info
        /// </summary>
        /// <returns>IArtistInfo</returns>
        Task<IArtistInfo> GetArtistInfo();

        /// <summary>
        /// Returns media stream url
        /// </summary>
        /// <returns>Uri</returns>
        Task<Uri> GetAudioStreamUri();
    }
}