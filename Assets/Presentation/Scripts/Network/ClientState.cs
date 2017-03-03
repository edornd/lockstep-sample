using UnityEngine;
using System.Collections;
namespace Presentation.Network { 
    public enum ClientState {
        Disconnected,
        Connecting,
        Connected,
        LoggedIn,
        LoggedOut
    }
}
