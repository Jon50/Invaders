using UnityEngine;

using Invaders.Combat;
using Invaders.Locator;
using Invaders.Managers;

namespace Invaders.Attributes
{
    public class Score : MonoBehaviour, IProcessHit
    {
        private int _score = 0;
        public int GetScore => _score;
        private ScoreManager _scoreManager;


        private void Start() => _scoreManager = ServiceLocator.Resolve<ScoreManager>();

        public void Initialize(int score) => _score = score;

        public void ProcessHit(GameObject obj = null) => SetScore();

        private void SetScore() => _scoreManager.Score(_score);
    }
}