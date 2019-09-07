using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFS : MonoBehaviour
{
    public NodeCreator nodeCreator;
    public Node[] nodes;
    public Node initNode;
    public Node objectiveNode;
    public List<Node> Path;

    private void PathFinder()
    {
        ClearPath();
        for (int i = 0; i < nodes.Length; i++)
        {    
            if(!nodes[i].visited)
            {
                NodeHandler(i, nodes[i].adjacent);
            } 
        }
    }

    private void NodeHandler(int iNode, int[] adjacent)
    {
        nodes[iNode].visited = true;
        for (int i = 0; i < adjacent.Length; i++)
        {
            if(!nodes[adjacent[i]].visited)
            {
                NodeHandler(adjacent[i], nodes[adjacent[i]].adjacent);
            }
        }
    }


    private void ClearPath()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].visited = false;
        }
    }
}
