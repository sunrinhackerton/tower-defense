using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct WaveData
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnInterval;
}

public class WaveManager2D : MonoBehaviour
{
    public static WaveManager2D Instance;

    [Header("Prefabs for Auto-Setup")]
    public GameObject goblinPrefab;
    public GameObject knightPrefab;
    public GameObject gargoylePrefab;

    [Header("Wave Configuration")]
    public WaveData[] waves;
    public float waveDelay = 5.0f;

    [Header("Waypoints")]
    public Transform waypointsParent;

    [Header("UI")]
    public TextMeshProUGUI waveText;

    private int _currentWaveIndex = 0;
    private List<GameObject> _activeEnemies = new List<GameObject>();
    private Transform[] _waypoints;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Auto-build 30 waves array with increasing difficulty
        if (goblinPrefab != null && knightPrefab != null && gargoylePrefab != null)
        {
            waves = new WaveData[30];
            for (int i = 0; i < 30; i++)
            {
                GameObject prefab = goblinPrefab;
                int count = 10 + i * 4; // Much larger swarms
                float interval = 2.5f; // Pause between clusters
                
                // 보스급 가고일 웨이브 (5웨이브 마다)
                if (i % 5 == 4) { prefab = gargoylePrefab; count = 1 + i / 5; interval = 4.0f; }
                // 기사 웨이브 (3웨이브 마다)
                else if (i % 3 == 2) { prefab = knightPrefab; count = 5 + i; interval = 3.0f; }

                waves[i] = new WaveData { enemyPrefab = prefab, count = count, spawnInterval = interval };
            }
        }
    }

    void Start()
    {
        if (waypointsParent != null)
        {
            int count = waypointsParent.childCount;
            _waypoints = new Transform[count];
            for (int i = 0; i < count; i++)
                _waypoints[i] = waypointsParent.GetChild(i);
        }

        StartCoroutine(WaveLoop());
    }

    void Update()
    {
        _activeEnemies.RemoveAll(e => e == null);
    }

    private IEnumerator WaveLoop()
    {
        yield return new WaitForSeconds(2.0f);

        while (_currentWaveIndex < waves.Length)
        {
            UpdateUI();
            
            WaveData currentWave = waves[_currentWaveIndex];
            
            // Cluster Spawning Logic
            int clusterSize = 5; 
            if (currentWave.enemyPrefab == gargoylePrefab) clusterSize = 1; // Bosses spawn individually
            else if (currentWave.enemyPrefab == knightPrefab) clusterSize = 3;

            int spawned = 0;
            while (spawned < currentWave.count)
            {
                int toSpawn = Mathf.Min(clusterSize, currentWave.count - spawned);
                for (int i = 0; i < toSpawn; i++)
                {
                    if (GameManager2D.Instance != null && GameManager2D.Instance.IsGameOver) yield break;

                    SpawnEnemy(currentWave.enemyPrefab);
                    spawned++;
                    yield return new WaitForSeconds(0.2f); // Rapid spawn within cluster
                }
                
                if (spawned < currentWave.count)
                {
                    yield return new WaitForSeconds(currentWave.spawnInterval); // Delay between clusters
                }
            }
            
            while (_activeEnemies.Count > 0)
            {
                if (GameManager2D.Instance != null && GameManager2D.Instance.IsGameOver) yield break;
                yield return null;
            }

            _currentWaveIndex++;
            if (_currentWaveIndex < waves.Length && (GameManager2D.Instance == null || !GameManager2D.Instance.IsGameOver))
            {
                if (waveText != null) waveText.text = "Wave Cleared! Next in " + waveDelay + "s...";
                yield return new WaitForSeconds(waveDelay);
            }
        }

        if (waveText != null && (GameManager2D.Instance == null || !GameManager2D.Instance.IsGameOver)) 
            waveText.text = "All Waves Cleared!";
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null || _waypoints == null || _waypoints.Length == 0) return;

        Vector3 spawnPos = _waypoints[0].position;
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        _activeEnemies.Add(enemy);

        if (enemy.TryGetComponent<WaypointMovement2D>(out var wpm))
            wpm.waypoints = _waypoints;
            
        if (enemy.TryGetComponent<MonsterAI2D>(out var ai))
            ai.Init(_currentWaveIndex);
    }

    private void UpdateUI()
    {
        if (waveText != null)
            waveText.text = $"Wave: {_currentWaveIndex + 1} / {waves.Length}";
    }
}
