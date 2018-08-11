using UnityEngine;

public abstract class IGameEvent : ScriptableObject {
	public abstract void Signal();
}
