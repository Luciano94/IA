
using UnityEngine;

public class Mining : References, IState {
    [HideInInspector] public Mine currentMine;
    public float mineRate;
    public int mineAmount;
    private Miner miner;
    private float timer;
    private SearchingMine searchingMine;

    private void Awake() {
        if (miner == null) {
            miner = GetComponent<Miner>();
        }
    }

    public void Init() {
        timer = 0f;
    }

    public void UpdateState(ref IState nextState) {
        timer += Time.deltaTime;

        if (timer > mineRate) {
            int extracted = currentMine.ExtractResources(mineAmount);
            miner.AddResources(extracted);

            if (extracted != 0) {
                timer = 0f;
            } else {
                miner.moving.nextState = miner.depositing;
                miner.moving.destination = miner.deposit.transform.position;
                nextState = miner.moving;
            }

        }
    }

    public void UpdateStatePhysics() {

    }

    public override void SearchReferences() {
        miner = GetComponent<Miner>();
        searchingMine = GetComponent<SearchingMine>();
    }
}