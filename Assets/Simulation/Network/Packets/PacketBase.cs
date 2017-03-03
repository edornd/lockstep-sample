using LiteNetLib.Utils;

namespace Game.Network {
    /// <summary>
    /// Simple abstract class defining a packet structure.
    /// </summary>
    public abstract class PacketBase : IEncodable{

        protected NetPacketType type;
        protected int sender;

        #region constructors

        public PacketBase() { }

        public PacketBase(NetPacketType type, int sender) {
            this.type = type;
            this.sender = sender;
        }

        #endregion

        #region properties

        public NetPacketType Type {
            get { return type; }
            set { type = value; }
        }

        public int Sender {
            get { return sender; }
            set { sender = value; }
        }

        #endregion

        #region virtual base functions

        /// <summary>
        /// Converts this packet into a byte array using the given writer.
        /// This method should be overridden in order to serialize custom fields.
        /// </summary>
        /// <param name="writer">the writer containing the buffer</param>
        public virtual void Serialize(NetDataWriter writer) {
            writer.Put((ushort)type);
            writer.Put(sender);
        }

        /// <summary>
        /// Extracts the packed data from the reader.
        /// This method should be overridden to deserialize custom fields.
        /// </summary>
        /// <param name="reader">the reader containing the buffer</param>
        public virtual void Deserialize(NetDataReader reader) {
            sender = reader.GetInt();
        }

        #endregion

        #region Static reader

        public static T Read<T> (NetDataReader reader) where T : PacketBase, new() {
            T packet = new T();
            packet.Deserialize(reader);
            return packet; 
        }

        #endregion
    }
}
