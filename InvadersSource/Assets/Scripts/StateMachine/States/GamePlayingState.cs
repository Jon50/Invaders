using Invaders.Managers;
using UnityEngine;

namespace Invaders.StateMachine
{
    public class GamePlayingState : GameState<GameManager>
    {
        public static GamePlayingState Instance => StateSingleton<GamePlayingState>.MakeInstatnce;

        private AudioManager _audioManager;


        public override void EnterState(GameManager owner)
        {
            Time.timeScale = 1;
            _audioManager = owner.transform.GetComponent<AudioManager>();
            _audioManager?.PlayMusic("GameMusic");
        }

        public override void ExitState(GameManager owner)
        {
        }

        public override void UpdateState(GameManager owner)
        {
        }
    }
}