namespace Nebula.Core.UI.Content
{
    public class SimplePanelDataContent : BaseDataContent
    {
        public int Spacing { get; set; } = 10;

        public override DataContentCache GetCache()
        {
            return null;
        }
    }
}