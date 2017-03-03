using UnityEngine;
using UnityEngine.UI;
using Presentation.Network;
using Game.Network.Players;

namespace Presentation.UI {
    public class UIMenuLobby : MonoBehaviour {

        public GameObject networkHolder;
        public VerticalLayoutGroup playersPanel;
        public GameObject elementPrefab;

        private LockstepClient client;
        private PlayerManager playerManager;

        void Awake() {
            networkHolder = GameObject.FindGameObjectWithTag("Network");
            client = networkHolder.GetComponent<LockstepClient>();
            playerManager = PlayerManager.Instance;
        }

        // Use this for initialization
        void Start() {
            foreach(Player player in playerManager.Players.Values) {
                AddPlayer(player);
            }
        }

        // Update is called once per frame
        void Update() {

        }

        private void AddPlayer(Player player) {
            GameObject labelObj = Instantiate(elementPrefab, playersPanel.transform, false) as GameObject;
            Text label = labelObj.GetComponentInChildren<Text>();
            label.text = player.ToString();
            if (player.ID == playerManager.Identity.ID) {
                Image background = labelObj.GetComponentInChildren<Image>();
                background.color = Color.yellow;
            }
        }
    }
}
