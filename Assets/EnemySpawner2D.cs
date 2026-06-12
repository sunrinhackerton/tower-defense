using System.Collections;
using UnityEngine;

/// <summary>
/// Infinite enemy spawner. Instantiates monsterPrefab every spawnInterval seconds
/// at its own position and injects the waypoint array so the monster can navigate.
/// </summary>
public class EnemySpawner2D : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject monsterPrefab;
    public float      spawnInterval = 2f;

    [Header("Waypoints")]
    [Tooltip("Parent Transform whose children are the ordered waypoints.")]
    public Transform waypointsParent;

    // Cached array built from waypointsParent's children
    private Transform[] _waypoints;
    private int         _spawnCount;

    void Start()
    {
        // --- Validation ---
        if (monsterPrefab == null)
        {
            Debug.LogError("[Spawner] monsterPrefab is not assigned!");
            return;
        }
        if (waypointsParent == null)
        {
            Debug.LogError("[Spawner] waypointsParent is not assigned!");
            return;
        }

        // Build cached waypoint array from parent's children
        int count = waypointsParent.childCount;
        _waypoints = new Transform[count];
        for (int i = 0; i < count; i++)
            _waypoints[i] = waypointsParent.GetChild(i);

        if (_waypoints.Length == 0)
        {
            Debug.LogError("[Spawner] waypointsParent has no children!");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        // Brief initial delay so the scene fully initialises
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        _spawnCount++;
        // Spawn at first waypoint position
        Vector3 spawnPos = (_waypoints.Length > 0)
            ? _waypoints[0].position
            : transform.position;

        GameObject enemy = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
        enemy.name = $"Monster_{_spawnCount:D3}";

        // Inject waypoints into WaypointMovement2D
        if (enemy.TryGetComponent<WaypointMovement2D>(out var wpm))
            wpm.waypoints = _waypoints;
        else
            Debug.LogWarning($"[Spawner] {enemy.name} has no WaypointMovement2D!");
    }
}
