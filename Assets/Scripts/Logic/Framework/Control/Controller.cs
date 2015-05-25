using UnityEngine;
using System.Collections;

public abstract class Controller : Notifier {
    public Controller()
    {
        this.RegisterEventHandler();
    }
    protected abstract void RegisterEventHandler();
}

