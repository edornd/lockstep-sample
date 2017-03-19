using Game.Players;
using LiteNetLib.Utils;

namespace Game.Network {
    /// <summary>
    /// Abstract class for packets sending a player. See more specific packets.
    /// </summary>
    public abstract class PacketPlayer : PacketBase {

        protected Player player;

        public PacketPlayer() { }

        public PacketPlayer(int sender, NetPacketType type, Player player) : base(type, sender) {
            this.player = player;
        }

        public Player Player { get { return player; } }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
            player.Serialize(writer);
        }

        public override void Deserialize(NetDataReader reader) {
            type = GetPacketType();
            base.Deserialize(reader);
            player = new Player();
            player.Deserialize(reader);
        }

        public abstract NetPacketType GetPacketType();
    }
}

