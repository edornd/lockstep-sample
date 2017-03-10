using Game.Network;
using Game.Players;
using Game.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace Presentation.Network {
    public class PlayerManager : Singleton<PlayerManager>, IResettable {

        private PlayersList playersList;
        private Player identity;

        #region Monobehaviour

        void Awake() {
            playersList = new PlayersList();
        }

        void OnEnable() {
            GameClient.Register(NetPacketType.PlayerEnter, OnPlayerEnter);
            GameClient.Register(NetPacketType.PlayerLeave, OnPlayerLeave);
            GameClient.Register(NetPacketType.PlayerReady, OnPlayerReady);

        }

        void OnDisable() {
            GameClient.Unregister(NetPacketType.PlayerEnter, OnPlayerEnter);
            GameClient.Unregister(NetPacketType.PlayerLeave, OnPlayerLeave);
            GameClient.Unregister(NetPacketType.PlayerReady, OnPlayerReady);

        }

        #endregion

        #region Resettable

        public void Reset() {
            this.playersList.Reset();
            identity = null;
        }

        #endregion

        #region Singleton

        public static Player Identity {
            get { return instance.identity; }
        }

        public static Dictionary<int, Player> Players {
            get { return instance.playersList.Players; }
        }

        public static void SetIdentity(Player identity) {
            instance.identity = identity;
        }

        #endregion

        #region Event Handlers

        private void OnPlayerEnter(NetPeer peer, NetEventArgs args) {
            PacketPlayerEnter message = PacketBase.Read<PacketPlayerEnter>((NetDataReader)(args.Data));
            if (message.Player.ID == identity.ID) {
                SetIdentity(message.Player);
            }
            playersList.AddPlayer(message.Player);
            NetEventManager.Trigger(NetEventType.PlayerEnter, args);
        }

        private void OnPlayerLeave(NetPeer peer, NetEventArgs args) {
            PacketPlayerEnter message = PacketBase.Read<PacketPlayerEnter>((NetDataReader)(args.Data));
            if (message.Player.ID == identity.ID) {
                UnityEngine.Debug.LogWarning("Dafuq, I can't leave myself!");
                return;
            }
            playersList.RemovePlayer(message.Player);
            NetEventManager.Trigger(NetEventType.PlayerLeave, args);
        }

        private void OnPlayerReady(NetPeer peer, NetEventArgs args) {
            PacketPlayerReady message = PacketBase.Read<PacketPlayerReady>((NetDataReader)(args.Data));
            Player player = null;
            if (playersList.Players.TryGetValue(message.Sender, out player)) {
                player.SetReady(message.Value);
                NetEventManager.Trigger(NetEventType.PlayerReady, args);
            }
        }

        #endregion
    }
}
