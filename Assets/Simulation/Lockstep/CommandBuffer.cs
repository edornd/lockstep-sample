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

        public CommandBuffer(int lookAhead, int numPlayers) {
            this.bufferSize = lookAhead*2;
            this.numPlayers = numPlayers;
            this.turnCommands = new TurnData[this.bufferSize];
            for (uint i = 0; i < bufferSize; i++) {
                turnCommands[i] = new TurnData(numPlayers);
            }
            position = 0;
            bufferLock = new object();
        }

        /// <summary>
        /// Inserts the given command in the correct list, checking the scheduled turn ID.
        /// </summary>
        /// <param name="command">command to schedule</param>
        public void Insert(List<Command> commands, long scheduledTurn, int source) {
            lock (bufferLock) {
                int offset = (int)scheduledTurn % bufferSize;
                if (turnCommands[offset].Count < numPlayers) {
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
                turnCommands[position] = new TurnData(numPlayers);
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
                return turnCommands[position].Count == numPlayers;
            }
        }
    }
}
