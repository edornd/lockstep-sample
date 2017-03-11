using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Presentation.Network;
using Game.Network;
using Game.Players;

public class LobbyManagerClient : MonoBehaviour {

    public void SetPlayerReady(bool value) {
        Player identity = PlayerManager.Identity;
        identity.SetReady(value);
        PacketBase readyMessage = new PacketPlayerReady(identity.ID, value);
        GameClient.NetworkClient.Send(readyMessage);
    }

    public void LeaveLobby() {
        NetEventManager.Trigger(NetEventType.LoggedOut, null);
        GameClient.Disconnect();
    }
}
