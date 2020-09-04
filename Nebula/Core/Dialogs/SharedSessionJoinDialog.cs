using System.Windows.Controls;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.Dialogs
{
    public class SharedSessionJoinDialog : DialogDataContent
    {
        private static readonly DataContentCache Cache = DataContentCache.BuildCache<SharedSessionJoinDialog>();

        public SharedSessionJoinDialog()
        {
            Title = NebulaClient.GetLocString("SharedSessionCreateRoom");
            PrimaryButtonText = NebulaClient.GetLocString("SharedSessionJoin");
            Cache.PrepareFor(this);
        }

        [DataProperty(typeof(TextBox), "TextProperty", "SharedSessionRoomPassword")]
        public string RoomPassword { get; set; }


        public override DataContentCache GetCache()
        {
            return Cache;
        }
    }
}