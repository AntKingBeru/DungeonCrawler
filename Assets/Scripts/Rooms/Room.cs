using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
    [Header("Doors")]
    [SerializeField] private DoorPair up;
    [SerializeField] private DoorPair down;
    [SerializeField] private DoorPair left;
    [SerializeField] private DoorPair right;
    
    [Header("Encounter")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private Transform[] spawnPoints;
    
    [Header("Room Type")]
    [SerializeField] private bool isStartRoom;
    [SerializeField] private bool isBossRoom;
    [SerializeField] private int bossIndex; // 0, 1 for mini bosses
    
    private bool _isActive;
    private bool _encounterCompleted;
    
    #region Setup

    public void Setup(bool hasUp, bool hasDown, bool hasLeft, bool hasRight)
    {
        up.SetConnection(hasUp);
        down.SetConnection(hasDown);
        left.SetConnection(hasLeft);
        right.SetConnection(hasRight);
        
        OpenAllDoors(); // default state
    }
    
    #endregion
    
    #region Entry Point

    public void ActivateRoom()
    {
        if (_isActive)
            return;

        _isActive = true;

        if (isStartRoom)
            return;
        
        StartCoroutine(EncounterRoutine());
    }
    
    #endregion
    
    #region Encounter Flow

    private IEnumerator EncounterRoutine()
    {
        CloseAllDoors();
        
        yield return new WaitForSecondsRealtime(0.2f);

        SpawnEnemies();

        yield return spawner.WaitForAllSpawned();

        yield return spawner.WaitForAllDead();
        
        _encounterCompleted = true;
        
        OpenAllDoors();
    }

    private void SpawnEnemies()
    {
        if (isBossRoom)
        {
            if (bossIndex < 2)
                spawner.SpawnMiniBossRoom(spawnPoints, bossIndex);
            else
                spawner.SpawnBossRoom(spawnPoints);
        }
        else
            spawner.SpawnNormalRoom(spawnPoints);
    }
    
    #endregion
    
    #region Doors

    public void OpenAllDoors()
    {
        up.Open();
        down.Open();
        left.Open();
        right.Open();
    }

    public void CloseAllDoors()
    {
        up.Close();
        down.Close();
        left.Close();
        right.Close();
    }
    
    #endregion
}