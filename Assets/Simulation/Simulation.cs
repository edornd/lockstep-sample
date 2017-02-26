using System.Diagnostics;
using System.Collections.Generic;
using Game.Lockstep;

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

        private List<EntityBase> entities;
        private LockstepLogic lockstep;

        #endregion

        #region singleton

        private static Simulation instance = new Simulation();

        public static Simulation Instance {
            get { return instance; }
        }


        public static double DeltaTime {
            get { return instance.targetTime; }
        }

        public static void Register(EntityBase entity) {
            instance.entities.Add(entity);
        }

        public static void Destroy(EntityBase entity) {
            instance.entities.Remove(entity);
        }

        #endregion

        #region constructors

        public Simulation() : this(0.02, 0.25) { }

        public Simulation(double deltaTime, double maxTime) {
            running = true;
            accumulator = 0.0;
            totalTime = 0.0;
            targetTime = deltaTime;
            maximumTime = maxTime;
            stopwatch = new Stopwatch();
            entities = new List<EntityBase>();
        }

        #endregion

        #region instance methods

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
            foreach (EntityBase entity in entities) {
                entity.Init();
            }
            stopwatch.Start();
        }

        public void Update() {
            foreach (EntityBase entity in entities) {
                entity.Update();
            }
        }

        public void Quit() {
            stopwatch.Stop();
            foreach (EntityBase entity in entities) {
                entity.Quit();
            }
        }

        private double GetElapsedTime() {
            double time = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();
             stopwatch.Start();
            if (elapsedTime > maximumTime)
                elapsedTime = maximumTime;
            return time;
        }

        #endregion
    }
}
