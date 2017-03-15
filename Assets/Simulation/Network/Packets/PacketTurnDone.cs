using LiteNetLib.Utils;

namespace Game.Network {
    public class PacketTurnDone : PacketBase {

        private ulong turnID;

        public PacketTurnDone() { }

        public PacketTurnDone(int sender, ulong turn) : base(NetPacketType.TurnDone, sender) {
            this.turnID = turn;
        }

        public ulong Turn { get { return turnID; } }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
            writer.Put(turnID);
        }

        public override void Deserialize(NetDataReader reader) {
            type = NetPacketType.TurnDone;
            base.Deserialize(reader);
            turnID = reader.GetULong();
        }
    }
}
