using Game.Network;
using LiteNetLib.Utils;

namespace Game.Lockstep {
    /// <summary>
    /// Abstract class representing a generic command.
    /// </summary>
    public abstract class Command : ICommand, IEncodable{

        protected CommandType type;
        protected int source;

        #region Constructors

        public Command() { }

        public Command(CommandType type, int playerID) {
            this.type = type;
            this.source = playerID;
        }

        #endregion

        #region Properties

        public CommandType Type {
            get { return type; }
            set { type = value; }
        }

        public int Source {
            get { return source; }
            set { source = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Abstract function for the command execution.
        /// Every command has its own implementation.
        /// </summary>
        public abstract void Process();

        /// <summary>
        /// Serializes the generic command values (type and turn).
        /// </summary>
        /// <param name="writer">writer containing the byte buffer</param>
        public virtual void Serialize(NetDataWriter writer) {
            writer.Put((ushort)type);
        }

        /// <summary>
        /// Doesn't need to do anything, the type is already extracted on creation,
        /// source and turn are assigned fro mthe packet.
        /// </summary>
        /// <param name="reader">reader containing the byte buffer</param>
        public virtual void Deserialize(NetDataReader reader) {
        }

        #endregion
    }
}
