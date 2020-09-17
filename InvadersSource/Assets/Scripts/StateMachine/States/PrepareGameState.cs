using System.Collections.Generic;
using UnityEngine;

using Invaders.Control;
using Invaders.Locator;
using Invaders.Managers;

namespace Invaders.StateMachine
{
    public class PrepareGameState : GameState<GameManager>
    {
        public static PrepareGameState Instance = StateSingleton<PrepareGameState>.MakeInstatnce;

        private AliensController _aliensController;
        private AudioManager _audioManager;
        private List<StatePreparator> _statePreparators = new List<StatePreparator>();
        private bool _startDescending;
        private float _timer = 0.3f;
        private float offsetUp = 6;
        private Transform alienTransform;
        private Vector3 initialPosition;


        public override void EnterState(GameManager owner)
        {
            _aliensController = ServiceLocator.Resolve<AliensController>();
            _statePreparators = ServiceLocator.ResolveList<StatePreparator>();
            _audioManager = ServiceLocator.Resolve<AudioManager>();

            if (_aliensController.IsNull()) return;
            if (_statePreparators.Count == 0) return;

            foreach (var sp in _statePreparators)
                sp.DisableComponents();

            Prepare(owner);
        }


        private void Prepare(GameManager owner)
        {
            alienTransform = _aliensController.transform;
            initialPosition = alienTransform.position;
            alienTransform.position += Vector3.up * offsetUp;

            _startDescending = true;
        }


        public override void ExitState(GameManager owner)
        {
            foreach (var sp in _statePreparators)
                sp.EnableComponents();

            // _statePreparators.Clear();
        }


        public override void UpdateState(GameManager owner)
        {
            if (!_startDescending) return;

            if (alienTransform.position.y > initialPosition.y)
            {
                _timer -= Time.unscaledDeltaTime;
                if (_timer <= 0f)
                {
                    alienTransform.position += Vector3.down * .75f;
                    _audioManager.PlaySFX("AlienDescending");
                    _timer = 0.3f;
                }

                return;
            }

            _startDescending = false;
            owner.ReportStateWhenDone();
            owner.SetNewState(GamePlayingState.Instance);
        }
    }
}