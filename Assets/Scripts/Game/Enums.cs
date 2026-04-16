public enum EnemyAttackType
{
    Melee,
    Ranged,
    AoE
}

public enum EnemyState
{
    Spawning,
    Idle,
    Chasing,
    Dead
}

public enum EnemyType
{
    Normal,
    MiniBoss,
    Boss
}

public enum FormationType
{
    Diamond,
    Square,
    HorizontalLine,
    Triangle,
    FollowLeader
}

public enum RewardType
{
    UnlockSkill,
    StatBoost
}

public enum RoomType
{
    Start,
    Normal,
    Boss,
    Reward,
    Victory
}

public enum SceneId
{
    MainMenu,
    CharacterCreationPlayer,
    CharacterCreationParty,
    GameScene
}

public enum SkillTargetType
{
    SingleTarget,
    Area,
    Line,
    Chain
}

public enum SkillType
{
    Active,
    Passive
}

public enum StatusEffectType
{
    // Buffs
    AttackSpeed,
    MoveSpeed,
    DamageBoost,
    DamageReduction,
    HealingBoost,

    // Over Time
    DamageOverTime,
    HealOverTime,

    // Crowd Control
    Root,
    Slow,
    Stun,
    Freeze,

    // Defensive
    DeathImmunity,
    Reflect,
    Untargetable,
    Block,

    // Aggro
    Taunt,

    // Conditional
    BackstabBonus,
    BonusVsCc,
    DistanceDamageBonus,
    TargetMarked,

    // Utility
    TeleportBehind,
    Trap
}