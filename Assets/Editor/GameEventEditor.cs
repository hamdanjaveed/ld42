using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GameEvent gameEvent = (GameEvent) target;

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("Signal")) gameEvent.Signal();
        EditorGUI.EndDisabledGroup();
    }
}
