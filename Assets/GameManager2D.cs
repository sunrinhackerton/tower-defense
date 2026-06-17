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
        
        // 초기 UI 셋업
        if (titleScreenPanel != null) titleScreenPanel.SetActive(true);
        if (inGameUIPanel != null) inGameUIPanel.SetActive(false);
        
        if (startGameBtn != null) startGameBtn.onClick.AddListener(StartGame);
        if (quitGameBtn != null) quitGameBtn.onClick.AddListener(QuitGame);
    }

    void Start()
    {
        _currentCoins = startingCoins;
        _currentLives = startingLives;
        IsGameOver = false;
        UpdateUI();
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
    [Header("Economy & Base")]
    public int startingCoins = 500;
    public int startingLives = 20;

    private int _currentCoins;
    private int _currentLives;
    public int CurrentCoins => _currentCoins;
    public int CurrentLives => _currentLives;
    public bool IsGameOver { get; private set; }

    /// <summary>Award coins to the player (e.g. kill reward).</summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        _currentCoins += amount;
        Debug.Log($"[Economy] +{amount} coins  |  Total: {_currentCoins}");
        UpdateUI();
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
        UpdateUI();
        return true;
    }

    public void TakeBaseDamage(int damage = 1)
    {
        if (IsGameOver) return;

        _currentLives -= damage;
        if (_currentLives <= 0)
        {
            _currentLives = 0;
            IsGameOver = true;
            Debug.Log("[GameManager] Game Over!");
            if (WaveManager2D.Instance != null && WaveManager2D.Instance.waveText != null)
            {
                WaveManager2D.Instance.waveText.text = "Game Over!";
                WaveManager2D.Instance.waveText.color = Color.red;
                Time.timeScale = 0f; // Halt the game
            }
        }
        UpdateUI();
    }

    // -------------------------------------------------------
    // UI
    // -------------------------------------------------------
    [Header("UI")]
    [Tooltip("Assign the TextMeshProUGUI component that shows the coin count.")]
    public TMP_Text coinText;

    private void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Coin :\n{_currentCoins} / A\nLives: {_currentLives}";
        }
    }
}
