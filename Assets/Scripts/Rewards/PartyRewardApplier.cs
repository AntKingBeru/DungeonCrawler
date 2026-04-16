public static class PartyRewardApplier
{
    public static void ApplyReward(RewardData reward)
    {
        ApplyToCharacter(GameSession.Instance.Player, reward);
        
        foreach (var member in GameSession.Instance.Party)
            ApplyToCharacter(member, reward);

        SkillSystemEvents.RaiseSkillsUpdated();
        PartySystemEvents.RaiseStatsUpdated();
    }

    private static void ApplyToCharacter(CharacterSelectionData character, RewardData reward)
    {
        switch (reward.type)
        {
            case RewardType.UnlockSkill:
                UnlockSkill(character, reward.skillIndex);
                break;
            case RewardType.StatBoost:
                ApplyStatBoost(character, reward);
                break;
        }
    }

    private static void UnlockSkill(CharacterSelectionData character, int index)
    {
        var unlocks = character.@class.unlockableSkills;
        
        if (index < 0 || index >= unlocks.Length)
            return;
        
        var skill = unlocks[index];
        
        if (!character.unlockedSkills.Contains(skill))
            character.unlockedSkills.Add(skill);
    }

    private static void ApplyStatBoost(CharacterSelectionData character, RewardData reward)
    {
        character.@class.maxHealth += reward.healthBonus;
        character.@class.maxMana += reward.manaBonus;
        character.@class.damage += reward.damageBonus;
        
        character.SetHealth(character.@class.maxHealth);
        character.SetMana(character.@class.maxMana);
    }
}