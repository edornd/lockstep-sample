using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace Game.Network {
    /// <summary>
    /// Wrapper around NetManager implementing a client architecture.
    /// </summary>
    public class NetClient : NetBase {

        #region Private variables

        private bool connected;
        private NetPeer server;
        private Queue<PacketBase> outputMessages;
        private object outputLock;

        #endregion

        #region Constructors

        public NetClient(NetConfig config) : base (config){
            outputMessages = new Queue<PacketBase>();
            outputLock = new object();
            connected = false;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Connects to the given address and port.
        /// </summary>
        /// <param name="address">the server IP.</param>
        /// <param name="port">the server port.</param>
        public void Connect(string address, int port) {
            network.Start();
            network.Connect(address, port);
        }

        /// <summary>
        /// Disconnects the client by stopping the NetManager and resetting values.
        /// </summary>
        public void Disconnect() {
            if (network != null) {
                server = null;
                connected = false;
                lock (outputLock) {
                    outputMessages.Clear();
                }
                network.Stop();
            }
        }

        /// <summary>
        /// Listens to the network, polling the events.
        /// </summary>
        public void Receive() {
            network.PollEvents();
        }

        /// <summary>
        /// Send every message waiting in queue.
        /// </summary>
        public void SendMessages() {
            if (!connected || server.ConnectionState != ConnectionState.Connected)
                return;
            lock (outputLock) {
                while (outputMessages.Count > 0) {
                    Send(outputMessages.Dequeue());
                }
            }
        }

        /// <summary>
        /// Adds a new message to be sent.
        /// </summary>
        /// <param name="packet">Message to send</param>
        public void AddOutputMessage(PacketBase packet) {
            lock (outputLock) {
                this.outputMessages.Enqueue(packet);
            }
        }

        /// <summary>
        /// Sends the given packet to the server, using a reliable QoS.
        /// </summary>
        /// <param name="packet">the packet to send</param>
        public void Send(PacketBase packet) {
            if (!connected) {
                NetUtils.DebugWriteError("Cannot send, client not connected!");
                return;
            }
            writer.Reset();
            packet.Serialize(writer);
            server.Send(writer, SendOptions.ReliableUnordered);
        }

        #endregion

        #region NetListener implementation

        public override void OnPeerConnected(NetPeer peer) {
            server = peer;
            connected = true;
            HandleEvent(NetPacketType.PeerConnect, peer, null);
        }

        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            Disconnect();
            HandleEvent(NetPacketType.PeerDisconnect, peer, new NetEventArgs(disconnectInfo));
        }

        public override void OnNetworkError(NetEndPoint endPoint, int socketErrorCode) {
            HandleEvent(NetPacketType.NetError, server, new NetEventArgs(socketErrorCode));
        }

        public override void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
            //HandleEvent(NetPacketType.PeerLatency, peer, new NetEventArgs(latency));
        }

        public override void OnNetworkReceive(NetPeer peer, NetDataReader reader) {
            NetPacketType type = (NetPacketType)reader.GetUShort();
            HandleEvent(type, peer, new NetEventArgs(reader));
        }

        public override void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType) {
            NetUtils.DebugWriteError("Received a discovery request, ignoring.");
        }

        #endregion
    }
}
