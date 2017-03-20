using System.Collections.Generic;

namespace Game.Lockstep {
    /// <summary>
    /// Class containing the command lists for the current and future turns.
    /// </summary>
    public class CommandBuffer {

        private TurnData[] turnCommands;
        private int bufferSize;
        private int numPlayers;
        private int position;

        private object bufferLock;
        private object countLock;

        public CommandBuffer(int lookAhead, int numPlayers) {
            this.bufferSize = lookAhead;
            this.numPlayers = numPlayers;
            this.turnCommands = new TurnData[this.bufferSize];
            for (uint i = 0; i < bufferSize; i++) {
                turnCommands[i] = new TurnData(numPlayers);
            }
            position = 0;
            bufferLock = new object();
            countLock = new object();
        }

        /// <summary>
        /// Safely access to the current number of players.
        /// </summary>
        public int NumPlayers {
            get {
                lock(countLock) {
                    return numPlayers;
                }
            }
        }

        /// <summary>
        /// Updates numPlayers to the new value, and inserts empty dummy data for the disconnected
        /// player, in order to keep the turns going.
        /// </summary>
        /// <param name="activePlayers">new player count</param>
        /// <param name="player">id of the disconnected user</param>
        public void SetPlayersCount(int activePlayers, int player) {
            lock (countLock) {
                int oldCount = numPlayers;
                numPlayers = activePlayers;

                lock (bufferLock) {
                    for (int i = 0; i < bufferSize ; i++) {
                        if (!turnCommands[i].IsCompleted() && turnCommands[i].Size == oldCount) {
                            turnCommands[i].Insert(new List<Command>(), player);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inserts the given command in the correct list, checking the scheduled turn ID.
        /// </summary>
        /// <param name="command">command to schedule</param>
        public void Insert(List<Command> commands, long scheduledTurn, int source) {
            lock (bufferLock) {
                int offset = (int)scheduledTurn % bufferSize;
                if (!turnCommands[offset].IsCompleted()) {
                    turnCommands[offset].Insert(commands, source);
                }
            }
        }

        /// <summary>
        /// Shifts the command buffer to the left, returning the commands stored in the first list.
        /// </summary>
        /// <returns>commands for the current turn</returns>
        public TurnData Advance() {
            lock (bufferLock) {
                TurnData current = turnCommands[position];
                turnCommands[position] = new TurnData(NumPlayers);
                position = (position + 1) % bufferSize;
                return current;
            }
        }

        /// <summary>
        /// Check if the buffer is ready to advance to the next turn.
        /// </summary>
        /// <returns></returns>
        public bool IsReadyToAdvance() {
            lock (bufferLock) {
                UnityEngine.Debug.Log(turnCommands[position]);
                return turnCommands[position].IsCompleted();
            }
        }
    }
}
