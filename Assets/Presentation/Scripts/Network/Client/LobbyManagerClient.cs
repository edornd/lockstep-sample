using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Presentation.Network;
using Game.Network;
using Game.Players;
using UnityEngine.SceneManagement;

public class LobbyManagerClient : MonoBehaviour {

    void OnEnable() {
        GameClient.Register(NetPacketType.GameStart, OnGameStart);
    }

    void OnDisable() {
        GameClient.Unregister(NetPacketType.GameStart, OnGameStart);
    }

    public void SetPlayerReady(bool value) {
        Player identity = PlayerManager.Identity;
        identity.SetReady(value);
        PacketBase readyMessage = new PacketPlayerReady(identity.ID, value);
        GameClient.Send(readyMessage);
    }

    public void LeaveLobby() {
        NetEventManager.Trigger(NetEventType.LoggedOut, null);
        GameClient.Disconnect();
    }

    private void OnGameStart(NetPeer peer, NetEventArgs args) {
        Debug.Log("[CLIENT] Game start received!");
        GameClient.CurrentState = ClientState.Game;
        SceneManager.LoadSceneAsync(3);
    }
}
