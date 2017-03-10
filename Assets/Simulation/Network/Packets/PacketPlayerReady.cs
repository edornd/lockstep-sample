using LiteNetLib.Utils;

namespace Game.Network {
    public class PacketPlayerReady : PacketBase {

        private bool value;

        public PacketPlayerReady() { }

        public PacketPlayerReady(int sender, bool value) : base(NetPacketType.PlayerReady, sender) {
            this.value = value;
        }

        public bool Value {  get { return value; } }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
            writer.Put(value);
        }

        public override void Deserialize(NetDataReader reader) {
            type = NetPacketType.PlayerReady;
            base.Deserialize(reader);
            value = reader.GetBool();
        }
    }
}
