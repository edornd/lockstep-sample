using System.Collections.Generic;

namespace Game.Lockstep {
    /// <summary>
    /// Class defining a command list, using a simple flag system to track
    /// done messages from every player.
    /// </summary>
    public class CommandList {

        private short readyFlag;
        private List<CommandBase> commands;

        public CommandList() {
            readyFlag = 0;
            commands = new List<CommandBase>();
        }

        /// <summary>
        /// Adds a new command to the list. If the command type is equal to "done",
        /// the relative player's flag is set.
        /// </summary>
        /// <param name="command"></param>
        public void AddCommand(CommandBase command) {
            if (command.Type == CommandType.Done) {
                //set flag of the given player using cmd.source
            }
            else {
                commands.Add(command);
            }
        }

        /// <summary>
        /// Checks whether the currrent list has received a "done" message from all the clients.
        /// </summary>
        /// <returns>true if the flag is equal to a certain value, false otherwise</returns>
        public bool IsComplete() {
            //check the readyFlag for a certain value
            return false;
        }
    }
}
