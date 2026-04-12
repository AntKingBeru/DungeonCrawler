using UnityEngine;

[CreateAssetMenu(menuName = "Game/Character Class Data")]
public class CharacterClassData : ScriptableObject
{
    [Header("Info")]
    public string className;
    [TextArea] public string description;
    public Sprite icon;
    
    [Header("Stats")]
    public float maxHealth;
    public float maxMana;
    public float damage;
    public float movementSpeed;
    
    [Header("Visuals")]
    public CharacterModelView modelPrefab;
    public Material[] colorVariants;
    
    [Header("Skills")]
    public SkillData[] startingSkills;
    public SkillData[] unlockableSkills;
}