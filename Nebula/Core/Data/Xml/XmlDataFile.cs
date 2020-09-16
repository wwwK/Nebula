using System.IO;
using System.Xml;

namespace Nebula.Core.Data.Xml
{
    public class XmlDataFile : IDataFile
    {
        public XmlDataFile(FileInfo fileInfo, bool ignoreNullValues = true)
        {
            File = fileInfo;
            IgnoreNullValues = ignoreNullValues;
        }

        public string   DataType         { get; }      = "Xml";
        public string   RootName         { get; set; } = "Root";
        public bool     IgnoreNullValues { get; }
        public FileInfo File             { get; }

        public bool Load(IDataLoadable dataLoadable)
        {
            if (!File.Exists || !File.FullName.EndsWith(".playlist"))
                return false;
            XmlDocument document = new XmlDocument();
            document.Load(File.FullName);
            return document.DocumentElement != null && dataLoadable.OnLoad(new XmlDataMember(this, document.DocumentElement));
        }

        public bool Save(IDataSaveable dataSaveable)
        {
            if (File.Exists)
                File.Delete();
            XmlDocument document = new XmlDocument();
            XmlElement rootElement = document.CreateElement(RootName);
            if (!dataSaveable.OnSave(new XmlDataMember(this, rootElement)))
                return false;
            document.AppendChild(rootElement);
            document.Save(File.FullName);
            return true;
        }
    }
}