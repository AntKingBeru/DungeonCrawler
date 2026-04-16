using UnityEngine;

[CreateAssetMenu(menuName = "Game/Skill/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public StatusEffectType type;

    public float duration;
    public bool isPermanent;
    
    public float value;
    public float tickInterval = 1f; // for DoT, HoT
    public float tickValue;

    public bool affectsMovement;
    public bool affectsDamage;
    public bool affectsHealing;

    public bool isStackable;
    public int maxStacks = 1;

    public bool scaleWithNearbyEnemies;
    public float radius;

    public bool applyToCaster;
    public bool applyToTarget;

    public float secondaryValue; // used for dual effects, like reflect % damage, damage boost
}