using System.IO;

namespace Nebula.Core.Data
{
    public interface IDataFile
    {
        public string   DataType         { get; }
        public bool     IgnoreNullValues { get; }
        public FileInfo File             { get; }

        public bool Load(IDataLoadable loadable);
        public bool Save(IDataSaveable saveable);
    }
}