using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SkillExecuter : MonoBehaviour, IDamageable
{
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private const string PlayerTag = "Player";
    
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform castPoint;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int maxHits = 32;
    
    private Collider[] _overlapBuffer;
    private RaycastHit[] _castBuffer;
    
    private void Awake()
    {
        _overlapBuffer = new Collider[maxHits];
        _castBuffer = new RaycastHit[maxHits];
    }

    #region Data
    
    private CharacterSelectionData _data;
    private Animator _animator;
    
    private readonly List<ActiveEffect> _effects = new();

    private float _currentHealth;
    
    #endregion
    
    #region Runtime Modifiers

    private float _damageMultiplier;
    private float _damageReduction;
    private float _moveSpeedMultiplier;
    private float _attackSpeedMultiplier;

    private bool _isRooted;
    private bool _isUntargetable;
    private bool _deathImmune;

    private Transform _forcedTarget;

    public float AttackSpeedMultiplier => _attackSpeedMultiplier;
    
    #endregion
    
    #region Initialize

    public void Initialize(CharacterSelectionData data, Animator animator)
    {
        _data = data;
        _animator = animator;

        _currentHealth = data.MaxHealth;
    }

    private void OnEnable()
    {
        PartyRegistry.Register(this);
    }

    private void OnDisable()
    {
        PartyRegistry.Unregister(this);
    }
    
    #endregion
    
    #region Update
    
    private void Update()
    {
        UpdateEffects(Time.deltaTime);
    }

    private void UpdateEffects(float dt)
    {
        _damageMultiplier = 1f;
        _damageReduction = 0f;
        _moveSpeedMultiplier = 1f;
        _attackSpeedMultiplier = 1f;
        
        _isRooted = false;
        _isUntargetable = false;
        _deathImmune = false;
        _forcedTarget = null;

        for (var i = _effects.Count - 1; i >= 0; i--)
        {
            var e = _effects[i];
            e.Timer -= dt;

            ApplyEffectTick(e, dt);
            
            if (e.Timer <= 0f)
                _effects.RemoveAt(i);
        }
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

        if (effect.type == StatusEffectType.TeleportBehind && source)
        {
            var dir = source.forward;
            transform.position = source.position - dir * 2f;
        }
    }

    private void ApplyEffectTick(ActiveEffect e, float dt)
    {
        var data = e.Data;

        if (data.tickInterval > 0f)
        {
            e.Timer -= dt;

            if (e.Timer <= 0f)
            {
                e.Timer = data.tickInterval;
                
                if (data.type == StatusEffectType.HealOverTime)
                    Heal(data.tickValue);
                
                if (data.type == StatusEffectType.DamageOverTime)
                    TakeDamage(data.tickValue);
            }
        }

        switch (data.type)
        {
            case StatusEffectType.AttackSpeed:
                _attackSpeedMultiplier += data.value;
                break;
            case StatusEffectType.MoveSpeed:
                _moveSpeedMultiplier += data.value;
                break;
            case StatusEffectType.Slow:
                _moveSpeedMultiplier -= data.value;
                break;
            case StatusEffectType.Root:
                _isRooted = true;
                break;
            case StatusEffectType.DamageReduction:
                _damageReduction += data.value;
                break;
            case StatusEffectType.DamageBoost:
                _damageMultiplier += data.value;
                break;
            case StatusEffectType.HealingBoost:
                // handled in Heal()
                break;
            case StatusEffectType.Taunt:
                _forcedTarget = e.Source;
                break;
            case StatusEffectType.Untargetable:
                _isUntargetable = true;
                break;
            case StatusEffectType.DeathImmunity:
                _deathImmune = true;
                break;
        }
    }
    
    #endregion
    
    #region Damage / Healing

    public void TakeDamage(float amount)
    {
        if (_isUntargetable)
            return;
        
        amount *= 1f - _damageReduction;
        
        _currentHealth -= amount;
        
        if (_animator)
            _animator.SetTrigger(HitHash);

        foreach (var e in _effects.Where(e => HasEffect(StatusEffectType.Reflect) && e.Source))
        {
            if (e.Source.TryGetComponent<IDamageable>(out var src))
                src.TakeDamage(amount * e.Data.secondaryValue);
        }

        if (_currentHealth <= 0f)
        {
            if (_deathImmune)
            {
                _currentHealth = 1f;
                return;
            }

            Die();
        }
    }

    public float GetHealthPercent()
    {
        if (_data == null || !_data.@class)
            return 1f;

        return _currentHealth / _data.MaxHealth;
    }

    private void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, _data.MaxHealth);
    }

    private void Die()
    {
        if (_animator)
            _animator.SetTrigger(DieHash);

        if (CompareTag(PlayerTag))
            GameSession.Instance?.HandlePlayerDeath();
        
        Destroy(gameObject, 2f);
    }
    
    #endregion
    
    #region Damage Calculation

    public float CalculateDamage(SkillData skill, Transform target)
    {
        var baseDamage = _data.Damage * skill.damageMultiplier;

        var final = baseDamage * _damageMultiplier;

        if (HasEffect(StatusEffectType.DistanceDamageBonus))
        {
            var dist = Vector3.Distance(transform.position, target.position);
            final *= 1f + dist * 0.05f;
        }

        if (HasEffect(StatusEffectType.BackstabBonus) && IsBehindTarget(target))
        {
            if (target.TryGetComponent<Enemy>(out var enemy) && enemy.HasCc)
                final *= 1.5f;
        }

        return final;
    }

    private bool IsBehindTarget(Transform target)
    {
        var dirToMe = (transform.position - target.position).normalized;
        return Vector3.Dot(transform.forward, dirToMe) > 0.5f;
    }

    private bool HasEffect(StatusEffectType type)
    {
        return _effects.Exists(e => e.Data.type == type);
    }
    
    #endregion
    
    #region Movement Helpers
    
    public float GetMoveSpeedMultiplier() => _moveSpeedMultiplier;
    public bool IsRooted() => _isRooted;
    public bool IsUntargetable() => _isUntargetable;
    public Transform GetForcedTarget() => _forcedTarget;
    
    #endregion
    
    #region Knockback
    
    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (!controller)
            return;
        
        controller.Move(direction * force);
    }
    
    #endregion
    
    #region Execution

    public void Execute(SkillData skill, CharacterSelectionData data, Transform target)
    {
        if(skill.castVFX)
            Instantiate(skill.castVFX, castPoint.position, Quaternion.identity);

        switch (skill.targetType)
        {
            case SkillTargetType.SingleTarget:
                ExecuteSingleTarget(skill, target);
                break;
            case SkillTargetType.Area:
                ExecuteAoe(skill);
                break;
            case SkillTargetType.Line:
                ExecuteLine(skill, target);
                break;
            case SkillTargetType.Chain:
                ExecuteChain(skill);
                break;
        }
    }

    private void ExecuteSingleTarget(SkillData skill, Transform target)
    {
        if (!target)
            return;
        
        var dir = GetDirection(target);
        
        if (Physics.Raycast(castPoint.position, dir, out var hit, skill.range, targetLayer))
        {
            if (hit.collider.TryGetComponent<Enemy>(out var enemy))
            {
                ApplyHit(skill, enemy, hit.point, hit.collider.transform);
            }
        }
    }

    private void ExecuteAoe(SkillData skill)
    {
        var hitCount = Physics.OverlapSphereNonAlloc(
            castPoint.position,
            skill.radius,
            _overlapBuffer,
            targetLayer
        );

        for (var i = 0; i < hitCount; i++)
        {
            if (_overlapBuffer[i].TryGetComponent<Enemy>(out var enemy))
            {
                ApplyHit(skill, enemy, _overlapBuffer[i].transform.position, _overlapBuffer[i].transform);
            }
        }
    }

    private void ExecuteLine(SkillData skill, Transform target)
    {
        if (!target)
            return;
        
        var dir = GetDirection(target);
        
        var hitCount = Physics.SphereCastNonAlloc(
            castPoint.position,
            0.5f,
            dir,
            _castBuffer,
            skill.range,
            targetLayer
        );
        
        for (var i = 0; i < hitCount; i++)
        {
            if (_castBuffer[i].transform.TryGetComponent<Enemy>(out var enemy))
            {
                ApplyHit(skill, enemy, _castBuffer[i].transform.position, _castBuffer[i].transform);
                enemy.ApplyKnockback(transform.forward, 5f);
            }
        }
    }

    private void ExecuteChain(SkillData skill)
    {
        var visited = new HashSet<Transform>();
        
        var first = FindClosestTarget(castPoint.position, skill.range);
        if (!first)
            return;
        
        var current = first;

        for (var i = 0; i < skill.maxTargets; i++)
        {
            if (!current || !visited.Add(current))
                break;

            if (current.TryGetComponent<Enemy>(out var target))
                ApplyHit(skill, target, current.position, current);
            
            current = FindClosestFrom(current.position, skill.radius, visited);
        }
    }

    private void ApplyHit(SkillData skill, IDamageable target, Vector3 hitPoint, Transform targetTransform)
    {
        if (!targetTransform)
            return;
        
        var damage = CalculateDamage(skill, targetTransform);
        
        target.TakeDamage(damage);
        
        if (skill.hitVFX)
            Instantiate(skill.hitVFX, hitPoint, Quaternion.identity);

        if (skill.effects == null || skill.effects.Length == 0)
            return;

        foreach (var effect in skill.effects)
        {
            if (!effect)
                continue;
            
            switch (target)
            {
                case SkillExecuter exec:
                    exec.ApplyEffect(effect);
                    break;
                case Enemy enemy:
                    enemy.ApplyEffect(effect, transform);
                    break;
            }
        }
    }

    private Transform FindClosestTarget(Vector3 position, float range)
    {
        var count = Physics.OverlapSphereNonAlloc(
            position,
            range,
            _overlapBuffer,
            targetLayer
        );

        Transform best = null;
        var bestDist = float.MaxValue;

        for (var i = 0; i < count; i++)
        {
            var col = _overlapBuffer[i];
            if (!col)
                continue;
            
            var dist = (col.transform.position - position).sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                best = col.transform;
            }
        }
        
        return best;
    }

    private Transform FindClosestFrom(Vector3 position, float range, HashSet<Transform> visited)
    {
        var count = Physics.OverlapSphereNonAlloc(
            position,
            range,
            _overlapBuffer,
            targetLayer
        );
        
        Transform best = null;
        var bestDist = float.MaxValue;
        
        for (var i = 0; i < count; i++)
        {
            var col = _overlapBuffer[i];
            if (!col)
                continue;

            var t = col.transform;

            if (visited.Contains(t))
                continue;

            var dist = (t.position - position).sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                best = t;
            }
        }

        return best;
    }

    private Vector3 GetDirection(Transform target)
    {
        return target ? (target.position - transform.position).normalized : transform.forward;
    }
    
    #endregion
}