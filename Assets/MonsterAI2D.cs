using System.Collections;
using UnityEngine;

/// <summary>
/// Monster combat and health component.
/// Implements IDamageable with Armor/Damage matrix.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class MonsterAI2D : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHP = 50;
    public ArmorType armorType = ArmorType.Light;

    [Header("Kill Reward")]
    public int coinReward = 20;

    [Header("Visual Feedback")]
    public Color hitFlashColor = Color.white;
    public float flashDuration  = 0.08f;
    
    [Tooltip("The fill Transform of the world-space HP bar to scale on damage")]
    public Transform hpFill;

    // -------------------------------------------------------
    // Runtime state
    // -------------------------------------------------------
    private int            _currentHP;
    private SpriteRenderer _sr;
    private Color          _baseColor;
    private bool           _dead;

    // -------------------------------------------------------
    // IDamageable
    // -------------------------------------------------------
    public void TakeDamage(int damage, WeaponDamageType dmgType = WeaponDamageType.Pierce)
    {
        if (_dead) return;

        // Apply Armor vs Damage Matrix
        int finalDamage = damage;
        
        if (dmgType == WeaponDamageType.Pierce)
        {
            if (armorType == ArmorType.Flying) finalDamage = damage * 2;
            else if (armorType == ArmorType.Heavy) finalDamage = Mathf.Max(1, (int)(damage * 0.1f));
        }
        else if (dmgType == WeaponDamageType.Splash)
        {
            // Catapult (Splash) cannot target Flying natively, but if it somehow hits:
            if (armorType == ArmorType.Flying) finalDamage = 0;
            else if (armorType == ArmorType.Heavy) finalDamage = Mathf.Max(1, (int)(damage * 0.5f));
        }

        if (finalDamage <= 0) return;

        _currentHP -= finalDamage;
        Debug.Log($"[Monster] {gameObject.name} (Armor: {armorType}) took {finalDamage} {dmgType} damage! HP: {_currentHP}/{maxHP}");
        
        // Update HP Bar
        if (hpFill != null)
        {
            float hpPercent = Mathf.Clamp01((float)_currentHP / maxHP);
            hpFill.localScale = new Vector3(hpPercent, 1f, 1f);
        }
        
        StopCoroutine(nameof(HitFlash));
        StartCoroutine(nameof(HitFlash));
        if (_currentHP <= 0) Die();
    }

    public bool IsDead() => _dead;

    // -------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------
    void Awake()
    {
        _sr        = GetComponent<SpriteRenderer>();
        _baseColor = _sr.color;
        _currentHP = maxHP;
    }

    // -------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------
    private IEnumerator HitFlash()
    {
        _sr.color = hitFlashColor;
        yield return new WaitForSeconds(flashDuration);
        _sr.color = _baseColor;
    }

    private void Die()
    {
        if (_dead) return;
        _dead = true;

        if (GameManager2D.Instance != null)
            GameManager2D.Instance.AddCoins(coinReward);

        // Visual fade then destroy
        _sr.color = new Color(0.3f, 0.0f, 0.5f, 0.4f);
        Destroy(gameObject, 0.35f);
    }
}
