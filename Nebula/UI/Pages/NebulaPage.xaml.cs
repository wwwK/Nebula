using System.Windows;
using System.Windows.Navigation;
using Nebula.Core.UI.Content;
using Page = ModernWpf.Controls.Page;

namespace Nebula.UI.Pages
{
    public partial class NebulaPage : Page
    {
        public NebulaPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!(e.ExtraData is SimplePanelDataContent dataContent))
                return;
            DataContent = dataContent;
            Panel.Spacing = dataContent.Spacing;
            if (dataContent.MaxWidth > 0)
                Panel.MaxWidth = dataContent.MaxWidth;
            foreach (FrameworkElement element in dataContent.GetCache().GetFrameworkElements())
                Panel.Children.Add(element);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Panel.Children.Clear();
            DataContent = null;
        }

        private SimplePanelDataContent DataContent { get; set; }
    }
}