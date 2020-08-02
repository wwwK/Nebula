using System.IO;
using System.Windows;
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
            bool justUpdated = false;

            foreach (string argument in e.Args)
            {
                if (argument == "-justUpdated")
                    justUpdated = true;
            }

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            if (justUpdated)
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
            NebulaClient.MediaPlayer.Stop();
        }
    }
}