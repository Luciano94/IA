using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeCreator))]
public class NodeCreatorGUI : Editor {
    int currentIndex = 0;
    static string currentIndexString = "0";
    int nodesAmount = 0;
    static string nodesAmountString = "0";
    static bool showNodes;
    Color lightRed = new Color(0.8f, 0.3f, 0.3f, 1f);
    Color lightGreen = new Color(0.3f, 0.8f, 0.3f, 1f);
   
    public override void OnInspectorGUI() {
        GUILayout.BeginHorizontal();
        NodeCreator nodeCreator = (NodeCreator)target;
        GUIStyle redText = new GUIStyle(GUI.skin.button);
        redText.normal.textColor = lightRed;
        if (GUILayout.Button("Clear Nodes", redText)) {
            nodeCreator.ClearNodes();
        }
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

        GUIStyle colorText = new GUIStyle(GUI.skin.label);
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
                GUILayout.BeginHorizontal();
                for (int j = 0; j < (int)Directions.Count; j++) {
                    colorText.normal.textColor = (currentNode.adjacent[j] >= 0)? lightGreen : lightRed;
                    EditorGUILayout.LabelField(((Directions)j).ToString(), colorText, GUILayout.Width(120));
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
        }
    }
}
