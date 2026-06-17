using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages both the Tower Building UI and the Tower Info (Upgrade/Sell) UI.
/// </summary>
public class BuildManager2D : MonoBehaviour
{
    public static BuildManager2D Instance;

    [Header("Build UI Panel (Left)")]
    public GameObject buildUIPanel;
    // Buttons for building
    public Button buildBasicBtn;
    public Button buildSniperBtn;
    public Button buildAoEBtn;
    public Button buildRapidBtn;
    public Button buildDebuffBtn;
    public Button buildBossBtn;

    [Header("Tower Info Panel (Right)")]
    public GameObject towerInfoPanel;
    public TMP_Text towerNameText;
    public TMP_Text towerStatsText;
    public Button upgradeButton;
    public Button sellButton;
    public Button closeInfoButton;

    [Header("Tower Prefabs")]
    public GameObject basicPrefab;
    public GameObject sniperPrefab;
    public GameObject aoePrefab;
    public GameObject rapidPrefab;
    public GameObject debuffPrefab;
    public GameObject bossPrefab;

    [Header("Tower Costs")]
    public int basicCost = 100;
    public int sniperCost = 250;
    public int aoeCost = 300;
    public int rapidCost = 150;
    public int debuffCost = 200;
    public int bossCost = 400;

    private BuildSite2D _currentSite;
    private Tower2D _selectedTower;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (buildUIPanel != null) buildUIPanel.SetActive(false);
        if (towerInfoPanel != null) towerInfoPanel.SetActive(false);
        
        // Setup Build Buttons
        if(buildBasicBtn) buildBasicBtn.onClick.AddListener(() => AttemptBuild(basicPrefab, basicCost));
        if(buildSniperBtn) buildSniperBtn.onClick.AddListener(() => AttemptBuild(sniperPrefab, sniperCost));
        if(buildAoEBtn) buildAoEBtn.onClick.AddListener(() => AttemptBuild(aoePrefab, aoeCost));
        if(buildRapidBtn) buildRapidBtn.onClick.AddListener(() => AttemptBuild(rapidPrefab, rapidCost));
        if(buildDebuffBtn) buildDebuffBtn.onClick.AddListener(() => AttemptBuild(debuffPrefab, debuffCost));
        if(buildBossBtn) buildBossBtn.onClick.AddListener(() => AttemptBuild(bossPrefab, bossCost));
        
        // Setup Info Buttons
        if(closeInfoButton) closeInfoButton.onClick.AddListener(CloseAllUI);
        if(upgradeButton) upgradeButton.onClick.AddListener(UpgradeTower);
        if(sellButton) sellButton.onClick.AddListener(SellTower);
    }

    // --- Build UI Logic ---
    public void OpenBuildUI(BuildSite2D site)
    {
        if (site.HasTower) return;
        
        CloseAllUI();
        _currentSite = site;
        
        if (buildUIPanel != null)
        {
            buildUIPanel.SetActive(true);
        }
    }

    // removed old Build wrappers

    private void AttemptBuild(GameObject towerPrefab, int cost)
    {
        if (_currentSite == null || towerPrefab == null) return;

        if (GameManager2D.Instance.CurrentCoins >= cost)
        {
            GameManager2D.Instance.UseCoins(cost);
            GameObject newTowerObj = Instantiate(towerPrefab, _currentSite.transform.position, Quaternion.identity);
            
            Tower2D towerComp = newTowerObj.GetComponent<Tower2D>();
            if(towerComp != null)
            {
                towerComp.SetBuildSite(_currentSite);
                _currentSite.builtTower = towerComp;
            }

            _currentSite.SetBuilt();
            CloseAllUI();
        }
        else
        {
            Debug.Log("[BuildManager] Not enough coins! Cost: " + cost);
        }
    }

    // --- Tower Info UI Logic (img2.png) ---
    public void OpenTowerInfo(Tower2D tower)
    {
        CloseAllUI();
        _selectedTower = tower;
        
        if (towerInfoPanel != null)
        {
            towerInfoPanel.SetActive(true);
            towerNameText.text = tower.gameObject.name.Replace("(Clone)", "");
            
            UpdateTowerStatsUI();
        }
    }

    private void UpdateTowerStatsUI()
    {
        if (_selectedTower == null) return;

        TowerWeapon2D weapon = _selectedTower.GetComponent<TowerWeapon2D>();
        TowerBarracks2D barracks = _selectedTower.GetComponent<TowerBarracks2D>();

        if (weapon != null)
        {
            string costStr = weapon.level < weapon.maxLevel ? $"{weapon.GetUpgradeCost()}$" : "MAX";
            towerStatsText.text = $"Lv. {weapon.level} / {weapon.maxLevel}\n\nAtk : {weapon.attackDamage}\nRange : 4.0\nCooldown : {weapon.attackDelay:F1}s";
            upgradeButton.GetComponentInChildren<TMP_Text>().text = $"Upgrade\n{costStr}";
            upgradeButton.interactable = weapon.level < weapon.maxLevel;
        }
        else if (barracks != null)
        {
            string costStr = barracks.level < barracks.maxLevel ? $"{barracks.GetUpgradeCost()}$" : "MAX";
            towerStatsText.text = $"Lv. {barracks.level} / {barracks.maxLevel}\n\nSoldiers : {barracks.maxSoldiers}\nRespawn : {barracks.respawnDelay:F1}s";
            upgradeButton.GetComponentInChildren<TMP_Text>().text = $"Upgrade\n{costStr}";
            upgradeButton.interactable = barracks.level < barracks.maxLevel;
        }
    }

    public void UpgradeTower()
    {
        if (_selectedTower == null) return;

        TowerWeapon2D weapon = _selectedTower.GetComponent<TowerWeapon2D>();
        TowerBarracks2D barracks = _selectedTower.GetComponent<TowerBarracks2D>();

        int cost = 0;
        if (weapon != null && weapon.level < weapon.maxLevel) cost = weapon.GetUpgradeCost();
        else if (barracks != null && barracks.level < barracks.maxLevel) cost = barracks.GetUpgradeCost();

        if (cost > 0 && GameManager2D.Instance.CurrentCoins >= cost)
        {
            GameManager2D.Instance.UseCoins(cost);
            
            if (weapon != null) weapon.Upgrade();
            if (barracks != null) barracks.Upgrade();

            UpdateTowerStatsUI();
        }
        else
        {
            Debug.Log("[BuildManager] Cannot upgrade: Not enough coins or MAX level.");
        }
    }

    public void SellTower()
    {
        if (_selectedTower != null)
        {
            // 임의의 판매 가격 (건설 비용의 50%)
            GameManager2D.Instance.AddCoins(50);
            
            if (_selectedTower.MyBuildSite != null)
            {
                _selectedTower.MyBuildSite.SetEmpty();
            }
            
            Destroy(_selectedTower.gameObject);
            CloseAllUI();
        }
    }

    public void CloseAllUI()
    {
        _currentSite = null;
        _selectedTower = null;
        if (buildUIPanel != null) buildUIPanel.SetActive(false);
        if (towerInfoPanel != null) towerInfoPanel.SetActive(false);
    }
}
