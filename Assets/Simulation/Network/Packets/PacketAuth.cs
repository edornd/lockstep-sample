using Game.Players;
using LiteNetLib.Utils;

namespace Game.Network {
    public class PacketAuth : PacketBase {

        private string message;
        private int id;
        private int length;

        public PacketAuth() { }

        public PacketAuth(int sender, string message, int id) : base(NetPacketType.PeerAuth, sender) {
            this.message = message;
            this.id = id;
            this.length = message.Length;
        }

        public string Message { get { return message; } }

        public int ID { get { return id; } }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
            writer.Put(length);
            writer.Put(message);
            writer.Put(id);
        }

        public override void Deserialize(NetDataReader reader) {
            type = NetPacketType.PeerAuth;
            base.Deserialize(reader);
            length = reader.GetInt();
            message = reader.GetString(length);
            id = reader.GetInt();
        }
    }
}

