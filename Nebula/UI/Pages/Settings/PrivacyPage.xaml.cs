
using ModernWpf.Controls;
using Nebula.Core;

namespace Nebula.UI.Pages.Settings
{
    public partial class PrivacyPage : Page
    {
        public PrivacyPage()
        {
            InitializeComponent();
            DataContext = NebulaClient.Settings.Privacy;
        }
    }
}