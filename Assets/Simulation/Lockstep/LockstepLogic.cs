using Game.Utils;
using Presentation;
using Presentation.Network;

namespace Game.Lockstep {
    /// <summary>
    /// Class implementing the main loop for the lockstep logic.
    /// </summary>
    public class LockstepLogic : Singleton<LockstepLogic>, IGameBehaviour {

        #region Variables
        private int identity;

        private ulong turnID;
        private ulong lastTurnID;

        private uint frame;
        private readonly uint maxFrames = 10;
        private bool[] playersReady;
        private CommandBuffer buffer;

        private object lockTurnID;
        private object lockBuffer;
        private object lockReady;

        #endregion

        #region Singleton - Static methods

        public static ulong CurrentTurn { get { return instance.CurrentTurnInternal; } }

        public static void Insert(CommandBase command, int source) {
            instance.InsertInternal(command, source);
        }

        public static void SetPlayerDone(int id) {
            instance.SetPlayerDoneInternal(id);
        }

        #endregion

        #region Constructors

        public LockstepLogic() {
            buffer = new CommandBuffer((int)maxFrames, PlayerManager.PlayerCount);
            playersReady = new bool[PlayerManager.PlayerCount];
            identity = PlayerManager.Identity.ID;
            lockTurnID = new object();
            lockBuffer = new object();
            lockReady = new object();
        }

        #endregion

        #region Lockstep

        public void Init() {
            turnID = 0;
            frame = 0;
        }

        public void Update() {
            if (IsLockstepTurn()) {
                SendTurnDone();
                if (!IsLockstepReady())
                    return;
                //process commands, physics...
                CommandList commands = buffer.Advance();
                //UnityEngine.Debug.Log(commands);
                NextTurn();
            }
            NextFrame();
        }

        public void Quit() { }

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
            lock (lockTurnID) {
                lastTurnID = turnID;
                turnID++;
            }
        }

        private void SendTurnDone() {
            ulong current = CurrentTurn;
            SetPlayerDoneInternal(identity);
            Scheduler.SendTurnDone(current);
        }

        /// <summary>
        /// Check if the current game frame is a lockstep frame.
        /// Lockstep frames are executed every "maxFrames" times to process every command.
        /// </summary>
        /// <returns></returns>
        private bool IsLockstepTurn() {
            ulong current = CurrentTurn;
            if (current == lastTurnID && current != 0)
                return false;
            return frame == 0;
        }

        /// <summary>
        /// Check the conditions for the next turn.
        /// 1 - every command was successfully received.
        /// 2 - every player sent done.
        /// </summary>
        /// <returns></returns>
        private bool IsLockstepReady() {
            if (CurrentTurn == 0)
                return true;

            lock (lockReady) {
                UnityEngine.Debug.Log("are they ready?");
                for (int i = 0; i < playersReady.Length; i++) {
                    if (!playersReady[i]) {
                        UnityEngine.Debug.LogWarning("player " + (i + 1) + " not ready!");
                        return false;
                    }
                }
                //if we got here everybody was ready, reset.
                for (int i= 0; i < playersReady.Length; i++){
                    playersReady[i] = false;
                }
            }
            return true;
            //return buffer.IsReady();
        }

        #endregion

        #region Instance functions

        private ulong CurrentTurnInternal {
            get {
                lock (lockTurnID) {
                    return turnID;
                }
            }
        }

        /// <summary>
        /// Instance function called by the static reference.
        /// </summary>
        /// <param name="command">comand to be added</param>
        /// <param name="source">player that sent it</param>
        private void InsertInternal(CommandBase command, int source) {
            lock (lockBuffer) {
                buffer.Insert(command, source);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="turn"></param>
        private void SetPlayerDoneInternal(int id) {
            lock(lockReady) {
                playersReady[id - 1] = true;
            }
        }

        #endregion

    }
}
