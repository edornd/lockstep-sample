using Game.Players;

namespace Game.Network {
    public class PacketPlayerLeave : PacketPlayer {

        public PacketPlayerLeave() { }

        public PacketPlayerLeave(int sender, Player player) : base(sender, NetPacketType.PlayerLeave, player) { }

        public override NetPacketType GetPacketType() {
            return NetPacketType.PlayerLeave;
        }
    }
}

