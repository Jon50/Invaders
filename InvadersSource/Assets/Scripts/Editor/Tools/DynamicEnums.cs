#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DynamicEnums : EditorWindow
{
    [SerializeField] private string enumName = "";
    [SerializeField] private string namespaceName = "";
    [SerializeField] private string folderPath = "Assets";
    [SerializeField] private string scenesPath = "Assets/Scenes";

    //*Dynamic Enum
    private SerializedObject thisFile = default;
    private SerializedProperty EnumName, NameSpace, FolderPath, ScenesPath;
    private Transform targetTransform = default;
    private string[] parameters = default;
    private int[] paramHash = default;
    private float minWindowSizeX = 0f;
    private float maxWindowSizeX = 0f;

    private static DynamicEnums window = default;

    //*Tags
    private SerializedObject tagManager = default;
    private SerializedProperty TagsArray = default;
    private string[] myTags = default;

    //*Layers
    private SerializedProperty LayersArray = default;
    private string[] myLayers = default;
    private int[] myLayersValue = default;

    //*Levels
    private FileInfo[] levelFiles = default;
    private string[] myLevels = default;
    private int[] buildIndex = default;


    [MenuItem("Tools/DynamicEnums")]
    private static void Awake()
    {
        window = GetWindow<DynamicEnums>();
        window.Show();
        window.autoRepaintOnSceneChange = true;
    }

    private void OnEnable()
    {
        thisFile = new SerializedObject(this);
        EnumName = thisFile.FindProperty(nameof(enumName));
        NameSpace = thisFile.FindProperty(nameof(namespaceName));
        FolderPath = thisFile.FindProperty(nameof(folderPath));
        ScenesPath = thisFile.FindProperty(nameof(scenesPath));

        tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset"));
        TagsArray = tagManager.FindProperty("tags");
        LayersArray = tagManager.FindProperty("layers");

        if (window == null) return;

        minWindowSizeX = window.minSize.x;
        maxWindowSizeX = window.maxSize.x;
    }

    private Vector2 scroll = default;
    private bool showTags = false;
    private bool showLayers = false;
    private bool showLevels = false;
    private float totalWidth = default;

    private void OnGUI()
    {
        scroll = GUILayout.BeginScrollView(scroll);

        // GenerateAnimatorLayout();

        // GUILayout.Space(30);

        if (showTags = GUILayout.Toggle(showTags, "Generate Enum Tags", "Button"))
        {
            GenerateTagsLayout();
            Seperator();
        }

        if (showLayers = GUILayout.Toggle(showLayers, "Generate Enum Layers", "Button"))
        {
            GenerateLayersLayout();
            Seperator();
        }

        if (showLevels = GUILayout.Toggle(showLevels, "Generate Enum Levels", "Button"))
        {
            GenerateLevelsLayout();
            Seperator();
        }

        GUILayout.EndScrollView();
    }

    private void Seperator()
    {
        GUILayout.HorizontalSlider(totalWidth, minWindowSizeX, maxWindowSizeX);
        GUILayout.HorizontalSlider(totalWidth, minWindowSizeX, maxWindowSizeX);
        GUILayout.HorizontalSlider(totalWidth, minWindowSizeX, maxWindowSizeX);
        GUILayout.Space(15);
    }

    /*****************
    **                        **
    **  GUI LAYOUT  **
    **                        **
    *****************/
    #region GUI_LAYOUT

    /**********************
    **  ANIMATOR lAYOUT  **
    **********************/
    // private void GenerateAnimatorLayout()
    // {
    //     GUILayout.Space(10);
    //     GUILayout.Label("Generate Enum - Animator Parameters", GetLabelStyle());

    //     targetTransform = Selection.activeTransform;

    //     if (targetTransform) { GUI.enabled = targetTransform.GetComponent<Animator>(); }
    //     else { GUI.enabled = false; }

    //     GUILayout.BeginHorizontal();

    //     if (GUILayout.Button(new GUIContent { text = "Generate", tooltip = "Select a GameObject in the Hierarchy and make sure it has an Animator component attached." }))
    //     {
    //         PrepareParameters();
    //         GenerateAnimatorParametersEnum();
    //     }

    //     GUILayout.EndHorizontal();

    //     GUI.enabled = true;

    //     PropertyFields();
    // }

    /******************
    **  TAGS lAYOUT  **
    ******************/
    private void GenerateTagsLayout()
    {
        EditorGUILayout.Separator();

        GUILayout.Label("Generate Enum - Tags", GetLabelStyle());

        EditorGUILayout.Separator();

        PropertyFields();

        EditorGUILayout.Separator();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent { text = "Generate" }))
        {
            PrepareTags();
            GenerateTagsEnum();
        }

        GUILayout.EndHorizontal();
    }

    /*******************
    ** LAYERS lAYOUT  **
    *******************/
    private void GenerateLayersLayout()
    {
        EditorGUILayout.Separator();

        GUILayout.Label("Generate Enum - Layers", GetLabelStyle());

        EditorGUILayout.Separator();

        PropertyFields();

        EditorGUILayout.Separator();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent { text = "Generate" }))
        {
            PrepareLayers();
            GenerateLayersEnum();
        }

        GUILayout.EndHorizontal();
    }

    /********************
    **  LEVELS lAYOUT  **
    ********************/
    private void GenerateLevelsLayout()
    {
        EditorGUILayout.Separator();

        GUILayout.Label("Generate Enum - Levels", GetLabelStyle());

        EditorGUILayout.Separator();

        PropertyFields();

        EditorGUILayout.PropertyField(ScenesPath, new GUIContent { text = "Scenes Path", tooltip = "Your Scenes path" });

        EditorGUILayout.Separator();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent { text = "Generate" }))
        {
            if (PrepareLevels())
                GenerateLevelsEnum();
        }

        GUILayout.EndHorizontal();
    }

    /*************/

    private GUIStyle GetLabelStyle()
    {
        var labelStyle = new GUIStyle();
        labelStyle.fontSize = 12;
        labelStyle.normal.textColor = Color.black;
        labelStyle.fontStyle = FontStyle.Bold;

        return labelStyle;
    }

    private void PropertyFields()
    {
        EditorGUILayout.PropertyField(EnumName, new GUIContent { text = "Enum Name" });
        EditorGUILayout.PropertyField(NameSpace, new GUIContent { text = "Namespace" });
        EditorGUILayout.PropertyField(FolderPath, new GUIContent
        {
            text = "Folder Path",
            tooltip = "Default path is the 'Assets' folder." + "\r\n" +
                        "Provide a path by typing the folder's name." + "\r\n" +
                        "Subfolders should be followed by '/'." + "\r\n" +
                        "Example: Folder/Subfolder"
        });

        thisFile.ApplyModifiedProperties();
    }
    #endregion

    /*********************
    **                               **
    **  INITIALIZATION   **
    **                               **
    *********************/

    #region INITIALIZATION

    /**********************
    **  PARAMETERS INIT  **
    **********************/

    private void PrepareParameters()
    {
        if (targetTransform == null || targetTransform.GetComponent<Animator>() == null) { return; }

        var animator = targetTransform.GetComponent<Animator>();
        var paramCount = animator.parameterCount;

        parameters = new string[paramCount];
        paramHash = new int[paramCount];

        for (int i = 0; i < paramCount; i++)
        {
            parameters[i] = animator.GetParameter(i).name;
            paramHash[i] = animator.GetParameter(i).nameHash;
        }
    }

    /****************
    **  TAGS INIT  **
    ****************/
    private void PrepareTags()
    {
        var tagsCount = TagsArray.arraySize;

        myTags = new string[tagsCount];

        for (int i = 0; i < tagsCount; i++)
        {
            myTags[i] = TagsArray.GetArrayElementAtIndex(i).stringValue;
        }
    }

    /******************
    **  LAYERS INIT  **
    ******************/
    private void PrepareLayers()
    {
        var layersCount = LayersArray.arraySize;

        myLayers = new string[layersCount];
        myLayersValue = new int[layersCount];

        for (int i = 0; i < layersCount; i++)
        {
            if (string.IsNullOrEmpty(LayersArray.GetArrayElementAtIndex(i).stringValue)) { continue; }

            myLayers[i] = LayersArray.GetArrayElementAtIndex(i).stringValue;
            myLayersValue[i] = i;
        }
    }

    /******************
    **  LEVELS INIT  **
    ******************/
    private bool PrepareLevels()
    {
        try
        {
            levelFiles = new DirectoryInfo(ScenesPath.stringValue).GetFiles("*.unity");
        }
        catch (Exception)
        {
            Debug.LogError("Path not found or folder has no scenes.");
            return false;
        }

        var levelCount = levelFiles.Length;

        myLevels = new string[levelCount];
        buildIndex = new int[levelCount];

        for (int i = 0; i < levelCount; i++)
        {
            string levelName = levelFiles[i].Name.Substring(0, levelFiles[i].Name.Length - Path.GetExtension(levelFiles[i].Name).Length);

            myLevels[i] = levelName;
            buildIndex[i] = SceneUtility.GetBuildIndexByScenePath(ScenesPath.stringValue + "/" + levelFiles[i].Name);
        }

        return true;
    }
    #endregion

    /**********************
    **                   **
    **  ENUM GENERATION  **
    **                   **
    **********************/
    #region ENUM_GENERATION

    /**********************
    **  PARAMETERS ENUM  **
    **********************/
    // private void GenerateAnimatorParametersEnum()
    // {
    //     if (string.IsNullOrWhiteSpace(EnumName.stringValue)) { Debug.LogError("Enum Name is empty or invalid."); return; }

    //     EnumName.stringValue = EnumName.stringValue.Replace(" ", string.Empty);
    //     NameSpace.stringValue = NameSpace.stringValue.Replace(" ", string.Empty);

    //     var hasNamespace = !string.IsNullOrWhiteSpace(NameSpace.stringValue);
    //     var namespaceFormat = hasNamespace ? "\t" : "";

    //     StringBuilder str = new StringBuilder();

    //     if (hasNamespace)
    //     {
    //         str.AppendFormat("namespace {0}\r\n", NameSpace.stringValue);
    //         str.Append("{\r\n");
    //     }

    //     str.AppendFormat("{0}public enum {1}\r\n", namespaceFormat, EnumName.stringValue);
    //     str.AppendFormat("{0}", namespaceFormat);
    //     str.Append("{\r\n");

    //     for (int i = 0; i < parameters.Length; i++)
    //     {
    //         if (string.IsNullOrEmpty(parameters[i])) { continue; }
    //         str.AppendFormat("{0}\t{1} = {2},\r\n", namespaceFormat, parameters[i], paramHash[i]);
    //     }

    //     str.AppendFormat("{0}", namespaceFormat);
    //     str.Append("}\r\n");

    //     if (hasNamespace)
    //     {
    //         str.Append("}");
    //     }

    //     string defaultPath = string.Format(@"Assets\{0}.cs", EnumName.stringValue);
    //     string customPath = string.Format(FolderPath.stringValue.Contains("Assets/") ? @"{0}/{1}.cs" : @"Assets/{0}/{1}.cs", FolderPath.stringValue, EnumName.stringValue);

    //     string pathString = string.IsNullOrWhiteSpace(FolderPath.stringValue) ? defaultPath : customPath;

    //     string path = AssetDatabase.GetAssetPath(thisFile.targetObject);
    //     path = path.Substring(0, path.Length - Path.GetExtension(path).Length) + pathString;
    //     File.WriteAllText(path, str.ToString());
    //     AssetDatabase.ImportAsset(path);
    // }

    /****************
    **  TAGS ENUM  **
    ****************/
    private void GenerateTagsEnum()
    {
        if (string.IsNullOrWhiteSpace(EnumName.stringValue)) { Debug.LogError("Enum Name is empty or invalid."); return; }

        EnumName.stringValue = EnumName.stringValue.Replace(" ", string.Empty);
        NameSpace.stringValue = NameSpace.stringValue.Replace(" ", string.Empty);

        var hasNamespace = !string.IsNullOrWhiteSpace(NameSpace.stringValue);
        var namespaceFormat = hasNamespace ? "\t" : "";

        StringBuilder str = new StringBuilder();

        if (hasNamespace)
        {
            str.AppendFormat("namespace {0}\r\n", NameSpace.stringValue);
            str.Append("{\r\n");
        }

        str.AppendFormat("{0}public enum {1}\r\n", namespaceFormat, EnumName.stringValue);
        str.AppendFormat("{0}", namespaceFormat);
        str.Append("{\r\n");

        str.AppendFormat("{0}\tUntagged,\r\n", namespaceFormat);
        str.AppendFormat("{0}\tRespawn,\r\n", namespaceFormat);
        str.AppendFormat("{0}\tFinish,\r\n", namespaceFormat);
        str.AppendFormat("{0}\tEditorOnly,\r\n", namespaceFormat);
        str.AppendFormat("{0}\tMainCamera,\r\n", namespaceFormat);
        str.AppendFormat("{0}\tPlayer,\r\n", namespaceFormat);
        str.AppendFormat("{0}\tGameController,\r\n", namespaceFormat);

        for (int i = 0; i < myTags.Length; i++)
        {
            if (string.IsNullOrEmpty(myTags[i])) { continue; }
            str.AppendFormat("{0}\t{1},\r\n", namespaceFormat, myTags[i]);
        }

        str.AppendFormat("{0}", namespaceFormat);
        str.Append("}\r\n");

        if (hasNamespace)
        {
            str.Append("}");
        }

        string defaultPath = string.Format(@"Assets/{0}.cs", EnumName.stringValue);
        string customPath = string.Format(FolderPath.stringValue.Contains("Assets/") ? @"{0}/{1}.cs" : @"Assets/{0}/{1}.cs", FolderPath.stringValue, EnumName.stringValue);

        string pathString = string.IsNullOrWhiteSpace(FolderPath.stringValue) ? defaultPath : customPath;

        string path = AssetDatabase.GetAssetPath(thisFile.targetObject);
        path = path.Substring(0, path.Length - Path.GetExtension(path).Length) + pathString;
        File.WriteAllText(path, str.ToString());
        AssetDatabase.ImportAsset(path);
    }

    /******************
    **  LAYERS ENUM  **
    ******************/
    private void GenerateLayersEnum()
    {
        if (string.IsNullOrWhiteSpace(EnumName.stringValue)) { Debug.LogError("Enum Name is empty or invalid."); return; }

        EnumName.stringValue = EnumName.stringValue.Replace(" ", string.Empty);
        NameSpace.stringValue = NameSpace.stringValue.Replace(" ", string.Empty);

        var hasNamespace = !string.IsNullOrWhiteSpace(NameSpace.stringValue);
        var namespaceFormat = hasNamespace ? "\t" : "";

        StringBuilder str = new StringBuilder();

        if (hasNamespace)
        {
            str.AppendFormat("namespace {0}\r\n", NameSpace.stringValue);
            str.Append("{\r\n");
        }

        str.AppendFormat("{0}public enum {1}\r\n", namespaceFormat, EnumName.stringValue);
        str.AppendFormat("{0}", namespaceFormat);
        str.Append("{\r\n");

        for (int i = 0; i < myLayers.Length; i++)
        {
            if (string.IsNullOrEmpty(myLayers[i])) { continue; }
            str.AppendFormat("{0}\t{1} = {2},\r\n", namespaceFormat, myLayers[i].Replace(" ", ""), myLayersValue[i]);
        }

        str.AppendFormat("{0}", namespaceFormat);
        str.Append("}\r\n");

        if (hasNamespace)
        {
            str.Append("}");
        }

        string defaultPath = string.Format(@"Assets\{0}.cs", EnumName.stringValue);
        string customPath = string.Format(FolderPath.stringValue.Contains("Assets/") ? @"{0}/{1}.cs" : @"Assets/{0}/{1}.cs", FolderPath.stringValue, EnumName.stringValue);

        string pathString = string.IsNullOrWhiteSpace(FolderPath.stringValue) ? defaultPath : customPath;

        string path = AssetDatabase.GetAssetPath(thisFile.targetObject);
        path = path.Substring(0, path.Length - Path.GetExtension(path).Length) + pathString;
        File.WriteAllText(path, str.ToString());
        AssetDatabase.ImportAsset(path);
    }

    /******************
    **  LEVELS ENUM  **
    ******************/
    private void GenerateLevelsEnum()
    {
        if (string.IsNullOrWhiteSpace(EnumName.stringValue)) { Debug.LogError("Enum Name is empty or invalid."); return; }

        EnumName.stringValue = EnumName.stringValue.Replace(" ", string.Empty);
        NameSpace.stringValue = NameSpace.stringValue.Replace(" ", string.Empty);

        var hasNamespace = !string.IsNullOrWhiteSpace(NameSpace.stringValue);
        var namespaceFormat = hasNamespace ? "\t" : "";

        StringBuilder str = new StringBuilder();

        if (hasNamespace)
        {
            str.AppendFormat("namespace {0}\r\n", NameSpace.stringValue);
            str.Append("{\r\n");
        }

        str.AppendFormat("{0}public enum {1}\r\n", namespaceFormat, EnumName.stringValue);
        str.AppendFormat("{0}", namespaceFormat);
        str.Append("{\r\n");

        for (int i = 0; i < myLevels.Length; i++)
        {
            if (string.IsNullOrEmpty(myLevels[i])) { continue; }
            str.AppendFormat("{0}\t{1} = {2},\r\n", namespaceFormat, myLevels[i], buildIndex[i]);
        }

        str.AppendFormat("{0}", namespaceFormat);
        str.Append("}\r\n");

        if (hasNamespace)
        {
            str.Append("}");
        }

        string defaultPath = string.Format(@"Assets\{0}.cs", EnumName.stringValue);
        string customPath = string.Format(FolderPath.stringValue.Contains("Assets/") ? @"{0}/{1}.cs" : @"Assets/{0}/{1}.cs", FolderPath.stringValue, EnumName.stringValue);

        string pathString = string.IsNullOrWhiteSpace(FolderPath.stringValue) ? defaultPath : customPath;

        string path = AssetDatabase.GetAssetPath(thisFile.targetObject);
        path = path.Substring(0, path.Length - Path.GetExtension(path).Length) + pathString;
        File.WriteAllText(path, str.ToString());
        AssetDatabase.ImportAsset(path);
    }
    #endregion
}
#endif
