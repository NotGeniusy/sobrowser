using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SOBrowserHome : Editor
{
    private VisualElement rootVisualElement;

    private ScrollView typeList;
    private List<ScriptableObject> types = new List<ScriptableObject>();

    public Action<ScriptableObject> OnTypeSelected;

    public static SOBrowserHome CreateInstance(VisualElement rootVisualElement, string[] ignoredNamespaces)
    {
        SOBrowserHome instance = new SOBrowserHome();

        instance.rootVisualElement = rootVisualElement;
        instance.Initialize();
        instance.ListAllTypes(ignoredNamespaces);

        return instance;
    }

    private void Initialize()
    {
        typeList = rootVisualElement.Q<ScrollView>("ScrollView");
        ToolbarSearchField typeSearchField = rootVisualElement.Q<ToolbarSearchField>("SearchField");

        typeSearchField.RegisterValueChangedCallback(OnTypeSearched);
    }

    private void ListAllTypes(string[] ignoredNamespaces)
    {
        typeList.Clear();

        types = new List<ScriptableObject>();

        string[] scriptableObjectFiles = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);

        foreach (string file in scriptableObjectFiles)
        {
            string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
            ScriptableObject scriptableObject = (ScriptableObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(ScriptableObject));

            if (scriptableObject != null)
            {
                string typeName = scriptableObject.GetType().Name;
                string namespaceName = scriptableObject.GetType().Namespace;

                if (types.Any(x => x.GetType().Name.Equals(typeName)) ||
                   (!string.IsNullOrEmpty(namespaceName) && ignoredNamespaces.Any(x => x.Contains(namespaceName) || namespaceName.Contains(x)))) continue;

                Button button = CreateTypeButton(typeName, scriptableObject);

                typeList.Add(button);
                types.Add(scriptableObject);
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

        List<ScriptableObject> matchingScriptableObjects = types.Where(x => x.GetType().Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();

        typeList.Clear();
        foreach (ScriptableObject scriptableObj in matchingScriptableObjects)
        {
            Button button = CreateTypeButton(scriptableObj.GetType().Name, scriptableObj);

            typeList.Add(button);
        }
    }
}
