using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform _target;
    private float _damage;
    private float _speed;
    
    public void Initialize(Transform target, float damage, float speed)
    {
        _target = target;
        _damage = damage;
        _speed = speed;
        
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        if (!_target)
        {
            Destroy(gameObject);
            return;
        }
        
        var dir = (_target.position - transform.position).normalized;
        transform.position += dir * _speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _target.position) < 0.5f)
        {
            if (_target.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(_damage);
            
            Destroy(gameObject);
        }
    }
}