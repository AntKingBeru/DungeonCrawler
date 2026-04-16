using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    #region Data
    
    [Header("Data")]
    [SerializeField] private EnemyDatabase database;
    
    #endregion
    
    #region Runtime
    
    private readonly List<Enemy> _spawned = new();
    private int _aliveEnemies;

    public int AliveEnemies => _aliveEnemies;
    public IReadOnlyList<Enemy> Spawned => _spawned;
    
    #endregion
    
    #region Public API

    public void Clear()
    {
        foreach (var enemy in _spawned.Where(e => e))
        {
            EnemyRegistry.Unregister(enemy);
            Destroy(enemy.gameObject);
        }

        ResetState();
    }
    
    public void SpawnNormalRoom(Transform[] spawnPoints)
    {
        ResetState();
        
        var count = Random.Range(2, 5);

        for (var i = 0; i < count; i++)
        {
            var data = database.GetRandomNormal();
            Spawn(data, spawnPoints[i % spawnPoints.Length]);
        }
    }
    
    public void SpawnMiniBossRoom(Transform[] spawnPoints, int bossIndex)
    {
        ResetState();
        
        Spawn(database.GetRandomMiniBoss(), spawnPoints[0]);
        
        var count = bossIndex == 0 ? 2 : 3;
        
        for (var i = 1; i <= count; i++)
            Spawn(database.GetRandomNormal(), spawnPoints[i % spawnPoints.Length]);
    }
    
    public void SpawnBossRoom(Transform[] spawnPoints)
    {
        ResetState();
        
        Spawn(database.necromancer, spawnPoints[0]);
    }
    
    #endregion
    
    #region Flow Control
    
    public IEnumerator WaitForAllSpawned()
    {
        var finished = 0;

        foreach (var enemy in _spawned.Where(e => e))
        {
            void OnSpawn(Enemy e)
            {
                finished++;
                e.onSpawnFinished.RemoveListener(OnSpawn);
            }

            enemy.onSpawnFinished.AddListener(OnSpawn);
        }

        while (finished < _spawned.Count)
            yield return null;
    }

    public IEnumerator WaitForAllDead()
    {
        while (_aliveEnemies > 0)
            yield return null;
    }
    
    #endregion
    
    #region Internal

    private void ResetState()
    {
        _spawned.Clear();
        _aliveEnemies = 0;
    }

    private void Spawn(EnemyData data, Transform point)
    {
        var enemy = Instantiate(
            data.enemyPrefab,
            point.position,
            Quaternion.identity
        );

        var player = GameInitializer.PlayerInstance.transform;
        enemy.Initialize(data, player);
        
        _spawned.Add(enemy);
        _aliveEnemies++;

        EnemyRegistry.Register(enemy);
        
        enemy.onDeath.AddListener(OnEnemyDeath);
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        _aliveEnemies--;
        
        EnemyRegistry.Unregister(enemy);
        
        enemy.onDeath.RemoveListener(OnEnemyDeath);
    }
    
    #endregion
}