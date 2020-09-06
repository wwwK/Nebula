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

        event EventHandler SettingsChanged;

        public void OnSettingsLoaded()
        {
            
        }
    }
}