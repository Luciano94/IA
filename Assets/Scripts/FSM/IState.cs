public interface IState {
    void Init();
    void UpdateState(ref IState nextState);
    void UpdateStatePhysics();
}
