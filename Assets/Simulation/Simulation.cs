using Game.Lockstep;
using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Presentation.Network;

namespace Game {
    /// <summary>
    /// Simulation class running on a separate thread.
    /// </summary>
    public sealed class Simulation : IGameLoop, IGameBehaviour {

        #region variables

        private volatile bool running;
        private Stopwatch stopwatch;
        private double elapsedTime;
        private double maximumTime;
        private double targetTime;
        private double accumulator;
        private double totalTime;

        private int myIdentity;
        private List<IGameBehaviour> entities;
        private LockstepLogic lockstep;

        #endregion

        #region singleton

        private static Simulation instance;

        public static void Create() {
            instance = new Simulation();
        }

        public static void Create(double delta, double maxDelta, int numFrames) {
            instance = new Simulation(delta, maxDelta, numFrames);
        }

        public static Simulation Instance {
            get { return instance; }
        }

        public static double DeltaTime {
            get { return instance.targetTime; }
        }

        public static void Register(IGameBehaviour entity) {
            instance.entities.Add(entity);
        }

        public static void Destroy(IGameBehaviour entity) {
            instance.entities.Remove(entity);
        }

        public static void Delay(int millisecs) {
            instance.Sleep(millisecs);
        }

        public static IGameBehaviour GetObjectOfType(Type type) {
            foreach(IGameBehaviour entity in instance.entities) {
                if (entity.GetType().Equals(type)) {
                    return entity;
                }
            }
            return null;
        }

        public static int Identity {
            get { return instance.myIdentity; }
        }

        #endregion

        #region constructors

        public Simulation() : this(0.1, 0.25, 4) { }

        public Simulation(double deltaTime, double maxTime, int numFrames) {
            running = true;
            accumulator = 0.0;
            totalTime = 0.0;
            targetTime = deltaTime;
            maximumTime = maxTime;
            stopwatch = new Stopwatch();
            entities = new List<IGameBehaviour>();
            lockstep = new LockstepLogic(numFrames);
        }

        #endregion

        #region instance methods

        /// <summary>
        /// Main thread loop, the cycle continues until the game is stopped.
        /// </summary>
        public void Run() {
            Init();
            while(running) {
                elapsedTime = GetElapsedTime();
                accumulator += elapsedTime;

                while (accumulator >= targetTime) {
                    Update();
                    totalTime += targetTime;
                    accumulator -= targetTime;
                }
                Sleep(1);
            }
            Quit();
        }

        public bool IsRunning() {
            return running;
        }

        public void Stop() {
            running = false;
        }

        public void Init() {
            //get the client's ID, so that we can use it inside the sim thread
            myIdentity = PlayerManager.Identity.ID;

            //initialize singleton instances
            Register(lockstep);
            LockstepLogic.InitSingleton();

            //run init on every entity
            foreach (IGameBehaviour entity in entities) {
                entity.Init();
            }
            stopwatch.Start();
        }

        /// <summary>
        /// Calss update for every registered entity.
        /// </summary>
        public void Update() {
            foreach (IGameBehaviour entity in entities) {
                entity.Update();
            }
        }

        /// <summary>
        /// Stops the timing mechanism and calls the quit function on every entity.
        /// </summary>
        public void Quit() {
            stopwatch.Stop();
            foreach (IGameBehaviour entity in entities) {
                entity.Quit();
            }
        }

        /// <summary>
        /// Gets the amount of time elapsed from the previous call.
        /// </summary>
        /// <returns>time elapsed in seconds</returns>
        private double GetElapsedTime() {
            double time = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();
             stopwatch.Start();
            if (elapsedTime > maximumTime)
                elapsedTime = maximumTime;
            return time;
        }

        /// <summary>
        /// Wrapper around the thread sleep function.
        /// </summary>
        /// <param name="millisecs">amount of milliseconds to sleep</param>
        private void Sleep(int millisecs) {
            Thread.Sleep(millisecs);
        }
        #endregion
    }
}
