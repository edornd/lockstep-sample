using LiteNetLib;
using System;
using System.Collections.Generic;
using LiteNetLib.Utils;

namespace Game.Network {

    /// <summary>
    /// Delegate for network events, used to dispatch the packets handling.
    /// </summary>
    /// <param name="peer">the message sender, it can be null for some packet types</param>
    /// <param name="args">additional arguments (they can be null as well)</param>
    public delegate void MessageDelegate(NetPeer peer, NetEventArgs args);

    /// <summary>
    /// Base for a network class, inherited by NetClient and NetServer.
    /// </summary>
    public abstract class NetBase : INetEventListener {

        protected NetManager network;
        protected Dictionary<NetPacketType, MessageDelegate> handlers;
        protected MessageDelegate defaultHandler;
        protected NetDataWriter writer;

        #region Constructors 

        public NetBase(NetConfig config) : this (1, config){ }

        public NetBase(int maxConnections, NetConfig config) {
            network = new NetManager(this, maxConnections, config.Key);
            handlers = new Dictionary<NetPacketType, MessageDelegate>();
            config.Configure(network);
            writer = new NetDataWriter();
        }

        #endregion

        #region Base methods

        /// <summary>
        /// Registers the given function delegate to the given packet type.
        /// </summary>
        /// <param name="type">packet type to handle</param>
        /// <param name="handler">function delegate designed to handle it</param>
        public void Bind(NetPacketType type, MessageDelegate handler) {
            if (handlers.ContainsKey(type)) {
                handlers[type] += handler;
            }
            else {
                handlers.Add(type, handler);
            }
        }

        /// <summary>
        /// Registers the given function delegate to the default handler.
        /// </summary>
        /// <param name="handler">function delegate designed to handle unknown packets</param>
        public void BindDefault(MessageDelegate handler) {
            defaultHandler += handler;
        }

        /// <summary>
        /// Registers the given function delegate to the given packet type.
        /// </summary>
        /// <param name="type">packet type to handle</param>
        /// <param name="handler">function delegate designed to handle it</param>
        public void Unbind(NetPacketType type, MessageDelegate handler) {
            if (handlers.ContainsKey(type)) {
                handlers[type] -= handler;
            }
        }

        /// <summary>
        /// Registers the given function delegate as default handler.
        /// </summary>
        /// <param name="handler">function delegate designed to handle unknown cases</param>
        public void UnbindDefault(MessageDelegate handler) {
            defaultHandler -= handler;
        }

        /// <summary>
        /// Invokes the function handler assigned to the specified packet type, using the EventArgs object as parameter.
        /// </summary>
        /// <param name="type">packet type</param>
        /// <param name="peer">sender client</param>
        /// <param name="args">parameters for the function (raw data etc.)</param>
        protected void HandleEvent(NetPacketType type, NetPeer source, NetEventArgs args) {
            MessageDelegate handler;
            if (handlers.TryGetValue(type, out handler)) {
                handler(source, args);
            }
            else {
                if (defaultHandler != null)
                    defaultHandler.Invoke(source, args);
            }
        }

        #endregion

        #region Abstract

        public abstract void OnPeerConnected(NetPeer peer);

        public abstract void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);

        public abstract void OnNetworkError(NetEndPoint endPoint, int socketErrorCode);

        public abstract void OnNetworkReceive(NetPeer peer, NetDataReader reader);

        public abstract void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType);

        public abstract void OnNetworkLatencyUpdate(NetPeer peer, int latency);

        #endregion
    }
}
