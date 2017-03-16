using Game.Lockstep;
using Game.Network;
using LiteNetLib;
using LiteNetLib.Utils;
using Presentation.Network;
using System.Collections.Generic;
using UnityEngine;

namespace Presentation {
    public class Scheduler : SingletonMono<Scheduler> {

        private int identity;
        private LockstepLogic lockstep;
        private long turn;

        void Awake() {
            Init(); 
        }

        void Start() {
            identity = PlayerManager.Identity.ID;
            lockstep = LockstepLogic.Instance;
        }

        void OnGUI() {
            GUI.Label(new Rect(20, 20, 300, 20), "turn:" + turn);
        }

        void OnEnable() {
            GameClient.Register(NetPacketType.TurnData, OnReceiveTurnData);
        }

        void OnDisable() {
            GameClient.Unregister(NetPacketType.TurnData, OnReceiveTurnData);
        }

        public void InsertCommand(CommandBase command) {
            lockstep.AddPendingCommand(command);
        }

        public void SendPendingCommands(List<CommandBase> commands, long scheduledTurn) {
            turn = scheduledTurn;
            //Debug.Log("Sending commands...");
            PacketTurnData message = new PacketTurnData(identity, scheduledTurn, commands);
            GameClient.Send(message);
        }

        private void OnReceiveTurnData(NetPeer peer, NetEventArgs args) {
            PacketTurnData message = PacketBase.Read<PacketTurnData>((NetDataReader)(args.Data));
            //UnityEngine.Debug.Log("Received data from player " + message.Sender);
            lockstep.Insert(message.Commands, message.Turn, message.Sender);
        }
    }
}
