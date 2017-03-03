using LiteNetLib;
using System;

namespace Game.Network {
    public class DisconnectEventArgs : EventArgs {
        private DisconnectInfo info;

        public DisconnectEventArgs(DisconnectInfo info) {
            this.info = info;
        }
    }
}
