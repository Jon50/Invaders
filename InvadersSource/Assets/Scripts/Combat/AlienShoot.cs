using System.Collections;
using UnityEngine;

using Invaders.Managers;
using Invaders.Locator;

namespace Invaders.Combat
{
    public class AlienShoot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Projectile _projectilePrefab = null;
        [SerializeField] private ProjectileConfiguration _projectileConfiguration = null;
        [SerializeField] private Transform _weaponPosition = null;


        [Header("Random Shoot Timer")]
        [SerializeField] private float _minTimer = 0;
        [SerializeField] private float _maxTimer = 0;

        [Space]
        [SerializeField] private LayerMask layer = default;


        private AudioManager _audioManager;
        private Projectile _projectile = null;
        private bool _canShoot = true;
        private float _shootTimer = 0f;
        private WaitForSeconds _waitForOneSecond = new WaitForSeconds(1f);


        private void Awake() => InitializeProjectile();


        private void InitializeProjectile()
        {
            var targetType = GetComponent<ICombatTarget>().GetType();

            _projectile = Instantiate(_projectilePrefab, _weaponPosition.position, Quaternion.identity);
            _projectile.InitializeProjectile(_projectileConfiguration, targetType);
            _projectile.gameObject.SetActive(false);
        }


        private void OnEnable() => _projectile.OnHitReloadProjectile += ReloadProjectile;
        private void OnDisable() => _projectile.OnHitReloadProjectile -= ReloadProjectile;


        public void SetProjectileHolder(Transform holder) => _projectile.transform.SetParent(holder);


        private IEnumerator Start()
        {
            _audioManager = ServiceLocator.Resolve<AudioManager>();

            RandomizeShootingTime();
            yield return _waitForOneSecond;
            CheckIfAllowedToShoot();
        }


        private void Update()
        {
            _shootTimer -= Time.deltaTime;

            if (_canShoot && _shootTimer < float.Epsilon)
                Shoot();
        }


        public void CheckIfAllowedToShoot()
        {
            if (Physics2D.Raycast(transform.position - Vector3.up * 0.5f, Vector2.down, 0.5f, layer))
            {
                _canShoot = false;
                RandomizeShootingTime();
                return;
            }

            _canShoot = true;
        }


        private void Shoot()
        {
            _audioManager?.PlaySFX("AlienShoot");
            _projectile.gameObject.SetActive(true);
            _projectile.transform.position = _weaponPosition.position;
            RandomizeShootingTime();
        }


        private void RandomizeShootingTime() => _shootTimer = UnityEngine.Random.Range(_minTimer, _maxTimer);


        private void ReloadProjectile()
        {
            _projectile.gameObject.SetActive(false);
            _projectile.transform.position = _weaponPosition.position;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position - Vector3.up * 0.5f, Vector2.down * 0.5f);
        }
#endif
    }
}