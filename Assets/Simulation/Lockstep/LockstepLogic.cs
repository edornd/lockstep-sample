namespace Game.Lockstep {
    /// <summary>
    /// Class implementing the main loop for the lockstep logic.
    /// </summary>
    public class LockstepLogic : EntityBase {

        #region Variables

        private ulong turnID;
        private uint frame;
        private readonly uint maxFrames = 4;
        private LockstepClient client;
        private CommandBuffer buffer;

        #endregion

        #region Singleton - Static methods

        private static LockstepLogic instance = new LockstepLogic();

        public LockstepLogic Instance {
            get { return instance; }
        }

        public static ulong CurrentTurn {
            get { return instance.turnID; }
        }

        #endregion

        #region Constructors

        public LockstepLogic() : base() {
            turnID = 0;
            frame = 0;
            client = new LockstepClient(12010, "127.0.0.1", 28960);
            buffer = new CommandBuffer((int)maxFrames);
        }

        #endregion

        #region Lockstep

        public override void Update() {
            if (IsLockstepTurn()) {
                if (!IsLockstepReady())
                    return;
                //process commands, physics...
                UnityEngine.Debug.Log("Lockstep turn: " + turnID);
                NextTurn();
            }
            NextFrame();
        }

        /// <summary>
        /// Advance to the next frame, the counter needs to be from 0 to 3.
        /// </summary>
        private void NextFrame() {
            frame = (frame + 1) % maxFrames;
        }

        /// <summary>
        /// Advance to the next turn.
        /// </summary>
        private void NextTurn() {
            turnID++;
        }

        /// <summary>
        /// Check if the current game frame is a lockstep frame.
        /// Lockstep frames are executed every "maxFrames" times to process every command.
        /// </summary>
        /// <returns></returns>
        private bool IsLockstepTurn() {
            return frame == 0;
        }

        /// <summary>
        /// Check the conditions for the next turn.
        /// 1 - every command was successfully received.
        /// 2 - ...
        /// </summary>
        /// <returns></returns>
        private bool IsLockstepReady() {
            return buffer.IsReady();
        }

        #endregion

    }
}
