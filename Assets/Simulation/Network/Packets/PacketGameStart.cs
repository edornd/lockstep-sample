using LiteNetLib.Utils;

namespace Game.Network {
    public class PacketGameStart : PacketBase {

        public PacketGameStart() { }

        public PacketGameStart(int sender) : base(NetPacketType.GameStart, sender) { }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
        }

        public override void Deserialize(NetDataReader reader) {
            base.Deserialize(reader);
        }
    }
}
