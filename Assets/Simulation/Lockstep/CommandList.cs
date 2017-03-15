using System.Collections.Generic;
using UnityEngine;

namespace Game.Lockstep {
    /// <summary>
    /// Class defining a command list, using a simple flag system to track
    /// done messages from every player.
    /// </summary>
    public class CommandList {

        private int readyFlag;
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
        public void AddCommand(CommandBase command, int source) {
            // if (command.Type == CommandType.Done) {
            //    readyFlag |= (int)(Mathf.Pow(2, source - 1));
            //}
            commands.Add(command);
        }

        public void Clear() {
            this.commands.Clear();
            readyFlag = 0;
        }

        /// <summary>
        /// Checks whether the currrent list has received a "done" message from all the clients.
        /// </summary>
        /// <returns>true if the flag is equal to a certain value, false otherwise</returns>
        public bool IsComplete() {
            //check the readyFlag for a certain value (based on the number of players).
            return readyFlag == CommandBuffer.ReadyValue;
        }

        public override string ToString() {
            string res = "[";
            foreach (CommandBase cmd in commands) {
                res += cmd.ToString();
                res += " ";
            }
            return res+"]";
        }
    }
}
