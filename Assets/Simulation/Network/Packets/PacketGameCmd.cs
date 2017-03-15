using Game.Lockstep;
using LiteNetLib.Utils;

namespace Game.Network {
    public class PacketGameCmd : PacketBase {

        private CommandBase command;

        public PacketGameCmd() { }

        public PacketGameCmd(int sender, CommandBase command) : base(NetPacketType.GameCmd, sender) {
            this.command = command;
        }

        public CommandBase Command {
            get { return command; }
        }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
            command.Serialize(writer);
        }

        public override void Deserialize(NetDataReader reader) {
            type = NetPacketType.GameCmd;
            base.Deserialize(reader);
            command = CommandBase.Read(reader);
        }

    }
}
