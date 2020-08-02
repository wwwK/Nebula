using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace Nebula.Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string DefaultPath;
        private const    string VersionUrl              = "https://pastebin.com/raw/G8uhLrcB";
        private const    string UpdateExtractorFileName = "Nebula.UpdateExtractor.exe";

        public MainWindow()
        {
            InitializeComponent();
            DefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Nebula");
            WebClient.DownloadFileCompleted += OnDownloadFileCompleted;
            InstallPath.Text = DefaultPath;
        }

        private WebClient                            WebClient       { get; } = new WebClient();
        private Queue<(string Url, string FileName)> QueuedDownloads { get; } = new Queue<(string, string)>();

        private async void DownloadNebula()
        {
            using WebResponse response = await WebRequest.Create(VersionUrl).GetResponseAsync();
            Stream responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                ExitInstaller("Failed to install nebula. Response Stream is null");
                return;
            }

            using StreamReader streamReader = new StreamReader(responseStream);
            string versionLine = await streamReader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(versionLine) || !versionLine.StartsWith("#"))
            {
                ExitInstaller("Failed to install nebula. Missing version line");
                return;
            }

            string line = string.Empty;
            while ((line = await streamReader.ReadLineAsync()) != null)
            {
                if (line.StartsWith("//"))
                    continue;
                if (line.StartsWith("http") && line.Contains("|"))
                {
                    string[] split = line.Split('|');
                    if (split.Length == 2)
                        QueuedDownloads.Enqueue((split[0], split[1]));
                }
            }

            ProcessDownloads();
        }

        private bool ProcessDownloads()
        {
            if (QueuedDownloads.Count <= 0)
                return false;
            (string Url, string FileName) file = QueuedDownloads.Dequeue();
            WebClient.DownloadFileAsync(new Uri(file.Url), Path.Combine(InstallPath.Text, file.FileName));
            return true;
        }

        private void ExitInstaller(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (message != null)
                    MessageBox.Show(message);
                Application.Current.Shutdown();
            });
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (ProcessDownloads())
                return;
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(InstallPath.Text, UpdateExtractorFileName),
                WorkingDirectory = InstallPath.Text,
                Arguments = "/justInstalled"
            };
            Process.Start(processStartInfo);
            ExitInstaller(null);
        }

        private void OnInstallClick(object sender, RoutedEventArgs e)
        {
            string path = InstallPath.Text;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string[] files = Directory.GetFiles(path);
            if (files.Length != 0)
            {
                MessageBoxResult result = MessageBox.Show(
                    "The specified directoy is not empty, installing nebula will wipe all the files within this directory, would you like to continue ?",
                    "Nebula Installer", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                    return;
                foreach (string file in files)
                    File.Delete(file);
            }

            InstallPath.IsEnabled = false;
            SelectPathButton.IsEnabled = false;
            ResetPathButton.IsEnabled = false;
            ((System.Windows.Controls.Button) sender).IsEnabled = false;
            InstallProgressBar.IsIndeterminate = true;
            InstallProgressBar.IsEnabled = true;
            DownloadNebula();
        }

        private void OnSelectPathClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            InstallPath.Text = dialog.SelectedPath;
        }

        private void OnResetPathClick(object sender, RoutedEventArgs e)
        {
            InstallPath.Text = DefaultPath;
        }
    }
}