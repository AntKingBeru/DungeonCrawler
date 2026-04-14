using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    
    [SerializeField] private GameObject modelRoot;
    
    [Header("Runtime")]
    protected EnemyData Data;
    protected Transform Target;
    protected float CurrentHealth;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");
    
    protected bool IsActive;
    protected bool IsAttacking;

    private float _attackTimer;

    public UnityEvent<Enemy> onDeath;
    public UnityEvent<Enemy> onSpawnFinished;

    public virtual void Initialize(EnemyData data, Transform target)
    {
        Data = data;
        Target = target;

        CurrentHealth = Data.maxHealth;
        
        agent.speed = Data.moveSpeed;
        agent.enabled = false;

        StartCoroutine(SpawnRoutine());
    }
    
    private IEnumerator SpawnRoutine()
    {
        IsActive = false;
        agent.enabled = false;
        modelRoot.SetActive(true);
        
        // Spawn VFX
        if (Data.spawnVFX)
            Instantiate(
                Data.spawnVFX,
                transform.position,
                Quaternion.identity
            );
        
        // Play spawn animation
        if (animator && Data.spawnAnimation)
            animator.Play(Data.spawnAnimation.name);
        
        yield return new WaitForSeconds(Data.spawnDuration);
        
        agent.enabled = true;
        IsActive = true;
        
        onSpawnFinished?.Invoke(this);
    }
    
    protected virtual void Update()
    {
        if (!IsActive || !Target)
            return;
        
        _attackTimer -= Time.deltaTime;
        
        var distance = Vector3.Distance(transform.position, Target.position);

        if (distance <= Data.attackRange)
            HandleAttack();
        else
            HandleMovement();
        
        UpdateAnimation();
    }

    protected virtual void HandleMovement()
    {
        if (IsAttacking)
            return;
        
        agent.isStopped = false;
        agent.SetDestination(Target.position);
        
        UpdateRotation();
    }
    
    protected virtual void UpdateRotation()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            var rot = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }

    protected virtual void HandleAttack()
    {
        agent.isStopped = true;

        LookAtTarget();

        if (_attackTimer > 0f || IsAttacking)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        IsAttacking = true;
        
        if (animator && Data.attackAnimation)
            animator.SetTrigger(AttackHash);

        yield return new WaitForSeconds(Data.attackDuration * 0.4f);

        DealDamage();

        yield return new WaitForSeconds(Data.attackDuration * 0.6f);

        _attackTimer = Data.attackCooldown;
        IsAttacking = false;
    }

    protected virtual void DealDamage()
    {
        if (!Target)
            return;

        if (Target.TryGetComponent(out IDamageable damageable))
        {
            var damage = Data.damage * Data.attackDamageMultiplier;
            damageable.TakeDamage(damage);
        }
    }

    protected virtual void LookAtTarget()
    {
        var dir = (Target.position - transform.position).normalized;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.01f)
        {
            var rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }
    
    protected virtual void UpdateAnimation()
    {
        if (!animator)
            return;

        animator.SetFloat(SpeedHash, agent.velocity.magnitude);
    }

    public virtual void TakeDamage(float amount)
    {
        if (!IsActive)
            return;
        
        if (animator)
            animator.SetTrigger(HitHash);

        CurrentHealth -= amount;

        Debug.Log($"{Data.name} took {amount} damage. Current health: {CurrentHealth}");
        
        if (CurrentHealth <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        onDeath?.Invoke(this);
        agent.enabled = false;

        if (animator)
            animator.SetTrigger(DieHash);
        
        Destroy(gameObject);
    }
}