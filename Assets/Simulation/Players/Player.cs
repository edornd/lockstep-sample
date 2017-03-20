using LiteNetLib.Utils;
using Game.Network;

namespace Game.Players {
    public class Player : IEncodable {

        private int id;
        private string name;
        private bool ready;
        private bool active;

        #region Constructors

        public Player() : this(0,""){ }

        public Player(int playerID) : this(playerID, ""){ }

        public Player(string username) : this(0, username){ }

        public Player(int playerID, string playerName) {
            this.id = playerID;
            this.name = playerName;
            this.ready = false;
            this.active = true;
        }

        #endregion

        #region Accessors

        public int ID {
            get { return id; }
        }

        public string Name {
            get { return name; }
        }

        public bool Ready {
            get { return ready; }
        }

        public void SetID(int identifier) {
            this.id = identifier;
        }

        public void SetUsername(string name) {
            this.name = name;
        }

        public void SetReady(bool value) {
            this.ready = value;
        }

        public void SetActive(bool value) {
            this.active = value;
        }

        #endregion

        public void Serialize(NetDataWriter writer) {
            writer.Put(id);
            writer.Put(name);
            writer.Put(ready);
            writer.Put(active);
        }

        public void Deserialize(NetDataReader reader) {
            id = reader.GetInt();
            name = reader.GetString(50);
            ready = reader.GetBool();
            active = reader.GetBool();
        }

        public override bool Equals(object obj) {
            if (!(obj is Player))
                return false;
            return ((Player)obj).id == this.id;
        }

        public override int GetHashCode() {
            return id;
        }

        public override string ToString() {
            return "[" + id + "] " + name + ((ready) ? " (ready) " : "(not ready)");
        }
    }
}
