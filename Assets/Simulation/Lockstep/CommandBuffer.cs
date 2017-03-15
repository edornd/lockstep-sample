using UnityEngine;

namespace Game.Lockstep {
    /// <summary>
    /// Class containing the command lists for the current and future turns.
    /// </summary>
    public class CommandBuffer {

        private static int readyValue;
        private CommandList[] commands;
        private int size;

        public CommandBuffer(int bufferSize, int numPlayers) {
            this.size = bufferSize;
            this.commands = new CommandList[size];
            for (uint i = 0; i < commands.Length; i++) {
                commands[i] = new CommandList();
            }
            readyValue = (int)(Mathf.Pow(2,numPlayers)) - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        public static int ReadyValue { get { return readyValue; } }

        /// <summary>
        /// Inserts the given command in the correct list, checking the scheduled turn ID.
        /// </summary>
        /// <param name="command">command to schedule</param>
        public void Insert(CommandBase command, int source) {
            int offset =(int)(command.Turn - LockstepLogic.CurrentTurn);
            if (offset < 0) {
                Debug.LogWarning("Turn " + command.Turn + " l'e gio' pasa'!");
            }
            else if (offset >= size) {
                Debug.LogWarning("Trop luntan!");
            }
            else {
                commands[offset].AddCommand(command, source);
            }
        }

        /// <summary>
        /// Shifts the command buffer to the left, returning the commands stored in the first list.
        /// </summary>
        /// <returns>commands for the current turn</returns>
        public CommandList Advance() {
            CommandList current = commands[0];
            for (int i = 0; i < commands.Length-1; i++) {
                commands[i] = commands[i + 1];
            }
            commands[commands.Length-1] = new CommandList();
            return current;
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
