using Game.Network;
using Game.Players;
using Game.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace Presentation.Network {
    public class GameServer : Singleton<GameServer> {

        public static readonly int DEFAULT_PORT = 28960;

        public int port = DEFAULT_PORT;
        public int maxConnections = 8;
        private NetServer baseServer;

        private int serverID;
        private PlayerBuffer players;
        private Dictionary<NetPeer, int> clients;

        #region MonoBehaviour

        void Awake() {
            DontDestroyOnLoad(this);
            serverID = 0;

            print("Server starting on port " + port + " ...");
            baseServer = new NetServer(port, maxConnections, NetConfig.ServerDefault);
            players = new PlayerBuffer(maxConnections);
            clients = new Dictionary<NetPeer, int>();
            StartServer();
        }

        void OnEnable() {
            baseServer.Bind(NetPacketType.PeerConnect, OnClientConnected);
            baseServer.Bind(NetPacketType.PeerDisconnect, OnClientDisconnected);
            baseServer.Bind(NetPacketType.NetError, OnNetworkError);

            baseServer.Bind(NetPacketType.PeerAuth, OnClientAuthRequest);
            baseServer.Bind(NetPacketType.PlayerReady, OnClientReady);
        }

        void OnDisable() {
            baseServer.Unbind(NetPacketType.PeerConnect, OnClientConnected);
            baseServer.Unbind(NetPacketType.PeerDisconnect, OnClientDisconnected);
            baseServer.Unbind(NetPacketType.NetError, OnNetworkError);

            baseServer.Unbind(NetPacketType.PeerAuth, OnClientAuthRequest);
            baseServer.Unbind(NetPacketType.PlayerReady, OnClientReady);

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

        public static NetServer NetworkServer {
            get { return Instance.baseServer; }
        }

        public static void StartServer() {
            Init();
            instance.baseServer.Start();
        }

        public static void Stop() {
            instance.baseServer.Stop();
        }

        public static void StartGame() {
            instance.StartGameInternal();
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
                baseServer.Send(new NetMessage(message));
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
        /// Handler for authentication requests
        /// </summary>
        /// <param name="client">client the requested to be authenticated</param>
        /// <param name="args">arguments containing raw data</param>
        private void OnClientAuthRequest(NetPeer client, NetEventArgs args) {
            PacketAuth packet = PacketBase.Read<PacketAuth>((NetDataReader)(args.Data));
            //refuse if ID not 0
            if (packet.Sender != 0) {
                UnityEngine.Debug.Log("[SERVER] Client already has an ID");
                Disconnect(client, "Authentication failed.");
                return;
            }
            //refuse if there's no available slot
            if (players.IsFull()) {
                UnityEngine.Debug.Log("[SERVER] players list full");
                Disconnect(client, "Server full.");
                return;
            }

            //refuse if the ID getter fails (it shouldn't though)
            int id;
            if (!players.FirstEmptyID(out id)) {
                UnityEngine.Debug.Log("[SERVER] no available id");
                Disconnect(client, "Authentication failed.");
                return;
            }
            //otherwise welcome new player
            Player newPlayer = new Player(id, packet.Message);
            PacketAuth response = new PacketAuth(serverID, "WLCM", newPlayer.ID);
            baseServer.Send(new NetMessage(response, client));

            //send already connected players to the new client
            foreach(Player p in players) {
                PacketBase playerPacket = new PacketPlayerEnter(serverID, p);
                baseServer.Send(new NetMessage(playerPacket, client));
            }

            //send game info
            PacketGameInfo gameInfo = new PacketGameInfo(serverID, 0);
            baseServer.Send(new NetMessage(gameInfo, client));

            //send new player to everybody, add him to the buffer
            baseServer.Send(new NetMessage(new PacketPlayerEnter(serverID, newPlayer)));
            clients.Add(client, newPlayer.ID);
            players.Add(newPlayer);
        }

        /// <summary>
        /// Handler for ready messages
        /// </summary>
        /// <param name="client">the sender</param>
        /// <param name="args">the </param>
        private void OnClientReady(NetPeer client, NetEventArgs args) {
            PacketPlayerReady message = PacketBase.Read<PacketPlayerReady>((NetDataReader)(args.Data));
            Player p = null;
            if (players.TryGetValue(message.Sender, out p)) {
                p.SetReady(message.Value);
                baseServer.SendExcluding(new NetMessage(message, client));
            }
            else {
                Disconnect(client, "Client not authenticated.");
            }
        }
        #endregion

        #region Private Helper Functions

        /// <summary>
        /// Disconnects the given client, sending the extra message as additional info.
        /// </summary>
        /// <param name="client">the client to refuse</param>
        /// <param name="additionalInfo">text describing the reason</param>
        private void Disconnect(NetPeer client, string additionalInfo) {
            clients.Remove(client);
            byte[] data = System.Text.Encoding.ASCII.GetBytes(additionalInfo);
            baseServer.Disconnect(client, data);
        }

        /// <summary>
        /// Instance function to start the game if all players are ready.
        /// </summary>
        private void StartGameInternal() {
            if (!instance.players.ReadyToStart()) {
                UnityEngine.Debug.Log("[SERVER] All clients must be ready in order to start!");
                return;
            }
            instance.baseServer.Send(new NetMessage(new PacketGameStart(instance.serverID)));
        }

        #endregion
    }
}
