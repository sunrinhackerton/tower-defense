using UnityEngine;
using UnityEditor;

public class ApplyUpdate9 : EditorWindow
{
    [MenuItem("Tools/Apply Flight Plan 9.0 Prefabs")]
    public static void Apply()
    {
        // 1. Remove HealthBar2D from Monsters
        string[] monsterPaths = new string[]
        {
            "Assets/Prefabs/Goblin_Warrior.prefab",
            "Assets/Prefabs/Gargoyle.prefab",
            "Assets/Prefabs/Iron_Knight.prefab",
            "Assets/Prefabs/Monster.prefab",
            "Assets/Prefabs/Enemy_Basic.prefab"
        };

        foreach (string path in monsterPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                Component oldHealthBar = prefab.GetComponent("HealthBar2D");
                if (oldHealthBar != null)
                {
                    DestroyImmediate(oldHealthBar, true);
                    EditorUtility.SetDirty(prefab);
                    Debug.Log($"Removed HealthBar2D from {prefab.name}");
                }
            }
        }

        // 2. Apply High-Quality AI Tower Assets
        ApplyTowerSprite("Assets/Prefabs/Tower_Basic.prefab", "Assets/Sprites/TowerHead_Basic.png");
        ApplyTowerSprite("Assets/Prefabs/Tower_Ballista.prefab", "Assets/Sprites/TowerHead_Ballista.png");
        ApplyTowerSprite("Assets/Prefabs/Tower_Catapult.prefab", "Assets/Sprites/TowerHead_Catapult.png");
        ApplyTowerSprite("Assets/Prefabs/Tower_Barracks.prefab", "Assets/Sprites/TowerHead_Basic.png"); // Re-use basic for barracks for now

        AssetDatabase.SaveAssets();
        Debug.Log("✅ Flight Plan 9.0 Prefabs Updated!");
    }

    private static void ApplyTowerSprite(string prefabPath, string spritePath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

        if (prefab != null && sprite != null)
        {
            // Usually the head is a child named "Head", or it's the root itself
            Transform head = prefab.transform.Find("Head");
            SpriteRenderer sr = head != null ? head.GetComponent<SpriteRenderer>() : prefab.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.sprite = sprite;
                // AI Sprites are high-res, we might need to adjust scale
                if (head != null)
                    head.localScale = new Vector3(0.6f, 0.6f, 1f); 
                else
                    prefab.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                    
                EditorUtility.SetDirty(prefab);
                Debug.Log($"Applied {sprite.name} to {prefab.name}");
            }
        }
    }
}
