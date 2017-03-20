using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Presentation.Network;
using Game.Network;

namespace Presentation.UI {
    public class UIMenuMultiplayer : MonoBehaviour {

        public InputField usernameField;
        public InputField addressField;
        public Button hostGameButton;
        public Button connectButton;

        private string username;
        private string address;
        private LoginManager loginHandler;

        #region Monobehaviour

        void OnEnable() {
            addressField.onValidateInput += OnValidateChar;
            NetEventManager.AddListener(NetEventType.Connected, OnConnectionEvent);
            NetEventManager.AddListener(NetEventType.Authenticated, OnAuthenticationEvent);
            NetEventManager.AddListener(NetEventType.LoggedIn, OnLoginEvent);
            NetEventManager.AddListener(NetEventType.Disconnected, OnConnectionFailed);
        }

        void OnDisable() {
            addressField.onValidateInput -= OnValidateChar;
            NetEventManager.RemoveListener(NetEventType.Connected, OnConnectionEvent);
            NetEventManager.RemoveListener(NetEventType.Authenticated, OnAuthenticationEvent);
            NetEventManager.RemoveListener(NetEventType.LoggedIn, OnLoginEvent);
            NetEventManager.RemoveListener(NetEventType.Disconnected, OnConnectionFailed);
        }

        void Start() {
            loginHandler = GetComponent<LoginManager>();
        }

        #endregion

        public void OnUsernameChanged() {
            username = usernameField.text;
        }

        public void OnAddressChanged() {
            address = addressField.text;
        }

        public void OnHostGameButtonPressed() {
            OnUsernameChanged();
            if (ValidateUsername()) {
                loginHandler.HostGame(username);
            }
        }

        //TODO reset localhost to address
        public void OnConnectButtonPressed() {
            OnUsernameChanged();
            OnAddressChanged();
            if (ValidateAddressIPv4() && ValidateUsername()) {
                loginHandler.Login(username, "localhost");
            }
        }

        private void OnConnectionEvent(NetEventArgs args) {
            print("Successfully connected, logging in...");
        }

        private void OnAuthenticationEvent(NetEventArgs args) {
            print("Successfully authenticated!");
        }

        private void OnLoginEvent(NetEventArgs args) {
            print("Successfully logged in!");
            if (GameClient.IsHost)
                SceneManager.LoadScene(2);
            else {
                SceneManager.LoadScene(1);
            }
        }

        private void OnConnectionFailed(NetEventArgs args) {
            print("Connection failed: " + args.Data.ToString());
        }

        public char OnValidateChar(string text, int charIndex, char addedChar) {
            char current = '\0';
            if (addedChar == '.' || char.IsDigit(addedChar)) {
                current = addedChar;
            }
            return current;
        }

        private bool ValidateAddressIPv4() {
            if (string.IsNullOrEmpty(address)) {
                Debug.LogWarning("Invalid ip 1");
                return false;
            }

            string[] parts = address.Split('.');
            if (parts.Length != 4) {
                Debug.LogWarning("Invalid ip 2");
                return false;
            }

            for (uint i = 0; i < parts.Length; i++) {
                int num;
                if (!int.TryParse(parts[i], out num) || num.ToString().Length != parts[i].Length || num < 0 || num > 255) {
                    Debug.LogWarning("Invalid ip 3");
                    return false;
                }
            }
            Debug.Log("Valido");
            return true;
        }

        private bool ValidateUsername() {
            if (string.IsNullOrEmpty(username)) {
                Debug.LogWarning("Invalid username");
                return false;
            }
            return username.Length <= 50;
        }  
    }
}
