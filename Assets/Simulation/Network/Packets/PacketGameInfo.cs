using LiteNetLib.Utils;

namespace Game.Network {
    public class PacketGameInfo : PacketBase {

        private int data;

        public PacketGameInfo() { }

        public PacketGameInfo(int sender, int data) : base(NetPacketType.GameInfo, sender) {
            this.data = data;
        }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
            writer.Put(data);
        }

        public override void Deserialize(NetDataReader reader) {
            base.Deserialize(reader);
            data = reader.GetInt();
        }

    }
}
