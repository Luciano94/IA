﻿using UnityEngine;

public class SearchingMine : MonoBehaviour, IState {
    private Miner miner;
    private Mining mining;
    public LayerMask minesLayer;

    private void Awake() {
        SearchReferences();
    }

    public void Init() {
    }

    public void UpdateState(ref IState nextState) {
        Collider[] mines = Physics.OverlapSphere(transform.position, 10f, minesLayer);
        if (mines != null) {
            mining.currentMine = mines[0].GetComponent<Mine>();
            miner.moving.destination = mines[0].transform.position;
            miner.moving.nextState = miner.mining;
            nextState = miner.moving;
        }
    }

    public void UpdateStatePhysics() {

    }

    public void SearchReferences() {
        if (miner == null) {
            miner = GetComponent<Miner>();
        }
        if (mining == null) {
            mining = GetComponent<Mining>();
        }
    }
}
