using System;

namespace Nebula.Core.Medias.Events
{
    public class MediaCollectionPageChangedEventArgs : EventArgs
    {
        public MediaCollectionPageChangedEventArgs(int page, int totalPages)
        {
            Page = page;
            TotalPages = totalPages;
        }

        public int Page       { get; }
        public int TotalPages { get; }
    }
}