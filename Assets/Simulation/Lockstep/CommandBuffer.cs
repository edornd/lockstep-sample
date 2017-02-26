using UnityEngine;

namespace Game.Lockstep {
    /// <summary>
    /// Class containing the command lists for the current and future turns.
    /// </summary>
    public class CommandBuffer {

        private CommandList[] commands;
        private int size;

        public CommandBuffer(int bufferSize) {
            this.size = bufferSize;
            this.commands = new CommandList[size];
            for (uint i = 0; i < size; i++) {
                commands[i] = new CommandList();
            }
        }

        /// <summary>
        /// Inserts the given command in the correct list, checking the scheduled turn ID.
        /// </summary>
        /// <param name="command">command to schedule</param>
        public void Put(CommandBase command) {
            int offset =(int)(LockstepLogic.CurrentTurn - command.Turn);
            if (offset < 0) {
                Debug.LogWarning("Turn " + command.Turn + " already processed!");
            }
            else if (offset >= size) {
                Debug.LogWarning("Turn too far ahead!");
            }
            else {
                commands[offset].AddCommand(command);
            }
        }

        /// <summary>
        /// Checks whether the first list (representing the current turn) is complete
        /// and ready to be processed.
        /// </summary>
        /// <returns>true if the list is complete, false otherwise</returns>
        public bool IsReady() {
            return commands[0].IsComplete();
        }
    }
}
