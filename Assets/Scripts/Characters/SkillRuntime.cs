using UnityEngine;

public class SkillRuntime
{
    public SkillData Data;

    private float _cooldownRemaining;
    
    public bool IsReady => _cooldownRemaining <= 0f;
    public float CooldownRemaining => _cooldownRemaining;

    public float CooldownNormalized => Data.cooldown <= 0f ? 0f : Mathf.Clamp01(_cooldownRemaining / Data.cooldown);

    public void Tick(float deltaTime)
    {
        if (_cooldownRemaining <= 0f)
            return;

        _cooldownRemaining -= deltaTime;

        if (_cooldownRemaining < 0f)
            _cooldownRemaining = 0f;
    }

    public void Trigger()
    {
        _cooldownRemaining = Data.cooldown;
    }

    public void ResetCooldown()
    {
        _cooldownRemaining = 0f;
    }
    
    public void SetCooldown(float value)
    {
        _cooldownRemaining = Mathf.Max(0f, value);
    }
}