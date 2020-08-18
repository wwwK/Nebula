using System;
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
            foreach (NavigationViewPaneDisplayMode value in Enum.GetValues(typeof(NavigationViewPaneDisplayMode)))
                DisplayModeCmb.Items.Add(value.ToString());
        }
    }
}