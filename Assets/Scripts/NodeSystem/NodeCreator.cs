using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCreator : MonoBehaviour {
    public GameObject nodePrefab;
    public Vector3 size;
    public float interval;
    public LayerMask affectedLayers;
    private GameObject[] nodes;

    public void BakeNodes()  {
        ClearNodes();
        nodes = new GameObject[(Mathf.FloorToInt(size.x/interval) + 1) * (Mathf.FloorToInt(size.z/interval) + 1)];
        Vector3 currentPos = transform.position - size*0.5f;
        currentPos.y = transform.position.y + size.y*0.5f;
        int iNode = 0;
        float maxZ = size.z*0.5f;
        float maxX = size.x*0.5f;
        RaycastHit raycastHit;
        while (currentPos.z < maxZ) {
            while (currentPos.x < maxX) {
                nodes[iNode] = Instantiate(nodePrefab, currentPos, Quaternion.identity, transform);
                if (Physics.Raycast(nodes[iNode].transform.position, Vector3.down, out raycastHit, size.y, affectedLayers)) {
                    Vector3 newPos = nodes[iNode].transform.position;
                    newPos.y -= raycastHit.distance;
                    nodes[iNode].transform.position = newPos;
                }
                currentPos.x += interval;
                ++iNode;
            }
            currentPos.z += interval;
            currentPos.x = transform.position.x - size.x*0.5f;
        }
    }

    
    public void ClearNodes()  {
        if (nodes != null) {
            int nodesCount = nodes.Length;
            for (int i = 0; i < nodesCount; ++i) {
                DestroyImmediate(nodes[i]);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
