using UnityEngine;

public abstract class IGameEventListener : MonoBehaviour {
    public abstract void OnEventRaised(IGameEvent gameEvent);
}
