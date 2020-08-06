using System.Collections.Generic;
using System.Windows.Navigation;
using ModernWpf.Controls;

namespace Nebula.Core.UI
{
    public class FrameNavigationTracker
    {
        public FrameNavigationTracker(Frame frame)
        {
            Frame = frame;
            Frame.Navigated += OnNavigated;
        }

        public Frame Frame { get; }

        public int MaxHistory { get; set; } = 0;

        public int HistoryCount { get; private set; }

        public void Clear()
        {
            JournalEntry entry = Frame.RemoveBackEntry();
            while (entry != null)
                entry = Frame.RemoveBackEntry();
            HistoryCount = 0;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (MaxHistory <= 0)
            {
                Frame.NavigationService.RemoveBackEntry();
                return;
            }

            if (HistoryCount == MaxHistory)
                Frame.NavigationService.RemoveBackEntry();
            HistoryCount++;
        }
    }
}