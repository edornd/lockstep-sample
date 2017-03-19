using Game.Network;
using LiteNetLib;
using LiteNetLib.Utils;
using System;

namespace Presentation.Network {
    /// <summary>
    /// Server state representing the game behaviour.
    /// </summary>
    public class GameState : ServerState {

        #region State Implementation

        public GameState(GameServer server) : base(server) {
            Enable();
        }

        public override void Enable() {
            GameServer.Register(NetPacketType.TurnData, OnClientTurnData);
        }

        public override void Disable() {
            GameServer.Unregister(NetPacketType.TurnData, OnClientTurnData);

        }

        public override ServerState Next() {
            throw new NotImplementedException();
        }

        #endregion

        #region Handlers

        /// <summary>
        /// Received turn information, simply forwards it to every other client.
        /// </summary>
        /// <param name="client">sender</param>
        /// <param name="args">wrapper containing a data reader</param>
        private void OnClientTurnData(NetPeer client, NetEventArgs args) {
            NetDataReader reader = (NetDataReader)(args.Data);
            PacketRawData rawdata = new PacketRawData(reader.Data);
            GameServer.SendExcluding(rawdata, client);
        }

        #endregion
    }
}
