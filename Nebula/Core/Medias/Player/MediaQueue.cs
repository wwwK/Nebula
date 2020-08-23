using System;
using System.Collections;
using System.Collections.Generic;
using Nebula.Core.Medias.Playlist;

namespace Nebula.Core.Medias.Player
{
    public class MediaQueue : IEnumerable<IMediaInfo>
    {
        private static readonly Random Random = new Random();

        public MediaQueue()
        {
        }

        public  bool             IsEmpty       => Queue.Count == 0;
        public  int              Count         => Queue.Count;
        private MediasCollection Queue         { get; } = new MediasCollection();
        private MediasCollection RecentDequeue { get; } = new MediasCollection();

        /// <summary>
        /// Enqueue the specified <see cref="IPlaylist"/>
        /// </summary>
        /// <param name="playlist">The playlist to enqueue</param>
        /// <param name="clear">Clear current queue</param>
        public void Enqueue(IPlaylist playlist, bool clear = true)
        {
            if (clear)
                Clear();
            foreach (IMediaInfo mediaInfo in playlist)
                Queue.Add(mediaInfo);
        }

        /// <summary>
        /// Enqueue the specified <see cref="IMediaInfo"/>
        /// </summary>
        /// <param name="mediaInfo">The media to enqueue</param>
        /// <param name="insertIndex">The insert index</param>
        public void Enqueue(IMediaInfo mediaInfo, int insertIndex = -1)
        {
            if (insertIndex == -1)
                Queue.Add(mediaInfo);
            else
                Queue.Insert(insertIndex, mediaInfo);
        }

        /// <summary>
        /// Remove the specified <see cref="IMediaInfo"/> from the queue
        /// </summary>
        /// <param name="mediaInfo">The media to remove</param>
        public void Remove(IMediaInfo mediaInfo)
        {
            if (Queue.Contains(mediaInfo))
                Queue.Remove(mediaInfo);
        }

        /// <summary>
        /// Clear queue
        /// </summary>
        public void Clear()
        {
            Queue.Clear();
            RecentDequeue.Clear();
        }

        /// <summary>
        /// Check if the specified <see cref="IMediaInfo"/> is already queued.
        /// </summary>
        /// <param name="mediaInfo">The media to check</param>
        /// <returns>Returns true if the specified <see cref="IMediaInfo"/> is already queued, false otherwise.</returns>
        public bool IsQueued(IMediaInfo mediaInfo)
        {
            return Queue.Contains(mediaInfo);
        }

        /// <summary>
        /// Dequeue a <see cref="IMediaInfo"/>.
        /// </summary>
        /// <param name="random">Queue randomly</param>
        /// <returns>Dequeued media info if queue is not empty, null otherwise</returns>
        public IMediaInfo Dequeue(bool random = false)
        {
            if (IsEmpty)
                return default;
            IMediaInfo mediaInfo = Queue[random ? Random.Next(Queue.Count) : 0];
            Queue.Remove(mediaInfo);
            RecentDequeue.Add(mediaInfo);
            return mediaInfo;
        }

        /// <summary>
        /// Rewind a dequeue.
        /// </summary>
        /// <returns>Rewinded Media</returns>
        public IMediaInfo RewindDequeue()
        {
            if (RecentDequeue.Count == 0)
                return null;
            IMediaInfo mediaInfo = RecentDequeue[^1];
            RecentDequeue.Remove(mediaInfo);
            //Enqueue(dequeueInfo.MediaInfo, dequeueInfo.Index);
            return mediaInfo;
        }

        /// <summary>
        /// Get a new Queue manager
        /// </summary>
        /// <returns>Queue Manager</returns>
        public MediasCollectionPages GetPages()
        {
            return new MediasCollectionPages(Queue);
        }

        public IEnumerator<IMediaInfo> GetEnumerator()
        {
            return Queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}