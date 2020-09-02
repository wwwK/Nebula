using LiteNetLib;

namespace Nebula.Core.Networking
{
    public class NebulaNetClient
    {
        public NebulaNetClient()
        {
            ClientListener = new EventBasedNetListener();
            Client = new NetManager(ClientListener){UnsyncedEvents = true};
        }

        public NetManager            Client         { get; }
        public EventBasedNetListener ClientListener { get; }

        public void Connect()
        {

        }
    }
}