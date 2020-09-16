using System;
using System.Collections.Generic;

namespace Nebula.Core.Data
{
    public interface IDataMember
    {
        public IDataFile DataFile { get; }

        public void SetValue<T>(string key, T value);
        public object GetValue(string key);
        public string GetString(string key);
        public int GetInt(string key);
        public double GetDouble(string key);
        public long GetLong(string key);
        public Guid GetGuid(string key);
        public IEnumerable<IDataMember> GetChilds();
        public IDataMember CreateChild(string name, IDataSaveable dataSaveable);
    }
}