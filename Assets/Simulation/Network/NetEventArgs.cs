using System;

namespace Game.Network {
    public class NetEventArgs : EventArgs{
        private object data;

        public NetEventArgs() {
            data = null;
        }

        public NetEventArgs(object data) {
            this.data = data;
        }

        public object Data {
            get { return data; }
        }
    }
}
