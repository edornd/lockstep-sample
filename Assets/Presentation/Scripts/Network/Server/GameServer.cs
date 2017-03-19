using Game.Network;
using Game.Players;
using Game.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace Presentation.Network {
    public class GameServer : SingletonMono<GameServer> {

        public static readonly int DEFAULT_PORT = 28960;

        public int port = DEFAULT_PORT;
        public int maxConnections = 8;
        private NetServer baseServer;

        private int serverID;
        private PlayerBuffer players;
        private Dictionary<NetPeer, int> clients;
        private ServerState currentState;

        #region Properties

        public int ID { get { return serverID; } }

        public PlayerBuffer Players {  get { return players; } }

        #endregion

        #region MonoBehaviour

        void Awake() {
            DontDestroyOnLoad(this);
            serverID = 0;

            print("Server starting on port " + port + " ...");
            baseServer = new NetServer(port, maxConnections, NetConfig.ServerDefault);
            players = new PlayerBuffer(maxConnections);
            clients = new Dictionary<NetPeer, int>();
            StartServer();
            currentState = new LobbyState(this);
        }

        void OnEnable() {
            baseServer.Bind(NetPacketType.PeerConnect, OnClientConnected);
            baseServer.Bind(NetPacketType.PeerDisconnect, OnClientDisconnected);
            baseServer.Bind(NetPacketType.NetError, OnNetworkError);
            baseServer.BindDefault(OnUnknownDataReceived);
        }

        void OnDisable() {
            baseServer.Unbind(NetPacketType.PeerConnect, OnClientConnected);
            baseServer.Unbind(NetPacketType.PeerDisconnect, OnClientDisconnected);
            baseServer.Unbind(NetPacketType.NetError, OnNetworkError);
            baseServer.UnbindDefault(OnUnknownDataReceived);

        }

        void Update() {
            if (baseServer.IsReady()) {
                baseServer.Listen();
                baseServer.SendMessages();
            }
        }

        void OnDestroy() {
            Stop();
            players.Reset();
            clients.Clear();
        }

        #endregion

        #region Singleton

        /// <summary>
        /// Gets the netServer instance.
        /// </summary>
        public static NetServer NetworkServer {
            get { return Instance.baseServer; }
        }

        /// <summary>
        /// Starts the network server.
        /// </summary>
        public static void StartServer() {
            Init();
            instance.baseServer.Start();
        }

        /// <summary>
        /// Stops the network server.
        /// </summary>
        public static void Stop() {
            instance.baseServer.Stop();
        }

        /// <summary>
        /// Starts the game, if the players are ready (host only)
        /// </summary>
        public static void StartGame() {
            instance.StartGameInternal();
        }

        /// <summary>
        /// Adds the packet to the output queue.
        /// </summary>
        /// <param name="packet">message to be sent</param>
        /// <param name="client">client receiver</param>
        public static void Send(PacketBase packet, NetPeer client) {
            instance.baseServer.AddOutputMessage(new NetMessage(packet, client));
        }

        /// <summary>
        /// Adds the packet to the output queue, for every client except the given one.
        /// </summary>
        /// <param name="packet">data to be sent</param>
        /// <param name="client">client excluded (usually the original sender)</param>
        public static void SendExcluding(PacketBase packet, NetPeer client) {
            instance.baseServer.AddMessageExcluding(packet, client);
        }

        /// <summary>
        /// Register an external handler for the given packet type.
        /// </summary>
        /// <param name="type">packet type</param>
        /// <param name="handler">function to add</param>
        public static void Register(NetPacketType type, MessageDelegate handler) {
            instance.baseServer.Bind(type, handler);
        }

        /// <summary>
        /// Unregister an external handler for the given packet type.
        /// </summary>
        /// <param name="type">packet type</param>
        /// <param name="handler">function to remove</param>
        public static void Unregister(NetPacketType type, MessageDelegate handler) {
            instance.baseServer.Unbind(type, handler);
        }

        #endregion

        #region Handlers

        /// <summary>
        /// A client connected, no actions are taken.
        /// </summary>
        /// <param name="client">client that connected</param>
        /// <param name="args">extra arguments (null in this case)</param>
        private void OnClientConnected(NetPeer client, NetEventArgs args) {
            UnityEngine.Debug.Log("[SERVER] Connected client, waiting for authentication request...");
        }

        /// <summary>
        /// Handler for disconnections.
        /// </summary>
        /// <param name="client">client that disconnnected from this server</param>
        /// <param name="args">extra arguments</param>
        private void OnClientDisconnected(NetPeer client, NetEventArgs args) {
            UnityEngine.Debug.Log("[SERVER] Client disconnected");
            int id;
            if (clients.TryGetValue(client, out id)) { 
                Player p = players.Get(id);
                players.Remove(p);
                clients.Remove(client);
                PacketBase message = new PacketPlayerLeave(serverID, p);
                baseServer.AddOutputMessage(new NetMessage(message));
            }
        }

        /// <summary>
        /// Handler for network errors.
        /// </summary>
        /// <param name="client">client tha probably caused it</param>
        /// <param name="args">data containing the error code</param>
        private void OnNetworkError(NetPeer client, NetEventArgs args) {
            UnityEngine.Debug.Log("[SERVER] Error on the network: " + (int)(args.Data));
        }

        /// <summary>
        /// Handler for unhandled packet types.
        /// </summary>
        /// <param name="client">client who sent the packet</param>
        /// <param name="args">unknown packet</param>
        private void OnUnknownDataReceived(NetPeer client, NetEventArgs args) {
            UnityEngine.Debug.LogWarning("[SERVER] Received unhandled data from " + client.EndPoint);
            UnityEngine.Debug.LogWarning("[SERVER] Data: " + args.Data);
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Disconnects the given client, sending the extra message as additional info.
        /// </summary>
        /// <param name="client">the client to refuse</param>
        /// <param name="additionalInfo">text describing the reason</param>
        public void Disconnect(NetPeer client, string additionalInfo) {
            clients.Remove(client);
            byte[] data = System.Text.Encoding.ASCII.GetBytes(additionalInfo);
            baseServer.Disconnect(client, data);
        }

        /// <summary>
        /// Registers the current player to the players buffer and assigns the relative client instance to the dictionary.
        /// </summary>
        /// <param name="client">peer instance belonging to the player</param>
        /// <param name="player">player instance containing every info</param>
        public void AddPlayer(NetPeer client, Player player) {
            clients.Add(client, player.ID);
            players.Add(player);
        }

        /// <summary>
        /// Instance function to start the game if all players are ready.
        /// </summary>
        private void StartGameInternal() {
            if (!players.ReadyToStart()) {
                UnityEngine.Debug.Log("[SERVER] All clients must be ready in order to start!");
                return;
            }
            instance.baseServer.AddOutputMessage(new NetMessage(new PacketGameStart(instance.serverID)));
            currentState = currentState.Next();
        }

        #endregion
    }
}
