using System.IO;

namespace Nebula.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceInvalidPathChars(this string str, string replaceWith = "")
        {
            return string.Join(replaceWith, str.Split(Path.GetInvalidFileNameChars()));
        }
    }
}