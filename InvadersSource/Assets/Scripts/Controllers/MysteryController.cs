using Invaders.Managers;
using Invaders.Combat;
using Invaders.Core;
using Invaders.Locator;
using UnityEngine;
using System.Collections;

namespace Invaders.Control
{
    public class MysteryController : MonoBehaviour, IProcessHit
    {
        [SerializeField] private Sprite _explosionSprite;

        [Header("Settings")]
        [SerializeField] private float _minRandomSpwnTimer = 20;
        [SerializeField] private float _maxRandomSpwnTimer = 30;

        private AudioManager _audioManager;
        private float _spawnTimer;
        private Transform _thisTransform;
        private SpriteRenderer _spriteRenderer;
        private GameAreaKeeper _gameAreaKeeper;
        private int _direction = 1;
        private float _speed = 5f;
        private bool _wasHit = false;


        private void Awake() => Initialize();


        public void Initialize()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.enabled = false;

            _spawnTimer = RandomSpawnTimer();
            _thisTransform = GetComponent<Transform>();
            _gameAreaKeeper = GetComponent<GameAreaKeeper>();
        }

        private void Start() => _audioManager = ServiceLocator.Resolve<AudioManager>();


        private void Update()
        {
            NextTimeToSpawn();
            Move();
        }


        private void NextTimeToSpawn()
        {
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer < Mathf.Epsilon)
            {
                var leftSide = RandomSpawnSide();

                if (leftSide)
                    SetDirection(spawnSide: _gameAreaKeeper.xMin, direction: 1);
                else
                    SetDirection(spawnSide: _gameAreaKeeper.xMax, direction: -1);
            }
        }


        private void SetDirection(float spawnSide, int direction)
        {
            _audioManager?.PlaySFX("Mystery");
            _thisTransform.position = new Vector2(spawnSide, _thisTransform.position.y);

            _spriteRenderer.enabled = true;
            _direction = direction;
            _spawnTimer = RandomSpawnTimer();
        }


        private void Move()
        {
            if (_wasHit) { return; }

            _thisTransform.Translate(Vector2.right * _direction * _speed * Time.deltaTime);
            _spriteRenderer.color = Color.HSVToRGB(Mathf.PingPong(Time.time, 1f), 1f, 1f);
        }


        private bool RandomSpawnSide() => (Random.Range(0, 2) == 0) ? true : false;
        private float RandomSpawnTimer() => Random.Range(_minRandomSpwnTimer, _maxRandomSpwnTimer);


        public void ProcessHit(GameObject obj = default)
        {
            StartCoroutine(DeathProcess());
        }

        private IEnumerator DeathProcess()
        {
            var originalSprite = _spriteRenderer.sprite;

            _spriteRenderer.sprite = _explosionSprite;
            _audioManager?.PlaySFX("AlienExplosion");
            _spawnTimer = RandomSpawnTimer();
            _wasHit = true;

            yield return new WaitForSeconds(0.5f);

            _spriteRenderer.sprite = originalSprite;
            _spriteRenderer.enabled = false;
            _wasHit = false;
        }
    }
}
