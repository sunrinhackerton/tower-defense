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

        // Auto-build waves array
        if (goblinPrefab != null && knightPrefab != null && gargoylePrefab != null)
        {
            waves = new WaveData[]
            {
                new WaveData { enemyPrefab = goblinPrefab, count = 10, spawnInterval = 0.5f },
                new WaveData { enemyPrefab = knightPrefab, count = 3, spawnInterval = 3.0f },
                new WaveData { enemyPrefab = gargoylePrefab, count = 5, spawnInterval = 1.5f }
            };
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
            for (int i = 0; i < currentWave.count; i++)
            {
                SpawnEnemy(currentWave.enemyPrefab);
                yield return new WaitForSeconds(currentWave.spawnInterval);
            }
            
            while (_activeEnemies.Count > 0)
            {
                yield return null;
            }

            _currentWaveIndex++;
            if (_currentWaveIndex < waves.Length)
            {
                if (waveText != null) waveText.text = "Wave Cleared! Next in " + waveDelay + "s...";
                yield return new WaitForSeconds(waveDelay);
            }
        }

        if (waveText != null) waveText.text = "All Waves Cleared!";
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null || _waypoints == null || _waypoints.Length == 0) return;

        Vector3 spawnPos = _waypoints[0].position;
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        _activeEnemies.Add(enemy);

        if (enemy.TryGetComponent<WaypointMovement2D>(out var wpm))
            wpm.waypoints = _waypoints;
    }

    private void UpdateUI()
    {
        if (waveText != null)
            waveText.text = $"Wave: {_currentWaveIndex + 1} / {waves.Length}";
    }
}
