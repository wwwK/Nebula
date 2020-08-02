using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;

namespace Nebula.Core.Updater
{
    public class NebulaUpdater
    {
        public NebulaUpdater()
        {
#if RELEASE
            Client.DownloadFileCompleted += OnDownloadFileCompleted;
            VerifyVersion();
#endif
        }

        private string                               VersionUrl { get; } = "https://pastebin.com/raw/G8uhLrcB";
        private string                               UpdateExtractorFileName { get; } = "Nebula.UpdateExtractor.exe";
        private WebClient                            Client { get; } = new WebClient();
        private Queue<(string Url, string FileName)> QueuedDownloads { get; } = new Queue<(string, string)>();

        public async void VerifyVersion()
        {
            using WebResponse responseAsync = await WebRequest.Create(VersionUrl).GetResponseAsync();
            Stream responseStream = responseAsync?.GetResponseStream();
            if (responseStream == null)
                return;
            using StreamReader streamReader = new StreamReader(responseStream);
            string str1 = await streamReader.ReadLineAsync();
            if (str1 == null || !str1.StartsWith("#") || IsCurrentVersion(str1.Replace("#", "")))
                return;
            string empty = string.Empty;
            string str2;
            while ((str2 = await streamReader.ReadLineAsync()) != null)
            {
                if (str2.StartsWith("http") && str2.Contains("|"))
                {
                    string[] strArray = str2.Split('|');
                    if (strArray.Length == 2)
                        QueuedDownloads.Enqueue((strArray[0], strArray[1]));
                }
            }

            int num = (int) MessageBox.Show(
                "A Nebula Update is available, the application will restart in order to apply this update.",
                "Nebula Updater");
            ProcessDownloads();
        }

        private bool IsCurrentVersion(string version)
        {
            Version assemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version;
            return assemblyVersion != null && version == assemblyVersion.ToString();
        }

        private bool ProcessDownloads()
        {
            if (QueuedDownloads.Count <= 0)
                return false;
            (string Url, string FileName) tuple = QueuedDownloads.Dequeue();
            Client.DownloadFileAsync(new Uri(tuple.Url), tuple.FileName);
            return true;
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (ProcessDownloads())
                return;
            Process.Start(UpdateExtractorFileName, Environment.CurrentDirectory);
            Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
        }
    }
}