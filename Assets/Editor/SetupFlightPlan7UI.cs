using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class SetupFlightPlan7UI : EditorWindow
{
    [MenuItem("Tools/Setup Flight Plan 7.0 UI")]
    public static void SetupUI()
    {
        // Clean up old UI to prevent duplicates
        Canvas oldCanvas = Object.FindFirstObjectByType<Canvas>();
        if (oldCanvas != null) DestroyImmediate(oldCanvas.gameObject);
        UnityEngine.EventSystems.EventSystem oldEs = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (oldEs != null) DestroyImmediate(oldEs.gameObject);

        // Create new Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create EventSystem
        GameObject esObj = new GameObject("EventSystem");
        esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        
        System.Type newModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (newModuleType != null)
        {
            esObj.AddComponent(newModuleType);
        }
        else
        {
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 1. Title Screen
        GameObject titlePanel = CreatePanel("TitleScreenPanel", canvas.transform, new Color(0.41f, 0.66f, 0.31f, 1f));
        titlePanel.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        titlePanel.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        titlePanel.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        titlePanel.GetComponent<RectTransform>().anchorMax = Vector2.one;
        
        CreateText("TitleText", titlePanel.transform, "MINI TOWER DEFENSE", 72, new Vector2(0, 150), new Vector2(800, 100));
        Button startBtn = CreateButton("StartButton", titlePanel.transform, "Assets/Sprites/ButtonGreen.png", "START", new Vector2(0, -50));
        Button quitBtn = CreateButton("QuitButton", titlePanel.transform, "Assets/Sprites/ButtonRed.png", "QUIT", new Vector2(0, -150));

        // 2. InGame UI Root
        GameObject inGamePanel = new GameObject("InGameUIPanel");
        inGamePanel.transform.SetParent(canvas.transform, false);
        RectTransform inGameRect = inGamePanel.AddComponent<RectTransform>();
        inGameRect.anchorMin = Vector2.zero; inGameRect.anchorMax = Vector2.one;
        inGameRect.sizeDelta = Vector2.zero;

        // 2-1. Coin UI (Bottom Left)
        GameObject coinUI = new GameObject("CoinUI");
        coinUI.transform.SetParent(inGamePanel.transform, false);
        RectTransform coinRect = coinUI.AddComponent<RectTransform>();
        coinRect.anchorMin = new Vector2(0, 0); coinRect.anchorMax = new Vector2(0, 0);
        coinRect.anchoredPosition = new Vector2(150, 50);
        coinRect.sizeDelta = new Vector2(250, 60);

        Image coinImg = new GameObject("CoinIcon").AddComponent<Image>();
        coinImg.transform.SetParent(coinUI.transform, false);
        coinImg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/CoinIcon.png");
        coinImg.rectTransform.anchoredPosition = new Vector2(-100, 0);
        coinImg.rectTransform.sizeDelta = new Vector2(40, 40);

        TMP_Text coinTxt = CreateText("CoinText", coinUI.transform, "Coin : 50000 / A", 36, new Vector2(20, 0), new Vector2(200, 50));
        coinTxt.alignment = TextAlignmentOptions.Left;

        // 2-2. Wave UI (Bottom Right)
        GameObject waveUI = new GameObject("WaveUI");
        waveUI.transform.SetParent(inGamePanel.transform, false);
        RectTransform waveRect = waveUI.AddComponent<RectTransform>();
        waveRect.anchorMin = new Vector2(1, 0); waveRect.anchorMax = new Vector2(1, 0);
        waveRect.anchoredPosition = new Vector2(-150, 50);
        waveRect.sizeDelta = new Vector2(250, 60);

        TMP_Text waveTxt = CreateText("WaveText", waveUI.transform, "Wave : 1 / 30", 36, Vector2.zero, new Vector2(250, 50));
        waveTxt.alignment = TextAlignmentOptions.Right;

        // 3. Build UI Panel (Left Center)
        GameObject buildPanel = CreatePanel("BuildUIPanel", inGamePanel.transform, new Color(0,0,0,0));
        buildPanel.GetComponent<Image>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UIPanelBackground.png");
        buildPanel.GetComponent<Image>().color = Color.white;
        RectTransform buildRect = buildPanel.GetComponent<RectTransform>();
        buildRect.anchorMin = new Vector2(0, 0.5f); buildRect.anchorMax = new Vector2(0, 0.5f);
        buildRect.anchoredPosition = new Vector2(150, 0);
        buildRect.sizeDelta = new Vector2(200, 350);

        Button buildBalBtn = CreateButton("Btn_Ballista", buildPanel.transform, "Assets/Sprites/ButtonGrey.png", "Ballista\n100$", new Vector2(0, 100));
        Button buildCatBtn = CreateButton("Btn_Catapult", buildPanel.transform, "Assets/Sprites/ButtonGrey.png", "Catapult\n150$", new Vector2(0, 0));
        Button buildBarBtn = CreateButton("Btn_Barracks", buildPanel.transform, "Assets/Sprites/ButtonGrey.png", "Barracks\n200$", new Vector2(0, -100));

        // 4. Tower Info Panel (Right Center)
        GameObject infoPanel = CreatePanel("TowerInfoPanel", inGamePanel.transform, Color.white);
        infoPanel.GetComponent<Image>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UIPanelBackground.png");
        RectTransform infoRect = infoPanel.GetComponent<RectTransform>();
        infoRect.anchorMin = new Vector2(1, 0.5f); infoRect.anchorMax = new Vector2(1, 0.5f);
        infoRect.anchoredPosition = new Vector2(-200, 0);
        infoRect.sizeDelta = new Vector2(300, 450);

        TMP_Text towerNameTxt = CreateText("TowerName", infoPanel.transform, "Tower Name", 28, new Vector2(0, 180), new Vector2(250, 40));
        Button closeInfoBtn = CreateButton("Btn_Close", infoPanel.transform, "Assets/Sprites/ButtonRed.png", "X", new Vector2(120, 190));
        closeInfoBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);

        Image towerIcon = new GameObject("TowerIcon").AddComponent<Image>();
        towerIcon.transform.SetParent(infoPanel.transform, false);
        towerIcon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/TowerHead_Basic.png");
        towerIcon.rectTransform.anchoredPosition = new Vector2(0, 90);
        towerIcon.rectTransform.sizeDelta = new Vector2(100, 100);

        TMP_Text towerStatsTxt = CreateText("TowerStats", infoPanel.transform, "Lv. 1 / 5\nAtk : 10\nRange : 4\nCooldown : 1", 24, new Vector2(0, -30), new Vector2(250, 120));
        
        Button upgradeBtn = CreateButton("Btn_Upgrade", infoPanel.transform, "Assets/Sprites/ButtonGreen.png", "Upgrade", new Vector2(0, -120));
        Button sellBtn = CreateButton("Btn_Sell", infoPanel.transform, "Assets/Sprites/ButtonRed.png", "Sell", new Vector2(0, -180));

        // 5. Assign to Managers
        GameManager2D gm = Object.FindFirstObjectByType<GameManager2D>();
        if (gm != null)
        {
            gm.titleScreenPanel = titlePanel;
            gm.inGameUIPanel = inGamePanel;
            gm.startGameBtn = startBtn;
            gm.quitGameBtn = quitBtn;
            gm.coinText = coinTxt;

            // Persistent Listeners
            UnityEditor.Events.UnityEventTools.AddPersistentListener(startBtn.onClick, new UnityEngine.Events.UnityAction(gm.StartGame));
            UnityEditor.Events.UnityEventTools.AddPersistentListener(quitBtn.onClick, new UnityEngine.Events.UnityAction(gm.QuitGame));

            EditorUtility.SetDirty(gm);
        }

        BuildManager2D bm = Object.FindFirstObjectByType<BuildManager2D>();
        if (bm != null)
        {
            bm.buildUIPanel = buildPanel;
            bm.buildBallistaBtn = buildBalBtn;
            bm.buildCatapultBtn = buildCatBtn;
            bm.buildBarracksBtn = buildBarBtn;

            bm.towerInfoPanel = infoPanel;
            bm.towerNameText = towerNameTxt;
            bm.towerStatsText = towerStatsTxt;
            bm.upgradeButton = upgradeBtn;
            bm.sellButton = sellBtn;
            bm.closeInfoButton = closeInfoBtn;

            // Persistent Listeners
            UnityEditor.Events.UnityEventTools.AddPersistentListener(buildBalBtn.onClick, new UnityEngine.Events.UnityAction(bm.BuildBallista));
            UnityEditor.Events.UnityEventTools.AddPersistentListener(buildCatBtn.onClick, new UnityEngine.Events.UnityAction(bm.BuildCatapult));
            UnityEditor.Events.UnityEventTools.AddPersistentListener(buildBarBtn.onClick, new UnityEngine.Events.UnityAction(bm.BuildBarracks));

            UnityEditor.Events.UnityEventTools.AddPersistentListener(closeInfoBtn.onClick, new UnityEngine.Events.UnityAction(bm.CloseAllUI));
            UnityEditor.Events.UnityEventTools.AddPersistentListener(upgradeBtn.onClick, new UnityEngine.Events.UnityAction(bm.UpgradeTower));
            UnityEditor.Events.UnityEventTools.AddPersistentListener(sellBtn.onClick, new UnityEngine.Events.UnityAction(bm.SellTower));

            EditorUtility.SetDirty(bm);
        }

        WaveManager2D wm = Object.FindFirstObjectByType<WaveManager2D>();
        if (wm != null)
        {
            wm.waveText = (TextMeshProUGUI)waveTxt;
            EditorUtility.SetDirty(wm);
        }

        // Save Scene
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("✅ Flight Plan 7.0 UI Setup Complete!");
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    private static TMP_Text CreateText(string name, Transform parent, string text, int fontSize, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return tmp;
    }

    private static Button CreateButton(string name, Transform parent, string spritePath, string text, Vector2 pos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        Button btn = go.AddComponent<Button>();
        
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(160, 50);

        CreateText("Text", go.transform, text, 24, Vector2.zero, rt.sizeDelta);

        return btn;
    }
}
