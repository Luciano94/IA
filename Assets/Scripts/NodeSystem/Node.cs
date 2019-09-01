using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
    public float occupiedDistanceRay;
    Transform parent;
    Node[] adjacent;
    bool occupied;
    const float OCCUPY_CHECK_RATE = 1f;
    float timer = 0f;

    private void Update() {
        timer += Time.deltaTime;

        if (timer >= OCCUPY_CHECK_RATE) {
            RaycastHit rh;
            occupied = Physics.SphereCast(transform.position, 1f, Vector3.up, out rh, occupiedDistanceRay);
            timer = 0f;
        }
    }

    private void OnDrawGizmos() {
        RaycastHit rh;
        occupied = Physics.SphereCast(transform.position, 1f, Vector3.up, out rh, occupiedDistanceRay);
        if (occupied) {
            Gizmos.color = Color.red;
        } else {
            Gizmos.color = Color.cyan;
        }
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
