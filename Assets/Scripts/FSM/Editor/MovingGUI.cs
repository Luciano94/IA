using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Moving))]
public class MovingGUI : Editor {
    public override void OnInspectorGUI() {
        Moving moving = (Moving)target;
        if (GUILayout.Button("Get References")) {
            moving.SearchReferences();
        }
        DrawDefaultInspector();
    }
}
