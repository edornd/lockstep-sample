using System;
using System.Collections.Generic;
using UnityEngine;

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
            count = 0;
        }

        /// <summary>
        /// Gets the number of players who already sent their commands.
        /// </summary>
        public int Count { get { return count; } }

        public List<Command>[] TurnCommands {  get { return commandLists; } }

        /// <summary>
        /// Adds the list of commands to the buffer.
        /// </summary>
        /// <param name="commands">list to be added</param>
        /// <param name="source">player who sent it</param>
        public void Insert(List<Command> commands, int source) {
            commandLists[source - 1] = commands;
            count++;
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
    }
}
