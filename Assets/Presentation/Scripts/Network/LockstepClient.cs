using Game.Network;
using Game.Network.Players;
using Game.Utils;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Presentation.Network {
    /// <summary>
    /// Higher level client class, using a NetClient for communication.
    /// </summary>
    public class LockstepClient : MonoBehaviour, IObservable {

        public int updateRate = 15;
        public string connectionKey = "test";
        private NetClient baseClient;
        private ClientState currentState;
        private List<IObserver> observers = new List<IObserver>();

        #region MonoBehaviour

        void Start() {
            DontDestroyOnLoad(this.gameObject);
            baseClient = new NetClient(updateRate, connectionKey);
            baseClient.Bind(NetPacketType.PeerConnect, OnConnect);
            baseClient.Bind(NetPacketType.PeerDisconnect, OnDisconnect);
            baseClient.Bind(NetPacketType.PeerIdentity, OnPlayerIdentity);
            baseClient.Bind(NetPacketType.PeerInfo, OnPlayerInfo);
            baseClient.Bind(NetPacketType.GameReady, OnClientReady);
            baseClient.Bind(NetPacketType.GameStart, OnGameStart);
            baseClient.Bind(NetPacketType.GameCmd, OnCommandReceived);
            baseClient.Bind(NetPacketType.NetError, OnNetworkError);
            baseClient.Bind(NetPacketType.PeerLatency, OnLatencyUpdate);
            currentState = ClientState.Disconnected;
        }

        void Update() {
            baseClient.Receive();
            baseClient.SendMessages();
        }

        void OnApplicationQuit() {
            baseClient.Disconnect();
        }

        #endregion

        #region Public methods

        public ClientState CurrentState {
            get { return currentState; }
            set {
                currentState = value;
                Notify(null);
            }
        }

        public virtual void Subscribe(IObserver ob) {
            this.observers.Add(ob);
        }

        public virtual void Unsubscribe(IObserver ob) {
            this.observers.Remove(ob);
        }

        public virtual void Notify(object args) {
            foreach (IObserver ob in observers) {
                ob.Signal(this, args);
            }
        }

        public void Connect(string hostAddress, int hostPort) {
            baseClient.Connect(hostAddress, hostPort);
        }

        public void Disconnect() {
            baseClient.Disconnect();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Simply gets the reader object from the EventArgs wrapper.
        /// </summary>
        /// <param name="args">object containing a DataEventArgs</param>
        /// <returns>the netDataReader</returns>
        private NetDataReader GetReader(EventArgs args) {
            return ((DataEventArgs)args).Reader;
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Function called on connect.
        /// </summary>
        /// <param name="args">object containing data about the connection</param>
        private void OnConnect(EventArgs e) {
            CurrentState = ClientState.Connecting;
        }

        /// <summary>
        /// Function called when disconnected from the server.
        /// </summary>
        /// <param name="args">object containing data about the disconnection</param>
        private void OnDisconnect(EventArgs e) {
            CurrentState = ClientState.Disconnected;
        }

        /// <summary>
        /// Function called on receiving a network error.
        /// </summary>
        /// <param name="args">object containing the error code</param>
        private void OnNetworkError(EventArgs e) {
            int error = ((ErrorEventArgs)e).Code;
            Debug.LogError("Network error, code: " + error);
        }

        /// <summary>
        /// Function called when receiving a latency update packet.
        /// </summary>
        /// <param name="args">net status update</param>
        private void OnLatencyUpdate(EventArgs e) {

        }

        /// <summary>
        /// Received the network unique identifier from the server.
        /// The function saves it in the current identity and send the username as well.
        /// </summary>
        /// <param name="e"></param>
        private void OnPlayerIdentity(EventArgs e) {
            NetDataReader reader = GetReader(e);
            NetIdentityPacket data = PacketBase.Read<NetIdentityPacket>(reader);
            if (data.Identifier > 0) {
                Player identity = PlayerManager.Instance.Identity;
                identity.SetID(data.Identifier);
                CurrentState = ClientState.Connected;
                baseClient.Send(new PlayerInfoPacket(NetPacketType.PeerInfo, identity.ID, identity));
            }
        }

        /// <summary>
        /// Received a list containing the players currently connected to the server (including this client),
        /// This method updates the players list and changes the state to logged in, a.k.a. ready for the lobby.
        /// </summary>
        /// <param name="e">arguments containing the raw data</param>
        private void OnPlayerInfo(EventArgs e) {
            NetDataReader reader = GetReader(e);
            PlayerInfoPacket data = PacketBase.Read<PlayerInfoPacket>(reader);
            foreach (Player p in data.Players) {
                PlayerManager.Instance.AddPlayer(p);
            }
            CurrentState = ClientState.LoggedIn;
        }

        /// <summary>
        /// Received a "ready" message from another player.
        /// Every player ready does not imply the game should start, the client
        /// needs to wait for the server's "game start" signal.
        /// </summary>
        /// <param name="reader">raw packet data</param>
        private void OnClientReady(EventArgs e) {
            //todo...
        }

        /// <summary>
        /// Received a "start" message from the server.
        /// </summary>
        /// <param name="reader">raw packet data</param>
        private void OnGameStart(EventArgs e) {
            //todo...
        }

        /// <summary>
        /// Received a command from another player.
        /// </summary>
        /// <param name="reader">raw packet data</param>
        private void OnCommandReceived(EventArgs e) {
            //unpack command
            //send to main ls manager
        }

        #endregion
    }
}
