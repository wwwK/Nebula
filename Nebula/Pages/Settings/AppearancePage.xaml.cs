using ModernWpf.Controls;
using Nebula.Core;

namespace Nebula.Pages.Settings
{
    public partial class AppearancePage : Page
    {
        public AppearancePage()
        {
            InitializeComponent();
            DataContext = NebulaClient.Settings.Appearance;
        }
    }
}