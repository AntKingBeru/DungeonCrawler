using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SkillController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference[] skillActions;
    
    [Header("Setup")]
    [SerializeField] private Transform castPoint;
    [SerializeField] private LayerMask targetLayer;
    
    [Header("Performance")]
    [SerializeField] private int maxHits = 32;
    
    [Header("Runtime")]
    private SkillRuntime[] _skills;
    private CharacterSelectionData _data;
    
    [Header("Animation")]
    private Animator _animator;
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private Collider[] _overlapBuffer;
    private RaycastHit[] _castBuffer;
    
    public UnityEvent<SkillRuntime> onSkillUsed;

    public void Initialize(CharacterSelectionData data, Animator animator = null)
    {
        _data = data;
        
        _animator = animator;
        
        var skillCount = _data.@class.startingSkills.Length;

        _skills = new SkillRuntime[skillCount];

        for (var i = 0; i < skillCount; i++)
        {
            _skills[i] = new SkillRuntime
            {
                Data = _data.@class.startingSkills[i],
            };
        }
        
        _overlapBuffer = new Collider[maxHits];
        _castBuffer = new RaycastHit[maxHits];
    }

    private void OnEnable()
    {
        foreach (var skillAction in skillActions)
            skillAction.action.Enable();
    }
    
    private void OnDisable()
    {
        foreach (var skillAction in skillActions)
            skillAction.action.Disable();
    }

    private void Update()
    {
        TickCooldowns();
        HandleInput();
    }
    
    private void TickCooldowns()
    {
        foreach (var skill in _skills)
            skill.Tick(Time.deltaTime);
    }

    private void HandleInput()
    {
        for (var i = 0; i < _skills.Length; i++)
        {
            if (i >= skillActions.Length)
                continue;

            if (skillActions[i].action.triggered)
                TryUseSkill(i);
        }
    }

    private void TryUseSkill(int index)
    {
        var skill = _skills[index];

        if (!skill.IsReady)
            return;

        if (_data.currentMana < skill.Data.manaCost)
            return;
        
        UseSkill(skill);
    }
    
    private void UseSkill(SkillRuntime skill)
    {
        _data.currentMana -= skill.Data.manaCost;
        skill.Trigger();
        ExecuteSkill(skill.Data);
        
        if (_animator)
            _animator.SetTrigger(AttackHash);
        
        onSkillUsed?.Invoke(skill);
    }

    private void ExecuteSkill(SkillData skill)
    {
        if (skill.castVFX)
            Instantiate(skill.castVFX, castPoint.position, Quaternion.identity);

        switch (skill.targetType)
        {
            case SkillTargetType.SingleTarget:
                ExecuteSingleTarget(skill);
                break;

            case SkillTargetType.Area:
                ExecuteAoe(skill);
                break;
        }
    }

    private void ExecuteSingleTarget(SkillData skill)
    {
        var hitCount = Physics.SphereCastNonAlloc(
            castPoint.position,
            0.5f,
            transform.forward,
            _castBuffer,
            skill.range,
            targetLayer
        );

        for (var i = 0; i < hitCount; i++)
        {
            var hit = _castBuffer[i];
            
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                DealDamage(skill, damageable, hit.collider.transform.position);
                return;
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
            var col = _overlapBuffer[i];
            
            if (col.TryGetComponent<IDamageable>(out var damageable))
                DealDamage(skill, damageable, col.transform.position);
        }
    }

    private void DealDamage(SkillData skill, IDamageable target, Vector3 hitPoint)
    {
        var damage = _data.@class.damage * skill.damageMultiplier;
        
        target.TakeDamage(damage);
        
        if (skill.hitVFX)
            Instantiate(skill.hitVFX, hitPoint, Quaternion.identity);
    }
}