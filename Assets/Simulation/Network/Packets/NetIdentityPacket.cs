using Game.Network.Players;
using LiteNetLib.Utils;

namespace Game.Network {
    public class NetIdentityPacket : PacketBase {

        private int identifier;

        public NetIdentityPacket() { }

        public NetIdentityPacket(NetPacketType type, int sender, int id) : base(type, sender) {
            this.identifier = id;
        }

        public int Identifier {
            get { return identifier; }
        }

        public override void Serialize(NetDataWriter writer) {
            type = NetPacketType.PeerIdentity;
            base.Serialize(writer);
            writer.Put(identifier);
        }

        public override void Deserialize(NetDataReader reader) {
            base.Deserialize(reader);
            identifier = reader.GetInt();
        }
    }
}

