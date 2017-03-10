using Game.Network;
using UnityEngine.Events;
using System.Collections.Generic;

public enum NetEventType {
    Connected,
    Disconnected,
    Authenticated,
    LoggedIn,
    LoggedOut,
    PlayerEnter,
    PlayerLeave,
    PlayerReady,
    NetworkError
}

public class NetworkEvent : UnityEvent<NetEventArgs> { }

public class NetEventManager : Singleton<NetEventManager> {

    private Dictionary<NetEventType, NetworkEvent> events;

	void Awake () {
        events = new Dictionary<NetEventType, NetworkEvent>();
        Init();
	}

    void OnDestroy() {
        foreach (UnityEvent<NetEventArgs> e in events.Values) {
            e.RemoveAllListeners();
        }
        events = null;
    }

    public static void AddListener(NetEventType type, UnityAction<NetEventArgs> listener) {
        NetworkEvent e = null;
        if (instance.events.TryGetValue(type, out e)) {
            e.AddListener(listener);
        }
        else {
            e = new NetworkEvent();
            e.AddListener(listener);
            instance.events.Add(type, e);
        }
    }

    public static void RemoveListener(NetEventType type, UnityAction<NetEventArgs> listener) {
        if (!instance) return;

        NetworkEvent e = null;
        if (instance.events.TryGetValue(type, out e)) {
            e.RemoveListener(listener);
        }
    }

    public static void Trigger(NetEventType eventName, NetEventArgs args) {
        NetworkEvent e = null;
        if (instance.events.TryGetValue(eventName, out e)) {
            e.Invoke(args);
        }
    }
}
