using LiteNetLib.Utils;
using System;

namespace Game.Lockstep {
    public class CommandTest : CommandBase {

        public override void Process() {
            UnityEngine.Debug.Log("wowo sun el cumand! test al turn: " + Turn);
        }

        public override void Serialize(NetDataWriter writer) {
            base.Serialize(writer);
        }

        public override void Deserialize(NetDataReader writer) {
            base.Deserialize(writer);
        }
    }
}
