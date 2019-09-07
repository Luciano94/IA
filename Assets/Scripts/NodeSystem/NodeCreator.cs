using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions
{
    Top = 0,
    Left,
    Right,
    Bottom,
    Count,
}
 
[System.Serializable]
public class Node {
    public Vector3 position;
    public int[] adjacent;
    public Node parent;
    public bool visited;
    public bool occupied;
    public int validAdjacents = 0;

#if UNITY_EDITOR
    public bool highlighted = false;
#endif

    public Node(Vector3 pos) {
        position = pos;
        occupied = false;
        adjacent = new int[(int)Directions.Count];
    }
}

public class NodeCreator : MonoBehaviour {
    public Vector3 size;
    public float interval;
    public LayerMask terrainLayers;
    public LayerMask obstaclesLayers;
    [HideInInspector][SerializeField] private Node[] nodes;
    private int width;
    private int height;

    public void BakeNodes() { 
        ClearNodes();
        width = Mathf.FloorToInt(size.x/interval) + 1;
        height = Mathf.FloorToInt(size.z/interval) + 1;
        nodes = new Node[width * height];
        Vector3 currentPos = transform.position - size*0.5f;
        currentPos.y = transform.position.y + size.y;
        int iNode = 0;
        float maxZ = transform.position.z + size.z*0.5f;
        float maxX = transform.position.x + size.x*0.5f;
        int column = 0;
        int row = 0;
        while (currentPos.z < maxZ) {
            while (currentPos.x < maxX) {
                Vector3 newPos = currentPos;
                RaycastHit raycastHit;
                if (Physics.Raycast(newPos, Vector3.down, out raycastHit, size.y, terrainLayers)) {
                    newPos.y -= raycastHit.distance;
                } 

                nodes[iNode] = new Node(newPos);
                nodes[iNode].occupied = Physics.Raycast(newPos, Vector3.up, out raycastHit, 1f, obstaclesLayers);

                if (column > 0) {
                    if (!nodes[iNode].occupied && !nodes[iNode - 1].occupied) {
                        nodes[iNode].adjacent[(int)Directions.Left] = iNode - 1;
                        nodes[iNode].validAdjacents++;
                        nodes[iNode - 1].adjacent[(int)Directions.Right] = iNode;
                        nodes[iNode - 1].validAdjacents++;
                    }
                }

                if (row > 0) {
                    if (!nodes[iNode].occupied && !nodes[iNode - width].occupied) {
                        nodes[iNode].adjacent[(int)Directions.Bottom] = iNode - width;
                        nodes[iNode].validAdjacents++;
                        nodes[iNode - width].adjacent[(int)Directions.Top] = iNode;
                        nodes[iNode - width].validAdjacents++;
                    }
                }

                currentPos.x += interval;
                column++;
                ++iNode;
            }
            currentPos.z += interval;
            currentPos.x = transform.position.x - size.x*0.5f;
            row++;
            column = 0;
        }
    }
    
    public void ClearNodes()  {
        nodes = null;
        CancelInvoke();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, size);
        CheckObstacles();
        int length = nodes?.Length ?? 0;
        for (int i = 0; i < length; ++i) {
            Node node = nodes[i];
            if (node.highlighted) {
                Gizmos.color = Color.magenta;
            }
            else if (node.occupied) {
                Gizmos.color = Color.red;
            } else if (node.validAdjacents == 4) {
                Gizmos.color = Color.white;
            } else if (node.validAdjacents == 3) {
                Gizmos.color = Color.cyan;
            } else if (node.validAdjacents == 2) {
                Gizmos.color = Color.green;
            } else if (node.validAdjacents == 1) {
                Gizmos.color = Color.yellow;
            } else {
                Gizmos.color = Color.black;
            }
            Gizmos.DrawWireSphere(nodes[i].position, 1f);
        }
    }

    private void CheckObstacles() {
        int length = nodes?.Length ?? 0;
        for (int i = 0; i < length; ++i) {
            Node node = nodes[i];
            RaycastHit raycastHit;
            node.occupied = Physics.Raycast(node.position, Vector3.up, out raycastHit, 1f, obstaclesLayers);
        }
    }
    
    public Node GetNode(int index) {
        return nodes[index];
    }

    public int GetNodesAmount() {
        return nodes.Length;
    }
}
