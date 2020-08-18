using System;
using System.Xml;

namespace Nebula.Core.Extensions
{
    public static class XmlElementExts
    {
        public static bool GetBoolAttribute(this XmlElement element, string attributeName, bool defaultValue)
        {
            return bool.TryParse(element.GetAttribute(attributeName), out bool value) ? value : defaultValue;
        }

        public static string GetStringAttribute(this XmlElement element, string attributeName, string defaultValue)
        {
            return element.GetAttribute(attributeName) ?? defaultValue;
        }

        public static int GetIntAttribute(this XmlElement element, string attributeName, int defaultValue)
        {
            return int.TryParse(element.GetAttribute(attributeName), out int value) ? value : defaultValue;
        }

        public static T GetEnumAttribute<T>(this XmlElement element, string attributeName, T defaultValue)
            where T : struct, IConvertible
        {
            return Enum.TryParse(element.GetStringAttribute(attributeName, ""), out T enumValue)
                ? enumValue
                : defaultValue;
        }
    }
}