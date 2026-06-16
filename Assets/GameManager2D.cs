using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Singleton game manager. Owns the coin economy and UI.
/// Access from anywhere via GameManager2D.Instance.
/// </summary>
public class GameManager2D : MonoBehaviour
{
    // -------------------------------------------------------
    // Singleton
    // -------------------------------------------------------
    public static GameManager2D Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _currentCoins = startingCoins;
        UpdateCoinUI();
        
        // 초기 UI 셋업
        if (titleScreenPanel != null) titleScreenPanel.SetActive(true);
        if (inGameUIPanel != null) inGameUIPanel.SetActive(false);
        
        if (startGameBtn != null) startGameBtn.onClick.AddListener(StartGame);
        if (quitGameBtn != null) quitGameBtn.onClick.AddListener(QuitGame);
    }

    // -------------------------------------------------------
    // Game Flow (Title Screen)
    // -------------------------------------------------------
    [Header("Game Flow UI")]
    public GameObject titleScreenPanel;
    public GameObject inGameUIPanel;
    public Button startGameBtn;
    public Button quitGameBtn;

    public void StartGame()
    {
        if (titleScreenPanel != null) titleScreenPanel.SetActive(false);
        if (inGameUIPanel != null) inGameUIPanel.SetActive(true);
        // 필요시 WaveManager 시작 신호 송신
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // -------------------------------------------------------
    // Economy
    // -------------------------------------------------------
    [Header("Economy")]
    public int startingCoins = 50000;

    private int _currentCoins;
    public int CurrentCoins => _currentCoins;

    /// <summary>Award coins to the player (e.g. kill reward).</summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        _currentCoins += amount;
        Debug.Log($"[Economy] +{amount} coins  |  Total: {_currentCoins}");
        UpdateCoinUI();
    }

    /// <summary>
    /// Deduct coins. Returns true on success, false if insufficient funds.
    /// </summary>
    public bool UseCoins(int amount)
    {
        if (amount <= 0) return true;
        if (_currentCoins < amount)
        {
            Debug.Log($"[Economy] 코인 부족! 필요: {amount}  보유: {_currentCoins}");
            return false;
        }
        _currentCoins -= amount;
        Debug.Log($"[Economy] -{amount} coins  |  Remaining: {_currentCoins}");
        UpdateCoinUI();
        return true;
    }

    // -------------------------------------------------------
    // UI
    // -------------------------------------------------------
    [Header("UI")]
    [Tooltip("Assign the TextMeshProUGUI component that shows the coin count.")]
    public TMP_Text coinText;

    private void UpdateCoinUI()
    {
        if (coinText == null) return;
        coinText.text = $"Coin : {_currentCoins} / A";
    }
}
