using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeCreator))]
public class NodeCreatorGUI : Editor {
   
    public override void OnInspectorGUI() {
        GUILayout.BeginHorizontal();
        NodeCreator nodeCreator = (NodeCreator)target;
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = new Color(0.8f, 0f, 0f, 1f);
        if (GUILayout.Button("Clear Nodes", style)) {
            nodeCreator.ClearNodes();
        }
        style.normal.textColor = Color.white;
        if (GUILayout.Button("Bake Nodes")) {
            nodeCreator.BakeNodes();
        }
        GUILayout.EndHorizontal();
        DrawDefaultInspector();
    }
}
