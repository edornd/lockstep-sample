using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Presentation.Network;
using Game.Network;
using Game.Players;

public class LobbyManagerClient : MonoBehaviour {

    public void SetPlayerReady() {
        Player identity = PlayerManager.Identity;
        identity.SetReady(true);
        PacketBase readyMessage = new PacketPlayerReady(identity.ID, true);
        GameClient.NetworkClient.Send(readyMessage);
    }

    public void LeaveLobby() {
        NetEventManager.Trigger(NetEventType.LoggedOut, null);
        GameClient.Disconnect();
    }
}
