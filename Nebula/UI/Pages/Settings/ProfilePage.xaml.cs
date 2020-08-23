using System.Windows.Navigation;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.Settings.Groups;

namespace Nebula.UI.Pages.Settings
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            DataContext = NebulaClient.Settings.UserProfile;
        }
    }
}