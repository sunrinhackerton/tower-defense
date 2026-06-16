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

        Vector3 moveDir = (_lastKnownPos - (Vector2)transform.position).normalized;
        if (moveDir != Vector3.zero)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

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
            SpawnExplosion(splashRadius * 2f, new Color(1f, 0.5f, 0f, 0.8f));
        }
        else
        {
            // Single Target
            if (_target != null)
            {
                IDamageable m = _target.GetComponent<IDamageable>();
                m?.TakeDamage(damage, damageType);
            }
            SpawnExplosion(1f, new Color(1f, 1f, 0f, 0.8f));
        }

        Destroy(gameObject);
    }

    private void SpawnExplosion(float scale, Color color)
    {
        GameObject fx = new GameObject("ExplosionFX");
        fx.transform.position = transform.position;
        fx.transform.localScale = new Vector3(scale, scale, 1f);
        
        SpriteRenderer sr = fx.AddComponent<SpriteRenderer>();
        SpriteRenderer mySr = GetComponent<SpriteRenderer>();
        if (mySr != null) sr.sprite = mySr.sprite;
        sr.color = color;
        sr.sortingOrder = 10;

        SelfDestruct2D sd = fx.AddComponent<SelfDestruct2D>();
        sd.lifetime = 0.2f;
        sd.animate = true;
    }
}
