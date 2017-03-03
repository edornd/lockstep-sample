using Game.Network;
using Game.Network.Players;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Presentation.Network {
    public class LockstepServer : MonoBehaviour {

        public int port = 28960;
        public int maxConnections = 8;
        public string connectionKey = "test";
        private NetServer baseServer;

        private static int identifier;
        private Dictionary<int, Player> players;
        private int serverID;

        #region MonoBehaviour

        void Start() {
            print("Server starting on port " + port + " ...");
            serverID = 0;
            identifier = serverID + 1;
            baseServer = new NetServer(port, connectionKey, maxConnections);
            players = new Dictionary<int, Player>();
            baseServer.Bind(NetPacketType.PeerConnect, OnClientConnected);
            baseServer.Bind(NetPacketType.PeerDisconnect, OnClientDisconnected);
            baseServer.Bind(NetPacketType.PeerInfo, OnPlayerInfo);
            baseServer.Bind(NetPacketType.GameReady, OnPlayerReady);
            baseServer.Bind(NetPacketType.GameStart, OnPlayerStart);
            baseServer.Bind(NetPacketType.GameCmd, OnPlayerCommand);
            baseServer.Bind(NetPacketType.NetError, OnNetworkError);
            baseServer.Start();
        }

        void Update() {
            if (baseServer.IsReady()) {
                baseServer.Listen();
                baseServer.SendMessages();
            }
        }

        public void OnApplicationQuit() {
            Stop();
        }

        #endregion

        #region Public methods

        public void Stop() {
            baseServer.Stop();
        }

        #endregion

        #region Private methods

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

        private void OnClientConnected(NetPeer client, EventArgs e) {
            Player newPlayer = new Player();
            newPlayer.SetID(identifier++);
            players.Add(newPlayer.ID, newPlayer);
            PacketBase data = new NetIdentityPacket(NetPacketType.PeerIdentity, serverID, newPlayer.ID);
            baseServer.Send(new NetMessage(data, client));
        }

        private void OnClientDisconnected(NetPeer client, EventArgs e) {

        }

        private void OnNetworkError(NetPeer client, EventArgs e) {

        }

        /// <summary>
        /// Received the informations about a player. The function checks for errors, adds it to the list
        /// and prepares the list of the other connected players as response.
        /// </summary>
        /// <param name="client">the client that sent his info</param>
        /// <param name="e"></param>
        private void OnPlayerInfo(NetPeer client, EventArgs e) {
            NetDataReader reader = GetReader(e);
            PlayerInfoPacket data = PacketBase.Read<PlayerInfoPacket>(reader);
            if (data.Count != 1) {
                Debug.LogError("Received multiple player informations, not good");
                return;
            }
            Player current = data.Players[0];
            if (players.ContainsKey(current.ID)) {
                players[current.ID] = current;
            }
            else {
                Debug.LogError("Unknown player!");
            }

            PlayerInfoPacket response = new PlayerInfoPacket(NetPacketType.PeerInfo, 0);
            foreach (Player p in players.Values) {
                response.AddPlayer(p);
            }
            baseServer.Send(new NetMessage(response, client));
        }

        private void OnPlayerReady(NetPeer client, EventArgs e) {
            
        }

        private void OnPlayerStart(NetPeer client, EventArgs e) {

        }

        private void OnPlayerCommand(NetPeer client, EventArgs e) {

        }

        #endregion
    }
}
