namespace Presentation.Network {
    /// <summary>
    /// Abstract class representing a generic state of the server.
    /// The class offers basic functionality to handle net events inding/unbinding
    /// and a simple Next method to advance to the next state.
    /// </summary>
    public abstract class ServerState {
        protected GameServer server;

        /// <summary>
        /// Every state uses the server instance to handle and send messages.
        /// </summary>
        /// <param name="server">game server instance</param>
        public ServerState(GameServer server) {
            this.server = server;
        }

        /// <summary>
        /// Called once instantiated, used to register to the desired network packet types.
        /// </summary>
        public abstract void Enable();

        /// <summary>
        /// Called before transitioning to the next state, used to unbind the delegates from the
        /// relative packet types.
        /// </summary>
        public abstract void Disable();

        public abstract ServerState Next();
    }
}
