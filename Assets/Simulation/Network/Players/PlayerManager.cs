using System.Collections.Generic;

namespace Game.Network.Players {
    public class PlayerManager {

        private static PlayerManager instance = new PlayerManager();

        private Dictionary<int, Player> players;
        private Player myIdentity;

        public PlayerManager() {
            this.players = new Dictionary<int, Player>();
        }

        public static PlayerManager Instance {
            get { return instance; }
        }

        public Player Identity {
            get { return myIdentity; }
        }

        public Dictionary<int, Player> Players {
            get { return players; }
        }

        public void SetIdentity(Player info) {
            this.myIdentity = info;
        }

        public void AddPlayer(Player info) {
            players[info.ID] = info;
        }

        public bool RemovePlayer(Player info) {
            return players.Remove(info.ID);
        }
    }
}
