using Game.Network.Players;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace Game.Network {
    public class PlayerInfoPacket : PacketBase {

        private List<Player> infos;
        private int count;

        public PlayerInfoPacket() {
            this.infos = new List<Player>();
            count = 0;
        }

        public PlayerInfoPacket(NetPacketType type, int sender) : base(type, sender) {
            this.infos = new List<Player>();
            count = 0;
        }

        public PlayerInfoPacket(NetPacketType type, int sender, Player info) : base(type, sender) {
            this.infos = new List<Player>();
            this.infos.Add(info);
            count = 1;
        }

        public List<Player> Players {
            get { return infos; }
        }

        public int Count {
            get { return count; }
        }

        public void AddPlayer(Player info) {
            this.infos.Add(info);
            count++;
        }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
            writer.Put(count);
            foreach (Player p in infos) {
                p.Serialize(writer);
            }
        }

        public override void Deserialize(NetDataReader reader) {
            type = NetPacketType.PeerInfo;
            base.Deserialize(reader);
            count = reader.GetInt();
            for (int i = 0; i < count; i++) {
                Player current = new Player();
                current.Deserialize(reader);
                infos.Add(current);
            }
        }
    }
}
