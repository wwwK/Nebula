﻿using System;
using System.Xml;

namespace Nebula.Core.Settings.Groups
{
    public class PrivacySettingsGroup : ISettingsGroup
    {
        public string GroupName { get; } = "Privacy";

        public event EventHandler SettingsChanged;

        public void Save(XmlElement element)
        {

        }
        
        public void Load(XmlElement element)
        {
            
        }
    }
}