using LiteNetLib;

namespace Nebula.Core.Networking
{
    public class NebulaNetWorker
    {
        public NebulaNetWorker()
        {
            ClientListener = new EventBasedNetListener();
            Client = new NetManager(ClientListener){UnsyncedEvents = true};
            
            Client.Start();
        }

        public NetManager            Client         { get; }
        public EventBasedNetListener ClientListener { get; }
    }
}