namespace Invaders.StateMachine
{
    public abstract class GameState<T>
    {
        public abstract void EnterState(T owner);
        public abstract void UpdateState(T owner);
        public abstract void ExitState(T owner);
    }
}