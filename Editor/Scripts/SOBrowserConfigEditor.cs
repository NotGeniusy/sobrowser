#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SOBrowserConfigEditor : EditorWindow
{
    private static EditorWindow window;

    private const string configPath = "Packages/com.enesborekci.sobrowser/Editor/Config.txt";

    public static Action OnIgnoredNamespacesChanged;

    public static void OpenConfigWindow()
    {
        window = GetWindow(typeof(SOBrowserConfigEditor));
        window.titleContent = new GUIContent("Edit Config");

        window.minSize = new Vector2(400, 300);
        window.maxSize = window.minSize;

        window.Show();
    }

    private void CreateGUI()
    {
        var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.enesborekci.sobrowser/Editor/Resources/UI Documents/Config.uxml");
        VisualElement page = tree.Instantiate();
        rootVisualElement.Add(page);

        TextField namespaceField = rootVisualElement.Q<TextField>("NamespaceField");
        namespaceField.SetValueWithoutNotify("");

        foreach(string _namespace in ReadConfig())
        {
            string text = namespaceField.text == "" ? _namespace : namespaceField.text + "\n" + _namespace;
            namespaceField.SetValueWithoutNotify(text);
        }

        Button saveButton = rootVisualElement.Q<Button>("SaveButton");
        saveButton.clicked += () =>
        {
            WriteConfig(namespaceField.text);
        };
    }

    public static string[] ReadConfig()
    {
        StreamReader reader = new StreamReader(configPath);

        string[] ignoredNamespaces = reader.ReadToEnd().Split("\n");

        reader.Close();

        return ignoredNamespaces;
    }

    private void WriteConfig(string ignoredNamespaces)
    {
        StreamWriter writer = new StreamWriter(configPath, false);
        writer.Write(ignoredNamespaces);
        writer.Close();

        AssetDatabase.ImportAsset(configPath);

        OnIgnoredNamespacesChanged?.Invoke();

        window.Close();
    }
}
#endif