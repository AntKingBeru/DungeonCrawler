using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class DungeonSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DungeonGenerator generator;
    [SerializeField] private RoomDatabase database;
    [SerializeField] private Transform dungeonRoot;
    [SerializeField] private NavMeshSurface navMesh;

    private readonly Dictionary<Vector2Int, Room> _spawnedRooms = new();

    private void Start()
    {
        navMesh.RemoveData();
        navMesh.BuildNavMesh();
    }
    
    public void SpawnDungeon()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        foreach (var room in _spawnedRooms.Values)
            Destroy(room.gameObject);
        
        _spawnedRooms.Clear();
        
        var rooms = generator.Generate();
        
        foreach (var kvp in rooms)
            SpawnRoom(kvp.Value, rooms);

        yield return null;
        yield return null;
        
        navMesh.UpdateNavMesh(navMesh.navMeshData);
    }

    private void SpawnRoom(DungeonRoomNode node, Dictionary<Vector2Int, DungeonRoomNode> allRooms)
    {
        var prefab = database.GetRoom(node.Type);
        
        var worldPosition = generator.GetWorldPosition(node.Position);

        var roomInstance = Instantiate(
            prefab,
            worldPosition,
            Quaternion.identity,
            dungeonRoot
        );
        
        SetupDoors(roomInstance, node.Position, allRooms);
        
        _spawnedRooms[node.Position] = roomInstance;
    }

    private void SetupDoors(Room room, Vector2Int pos, Dictionary<Vector2Int, DungeonRoomNode> allRooms)
    {
        var up = allRooms.ContainsKey(pos + Vector2Int.up);
        var down = allRooms.ContainsKey(pos + Vector2Int.down);
        var left = allRooms.ContainsKey(pos + Vector2Int.left);
        var right = allRooms.ContainsKey(pos + Vector2Int.right);
        
        room.Setup(up, down, left, right);
    }
}