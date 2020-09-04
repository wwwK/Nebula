using System.Windows.Controls;
using ModernWpf.Controls;
using Nebula.Core.UI.Content;
using Nebula.Core.UI.Content.Attributes;
using Nebula.Net.Packets.C2S;

namespace Nebula.Core.Dialogs
{
    public class SharedSessionRoomCreationDialog : DialogDataContent
    {
        private static readonly DataContentCache Cache = DataContentCache.BuildCache<SharedSessionRoomCreationDialog>();

        public SharedSessionRoomCreationDialog()
        {
            Title = NebulaClient.GetLocString("SharedSessionCreateRoom");
            PrimaryButtonText = NebulaClient.GetLocString("Create");
            Cache.PrepareFor(this);
        }

        [DataProperty(typeof(TextBox), "TextProperty", "SharedSessionRoomName")]
        public string RoomName { get; set; }

        [DataProperty(typeof(TextBox), "TextProperty", "SharedSessionRoomPassword")]
        public string RoomPassword { get; set; }

        [DataProperty(typeof(NumberBox), "ValueProperty", "SharedSessionRoomSize")]
        public int MaxUsers { get; set; }

        public override DataContentCache GetCache()
        {
            return Cache;
        }

        public override void OnPrimaryClick()
        {
            if (!NebulaClient.Network.IsConnected)
                return;
            SharedSessionCreationRequest request = new SharedSessionCreationRequest
            {
                Name = RoomName,
                Password = RoomPassword,
                Size = MaxUsers
            };
            NebulaClient.Network.SendPacket(request);
        }
    }
}