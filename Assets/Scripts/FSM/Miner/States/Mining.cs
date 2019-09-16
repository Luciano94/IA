
using UnityEngine;

public class Mining : MonoBehaviour, IState {
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

            if (extracted != 0) {
                timer = 0f;
            } else {
                nextState = searchingMine;
            }

        }
    }

    public void UpdateStatePhysics() {

    }

    public void SetMiner() {
        miner = GetComponent<Miner>();
        searchingMine = GetComponent<SearchingMine>();
    }
}