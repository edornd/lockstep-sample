using LiteNetLib;

namespace Game.Network {
    /// <summary>
    /// Simple class that handles every main network parameter.
    /// </summary>
    public class NetConfig {

        #region Default values

        private static readonly string DEFAULT_CONN_KEY = "test";
        private static readonly int DEFAULT_PING_RATE = 1000;
        private static readonly int DEFAULT_UPDATE_RATE = 15;
        private static readonly int DEFAULT_DISC_TIMEOUT = 5000;
        private static readonly int DEFAULT_RECONN_DELAY = 500;
        private static readonly int DEFAULT_MAX_CONN_ATT = 10;
        private static readonly bool DEFAULT_UNCON_MSG = false;
        private static readonly bool DEFAULT_NAT_PUNCH = false;
        private static readonly bool DEFAULT_DISCOVERY = false;

        #endregion

        private string connectionKey;
        private int pingInterval;
        private int updateRate;
        private int disconnectionTimeout;
        private int reconnectionDelay;
        private int maxConnectionAttempts;

        private bool unconnectedMsg;
        private bool natPunch;
        private bool discovery;

        #region Constructors

        public NetConfig() : this(
                DEFAULT_CONN_KEY,
                DEFAULT_PING_RATE,
                DEFAULT_UPDATE_RATE,
                DEFAULT_DISC_TIMEOUT,
                DEFAULT_RECONN_DELAY,
                DEFAULT_MAX_CONN_ATT,
                DEFAULT_UNCON_MSG,
                DEFAULT_NAT_PUNCH,
                DEFAULT_DISCOVERY
            ){ }

        public NetConfig(bool enableUnconnected, bool enableNatPunch, bool enableDiscovery) : this(
        DEFAULT_CONN_KEY,
        DEFAULT_PING_RATE,
        DEFAULT_UPDATE_RATE,
        DEFAULT_DISC_TIMEOUT,
        DEFAULT_RECONN_DELAY,
        DEFAULT_MAX_CONN_ATT,
        enableUnconnected,
        enableNatPunch,
        enableDiscovery
    ) { }

        public NetConfig(
                string connectionKey,
                int pingInterval,
                int updateRate,
                int disconnectionTimeout,
                int reconnectionDelay,
                int maxConnectionAttempts,
                bool unconnectedMsg,
                bool natPunch,
                bool discovery
            ) {
            this.connectionKey = connectionKey;
            this.pingInterval = pingInterval;
            this.updateRate = updateRate;
            this.disconnectionTimeout = disconnectionTimeout;
            this.reconnectionDelay = reconnectionDelay;
            this.maxConnectionAttempts = maxConnectionAttempts;
            this.unconnectedMsg = unconnectedMsg;
            this.natPunch = natPunch;
            this.discovery = discovery;
        }

        #endregion

        #region Properties

        public static NetConfig ClientDefault {
            get { return new NetConfig(false, true, false); }
        }

        public static NetConfig ServerDefault {
            get { return new NetConfig(false, false, true); }
        }

        public string Key { get { return connectionKey; } }

        public int PingInterval { get { return pingInterval; } }

        public int UpdateRate { get { return updateRate; } }

        public int DisconnectTimeout { get { return disconnectionTimeout; } }

        public int ReconnectDelay { get { return reconnectionDelay; } }

        public int ConnectAttempts { get { return maxConnectionAttempts; } }

        public bool UnconnectMessages { get { return unconnectedMsg; } }

        public bool NatPunchEnabled { get { return natPunch; } }

        public bool DiscoveryEnabled { get { return discovery; } }

        #endregion

        #region Public methods

        public void Configure(NetManager manager) {
            if (manager == null)
                return;
            manager.PingInterval = pingInterval;
            manager.UpdateTime = updateRate;
            manager.DisconnectTimeout = disconnectionTimeout;
            manager.ReconnectDelay = reconnectionDelay;
            manager.MaxConnectAttempts = maxConnectionAttempts;
            manager.UnconnectedMessagesEnabled = unconnectedMsg;
            manager.NatPunchEnabled = natPunch;
            manager.DiscoveryEnabled = discovery;
        }

        #endregion
    }
}
