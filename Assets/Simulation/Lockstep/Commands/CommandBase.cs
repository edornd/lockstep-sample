using Game.Network;
using LiteNetLib.Utils;

namespace Game.Lockstep {

    public abstract class CommandBase : ICommand, IEncodable{

        protected CommandType type;
        protected ulong turn;

        #region Constructors

        public CommandBase() { }

        public CommandBase(CommandType type, ulong turn) {
            this.type = type;
            this.turn = turn;
        }

        #endregion

        #region Properties

        public CommandType Type {
            get { return type; }
            set { type = value; }
        }

        public ulong Turn {
            get { return turn; }
            set { turn = value; }
        }

        #endregion

        #region Public methods

        public abstract void Process();

        public virtual void Serialize(NetDataWriter writer) {
            writer.Put((ushort)type);
            writer.Put(turn);
        }

        public virtual void Deserialize(NetDataReader reader) {
            turn = reader.GetULong();
        }

        #endregion

        #region Static reader

        public static T Read<T>(NetDataReader reader) where T : CommandBase, new() {
            T command = new T();
            command.Deserialize(reader);
            return command;
        }

        public static CommandBase Read(NetDataReader reader) {
            CommandType type = (CommandType)(reader.GetUShort());
            switch (type) {
                case CommandType.Test:
                    return Read<CommandTest>(reader);
                default:
                    UnityEngine.Debug.Log("L'é sciapase tut! Sei nen co l'é: " + type);
                    return null;
            }
        }

        #endregion
    }
}
