namespace Invaders.StateMachine
{
    public class StateMachine<T>
    {
        public GameState<T> CurrentState { get; private set; }
        private T _owner;

        public StateMachine(T owner)
        {
            _owner = owner;
            CurrentState = null;
        }

        public void ChangeState(GameState<T> newState)
        {
            if (newState.IsNull()) return;

            CurrentState?.ExitState(_owner);
            CurrentState = newState;
            CurrentState.EnterState(_owner);
        }


        public void ClearCurrentState()
        {
            CurrentState.ExitState(_owner);
        }

        public void StateMachineUpdate() => CurrentState?.UpdateState(_owner);
    }
}