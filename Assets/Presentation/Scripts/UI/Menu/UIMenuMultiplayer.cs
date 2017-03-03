using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Presentation.Network;
using Game.Utils;
using Game.Network.Players;

namespace Presentation.UI {
    public class UIMenuMultiplayer : MonoBehaviour, IObserver {

        public InputField usernameField;
        public InputField addressField;
        public Button hostGameButton;
        public Button connectButton;
        public GameObject networkHolder;

        private string username;
        private string address;
        private LockstepClient client;
        private LockstepServer server;

        void Start() {
            addressField.onValidateInput += OnValidateChar;
            client = networkHolder.GetComponent<LockstepClient>();
            client.Subscribe(this);
        }

        public void Signal(IObservable ob, object args) {
            switch (client.CurrentState) {
                case ClientState.Connected:
                    OnConnectionEvent();
                    break;
                case ClientState.LoggedIn:
                    OnLogInEvent();
                    break;
                default:
                    break;
            }
        }

        public void OnUsernameChanged() {
            username = usernameField.text;
        }

        public char OnValidateChar(string text, int charIndex, char addedChar) {
            char current = '\0';
            if (addedChar == '.' || char.IsDigit(addedChar)) {
                current = addedChar;
            }
            return current;
        }

        public void OnAddressChanged() {
            address = addressField.text;
        }

        public void OnHostGamePressed() {
            OnUsernameChanged();
            if (ValidateUsername()) {
                server = networkHolder.AddComponent<LockstepServer>();
                PlayerManager.Instance.SetIdentity(new Player(0, username));
                client.Connect("localhost", server.port);
            }
        }

        public void OnConnectPressed() {
            OnUsernameChanged();
            OnAddressChanged();
            if (ValidateAddressIPv4() && ValidateUsername()) {
                PlayerManager.Instance.SetIdentity(new Player(0, username));
                client.Connect(address, 28960);
            }
        }

        private bool ValidateAddressIPv4() {
            if (string.IsNullOrEmpty(address))
                return false;

            string[] parts = address.Split('.');
            if (parts.Length != 4)
                return false;

            for (uint i = 0; i < parts.Length; i++) {
                int num;
                if (!int.TryParse(parts[i], out num) || num.ToString().Length != parts[i].Length || num < 0 || num > 255) {
                    return false;
                }
            }
            return true;
        }

        private bool ValidateUsername() {
            if (string.IsNullOrEmpty(username)) {
                Debug.LogWarning("Invalid username");
                return false;
            }
            return username.Length <= 50;
        }

        private void OnConnectionEvent() {
            print("successfuly connected, logging in...");
        }

        private void OnLogInEvent() {
            print("successfully logged in!");
            SceneManager.LoadSceneAsync(1);
        }
    }
}
