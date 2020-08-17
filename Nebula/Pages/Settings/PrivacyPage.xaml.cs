
using ModernWpf.Controls;
using Nebula.Core;

namespace Nebula.Pages.Settings
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