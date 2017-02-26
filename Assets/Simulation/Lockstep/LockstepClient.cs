using Game.Network;
using LiteNetLib.Utils;

namespace Game.Lockstep {
    /// <summary>
    /// Higher level client class, using a NetClient for communication.
    /// </summary>
    public class LockstepClient : EntityBase {

        #region Variables

        private NetClient baseClient;
        private string hostIP;
        private int hostPort;

        #endregion

        #region Constructors

        public LockstepClient(int listenPort, string hostIP, int hostPort) : base() {
            this.baseClient = new NetClient(listenPort, 15, "testkey");
            this.hostIP = hostIP;
            this.hostPort = hostPort;
        }

        #endregion

        #region Game behaviour

        public override void Init() {
            baseClient.Bind(NetPacketType.GameReady, OnClientReady);
            baseClient.Bind(NetPacketType.GameStart, OnGameStart);
            baseClient.Bind(NetPacketType.GameCmd, OnCommandReceived);
            baseClient.Connect(hostIP, hostPort);
        }

        public override void Update() {
            baseClient.Receive();
            baseClient.SendMessages();
        }

        public override void Quit() {
            baseClient.Disconnect();
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Received a "ready" message from another player.
        /// Every player ready does not imply the game should start, the client
        /// needs to wait for the server's "game start" signal.
        /// </summary>
        /// <param name="reader">raw packet data</param>
        private void OnClientReady(NetDataReader reader) {
            //todo...
        }

        /// <summary>
        /// Received a "start" message from the server.
        /// </summary>
        /// <param name="reader">raw packet data</param>
        private void OnGameStart(NetDataReader reader) {
            //todo...
        }

        /// <summary>
        /// Received a command from another player.
        /// </summary>
        /// <param name="reader">raw packet data</param>
        private void OnCommandReceived(NetDataReader reader) {
            //unpack command
            //send to main ls manager
        }

        #endregion
    }
}
