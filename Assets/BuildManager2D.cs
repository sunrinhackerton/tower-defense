using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Tower Building UI and construction logic.
/// </summary>
public class BuildManager2D : MonoBehaviour
{
    public static BuildManager2D Instance;

    [Header("UI Panel")]
    public GameObject buildUIPanel;

    [Header("Tower Prefabs")]
    public GameObject ballistaPrefab;
    public GameObject catapultPrefab;
    public GameObject barracksPrefab;

    [Header("Tower Costs")]
    public int ballistaCost = 100;
    public int catapultCost = 150;
    public int barracksCost = 200;

    private BuildSite2D _currentSite;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (buildUIPanel != null)
            buildUIPanel.SetActive(false);
    }

    public void OpenBuildUI(BuildSite2D site)
    {
        if (site.HasTower) return;
        
        _currentSite = site;
        if (buildUIPanel != null)
        {
            buildUIPanel.SetActive(true);
            // Optionally set UI position near the site via RectTransform, but center is fine for now
        }
    }

    public void CloseBuildUI()
    {
        _currentSite = null;
        if (buildUIPanel != null)
            buildUIPanel.SetActive(false);
    }

    public void BuildBallista()
    {
        AttemptBuild(ballistaPrefab, ballistaCost);
    }

    public void BuildCatapult()
    {
        AttemptBuild(catapultPrefab, catapultCost);
    }

    public void BuildBarracks()
    {
        AttemptBuild(barracksPrefab, barracksCost);
    }

    private void AttemptBuild(GameObject towerPrefab, int cost)
    {
        if (_currentSite == null || towerPrefab == null) return;

        if (GameManager2D.Instance.CurrentCoins >= cost)
        {
            GameManager2D.Instance.UseCoins(cost);
            Instantiate(towerPrefab, _currentSite.transform.position, Quaternion.identity);
            
            _currentSite.SetBuilt();
            CloseBuildUI();
        }
        else
        {
            Debug.Log("[BuildManager] Not enough coins! Cost: " + cost);
        }
    }
}
