using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Invaders.Control;
using Invaders.Managers;
using Invaders.Combat;
using Invaders.Locator;
using Invaders.Preservable;

using static Invaders.Managers.UIManager;

namespace Invaders.Attributes
{
    public class PlayerLives : MonoBehaviour, IPreservable, IProcessHit
    {
        [Header("References")]
        [SerializeField] private ParticleSystem _playerExplotion;

        [Header("Settings")]
        [SerializeField] private int _playerLives = 2;

        private AudioManager _audioManager;
        private GameManager _gameManager;
        private PlayerController _playerController;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _playerCollider;
        private WaitWhile _waitWhileExplotionIsPlaying;


        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playerCollider = GetComponent<Collider2D>();

            _waitWhileExplotionIsPlaying = new WaitWhile(() => _playerExplotion.isPlaying);

            RegisterPreservable();
        }


        private void Start()
        {
            _gameManager = ServiceLocator.Resolve<GameManager>();
            _audioManager = ServiceLocator.Resolve<AudioManager>();
            OnPlayerLivesUpdate?.Invoke(_playerLives);
        }


        public void ProcessHit(GameObject obj = default)
        {
            _playerLives -= 1;
            _playerExplotion?.Play();
            _audioManager?.PlaySFX("PlayerExplosion");

            OnPlayerLivesUpdate?.Invoke(_playerLives);

            if (_playerLives == 0)
            {
                StartCoroutine(LoseGame());
                return;
            }

            StartCoroutine(ResetPlayer());
        }


        private IEnumerator ResetPlayer()
        {
            DisablePlayer();

            yield return _waitWhileExplotionIsPlaying;

            _playerExplotion?.Stop();
            _playerController?.ResetPosition();

            EnablePlayer();
        }


        private IEnumerator LoseGame()
        {
            DisablePlayer();
            yield return _waitWhileExplotionIsPlaying;
            _gameManager?.LoseGame();
        }


        private void DisablePlayer()
        {
            _playerController.enabled = false;
            _spriteRenderer.enabled = false;
            _playerCollider.enabled = false;
        }


        private void EnablePlayer()
        {
            _spriteRenderer.enabled = true;
            _playerCollider.enabled = true;
            _playerController.enabled = true;
        }


        public void RegisterPreservable() => ValuePreserver.RegisterPerservable(this);

        public (string, object) PreserveValue() => (nameof(_playerLives), _playerLives);

        public void SetPreservedValue(Dictionary<string, object> preserved)
        {
            var returnedValue = preserved[nameof(_playerLives)];
            if (returnedValue == null) return;

            _playerLives = (int)returnedValue;

            OnPlayerLivesUpdate?.Invoke(_playerLives);
        }
    }
}