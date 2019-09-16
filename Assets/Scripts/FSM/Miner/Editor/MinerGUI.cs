using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Miner))]
public class MinerGUI : Editor {
    public override void OnInspectorGUI() {
        Miner miner = (Miner)target;
        if (GUILayout.Button("Get References")) {
            miner.seachingMines = miner.GetComponent<SearchingMine>();
            miner.mining = miner.GetComponent<Mining>();
            miner.depositing = miner.GetComponent<Depositing>();
            miner.moving = miner.GetComponent<Moving>();
            miner.characterController = miner.GetComponent<CharacterController>();
        }

        EditorGUILayout.BeginHorizontal();
        string currentState = miner.currentState.GetType().ToString();
        string nextState = miner.nextState.GetType().ToString();
        EditorGUILayout.LabelField($"Current State: {currentState}");
        EditorGUILayout.LabelField($"Next State: {nextState}");
        EditorGUILayout.EndHorizontal();
        DrawDefaultInspector();
    }
}
