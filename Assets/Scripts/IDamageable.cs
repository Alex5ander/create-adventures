public interface IDamageable
{
    float Health { get; set; }
    int Defense { get; set; }
    void Die();
    void TakeDamage(float damage);
}