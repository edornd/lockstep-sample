using System.Collections.Generic;

namespace Game.Utils {
    public abstract class Observable : IObservable {

        private List<IObserver> observers = new List<IObserver>();

        public virtual void Subscribe(IObserver ob) {
            this.observers.Add(ob);
        }

        public virtual void Unsubscribe(IObserver ob) {
            this.observers.Remove(ob);
        }

        public virtual void Notify(object args) {
            foreach (IObserver ob in observers) {
                ob.Signal(this, args);
            }
        }
    }
}
