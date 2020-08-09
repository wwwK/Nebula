using System;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Core.Medias
{
    public class MediaQueue : IEnumerable<IMediaInfo>
    {
        private static readonly Random _random = new Random();

        public MediaQueue()
        {
        }

        public bool IsEmpty => Queue.Count == 0;
        public int  Count   => Queue.Count;

        private List<IMediaInfo> Queue { get; } = new List<IMediaInfo>();

        public void Enqueue(IMediaInfo mediaInfo, int insertIndex = -1)
        {
            if (insertIndex == -1)
                Queue.Add(mediaInfo);
            else
                Queue.Insert(insertIndex, mediaInfo);
        }

        public void Remove(IMediaInfo mediaInfo)
        {
            if (Queue.Contains(mediaInfo))
                Queue.Remove(mediaInfo);
        }

        public void Clear()
        {
            Queue.Clear();
        }

        public bool IsQueued(IMediaInfo mediaInfo)
        {
            return Queue.Contains(mediaInfo);
        }

        public IMediaInfo Dequeue(bool random = false)
        {
            if (IsEmpty)
                return default;
            IMediaInfo mediaInfo = Queue[random ? _random.Next(Queue.Count) : 0];
            Queue.Remove(mediaInfo);
            return mediaInfo;
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