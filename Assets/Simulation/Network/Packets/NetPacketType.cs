
namespace Game.Network {
    public enum NetPacketType : ushort {
        PeerConnect,
        PeerDisconnect,
        PeerAuth,
        PeerLatency,
        PlayerEnter,
        PlayerLeave,
        PlayerReady,
        GameInfo,
        GameStart,
        GamePause,
        GameStop,
        TurnData,
        NetError
    }
}
