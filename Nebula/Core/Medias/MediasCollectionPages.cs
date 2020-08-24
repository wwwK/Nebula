using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nebula.Core.Medias.Events;

namespace Nebula.Core.Medias
{
    public class MediasCollectionPages : ObservableCollection<IMediaInfo>
    {
        public MediasCollectionPages(MediasCollection collection)
        {
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
            UpdateMedias();
        }

        public MediasCollection Collection    { get; }
        public int              CurrentPage   { get; private set; } = 0;
        public bool             InfiniteCycle { get; set; }         = true;
        public int              TotalPages    => Collection.TotalPages;

        public event EventHandler<MediaCollectionPageChangedEventArgs> PageChanged;

        public void NextPage()
        {
            if (Collection.Count == 0)
                return;
            int oldPage = CurrentPage;
            if (CurrentPage == TotalPages - 1)
            {
                if (!InfiniteCycle)
                    return;
                CurrentPage = 0;
            }
            else
                CurrentPage++;

            UpdateMedias();
            if (oldPage != CurrentPage)
                PageChanged?.Invoke(this, new MediaCollectionPageChangedEventArgs(CurrentPage, TotalPages));
        }


        public void PreviousPage()
        {
            if (Collection.Count == 0)
                return;
            int oldPage = CurrentPage;
            if (CurrentPage == 0)
            {
                if (!InfiniteCycle)
                    return;
                CurrentPage = TotalPages - 1;
            }
            else
                CurrentPage--;

            UpdateMedias();
            if (oldPage != CurrentPage)
                PageChanged?.Invoke(this, new MediaCollectionPageChangedEventArgs(CurrentPage, TotalPages));
        }

        public IEnumerable<IMediaInfo> GetMediasFromPage(int page) => Collection.GetMediasFromPage(page);

        public void UpdateMedias()
        {
            if (Collection.Count == 0)
                return;
            Clear();
            foreach (IMediaInfo mediaInfo in GetMediasFromPage(CurrentPage))
                Add(mediaInfo);
        }
    }
}