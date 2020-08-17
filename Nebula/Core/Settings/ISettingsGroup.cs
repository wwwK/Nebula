using System;
using System.Xml;

namespace Nebula.Core.Settings
{
    public interface ISettingsGroup
    {
        /// <summary>
        /// Settings Group Name
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// Save Settings
        /// </summary>
        /// <param name="document">Document</param>
        /// <returns>XmlElement</returns>
        void Save(XmlElement element);

        void Load(XmlElement element);

        event EventHandler SettingsChanged;
    }
}