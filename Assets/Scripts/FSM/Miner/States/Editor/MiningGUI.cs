using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Mining))]
public class MiningGUI : Editor {
    public override void OnInspectorGUI() {
        Mining mining = (Mining)target;
        if (GUILayout.Button("Get References")) {
            mining.SetMiner();
        }
        DrawDefaultInspector();
    }
}
