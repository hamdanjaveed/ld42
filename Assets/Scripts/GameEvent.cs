
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEvent", menuName = "Events/GameEvent")]
public class GameEvent : IGameEvent {
    public List<GameEventListener> listeners = new List<GameEventListener>();

    public override void Signal() {
        // Loop backwards incase a listener removes themselves
        for (int i = listeners.Count - 1; i >= 0; i--) {
            listeners[i].OnEventRaised(this);
        }
    }

    public void Subscribe(GameEventListener listener) {
        if (!listeners.Contains(listener)) listeners.Add(listener);
    }

    public void Unsubscribe(GameEventListener listener) {
        listeners.Remove(listener);
    }
}
