using System;
using DiscordRPC;
using Nebula.Core.Medias;

namespace Nebula.Core.Discord
{
    public class DiscordRpc
    {
        public DiscordRpc()
        {
            RpcClient = new DiscordRpcClient("740292732794306690");
            RpcClient.Initialize();
        }

        private DiscordRpcClient RpcClient { get; }

        public void Exit()
        {
            if (!RpcClient.IsInitialized)
                return;
            RpcClient.Deinitialize();
            RpcClient.Dispose();
        }

        public void InvokeClient()
        {
            if (RpcClient.IsInitialized)
                RpcClient.Invoke();
        }

        public void Set(IMediaInfo mediaInfo)
        {
            RichPresence richPresence = new RichPresence
            {
                Details = "Listening",
                State = mediaInfo.Title,
                Timestamps = Timestamps.FromTimeSpan(mediaInfo.Duration),
                Party = new Party {ID = "12654487486", Max = 5, Size = 1},
            };
            RpcClient.SetPresence(richPresence);
        }
    }
}