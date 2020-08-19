using System;
using System.Xml;

namespace Nebula.Core.Extensions
{
    /// <summary>
    /// Provide extensions for <see cref="XmlElement"/>
    /// </summary>
    public static class XmlElementExtensions
    {
        /// <summary>
        /// Get a boolean attribute from <see cref="XmlElement"/>
        /// </summary>
        /// <param name="element">Xml Element</param>
        /// <param name="attributeName">Attribute Name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns><see cref="bool"/> Attribute value or the provided default value if no value is found</returns>
        public static bool GetBoolAttribute(this XmlElement element, string attributeName, bool defaultValue)
        {
            return bool.TryParse(element.GetAttribute(attributeName), out bool value) ? value : defaultValue;
        }

        /// <summary>
        /// Get a string attribute from <see cref="XmlElement"/>
        /// </summary>
        /// <param name="element">Xml Element</param>
        /// <param name="attributeName">Attribute Name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns><see cref="string"/> Attribute value or the provided default value if no value is found</returns>
        public static string GetStringAttribute(this XmlElement element, string attributeName, string defaultValue)
        {
            return element.GetAttribute(attributeName) ?? defaultValue;
        }

        /// <summary>
        /// Get a int attribute from <see cref="XmlElement"/>
        /// </summary>
        /// <param name="element">Xml Element</param>
        /// <param name="attributeName">Attribute Name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns><see cref="int"/> Attribute value or the provided default value if no value is found</returns>
        public static int GetIntAttribute(this XmlElement element, string attributeName, int defaultValue)
        {
            return int.TryParse(element.GetAttribute(attributeName), out int value) ? value : defaultValue;
        }

        /// <summary>
        /// Get a enum attribute from <see cref="XmlElement"/>
        /// </summary>
        /// <param name="element">Xml Element</param>
        /// <param name="attributeName">Attribute Name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns><see cref="T"/> Attribute value or the provided default value if no value is found</returns>
        public static T GetEnumAttribute<T>(this XmlElement element, string attributeName, T defaultValue)
            where T : struct, IConvertible
        {
            return Enum.TryParse(element.GetStringAttribute(attributeName, ""), out T enumValue)
                ? enumValue
                : defaultValue;
        }
    }
}