using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class AISkillController : MonoBehaviour
{
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    
    [SerializeField] private SkillExecuter skillExecuter;
    
    private Animator _animator;

    private CharacterSelectionData _data;
    private SkillRuntime[] _activeSkills;
    private List<SkillData> _passiveSkills;

    public void Initialize(CharacterSelectionData data, Animator animator)
    {
        _data = data;
        _animator = animator;

        BuildSkills();
    }

    private void OnEnable()
    {
        SkillSystemEvents.OnSkillsUpdated += RebuildSkills;
    }

    private void OnDisable()
    {
        SkillSystemEvents.OnSkillsUpdated -= RebuildSkills;
    }

    private void Update()
    {
        TickCooldowns();
    }

    private void RebuildSkills()
    {
        BuildSkills();
        ApplyPassives();
    }

    private void BuildSkills()
    {
        var unique = new HashSet<SkillData>();
        
        foreach (var skill in _data.@class.startingSkills)
            unique.Add(skill);

        if (_data.unlockedSkills != null)
        {
            foreach (var skill in _data.unlockedSkills)
                unique.Add(skill);
        }
        
        var active = new List<SkillData>();
        var passive = new List<SkillData>();
        
        foreach (var skill in unique)
        {
            if (skill.type == SkillType.Active)
                active.Add(skill);
            else
                passive.Add(skill);
        }

        active.Sort((a, b) => GetPriority(b).CompareTo(GetPriority(a)));

        _activeSkills = new SkillRuntime[active.Count];

        for (var i = 0; i < active.Count; i++)
        {
            _activeSkills[i] = new SkillRuntime
            {
                Data = active[i],
            };
        }

        _passiveSkills = passive;
    }

    private void ApplyPassives()
    {
        if (!skillExecuter)
            return;

        foreach (var effect in from passive in _passiveSkills from effect in passive.effects where effect.applyToCaster select effect)
        {
            skillExecuter.ApplyEffect(effect, transform);
        }
    }

    private int GetPriority(SkillData skill)
    {
        return skill.targetType switch
        {
            SkillTargetType.Area => 3,
            SkillTargetType.Chain => 3,
            SkillTargetType.Line => 2,
            SkillTargetType.SingleTarget => 1,
            _ => 0
        };
    }

    public void TryUseBestSkill(Transform target)
    {
        if (_activeSkills == null || _activeSkills.Length == 0)
            return;

        SkillRuntime bestSkill = null;
        var bestScore = 0f;

        foreach (var skill in _activeSkills)
        {
            if (!skill.IsReady)
                continue;

            if (_data.currentMana < skill.Data.manaCost)
                continue;

            var score = ScoreSkill(skill, target);

            if (score > bestScore)
            {
                bestScore = score;
                bestSkill = skill;
            }
        }

        if (bestSkill != null)
            UseSkill(bestSkill, target);
    }

    private float ScoreSkill(SkillRuntime skill, Transform target)
    {
        var data = skill.Data;
        
        var score = 0f;

        var distance = Vector3.Distance(transform.position, target.position);

        if (distance > data.range)
            return 0f;

        if (data.targetType == SkillTargetType.Area || data.targetType == SkillTargetType.Chain)
        {
            var nearbyEnemies = CountEnemiesAround(target.position, data.range);
            score += nearbyEnemies * 2f;
        }

        if (data.targetType == SkillTargetType.SingleTarget)
        {
            var nearbyEnemies = CountEnemiesAround(target.position, 3f);
            if (nearbyEnemies <= 1)
                score += 2f;
        }

        if (skillExecuter)
        {
            var healthPercent = skillExecuter.GetHealthPercent();
            
            if (HasEffectType(data, StatusEffectType.HealOverTime) ||
                HasEffectType(data, StatusEffectType.DamageReduction))
                score += (1f - healthPercent) * 5f;

            score += healthPercent * 2f;
        }
        
        var manaRatio = _data.currentMana / _data.@class.maxMana;
        score *= Mathf.Lerp(0.5f, 1.5f, manaRatio);
        
        score += 1f / (1f + data.cooldown);

        return score;
    }

    private int CountEnemiesAround(Vector3 position, float radius)
    {
        return EnemyRegistry.RegisteredEnemies.Where(enemy => enemy)
            .Count(enemy => Vector3.Distance(position, enemy.transform.position) <= radius);
    }

    private bool HasEffectType(SkillData skill, StatusEffectType type)
    {
        return skill.effects.Any(effect => effect.type == type);
    }

    private void UseSkill(SkillRuntime skill, Transform target)
    {
        _data.SetMana(_data.currentMana - skill.Data.manaCost);
        
        skill.Trigger();
        
        skillExecuter.Execute(skill.Data, _data, target);
        
        if (_animator)
            _animator.SetTrigger(AttackHash);
    }

    private void TickCooldowns()
    {
        foreach (var skill in _activeSkills)
            skill.Tick(skillExecuter.AttackSpeedMultiplier * Time.deltaTime);
    }
}