using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Invaders.Managers
{
    public class UIManager : MonoBehaviour
    {
        [Header("Interface")]
        [SerializeField] private Slider _livesSlider;
        [SerializeField] private TextMeshProUGUI _livesText;
        [SerializeField] private TextMeshProUGUI _scoreValue;
        [SerializeField] private TextMeshProUGUI _highScoreValue;

        [Header("Menus")]
        [SerializeField] private GameObject _loseMenu;

        public static Action<int> OnPlayerLivesUpdate { get; private set; }
        public static Action<int> OnScoreUpdate { get; private set; }
        public static Action<int> OnHighScoreUpdate { get; private set; }
        public static Action OnLoseGame { get; private set; }


        private void OnEnable()
        {
            OnPlayerLivesUpdate += UpdateLivesDisplay;
            OnScoreUpdate += UpdateScoreDisplay;
            OnHighScoreUpdate += UpdateHighScoreDisplay;
            OnLoseGame += LoseGame;
        }


        private void OnDisable()
        {
            OnPlayerLivesUpdate -= UpdateLivesDisplay;
            OnScoreUpdate -= UpdateScoreDisplay;
            OnHighScoreUpdate -= UpdateHighScoreDisplay;
            OnLoseGame -= LoseGame;
        }


        private void UpdateLivesDisplay(int lives)
        {
            _livesText.text = lives.ToString();
            _livesSlider.value = lives;
        }


        private void UpdateScoreDisplay(int score) => _scoreValue.text = score.ToString();

        private void UpdateHighScoreDisplay(int score) => _highScoreValue.text = score.ToString();

        private void LoseGame() => _loseMenu.SetActive(true);
    }
}