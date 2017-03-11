using UnityEngine;
using UnityEngine.UI;

public class UIMenuLobbyHost : MonoBehaviour {

    public Button startGameButton;
    public Button closeLobbyButton;
    public Toggle readyToggle;

    #region Monobehaviour

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    #endregion

    #region UI Events

    public void OnReadyToggleChanged() {
        Debug.Log("Value: " + readyToggle.isOn);
    }

    public void OnStartGameButtonPressed() {
        Debug.Log("Starting game");
    }

    public void OnCloseLobbyButtonPressed() {
        Debug.Log("Close lobby");
    }

    #endregion
}
