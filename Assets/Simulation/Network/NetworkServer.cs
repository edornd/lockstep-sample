using UnityEngine;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Text;

public class NetworkServer : MonoBehaviour, INetEventListener {

    public int listenPort       = 28960;
    public int maxConnections   = 8;
    public string connectKey    = "abc_123";

    private NetManager network;
    private List<NetPeer> clients;

	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
        clients = new List<NetPeer>();
        network = new NetManager(this, maxConnections, connectKey);
        network.UpdateTime = 15;
        network.DiscoveryEnabled = true;
        //network.NatPunchEnabled = true;
        network.Start(listenPort);
    }

    // Update is called once per frame
    void Update () {
        network.PollEvents();
	}

    void OnDestroy() {
        if (network != null) {
            clients.Clear();
            network.Stop();
        }
    }

    public void OnPeerConnected(NetPeer peer) {
        Debug.Log("[SERVER] Accepted connection: " + peer.EndPoint);
        clients.Add(peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Debug.Log("[SERVER] Client disconnected: " + peer.EndPoint + " info: " + disconnectInfo.Reason);
        clients.Remove(peer);
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode) {
        Debug.Log("[SERVER] Error: " + endPoint + " - code: " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader) {
        Debug.Log("[SERVER] Received data from: " + peer.EndPoint);
        Debug.Log("[SERVER] Message: " + Encoding.ASCII.GetString(reader.Data));
        NetDataWriter writer = new NetDataWriter();
        writer.Put(reader.Data);
        foreach (NetPeer client in clients) {
                Debug.Log("[SERVER] Sending to: " + client.EndPoint);
                client.Send(writer, SendOptions.ReliableUnordered);
        }
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType) {
        Debug.Log("[SERVER] Received data from unconnected peer, type: " + messageType);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {

    }
}
