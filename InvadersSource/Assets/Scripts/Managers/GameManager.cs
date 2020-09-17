using UnityEngine;
using Invaders.StateMachine;
using Invaders.Locator;

using static Invaders.Managers.UIManager;

namespace Invaders.Managers
{
    public class GameManager : ServiceRegister<GameManager>
    {
        [SerializeField] private SceneLoader _sceneLoader;
        private StateMachine<GameManager> _stateMachine;
        private AudioManager _audioManager;
        private bool _blockStateMachine;

        private void Awake()
        {
            _stateMachine = new StateMachine<GameManager>(owner: this);
            _audioManager = GetComponent<AudioManager>();
            RegisterService(this);
        }


        private void Start()
        {
            SetNewState(PrepareGameState.Instance);
            _blockStateMachine = true;
        }

        private void Update() => _stateMachine.StateMachineUpdate();


        public void ReportStateWhenDone()
        {
            _blockStateMachine = false;
        }


        public void ClearCurrentState()
        {
            _stateMachine.ClearCurrentState();
        }


        internal void WinGame()
        {
            NextLevelState.Instance.audioSource = _audioManager.PlayMusic("WinSound");
            SetNewState(NextLevelState.Instance);
            _blockStateMachine = true;
        }


        internal void LoseGame()
        {
            SetNewState(GamePauseState.Instance);

            _audioManager.PlayMusic("LoseSound");
            _blockStateMachine = true;

            OnLoseGame?.Invoke();
        }


        internal void SetNewState(GameState<GameManager> newState)
        {
            if (!_blockStateMachine)
                _stateMachine.ChangeState(newState);
        }

        public void NextLevel() => _sceneLoader.RestartGame();
    }
}
