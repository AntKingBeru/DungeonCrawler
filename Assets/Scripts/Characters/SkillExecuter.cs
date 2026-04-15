using UnityEngine;

public class SkillExecuter : MonoBehaviour
{
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

    public void Execute(SkillData skill, CharacterSelectionData data)
    {
        if (skill.castVFX)
            Instantiate(skill.castVFX, castPoint.position, Quaternion.identity);

        switch (skill.targetType)
        {
            case SkillTargetType.SingleTarget:
                ExecuteSingleTarget(skill, data);
                break;
            case SkillTargetType.Area:
                ExecuteAoe(skill, data);
                break;
        }
    }
    
    private void ExecuteSingleTarget(SkillData skill, CharacterSelectionData data)
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
            if (_castBuffer[i].collider.TryGetComponent<IDamageable>(out var damageable))
            {
                DealDamage(skill, data, damageable, _castBuffer[i].point);
                return;
            }
        }
    }
    
    private void ExecuteAoe(SkillData skill, CharacterSelectionData data)
    {
        var hitCount = Physics.OverlapSphereNonAlloc(
            castPoint.position,
            skill.radius,
            _overlapBuffer,
            targetLayer
        );

        for (var i = 0; i < hitCount; i++)
        {
            if (_overlapBuffer[i].TryGetComponent<IDamageable>(out var damageable))
                DealDamage(skill, data, damageable, _overlapBuffer[i].transform.position);
        }
    }

    private void DealDamage(SkillData skill, CharacterSelectionData data, IDamageable target, Vector3 hitPoint)
    {
        var damage = data.@class.damage * skill.damageMultiplier;
        
        target.TakeDamage(damage);
        
        if (skill.hitVFX)
            Instantiate(skill.hitVFX, hitPoint, Quaternion.identity);
    }
}