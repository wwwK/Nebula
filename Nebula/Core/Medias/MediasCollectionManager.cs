using System;
using System.Collections.Generic;
using Nebula.Core.Medias.Events;

namespace Nebula.Core.Medias
{
    public class MediasCollectionManager
    {
        public MediasCollectionManager(MediasCollection collection)
        {
            Medias = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public MediasCollection Medias        { get; }
        public int              CurrentPage   { get; private set; } = 0;
        public int              TotalPages    => Medias.TotalPages;
        public bool             InfiniteCycle { get; set; } = true;

        public event EventHandler<MediaCollectionPageChangedEventArgs> PageChanged;

        public void NextPage()
        {
            if (Medias.Count == 0)
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

            if (oldPage != CurrentPage)
                PageChanged?.Invoke(this, new MediaCollectionPageChangedEventArgs(CurrentPage, TotalPages));
        }


        public void PreviousPage()
        {
            if (Medias.Count == 0)
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

            if (oldPage != CurrentPage)
                PageChanged?.Invoke(this, new MediaCollectionPageChangedEventArgs(CurrentPage, TotalPages));
        }

        public IEnumerable<IMediaInfo> GetMediasFromPage(int page) => Medias.GetMediasFromPage(page);
    }
}