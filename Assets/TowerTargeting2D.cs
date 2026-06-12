using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finds the best target within range, filtering by ArmorType capabilities.
/// Can prioritize specific enemy types or just find the closest valid target.
/// </summary>
public class TowerTargeting2D : MonoBehaviour
{
    [Header("Detection")]
    public float range = 4.0f;
    public string enemyTag = "Monster";

    [Header("Targeting Filters")]
    public bool canTargetGround = true;
    public bool canTargetFlying = true;

    [Header("Debug")]
    public Transform currentTarget;

    public Transform CurrentTarget => currentTarget;

    void Update()
    {
        // 1. Check if current target is still valid
        if (currentTarget != null)
            if (!IsTargetValid(currentTarget))
                currentTarget = null;

        // 2. Find new target if we don't have one
        if (currentTarget == null)
            currentTarget = FindBestTarget();
    }

    private bool IsTargetValid(Transform target)
    {
        if (target == null) return false;
        
        // Out of range?
        if (Vector2.Distance(transform.position, target.position) > range)
            return false;

        // Dead?
        IDamageable d = target.GetComponent<IDamageable>();
        if (d != null && d.IsDead()) return false;

        // Check ArmorType Filter
        MonsterAI2D m = target.GetComponent<MonsterAI2D>();
        if (m != null)
        {
            if (!canTargetFlying && m.armorType == ArmorType.Flying) return false;
            if (!canTargetGround && (m.armorType == ArmorType.Light || m.armorType == ArmorType.Heavy)) return false;
        }

        return true;
    }

    private Transform FindBestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform bestTarget = null;
        float closestDist = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (!IsTargetValid(enemy.transform)) continue;

            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            
            // Basic Closest Targeting
            if (dist < closestDist)
            {
                closestDist = dist;
                bestTarget = enemy.transform;
            }
        }

        return bestTarget;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, range);

        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}
