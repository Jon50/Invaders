using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ActionKeyRebind : MonoBehaviour
{
#pragma warning disable CS0414
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private TextMeshProUGUI _bindingText;
    [SerializeField] private string _actionMapId;
    [SerializeField] private string _actionId;
    [SerializeField] private string _bindingId;
    [SerializeField] private string _cancelAction;
    [SerializeField] private bool _needsBinding;
#pragma warning restore

    private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

    private void OnEnable() => UpdateBindingUI();


    public void StartRebindInteraction()
    {
        if (!ResolveActionAndBinding(out var action, out var bindingIndex))
            return;

        PerformInteractiveRebind(action, bindingIndex);
    }


    private bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
    {
        action = _playerInput.actions[_actionId];
        bindingIndex = 0;

        if (action == null)
        {
#if UNITY_EDITOR
            Debug.LogError($"Action {_actionId} does not exist - {gameObject.name}");
#endif
            return false;
        }

        foreach (var binding in action.bindings)
        {
            if (binding.isComposite || binding.groups != _playerInput.currentControlScheme)
            {
                bindingIndex++;
                continue;
            }

            var bindingName = binding.name;
            if (string.IsNullOrEmpty(bindingName) && binding.groups == _playerInput.currentControlScheme)
                break;

            var isPartOfComposite = binding.isPartOfComposite;
            var doesNameContainId = string.IsNullOrEmpty(bindingName) ? false : bindingName.ToLower().Contains(_bindingId.ToLower());
            var isSameControlScheme = binding.groups == _playerInput.currentControlScheme;

            // Debug.Log("Is part of composite :: " + isPartOfComposite);
            // Debug.Log("Does name contain ID :: " + doesNameContainId + " :: " + bindingName + " - " + _bindingId);
            // Debug.Log(isSameControlScheme + " :: " + _playerInput.currentControlScheme);

            if (isPartOfComposite && doesNameContainId && isSameControlScheme)
                break;

            bindingIndex++;
        }

        return true;
    }


    private void PerformInteractiveRebind(InputAction action, int bindingIndex)
    {
        _rebindOperation?.Cancel();

        void Clear()
        {
            _rebindOperation?.Dispose();
            _rebindOperation = null;
        }

        action.Disable();

        _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
        .OnCancel(
            operation =>
            {
                UpdateBindingUI();
                Clear();
            })
        .WithCancelingThrough(
            (
                from binding in _playerInput.actions[_cancelAction].bindings
                where binding.groups == _playerInput.currentControlScheme
                select binding).FirstOrDefault().path
            )
        .OnApplyBinding(
            (operation, keyPath) =>
            {
                action.ChangeBinding(bindingIndex).WithPath(keyPath);
                // ResolveBindingNames();
            })
        .OnComplete(
            operation =>
            {
                UpdateBindingUI();
                Clear();
            });

        action.Enable();

        _rebindOperation.Start();
    }


    public void UpdateBindingUI()
    {
        if (_bindingText != null)
            _bindingText.text = ResolveBindingNames();
    }


    public void ResetToDefault()
    {
        var defaultPlayerControls = new PlayerControls();
        _playerInput.actions.LoadFromJson(defaultPlayerControls.asset.ToJson());

        UpdateBindingUI();
    }


    public void OnButtonOrKey_ResetPlayerInput(bool isUIButton = false)
    {
        if (PlayerInputRef.PlayerInput.actions["CancelReturn"].triggered || isUIButton)
        {
            _playerInput.enabled = false;
            _playerInput.enabled = true;
        }
    }


    private string ResolveBindingNames()
    {
        if (!ResolveActionAndBinding(out var action, out var bindingIndex))
            return default;

        var binding = action.bindings[bindingIndex];
        var bindingName = string.IsNullOrEmpty(binding.name) ? BuildFromPath() : binding.name;
        var displayName = BuildFromPath();

        string BuildFromPath()
        {
            var start = binding.path.LastIndexOf('/') + 1;
            var bindName = binding.path.Substring(start);
            return bindName.ToUpper();
        }

        return displayName;
    }
}