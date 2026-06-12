using UnityEngine;

/// <summary>
/// Moves toward a target Transform and applies damage upon arrival.
/// Supports both single-target and splash area damage.
/// </summary>
public class Projectile2D : MonoBehaviour
{
    [Header("Flight Stats")]
    public float speed = 10f;
    public float hitRadius = 0.2f;

    [Header("Combat Stats")]
    public int              damage       = 10;
    public float            splashRadius = 0f;
    public WeaponDamageType damageType   = WeaponDamageType.Pierce;

    private Transform _target;
    private Vector2   _lastKnownPos;

    public void Setup(Transform target, int dmg, float splash, WeaponDamageType dmgType)
    {
        _target      = target;
        damage       = dmg;
        splashRadius = splash;
        damageType   = dmgType;
        
        if (_target != null)
            _lastKnownPos = _target.position;
    }

    void Update()
    {
        if (_target != null)
            _lastKnownPos = _target.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _lastKnownPos,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, _lastKnownPos) <= hitRadius)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (splashRadius > 0f)
        {
            // Splash Damage
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, splashRadius);
            foreach (var col in cols)
            {
                if (col.CompareTag("Monster"))
                {
                    IDamageable hitMon = col.GetComponent<IDamageable>();
                    hitMon?.TakeDamage(damage, damageType);
                }
            }
            // Optional: Spawn explosion effect here
        }
        else
        {
            // Single Target
            if (_target != null)
            {
                IDamageable m = _target.GetComponent<IDamageable>();
                m?.TakeDamage(damage, damageType);
            }
        }

        Destroy(gameObject);
    }
}
