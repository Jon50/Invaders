using UnityEngine;
using UnityEngine.InputSystem;

namespace Invaders.Control
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _playerSpawnPosition;

        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 4f;

        private PlayerInput _playerInput;
        private Transform _thisTransform;
        private Vector2 _direction = Vector2.zero;
        private bool _moveLeft;
        private bool _moveRight;

        private void Awake()
        {
            _thisTransform = transform;
            _thisTransform.position = _playerSpawnPosition.position;
            _playerSpawnPosition.SetParent(null);
            _playerInput = PlayerInputRef.PlayerInput;
        }


        private void Update()
        {
            MovePlayer();
            AndroidMove();
        }


        private void MovePlayer()
        {
            _direction.x = _playerInput.actions["Move"].ReadValue<float>();
            _thisTransform.Translate(_direction * _moveSpeed * Time.deltaTime);
        }


        private void AndroidMove()
        {
            float direction = 0f;

            if (_moveLeft)
                direction = -1f;
            if (_moveRight)
                direction = 1f;

            _direction.x = direction;
            _thisTransform.Translate(_direction * _moveSpeed * Time.deltaTime);
        }


        public void MoveLeft() => _moveLeft = !_moveLeft;

        public void MoveRight() => _moveRight = !_moveRight;


        public void ResetPosition()
        {
            _thisTransform.gameObject.SetActive(false);
            _thisTransform.position = _playerSpawnPosition.position;
            _thisTransform.gameObject.SetActive(true);
        }
    }
}