using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IDamageable
{
    protected static readonly int SpeedHash = Animator.StringToHash("Speed");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");
    protected static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");

    public bool HasCc => IsRooted || ForcedTarget;
    
    #region Events

    public UnityEvent<Enemy> onDeath;
    public UnityEvent<Enemy> onSpawnFinished;
    
    #endregion

    #region References

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Animator animator;

    #endregion
    
    #region Data

    protected EnemyData Data;
    protected Transform Target;

    protected float CurrentHealth;

    private readonly List<ActiveEffect> _effects = new();

    #endregion
    
    #region State

    protected bool IsActive;
    protected bool IsAttacking;
    protected float AttackTimer;
    
    protected Transform ForcedTarget;
    protected bool IsRooted;
    
    #endregion
    
    #region Initialization

    public virtual void Initialize(EnemyData data, Transform target)
    {
        Data = data;
        Target = target;

        CurrentHealth = Data.maxHealth;
        
        agent.speed = Data.moveSpeed;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        IsActive = false;
        agent.enabled = false;
        
        if (Data.spawnVFX)
            Instantiate(Data.spawnVFX, transform.position, Quaternion.identity);
        
        if (animator && Data.spawnAnimation)
            animator.Play(Data.spawnAnimation.name);

        yield return new WaitForSeconds(Data.spawnDuration);
        
        agent.enabled = true;
        
        if (NavMesh.SamplePosition(transform.position, out var hit, 1f, NavMesh.AllAreas))
            agent.Warp(hit.position);

        yield return null;

        if (!agent.isOnNavMesh)
            yield break;
        
        IsActive = true;
        
        onSpawnFinished?.Invoke(this);
    }
    
    #endregion
    
    #region Update

    protected virtual void Update()
    {
        if (!IsActive)
            return;
        
        UpdateEffects(Time.deltaTime);
        
        AttackTimer -= Time.deltaTime;
        
        var currentTarget = ForcedTarget ? ForcedTarget : Target;

        if (!currentTarget)
            return;
        
        var dist = Vector3.Distance(transform.position, currentTarget.position);
        
        if (dist <= Data.attackRange)
            TryAttack(currentTarget);
        else
            MoveTo(currentTarget);
        
        UpdateAnimation();
    }

    protected virtual void UpdateAnimation()
    {
        if (animator)
            animator.SetFloat(SpeedHash, agent.velocity.magnitude);
    }
    
    #endregion
    
    #region Movement

    protected void MoveTo(Transform target)
    {
        if (IsRooted)
            return;
        
        if (!agent || !agent.enabled || !agent.isOnNavMesh)
            return;
        
        agent.isStopped = false;
        agent.SetDestination(target.position);

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
    
    #endregion
    
    #region Attack

    protected void TryAttack(Transform target)
    {
        if (agent && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = true;

        if (AttackTimer > 0f || IsAttacking)
            return;
        
        StartCoroutine(AttackRoutine(target));
    }

    private IEnumerator AttackRoutine(Transform target)
    {
        IsAttacking = true;
        
        animator.SetTrigger(AttackHash);

        yield return new WaitForSeconds(Data.damageDelay);

        ExecuteAttack(target);

        yield return new WaitForSeconds(Data.attackCooldown);

        AttackTimer = Data.attackCooldown;
        IsAttacking = false;
    }

    private void ExecuteAttack(Transform target)
    {
        if (!target.TryGetComponent(out SkillExecuter party))
            return;
        
        party.TakeDamage(Data.attackDamage);
    }
    
    #endregion
    
    #region Effects

    public void ApplyEffect(StatusEffectData effect, Transform source = null)
    {
        _effects.Add(new ActiveEffect
        {
            Data = effect,
            Timer = effect.duration,
            Source = source
        });
    }

    private void UpdateEffects(float dt)
    {
        IsRooted = false;
        ForcedTarget = null;

        for (var i = _effects.Count - 1; i >= 0; i--)
        {
            var e = _effects[i];
            e.Timer -= dt;
            
            switch (e.Data.type)
            {
                case StatusEffectType.Root:
                    IsRooted = true;
                    break;

                case StatusEffectType.Taunt:
                    ForcedTarget = e.Source;
                    break;
            }
            
            if (e.Timer <= 0f)
                _effects.RemoveAt(i);
        }
    }
    
    #endregion
    
    #region Damage

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;

        animator.SetTrigger(HitHash);

        if (CurrentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        onDeath?.Invoke(this);
        animator.SetTrigger(DieHash);
        Destroy(gameObject, 2f);
    }

    private void OnDestroy()
    {
        EnemyRegistry.Unregister(this);
    }
    
    #endregion
    
    #region Knockback

    public void ApplyKnockback(Vector3 direction, float force)
    {
        agent.Move(direction * force);
    }
    
    #endregion
}