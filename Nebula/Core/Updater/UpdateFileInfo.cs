using System;

namespace Nebula.Core.Updater
{
    public class UpdateFileInfo
    {
        public UpdateFileInfo(Uri uri, string fileName)
        {
            Uri = uri;
            FileName = fileName;
        }

        public Uri    Uri      { get; }
        public string FileName { get; }
    }
}