using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using ModernWpf.Controls;
using Nebula.Core;
using Page = ModernWpf.Controls.Page;

namespace Nebula.UI.Pages.Settings
{
    public partial class AppearancePage : Page
    {
        public AppearancePage()
        {
            InitializeComponent();
            DataContext = NebulaClient.Settings.Appearance;
            foreach (Stretch value in Enum.GetValues(typeof(Stretch)))
                BackgroundImageCmb.Items.Add(value.ToString());
        }

        private void OnBackgroundImageMouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Multiselect = false
            };

            if (dialog.ShowDialog() != true || !File.Exists(dialog.FileName))
                return;
            if (sender is TextBox textBox)
            {
                textBox.Text = dialog.FileName;
                NebulaClient.Settings.Appearance.BackgroundImage = dialog.FileName;
            }
        }

        private void OnBackgroundImageKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                NebulaClient.Settings.Appearance.BackgroundImage = textBox.Text;
            }
        }
    }
}