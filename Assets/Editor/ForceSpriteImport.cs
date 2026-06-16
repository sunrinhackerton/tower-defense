using UnityEngine;
using UnityEditor;

public class ForceSpriteImport : EditorWindow
{
    [MenuItem("Tools/Force Apply Update 9")]
    public static void Apply()
    {
        string[] spritePaths = {
            "Assets/Sprites/TowerHead_Basic.png",
            "Assets/Sprites/TowerHead_Ballista.png",
            "Assets/Sprites/TowerHead_Catapult.png"
        };

        foreach(string path in spritePaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.SaveAndReimport();
                    Debug.Log("Converted to Sprite: " + path);
                }
            }
            else
            {
                Debug.LogError("Could not find texture at: " + path);
            }
        }

        // Apply
        ApplyTowerSprite("Assets/Prefabs/Tower_Basic.prefab", "Assets/Sprites/TowerHead_Basic.png");
        ApplyTowerSprite("Assets/Prefabs/Tower_Ballista.prefab", "Assets/Sprites/TowerHead_Ballista.png");
        ApplyTowerSprite("Assets/Prefabs/Tower_Catapult.prefab", "Assets/Sprites/TowerHead_Catapult.png");
        ApplyTowerSprite("Assets/Prefabs/Tower_Barracks.prefab", "Assets/Sprites/TowerHead_Basic.png");
        
        AssetDatabase.SaveAssets();
        Debug.Log("Force Sprite Update Complete!");
    }

    private static void ApplyTowerSprite(string prefabPath, string spritePath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

        if (prefab != null && sprite != null)
        {
            Transform head = prefab.transform.Find("Head");
            SpriteRenderer sr = head != null ? head.GetComponent<SpriteRenderer>() : prefab.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.sprite = sprite;
                if (head != null)
                    head.localScale = new Vector3(0.6f, 0.6f, 1f); 
                else
                    prefab.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                    
                EditorUtility.SetDirty(prefab);
                Debug.Log($"SUCCESS: Applied {sprite.name} to {prefab.name}");
            }
        }
        else
        {
            Debug.LogError($"FAIL: Prefab or Sprite is null. Prefab: {prefabPath}, Sprite: {spritePath}");
        }
    }
}
