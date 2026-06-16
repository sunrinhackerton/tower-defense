using UnityEngine;
using UnityEditor;

public class AssignAssets : EditorWindow
{
    [MenuItem("Tools/Assign Flight Plan 7.0 Assets to Prefabs")]
    public static void AssignAll()
    {
        AssignSpriteToPrefab("Assets/Prefabs/Tower_Basic.prefab", "Assets/Sprites/TowerBase_Basic.png", "Assets/Sprites/TowerHead_Basic.png");
        AssignSpriteToPrefab("Assets/Prefabs/Tower_Catapult.prefab", "Assets/Sprites/TowerBase_Catapult.png", "Assets/Sprites/TowerHead_Catapult.png");
        AssignSpriteToPrefab("Assets/Prefabs/Tower_Ballista.prefab", "Assets/Sprites/TowerBase_Ballista.png", "Assets/Sprites/TowerHead_Ballista.png");
        
        // BuildSite2D 업데이트
        AssignSingleSpriteToPrefab("Assets/Prefabs/BuildSite.prefab", "Assets/Sprites/TowerBase_Basic.png"); // 혹은 전용 BuildSiteSprite

        Debug.Log("✅ 모든 텍스처가 프리팹에 할당되었습니다!");
    }

    private static void AssignSpriteToPrefab(string prefabPath, string baseSpritePath, string headSpritePath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) return;

        Sprite baseSprite = AssetDatabase.LoadAssetAtPath<Sprite>(baseSpritePath);
        Sprite headSprite = AssetDatabase.LoadAssetAtPath<Sprite>(headSpritePath);

        SpriteRenderer baseRenderer = prefab.GetComponent<SpriteRenderer>();
        if (baseRenderer != null && baseSprite != null) baseRenderer.sprite = baseSprite;

        Transform headTransform = prefab.transform.Find("Head"); // 혹은 포탑 머리 이름
        if (headTransform != null)
        {
            SpriteRenderer headRenderer = headTransform.GetComponent<SpriteRenderer>();
            if (headRenderer != null && headSprite != null) headRenderer.sprite = headSprite;
        }

        EditorUtility.SetDirty(prefab);
        PrefabUtility.SavePrefabAsset(prefab);
    }

    private static void AssignSingleSpriteToPrefab(string prefabPath, string spritePath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) return;

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
        if (renderer != null && sprite != null) renderer.sprite = sprite;

        EditorUtility.SetDirty(prefab);
        PrefabUtility.SavePrefabAsset(prefab);
    }
}
