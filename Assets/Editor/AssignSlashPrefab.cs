using UnityEngine;
using UnityEditor;

public class AssignSlashPrefab
{
    [MenuItem("Tools/Assign Slash Prefab")]
    public static void Assign()
    {
        GameObject soldierPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Soldier.prefab");
        GameObject slashPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Slash_VFX.prefab");
        
        if (soldierPrefab != null && slashPrefab != null)
        {
            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(soldierPrefab);
            MilitiaUnit2D militia = inst.GetComponent<MilitiaUnit2D>();
            
            SerializedObject so = new SerializedObject(militia);
            so.FindProperty("slashPrefab").objectReferenceValue = slashPrefab;
            so.ApplyModifiedProperties();
            
            PrefabUtility.SaveAsPrefabAssetAndConnect(inst, "Assets/Prefabs/Soldier.prefab", InteractionMode.AutomatedAction);
            Object.DestroyImmediate(inst);
        }
        
        Debug.Log("Assigned Slash Prefab!");
    }
}
