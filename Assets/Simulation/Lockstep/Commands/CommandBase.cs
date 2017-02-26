namespace Game.Lockstep {

    public abstract class CommandBase : ICommand {

        private CommandType type;
        private int source;
        private ulong turn;

        #region Properties

        public CommandType Type {
            get { return type; }
        }

        public int Source {
            get { return source; }
        }

        public ulong Turn {
            get { return turn; }
        }

        #endregion

        public virtual void Process() { }
    }
}
