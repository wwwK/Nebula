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

        public int  CurrentPage      { get; private set; } = 0;
        public int  TotalPages       { get; private set; }
        public int  MaxMediasPerPage { get; }
        public bool InfiniteCycle    { get; set; } = true;

        public event EventHandler<MediaCollectionPageChangedEventArgs> PageChanged;

        private void CalculateTotalPages()
        {
            TotalPages = Count / MaxMediasPerPage;
            if (TotalPages == 0)
                TotalPages = 1;
        }

        public void NextPage()
        {
            int oldPage = CurrentPage;
            if (CurrentPage == TotalPages)
            {
                if (!InfiniteCycle)
                    return;
                CurrentPage = 0;
            }
            else
                CurrentPage++;

            if (oldPage != CurrentPage)
                PageChanged?.Invoke(this, new MediaCollectionPageChangedEventArgs(CurrentPage, TotalPages));
        }


        public void PreviousPage()
        {
            int oldPage = CurrentPage;
            if (CurrentPage == 0)
            {
                if (!InfiniteCycle)
                    return;
                CurrentPage = TotalPages;
            }
            else
                CurrentPage--;

            if (oldPage != CurrentPage)
                PageChanged?.Invoke(this, new MediaCollectionPageChangedEventArgs(CurrentPage, TotalPages));
        }

        public IEnumerable<IMediaInfo> GetMediasFromPage(int page)
        {
            int startIndex = page * MaxMediasPerPage;
            int endIndex = startIndex + (startIndex + Count < MaxMediasPerPage ? Count : MaxMediasPerPage);
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