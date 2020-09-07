using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ModernWpf.Controls;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.UI.Dialogs
{
    public class NebulaMessageBox : DialogDataContent
    {
        private static readonly DataContentCache Cache = DataContentCache.BuildCache<NebulaMessageBox>();

        public NebulaMessageBox(string title, string content, string primaryButton, string closeButton)
        {
            Title = title;
            PrimaryButtonText = primaryButton;
            CloseButtonText = closeButton;
            Content = content;
            Cache.PrepareFor(this);
        }

        [DataProperty(typeof(TextBlock))] public string Content { get; }

        public override DataContentCache GetCache()
        {
            return Cache;
        }

        public static async Task<ContentDialogResult> ShowYesNo(string titleKey, string content, params object[] format)
        {
            return await new NebulaMessageBox(NebulaClient.GetLocString(titleKey), NebulaClient.GetLocString(content, format),
                    NebulaClient.GetLocString("ButtonYes"), NebulaClient.GetLocString("ButtonNo"))
                .ShowDialogAsync();
        }

        public static async Task<ContentDialogResult> ShowOk(string titleKey, string content, params object[] format)
        {
            return await new NebulaMessageBox(NebulaClient.GetLocString(titleKey), NebulaClient.GetLocString(content, format), "",
                NebulaClient.GetLocString("ButtonOk")).ShowDialogAsync();
        }
    }
}