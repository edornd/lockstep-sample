namespace Game {
    /// <summary>
    /// Defines start and stop functions for the main game loop.
    /// </summary>
    public interface IGameLoop {
        void Run();
        void Stop();
    }
}
