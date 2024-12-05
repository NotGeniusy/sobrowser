#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SOBrowserConfigEditor : EditorWindow
{
    private static EditorWindow _window;

    private const string CONFIGPATH = "Packages/com.enesborekci.sobrowser/Editor/Config.txt";

    public static Action OnIgnoredNamespacesChanged;

    public static void OpenConfigWindow()
    {
        _window = GetWindow(typeof(SOBrowserConfigEditor));
        _window.titleContent = new GUIContent("Edit Config");

        _window.minSize = new Vector2(400, 300);
        _window.maxSize = _window.minSize;

        _window.Show();
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
        StreamReader reader = new StreamReader(CONFIGPATH);

        string[] ignoredNamespaces = reader.ReadToEnd().Split("\n");

        reader.Close();

        return ignoredNamespaces;
    }

    private void WriteConfig(string ignoredNamespaces)
    {
        StreamWriter writer = new StreamWriter(CONFIGPATH, false);
        writer.Write(ignoredNamespaces);
        writer.Close();

        AssetDatabase.ImportAsset(CONFIGPATH);

        OnIgnoredNamespacesChanged?.Invoke();

        _window.Close();
    }
}
#endif