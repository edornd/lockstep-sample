using System;
using Game.Utils;
using Presentation;
using Presentation.Network;
using System.Collections.Generic;

namespace Game.Lockstep {

    /// <summary>
    /// Enum describing the current lockstep logic status
    /// </summary>
    public enum LockstepStatus {
        Playing,
        Delay
    }
    /// <summary>
    /// Class implementing the main loop for the lockstep logic.
    /// </summary>
    public class LockstepLogic : Singleton<LockstepLogic>, IGameBehaviour {

        #region Variables
        private int identity;

        private long currentTurn;
        private long commandTurn;
        private int currentFrame;
        private int framesPerTurn;
        private LockstepStatus status;
        private CommandBuffer buffer;
        private TurnData currentData;
        private List<Command> pendingCommands;
        private int recoverTime;

        private object lockTurn;
        private object lockPending;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the lockstep main logic.
        /// </summary>
        /// <param name="frames">number of frames per turn</param>
        public LockstepLogic(int frames) {
            framesPerTurn = frames;
            identity = PlayerManager.Identity.ID;
            buffer = new CommandBuffer(8, PlayerManager.PlayerCount);
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
            commandTurn = -3;
            currentFrame = 0;
            recoverTime = 100;
            status = LockstepStatus.Playing;
        }

        /// <summary>
        /// Tries to step forward with the game.
        /// </summary>
        public void Update() {
            if (IsLockstepturn()) {
                SendPendingCommands();
                if (currentTurn >= 0) { //Execute an actual turn only after the first two starting rounds
                    if (!IsLockstepReady()) { 
                        status = LockstepStatus.Delay;
                        UnityEngine.Debug.LogWarning("Not ready for next turn! Stuck at " + CurrentTurn);
                        return; // no need to go on, there's not enough data to proceed
                    }
                    ProcessTurn(); //finally ready to process stuff
                }
                else {
                    NextTurn(); //still in the starting turns, advance without processing
                }
            }
            // frame update functions
            NextFrame();
        }

        /// <summary>
        /// Deletes everything lockstep related.
        /// </summary>
        public void Quit() {
            buffer = null;
            currentData = null;
            pendingCommands = null;
        }

        #endregion

        #region Lockstep

        /// <summary>
        /// Gets the current turn, thread safe.
        /// </summary>
        private long CurrentTurn {
            get {
                lock (lockTurn) {
                    return currentTurn;
                }
            }
        }

        /// <summary>
        /// Checks whether the current frame is the first one of the turn.
        /// </summary>
        /// <returns>true if the frame is 0, false otherwise</returns>
        private bool IsLockstepturn() {
            return currentFrame == 0;
        }

        /// <summary>
        /// Checks whether we received turn commands from everyone or not.
        /// </summary>
        /// <returns></returns>
        private bool IsLockstepReady() {
            return buffer.IsReadyToAdvance();
        }

        /// <summary>
        /// Increments the turn counter.
        /// </summary>
        private void NextTurn() {
            lock (lockTurn) {
                currentTurn++;
            }
        }

        /// <summary>
        /// Increments the frame counter.
        /// Frames go from 0 to the specified number of frames per turn.
        /// </summary>
        private void NextFrame() {
            currentFrame = (currentFrame + 1) % framesPerTurn;
        }

        /// <summary>
        /// Forwards the pending commands to the scheduler, in order to be sent to the other players
        /// and executed 2 turns in the future.
        /// The commands are also inserted into the buffer for the current player.
        /// </summary>
        private void SendPendingCommands() {
            lock (lockPending) {
                long current = CurrentTurn;
                //if we already sent current turn's data
                if (commandTurn == current)
                    return;

                long scheduled = current + 2;
                Scheduler.Instance.SendPendingCommands(pendingCommands, scheduled);
                Insert(pendingCommands, scheduled, identity);
                pendingCommands = new List<Command>();
                commandTurn = current;
            }
        }

        /// <summary>
        /// Process commands for the current turn, recovering from a delay if necessary.
        /// </summary>
        private void ProcessTurn() {
            if (status == LockstepStatus.Delay) {
                ClearPendingCommands();
                status = LockstepStatus.Playing;
                Simulation.Delay(recoverTime);
            }
            currentData = buffer.Advance();
            currentData.ProcessCommands();
            NextTurn();
        }

        #endregion

        #region Thread Communication

        /// <summary>
        /// Adds a new command to the pending list.
        /// </summary>
        /// <param name="command">generic command to be added</param>
        public void AddPendingCommand(Command command) {
            lock (lockPending) {
                pendingCommands.Add(command);
            }
        }

        /// <summary>
        /// Thread safe clear for the pending commands list.
        /// </summary>
        public void ClearPendingCommands() {
            lock (lockPending) {
                pendingCommands.Clear();
            }
        }

        /// <summary>
        /// Inserts the list of commands inside the buffer, at the position specified by the turn and the playerID.
        /// </summary>
        /// <param name="commands">list of commands for the given turn and player</param>
        /// <param name="scheduledTurn">scheduled turn for the commands</param>
        /// <param name="playerID">player who issued the commands</param>
        public void Insert(List<Command> commands, long scheduledTurn, int playerID) {
            buffer.Insert(commands, scheduledTurn, playerID);
        }

        /// <summary>
        /// Updates the players count inside the buffer, in order to set ready
        /// even without the disconnected player.
        /// </summary>
        /// <param name="activePlayers">new count of the active players</param>
        /// <param name="playerID">id of the player who left</param>
        public void UpdatePlayersCount(int activePlayers, int playerID) {
            buffer.SetPlayersCount(activePlayers, playerID);
        }

        #endregion

    }
}
