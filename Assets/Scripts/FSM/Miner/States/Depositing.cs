using UnityEngine;

public class Depositing : References, IState {
    [SerializeField] private Miner miner;

    public void Init() {
        miner.deposit.DepositResources(miner.SaveResources());
    }

    public void UpdateState(ref IState nextState) {
        nextState = miner.seachingMines;
    }

    public void UpdateStatePhysics() {
    }

    public override void SearchReferences() {
        if (miner == null) {
            miner = GetComponent<Miner>();
        }
    }
}
