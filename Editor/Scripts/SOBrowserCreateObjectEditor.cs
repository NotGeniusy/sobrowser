using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SOBrowserCreateObjectEditor : EditorWindow
{
    private static EditorWindow window;

    private static ScriptableObject selectedScriptableObject;
    private static string path = "";

    private string fileName;

    public static System.Action<ScriptableObject> OnScriptableObjectCreated;

    public static void OpenPopup(ScriptableObject scriptableObject)
    {
        if(window != null) window.Close();

        selectedScriptableObject = scriptableObject;
        path = "Assets";

        window = GetWindow(typeof(SOBrowserCreateObjectEditor));
        window.titleContent = new GUIContent("Create new ScriptableObject");
        window.minSize = new Vector2(500, 120);
        window.maxSize = window.minSize;
        window.Show();
    }

    public void CreateGUI()
    {
        var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.enesborekci.sobrowser/Editor/Resources/UI Documents/CreateObject.uxml");
        VisualElement page = tree.Instantiate();
        rootVisualElement.Add(page);

        TextField nameField = rootVisualElement.Q<TextField>("NameField");
        nameField.SetValueWithoutNotify(selectedScriptableObject.GetType().Name);

        nameField.RegisterValueChangedCallback(OnNameChanged);
        fileName = selectedScriptableObject.GetType().Name;

        Label directoryLabel = rootVisualElement.Q<Label>("DirectoryText");
        directoryLabel.text = path;

        Button directoryButton = rootVisualElement.Q<Button>("SelectDirectoryButton");
        directoryButton.clicked += () =>
        {
            string newPath = EditorUtility.OpenFolderPanel("Select folder", path, "");
            
            if (!string.IsNullOrEmpty(newPath))
            {
                if (!newPath.Contains("Assets"))
                {
                    Debug.LogError("You can't create objects outside the Assets folder!");
                }
                else
                {
                    path = "Assets" + newPath.Split("Assets")[1];

                    directoryLabel.text = path;
                }
            }
        };

        Button createButton = rootVisualElement.Q<Button>("CreateButton");
        createButton.clicked += CreateScriptableObject;
    }

    private void CreateScriptableObject()
    {
        var createdObj = Instantiate(selectedScriptableObject);

        int index = -1;
        string[] nameArray = fileName.Split(" ");

        string nameBase = string.Copy(fileName);

        if (nameArray.Length > 1 && int.TryParse(nameArray[nameArray.Length - 1], out int startIndex))
        {
            nameBase = "";
            for(var n = 0; n < nameArray.Length - 1; n++)
            {
                nameBase += nameArray[n];

                if (n < nameArray.Length - 2)
                    nameBase += " ";
            }

            index = startIndex;
        }

        while (AssetDatabase.LoadAssetAtPath<ScriptableObject>(path + $"/{fileName}.asset") != null)
        {
            index++;
            fileName = nameBase + " " + index;
        }

        AssetDatabase.CreateAsset(createdObj, path+$"/{fileName}.asset");

        fileName = nameBase;

        OnScriptableObjectCreated?.Invoke(createdObj);
    }

    private void OnNameChanged(ChangeEvent<string> evt)
    {
        string newName = evt.newValue;

        fileName = newName;
    }
}
