public interface IDamageable
{
    void TakeDamage(int damage, WeaponDamageType dmgType = WeaponDamageType.Pierce);
    bool IsDead();
}
