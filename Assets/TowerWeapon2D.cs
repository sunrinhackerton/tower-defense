using UnityEngine;

/// <summary>
/// Fires projectiles at the target acquired by TowerTargeting2D.
/// Passes DamageType and Splash settings to the Projectile.
/// </summary>
[RequireComponent(typeof(TowerTargeting2D))]
public class TowerWeapon2D : Tower2D
{
    [Header("Combat Stats")]
    public int   attackDamage = 10;
    public float attackDelay  = 1.0f;
    
    [Header("Weapon Type")]
    public WeaponDamageType damageType = WeaponDamageType.Pierce;
    public float splashRadius = 0f;

    [Header("References")]
    public GameObject projectilePrefab;
    public Transform  muzzlePoint;

    [Header("Upgrade System")]
    public int level = 1;
    public int maxLevel = 5;
    public int baseUpgradeCost = 100;

    private TowerTargeting2D _targeting;
    private float _attackTimer;
    private Transform _headTransform;

    void Awake()
    {
        _targeting = GetComponent<TowerTargeting2D>();
        _headTransform = transform.Find("Head");
    }

    public int GetUpgradeCost() => baseUpgradeCost * level;

    public bool Upgrade()
    {
        if (level >= maxLevel) return false;
        level++;
        attackDamage += (int)(attackDamage * 0.3f); // 30% 증가
        attackDelay = Mathf.Max(0.2f, attackDelay * 0.85f); // 15% 쿨타임 감소
        return true;
    }

    void Update()
    {
        _attackTimer += Time.deltaTime;

        Transform target = _targeting.CurrentTarget;
        if (target != null && _attackTimer >= attackDelay)
        {
            Fire(target);
            _attackTimer = 0f;
        }
    }

    private void Fire(Transform target)
    {
        if (projectilePrefab == null || muzzlePoint == null)
        {
            Debug.LogError($"[TowerWeapon2D] Cannot fire! {gameObject.name} is missing projectilePrefab or muzzlePoint.");
            return;
        }

        GameObject projObj = Instantiate(projectilePrefab, muzzlePoint.position, Quaternion.identity);
        Projectile2D proj = projObj.GetComponent<Projectile2D>();
        
        if (proj != null)
        {
            proj.Setup(target, attackDamage, splashRadius, damageType);
        }

        // 반동 애니메이션 코루틴 실행
        StartCoroutine(RecoilAnimation(target));
    }

    private System.Collections.IEnumerator RecoilAnimation(Transform target)
    {
        Transform targetTr = _headTransform != null ? _headTransform : transform;
        Vector3 originalPos = targetTr.localPosition;
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 recoilPos = originalPos - direction * 0.2f;

        // 1. 강하게 뒤로 밀림 (Recoil)
        float t = 0;
        while (t < 0.05f)
        {
            t += Time.deltaTime;
            targetTr.localPosition = Vector3.Lerp(originalPos, recoilPos, t / 0.05f);
            yield return null;
        }

        // 2. 원래 자리로 부드럽게 복구 (Recovery)
        t = 0;
        while (t < 0.15f)
        {
            t += Time.deltaTime;
            targetTr.localPosition = Vector3.Lerp(recoilPos, originalPos, t / 0.15f);
            yield return null;
        }
        targetTr.localPosition = originalPos;
    }
}
