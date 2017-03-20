using Game.Network;
using Game.Utils;
using LiteNetLib;
using UnityEngine;

namespace Presentation.Network {

    public enum ClientState {
        Disconnected,
        Connected,
        Lobby,
        Game
    }

    /// <summary>
    /// Higher level client class, using a NetClient for communication.
    /// </summary>
    public class GameClient : SingletonMono<GameClient>, IResettable {

        public GameObject serverPrefab;
        private NetClient baseClient;
        private ClientState currentState;

        private GameObject serverObject;
        private bool isHost;


        #region MonoBehaviour

        void Awake() {
            DontDestroyOnLoad(this.gameObject);
            //"there can only be one"
            if (FindObjectsOfType(GetType()).Length > 1) {
                Destroy(this.gameObject);
                return;
            }
            Init();
            PlayerManager.Init();
            baseClient = new NetClient(NetConfig.ClientDefault);
            currentState = ClientState.Disconnected;
            isHost = false;
        }

        void OnEnable() {
            if (baseClient != null) {
                baseClient.Bind(NetPacketType.PeerDisconnect, OnDisconnect);
                baseClient.Bind(NetPacketType.NetError, OnNetworkError);
                baseClient.BindDefault(OnUnknownDataReceived);
            }
        }

        void OnDisable() {
            if (baseClient != null) {
                baseClient.Unbind(NetPacketType.PeerDisconnect, OnDisconnect);
                baseClient.Unbind(NetPacketType.NetError, OnNetworkError);
                baseClient.UnbindDefault(OnUnknownDataReceived);

            }
        }

        void Update() {
            baseClient.Receive();
            baseClient.SendMessages();
        }

        void OnDestroy() {
            if (baseClient != null)
                baseClient.Disconnect();
        }

        #endregion

        #region Singleton methods

        /// <summary>
        /// Static reference to the network level, useful to enqueue packets from outside
        /// this class.
        /// </summary>
        public static NetClient NetworkClient { get { return Instance.baseClient; } }

        /// <summary>
        /// Returns true whether the current game is hosting the match.
        /// </summary>
        public static bool IsHost { get { return instance.isHost; } }

        public static ClientState CurrentState {
            get { return instance.currentState; }
            set { instance.currentState = value; }
        }

        /// <summary>
        /// Tries to connect to the given address and port.
        /// </summary>
        /// <param name="hostAddress">ip address</param>
        /// <param name="hostPort">server port</param>
        public static void HostGame() {
            instance.Host();
        }

        /// <summary>
        /// Tries to connect to the given address and port.
        /// </summary>
        /// <param name="hostAddress">ip address</param>
        /// <param name="hostPort">server port</param>
        public static void Connect(string hostAddress, int hostPort) {
            instance.baseClient.Connect(hostAddress, hostPort);
        }

        /// <summary>
        /// Calls the disconnect procedure from the network client and triggers a disconnection event.
        /// The function iterates through every resettable component, invoking the reset method.
        /// </summary>
        public static void Disconnect() {
            foreach (IResettable comp in instance.gameObject.GetComponents<IResettable>()) {
                comp.Reset();
            }
            instance.baseClient.Disconnect();
            DisconnectInfo info = new DisconnectInfo();
            info.Reason = DisconnectReason.DisconnectPeerCalled;
            NetEventManager.Trigger(NetEventType.Disconnected, new NetEventArgs(info));
        }

        /// <summary>
        /// Adds the given message to the output queue of the network client.
        /// </summary>
        /// <param name="message">packet to be sent</param>
        public static void Send(PacketBase message) {
            instance.baseClient.AddOutputMessage(message);
        }

        /// <summary>
        /// Register an external handler for the given packet type.
        /// </summary>
        /// <param name="type">packet type</param>
        /// <param name="handler">function to add</param>
        public static void Register(NetPacketType type, MessageDelegate handler) {
            instance.baseClient.Bind(type, handler);
        }

        /// <summary>
        /// Unregister an external handler for the given packet type.
        /// </summary>
        /// <param name="type">packet type</param>
        /// <param name="handler">function to remove</param>
        public static void Unregister(NetPacketType type, MessageDelegate handler) {
            instance.baseClient.Unbind(type, handler);
        }

        #endregion

        #region Instance Methods

        /// <summary>
        /// Resets the game client instance, destroying any server object.
        /// </summary>
        public void Reset() {
            baseClient.Disconnect();
            if (isHost) {
                Destroy(serverObject);
                isHost = false;
            }
        }

        /// <summary>
        /// Instantiates a new server prefab and connects to the newly created server.
        /// </summary>
        public void Host() {
            if (serverObject != null) {
                Debug.LogWarning("Server GameObject already instantiated!");
                return;
            }
            serverObject = GameObject.Instantiate(serverPrefab, null, false) as GameObject;
            baseClient.Connect("localhost", serverObject.GetComponent<GameServer>().port);
            isHost = true;
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Function called when losing connection from the server.
        /// </summary>
        /// <param name="args">object containing data about the disconnection</param>
        private void OnDisconnect(NetPeer peer, NetEventArgs args) {
            PlayerManager.Instance.Reset();
            NetEventManager.Trigger(NetEventType.Disconnected, args);
        }

        /// <summary>
        /// Function called on receiving a network error.
        /// </summary>
        /// <param name="args">object containing the error code</param>
        private void OnNetworkError(NetPeer peer, NetEventArgs args) {
            NetEventManager.Trigger(NetEventType.NetworkError, args);
        }

        /// <summary>
        /// Handler for unhandled packet types.
        /// </summary>
        /// <param name="client">client who sent the packet</param>
        /// <param name="args">unknown packet</param>
        private void OnUnknownDataReceived(NetPeer client, NetEventArgs args) {
            UnityEngine.Debug.LogWarning("Received unhandled data from the server");
            UnityEngine.Debug.LogWarning("Data: " + args.Data);
        }

        #endregion
    }
}
