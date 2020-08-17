using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using EasySharp.Misc;

namespace Nebula.Core.Updater
{
    public class NebulaUpdaterV2
    {
        public NebulaUpdaterV2()
        {
            WebClient.DownloadFileCompleted += OnDownloadFileCompleted;
        }

        public  WebClient             WebClient               { get; } = new WebClient();
        public  string                CurrentDownloadedFile   { get; private set; }
        private Uri                   VersionUri              { get; } = new Uri("https://pastebin.com/raw/G8uhLrcB");
        private string                UpdateExtractorFileName { get; } = "Nebula.UpdateExtractor.exe";
        private Queue<UpdateFileInfo> QueuedFiles             { get; } = new Queue<UpdateFileInfo>();

        public async Task<UpdateCheckResult> CheckForUpdate()
        {
            await foreach (string line in ReadUpdateFile())
            {
                if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("#"))
                    return UpdateCheckResult.Failed;
                return IsCurrentVersion(line.Replace("#", ""))
                    ? UpdateCheckResult.UpToDate
                    : UpdateCheckResult.UpdateAvailable;
            }

            return UpdateCheckResult.Failed;
        }

        public async void DownloadUpdate()
        {
            await foreach (string line in ReadUpdateFile())
            {
                if (!line.StartsWith("http") && !line.Contains("|"))
                    continue;
                string[] split = line.Split('|');
                if (split.Length != 2)
                    continue;
                QueuedFiles.Enqueue(new UpdateFileInfo(new Uri(split[0]), split[1]));
            }

            if (QueuedFiles.Count > 2)
                ProcessDownloads();
        }

        private async IAsyncEnumerable<string> ReadUpdateFile()
        {
            using WebResponse response = await WebRequest.Create(VersionUri).GetResponseAsync();
            await using Stream responseStream = response?.GetResponseStream();
            if (responseStream != null)
            {
                using StreamReader streamReader = new StreamReader(responseStream);
                string line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                    yield return line;
            }
        }

        private bool ProcessDownloads()
        {
            if (QueuedFiles.Count <= 0)
                return false;
            UpdateFileInfo fileInfo = QueuedFiles.Dequeue();
            CurrentDownloadedFile = fileInfo.FileName;
            WebClient.DownloadFileAsync(fileInfo.Uri, fileInfo.FileName);
            return true;
        }

        private bool IsCurrentVersion(string version)
        {
            Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return assemblyVersion != null && version == assemblyVersion.ToString();
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (ProcessDownloads())
                return;
#if RELEASE
            Process.Start(UpdateExtractorFileName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
#endif
        }
    }
}