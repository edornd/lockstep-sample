using Game.Lockstep;
using System.Collections.Generic;
using LiteNetLib.Utils;

namespace Game.Network {
    public class PacketTurnData : PacketBase {

        private long turn;
        private List<CommandBase> commands;
        private int count;

        public PacketTurnData() { }

        public PacketTurnData(int sender, long turn, List<CommandBase> commands) : base(NetPacketType.TurnData, sender) {
            this.turn = turn;
            this.commands = commands;
            this.count = commands.Count;
        }

        public long Turn { get { return turn; } }

        public List<CommandBase> Commands { get { return commands; } }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
            writer.Put(turn);
            writer.Put(count);
            foreach(CommandBase cmd in commands) {
                cmd.Serialize(writer);
            }
        }

        public override void Deserialize(NetDataReader reader) {
            type = NetPacketType.TurnData;
            base.Deserialize(reader);
            turn = reader.GetLong();
            count = reader.GetInt();
            commands = new List<CommandBase>();
            for (int i = 0; i < count; i++) {
                CommandBase cmd = CommandBase.Read(reader);
                commands.Add(cmd);
            }
        }
    }
}