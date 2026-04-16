using UnityEngine;
using System.Collections.Generic;

public static class RewardGenerator
{
    public static List<RewardData> GenerateRewards(CharacterSelectionData player, int count)
    {
        var rewards = new List<RewardData>();

        var availableSkillIndices = GetAvailableSkillUnlocks(player);

        for (var i = 0; i < count; i++)
        {
            if (availableSkillIndices.Count > 0 && Random.value > 0.4f)
            {
                var index = availableSkillIndices[Random.Range(0, availableSkillIndices.Count)];
                availableSkillIndices.Remove(index);

                rewards.Add(CreateSkillReward(index));
            }
            else
                rewards.Add(CreateStatReward());
        }
        
        return rewards;
    }

    private static List<int> GetAvailableSkillUnlocks(CharacterSelectionData player)
    {
        List<int> list = new();
        var unlocks = player.@class.unlockableSkills;
        
        for (var i = 0; i < unlocks.Length; i++)
        {
            if (!player.unlockedSkills.Contains(unlocks[i]))
                list.Add(i);
        }
        
        return list;
    }

    private static RewardData CreateSkillReward(int index)
    {
        var data = ScriptableObject.CreateInstance<RewardData>();
        
        data.type = RewardType.UnlockSkill;
        data.skillIndex = index;
        
        return data;
    }

    private static RewardData CreateStatReward()
    {
        var data = ScriptableObject.CreateInstance<RewardData>();

        data.type = RewardType.StatBoost;
        data.healthBonus = Random.Range(10f, 25f);
        data.manaBonus = Random.Range(5f, 15f);
        data.damageBonus = Random.Range(2f, 6f);
        
        return data;
    }
}