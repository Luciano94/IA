using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Moving : References, IState {
    [HideInInspector] public Vector3 destination;
    private Vector3 direction;
    [HideInInspector] public IState nextState;
    [HideInInspector][SerializeField] private CharacterController characterController;
    private float speed = 0.02f;

    public void Init() {
    }

    public void UpdateState(ref IState nextState) {
        Vector3 diff = destination - transform.position;
        direction = diff.normalized;
        characterController.Move(direction * speed);
        if (diff.sqrMagnitude <= 1.5f) {
            nextState = this.nextState;
        }
    }

    public void UpdateStatePhysics() {
    }

    public override void SearchReferences() {
        if (characterController == null) {
            characterController = GetComponent<CharacterController>();
        }
    }
}