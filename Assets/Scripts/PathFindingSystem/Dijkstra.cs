using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public Node[] nodes;
    public int distance;
    
    private void PathFinder()
    {
        ClearPath();
        distance = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            int nextNodeToVisit = getAdjacentWithLessDistance(nodes[i].adjacent);
            nodes[nextNodeToVisit].visited = true;    
            for (int j = 0; j < nodes[nextNodeToVisit].adjacent.Length; j++)
            {
                if(!nodes[nodes[nextNodeToVisit].adjacent[j]].visited)
                {
                    int distanceBetweenNodes =Mathf.Abs(nodes[nodes[nextNodeToVisit].adjacent[j]].distance 
                                                            - nodes[nextNodeToVisit].distance);
                    if(nodes[nodes[nextNodeToVisit].adjacent[j]].distance >
                        (nodes[nextNodeToVisit].distance + distanceBetweenNodes))
                    {
                        nodes[nodes[nextNodeToVisit].adjacent[j]].distance = nodes[nextNodeToVisit].distance + distanceBetweenNodes;
                        nodes[nodes[nextNodeToVisit].adjacent[j]].weight = nodes[nextNodeToVisit].distance;
                    }
                }
            }
        }
    }

    private int getAdjacentWithLessDistance(int[] adjacent)
    {
        int resultIndex = 0;
        int distance = int.MaxValue;
        for (int i = 0; i < adjacent.Length; i++)
        {
            if(nodes[adjacent[i]].distance < distance)
            {
                resultIndex = i;
                distance =  nodes[adjacent[i]].distance;
            }
        }
        return resultIndex;
    } 

    private void ClearPath()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].weight = 0;
            nodes[i].distance = int.MaxValue;
        }
    }
    
    /*private void NodeHandler(int iNode, int[] adjacent)
    {
        nodes[iNode].visited = true;
        for (int i = 0; i < adjacent.Length; i++)
        {
            if(!nodes[adjacent[i]].visited)
            {
                NodeHandler(adjacent[i], nodes[adjacent[i]].adjacent);
            }
        }
    }*/


}
