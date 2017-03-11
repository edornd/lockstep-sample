using UnityEngine;
using UnityEngine.UI;

public class UIMenuLobbyHost : MonoBehaviour {

    public Button startGameButton;
    private LobbyManagerHost hostManager;

    #region Monobehaviour

    void Start () {
        hostManager = GetComponent<LobbyManagerHost>();
	}

    #endregion

    #region UI Events

    public void OnStartGameButtonPressed() {
        hostManager.CallStartGame();
    }

    #endregion
}
