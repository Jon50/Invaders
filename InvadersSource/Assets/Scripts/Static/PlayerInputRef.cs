using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputRef : MonoBehaviour
{
    private static PlayerInput _playerInput;
    public static PlayerInput PlayerInput
    {
        get
        {
            if (_playerInput == null)
                _playerInput = FindObjectOfType<PlayerInput>();

            return _playerInput;
        }
    }

    private void Awake() => _playerInput = GetComponent<PlayerInput>();
}
