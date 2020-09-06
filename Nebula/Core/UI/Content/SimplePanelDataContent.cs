using System;
using System.Text.Json.Serialization;

namespace Nebula.Core.UI.Content
{
    public class SimplePanelDataContent : BaseDataContent
    {
        [JsonIgnore] public int Spacing  { get; set; } = 10;
        [JsonIgnore] public int MaxWidth { get; set; } = -1;

        public override DataContentCache GetCache()
        {
            throw new NotImplementedException();
        }
    }
}