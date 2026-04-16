using System;

public static class SkillSystemEvents
{
    public static event Action OnSkillsUpdated;
    
    public static void RaiseSkillsUpdated() => OnSkillsUpdated?.Invoke();
}