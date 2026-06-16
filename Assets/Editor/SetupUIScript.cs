using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class SetupUIScript
{
    [MenuItem("Tools/Setup Build UI")]
    public static void SetupUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null) return;

        GameObject panel = GameObject.Find("BuildUI_Panel");
        if (panel == null)
        {
            panel = new GameObject("BuildUI_Panel");
            panel.transform.SetParent(canvas.transform, false);
        }

        Image bg = panel.GetComponent<Image>();
        if (bg == null) bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        RectTransform pRect = panel.GetComponent<RectTransform>();
        pRect.sizeDelta = new Vector2(300, 400);

        CreateButton(panel.transform, "Btn_Ballista", "Ballista (100)", new Vector2(0, 100), 1);
        CreateButton(panel.transform, "Btn_Catapult", "Catapult (150)", new Vector2(0, 20), 2);
        CreateButton(panel.transform, "Btn_Barracks", "Barracks (200)", new Vector2(0, -60), 3);
        CreateButton(panel.transform, "Btn_Cancel", "Cancel", new Vector2(0, -140), 4);

        // Bind to BuildManager2D
        BuildManager2D bm = Object.FindObjectOfType<BuildManager2D>();
        if (bm != null)
        {
            bm.buildUIPanel = panel;
            
            Button b1 = GameObject.Find("Btn_Ballista")?.GetComponent<Button>();
            if (b1) UnityEditor.Events.UnityEventTools.AddPersistentListener(b1.onClick, new UnityEngine.Events.UnityAction(bm.BuildBallista));
            
            Button b2 = GameObject.Find("Btn_Catapult")?.GetComponent<Button>();
            if (b2) UnityEditor.Events.UnityEventTools.AddPersistentListener(b2.onClick, new UnityEngine.Events.UnityAction(bm.BuildCatapult));
            
            Button b3 = GameObject.Find("Btn_Barracks")?.GetComponent<Button>();
            if (b3) UnityEditor.Events.UnityEventTools.AddPersistentListener(b3.onClick, new UnityEngine.Events.UnityAction(bm.BuildBarracks));
            
            Button b4 = GameObject.Find("Btn_Cancel")?.GetComponent<Button>();
            if (b4) UnityEditor.Events.UnityEventTools.AddPersistentListener(b4.onClick, new UnityEngine.Events.UnityAction(bm.CloseAllUI));

            // Load prefabs
            bm.ballistaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tower_Ballista.prefab");
            bm.catapultPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tower_Catapult.prefab");
            bm.barracksPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tower_Barracks.prefab");
        }

        panel.SetActive(false);
        Debug.Log("UI Setup Complete");
    }

    private static void CreateButton(Transform parent, string name, string label, Vector2 pos, int index)
    {
        GameObject btnObj = GameObject.Find(name);
        if (btnObj == null)
        {
            btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
        }

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        if (rt == null) rt = btnObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(250, 60);
        rt.anchoredPosition = pos;

        Image img = btnObj.GetComponent<Image>();
        if (img == null) img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        Button btn = btnObj.GetComponent<Button>();
        if (btn == null) btn = btnObj.AddComponent<Button>();

        // Text child
        Transform txtChild = btnObj.transform.Find("Text");
        GameObject txtObj = txtChild != null ? txtChild.gameObject : new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        
        RectTransform trt = txtObj.GetComponent<RectTransform>();
        if (trt == null) trt = txtObj.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = txtObj.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = txtObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 24;
    }
}
