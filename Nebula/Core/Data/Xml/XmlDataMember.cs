using System;
using System.Collections.Generic;
using System.Xml;

namespace Nebula.Core.Data.Xml
{
    public class XmlDataMember : IDataMember
    {
        public XmlDataMember(IDataFile file, XmlElement xmlElement)
        {
            DataFile = file;
            XmlElement = xmlElement;
        }

        public IDataFile  DataFile   { get; }
        public XmlElement XmlElement { get; }

        public void SetValue<T>(string key, T value)
        {
            if (value == null && DataFile.IgnoreNullValues)
                return;
            XmlElement.SetAttribute(key, value?.ToString() ?? "NULL");
        }

        public object GetValue(string key)
        {
            return XmlElement.GetAttribute(key);
        }

        public string GetString(string key)
        {
            return XmlElement.GetAttribute(key);
        }

        public int GetInt(string key)
        {
            return int.TryParse(GetString(key), out int value) ? value : -1;
        }

        public double GetDouble(string key)
        {
            return double.TryParse(GetString(key), out double value) ? value : -1D;
        }

        public long GetLong(string key)
        {
            return long.TryParse(GetString(key), out long value) ? value : -1L;
        }

        public float GetFloat(string key)
        {
            return float.TryParse(GetString(key), out float value) ? value : -1f;
        }

        public short GetShort(string key)
        {
            return short.TryParse(GetString(key), out short value) ? value : (short) -1;
        }

        public Guid GetGuid(string key)
        {
            return Guid.TryParse(GetString(key), out Guid guid) ? guid : Guid.Empty;
        }

        public IEnumerable<IDataMember> GetChilds()
        {
            foreach (object child in XmlElement.ChildNodes)
                yield return new XmlDataMember(DataFile, (XmlElement) child);
        }

        public IDataMember CreateChild(string name, IDataSaveable dataSaveable)
        {
            XmlElement element = XmlElement.OwnerDocument.CreateElement(name);
            IDataMember dataMember = new XmlDataMember(DataFile, element);
            dataSaveable.OnSave(dataMember);
            XmlElement.AppendChild(element);
            return dataMember;
        }
    }
}