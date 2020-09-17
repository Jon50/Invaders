using UnityEngine;
using UnityEngine.InputSystem;

using Invaders.Managers;
using Invaders.Locator;

namespace Invaders.Combat
{
    public class PlayerShoot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Projectile _projectilePrefab = null;
        [SerializeField] private ProjectileConfiguration _projectileConfiguration = null;
        [SerializeField] private Transform _projectilePosition = null;

        private AudioManager _audioManager;
        private PlayerInput _playerInput = default;
        private Projectile _projectile = null;
        private bool _canShoot = true;


        private void Awake()
        {
            var targetType = GetComponent<ICombatTarget>().GetType();

            _projectile = Instantiate(_projectilePrefab, _projectilePosition.position, Quaternion.identity);
            _projectile.InitializeProjectile(_projectileConfiguration, targetType);
            _projectile.gameObject.SetActive(false);

            _playerInput = PlayerInputRef.PlayerInput;
        }


        private void Start() => _audioManager = ServiceLocator.Resolve<AudioManager>();

        private void OnEnable() => _projectile.OnHitReloadProjectile += ReloadProjectile;

        private void OnDisable() => _projectile.OnHitReloadProjectile -= ReloadProjectile;


        private void Update()
        {
            if (_canShoot && _playerInput.actions["Shoot"].triggered)
            {
                _audioManager?.PlaySFX("PlayerShoot");
                SetProjectileState(canShoot: false, active: true);
            }
        }


        public void AndroidShoot()
        {
            if (_canShoot)
            {
                _audioManager?.PlaySFX("PlayerShoot");
                SetProjectileState(canShoot: false, active: true);
            }
        }


        private void ReloadProjectile() => SetProjectileState(canShoot: true, active: false);


        private void SetProjectileState(bool canShoot, bool active)
        {
            _canShoot = canShoot;
            _projectile.gameObject.SetActive(active);
            _projectile.gameObject.transform.position = _projectilePosition.position;
        }
    }
}