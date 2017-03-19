using LiteNetLib.Utils;

namespace Game.Network {
    /// <summary>
    /// Packet used to wrap unprocessed serialized data and forward it, without wasting time deserializing it.
    /// This class should only be used inside the GameServer environment, since it does not provide a deserialize function.
    /// </summary>
    public class PacketRawData : PacketBase {
        private byte[] data;

        public PacketRawData(byte[] data) : base() {
            this.data = data;
        }

        public override void Serialize(NetDataWriter writer) {
            writer.Put(data);
        }
    }
}
