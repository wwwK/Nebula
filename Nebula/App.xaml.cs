using System.IO;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Nebula.Core;

namespace Nebula
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStart(object sender, StartupEventArgs e)
        {
            AppCenter.Start("df3a859e-110a-43b2-892d-71f4650c9c70", typeof(Analytics), typeof(Crashes));

            bool justUpdated = false;
            bool justInstalled = false;

            foreach (string argument in e.Args)
            {
                if (argument == "/justUpdated")
                    justUpdated = true;
                else if (argument == "/justInstalled")
                    justInstalled = true;
            }

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            if (justUpdated || justInstalled)
            {
                foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                {
                    if (file.ToLower().Contains("update"))
                        File.Delete(file);
                }
            }
        }

        private void OnAppExit(object sender, ExitEventArgs e)
        {
            NebulaClient.Discord?.Exit();
            NebulaClient.MediaPlayer.Stop();
        }
    }
}