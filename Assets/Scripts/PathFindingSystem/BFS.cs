using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : MonoBehaviour
{
    Queue<Node> nodeQueue;
    public Node[] nodes;


    private void PathFinder()
    {
        ClearPath();
        for (int i = 0; i < nodes.Length; i++)
        {    
            if(!nodes[i].visited)
            {
                NodeHandler(nodes[i]);
            } 
        }
    }

    private void NodeHandler(Node n)
    {

    }


    private void ClearPath()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].visited = false;
        }
    }
}
