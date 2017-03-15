using System.Diagnostics;
using System.Collections.Generic;
using Game.Lockstep;
using System;
using Game.Utils;

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

        private List<IGameBehaviour> entities;
        private LockstepLogic lockstep;

        #endregion

        #region singleton

        private static Simulation instance;

        public static void Create() {
            instance = new Simulation();
        }

        public static void Create(double delta, double maxDelta) {
            instance = new Simulation(delta, maxDelta);
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

        public static IGameBehaviour GetObjectOfType(Type type) {
            foreach(IGameBehaviour entity in instance.entities) {
                if (entity.GetType().Equals(type)) {
                    return entity;
                }
            }
            return null;
        }

        #endregion

        #region constructors

        public Simulation() : this(0.1, 0.25) { }

        public Simulation(double deltaTime, double maxTime) {
            running = true;
            accumulator = 0.0;
            totalTime = 0.0;
            targetTime = deltaTime;
            maximumTime = maxTime;
            stopwatch = new Stopwatch();
            entities = new List<IGameBehaviour>();
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
            //create objects and add them to the simulation list
            lockstep = new LockstepLogic();
            Register(lockstep);

            //initialize singleton instances
            LockstepLogic.InitSingleton();

            foreach (IGameBehaviour entity in entities) {
                entity.Init();
            }
            stopwatch.Start();
        }

        public void Update() {
            foreach (IGameBehaviour entity in entities) {
                entity.Update();
            }
        }

        public void Quit() {
            stopwatch.Stop();
            foreach (IGameBehaviour entity in entities) {
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
