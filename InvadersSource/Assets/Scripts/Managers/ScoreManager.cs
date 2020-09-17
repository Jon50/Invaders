using System.IO;
using System.Collections.Generic;
using Invaders.Locator;
using Invaders.Preservable;

using static Invaders.Managers.UIManager;
using static Invaders.Save.SavingSystem;
using static Invaders.Static.ConstantValues;

namespace Invaders.Managers
{
    public class ScoreManager : ServiceRegister<ScoreManager>, IPreservable
    {
        private int _score = 0;
        private int _highScore;

        public int GetScore => _score;
        public int GetHighScore => File.Exists(GetSavePath(HIGH_SCORE_SAVE)) ? LoadValue<int>(HIGH_SCORE_SAVE) : default;


        private void Awake()
        {
            RegisterService(this);
            RegisterPreservable();
        }


        private void Start()
        {
            _highScore = GetHighScore;

            OnScoreUpdate?.Invoke(_score);
            OnHighScoreUpdate?.Invoke(_highScore);
        }


        private void OnDisable() => UnregisterService();

        public void Score(int score)
        {
            _score += score;
            OnScoreUpdate?.Invoke(_score);

            if (_score > _highScore)
            {
                _highScore = _score;
                OnHighScoreUpdate?.Invoke(_highScore);
                SaveValue(HIGH_SCORE_SAVE, _highScore);
            }
        }


        public void RegisterPreservable() => ValuePreserver.RegisterPerservable(this);

        public (string, object) PreserveValue() => (nameof(_score), _score);

        public void SetPreservedValue(Dictionary<string, object> preserved)
        {
            var returnedValue = preserved[nameof(_score)];
            if (returnedValue.IsNull()) return;

            _score = (int)returnedValue;

            OnScoreUpdate?.Invoke(_score);
        }
    }
}