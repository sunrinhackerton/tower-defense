using UnityEngine;
using UnityEditor;

public class SetupFigmaMap : EditorWindow
{
    [MenuItem("Tools/SetupFigmaMap")]
    public static void ReconstructMap()
    {
        // 1. Delete Old Map Objects
        GameObject oldEnv = GameObject.Find("MapEnvironment");
        if (oldEnv != null) DestroyImmediate(oldEnv);
        
        GameObject oldWp = GameObject.Find("Waypoints");
        if (oldWp != null) DestroyImmediate(oldWp);

        GameObject pathWp = GameObject.Find("Path_Waypoints");
        if (pathWp != null) DestroyImmediate(pathWp);

        GameObject pathVis = GameObject.Find("Path_Visuals");
        if (pathVis != null) DestroyImmediate(pathVis);
        
        BuildSite2D[] oldSites = Object.FindObjectsByType<BuildSite2D>(FindObjectsSortMode.None);
        foreach(var site in oldSites) DestroyImmediate(site.gameObject);

        // 2. Setup Waypoints (Figma S-Shape)
        GameObject waypointsRoot = new GameObject("Waypoints");
        Vector3[] pathPoints = new Vector3[] {
            new Vector3(-9, 3, 0),
            new Vector3(3, 3, 0),
            new Vector3(3, -1, 0),
            new Vector3(-3, -1, 0),
            new Vector3(-3, -4.5f, 0),
            new Vector3(9, -4.5f, 0)
        };

        for (int i = 0; i < pathPoints.Length; i++)
        {
            GameObject wp = new GameObject("Waypoint_" + i);
            wp.transform.position = pathPoints[i];
            wp.transform.SetParent(waypointsRoot.transform);
        }

        // 3. Map Environment (Grass & Roads)
        GameObject envRoot = new GameObject("MapEnvironment");

        // Grass Background
        GameObject grass = new GameObject("GrassBG");
        grass.transform.SetParent(envRoot.transform);
        SpriteRenderer srGrass = grass.AddComponent<SpriteRenderer>();
        srGrass.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/AI_Assets/grass_tile_1781625147606.png");
        srGrass.drawMode = SpriteDrawMode.Tiled;
        srGrass.size = new Vector2(30, 20); // Cover entire camera view
        srGrass.sortingOrder = -10;
        grass.transform.position = new Vector3(0, 0, 10);

        // Road Configuration
        Sprite roadSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/AI_Assets/road_tile_1781625158050.png");
        Sprite dashSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Figma/CleanDash.png");
        float roadWidth = 1.6f;

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            Vector3 start = pathPoints[i];
            Vector3 end = pathPoints[i+1];
            Vector3 dir = (end - start).normalized;
            float dist = Vector3.Distance(start, end);
            Vector3 center = start + dir * (dist / 2f);
            
            // Draw continuous stretched road
            GameObject road = new GameObject("RoadSegment");
            road.transform.SetParent(envRoot.transform);
            road.transform.position = new Vector3(center.x, center.y, 5); 
            
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            road.transform.rotation = Quaternion.Euler(0, 0, angle);
            // Overlap width at corners by adding roadWidth to length
            road.transform.localScale = new Vector3(dist + roadWidth, roadWidth, 1);
            
            SpriteRenderer srRoad = road.AddComponent<SpriteRenderer>();
            srRoad.sprite = roadSprite;
            srRoad.sortingOrder = -5;

            // Draw Dashed Lines
            float dashStep = 0.5f;
            // 코너 십자 겹침 방지를 위해 양 끝에서 여백을 둠
            for (float d = 0.6f; d <= dist - 0.6f; d += dashStep)
            {
                if (d % 1.0f > 0.5f) continue; // Skip every other step to make it "dashed"
                
                Vector3 dashPos = start + dir * d;
                GameObject dash = new GameObject("Dash");
                dash.transform.SetParent(road.transform); // Child of road segment
                dash.transform.position = new Vector3(dashPos.x, dashPos.y, 4); // Slightly above road
                dash.transform.rotation = Quaternion.Euler(0, 0, angle);
                dash.transform.localScale = new Vector3(0.4f, 0.08f, 1); // Thin white line
                
                SpriteRenderer srDash = dash.AddComponent<SpriteRenderer>();
                srDash.sprite = dashSprite;
                srDash.sortingOrder = -4;
            }
        }

        // 4. Build Sites (Grey squares in Figma)
        Vector3[] sitePositions = new Vector3[] {
            new Vector3(-1, 1, 0),  
            new Vector3(-5, 1, 0),
            new Vector3(0, -3, 0),  
            new Vector3(6, 1, 0),   
            new Vector3(6, -2, 0),
            new Vector3(-6, -2.5f, 0)
        };

        // Fix BuildSite Prefab visualization or create dynamically if missing
        GameObject bsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/BuildSite.prefab");
        Sprite cleanBuildSiteSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Figma/CleanBuildSite.png");
        GameObject basicTowerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tower_Basic.prefab");

        if (bsPrefab != null)
        {
            SpriteRenderer bsRenderer = bsPrefab.GetComponentInChildren<SpriteRenderer>();
            if (bsRenderer == null) bsRenderer = bsPrefab.AddComponent<SpriteRenderer>();
            bsRenderer.sprite = cleanBuildSiteSprite;
            bsRenderer.color = Color.white;
            bsRenderer.sortingOrder = 10; // Ensure it renders above the grass
            EditorUtility.SetDirty(bsPrefab);

            foreach (var pos in sitePositions)
            {
                GameObject bs = (GameObject)PrefabUtility.InstantiatePrefab(bsPrefab);
                bs.transform.position = pos;
            }
        }
        else
        {
            // 프리팹이 없으면 동적으로 6개의 영역 생성
            foreach (var pos in sitePositions)
            {
                GameObject bs = new GameObject("BuildSite");
                bs.transform.position = pos;
                bs.transform.SetParent(envRoot.transform);

                SpriteRenderer bsRenderer = bs.AddComponent<SpriteRenderer>();
                bsRenderer.sprite = cleanBuildSiteSprite;
                bsRenderer.color = Color.white;
                bsRenderer.sortingOrder = 10;

                BoxCollider2D col = bs.AddComponent<BoxCollider2D>();
                col.isTrigger = false;
                
                BuildSite2D bsComponent = bs.AddComponent<BuildSite2D>();
                if (basicTowerPrefab != null) bsComponent.towerPrefab = basicTowerPrefab;
            }
        }

        // 5. Apply Clean Figma Assets to Spawner
        GameObject monsterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Goblin_Warrior.prefab"); 
        if (monsterPrefab == null) monsterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gargoyle.prefab");

        EnemySpawner2D spawner = Object.FindFirstObjectByType<EnemySpawner2D>();
        // 만약 스포너가 씬에서 같이 지워졌다면 동적으로 다시 생성
        if (spawner == null)
        {
            GameObject spawnerObj = new GameObject("EnemySpawner");
            spawner = spawnerObj.AddComponent<EnemySpawner2D>();
            spawner.spawnInterval = 2f;
        }

        if (spawner != null && monsterPrefab != null)
        {
            spawner.transform.position = pathPoints[0];
            spawner.waypointsParent = waypointsRoot.transform;
            spawner.monsterPrefab = monsterPrefab;
            EditorUtility.SetDirty(spawner);
        }

        // 6. Set Camera Size to fit the Figma View
        if (Camera.main != null)
        {
            Camera.main.orthographicSize = 5.5f;
            Camera.main.transform.position = new Vector3(0, -0.5f, -10);
            Camera.main.backgroundColor = new Color(108f/255f, 169f/255f, 81f/255f);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("✅ Figma Map Reconstructed successfully with Clean Assets!");
    }
}
