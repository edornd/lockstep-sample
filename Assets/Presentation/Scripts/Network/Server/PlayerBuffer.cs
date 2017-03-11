using Game.Utils;
using Game.Players;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Presentation.Network {
    public class PlayerBuffer : IEnumerable<Player>, IResettable {

        private int maxSize;
        private int count;
        private Player[] playerBuffer;

        public PlayerBuffer(int maxSize) {
            this.maxSize = maxSize;
            this.playerBuffer = new Player[maxSize];
            count = 0;
        }

        public int Count {  get { return count; } }

        public void Add(Player player) {
            if (player == null)
                return;

            int pos = player.ID - 1;
            if (pos >= 0) {
                playerBuffer[pos] = player;
                count++;
            }
        }

        public Player Get(int identifier) {
            return playerBuffer[identifier-1];
        }

        public bool TryGetValue(int identifier, out Player player) {
            int position = identifier - 1;
            player = null;
            if (playerBuffer[position] != null) {
                player = playerBuffer[position];
                return true;
            }
            return false;
        }

        public bool Remove(Player player) {
            if (player == null)
                return false;
            int pos = player.ID - 1;
            if (pos >= 0) {
                playerBuffer[pos] = null;
                count--;
                return true;
            }
            return false;
        }

        public bool FirstEmptyID(out int identifier) {
            identifier = 0;
            for (int i = 0; i < maxSize; i++) {
                if (playerBuffer[i] == null) {
                    identifier = i+1;
                    return true;
                }
            }
            return false;
        }

        public bool IsEmpty() {
            return count == 0;
        }

        public bool IsFull() {
            return count == maxSize;
        }

        public void Reset() {
            this.playerBuffer = new Player[maxSize];
            count = 0;
        }

        public bool ReadyToStart() {
            for (int i = 0; i < maxSize; i++) {
                if (playerBuffer[i] != null) {
                    if (!playerBuffer[i].Ready)
                        return false;
                }
            }
            return true;
        }

        public IEnumerator<Player> GetEnumerator() {
            for (int i = 0; i < maxSize; i++) {
                if (playerBuffer[i] != null)
                    yield return playerBuffer[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
