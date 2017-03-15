using UnityEngine.Events;
using System.Collections.Generic;
using Game.Network;
using Game.Utils;
using UnityEngine;

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

public class NetEventManager : SingletonMono<NetEventManager> {

    private Dictionary<NetEventType, NetworkEvent> events;

    #region Monobehaviour - Instance methods

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

    #endregion

    #region Singleton

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
            if (e.GetPersistentEventCount() == 0) {
               instance.events.Remove(type);
            }
        }
    }

    public static void Trigger(NetEventType eventName, NetEventArgs args) {
        NetworkEvent e = null;
        if (instance.events.TryGetValue(eventName, out e)) {
            e.Invoke(args);
        }
    }

    #endregion
}
