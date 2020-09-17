using System;
using System.Collections.Generic;
using UnityEngine;

using Invaders.Core;
using Invaders.Managers;
using Invaders.Combat;
using Invaders.Locator;

using static Invaders.Enums.Tags;
using Invaders.Attributes;
using System.Collections;

namespace Invaders.Control
{
    public class AliensController : ServiceRegister<AliensController>, IProcessHit
    {
        [SerializeField] private Sprite _explosionSprite;

        [Header("Movement")]
        [Range(0.004f, 0.01f)] [SerializeField] private float _moveIncreasePerAlienKilled = 0.004f;
        [Range(0.01f, 0.1f)] [SerializeField] private float _moveIncreasePerDescending = 0.05f;
        [SerializeField] private float _maxMoveTimer = 0.05f;
        [SerializeField] private float _moveTimer = 0.5f;
        [SerializeField] private float _translate_X_Amount = 0.5f;
        [SerializeField] private float _translate_Y_Amount = 0.5f;
        [SerializeField] private float _timeToMove;

        private AudioManager _audioManager;
        private GameArea _gameArea;
        private GameManager _gameManager;
        private int _changeDirection = 1;
        private bool _movingDown = false;
        private Transform _thisTransform;
        private CompositeCollider2D _aliensFormationCollider;
        private Action OnCheckIfAllowedToShoot;
        private readonly List<GameObject> _aliens = new List<GameObject>();


        private void Awake() => RegisterService(this);

        private void Start() => Initialization();


        private void Initialization()
        {
            _thisTransform = transform;
            _aliensFormationCollider = GetComponent<CompositeCollider2D>();
            _gameManager = ServiceLocator.Resolve<GameManager>();
            _gameArea = ServiceLocator.Resolve<GameArea>();
            _audioManager = ServiceLocator.Resolve<AudioManager>();

            foreach (var alien in _aliens)
                OnCheckIfAllowedToShoot += alien.GetComponent<AlienShoot>().CheckIfAllowedToShoot;
        }


        private void Update()
        {
            MoveAliens();
            MoveAliensDown();
        }


        private void OnTriggerEnter2D(Collider2D hitCollider)
        {
            if (hitCollider.CompareTag(nameof(Shield)))
                hitCollider.gameObject.SetActive(false);
        }


        private void MoveAliens()
        {
            _timeToMove -= Time.deltaTime;

            if (_timeToMove < float.Epsilon)
            {
                _thisTransform.Translate(_translate_X_Amount * _changeDirection, 0f, 0f);
                _timeToMove = _moveTimer;

                _aliens.ForEach(alien => alien.GetComponent<AlienAnimation>().AnimateAlien());
            }

            if (_moveTimer <= _maxMoveTimer)
                _moveTimer = _maxMoveTimer;
        }


        private void MoveAliensDown()
        {
            var minBounds = _aliensFormationCollider.bounds.min;
            var maxBounds = _aliensFormationCollider.bounds.max;

            if (minBounds.x < _gameArea.xMin || maxBounds.x > _gameArea.xMax)
            {
                if (!_movingDown)
                {
                    _movingDown = true;
                    _thisTransform.position += Vector3.down * _translate_Y_Amount;
                    _changeDirection *= -1;

                    _moveTimer -= _moveIncreasePerDescending;
                }
            }
            else
                _movingDown = false;

            if (minBounds.y < _gameArea.LimitLose)
                LoseGame();
        }


        public void AddAlien(GameObject alien) => _aliens.Add(alien);


        public void ProcessHit(GameObject alien)
        {
            if (_aliens.DoesNotContain(alien)) return;

            StartCoroutine(RemoveAlien(alien));
            OnCheckIfAllowedToShoot?.Invoke();

            if (_aliens.Count == 0)
                WinGame();
        }


        private IEnumerator RemoveAlien(GameObject alien)
        {
            OnCheckIfAllowedToShoot -= alien.GetComponent<AlienShoot>().CheckIfAllowedToShoot;

            alien.GetComponent<SpriteRenderer>().sprite = _explosionSprite;
            Array.ForEach(alien.GetComponents<Collider2D>(), collider => collider.enabled = false);

            _aliens.Remove(alien);
            _audioManager?.PlaySFX("AlienExplosion");
            _moveTimer -= _moveIncreasePerAlienKilled;

            yield return new WaitForSeconds(0.5f);

            alien.SetActive(false);
        }


        private void WinGame()
        {
            _gameManager.WinGame();
            this.gameObject.SetActive(false);
        }

        private void LoseGame() => _gameManager.LoseGame();
    }
}