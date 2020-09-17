using UnityEngine;

using Invaders.Managers;
using Invaders.Preservable;
using Invaders.Locator;
using System.Collections.Generic;

namespace Invaders.StateMachine
{
    public class NextLevelState : GameState<GameManager>
    {
        public static NextLevelState Instance = StateSingleton<NextLevelState>.MakeInstatnce;
        private ValuePreserver _valuePreserver;
        public AudioSource audioSource;
        private float _clipLength;
        private List<StatePreparator> _statePreparators;

        public override void EnterState(GameManager owner)
        {
            _statePreparators = ServiceLocator.ResolveList<StatePreparator>();
            if (_statePreparators.Count != 0)
                foreach (var sp in _statePreparators)
                    sp.DisableComponents();

            _valuePreserver = ServiceLocator.Resolve<ValuePreserver>();
            _valuePreserver?.PreserveValue();

            Time.timeScale = 0;

            _clipLength = 5f;
            if (audioSource.IsNotNull())
                _clipLength = audioSource.clip.length;
        }


        public override void ExitState(GameManager owner)
        {
            foreach (var sp in _statePreparators)
                sp.EnableComponents();

            // _statePreparators.Clear();
        }


        public override void UpdateState(GameManager owner)
        {
            _clipLength -= Time.unscaledDeltaTime;

            if (_clipLength <= 0f)
            {
                owner.ReportStateWhenDone();
                owner.NextLevel();
            }
        }
    }
}