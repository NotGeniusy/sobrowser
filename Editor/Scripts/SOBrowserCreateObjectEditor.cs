#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SOBrowserCreateObjectEditor : EditorWindow
{
    private static EditorWindow _window;

    private static ScriptableObject _selectedScriptableObject;
    private static string _path = "";

    private string _fileName;

    public static System.Action<ScriptableObject> OnScriptableObjectCreated;

    public static void OpenPopup(ScriptableObject scriptableObject)
    {
        if(_window != null) _window.Close();

        _selectedScriptableObject = scriptableObject;
        _path = "Assets";

        _window = GetWindow(typeof(SOBrowserCreateObjectEditor));
        _window.titleContent = new GUIContent("Create new ScriptableObject");
        _window.minSize = new Vector2(500, 120);
        _window.maxSize = _window.minSize;
        _window.Show();
    }

    public void CreateGUI()
    {
        var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.enesborekci.sobrowser/Editor/Resources/UI Documents/CreateObject.uxml");
        VisualElement page = tree.Instantiate();
        rootVisualElement.Add(page);

        TextField nameField = rootVisualElement.Q<TextField>("NameField");
        nameField.SetValueWithoutNotify(_selectedScriptableObject.GetType().Name);

        nameField.RegisterValueChangedCallback(OnNameChanged);
        _fileName = _selectedScriptableObject.GetType().Name;

        Label directoryLabel = rootVisualElement.Q<Label>("DirectoryText");
        directoryLabel.text = _path;

        Button directoryButton = rootVisualElement.Q<Button>("SelectDirectoryButton");
        directoryButton.clicked += () =>
        {
            string newPath = EditorUtility.OpenFolderPanel("Select folder", _path, "");
            
            if (!string.IsNullOrEmpty(newPath))
            {
                if (!newPath.Contains("Assets"))
                {
                    Debug.LogError("You can't create objects outside the Assets folder!");
                }
                else
                {
                    _path = "Assets" + newPath.Split("Assets")[1];

                    directoryLabel.text = _path;
                }
            }
        };

        Button createButton = rootVisualElement.Q<Button>("CreateButton");
        createButton.clicked += CreateScriptableObject;
    }

    private void CreateScriptableObject()
    {
        var createdObj = Instantiate(_selectedScriptableObject);

        int index = -1;
        string[] nameArray = _fileName.Split(" ");

        string nameBase = string.Copy(_fileName);

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

        while (AssetDatabase.LoadAssetAtPath<ScriptableObject>(_path + $"/{_fileName}.asset") != null)
        {
            index++;
            _fileName = nameBase + " " + index;
        }

        AssetDatabase.CreateAsset(createdObj, _path+$"/{_fileName}.asset");

        _fileName = nameBase;

        OnScriptableObjectCreated?.Invoke(createdObj);
    }

    private void OnNameChanged(ChangeEvent<string> evt)
    {
        string newName = evt.newValue;

        _fileName = newName;
    }
}
#endif