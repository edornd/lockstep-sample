using Game.Network;
using Game.Players;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace Presentation.Network {
    /// <summary>
    /// Higher level client class, using a NetClient for communication.
    /// </summary>
    public class GameClient : Singleton<GameClient> {

        private NetClient baseClient;

        #region MonoBehaviour

        void Awake() {
            DontDestroyOnLoad(this.gameObject);
            Init();
            baseClient = new NetClient(NetConfig.ClientDefault);
            PlayerManager.Init();
        }

        void OnEnable() { 
            baseClient.Bind(NetPacketType.PeerDisconnect, OnDisconnect);
            baseClient.Bind(NetPacketType.NetError, OnNetworkError);
        }

        void OnDisable() {
            baseClient.Unbind(NetPacketType.PeerDisconnect, OnDisconnect);
            baseClient.Unbind(NetPacketType.NetError, OnNetworkError);
        }

        void Update() {
            baseClient.Receive();
            baseClient.SendMessages();
        }

        void OnDestroy() {
            baseClient.Disconnect();
        }

        #endregion

        #region Singleton methods

        public static NetClient NetworkClient { get { return Instance.baseClient; } }

        public static void Connect(string hostAddress, int hostPort) {
            instance.baseClient.Connect(hostAddress, hostPort);
        }

        public static void Disconnect() {
            instance.baseClient.Disconnect();
            DisconnectInfo info = new DisconnectInfo();
            info.Reason = DisconnectReason.DisconnectPeerCalled;
            NetEventManager.Trigger(NetEventType.Disconnected, new NetEventArgs(info));
        }

        public static void Register(NetPacketType type, MessageDelegate handler) {
            instance.baseClient.Bind(type, handler);
        }

        public static void Unregister(NetPacketType type, MessageDelegate handler) {
            instance.baseClient.Unbind(type, handler);
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

        #endregion
    }
}
