#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SOBrowserHome : Editor
{
    private VisualElement _rootVisualElement;

    private ScrollView _typeList;
    private List<ScriptableObject> _types = new List<ScriptableObject>();

    public Action<ScriptableObject> OnTypeSelected;

    public static SOBrowserHome CreateInstance(VisualElement rootVisualElement, string[] ignoredNamespaces)
    {
        SOBrowserHome instance = new SOBrowserHome();

        instance._rootVisualElement = rootVisualElement;
        instance.Initialize();
        instance.ListAllTypes(ignoredNamespaces);

        return instance;
    }

    private void Initialize()
    {
        _typeList = _rootVisualElement.Q<ScrollView>("ScrollView");
        ToolbarSearchField typeSearchField = _rootVisualElement.Q<ToolbarSearchField>("SearchField");

        typeSearchField.RegisterValueChangedCallback(OnTypeSearched);

        Button configButton = _rootVisualElement.Q<Button>("ConfigButton");
        configButton.clicked += () =>
        {
            SOBrowserConfigEditor.OpenConfigWindow();
        };

        SOBrowserConfigEditor.OnIgnoredNamespacesChanged += OnIgnoredNamespacesChanged;
    }

    private void ListAllTypes(string[] ignoredNamespaces)
    {
        _typeList.Clear();

        _types = new List<ScriptableObject>();

        string[] scriptableObjectFiles = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);

        foreach (string file in scriptableObjectFiles)
        {
            string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
            ScriptableObject scriptableObject = (ScriptableObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject));

            if (scriptableObject != null)
            {
                string typeName = scriptableObject.GetType().Name;
                string namespaceName = scriptableObject.GetType().Namespace;

                if (_types.Any(x => x.GetType().Name.Equals(typeName)) ||
                   (!string.IsNullOrEmpty(namespaceName) && ignoredNamespaces.Any(x => x.Contains(namespaceName) || namespaceName.Contains(x)))) continue;

                Button button = CreateTypeButton(typeName, scriptableObject);

                _typeList.Add(button);
                _types.Add(scriptableObject);
            }
        }
    }

    private Button CreateTypeButton(string typeName, ScriptableObject obj)
    {
        Button button = new Button();
        button.text = typeName;
        button.clicked += () =>
        {
            OnTypeSelected?.Invoke(obj);
        };

        button.style.width = 200;
        button.style.height = 40;

        return button;
    }

    private void OnTypeSearched(ChangeEvent<string> evt)
    {
        string searchText = evt.newValue;

        List<ScriptableObject> matchingScriptableObjects = _types.Where(x => x.GetType().Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();

        _typeList.Clear();
        foreach (ScriptableObject scriptableObj in matchingScriptableObjects)
        {
            Button button = CreateTypeButton(scriptableObj.GetType().Name, scriptableObj);

            _typeList.Add(button);
        }
    }

    private void OnIgnoredNamespacesChanged()
    {
        ListAllTypes(SOBrowserConfigEditor.ReadConfig());
    }
}
#endif