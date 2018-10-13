public interface IGameStateObserver
{
    void OnLeaveState(GameState state);
    void OnEnterState(GameState state);
}
