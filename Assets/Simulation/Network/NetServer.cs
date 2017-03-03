using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace Game.Network {
    public class NetServer : INetEventListener {

        public delegate void MessageDelegate(NetPeer client, EventArgs e);

        #region Variables

        private int port;
        private NetManager network;
        private List<NetPeer> clients;
        private Dictionary<NetPacketType, MessageDelegate> handlers;
        private Queue<NetMessage> outputMessages;
        private bool ready;

        #endregion

        #region Constructors

        public NetServer(int port, string connectKey) : this(port, connectKey, 8) { }

        public NetServer(int port, string connectKey, int maxConnections) {
            this.port = port;
            network = new NetManager(this, maxConnections, connectKey);
            clients = new List<NetPeer>();
            handlers = new Dictionary<NetPacketType, MessageDelegate>();
            outputMessages = new Queue<NetMessage>();
            network.UpdateTime = 15;
            network.DiscoveryEnabled = true;
            ready = false;
        }

        #endregion

        #region Public methods

        public void Start() {
            network.Start(port);
            ready = true;
        }

        public void Listen() {
            if (ready) {
                network.PollEvents();
            }
        }

        public void Stop() {
            if (network != null) {
                clients.Clear();
                network.Stop();
                ready = false;
            }
        }

        public bool IsReady() {
            return ready;
        }

        public void Bind(NetPacketType type, MessageDelegate handler) {
            if (handlers.ContainsKey(type)) {
                handlers[type] += handler;
            }
            else {
                handlers.Add(type, handler);
            }
        }

        public void SendMessages() {
            if (!ready) {
                Debug.LogWarning("Server not ready!");
                return;
            }
            while (outputMessages.Count > 0) {
                Send(outputMessages.Dequeue());
            }
        }

        public void Send(NetMessage message) {
            if (message.Client != null) {
                Send(message.Client, message.Packet);
            }
            else {
                foreach (NetPeer client in clients) {
                    Send(client, message.Packet);
                }
            }
        }

        #endregion

        #region Private methods

        private void Send(NetPeer client, PacketBase packet) {
            NetDataWriter writer = new NetDataWriter();
            packet.Serialize(writer);
            client.Send(writer, SendOptions.ReliableUnordered);
        }

        private void HandleEvent(NetPacketType type, NetPeer peer, EventArgs args) {
            MessageDelegate handler;
            if (handlers.TryGetValue(type, out handler)) {
                handler.Invoke(peer, args);
            }
            else {
                Debug.LogWarning("No handler designed for the packet type: " + type);
            }
        }

        #endregion

        #region EventListener implementation

        public void OnPeerConnected(NetPeer peer) {
            clients.Add(peer);
            HandleEvent(NetPacketType.PeerConnect, peer, null);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            clients.Remove(peer);
            HandleEvent(NetPacketType.PeerDisconnect, peer, new DisconnectEventArgs(disconnectInfo));
        }

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode) {
            HandleEvent(NetPacketType.NetError, null, new ErrorEventArgs(socketErrorCode));
        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader) {
            NetPacketType type = (NetPacketType)reader.GetUShort();
            HandleEvent(type, peer, new DataEventArgs(reader));
        }

        public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType) {
            Debug.Log("[SERVER] Received data from unconnected peer, type: " + messageType);
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {

        }

        #endregion
    }
}
