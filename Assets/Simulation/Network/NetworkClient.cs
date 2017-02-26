using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network {
    /// <summary>
    /// Wrapper around NetManager implementing a client architecture.
    /// </summary>
    public class NetClient : INetEventListener {

        //function delegate for packet handling
        public delegate void MessageDelegate(NetDataReader reader);

        #region Private variables

        private int listenPort;
        private bool connected;
        private NetManager network;
        private NetPeer server;
        private Dictionary<NetPacketType, MessageDelegate> handlers;
        private Queue<NetPacket> outputMessages;

        #endregion

        #region Constructors

        public NetClient(int port, int rate, string key) {
            network = new NetManager(this, key);
            connected = false;
            listenPort = port;
            network.UpdateTime = rate;
            network.NatPunchEnabled = true;
            handlers = new Dictionary<NetPacketType, MessageDelegate>();
            outputMessages = new Queue<NetPacket>();
            network.Start(listenPort);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Connects to the given address and port.
        /// </summary>
        /// <param name="address">the server IP.</param>
        /// <param name="port">the server port.</param>
        public void Connect(string address, int port) {
            network.Connect(address, port);
        }

        /// <summary>
        /// Disconnects the client by stopping the NetManager.
        /// </summary>
        public void Disconnect() {
            if (network != null) {
                server = null;
                connected = false;
                network.Stop();
            }
        }

        /// <summary>
        /// Listens to the network, polling the events.
        /// </summary>
        public void Receive() {
            if (connected && server.ConnectionState == ConnectionState.Connected) {
                network.PollEvents();
            }
        }

        /// <summary>
        /// Send every message waiting in queue.
        /// </summary>
        public void SendMessages() {
            if (!connected)
                return;
            while (outputMessages.Count > 0) {
                Send(outputMessages.Dequeue());
            }
        }

        /// <summary>
        /// Adds the given function delegate to the dictionary.
        /// </summary>
        /// <param name="type">the packet type</param>
        /// <param name="handler">the function used to handle the given packet type</param>
        public void Bind(NetPacketType type, MessageDelegate handler) {
            if (handlers.ContainsKey(type)) {
                handlers[type] += handler;
            }
            else {
                handlers.Add(type, handler);
            }
        }

        /// <summary>
        /// Adds a new message to be sent.
        /// </summary>
        /// <param name="packet">Message to send</param>
        public void AddOutputMessage(NetPacket packet) {
            this.outputMessages.Enqueue(packet);
        }

        /// <summary>
        /// Sends the given packet to the server, using a reliable QoS.
        /// </summary>
        /// <param name="packet">the packet to send</param>
        public void Send(NetPacket packet) {
            if (!connected) {
                Debug.LogWarning("Cannot send, client not connected");
                return;
            }
            NetDataWriter writer = new NetDataWriter();
            packet.Serialize(writer);
            server.Send(writer, SendOptions.ReliableUnordered);
        }

        #endregion

        #region NetListener implementation

        public void OnPeerConnected(NetPeer peer) {
            Debug.Log("Client successfully connected to the host: " + peer.EndPoint);
            server = peer;
            connected = true;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            Debug.Log("Conection error. Server down?");
            connected = false;
        }

        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode) {
            Debug.Log("Received error with code: " + socketErrorCode);
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {

        }

        public void OnNetworkReceive(NetPeer peer, NetDataReader reader) {
            NetPacketType type = (NetPacketType)reader.GetUShort();
            MessageDelegate handler;
            if (handlers.TryGetValue(type, out handler)) {
                handler.Invoke(reader);
            }
            else {
                Debug.LogError("No handler designed for the packet type: " + type);
            }
        }

        public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType) {
            Debug.LogWarning("Received a discovery request, ignoring.");
        }

        #endregion
    }
}
