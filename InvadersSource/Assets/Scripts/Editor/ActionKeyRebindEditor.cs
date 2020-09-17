using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(ActionKeyRebind))]
[CanEditMultipleObjects]
public class ActionKeyRebindEditor : Editor
{
    private SerializedProperty m_ScriptField;
    private SerializedProperty _playerInputProperty;
    private SerializedProperty _cancelActionProperty;
    private SerializedProperty _bindingTextProperty;
    private SerializedProperty _needsBindingProperty;
    private int _selectedCancelActionIndex;

    #region Action Maps Properties
    private GUIContent _actionMapLabel = new GUIContent("Action Map");
    private SerializedProperty _actionMapIdProperty;
    private GUIContent[] _actionMapOptions;
    private string[] _actionMapOptionValue;
    private int _selectedActionMapIndex;
    #endregion

    #region Actions Properties
    private GUIContent _actionLabel = new GUIContent("Action");
    private SerializedProperty _actionIdProperty;
    private GUIContent[] _actionOptions;
    private string[] _actionOptionValue;
    private int _selectedActionIndex;
    #endregion

    #region Bindings Properties
    private GUIContent _bindingLabel = new GUIContent("Binding");
    private SerializedProperty _bindingIdProperty;
    private GUIContent[] _bindingOptions;
    private string[] _bindingOptionValue;
    private int _selectedBindingIndex;
    private List<GUIContent> _guiContents = new List<GUIContent>();
    private List<string> _inputBindings = new List<string>();
    #endregion

    private void OnEnable()
    {
        m_ScriptField = serializedObject.FindProperty("m_Script");

        _playerInputProperty = serializedObject.FindProperty("_playerInput");
        _cancelActionProperty = serializedObject.FindProperty("_cancelAction");
        _bindingTextProperty = serializedObject.FindProperty("_bindingText");
        _needsBindingProperty = serializedObject.FindProperty("_needsBinding");

        _actionMapIdProperty = serializedObject.FindProperty("_actionMapId");
        _actionIdProperty = serializedObject.FindProperty("_actionId");
        _bindingIdProperty = serializedObject.FindProperty("_bindingId");

        RefreshInspector();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(m_ScriptField);
        GUI.enabled = true;

        EditorGUILayout.LabelField("Bindings", EditorStyles.boldLabel);

        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(_playerInputProperty);
        EditorGUILayout.PropertyField(_bindingTextProperty);
        EditorGUI.indentLevel = 0;

        if (_playerInputProperty.objectReferenceValue != null)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                var currentActionMapIndex = EditorGUILayout.Popup(_actionMapLabel, _selectedActionMapIndex, _actionMapOptions);
                if (currentActionMapIndex != _selectedActionMapIndex)
                {
                    var actionMapId = _actionMapOptionValue[currentActionMapIndex];
                    _actionMapIdProperty.stringValue = actionMapId;
                    _selectedActionMapIndex = currentActionMapIndex;
                }

                EditorGUILayout.BeginHorizontal();
                var currentActionIndex = EditorGUILayout.Popup(_actionLabel, _selectedActionIndex, _actionOptions, GUILayout.MinWidth(250));
                if (currentActionIndex != _selectedActionIndex)
                {
                    var actionId = _actionOptionValue[currentActionIndex];
                    _actionIdProperty.stringValue = actionId;
                    _selectedActionIndex = currentActionIndex;

                    _bindingIdProperty.stringValue = "";
                    _selectedBindingIndex = -1;
                }

                var originalWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 150;

                _needsBindingProperty.boolValue = EditorGUILayout.Toggle("Needs Specific Binding", _needsBindingProperty.boolValue);

                EditorGUIUtility.labelWidth = originalWidth;
                EditorGUILayout.EndHorizontal();

                if (_needsBindingProperty.boolValue)
                {
                    var currentBindingIndex = EditorGUILayout.Popup(_bindingLabel, _selectedBindingIndex, _bindingOptions);
                    if (currentBindingIndex != _selectedBindingIndex)
                    {
                        var bindingId = _bindingOptionValue[currentBindingIndex];
                        _bindingIdProperty.stringValue = bindingId;
                        _selectedBindingIndex = currentBindingIndex;
                    }
                }
            }

            EditorGUILayout.LabelField("Cancel Action", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                // EditorGUILayout.PropertyField(_cancelActionProperty);
                var currentActionIndex = EditorGUILayout.Popup(new GUIContent("Cancel Action"), _selectedCancelActionIndex, _actionOptions);
                if (currentActionIndex != _selectedCancelActionIndex)
                {
                    var actionId = _actionOptionValue[currentActionIndex];
                    _cancelActionProperty.stringValue = actionId;
                    _selectedCancelActionIndex = currentActionIndex;
                }
            }
        }
        else
        {
            Clear();
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            RefreshInspector();
        }
    }

    private void Clear()
    {
        _actionMapOptions = new GUIContent[0];
        _actionMapOptionValue = new string[0];
        _selectedActionMapIndex = -1;

        _actionOptions = new GUIContent[0];
        _actionOptionValue = new string[0];
        _selectedActionIndex = -1;
        _selectedCancelActionIndex = -1;

        _bindingOptions = new GUIContent[0];
        _bindingOptionValue = new string[0];
        _selectedBindingIndex = -1;
    }

    private void RefreshInspector()
    {
        var playerInput = (PlayerInput)_playerInputProperty.objectReferenceValue;
        var generalActions = playerInput?.actions;

        if (playerInput == null)
        {
            Clear();
            return;
        }

        var actionMaps = generalActions.actionMaps;
        var actionMapCount = actionMaps.Count;

        _actionMapOptions = new GUIContent[actionMapCount];
        _actionMapOptionValue = new string[actionMapCount];
        _selectedActionMapIndex = -1;

        var currentActionMap = _actionMapIdProperty.stringValue;

        for (int i = 0; i < actionMapCount; i++)
        {
            #region Action Map ID
            var actionMap = actionMaps[i];
            var actionMapId = actionMap.id.ToString();
            var displayMap = actionMap.name;

            _actionMapOptions[i] = new GUIContent(displayMap);
            _actionMapOptionValue[i] = actionMapId;

            if (currentActionMap == actionMapId)
            {
                _selectedActionMapIndex = i;

                #region Action ID
                var actions = actionMap.actions;
                var actionCount = actions.Count;

                _actionOptions = new GUIContent[actionCount];
                _actionOptionValue = new string[actionCount];
                _selectedActionIndex = -1;
                _selectedCancelActionIndex = -1;

                var currentAction = _actionIdProperty.stringValue;
                var currentCancelAction = _cancelActionProperty.stringValue;

                for (int k = 0; k < actionCount; k++)
                {
                    var action = actions[k];
                    var actionId = action.id.ToString();
                    var displayAction = action.name;

                    _actionOptions[k] = new GUIContent(displayAction);
                    _actionOptionValue[k] = actionId;

                    if (currentCancelAction == actionId)
                    {
                        _selectedCancelActionIndex = k;
                        continue;
                    }

                    if (currentAction == actionId)
                    {
                        _selectedActionIndex = k;

                        if (!_needsBindingProperty.boolValue)
                            continue;

                        #region Binding ID
                        var bindings = action.bindings;
                        var bindingCount = bindings.Count;

                        _guiContents.Clear();
                        _inputBindings.Clear();
                        _selectedBindingIndex = -1;

                        var currentBinding = _bindingIdProperty.stringValue;

                        var showOneComposite = 0;
                        for (int j = 0; j < bindingCount; j++)
                        {
                            if (bindings[j].isComposite || showOneComposite > 1)
                            {
                                showOneComposite++;
                                continue;
                            }

                            var binding = bindings[j];
                            var displayBinding = binding.name;

                            if (string.IsNullOrEmpty(displayBinding))
                            {
                                var start = binding.path.IndexOf('/') + 1;
                                var bind = binding.path.Substring(start);

                                _guiContents.Add(new GUIContent(bind));
                                _inputBindings.Add(bind);
                                continue;
                            }

                            _guiContents.Add(new GUIContent(displayBinding));
                            _inputBindings.Add(displayBinding);

                        }

                        for (int m = 0; m < _inputBindings.Count; m++)
                        {
                            if (currentBinding == _inputBindings[m])
                            {
                                _selectedBindingIndex = m;
                            }
                        }

                        _bindingOptions = _guiContents.ToArray();
                        _bindingOptionValue = _inputBindings.ToArray();
                        #endregion
                    }
                }
                #endregion
            }
            #endregion
        }
    }
}