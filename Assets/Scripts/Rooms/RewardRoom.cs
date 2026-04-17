using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardRoom : Room
{
    [Header("Rewards")]
    [SerializeField] private Transform[] chestSpawnPoints;
    [SerializeField] private RewardChest chestPrefab;
    
    private readonly List<RewardChest> _spawnedChests = new();

    private bool _hasSaved;
    private int _openedCount;

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
        
        _spawnedChests.Clear();
        _openedCount = 0;
        _hasSaved = false;

        for (var i = 0; i < rewards.Count && i < chestSpawnPoints.Length; i++)
        {
            var chest = Instantiate(chestPrefab, chestSpawnPoints[i]);
            chest.Initialize(rewards[i]);

            chest.OnOpened += HandleChestOpened;
            
            _spawnedChests.Add(chest);
        }
    }

    private void HandleChestOpened(RewardChest chest)
    {
        _openedCount++;

        if (!_hasSaved && _openedCount == _spawnedChests.Count)
        {
            AutoSave();
            _hasSaved = true;
        }
    }

    private void AutoSave()
    {
        if (GameSession.Instance.CurrentSlot < 0)
            return;

        var saveData = GameSession.Instance.CreateSaveData();
        SaveSystem.Save(GameSession.Instance.CurrentSlot, saveData);
    }
}