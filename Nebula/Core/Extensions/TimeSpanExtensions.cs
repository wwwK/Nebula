using System;

namespace Nebula.Core.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToFormattedHMS(this TimeSpan timeSpan)
        {
            string hours = timeSpan.Hours > 9 ? timeSpan.Hours.ToString() : $"0{timeSpan.Hours}";
            string minutes = timeSpan.Minutes > 9 ? timeSpan.Minutes.ToString() : $"0{timeSpan.Minutes}";
            string seconds = timeSpan.Seconds > 9 ? timeSpan.Seconds.ToString() : $"0{timeSpan.Seconds}";
            return $"{hours}:{minutes}:{seconds}";
        }
    }
}