public interface IDamageable
{
    void TakeDamage(int damage);
    void Heal(int amount);
    float HealthPercentage { get; }
    bool IsAlive { get; }
}