using LiteNetLib;

namespace Game.Network {
    public class NetMessage {
        private PacketBase packet;
        private NetPeer client;

        public NetMessage(PacketBase packet) : this(packet, null){ }

        public NetMessage(PacketBase packet, NetPeer client) {
            this.packet = packet;
            this.client = client;
        }

        public PacketBase Packet {
            get { return packet; }
        }

        public NetPeer Client {
            get { return client; }
        }
    }
}
