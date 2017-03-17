using System;
using Game.Utils;
using Presentation;
using Presentation.Network;
using System.Collections.Generic;

namespace Game.Lockstep {
    /// <summary>
    /// Class implementing the main loop for the lockstep logic.
    /// </summary>
    public class LockstepLogic : Singleton<LockstepLogic>, IGameBehaviour {

        #region Variables
        private int identity;

        private long currentTurn;
        private long lastTurn;

        private int currentFrame;
        private int framesPerTurn;
        private CommandBuffer buffer;
        private TurnData currentData;
        private List<Command> pendingCommands;

        private object lockTurn;
        private object lockPending;

        #endregion

        #region Constructors

        public LockstepLogic(int frames) {
            framesPerTurn = frames;
            identity = PlayerManager.Identity.ID;
            buffer = new CommandBuffer(2, PlayerManager.PlayerCount);
            pendingCommands = new List<Command>();

            lockTurn = new object();
            lockPending = new object();
        }

        #endregion

        #region Game Behaviour

        /// <summary>
        /// Initializes the lockstep variables.
        /// The turn starts from -2 to allow commands to be scheduled for turn 0.
        /// </summary>
        public void Init() {
            currentTurn = -2;
            lastTurn = -3;
            currentFrame = 0;
        }

        /// <summary>
        /// Tries to step forward with the game.
        /// </summary>
        public void Update() {
            if (IsLockstepturn()) {
                SendPendingCommands();
                //Execute an actual turn only after the first two starting rounds
                if (currentTurn >= 0) {
                    if (!IsLockstepReady()) {
                        //state: delay
                        UnityEngine.Debug.LogWarning("Not ready for next turn!");
                        return;
                    }
                    currentData = buffer.Advance();
                    //process commands checking integrity
                    currentData.ProcessCommands();
                    //we are ready to step forward
                    NextTurn();
                }
                //we are still in the starting turns, advance without processing
                else {
                    NextTurn();
                }
            }
            NextFrame();
        }

        /// <summary>
        /// Deletes everything lockstep related.
        /// </summary>
        public void Quit() {

        }

        #endregion

        #region Lockstep

        private long CurrentTurn {
            get {
                lock (lockTurn) {
                    return currentTurn;
                }
            }
        }

        private bool IsLockstepturn() {
            return currentFrame == 0;
        }

        private bool IsLockstepReady() {
            return buffer.IsReadyToAdvance();
        }

        private void NextTurn() {
            lock (lockTurn) {
                lastTurn = currentTurn;
                currentTurn++;
            }
        }

        private void NextFrame() {
            currentFrame = (currentFrame + 1) % framesPerTurn;
        }

        private void SendPendingCommands() {
            lock (lockPending) {
                long turn = CurrentTurn+2;
                Scheduler.Instance.SendPendingCommands(pendingCommands, turn);
                Insert(pendingCommands, turn, identity);
                pendingCommands = new List<Command>();
            }
        }

        #endregion

        #region Thread Communication

        public void AddPendingCommand(Command command) {
            lock (lockPending) {
                pendingCommands.Add(command);
            }
        }

        public void Insert(List<Command> commands, long scheduledTurn, int playerID) {
            buffer.Insert(commands, scheduledTurn, playerID);
        }

        #endregion

    }
}
