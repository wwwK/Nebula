using System.Linq;
using System.Text.RegularExpressions;

namespace Nebula.Server.Extensions
{
    public static class StringExtensions
    {
        public static string[] SplitWithoutQuotes(this string str)
        {
            return Regex.Matches(str, @"[\""].+?[\""]|[^ ]+").Select(m => m.Value.Replace("\"", string.Empty)).ToArray();
        }
    }
}