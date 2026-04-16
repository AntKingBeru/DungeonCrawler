using UnityEngine;
using System.Collections;

public class RewardRoom : Room
{
    [Header("Rewards")]
    [SerializeField] private Transform[] chestSpawnPoints;
    [SerializeField] private RewardChest chestPrefab;

    protected override IEnumerator EncounterRoutine()
    {
        CloseAllDoors();
        
        yield return new WaitForSecondsRealtime(0.2f);

        SpawnRewards();
        
        yield return new WaitForSecondsRealtime(0.5f);
        
        OpenAllDoors();
    }

    private void SpawnRewards()
    {
        var player = GameSession.Instance.Player;

        var count = Random.Range(2, 5);
        
        var rewards = RewardGenerator.GenerateRewards(player, count);

        for (var i = 0; i < rewards.Count && i < chestSpawnPoints.Length; i++)
        {
            var chest = Instantiate(chestPrefab, chestSpawnPoints[i]);
            chest.Initialize(rewards[i]);
        }
    }
}
