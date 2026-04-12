using UnityEngine;

public class DungeonRoomNode
{
    public Vector2Int Position;
    public RoomType Type;

    public bool Visited;

    public DungeonRoomNode(Vector2Int pos, RoomType type)
    {
        Position = pos;
        Type = type;
        Visited = false;
    }
}