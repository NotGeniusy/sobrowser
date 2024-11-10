#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SOBrowserEditor : EditorWindow
{
    private SOBrowserMain mainPage;

    [MenuItem("Tools/ScriptableObject Browser")]
    public static void OpenBrowser()
    {
        EditorWindow window = GetWindow(typeof(SOBrowserEditor));
        window.titleContent = new GUIContent("ScriptableObject Browser");
        window.minSize = new Vector2(700, 400);
        window.Show();
    }

    public void CreateGUI()
    {
        OpenHomePage();
    }

    private void OnGUI()
    {
        if (mainPage != null && !mainPage.isGUIPainted)
        {
            mainPage.PaintMainGUI();
        }
    }

    private void OpenHomePage()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.enesborekci.sobrowser/Editor/Resources/UI Documents/Home.uxml");
        VisualElement tree = visualTree.Instantiate();
        rootVisualElement.Add(tree);

        string[] ignoredNamespaces = SOBrowserConfigEditor.ReadConfig();

        SOBrowserHome homePage = SOBrowserHome.CreateInstance(rootVisualElement, ignoredNamespaces);
        homePage.OnTypeSelected += OpenScriptableListPage;
    }

    private void OpenScriptableListPage(ScriptableObject scriptableObj)
    {
        rootVisualElement.Clear();

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.enesborekci.sobrowser/Editor/Resources/UI Documents/Browser.uxml");
        VisualElement tree = visualTree.Instantiate();
        rootVisualElement.Add(tree);

        mainPage = SOBrowserMain.CreateInstance(rootVisualElement, scriptableObj);

        Button returnButton = rootVisualElement.Q<Button>("ReturnButton");
        returnButton.clicked += () =>
        {
            ReturnToHome();
        };

        Button createObjectButton = rootVisualElement.Q<Button>("CreateButton");
        createObjectButton.clicked += () =>
        {
            SOBrowserCreateObjectEditor.OpenPopup(scriptableObj);
        };

        SOBrowserCreateObjectEditor.OnScriptableObjectCreated += mainPage.OnScriptableObjectCreated;
    }

    private void ReturnToHome()
    {
        rootVisualElement.Clear();

        OpenHomePage();
    }
}
#endif