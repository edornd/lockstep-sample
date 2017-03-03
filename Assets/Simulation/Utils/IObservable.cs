using UnityEngine;
using System.Collections;

public interface IObservable {
    void Notify(object args);
}
