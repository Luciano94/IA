using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Miner))]
public class MinerGUI : ReferencesGUI {
    public override void OnInspectorGUI() {
        Miner miner = (Miner)target;
        EditorGUILayout.BeginHorizontal();
        string currentState = miner.currentState?.GetType().ToString();
        string nextState = miner.nextState?.GetType().ToString();
        EditorGUILayout.LabelField($"Current State: {currentState}");
        EditorGUILayout.LabelField($"Next State: {nextState}");
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}
