using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(References), true)]
public class ReferencesGUI : Editor {
    public override void OnInspectorGUI() {
        References references = (References)target;
        if (GUILayout.Button("Get References")) {
            references.SearchReferences();
        }
        DrawDefaultInspector();
    }
}
