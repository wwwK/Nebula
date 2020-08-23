using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Nebula.Core.Medias.Events;

namespace Nebula.Core.Medias
{
    public class MediasCollection : ObservableCollection<IMediaInfo>
    {
        public MediasCollection(int maxMediasPerPage = -1)
        {
            MaxMediasPerPage = maxMediasPerPage == -1 ? NebulaClient.Settings.General.PlaylistMaxMediasPerPage : maxMediasPerPage;
        }
        
        public int  TotalPages       { get; private set; }
        public int  MaxMediasPerPage { get; }

        public event EventHandler<MediaCollectionPageChangedEventArgs> PageChanged;

        private void CalculateTotalPages()
        {
            TotalPages = (int) Math.Ceiling((double) Count / MaxMediasPerPage);
            if (TotalPages == 0)
                TotalPages = 1;
        }

        public IEnumerable<IMediaInfo> GetMediasFromPage(int page)
        {
            int startIndex = page * MaxMediasPerPage;
            int dif = Count - startIndex;
            int endIndex = startIndex + (dif < MaxMediasPerPage ? dif : MaxMediasPerPage);
            for (int i = startIndex; i != endIndex; i++)
                yield return this[i];
        }

        public void AddRange(IEnumerable<IMediaInfo> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (IMediaInfo mediaInfo in collection)
                Items.Add(mediaInfo);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary> 
        /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
        /// </summary> 
        public void RemoveRange(IEnumerable<IMediaInfo> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (IMediaInfo mediaInfo in collection)
                Items.Remove(mediaInfo);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            CalculateTotalPages();
        }
    }
}