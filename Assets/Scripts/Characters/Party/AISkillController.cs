using UnityEngine;

public class AISkillController : MonoBehaviour
{
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    
    [SerializeField] private SkillExecuter skillExecuter;
    
    [Header("Performance")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int maxHits = 32;
    
    private Animator _animator;

    private CharacterSelectionData _data;
    private SkillRuntime[] _skills;
    
    private Collider[] _overlapBuffer;

    private void Awake()
    {
        _overlapBuffer = new Collider[maxHits];
    }

    public void Initialize(CharacterSelectionData data, Animator animator)
    {
        _data = data;
        _animator = animator;

        var skills = data.@class.startingSkills;

        _skills = new SkillRuntime[skills.Length];

        for (var i = 0; i < skills.Length; i++)
        {
            _skills[i] = new SkillRuntime
            {
                Data = skills[i],
            };
        }
    }

    private void Update()
    {
        TickCooldowns();
    }

    public void TryUseBestSkill(Transform target)
    {
        if (_skills == null || _skills.Length == 0)
            return;

        SkillRuntime bestSkill = null;
        var bestScore = float.MinValue;

        foreach (var skill in _skills)
        {
            if (!skill.IsReady)
                continue;

            if (_data.currentMana < skill.Data.manaCost)
                continue;

            var score = EvaluateSkill(skill, target);

            if (score > bestScore)
            {
                bestScore = score;
                bestSkill = skill;
            }
        }

        if (bestSkill != null)
            UseSkill(bestSkill);
    }

    private float EvaluateSkill(SkillRuntime skill, Transform target)
    {
        var distance = Vector3.Distance(transform.position, target.position);

        var score = 0f;

        if (skill.Data.targetType == SkillTargetType.SingleTarget)
        {
            if (distance > skill.Data.range)
                return -100f; // invalid
            score += 10f;
        }

        if (skill.Data.targetType == SkillTargetType.Area)
        {
            var hits = CountEnemiesInRadius(skill.Data.range);
            score += hits * 5f; // prefer AoE when clustered
        }
        
        // Prefer higher damage
        score += skill.Data.damageMultiplier * 2f;
        
        // Prefer cheaper skills slightly
        score -= skill.Data.manaCost * 0.1f;

        return score;
    }

    private int CountEnemiesInRadius(float radius)
    {
        var count = Physics.OverlapSphereNonAlloc(
            transform.position,
            radius,
            _overlapBuffer,
            targetLayer
        );
        
        return count;
    }

    private void UseSkill(SkillRuntime skill)
    {
        _data.SetMana(_data.currentMana - skill.Data.manaCost);
        
        skill.Trigger();
        
        skillExecuter.Execute(skill.Data, _data);

        if (_animator)
            _animator.SetTrigger(AttackHash);
    }

    private void TickCooldowns()
    {
        foreach (var skill in _skills)
            skill.Tick(skillExecuter.AttackSpeedMultiplier * Time.deltaTime);
    }
}