namespace Game.Utils {
    public class Singleton<T> where T : IGameBehaviour {
        protected static T instance;

        public static T Instance {
            get {
                if (instance == null) {
                    instance = (T)Simulation.GetObjectOfType(typeof(T));
                    if (instance == null) {
                        UnityEngine.Debug.Log("No instance of " + typeof(T) + " running in the simulation.");
                    }
                }
                return instance;
            }
        }

        public static void InitSingleton() {
            instance = (T)Simulation.GetObjectOfType(typeof(T));
        }
    }
}
