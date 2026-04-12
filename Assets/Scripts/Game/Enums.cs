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

public enum RoomType
{
    Start,
    Normal,
    Boss,
    Reward
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