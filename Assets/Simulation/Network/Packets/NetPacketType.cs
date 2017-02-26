
namespace Game.Network {
    public enum NetPacketType : ushort {
        Connect,
        Disconnect,
        GameLogIn,
        GameLogOut,
        GameSeed,
        GameReady,
        GameStart,
        GamePause,
        GameStop,
        GameCmd,
        Ack,
        Done
    }
}
