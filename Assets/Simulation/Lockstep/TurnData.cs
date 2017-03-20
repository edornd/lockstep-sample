using System;
using System.Collections.Generic;

namespace Game.Lockstep {
    /// <summary>
    /// Class defining a command list, using a simple flag system to track
    /// done messages from every player.
    /// </summary>
    public class TurnData {

        private List<Command>[] commandLists;
        private int count;

        /// <summary>
        /// Constructor that initializes the turn data using the given size.
        /// </summary>
        /// <param name="size">size of the buffer, usually the number of players</param>
        public TurnData(int size) {
            commandLists = new List<Command>[size];
            this.count = 0;
        }

        /// <summary>
        /// Gets the number of players who already sent their commands.
        /// </summary>
        public int Count { get { return count; } }

        /// <summary>
        /// Gets the total size of the turn data.
        /// </summary>
        public int Size { get { return commandLists.Length; } }

        /// <summary>
        /// Gets the commands array.
        /// </summary>
        public List<Command>[] TurnCommands {  get { return commandLists; } }

        /// <summary>
        /// Adds the list of commands to the buffer.
        /// </summary>
        /// <param name="commands">list to be added</param>
        /// <param name="source">player who sent it</param>
        public void Insert(List<Command> commands, int source) {
            int pos = source - 1;
            if (commandLists[pos] == null) {
                commandLists[pos] = commands;
                count++;
            }
        }

        /// <summary>
        /// Simply checks if the current data count is equal to the maximum size
        /// (we received everything).
        /// </summary>
        /// <returns>true if it0s equal, false otherwise</returns>
        public bool IsCompleted() {
            return count == commandLists.Length;
        }

        /// <summary>
        /// Clears the command dictionary.
        /// </summary>
        public void Clear() {
            Array.Clear(commandLists, 0, commandLists.Length);
            count = 0;
        }

        /// <summary>
        /// Iterates through the commands, processing them.
        /// </summary>
        public void ProcessCommands() {
            for (int i = 0; i < commandLists.Length; i++) {
                foreach(Command command in commandLists[i]){
                    command.Process();
                }
            }
        }

        /// <summary>
        /// String used to debug the command list.
        /// </summary>
        /// <returns>string representing the turn data</returns>
        public override string ToString() {
            string res = "{";
            for (int i = 0; i < commandLists.Length; i++) {
                res += "[ " + commandLists[i] + " ]\n";
            }
            return res + "}";
        }
    }
}
