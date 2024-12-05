#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SOBrowserMain : Editor
{
    private VisualElement _rootVisualElement;

    private SerializedObject _serializedObject;
    private SerializedProperty _property;

    private ScrollView _scriptableObjectList, _contentList;

    private List<ScriptableObject> _scriptableObjects = new List<ScriptableObject>();

    private Label _selectedScriptableLabel;

    public bool IsGUIPainted = false;

    public static SOBrowserMain CreateInstance(VisualElement root, ScriptableObject scriptableObj)
    {
        SOBrowserMain instance = new SOBrowserMain();

        instance._rootVisualElement = root;
        instance.IsGUIPainted = false;

        instance.Initialize(scriptableObj);

        return instance;
    }

    private void Initialize(ScriptableObject scriptableObj)
    {
        _scriptableObjectList = _rootVisualElement.Q<ScrollView>("ItemsList");
        _contentList = _rootVisualElement.Q<ScrollView>("ContentList");

        ToolbarSearchField scriptableSearchField = _rootVisualElement.Q<ToolbarSearchField>("ScriptableSearchField");
        scriptableSearchField.RegisterValueChangedCallback(OnScriptableSearched);

        Button directoryButton = _rootVisualElement.Q<Button>("DirectoryButton");
        directoryButton.clicked += () =>
        {
            EditorGUIUtility.PingObject(_serializedObject.targetObject);
        };

        _selectedScriptableLabel = _rootVisualElement.Q<Label>("SelectedScriptableText");

        Type type = scriptableObj.GetType();

        Button createObjectButton = _rootVisualElement.Q<Button>("CreateButton");
        createObjectButton.text = $"Create {type.Name} Clone";

        LoadScriptableObjectData(scriptableObj);
        ListAllScriptableObjects(type);
    }

    public void PaintMainGUI()
    {
        IsGUIPainted = true;

        _contentList.Clear();

        _property.NextVisible(true);

        while (_property.NextVisible(true))
        {
            if (_property.propertyPath.Contains("Array") || _property.propertyPath.Contains('.'))
            {
                continue;
            }

            PropertyField field = new PropertyField(_property);
            field.Bind(_serializedObject);

            _contentList.Add(field);
        }
    }

    private void ListAllScriptableObjects(System.Type searchingType)
    {
        _scriptableObjectList.Clear();
        _scriptableObjects.Clear();

        string[] scriptableObjectFiles = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);

        foreach (string file in scriptableObjectFiles)
        {
            string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
            ScriptableObject scriptableObject = (ScriptableObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject));

            if (scriptableObject != null && scriptableObject.GetType() == searchingType)
            {
                Button button = CreateScriptableObjectButton(scriptableObject);

                _scriptableObjectList.Add(button);
                _scriptableObjects.Add(scriptableObject);
            }
        }
    }

    private void LoadScriptableObjectData(ScriptableObject scriptableObj)
    {
        _serializedObject = new SerializedObject(scriptableObj);
        _property = _serializedObject.GetIterator();

        IsGUIPainted = false;

        _selectedScriptableLabel.text = scriptableObj.name;
    }

    private Button CreateScriptableObjectButton(ScriptableObject scriptableObj)
    {
        Button button = new Button();
        button.text = scriptableObj.name;
        button.clicked += () =>
        {
            LoadScriptableObjectData(scriptableObj);
        };

        return button;
    }

    public void OnScriptableObjectCreated(ScriptableObject obj)
    {
        Button button = CreateScriptableObjectButton(obj);
        _scriptableObjectList.Add(button);
    }

    private void OnScriptableSearched(ChangeEvent<string> evt)
    {
        string searchText = evt.newValue;

        List<ScriptableObject> matchingScriptableObjects = _scriptableObjects.Where(x => x.name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                                                                            .ToList();

        _scriptableObjectList.Clear();
        foreach (ScriptableObject scriptableObj in matchingScriptableObjects)
        {
            Button button = CreateScriptableObjectButton(scriptableObj);

            _scriptableObjectList.Add(button);
        }
    }
}
#endif