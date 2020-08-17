using System.Windows.Navigation;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Settings.Groups;

namespace Nebula.Pages.Settings
{
    public partial class GeneralPage : Page
    {
        public GeneralPage()
        {
            InitializeComponent();
            DataContext = NebulaClient.Settings.General;
        }
    }
}