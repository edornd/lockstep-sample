using Game.Players;

namespace Game.Network {
    public class PacketPlayerEnter : PacketPlayer {

        public PacketPlayerEnter() { }

        public PacketPlayerEnter(int sender, Player player) : base(sender, NetPacketType.PlayerEnter, player) { }

        public override NetPacketType GetPacketType() {
            return NetPacketType.PlayerEnter;
        }
    }
}
