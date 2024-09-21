using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SOBrowserMain : Editor
{
    private VisualElement rootVisualElement;

    private SerializedObject _serializedObject;
    private SerializedProperty _property;

    private ScrollView scriptableObjectList, contentList;

    private List<ScriptableObject> scriptableObjects = new List<ScriptableObject>();

    private Label selectedScriptableLabel;

    public bool isGUIPainted = false;

    public static SOBrowserMain CreateInstance(VisualElement root, ScriptableObject scriptableObj)
    {
        SOBrowserMain instance = new SOBrowserMain();

        instance.rootVisualElement = root;
        instance.isGUIPainted = false;

        instance.Initialize(scriptableObj);

        return instance;
    }

    private void Initialize(ScriptableObject scriptableObj)
    {
        scriptableObjectList = rootVisualElement.Q<ScrollView>("ItemsList");
        contentList = rootVisualElement.Q<ScrollView>("ContentList");

        ToolbarSearchField scriptableSearchField = rootVisualElement.Q<ToolbarSearchField>("ScriptableSearchField");
        scriptableSearchField.RegisterValueChangedCallback(OnScriptableSearched);

        Button directoryButton = rootVisualElement.Q<Button>("DirectoryButton");
        directoryButton.clicked += () =>
        {
            EditorGUIUtility.PingObject(_serializedObject.targetObject);
        };

        selectedScriptableLabel = rootVisualElement.Q<Label>("SelectedScriptableText");

        Type type = scriptableObj.GetType();

        Button createObjectButton = rootVisualElement.Q<Button>("CreateButton");
        createObjectButton.text = $"Create {type.Name} Clone";

        LoadScriptableObjectData(scriptableObj);
        ListAllScriptableObjects(type);
    }

    public void PaintMainGUI()
    {
        isGUIPainted = true;

        contentList.Clear();

        _property.NextVisible(true);

        while (_property.NextVisible(true))
        {
            if (_property.propertyPath.Contains("Array") || _property.propertyPath.Contains('.'))
            {
                continue;
            }

            PropertyField field = new PropertyField(_property);
            field.Bind(_serializedObject);

            contentList.Add(field);
        }
    }

    private void ListAllScriptableObjects(System.Type searchingType)
    {
        scriptableObjectList.Clear();
        scriptableObjects.Clear();

        string[] scriptableObjectFiles = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);

        foreach (string file in scriptableObjectFiles)
        {
            string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
            ScriptableObject scriptableObject = (ScriptableObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject));

            if (scriptableObject != null && scriptableObject.GetType() == searchingType)
            {
                Button button = CreateScriptableObjectButton(scriptableObject);

                scriptableObjectList.Add(button);
                scriptableObjects.Add(scriptableObject);
            }
        }
    }

    private void LoadScriptableObjectData(ScriptableObject scriptableObj)
    {
        _serializedObject = new SerializedObject(scriptableObj);
        _property = _serializedObject.GetIterator();

        isGUIPainted = false;

        selectedScriptableLabel.text = scriptableObj.name;
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
        scriptableObjectList.Add(button);
    }

    private void OnScriptableSearched(ChangeEvent<string> evt)
    {
        string searchText = evt.newValue;

        List<ScriptableObject> matchingScriptableObjects = scriptableObjects.Where(x => x.name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                                                                            .ToList();

        scriptableObjectList.Clear();
        foreach (ScriptableObject scriptableObj in matchingScriptableObjects)
        {
            Button button = CreateScriptableObjectButton(scriptableObj);

            scriptableObjectList.Add(button);
        }
    }
}
