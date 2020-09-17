using System.Collections.Generic;
using Invaders.Locator;
using Invaders.Managers;
using UnityEngine;

namespace Invaders.StateMachine
{
    public class GamePauseState : GameState<GameManager>
    {
        public bool IsGamePaused { get; private set; }
        public static GamePauseState Instance => StateSingleton<GamePauseState>.MakeInstatnce;
        private List<StatePreparator> _statePreparators;

        public override void EnterState(GameManager owner)
        {
            _statePreparators = ServiceLocator.ResolveList<StatePreparator>();

            if (_statePreparators.Count != 0)
                foreach (var sp in _statePreparators)
                    sp.DisableComponents();

            Time.timeScale = 0;
            IsGamePaused = true;
        }

        public override void ExitState(GameManager owner)
        {
            foreach (var sp in _statePreparators)
                sp.EnableComponents();

            // _statePreparators.Clear();

            IsGamePaused = false;
        }

        public override void UpdateState(GameManager owner)
        {
        }
    }
}