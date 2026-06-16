using UnityEngine;
using UnityEditor;

public class ForceSpriteImportV2 : EditorWindow
{
    [MenuItem("Tools/Force Apply AI Backups")]
    public static void Apply()
    {
        string[] spritePaths = {
            "Assets/Sprites/AI_Assets/basic_tower_1781625173629.png",
            "Assets/Sprites/AI_Assets/ballista_tower_1781625195365.png",
            "Assets/Sprites/AI_Assets/catapult_tower_1781625185615.png",
            "Assets/Sprites/AI_Assets/enemy_goblin_1781625206931.png",
            "Assets/Sprites/AI_Assets/grass_tile_1781625147606.png",
            "Assets/Sprites/AI_Assets/road_tile_1781625158050.png"
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

        // Apply Towers
        ApplyTowerSprite("Assets/Prefabs/Tower_Basic.prefab", "Assets/Sprites/AI_Assets/basic_tower_1781625173629.png");
        ApplyTowerSprite("Assets/Prefabs/Tower_Ballista.prefab", "Assets/Sprites/AI_Assets/ballista_tower_1781625195365.png");
        ApplyTowerSprite("Assets/Prefabs/Tower_Catapult.prefab", "Assets/Sprites/AI_Assets/catapult_tower_1781625185615.png");
        ApplyTowerSprite("Assets/Prefabs/Tower_Barracks.prefab", "Assets/Sprites/AI_Assets/basic_tower_1781625173629.png");

        // Apply Enemy Goblin
        ApplyMonsterSprite("Assets/Prefabs/Goblin_Warrior.prefab", "Assets/Sprites/AI_Assets/enemy_goblin_1781625206931.png");

        AssetDatabase.SaveAssets();
        Debug.Log("Force Sprite Update V2 Complete!");
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
    }

    private static void ApplyMonsterSprite(string prefabPath, string spritePath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

        if (prefab != null && sprite != null)
        {
            SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = sprite;
                prefab.transform.localScale = new Vector3(1f, 1f, 1f); // Reset scale
                EditorUtility.SetDirty(prefab);
            }
        }
    }
}
