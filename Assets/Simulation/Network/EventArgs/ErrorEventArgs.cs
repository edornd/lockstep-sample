using System;

namespace Game.Network {
    public class ErrorEventArgs : EventArgs {
        private int code;

        public ErrorEventArgs(int code) {
            this.code = code;
        }

        public int Code {
            get { return code; }
        }
    }
}
