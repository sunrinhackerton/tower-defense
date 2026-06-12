using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns melee soldiers onto the path to block enemies.
/// </summary>
public class TowerBarracks2D : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject soldierPrefab;
    public int maxSoldiers = 2;
    public float respawnDelay = 10f;
    
    [Header("Detection")]
    public float range = 4f;

    private List<GameObject> _activeSoldiers = new List<GameObject>();
    private float _respawnTimer;
    private Vector2 _rallyPoint;

    void Start()
    {
        CalculateRallyPoint();
        SpawnSoldiers();
    }

    void Update()
    {
        // Clean up dead soldiers
        _activeSoldiers.RemoveAll(s => s == null);

        if (_activeSoldiers.Count < maxSoldiers)
        {
            _respawnTimer += Time.deltaTime;
            if (_respawnTimer >= respawnDelay)
            {
                SpawnSoldier();
                _respawnTimer = 0f;
            }
        }
    }

    private void CalculateRallyPoint()
    {
        // Find nearest waypoint or path point
        GameObject waypointsContainer = GameObject.Find("Path_Waypoints");
        if (waypointsContainer != null)
        {
            Transform closestWP = null;
            float minDist = float.MaxValue;
            foreach (Transform wp in waypointsContainer.transform)
            {
                float dist = Vector2.Distance(transform.position, wp.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestWP = wp;
                }
            }

            if (closestWP != null && minDist <= range)
            {
                _rallyPoint = closestWP.position;
                return;
            }
        }

        // Fallback
        _rallyPoint = (Vector2)transform.position + new Vector2(0, -1.5f);
    }

    private void SpawnSoldiers()
    {
        for (int i = 0; i < maxSoldiers; i++)
        {
            SpawnSoldier();
        }
    }

    private void SpawnSoldier()
    {
        if (soldierPrefab == null) return;

        // Spawn at tower
        Vector2 spawnPos = transform.position;

        // Jitter rally point slightly so they don't overlap exactly
        Vector2 jitteredRally = _rallyPoint + Random.insideUnitCircle * 0.5f;

        GameObject soldier = Instantiate(soldierPrefab, spawnPos, Quaternion.identity);
        _activeSoldiers.Add(soldier);

        MilitiaUnit2D militia = soldier.GetComponent<MilitiaUnit2D>();
        if (militia != null)
        {
            militia.SetRallyPoint(jitteredRally);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, 0.2f);
        Gizmos.DrawWireSphere(transform.position, range);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _rallyPoint);
        Gizmos.DrawWireSphere(_rallyPoint, 0.5f);
    }
}
