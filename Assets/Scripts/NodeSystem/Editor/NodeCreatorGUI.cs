using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeCreator))]
public class NodeCreatorGUI : Editor {
    int currentIndex = 0;
    string currentIndexString = "0";
    int nodesAmount = 0;
    string nodesAmountString = "0";
    bool showNodes;
   
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

        EditorGUILayout.Separator();

        int totalNodes = nodeCreator.GetNodesAmount();
        EditorGUILayout.LabelField($"Total Nodes: {totalNodes}");
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Starting Node");
        currentIndexString = EditorGUILayout.TextField(currentIndexString);
        int index;
        if (int.TryParse(currentIndexString, out index)) {
            currentIndex = index;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Nodes Amount To Show");
        nodesAmountString = EditorGUILayout.TextField(nodesAmountString);
        int amount;
        if (int.TryParse(nodesAmountString, out amount)) {
            nodesAmount = amount;
        }
        GUILayout.EndHorizontal();

        showNodes = EditorGUILayout.Foldout(showNodes, "Show Nodes");

        if (showNodes) {
            int upToNode = (currentIndex + nodesAmount) < totalNodes? (currentIndex + nodesAmount) : totalNodes;
            for (int i = currentIndex; i < upToNode; i++) {
                Node currentNode = nodeCreator.GetNode(i);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Node {i}:");
                EditorGUILayout.PrefixLabel("Highlight:");
                currentNode.highlighted = EditorGUILayout.Toggle(currentNode.highlighted);
                GUILayout.EndHorizontal();
                EditorGUILayout.Vector3Field("Position", currentNode.position);
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Occupied:");
                EditorGUILayout.Toggle(currentNode.occupied);
                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
        }
    }
}
