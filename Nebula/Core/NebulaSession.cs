using System.Collections.Generic;
using System.Linq;
using Nebula.Core.Medias;

namespace Nebula.Core
{
    public class NebulaSession
    {
        public NebulaSession()
        {

        }

        public  int          MaxSearchHistory { get; set; } = 5;
        private List<string> SearchHistory    { get; }      = new List<string>();

        public IEnumerable<string> GetSearchHistory()
        {
            return SearchHistory.ToList();
        }

        public void AddBrowserSearch(string query)
        {
            if (SearchHistory.Contains(query))
                return;
            if (SearchHistory.Count >= MaxSearchHistory)
                SearchHistory.RemoveAt(0);
            SearchHistory.Add(query);
        }
    }
}