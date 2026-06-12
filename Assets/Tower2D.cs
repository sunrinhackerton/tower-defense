using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower2D : MonoBehaviour
{
    public int atk = 10;
    public float delayAtk = 1.0f;
    public float range = 4.0f;

    [Header("Target")]
    public GameObject targetMob;

    private float _attackTimer = 0f;

    void Update()
    {
        // 타겟이 없거나 사정거리를 벗어난 경우 재탐색
        if (targetMob == null ||
            Vector2.Distance(transform.position, targetMob.transform.position) > range)
        {
            targetMob = null;
            SearchMobwithinRange();
        }

        // 타겟이 있고 쿨타임이 지났으면 공격
        if (targetMob != null)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= delayAtk)
            {
                Attack();
                _attackTimer = 0f;
            }
        }
        else
        {
            // 타겟 없을 때 타이머 리셋
            _attackTimer = 0f;
        }
    }

    void SearchMobwithinRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Monster"))
            {
                targetMob = col.gameObject;
                return;
            }
        }
    }

    void Attack()
    {
        Debug.Log($"{targetMob.name}에게 {atk} 데미지!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
