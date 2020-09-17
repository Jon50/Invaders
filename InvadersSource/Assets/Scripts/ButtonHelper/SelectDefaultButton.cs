using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectDefaultButton : MonoBehaviour
{
    private Button _defaultButton;
    private Slider _defaultSlider;
    private Toggle _defaultToggle;
    private EventSystem _eventSystem;


    private void Awake()
    {
        _defaultButton = GetComponent<Button>();
        _defaultSlider = GetComponent<Slider>();
        _defaultToggle = GetComponent<Toggle>();
        _eventSystem = EventSystem.current;
    }


    private void OnEnable()
    {
        _eventSystem?.SetSelectedGameObject(null);

        if (_defaultButton.IsNotNull())
        {
            _defaultButton.enabled = false;
            _defaultButton.enabled = true;
            _defaultButton.Select();
        }

        if (_defaultSlider.IsNotNull())
        {
            _defaultSlider.enabled = false;
            _defaultSlider.enabled = true;
            _defaultSlider.Select();
        }

        if (_defaultToggle.IsNotNull())
        {
            _defaultToggle.enabled = false;
            _defaultToggle.enabled = true;
            _defaultToggle.Select();
        }
    }
}
