using Game.Network;
using Game.Players;
using LiteNetLib;
using LiteNetLib.Utils;
using System;

namespace Presentation.Network {
    public class LobbyState : ServerState {

        #region State Implementation

        public LobbyState(GameServer server) : base(server) {
            Enable();
        }

        public override void Enable() {
            GameServer.Register(NetPacketType.PeerAuth, OnClientAuthRequest);
            GameServer.Register(NetPacketType.PlayerReady, OnClientReady);
        }

        public override void Disable() {
            GameServer.Unregister(NetPacketType.PeerAuth, OnClientAuthRequest);
            GameServer.Unregister(NetPacketType.PlayerReady, OnClientReady);

        }

        public override ServerState Next() {
            Disable();
            return new GameState(server);
        }

        #endregion

        #region Handlers

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
                server.Disconnect(client, "Authentication failed.");
                return;
            }
            PlayerBuffer players = server.Players;
            //refuse if there's no available slot
            if (players.IsFull()) {
                UnityEngine.Debug.Log("[SERVER] players list full");
                server.Disconnect(client, "Server full.");
                return;
            }

            //refuse if the ID getter fails (it shouldn't though)
            int id;
            if (!players.FirstEmptyID(out id)) {
                UnityEngine.Debug.Log("[SERVER] no available id");
                server.Disconnect(client, "Authentication failed.");
                return;
            }
            //otherwise welcome new player
            Player newPlayer = new Player(id, packet.Message);
            PacketAuth response = new PacketAuth(server.ID, "WLCM", newPlayer.ID);
            GameServer.Send(response, client);

            //send already connected players to the new client
            foreach (Player p in players) {
                PacketBase playerPacket = new PacketPlayerEnter(server.ID, p);
                GameServer.Send(playerPacket, client);
            }

            //send game info
            PacketGameInfo gameInfo = new PacketGameInfo(server.ID, 0);
            GameServer.Send(gameInfo, client);

            //send new player to everybody, add him to the buffer
            GameServer.Send(new PacketPlayerEnter(server.ID, newPlayer), null);
            server.AddPlayer(client, newPlayer);
        }

        /// <summary>
        /// Handler for ready messages
        /// </summary>
        /// <param name="client">the sender</param>
        /// <param name="args">network event arguments</param>
        private void OnClientReady(NetPeer client, NetEventArgs args) {
            PacketPlayerReady message = PacketBase.Read<PacketPlayerReady>((NetDataReader)(args.Data));
            Player p = null;
            if (server.Players.TryGetValue(message.Sender, out p)) {
                p.SetReady(message.Value);
                GameServer.SendExcluding(message, client);
            }
            else {
                server.Disconnect(client, "Client not authenticated.");
            }
        }

        #endregion
    }
}
