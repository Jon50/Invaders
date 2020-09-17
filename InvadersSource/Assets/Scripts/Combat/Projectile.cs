using UnityEngine;
using System;
using Invaders.Core;
using Invaders.Locator;

using static Invaders.Enums.Tags;

namespace Invaders.Combat
{
    public class Projectile : MonoBehaviour
    {
        private GameArea _gameArea;
        private Rigidbody2D _rb2d = null;
        private Vector2 _direction = Vector2.zero;
        private Type _targetType;
        private float _velocity = 0f;
        private float _explosionRadius = 0f;
        private bool _hasTarget = false;

        public Action OnHitReloadProjectile = null;


        public void InitializeProjectile(ProjectileConfiguration configuration, Type targetType)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = configuration.color;

            _velocity = configuration.velocity;
            _direction = configuration.direction;
            _explosionRadius = configuration.explosionRadius;
            _targetType = targetType;
        }


        private void Start()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _gameArea = ServiceLocator.Resolve<GameArea>();
        }

        private void OnEnable() => _hasTarget = false;

        private void FixedUpdate()
        {
            MoveProjectile();
            OutOfBoundsReload();
        }


        private void MoveProjectile() => _rb2d.velocity = _velocity * _direction * Time.deltaTime;


        private void OutOfBoundsReload()
        {
            var yPosition = _rb2d.position.y;
            var yMin = _gameArea.yMin;
            var yMax = _gameArea.yMax;

            if (yPosition < yMin || yPosition > yMax)
                OnHitReloadProjectile?.Invoke();
        }


        private void OnTriggerEnter2D(Collider2D hitCollider)
        {
            if (_hasTarget) return;

            if (hitCollider.CompareTag(nameof(Shield)))
            {
                ShieldHit(hitCollider);
                return;
            }

            var target = hitCollider.GetComponent<ICombatTarget>();
            if (target == null || target.GetType() == _targetType) return;

            _hasTarget = true;

            OnHitReloadProjectile?.Invoke();
            target.ProcessHit();
        }


        private void ShieldHit(Collider2D hitCollider)
        {
            _hasTarget = true;
            hitCollider.gameObject.SetActive(false);

            var colliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag(nameof(Shield)))
                {
                    var rnd = (UnityEngine.Random.Range(-1, 1) == 0) ? true : false;
                    if (rnd) continue;
                    collider.gameObject.SetActive(rnd);
                }
            }

            if (OnHitReloadProjectile != null)
                OnHitReloadProjectile.Invoke();
            else
                this.gameObject.SetActive(false);
        }
    }
}