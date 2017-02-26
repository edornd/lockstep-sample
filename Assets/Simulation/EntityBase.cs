namespace Game {
    /// <summary>
    /// Base class for every main game entity.
    /// The base constructor registers itself to the simulation behaviour.
    /// </summary>
    public abstract class EntityBase : IGameBehaviour {

        public EntityBase() {
            Simulation.Register(this);
        }

        public virtual void Init() { }

        public virtual void Update() { }

        public virtual void Quit() { }
    }
}
