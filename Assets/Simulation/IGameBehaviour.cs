namespace Game {
    /// <summary>
    /// Defines functions to be called during the game loop, in a similar
    /// fashion to the Monobehaviour class.
    /// </summary>
    public interface IGameBehaviour {

        /// <summary>
        /// Used to initialize the object.
        /// </summary>
        void Init();

        /// <summary>
        /// Called once per step.
        /// </summary>
        void Update();

        /// <summary>
        /// Called on exit.
        /// </summary>
        void Quit();
    }
}
