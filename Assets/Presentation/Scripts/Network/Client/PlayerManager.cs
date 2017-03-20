using Game.Lockstep;
using Game.Network;
using Game.Players;
using Game.Utils;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace Presentation.Network {
    /// <summary>
    /// Monobehaviour class listening to network events regarding players.
    /// The static functions are thread safe in order to use them from the simulation thread,
    /// if needed.
    /// </summary>
    public class PlayerManager : SingletonMono<PlayerManager>, IResettable {

        private Dictionary<int, Player> players;
        private Player identity;
        private int totalPlayers;
        private int activePlayers;

        //lock for each field
        private object playersLock;
        private object countLock;
        private object idLock;

        #region Monobehaviour

        void Awake() {
            players = new Dictionary<int, Player>();
            totalPlayers = 0;
            activePlayers = 0;
            playersLock = new object();
            countLock = new object();
            idLock = new object();
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

        void OnDestroy() {
            Reset();
        }

        #endregion

        #region Resettable

        public void Reset() {
            players.Clear();
            identity = null;
            totalPlayers = 0;
            activePlayers = 0;
        }

        #endregion

        #region Singleton

        public static Player Identity {
            get {
                lock (instance.idLock) {
                    return instance.identity;
                }
            }
        }

        public static Dictionary<int, Player> Players {
            get {
                lock (instance.playersLock) {
                    return instance.players;
                }
            }
        }

        public static int PlayerCount {
            get {
                return instance.TotalPlayersInternal;
            }
        }

        public static int ActivePlayerCount {
            get {
                return instance.ActivePlayersInternal;
            }
        }

        public static void SetIdentity(Player identity) {
            lock (instance.idLock) {
                instance.identity = identity;
            }
        }

        public static void Add(Player player) {
            lock (instance.playersLock) {
                instance.players.Add(player.ID, player);
                instance.TotalPlayersInternal++;
                instance.ActivePlayersInternal++;
            }
        }

        public static void Remove(Player player) {
            lock (instance.playersLock) {
                if (instance.players.Remove(player.ID)) {
                    instance.TotalPlayersInternal--;
                    instance.ActivePlayersInternal--;
                }
            }
        }

        public static void SetDisconnected(int playerID) {
            lock (instance.playersLock) {
                Player player = null;
                if (instance.players.TryGetValue(playerID, out player)) {
                    player.SetActive(false);
                    instance.ActivePlayersInternal--;
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handler for "player enter" packets.
        /// </summary>
        /// <param name="peer">server</param>
        /// <param name="args">raw data containing the player packet</param>
        private void OnPlayerEnter(NetPeer peer, NetEventArgs args) {
            if (GameClient.CurrentState == ClientState.Game)
                return;

            PacketPlayerEnter message = PacketBase.Read<PacketPlayerEnter>((NetDataReader)(args.Data));
            if (message.Player.ID == identity.ID) {
                SetIdentity(message.Player);
            }
            Add(message.Player);
            NetEventManager.Trigger(NetEventType.PlayerEnter, args);
        }

        /// <summary>
        /// Handler for 'player leaving' packets.
        /// Removes the player if the game is not started yet, otherwise mark him as 'disconnected'.
        /// </summary>
        /// <param name="peer">server</param>
        /// <param name="args">wrapper around raw data containing the packet</param>
        private void OnPlayerLeave(NetPeer peer, NetEventArgs args) {
            PacketPlayerEnter message = PacketBase.Read<PacketPlayerEnter>((NetDataReader)(args.Data));
            if (message.Player.ID == Identity.ID) {
                UnityEngine.Debug.LogWarning("Dafuq, I can't leave myself!");
                return;
            }
            if (GameClient.CurrentState != ClientState.Game) {
                Remove(message.Player);
            }
            else {
                SetDisconnected(message.Player.ID);
                LockstepLogic.Instance.UpdatePlayersCount(activePlayers, message.Player.ID);
            }
            NetEventManager.Trigger(NetEventType.PlayerLeave, args);
        }

        private void OnPlayerReady(NetPeer peer, NetEventArgs args) {
            if (GameClient.CurrentState == ClientState.Game)
                return;
            PacketPlayerReady message = PacketBase.Read<PacketPlayerReady>((NetDataReader)(args.Data));
            Player player = null;
            if (players.TryGetValue(message.Sender, out player)) {
                player.SetReady(message.Value);
                NetEventManager.Trigger(NetEventType.PlayerReady, args);
            }
        }

        #endregion

        #region Private Helper functions

        private int TotalPlayersInternal {
            get {
                lock (countLock) {
                    return totalPlayers;
                }
            }
            set {
                lock (countLock) {
                    totalPlayers = value;
                }
            }
        }

        private int ActivePlayersInternal {
            get {
                return activePlayers;
            }
            set {
                activePlayers = value;
            }
        }

        #endregion
    }
}
