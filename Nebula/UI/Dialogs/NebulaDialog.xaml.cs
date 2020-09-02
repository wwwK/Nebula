using System.Windows;
using System.Windows.Input;
using ModernWpf.Controls;
using Nebula.Core;
using Nebula.Core.UI.Content;

namespace Nebula.UI.Dialogs
{
    public partial class NebulaDialog : ContentDialog
    {
        public NebulaDialog(DialogDataContent dialog)
        {
            InitializeComponent();
            DialogDataContent = dialog;
            Title = dialog.Title;
            PrimaryButtonText = dialog.PrimaryButtonText;
            SecondaryButtonText = dialog.SecondaryButtonText;
            CloseButtonText = dialog.CloseButtonText;
            DialogPanel.Spacing = dialog.Spacing;
            Closing += OnClosing;
            PrimaryButtonClick += (sender, e) => dialog.OnPrimaryClick();
            SecondaryButtonClick += (sender, e) => dialog.OnSecondaryClick();
            CloseButtonClick += (sender, e) => dialog.OnCloseClick();
            foreach (FrameworkElement element in dialog.GetCache().GetFrameworkElements())
                DialogPanel.Children.Add(element);
        }

        private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            DialogPanel.Children.Clear();
        }

        private DialogDataContent DialogDataContent { get; }
    }
}