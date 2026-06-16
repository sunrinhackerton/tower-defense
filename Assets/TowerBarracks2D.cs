using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns melee soldiers onto the path to block enemies.
/// </summary>
public class TowerBarracks2D : Tower2D
{
    [Header("Spawning")]
    public GameObject soldierPrefab;
    public int maxSoldiers = 2;
    public float respawnDelay = 10f;
    
    [Header("Detection")]
    public float range = 4f;

    [Header("Upgrade System")]
    public int level = 1;
    public int maxLevel = 5;
    public int baseUpgradeCost = 150;

    private List<GameObject> _activeSoldiers = new List<GameObject>();
    private float _respawnTimer;
    private Vector2 _rallyPoint;

    public int GetUpgradeCost() => baseUpgradeCost * level;

    public bool Upgrade()
    {
        if (level >= maxLevel) return false;
        level++;
        maxSoldiers += (level % 2 == 0) ? 1 : 0; // 짝수 레벨마다 병사 1명 추가
        respawnDelay = Mathf.Max(2f, respawnDelay * 0.8f);
        return true;
    }

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
        GameObject waypointsContainer = GameObject.Find("Waypoints");
        if (waypointsContainer != null && waypointsContainer.transform.childCount >= 2)
        {
            float minDistance = float.MaxValue;
            Vector2 bestPoint = transform.position;

            for (int i = 0; i < waypointsContainer.transform.childCount - 1; i++)
            {
                Vector2 a = waypointsContainer.transform.GetChild(i).position;
                Vector2 b = waypointsContainer.transform.GetChild(i + 1).position;
                Vector2 closestPoint = GetClosestPointOnSegment(a, b, transform.position);
                float dist = Vector2.Distance(transform.position, closestPoint);
                
                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestPoint = closestPoint;
                }
            }

            if (minDistance <= range)
            {
                _rallyPoint = bestPoint;
                return;
            }
        }

        // Fallback
        _rallyPoint = (Vector2)transform.position + new Vector2(0, -1.5f);
    }

    private Vector2 GetClosestPointOnSegment(Vector2 a, Vector2 b, Vector2 p)
    {
        Vector2 aToP = p - a;
        Vector2 aToB = b - a;
        float sqDist = aToB.sqrMagnitude;
        if (sqDist == 0) return a;
        float t = Mathf.Clamp01(Vector2.Dot(aToP, aToB) / sqDist);
        return a + aToB * t;
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
