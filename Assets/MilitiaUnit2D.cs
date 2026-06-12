using System.Collections;
using UnityEngine;

/// <summary>
/// Melee unit spawned by the Barracks.
/// Moves to rally point, blocks ground enemies and engages in melee combat.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class MilitiaUnit2D : MonoBehaviour, IDamageable
{
    public enum State { Moving, Idling, Engaged }

    [Header("Stats")]
    public int maxHP = 40;
    public int attackDamage = 10;
    public float attackDelay = 1.0f;
    public float moveSpeed = 2.0f;

    [Header("Visuals")]
    public Color normalColor = Color.cyan;
    public Color hitColor = Color.white;
    public Transform hpFill;
    public GameObject slashPrefab;

    private int _currentHP;
    private bool _dead;
    private SpriteRenderer _sr;
    
    // State
    public State currentState = State.Moving;
    private Vector2 _rallyPoint;

    // Combat state
    private MonsterAI2D _engagedMonster;
    private WaypointMovement2D _engagedMovement;
    private Coroutine _combatCoroutine;

    void Awake()
    {
        _currentHP = maxHP;
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = normalColor;
        
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        
        UpdateHPBar();
    }

    public void SetRallyPoint(Vector2 rallyPoint)
    {
        _rallyPoint = rallyPoint;
        currentState = State.Moving;
    }

    void Update()
    {
        if (_dead) return;

        if (currentState == State.Moving)
        {
            transform.position = Vector2.MoveTowards(transform.position, _rallyPoint, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, _rallyPoint) < 0.05f)
            {
                currentState = State.Idling;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_dead || currentState == State.Engaged) return;

        if (other.CompareTag("Monster"))
        {
            MonsterAI2D mon = other.GetComponent<MonsterAI2D>();
            WaypointMovement2D move = other.GetComponent<WaypointMovement2D>();

            // Don't block flying units
            if (mon != null && move != null && !move.isFlying && !move.isBlocked && !mon.IsDead())
            {
                Engage(mon, move);
            }
        }
    }

    private void Engage(MonsterAI2D mon, WaypointMovement2D move)
    {
        currentState = State.Engaged;
        _engagedMonster = mon;
        _engagedMovement = move;

        // Block movement
        _engagedMovement.isBlocked = true;

        // Start melee combat
        _combatCoroutine = StartCoroutine(CombatLoop());
    }

    private IEnumerator CombatLoop()
    {
        while (_engagedMonster != null && !_engagedMonster.IsDead() && !_dead)
        {
            yield return new WaitForSeconds(attackDelay);
            
            // Both deal damage simultaneously for simplicity
            if (!_dead && _engagedMonster != null && !_engagedMonster.IsDead())
            {
                if (slashPrefab != null)
                {
                    Vector2 slashPos = Vector2.Lerp(transform.position, _engagedMonster.transform.position, 0.5f);
                    Instantiate(slashPrefab, slashPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
                }
                _engagedMonster.TakeDamage(attackDamage, WeaponDamageType.Melee);
            }

            if (_engagedMonster != null && !_engagedMonster.IsDead() && !_dead)
            {
                TakeDamage(20, WeaponDamageType.Melee); // Monster always deals 20 to soldier
            }
        }

        // Combat ended (Monster died)
        if (_engagedMovement != null)
        {
            _engagedMovement.isBlocked = false;
        }
        _engagedMonster = null;
        _engagedMovement = null;
        
        // Return to idling
        currentState = State.Idling;
    }

    public void TakeDamage(int damage, WeaponDamageType dmgType = WeaponDamageType.Melee)
    {
        if (_dead) return;
        _currentHP -= damage;
        
        UpdateHPBar();
        StartCoroutine(HitFlash());

        if (_currentHP <= 0) Die();
    }

    private void UpdateHPBar()
    {
        if (hpFill != null)
        {
            float hpPercent = Mathf.Clamp01((float)_currentHP / maxHP);
            hpFill.localScale = new Vector3(hpPercent, 1f, 1f);
        }
    }

    public bool IsDead() => _dead;

    private IEnumerator HitFlash()
    {
        _sr.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        if (!_dead) _sr.color = normalColor;
    }

    private void Die()
    {
        if (_dead) return;
        _dead = true;

        if (_engagedMovement != null)
        {
            _engagedMovement.isBlocked = false; // Release the block
        }

        Destroy(gameObject);
    }
}
