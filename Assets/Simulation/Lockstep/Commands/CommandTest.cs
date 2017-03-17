using LiteNetLib.Utils;
using System;

namespace Game.Lockstep {
    public class CommandTest : Command {

        public CommandTest() { }

        public CommandTest(int playerID) : base(CommandType.Test, playerID) { }

        public override void Process() {
            UnityEngine.Debug.Log("wowo son el cumand di Playah: " + Source);
        }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
        }

        public override void Deserialize(NetDataReader reader) {
            type = CommandType.Test;
            base.Deserialize(reader);
        }
    }
}
