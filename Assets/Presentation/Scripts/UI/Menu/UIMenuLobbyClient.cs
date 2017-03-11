using UnityEngine;
using UnityEngine.UI;
using Game.Players;
using Game.Network;
using Presentation.Network;
using UnityEngine.SceneManagement;

namespace Presentation.UI {
    public class UIMenuLobbyClient : MonoBehaviour {

        public VerticalLayoutGroup playersPanel;
        public Toggle readyToggle;
        public GameObject elementPrefab;

        private LobbyManagerClient lobbyManager;

        #region Monobehaviour

        void OnEnable() {
            NetEventManager.AddListener(NetEventType.PlayerEnter, OnPlayerEnterOrLeave);
            NetEventManager.AddListener(NetEventType.PlayerLeave, OnPlayerEnterOrLeave);
            NetEventManager.AddListener(NetEventType.PlayerReady, OnPlayerReady);
            NetEventManager.AddListener(NetEventType.LoggedOut, OnGameLogout);
            NetEventManager.AddListener(NetEventType.Disconnected, OnGameDisconnected);
        }

        void Start() {
            UpdatePlayersList();
            lobbyManager = GetComponent<LobbyManagerClient>();
        }

        void OnDisable() {
            NetEventManager.RemoveListener(NetEventType.PlayerEnter, OnPlayerEnterOrLeave);
            NetEventManager.RemoveListener(NetEventType.PlayerLeave, OnPlayerEnterOrLeave);
            NetEventManager.RemoveListener(NetEventType.PlayerReady, OnPlayerReady);
            NetEventManager.RemoveListener(NetEventType.LoggedOut, OnGameLogout);
            NetEventManager.RemoveListener(NetEventType.Disconnected, OnGameDisconnected);
        }

        #endregion

        #region UI Events

        public void OnReadyTogglePressed() {
            lobbyManager.SetPlayerReady(readyToggle.isOn);
            UpdatePlayersList();
        }

        public void OnLeaveButtonPressed() {
            lobbyManager.LeaveLobby();
        }

        #endregion

        #region Event Listeners

        private void OnPlayerEnterOrLeave(NetEventArgs args) {
            UpdatePlayersList();
        }

        private void OnPlayerReady(NetEventArgs args) {
            UpdatePlayersList();
        }

        private void OnGameLogout(NetEventArgs args) {
            Debug.Log("Leaving the game...");
        }

        private void OnGameDisconnected(NetEventArgs args) {
            Debug.Log("Disconnected from the game.");
            SceneManager.LoadSceneAsync(0);
        }

        #endregion

        private void UpdatePlayersList() {
            ResetPanel();
            foreach (Player player in PlayerManager.Players.Values) {
                GameObject labelObj = Instantiate(elementPrefab, playersPanel.transform, false) as GameObject;
                Text label = labelObj.GetComponentInChildren<Text>();
                label.text = player.ToString();
                if (player.ID == PlayerManager.Identity.ID) {
                    Image background = labelObj.GetComponentInChildren<Image>();
                    background.color = Color.magenta;
                }
            }
        }

        private void ResetPanel() {
            for (int i = playersPanel.transform.childCount - 1; i >= 0; i--) {
                Destroy(playersPanel.transform.GetChild(i).gameObject);
            }
            transform.DetachChildren();
        }
    }
}
