using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int roomsPerBoss = 3;
    [SerializeField] private int bossCount = 3;
    [SerializeField] private Vector3 worldOffset;

    private Dictionary<Vector2Int, DungeonRoomNode> _rooms;
    
    public Dictionary<Vector2Int, DungeonRoomNode> Rooms => _rooms;

    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public Dictionary<Vector2Int, DungeonRoomNode> Generate()
    {
        _rooms = new Dictionary<Vector2Int, DungeonRoomNode>();
        
        var currentPos = Vector2Int.zero;
        
        // Start room
        _rooms[currentPos] = new DungeonRoomNode(currentPos, RoomType.Start);

        for (var bossIndex = 0; bossIndex < bossCount; bossIndex++)
        {
            
            // Generate normal rooms
            for (var i = 0; i < roomsPerBoss; i++)
            {
                currentPos = GetNextPosition(currentPos);
                _rooms[currentPos] = new DungeonRoomNode(currentPos, RoomType.Normal);
            }
            
            // Boss room
            currentPos = GetNextPosition(currentPos);
            _rooms[currentPos] = new DungeonRoomNode(currentPos, RoomType.Boss);

            if (bossIndex < bossCount - 1)
            {
                // Reward room
                currentPos = GetNextPosition(currentPos);
                _rooms[currentPos] = new DungeonRoomNode(currentPos, RoomType.Reward);
            }
            else
            {
                currentPos = GetNextPosition(currentPos);
                _rooms[currentPos] = new DungeonRoomNode(currentPos, RoomType.Victory);
            }
        }
        
        return _rooms;
    }

    private Vector2Int GetNextPosition(Vector2Int from)
    {
        var shuffled = new List<Vector2Int>(Directions);
        
        // Shuffle directions
        for (var i = 0; i < shuffled.Count; i++)
        {
            var rand = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
        }

        foreach (var newPos in shuffled.Select(dir => from + dir)
                     .Where(newPos => !_rooms.ContainsKey(newPos)))
        {
            return newPos;
        }
        
        return from + Directions[Random.Range(0, Directions.Length)];
    }

    public Vector3 GetWorldPosition(Vector2Int pos)
    {
        return new Vector3(
                pos.x * DungeonConstants.RoomSize,
                0,
                pos.y * DungeonConstants.RoomSize
        ) + worldOffset;
    }
}