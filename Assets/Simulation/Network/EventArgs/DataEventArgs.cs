using LiteNetLib.Utils;
using System;

namespace Game.Network {
    public class DataEventArgs : EventArgs {
        private NetDataReader data;

        public DataEventArgs(NetDataReader reader) {
            this.data = reader;
        }

        public NetDataReader Reader {
            get { return data; }
        }
    }
}
