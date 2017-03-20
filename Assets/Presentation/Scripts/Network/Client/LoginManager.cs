using UnityEngine;
using LiteNetLib;
using Game.Network;
using System;
using LiteNetLib.Utils;
using Game.Players;

namespace Presentation.Network {
    public class LoginManager : MonoBehaviour {

        private string username;

        void OnEnable() {
            GameClient.Register(NetPacketType.PeerConnect, OnConnect);
            GameClient.Register(NetPacketType.PeerAuth, OnClientAuthenticated);
            GameClient.Register(NetPacketType.GameInfo, OnReceiveGameInfo);
        }

        void OnDisable() {
            GameClient.Unregister(NetPacketType.PeerConnect, OnConnect);
            GameClient.Unregister(NetPacketType.PeerAuth, OnClientAuthenticated);
            GameClient.Unregister(NetPacketType.GameInfo, OnReceiveGameInfo);
        }

        public void HostGame(string username) {
            this.username = username;
            GameClient.HostGame();
        }

        public void Login(string username, string address) {
            this.username = username;
            GameClient.Connect(address, GameServer.DEFAULT_PORT);
        }

        private void OnConnect(NetPeer peer, NetEventArgs args) {
            NetEventManager.Trigger(NetEventType.Connected, args);
            PacketBase message = new PacketAuth(0, username, 0);
            GameClient.CurrentState = ClientState.Connected;
            GameClient.Send(message);
        }

        private void OnClientAuthenticated(NetPeer peer, NetEventArgs args) {
            PacketAuth message = PacketBase.Read<PacketAuth>((NetDataReader)(args.Data));
            PlayerManager.SetIdentity(new Player(message.ID, username));
            NetEventManager.Trigger(NetEventType.Authenticated, null);
        }

        private void OnReceiveGameInfo(NetPeer peer, NetEventArgs args) {
            //TODO deserialize game info
            NetEventManager.Trigger(NetEventType.LoggedIn, null);
            GameClient.CurrentState = ClientState.Lobby;
        }
    }
}
