using UnityEngine;

[CreateAssetMenu(menuName = "Game/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;
    
    public SkillType type;
    public SkillTargetType targetType;

    public float cooldown;
    public float manaCost;

    public float damageMultiplier;
    public float range;
    public float radius;
    public float maxTargets;

    public GameObject castVFX;
    public GameObject hitVFX;
}