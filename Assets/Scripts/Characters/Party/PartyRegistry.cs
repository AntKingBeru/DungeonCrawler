using System.Collections.Generic;

public static class PartyRegistry
{
    private static readonly List<SkillExecuter> Members = new();
    
    public static IReadOnlyList<SkillExecuter> Registered => Members;
    
    public static void Register(SkillExecuter member)
    {
        if (!Members.Contains(member))
            Members.Add(member);
    }
    
    public static void Unregister(SkillExecuter member)
    {
        Members.Remove(member);
    }
}