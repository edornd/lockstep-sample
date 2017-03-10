using Game.Utils;
using System.Collections.Generic;

namespace Game.Players {
    public class PlayersList : IResettable {

        private Dictionary<int, Player> players;

        public PlayersList() {
            this.players = new Dictionary<int, Player>();
        }

        public Dictionary<int, Player> Players {
            get { return players; }
        }

        public void AddPlayer(Player player) {
            players[player.ID] = player;
        }

        public bool RemovePlayer(Player player) {
            return players.Remove(player.ID);
        }

        public void Reset() {
            players.Clear();
        }
    }
}
