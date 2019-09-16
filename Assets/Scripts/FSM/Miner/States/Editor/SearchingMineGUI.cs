using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SearchingMine))]
public class SearchingMineGUI : Editor {
    public override void OnInspectorGUI() {
        SearchingMine seachingMines = (SearchingMine)target;
        if (GUILayout.Button("Get References")) {
            seachingMines.SearchReferences();
        }
        DrawDefaultInspector();
    }
}
