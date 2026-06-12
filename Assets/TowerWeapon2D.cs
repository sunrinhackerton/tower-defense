using UnityEngine;

/// <summary>
/// Fires projectiles at the target acquired by TowerTargeting2D.
/// Passes DamageType and Splash settings to the Projectile.
/// </summary>
[RequireComponent(typeof(TowerTargeting2D))]
public class TowerWeapon2D : MonoBehaviour
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

    private TowerTargeting2D _targeting;
    private float _attackTimer;

    void Awake()
    {
        _targeting = GetComponent<TowerTargeting2D>();
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
    }
}
