namespace Game.Utils {
    public interface IObserver {
        void Signal(IObservable ob, object args);
    }
}
