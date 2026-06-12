using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SetupPhaseScript
{
    [MenuItem("Tools/Setup Phase 6.2")]
    public static void Setup()
    {
        // 1. Create Slash_VFX Prefab
        GameObject slashObj = new GameObject("Slash_VFX");
        SpriteRenderer sr = slashObj.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/SlashSprite.png");
        sr.color = Color.white;
        
        SelfDestruct2D sd = slashObj.AddComponent<SelfDestruct2D>();
        sd.lifetime = 0.2f;

        PrefabUtility.SaveAsPrefabAsset(slashObj, "Assets/Prefabs/Slash_VFX.prefab");
        Object.DestroyImmediate(slashObj);

        // 2. Add HP Bar to Soldier Prefab
        GameObject soldierPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Soldier.prefab");
        if (soldierPrefab != null)
        {
            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(soldierPrefab);
            
            // Check if it already has UI
            if (inst.transform.Find("HP_Canvas") == null)
            {
                GameObject canvasObj = new GameObject("HP_Canvas");
                canvasObj.transform.SetParent(inst.transform, false);
                canvasObj.transform.localPosition = new Vector3(0, 0.8f, 0);
                
                Canvas canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                
                RectTransform crt = canvasObj.GetComponent<RectTransform>();
                crt.sizeDelta = new Vector2(1.5f, 0.2f);

                GameObject bgObj = new GameObject("Background");
                bgObj.transform.SetParent(canvasObj.transform, false);
                Image bgImg = bgObj.AddComponent<Image>();
                bgImg.color = Color.black;
                RectTransform bgRt = bgObj.GetComponent<RectTransform>();
                bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one; bgRt.sizeDelta = Vector2.zero;

                GameObject fillObj = new GameObject("Fill");
                fillObj.transform.SetParent(canvasObj.transform, false);
                Image fillImg = fillObj.AddComponent<Image>();
                fillImg.color = Color.green;
                RectTransform fillRt = fillObj.GetComponent<RectTransform>();
                fillRt.anchorMin = new Vector2(0, 0.5f);
                fillRt.anchorMax = new Vector2(0, 0.5f);
                fillRt.pivot = new Vector2(0, 0.5f);
                fillRt.anchoredPosition = new Vector2(-0.75f, 0); // Left aligned
                fillRt.sizeDelta = new Vector2(1.5f, 0.2f);

                MilitiaUnit2D militia = inst.GetComponent<MilitiaUnit2D>();
                if (militia != null)
                {
                    // assign hpFill
                    SerializedObject so = new SerializedObject(militia);
                    so.FindProperty("hpFill").objectReferenceValue = fillObj.transform;
                    so.ApplyModifiedProperties();
                }

                PrefabUtility.SaveAsPrefabAssetAndConnect(inst, "Assets/Prefabs/Soldier.prefab", InteractionMode.AutomatedAction);
            }
            Object.DestroyImmediate(inst);
        }

        Debug.Log("Phase 6.2 Setup Complete");
    }
}
