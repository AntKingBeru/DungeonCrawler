using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage);
    public void ApplyKnockback(Vector3 direction, float force);
}