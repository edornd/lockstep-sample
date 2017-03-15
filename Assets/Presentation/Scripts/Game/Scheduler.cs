using Game.Lockstep;
using Game.Network;
using LiteNetLib;
using LiteNetLib.Utils;
using Presentation.Network;
using UnityEngine;

namespace Presentation {
    public class Scheduler : SingletonMono<Scheduler> {

        public uint turnOffset = 2;

        private int identity;
        private PacketTurnDone lastDoneSent;
        private string turnText;
        private string lastDone;

        void OnEnable() {
            Init();
            identity = PlayerManager.Identity.ID;
            GameClient.Register(NetPacketType.GameCmd, OnIssueCommand);
            GameClient.Register(NetPacketType.TurnDone, OnTurnDoneReceived);
        }

        void OnDisable() {
            GameClient.Unregister(NetPacketType.GameCmd, OnIssueCommand);
            GameClient.Unregister(NetPacketType.TurnDone, OnTurnDoneReceived);

        }

        void OnGUI() {
            GUI.Label(new Rect(10, 10, 300, 20), turnText);
            GUI.Label(new Rect(10, 20, 300, 20), lastDone);
        }

        #region Singleton

        public static void SendTurnDone(ulong currentTurn) {
            instance.SendDone(currentTurn);
            instance.UpdateTurn(currentTurn);
        }

        #endregion

        private void OnIssueCommand(NetPeer peer, NetEventArgs args) {
            PacketGameCmd packet = PacketBase.Read<PacketGameCmd>((NetDataReader)(args.Data));
            Debug.Log("Received: " + packet.Command.Type);
            LockstepLogic.Insert(packet.Command, packet.Sender);
        }

        private void OnTurnDoneReceived(NetPeer peer, NetEventArgs args) {
            PacketTurnDone packet = PacketBase.Read<PacketTurnDone>((NetDataReader)(args.Data));
            Debug.Log("Received done for turn: " + packet.Turn);
            lastDone = "Player " + packet.Sender + " did turn: " + packet.Turn;
            LockstepLogic.SetPlayerDone(packet.Sender);
        }

        public void IssueCommand(CommandBase command) {
            //Debug.Log("Sending command: " + command.Type);
            LockstepLogic.Insert(command, identity);
            GameClient.Send(new PacketGameCmd(identity, command));
        }

        private void SendDone(ulong currentTurn) {
            if (lastDoneSent == null || lastDoneSent.Turn < currentTurn) {
                Debug.Log("Sending done for turn: " + currentTurn);
                UpdateTurn(currentTurn);
                PacketTurnDone doneMsg = new PacketTurnDone(identity, currentTurn);
                GameClient.Send(doneMsg);
                lastDoneSent = doneMsg;
            }
        }

        private void UpdateTurn(ulong currentTurn) {
            turnText = "Turn ID: " + currentTurn;
        }
    }
}
