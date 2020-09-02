using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Nebula.Core;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Controls;

namespace Nebula.UI.Controls
{
    public partial class PathSelectorControl : UserControl, IDataControlsContainer
    {
        public PathSelectorControl(params object[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
                Filter = args[0] as string;
        }

        private string Filter { get; }

        public FrameworkElement GetBindableElement()
        {
            return TextContent;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = Filter,
                Multiselect = false
            };

            if (dialog.ShowDialog() != true || !File.Exists(dialog.FileName))
                return;
            TextContent.Text = dialog.FileName;
            TextContent.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        }
    }
}