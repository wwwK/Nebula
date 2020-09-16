using System.IO;

namespace Nebula.Core.Data
{
    public static class DataFileManager
    {
        public static void SaveFile(IDataFile dataFile)
        {
            if (dataFile.File.Exists)
                dataFile.File.Delete();
            dataFile.File.Create();
        }
    }
}