using Game.Lockstep;
using System.Collections.Generic;
using LiteNetLib.Utils;

namespace Game.Network {
    public class PacketTurnData : PacketBase {

        private long turn;
        private List<Command> commands;
        private int count;

        public PacketTurnData() { }

        public PacketTurnData(int sender, long turn, List<Command> commands) : base(NetPacketType.TurnData, sender) {
            this.turn = turn;
            this.commands = commands;
            this.count = commands.Count;
        }

        public long Turn { get { return turn; } }

        public List<Command> Commands { get { return commands; } }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
            writer.Put(turn);
            writer.Put(count);
            foreach(Command cmd in commands) {
                cmd.Serialize(writer);
            }
        }

        public override void Deserialize(NetDataReader reader) {
            type = NetPacketType.TurnData;
            base.Deserialize(reader);
            turn = reader.GetLong();
            count = reader.GetInt();
            commands = new List<Command>();
            for (int i = 0; i < count; i++) {
                Command cmd = CommandFactory.Create(reader);
                cmd.Source = sender;
                commands.Add(cmd);
            }
        }
    }
}