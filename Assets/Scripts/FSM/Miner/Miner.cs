using UnityEngine;

[RequireComponent(typeof(SearchingMine))]
[RequireComponent(typeof(Mining))]
[RequireComponent(typeof(Depositing))]
[RequireComponent(typeof(Moving))]
[RequireComponent(typeof(CharacterController))]
public class Miner : MonoBehaviour {
    public SearchingMine seachingMines;
    public Mining mining;
    public Depositing depositing;
    public Moving moving;
    public CharacterController characterController;
    public IState currentState;
    public IState nextState;

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
}
