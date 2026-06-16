using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ForceSaveScene : EditorWindow
{
    [MenuItem("Tools/Force Save Everything")]
    public static void SaveAll()
    {
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        Debug.Log("All Scenes and Assets Saved!");
    }
}
