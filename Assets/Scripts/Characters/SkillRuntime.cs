public class SkillRuntime
{
    public SkillData Data;

    private float _cooldownTimer;
    
    public bool IsReady => _cooldownTimer <= 0f;
    
    public void Tick(float deltaTime)
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= deltaTime;
    }

    public void Trigger()
    {
        _cooldownTimer = Data.cooldown;
    }
}