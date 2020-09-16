using System;
using System.Globalization;
using System.Threading.Tasks;
using Nebula.Core.Data;
using Nebula.Core.Medias.Provider;
using Nebula.Net.Packets;

namespace Nebula.Core.Medias
{
    /// <summary>
    /// Provide Media infos to implement by media provider results.
    /// </summary>
    public interface IMediaInfo : IDataSaveable, IDataLoadable
    {
        /// <summary>
        /// Media ID
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// Media Owner Id ( Channel, Profile )
        /// </summary>
        public string OwnerId { get; protected set; }

        /// <summary>
        /// Media Name
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Media Author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Media Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Thumbnail Url
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Media Total Duration
        /// </summary>
        TimeSpan Duration { get; set; }

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

        /// <summary>
        /// Returns media video stream uri
        /// </summary>
        /// <returns>Uri</returns>
        Task<Uri> GetVideoStreamUri();

        /// <summary>
        /// Returns media audi and video stream uri
        /// Note: Support Muxed must be true
        /// </summary>
        /// <returns>Uri</returns>
        Task<Uri> GetAudioVideoStreamUri();

        /// <summary>
        /// Media Provider
        /// </summary>
        /// <returns>Media Provider</returns>
        IMediaProvider GetMediaProvider();

        MediaInfo AsMediaInfo() => new MediaInfo {Id = Id, Title = Title, Provider = GetMediaProvider().Name};

        bool IDataLoadable.OnLoad(IDataMember member)
        {
            Id = member.GetString("Id");
            OwnerId = member.GetString("OwnerId");
            Title = member.GetString("Title");
            Description = member.GetString("Description");
            Author = member.GetString("Author");
            ThumbnailUrl = member.GetString("Thumbnail");
            Duration = TimeSpan.FromSeconds(member.GetDouble("Duration"));
            return true;
        }

        bool IDataSaveable.OnSave(IDataMember member)
        {
            member.SetValue("ProviderType", GetType().FullName);
            member.SetValue("Id", Id);
            member.SetValue("OwnerId", OwnerId);
            member.SetValue("Title", Title);
            member.SetValue("Description", Description);
            member.SetValue("Author", Author);
            member.SetValue("Thumbnail", ThumbnailUrl);
            member.SetValue("Duration", Duration.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            return true;
        }
    }
}