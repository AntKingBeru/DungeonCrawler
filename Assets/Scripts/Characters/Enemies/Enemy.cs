using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] protected NavMeshAgent agent;
    
    [SerializeField] protected GameObject modelRoot;
    
    [Header("Runtime")]
    protected EnemyData Data;
    protected Transform Target;
    protected float CurrentHealth;
    
    [Header("Animation")]
    [SerializeField] protected Animator animator;
    protected static readonly int SpeedHash = Animator.StringToHash("Speed");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");
    protected static readonly int HitHash = Animator.StringToHash("Hit");
    protected static readonly int DieHash = Animator.StringToHash("Die");
    
    [Header("Performance")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int maxHits = 32;
    
    protected bool IsActive;
    protected bool IsAttacking;

    protected float AttackTimer;
    
    private Collider[] _overlapBuffer;

    public UnityEvent<Enemy> onDeath;
    public UnityEvent<Enemy> onSpawnFinished;

    public virtual void Initialize(EnemyData data, Transform target)
    {
        Data = data;
        Target = target;

        CurrentHealth = Data.maxHealth;
        
        agent.speed = Data.moveSpeed;
        agent.enabled = false;
        
        _overlapBuffer = new Collider[maxHits];

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
        
        AttackTimer -= Time.deltaTime;
        
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

        if (AttackTimer > 0f || IsAttacking)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        IsAttacking = true;
        
        if (animator && Data.attackAnimation)
            animator.SetTrigger(AttackHash);

        yield return new WaitForSeconds(Data.damageDelay);

        ExecuteAttack();

        yield return new WaitForSeconds(Data.attackDuration - Data.damageDelay);

        AttackTimer = Data.attackCooldown;
        IsAttacking = false;
    }

    protected virtual void ExecuteAttack()
    {
        switch (Data.attackType)
        {
            case EnemyAttackType.Melee:
                DoMeleeAttack();
                break;
            case EnemyAttackType.Ranged:
                DoRangedAttack();
                break;
            case EnemyAttackType.AoE:
                DoAoEAttack();
                break;
        }
    }

    protected virtual void DoMeleeAttack()
    {
        if (!Target)
            return;

        if (Vector3.Distance(transform.position, Target.position) > Data.attackRange)
            return;
        
        if (Target.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(Data.attackDamage);
    }

    protected virtual void DoRangedAttack()
    {
        if (!Target || !Data.projectilePrefab)
            return;

        var dir = (Target.position - transform.position).normalized;

        var proj = Instantiate(
            Data.projectilePrefab,
            transform.position + Vector3.up * 1.5f,
            Quaternion.LookRotation(dir)
        );
        
        proj.Initialize(Target, Data.attackDamage, Data.projectileSpeed);
    }

    protected virtual void DoAoEAttack()
    {
        var hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            Data.aoeRadius,
            _overlapBuffer,
            targetLayer
        );

        for (var i = 0; i < hitCount; i++)
        {
            var col = _overlapBuffer[i];
            
            if (col.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(Data.attackDamage);
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
        
        if (CurrentHealth <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        onDeath?.Invoke(this);
        agent.enabled = false;

        if (animator)
            animator.SetTrigger(DieHash);

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}