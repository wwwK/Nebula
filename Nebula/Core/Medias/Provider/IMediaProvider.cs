using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nebula.Core.Medias.Provider
{
    public interface IMediaProvider
    {
        /// <summary>
        /// Media Provider Url
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Search medias
        /// </summary>
        /// <param name="query">Media Query, usually keywords</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>IMediaInfo</returns>
        IAsyncEnumerable<IMediaInfo> SearchMedias(string query, params object[] args);

        /// <summary>
        /// Get Artist's medias
        /// </summary>
        /// <param name="query">Media query, usually artist's Id</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>IMediaInfo</returns>
        IAsyncEnumerable<IMediaInfo> GetArtistMedias(string query, params object[] args);

        /// <summary>
        /// Get Media info
        /// </summary>
        /// <param name="query">Media query, usually media Id or Url</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns></returns>
        Task<IMediaInfo> GetMediaInfo(string query, params object[] args);

        /// <summary>
        /// Get Artist info
        /// </summary>
        /// <param name="query">Artist Query, usually artist's Id</param>
        /// <param name="args">Optional Parameters</param>
        /// <returns>IArtistInfo</returns>
        Task<IArtistInfo> GetArtistInfo(string query, params object[] args);
    }
}