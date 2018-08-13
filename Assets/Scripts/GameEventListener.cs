using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour {
    [Serializable]
    public struct EventResponsePair {
        public GameEvent gameEvent;
        public UnityEvent response;
    }

    public List<EventResponsePair> pairs;

    void OnEnable() {
        foreach (EventResponsePair pair in pairs) {
            pair.gameEvent.Subscribe(this);
        }
    }

    void OnDisable() {
        foreach (EventResponsePair pair in pairs) {
            pair.gameEvent.Unsubscribe(this);
        }
    }

    public void OnEventRaised(GameEvent gameEvent) {
        foreach (EventResponsePair pair in pairs) {
            if (gameEvent == pair.gameEvent) pair.response.Invoke();
        }
    }
}
