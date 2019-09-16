using UnityEngine;

[RequireComponent(typeof(SearchingMine))]
[RequireComponent(typeof(Mining))]
[RequireComponent(typeof(Depositing))]
[RequireComponent(typeof(Moving))]
[RequireComponent(typeof(CharacterController))]
public class Miner : References {
    public SearchingMine seachingMines;
    public Mining mining;
    public Depositing depositing;
    public Moving moving;
    public CharacterController characterController;
    public IState currentState;
    public IState nextState;
    public Deposit deposit;
    private int resources;

    private void Awake() {
        currentState.Init();
    }

    private void Update() {
        if (nextState != currentState) {
            currentState = nextState;
            currentState.Init();
        }
        currentState.UpdateState(ref nextState);
    }

    private void OnValidate() {
        currentState = seachingMines;
        nextState = seachingMines; 
    }

    public int SaveResources() {
        int deposited = resources;
        resources = 0;
        return deposited;
    }

    public void AddResources(int amount) {
        if (amount > 0) {
            resources += amount;
        }
    }

    public override void SearchReferences() {
        if (seachingMines == null) {
            seachingMines = GetComponent<SearchingMine>();
        }
        if (mining == null) {
            mining = GetComponent<Mining>();
        }
        if (depositing == null) {
            depositing = GetComponent<Depositing>();
        }
        if (moving == null) {
            moving = GetComponent<Moving>();
        }
        if (characterController == null) {
            characterController = GetComponent<CharacterController>();
        }
    }
}
