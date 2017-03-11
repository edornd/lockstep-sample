using Game.Network;
using LiteNetLib;
using Presentation.Network;
using UnityEngine;

public class LobbyManagerHost : MonoBehaviour {

    public void CallStartGame() {
        Debug.Log("Starting game...");
        GameServer.StartGame();
    }
}
