
namespace Game.Network {
    public enum NetPacketType : ushort {
        PeerConnect,
        PeerDisconnect,
        PeerIdentity,
        PeerInfo,
        PeerLatency,
        GameLogIn,
        GameLogOut,
        GameSeed,
        GameReady,
        GameStart,
        GamePause,
        GameStop,
        GameCmd,
        NetError
    }
}
