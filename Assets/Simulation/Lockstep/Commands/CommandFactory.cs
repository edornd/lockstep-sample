using LiteNetLib.Utils;
using System;

namespace Game.Lockstep {
    public class CommandFactory {

        /// <summary>
        /// Dynamically creates a new command. The function reads the command enum and tries to 
        /// find the corresponding Command subclass type. If the GetType is successful, the Activator
        /// generates an instance of the specific type and the new object calls its own Deserialize to exctrat
        /// the data from the network buffer.
        /// </summary>
        /// <param name="reader">data reader containing the network buffer</param>
        /// <returns>new command instance</returns>
        public static Command Create(NetDataReader reader) {
            CommandType commandType = (CommandType)reader.GetUShort();
            string className = "Command" + commandType.ToString();

            //name of each command class needs to match the corresponding enum, e.g.: 'Test' -> 'CommandTest'
            Type type = Type.GetType(typeof(Command).Namespace + "." + className, false);
            if (type == null) {
                throw new InvalidOperationException("Cannot find the command type '" + className + "'");
            }
            else if (!typeof(Command).IsAssignableFrom(type)) {
                throw new InvalidOperationException("The type '" + type.Name + "' does not inherit from Command");
            }
            Command command = (Command)Activator.CreateInstance(type);
            command.Deserialize(reader);
            return command;
        } 
    }
}
