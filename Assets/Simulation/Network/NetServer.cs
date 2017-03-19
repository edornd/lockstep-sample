using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace Game.Network {
    /// <summary>
    /// Wrapper around NetManager, inheriting from NetBase and implementing a simple network server.
    /// </summary>
    public class NetServer : NetBase {

        #region Private variables

        private List<NetPeer> clients;
        private Queue<NetMessage> outputMessages;
        private int listenPort;
        private bool ready;

        #endregion

        #region Constructors

        public NetServer(int port, NetConfig config) : this(port, 8, config) { }

        public NetServer(int port, int maxConnections, NetConfig config) : base (maxConnections, config){
            outputMessages = new Queue<NetMessage>();
            clients = new List<NetPeer>();
            listenPort = port;
            ready = false;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts the server by initializing the network on the given port.
        /// </summary>
        public void Start() {
            network.Start(listenPort);
            ready = true;
        }

        /// <summary>
        /// Listens to the network events.
        /// </summary>
        public void Listen() {
            if (ready) {
                network.PollEvents();
            }
        }

        /// <summary>
        /// Stops the server by stopping the net manager.
        /// </summary>
        public void Stop() {
            clients.Clear();
            network.Stop();
            ready = false;
        }

        /// <summary>
        /// Checks whether the server is currently ready to work or not.
        /// </summary>
        /// <returns>true if is ready, false otherwise</returns>
        public bool IsReady() {
            return ready;
        }

        /// <summary>
        /// Adds a message to be sent during the next game server update.
        /// </summary>
        /// <param name="message">message to be sent</param>
        public void AddOutputMessage(NetMessage message) {
            outputMessages.Enqueue(message);
        }

        /// <summary>
        /// Sends to everybody, except for the client specified into the message instance.
        /// </summary>
        /// <param name="message">message to be sent</param>
        public void AddMessageExcluding(PacketBase packet, NetPeer excluded) {
            if (excluded != null) {
                foreach (NetPeer client in clients) {
                    if (client != excluded)
                        AddOutputMessage(new NetMessage(packet, client));
                }
            }
            else {
                AddOutputMessage(new NetMessage(packet, null));
            }
        }

        /// <summary>
        /// Sends every message in the output queue.
        /// </summary>
        public void SendMessages() {
            if (!ready) {
                NetUtils.DebugWriteError("Server not ready!");
                return;
            }
            while (outputMessages.Count > 0) {
                Send(outputMessages.Dequeue());
            }
        }

        /// <summary>
        /// Disconnect the given client fro mthe server, sending the extra info.
        /// </summary>
        /// <param name="peer">client to disconnect</param>
        /// <param name="extra">serialized extra info</param>
        public void Disconnect(NetPeer peer, byte[] extra) {
            network.DisconnectPeer(peer, extra);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Sends the message to a single client, or to every client if it's not specified.
        /// </summary>
        /// <param name="message"></param>
        private void Send(NetMessage message) {
            if (message.Client != null) {
                Send(message.Client, message.Packet);
            }
            else {
                foreach (NetPeer client in clients) {
                    Send(client, message.Packet);
                }
            }
        }

        /// <summary>
        /// Sends raw data to the given client by serializing the packet.
        /// This function uses the reliable channel.
        /// </summary>
        /// <param name="client">receiver</param>
        /// <param name="packet">data to be sent</param>
        private void Send(NetPeer client, PacketBase packet) {
            writer.Reset();
            packet.Serialize(writer);
            client.Send(writer, SendOptions.ReliableOrdered);
        }

        #endregion

        #region EventListener implementation

        public override void OnPeerConnected(NetPeer peer) {
            clients.Add(peer);
            HandleEvent(NetPacketType.PeerConnect, peer, new NetEventArgs());
        }

        public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            clients.Remove(peer);
            HandleEvent(NetPacketType.PeerDisconnect, peer, new NetEventArgs(disconnectInfo));
        }

        public override void OnNetworkError(NetEndPoint endPoint, int socketErrorCode) {
            HandleEvent(NetPacketType.NetError, null, new NetEventArgs(socketErrorCode));
        }

        public override void OnNetworkReceive(NetPeer peer, NetDataReader reader) {
            NetPacketType type = (NetPacketType)reader.GetUShort();
            HandleEvent(type, peer, new NetEventArgs(reader));
        }

        public override void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType) {
            NetUtils.DebugWrite(ConsoleColor.Black,"[SERVER] Received data from unconnected peer, type: " + messageType);
        }

        public override void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
            //HandleEvent(NetPacketType.PeerLatency, peer, new NetEventArgs(latency));
        }

        #endregion
    }
}
